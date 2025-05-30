
using Kursach.Database;
using Kursach.Database.WarehouseApp.Database;
using Kursach.main_windows.admin.admin_product;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.main_windows.admin
{
    /// <summary>
    /// Логика взаимодействия для ProductManagementWindow.xaml
    /// </summary>
    public partial class ProductManagementWindow : Window
    {
        private bool isInitialized = false;
        private DataTable originalTable;
        private readonly string adminUsername; // сделаем readonly, чтобы случайно не переписать

        public ProductManagementWindow(string username)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("В конструктор ProductManagementWindow передано пустое имя администратора!", "Ошибка");
                this.Close();
                return;
            }

            this.adminUsername = username;

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;

            LoadProducts(); // Теперь поле заполнено, можно загружать
            isInitialized = true;
        }


        private void LoadProducts()
        {
            try
            {
                var products = Queries.GetProducts(adminUsername);
                ApplyPromotionsToProducts(products);
                originalTable = products.Copy(); 
                ProductsDataGrid.ItemsSource = originalTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка");
            }
        }


        private void ApplyPromotionsToProducts(DataTable products)
        {
            var activePromotions = GetActivePromotions();

            foreach (DataRow productRow in products.Rows)
            {
                decimal originalPrice = Convert.ToDecimal(productRow["Price"]);
                string productName = productRow["Name"].ToString();
                string brand = productRow["Brand"].ToString();
                string category = productRow["CategoryName"].ToString();

                // Вычисляем общую скидку для товара
                decimal totalDiscount = 0;

                foreach (var promotion in activePromotions)
                {
                    bool isApplicable = false;

                    switch (promotion.TargetType)
                    {
                        case "Товар":
                            isApplicable = productName == promotion.TargetValue;
                            break;

                        case "Бренд":
                            isApplicable = brand == promotion.TargetValue;
                            break;

                        case "Категория":
                            isApplicable = category == promotion.TargetValue;
                            break;
                    }

                    if (isApplicable)
                    {
                        totalDiscount += promotion.DiscountPercentage;
                    }
                }

                if (totalDiscount > 0)
                {
                    decimal maxDiscount = 80m;
                    decimal minPrice = 10m;

                    if (totalDiscount > maxDiscount)
                        totalDiscount = maxDiscount;

                    decimal discountedPrice = originalPrice * (1 - totalDiscount / 100);

                    if (discountedPrice < minPrice)
                        discountedPrice = minPrice;

                    productRow["Price"] = Math.Round(discountedPrice, 2);
                }
            }
        }


        private List<Promotion> GetActivePromotions()
        {
            string query = @"
SELECT 
    p.PromotionID,
    pr.TargetType,
    pr.TargetValue,
    p.DiscountPercentage
FROM 
    Promotions p
INNER JOIN 
    PromotionRules pr ON p.PromotionID = pr.PromotionID
WHERE 
    @Today BETWEEN p.StartDate AND p.EndDate";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Today", DateTime.Today);
            }).AsEnumerable().Select(row => new Promotion
            {
                PromotionID = row.Field<int>("PromotionID"),
                TargetType = row.Field<string>("TargetType"),
                TargetValue = row.Field<string>("TargetValue"),
                DiscountPercentage = row.Field<decimal>("DiscountPercentage")
            }).ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (originalTable == null || originalTable.Rows.Count == 0)
            {
                ProductsDataGrid.ItemsSource = null;
                return;
            }

            string searchQuery = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                ProductsDataGrid.ItemsSource = originalTable.DefaultView;
                return;
            }

            var filteredRows = originalTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(field => field?.ToString().ToLower().Contains(searchQuery) == true))
                .ToList();

            if (filteredRows.Count == 0)
            {
                ProductsDataGrid.ItemsSource = null;
                return;
            }

            DataTable filteredTable = filteredRows.CopyToDataTable();
            ProductsDataGrid.ItemsSource = filteredTable.DefaultView;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow(adminUsername);
            addProductWindow.ShowDialog();
            LoadProducts();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string name = selectedRow["Name"].ToString();
                string category = selectedRow["CategoryName"].ToString();
                decimal price = Convert.ToDecimal(selectedRow["Price"]);
                int quantity = Convert.ToInt32(selectedRow["Quantity"]);
                string size = selectedRow["Size"].ToString();
                string composition = selectedRow["Composition"].ToString();
                string shelfLife = selectedRow["ShelfLife"].ToString();
                string deliveryTime = selectedRow["DeliveryTime"].ToString();
                string brand = selectedRow["Brand"].ToString();
                int minStockLevel = Convert.ToInt32(selectedRow["MinStockLevel"]);

                var editProductWindow = new EditProductWindow(
                    name: name,
                    category: category,
                    price: price,
                    quantity: quantity,
                    size: size,
                    composition: composition,
                    shelfLife: shelfLife,
                    deliveryTime: deliveryTime,
                    adminUsername: adminUsername,
                    brand: brand,
                    minStockLevel: minStockLevel
                );

                editProductWindow.ShowDialog();

                LoadProducts();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string productName = selectedRow["Name"].ToString();
                int productId = Convert.ToInt32(selectedRow["ProductID"]);

                string checkUsageQuery = @"
                    SELECT COUNT(*) 
                    FROM TransactionDetails 
                    WHERE ProductID = @ProductID";

                int usageCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkUsageQuery, command =>
                {
                    command.Parameters.AddWithValue("@ProductID", productId);
                }));

                if (usageCount > 0)
                {
                    MessageBox.Show("Невозможно удалить товар, так как он используется в транзакциях.", "Ошибка");
                    return;
                }

                try
                {
                    Queries.DeleteProduct(productId, adminUsername);
                    MessageBox.Show($"Товар успешно удален.", "Успех");
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка");
                }

            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized) return;

            if (FilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string filterType = selectedItem.Content.ToString();

                switch (filterType)
                {
                    case "Все товары":
                        LoadProducts();
                        break;
                    case "Часто продаваемые":
                        LoadFrequentProducts();
                        break;
                    case "Нечасто продаваемые":
                        LoadInfrequentProducts();
                        break;
                }
            }
        }


        private void LoadFrequentProducts()
        {
            try
            {
                if (string.IsNullOrEmpty(adminUsername))
                {
                    MessageBox.Show("Имя администратора пустое при загрузке часто продаваемых товаров.", "Ошибка");
                    return;
                }

                var frequentProducts = Queries.GetProductsBySalesFrequency(adminUsername);

                var filteredRows = frequentProducts.AsEnumerable()
                    .Where(row => row.Field<int>("TotalSoldQuantity") > 5)
                    .ToList();

                if (filteredRows.Count == 0)
                {
                    ProductsDataGrid.ItemsSource = null;
                    originalTable = null;
                    return;
                }

                originalTable = filteredRows.CopyToDataTable();  // Сохраняем для поиска
                ProductsDataGrid.ItemsSource = originalTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        private void LoadInfrequentProducts()
        {
            try
            {
                if (string.IsNullOrEmpty(adminUsername))
                {
                    MessageBox.Show("Имя администратора пустое при загрузке нечасто продаваемых товаров.", "Ошибка");
                    return;
                }

                var infrequentProducts = Queries.GetProductsBySalesFrequency(adminUsername);

                var filteredRows = infrequentProducts.AsEnumerable()
                    .Where(row => row.Field<int>("TotalSoldQuantity") <= 10)
                    .ToList();

                if (filteredRows.Count == 0)
                {
                    ProductsDataGrid.ItemsSource = null;
                    originalTable = null;
                    return;
                }

                originalTable = filteredRows.CopyToDataTable(); 
                ProductsDataGrid.ItemsSource = originalTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }


        private void OpenCategoryManagement(object sender, RoutedEventArgs e)
        {
            var categoryManagementWindow = new CategoryManagementWindow(adminUsername);
            categoryManagementWindow.ShowDialog();
        }
    }
}
