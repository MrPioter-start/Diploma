using Kursach.Database;
using Kursach.Database.WarehouseApp.Database;
using Kursach.main_windows.admin.admin_product;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kursach.main_windows.admin
{
    /// <summary>
    /// Логика взаимодействия для ProductManagementWindow.xaml
    /// </summary>
    public partial class ProductManagementWindow : Window
    {
        private string adminUsername;
        private DataTable originalTable;

        public ProductManagementWindow(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            LoadProducts();
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

        private void LoadProducts()
        {
            try
            {
                originalTable = Queries.GetProducts(adminUsername); 
                ProductsDataGrid.ItemsSource = originalTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
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
                    Queries.DeleteProduct(productName, adminUsername);
                    MessageBox.Show($"Товар {productName} успешно удален.", "Успех");
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
                var frequentProducts = Queries.GetProductsBySalesFrequency(adminUsername);

                var filteredRows = frequentProducts.AsEnumerable()
                    .Where(row => row.Field<int>("TotalSoldQuantity") > 5) 
                    .CopyToDataTable();

                ProductsDataGrid.ItemsSource = filteredRows.DefaultView;
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
                var infrequentProducts = Queries.GetProductsBySalesFrequency(adminUsername);

                var filteredRows = infrequentProducts.AsEnumerable()
                    .Where(row => row.Field<int>("TotalSoldQuantity") <= 10) 
                    .CopyToDataTable();

                ProductsDataGrid.ItemsSource = filteredRows.DefaultView;
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
