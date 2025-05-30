using Diploma.main_windows;
using Kursach.Database;
using Microsoft.Win32;
using OfficeOpenXml;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Kursach.Database.WarehouseApp.Database;
using System.Windows.Input;


namespace Kursach.main_windows.admin
{
    public partial class SalesWindow : Window
    {
        private DataTable productsTable;
        private string adminUsername;
        private decimal discountValue = 0;
        private bool isDiscountInPercent = false;

        public SalesWindow(string adminUsername)
        {
            this.adminUsername = adminUsername;
            InitializeComponent();
            isDiscountInPercent = false;

            this.Loaded += SalesWindow_Loaded;  
        }

        private void SalesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            if (ProductsDataGrid == null)
                return;

            productsTable = Queries.GetProductsForSale(adminUsername);
            if (productsTable == null || productsTable.Rows.Count == 0)
            {
                MessageBox.Show("Нет доступных товаров.", "Информация");
                ProductsDataGrid.ItemsSource = null;
                return;
            }

            if (!productsTable.Columns.Contains("OrderQuantity"))
            {
                productsTable.Columns.Add("OrderQuantity", typeof(int));
                foreach (DataRow row in productsTable.Rows)
                {
                    row["OrderQuantity"] = 0;
                }
            }

            ApplyPromotionsToProducts(productsTable);

            ProductsDataGrid.ItemsSource = productsTable.DefaultView;

            UpdateTotalPrice();
        }

        private async void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DataRowView rowView)
            {
                button.IsEnabled = false; 

                int quantity = rowView["Quantity"] != DBNull.Value ? Convert.ToInt32(rowView["Quantity"]) : 0;
                int orderQuantity = rowView["OrderQuantity"] != DBNull.Value ? Convert.ToInt32(rowView["OrderQuantity"]) : 0;

                if (orderQuantity < quantity)
                {
                    rowView["OrderQuantity"] = orderQuantity + 1;

                    ProductsDataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                    ProductsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                    Keyboard.ClearFocus();
                    FocusManager.SetFocusedElement(this, (IInputElement)TotalPriceTextBlock);

                    UpdateTotalPrice();
                }

                await Task.Delay(50); 
                button.IsEnabled = true;
            }
        }






        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DataRowView rowView)
            {
                int orderQuantity = rowView["OrderQuantity"] != DBNull.Value ? Convert.ToInt32(rowView["OrderQuantity"]) : 0;

                if (orderQuantity > 0)
                {
                    rowView["OrderQuantity"] = orderQuantity - 1;

                    ProductsDataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                    ProductsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                    Keyboard.ClearFocus();
                    FocusManager.SetFocusedElement(this, (IInputElement)TotalPriceTextBlock);

                    UpdateTotalPrice();
                }
            }
        }




        private decimal CalculateTotalPrice(System.Collections.Generic.List<DataRow> selectedProducts)
        {
            decimal total = 0;
            foreach (var row in selectedProducts)
            {
                total += row.Field<decimal>("Price") * row.Field<int>("OrderQuantity");
            }
            return total;
        }

        private decimal ApplyDiscount(decimal totalPrice)
        {
            if (discountValue > 0)
            {
                return isDiscountInPercent
                    ? totalPrice - (totalPrice * discountValue / 100)
                    : totalPrice - discountValue;
            }
            return totalPrice;
        }

        private void UpdateQuantities(System.Collections.Generic.List<DataRow> selectedProducts)
        {
            foreach (var row in selectedProducts)
            {
                string productName = row.Field<string>("Name");
                int orderQuantity = row.Field<int>("OrderQuantity");

                var originalRow = productsTable.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("Name") == productName);

                if (originalRow != null)
                {
                    int currentQuantity = originalRow.Field<int>("Quantity");
                    int newQuantity = currentQuantity - orderQuantity;

                    Queries.UpdateProductQuantity(productName, newQuantity);
                }
                else
                {
                    MessageBox.Show($"Не удалось обновить количество для товара {productName}", "Ошибка");
                }
            }
        }


        private void ResetForm()
        {
            if (productsTable == null)
                return;

            foreach (DataRow row in productsTable.Rows)
            {
                row["OrderQuantity"] = 0;
            }
            discountValue = 0;
            DiscountTextBox.Text = "0";
            LoadProducts();
        }

        private void UpdateTotalPrice()
        {
            if (ProductsDataGrid.ItemsSource == null)
                return;

            decimal total = 0;

            foreach (DataRowView rowView in ProductsDataGrid.ItemsSource)
            {
                int orderQuantity = rowView["OrderQuantity"] != DBNull.Value ? Convert.ToInt32(rowView["OrderQuantity"]) : 0;
                decimal price = rowView["Price"] != DBNull.Value ? Convert.ToDecimal(rowView["Price"]) : 0;

                total += orderQuantity * price;
            }

            if (discountValue > 0)
            {
                if (isDiscountInPercent)
                {
                    total -= total * discountValue / 100;
                }
                else
                {
                    total -= discountValue;
                }

                total = Math.Max(total, 0);
            }

            TotalPriceTextBlock.Text = $"{total:F2} byn";
        }


        private void DiscountTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DiscountTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                isDiscountInPercent = selectedItem.Content.ToString() == "%";

                UpdateTotalPrice();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productsTable == null)
                return;

            string searchQuery = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                ProductsDataGrid.ItemsSource = productsTable.DefaultView;
                return;
            }

            var filteredRows = productsTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(field => field.ToString().ToLower().Contains(searchQuery)));

            if (filteredRows.Any())
            {
                ProductsDataGrid.ItemsSource = filteredRows.CopyToDataTable().DefaultView;
            }
            else
            {
                ProductsDataGrid.ItemsSource = null;
            }
        }

        private void OrderQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productsTable == null)
                return;

            if (sender is TextBox textBox && textBox.Tag is string productName)
            {
                var row = productsTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("Name") == productName);
                if (row != null)
                {
                    int availableQuantity = row.Field<int>("Quantity");
                    int orderQuantity = 0;

                    if (!int.TryParse(textBox.Text, out orderQuantity))
                    {
                        textBox.Text = "0";
                        return;
                    }

                    if (orderQuantity > availableQuantity)
                    {
                        MessageBox.Show($"Недостаточно товара '{productName}' в наличии.", "Ошибка");
                        textBox.Text = availableQuantity.ToString();
                        return;
                    }

                    row.SetField("OrderQuantity", orderQuantity);

                    UpdateTotalPrice();
                }
            }
        }

        private void DiscountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!decimal.TryParse(textBox.Text, out discountValue))
                {
                    discountValue = 0;
                }

                UpdateTotalPrice();
            }
        }

        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (productsTable == null)
                    return;

                var selectedProducts = productsTable.AsEnumerable()
    .Where(row => Convert.ToInt32(row["OrderQuantity"]) > 0)
    .ToList();


                if (selectedProducts.Count == 0)
                {
                    MessageBox.Show("Нет выбранных товаров.", "Ошибка");
                    return;
                }

                var formattedProducts = new DataTable();
                formattedProducts.Columns.Add("ProductID", typeof(int));
                formattedProducts.Columns.Add("Name", typeof(string));
                formattedProducts.Columns.Add("Brand", typeof(string));
                formattedProducts.Columns.Add("CategoryName", typeof(string));
                formattedProducts.Columns.Add("OrderQuantity", typeof(int));
                formattedProducts.Columns.Add("Price", typeof(decimal));

                foreach (var row in selectedProducts)
                {
                    var newRow = formattedProducts.NewRow();
                    newRow["ProductID"] = row.Field<int>("ProductID"); 
                    newRow["Name"] = row.Field<string>("Name");
                    newRow["Brand"] = row.Field<string>("Brand") ?? "Неизвестно";
                    newRow["CategoryName"] = row.Field<string>("CategoryName") ?? "Неизвестно";
                    newRow["OrderQuantity"] = row.Field<int>("OrderQuantity");
                    newRow["Price"] = row.Field<decimal>("Price");
                    formattedProducts.Rows.Add(newRow);
                }

                var formattedProductsList = formattedProducts.AsEnumerable().ToList();

                decimal originalTotal = CalculateTotalPrice(formattedProductsList);
                decimal discountedTotal = ApplyDiscount(originalTotal);
                decimal paymentAmount = discountedTotal;

                if (!string.IsNullOrEmpty(PaymentAmountTextBox.Text))
                {
                    if (!decimal.TryParse(PaymentAmountTextBox.Text, out paymentAmount) || paymentAmount < discountedTotal)
                    {
                        MessageBox.Show("Введите корректную сумму для оплаты.", "Ошибка");
                        return;
                    }
                }

                var paymentWindow = new PaymentConfirmationWindow(formattedProductsList, originalTotal, discountedTotal, paymentAmount);
                bool? result = paymentWindow.ShowDialog();

                if (result == true && paymentWindow.IsConfirmed)
                {
                    UpdateQuantities(formattedProductsList);

                    Queries.AddTransaction(formattedProductsList, discountedTotal, adminUsername);

                    Queries.UpdateCashAmount(discountedTotal, "Продажа", adminUsername);

                    MessageBox.Show("Заказ успешно оплачен!", "Успех");

                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Оплата отменена.", "Информация");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подтверждении заказа: {ex.Message}", "Ошибка");
            }
        }


        private void LoadOrderFromExcel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(dialog.FileName)))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    // Считываем данные клиента из первой строки
                    string clientName = worksheet.Cells[2, 1].Text.Trim();
                    string clientEmail = worksheet.Cells[2, 2].Text.Trim();
                    string clientContact = worksheet.Cells[2, 3].Text.Trim();

                    var existing = Queries.GetCustomerByEmail(clientEmail);
                    if (existing.Rows.Count == 0)
                    {
                        Queries.AddOrUpdateCustomer(clientName, clientEmail, clientContact, adminUsername);
                    }

                    var selectedProducts = new DataTable();
                    selectedProducts.Columns.Add("ProductID", typeof(int));
                    selectedProducts.Columns.Add("Name", typeof(string));
                    selectedProducts.Columns.Add("Brand", typeof(string));
                    selectedProducts.Columns.Add("OrderQuantity", typeof(int));
                    selectedProducts.Columns.Add("Price", typeof(decimal));

                    var productsTable = Queries.GetProductsBySalesFrequency(adminUsername);

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        string productName = worksheet.Cells[row, 4].Text.Trim();
                        string brand = worksheet.Cells[row, 5].Text.Trim();
                        if (!int.TryParse(worksheet.Cells[row, 6].Text.Trim(), out int qty) || qty <= 0)
                            continue;

                        var prod = productsTable.AsEnumerable()
                            .FirstOrDefault(p =>
                                p.Field<string>("Name").Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                                p.Field<string>("Brand").Equals(brand, StringComparison.OrdinalIgnoreCase));

                        if (prod != null)
                        {
                            selectedProducts.Rows.Add(
                                prod.Field<int>("ProductID"),
                                productName,
                                brand,
                                qty,
                                prod.Field<decimal>("Price"));
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Товар «{productName}» (бренд «{brand}») не найден в справочнике.",
                                "Предупреждение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }

                    if (selectedProducts.Rows.Count == 0)
                    {
                        MessageBox.Show("Нет ни одного корректного товара для заказа.", "Информация");
                        return;
                    }

                    // Применяем акции и открываем окно подтверждения
                    ApplyPromotionsToProductsFromExcel(selectedProducts, productsTable);
                    var clientInfo = new { ClientName = clientName, Email = clientEmail, ContactInfo = clientContact };
                    var wnd = new OrderConfirmationWindow(clientInfo, selectedProducts, adminUsername);
                    wnd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке из Excel:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ApplyPromotionsToProductsFromExcel(DataTable selectedProducts, DataTable productsTable)
        {
            // Получаем активные акции
            var activePromotions = GetActivePromotions();

            // Создаем словарь ProductID -> CategoryName из productsTable
            var productCategories = productsTable.AsEnumerable()
                .ToDictionary(row => row.Field<int>("ProductID"),
                              row => row.Field<string>("CategoryName") ?? string.Empty);

            // Добавляем колонку CategoryName в selectedProducts, если её нет
            if (!selectedProducts.Columns.Contains("CategoryName"))
                selectedProducts.Columns.Add("CategoryName", typeof(string));

            // Заполняем CategoryName по ProductID
            foreach (DataRow row in selectedProducts.Rows)
            {
                int productId = (int)row["ProductID"];
                if (productCategories.TryGetValue(productId, out var categoryName))
                {
                    row["CategoryName"] = categoryName;
                }
                else
                {
                    row["CategoryName"] = string.Empty; // или "Неизвестно"
                }
            }

            // Теперь применяем скидки по тем же правилам, что и в оригинальном методе
            foreach (DataRow productRow in selectedProducts.Rows)
            {
                decimal originalPrice = Convert.ToDecimal(productRow["Price"]);
                string productName = productRow["Name"].ToString();
                string brand = productRow["Brand"].ToString();
                string category = productRow["CategoryName"].ToString();

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



        private void DownloadExcelTemplate_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "OrderTemplate.xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Шаблон заказа");

                        worksheet.Cells[1, 1].Value = "Имя клиента";
                        worksheet.Cells[1, 2].Value = "Электронная почта";
                        worksheet.Cells[1, 3].Value = "Контактная информация";
                        worksheet.Cells[1, 4].Value = "Наименование товара";
                        worksheet.Cells[1, 5].Value = "Бренд";
                        worksheet.Cells[1, 6].Value = "Количество";
                        worksheet.Cells[1, 7].Value = "Комментарий";

                        using (var range = worksheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FF00A651"));
                            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        }

                        worksheet.Cells[2, 1].Value = "Иван Иванов";
                        worksheet.Cells[2, 2].Value = "ivan@example.com";
                        worksheet.Cells[2, 3].Value = "+375291234567";

                        package.SaveAs(new FileInfo(dialog.FileName));

                        MessageBox.Show("Шаблон успешно сохранён.", "Успех");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении шаблона: {ex.Message}", "Ошибка");
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
                }
            }
        }

        private void LoadFrequentProducts()
        {
            Dictionary<int, int> savedQuantities = new();
            if (ProductsDataGrid.ItemsSource is DataView currentView)
            {
                foreach (DataRowView rowView in currentView)
                {
                    int productId = rowView["ProductID"] != DBNull.Value ? Convert.ToInt32(rowView["ProductID"]) : 0;
                    int orderQuantity = rowView["OrderQuantity"] != DBNull.Value ? Convert.ToInt32(rowView["OrderQuantity"]) : 0;
                    savedQuantities[productId] = orderQuantity;
                }
            }

            var allProducts = Queries.GetProductsForSale(adminUsername);

            if (allProducts == null || !allProducts.Columns.Contains("TotalSoldQuantity"))
            {
                MessageBox.Show("Нет данных о количестве продаж.", "Информация");
                ProductsDataGrid.ItemsSource = null;
                return;
            }

            var filteredRows = allProducts.AsEnumerable()
                .Where(row => row.Field<int>("TotalSoldQuantity") >= 5);

            if (!filteredRows.Any())
            {
                MessageBox.Show("Нет часто продаваемых товаров.", "Информация");
                ProductsDataGrid.ItemsSource = null;
                return;
            }

            var frequentProducts = filteredRows.CopyToDataTable();

            if (!frequentProducts.Columns.Contains("OrderQuantity"))
                frequentProducts.Columns.Add("OrderQuantity", typeof(int));

            foreach (DataRow row in frequentProducts.Rows)
            {
                int productId = row.Field<int>("ProductID");
                if (savedQuantities.TryGetValue(productId, out int qty))
                    row["OrderQuantity"] = qty;
                else
                    row["OrderQuantity"] = 0;
            }

            ApplyPromotionsToProducts(frequentProducts);

            productsTable = frequentProducts;
            ProductsDataGrid.ItemsSource = frequentProducts.DefaultView;

            UpdateTotalPrice();
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

        private void ApplyPromotionsToProducts(DataTable products)
        {
            var activePromotions = GetActivePromotions();

            foreach (DataRow productRow in products.Rows)
            {
                decimal originalPrice = Convert.ToDecimal(productRow["Price"]);
                string productName = productRow["Name"].ToString();
                string brand = productRow["Brand"].ToString();
                string category = productRow["CategoryName"].ToString();

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


    }
}
