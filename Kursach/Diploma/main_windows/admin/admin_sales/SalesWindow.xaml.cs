using Diploma.main_windows;
using Kursach.Database;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using ExcelDataReader;
using System.Windows.Controls;

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

            this.Loaded += SalesWindow_Loaded;  // вызов LoadProducts после загрузки окна
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

            Queries.ApplyPromotions(productsTable);

            ProductsDataGrid.ItemsSource = productsTable.DefaultView;

            UpdateTotalPrice();
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string productName)
            {
                var row = productsTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("Name") == productName);
                if (row != null)
                {
                    int currentQuantity = row.Field<int>("Quantity");
                    int orderQuantity = row.Field<int>("OrderQuantity");

                    if (orderQuantity < currentQuantity)
                    {
                        row.SetField("OrderQuantity", orderQuantity + 1);
                        UpdateTotalPrice();
                    }
                }
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string productName)
            {
                var row = productsTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("Name") == productName);
                if (row != null)
                {
                    int orderQuantity = row.Field<int>("OrderQuantity");

                    if (orderQuantity > 0)
                    {
                        row.SetField("OrderQuantity", orderQuantity - 1);
                        UpdateTotalPrice();
                    }
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
                int currentQuantity = row.Field<int>("Quantity");
                int orderQuantity = row.Field<int>("OrderQuantity");

                int newQuantity = currentQuantity - orderQuantity;
                Queries.UpdateProductQuantity(productName, newQuantity);
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
            if (TotalPriceTextBlock == null)
            {
                return;
            }

            decimal totalPrice = 0;

            if (productsTable != null && productsTable.Rows.Count > 0)
            {
                foreach (DataRow row in productsTable.Rows)
                {
                    int orderQuantity = row.Field<int>("OrderQuantity");
                    decimal price = row.Field<decimal>("Price");
                    totalPrice += price * orderQuantity;
                }
            }

            if (discountValue > 0)
            {
                if (isDiscountInPercent)
                {
                    totalPrice -= totalPrice * (discountValue / 100);
                }
                else
                {
                    totalPrice -= discountValue;
                }

                totalPrice = Math.Max(totalPrice, 0);
            }

            TotalPriceTextBlock.Text = $"{totalPrice:F2} byn";
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

                // Фильтруем товары с OrderQuantity > 0
                var selectedProducts = productsTable.AsEnumerable()
                    .Where(row => row.Field<int>("OrderQuantity") > 0)
                    .ToList();

                if (selectedProducts.Count == 0)
                {
                    MessageBox.Show("Нет выбранных товаров.", "Ошибка");
                    return;
                }

                // Создаем новую таблицу с необходимыми колонками
                var formattedProducts = new DataTable();
                formattedProducts.Columns.Add("ProductID", typeof(int));
                formattedProducts.Columns.Add("Name", typeof(string));
                formattedProducts.Columns.Add("OrderQuantity", typeof(int));
                formattedProducts.Columns.Add("Price", typeof(decimal));

                foreach (var row in selectedProducts)
                {
                    var newRow = formattedProducts.NewRow();
                    newRow["ProductID"] = row.Field<int>("ProductID"); // Убедитесь, что колонка ProductID существует
                    newRow["Name"] = row.Field<string>("Name");
                    newRow["OrderQuantity"] = row.Field<int>("OrderQuantity");
                    newRow["Price"] = row.Field<decimal>("Price");
                    formattedProducts.Rows.Add(newRow);
                }

                // Преобразуем данные в List<DataRow>
                var formattedProductsList = formattedProducts.AsEnumerable().ToList();

                // Подсчитываем общую сумму
                decimal originalTotal = CalculateTotalPrice(formattedProductsList);
                decimal discountedTotal = ApplyDiscount(originalTotal);
                decimal paymentAmount = discountedTotal;

                // Проверяем введенную сумму оплаты
                if (!string.IsNullOrEmpty(PaymentAmountTextBox.Text))
                {
                    if (!decimal.TryParse(PaymentAmountTextBox.Text, out paymentAmount) || paymentAmount < discountedTotal)
                    {
                        MessageBox.Show("Введите корректную сумму для оплаты.", "Ошибка");
                        return;
                    }
                }

                // Открываем окно подтверждения
                var paymentWindow = new PaymentConfirmationWindow(formattedProductsList, originalTotal, discountedTotal, paymentAmount);
                bool? result = paymentWindow.ShowDialog();

                if (result == true && paymentWindow.IsConfirmed)
                {
                    // Обновляем количество товаров
                    UpdateQuantities(formattedProductsList);

                    // Добавляем транзакцию
                    Queries.AddTransaction(formattedProductsList, discountedTotal, adminUsername);

                    // Обновляем баланс кассы
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
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(new FileInfo(dialog.FileName)))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        var clientInfo = new
                        {
                            ClientName = worksheet.Cells[2, 1].Text,
                            Email = worksheet.Cells[2, 2].Text,
                            ContactInfo = worksheet.Cells[2, 3].Text
                        };

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
                            string quantityStr = worksheet.Cells[row, 6].Text.Trim();

                            if (!int.TryParse(quantityStr, out int quantity) || quantity <= 0)
                            {
                                continue; 
                            }

                            var productRow = productsTable.AsEnumerable()
                                .FirstOrDefault(p =>
                                    p.Field<string>("Name").Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                                    p.Field<string>("Brand").Equals(brand, StringComparison.OrdinalIgnoreCase));

                            if (productRow != null)
                            {
                                int productID = productRow.Field<int>("ProductID");
                                decimal price = productRow.Field<decimal>("Price");

                                selectedProducts.Rows.Add(productID, productName, brand, quantity, price);
                            }
                            else
                            {
                                MessageBox.Show($"Продукт '{productName}' с брендом '{brand}' не найден.", "Предупреждение");
                            }
                        }

                        if (selectedProducts.Rows.Count > 0)
                        {
                            var orderConfirmationWindow = new OrderConfirmationWindow(clientInfo, selectedProducts, adminUsername);
                            orderConfirmationWindow.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Нет доступных продуктов для создания заказа.", "Предупреждение");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке заказа: {ex.Message}", "Ошибка");
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

                    case "Нечасто продаваемые":
                        LoadInfrequentProducts();
                        break;
                }
            }
        }

        // Здесь добавь методы LoadFrequentProducts и LoadInfrequentProducts, если они есть, или заглушки:
        private void LoadFrequentProducts()
        {
            var allProducts = Queries.GetProductsForSale(adminUsername);

            if (allProducts == null || !allProducts.Columns.Contains("TotalSoldQuantity"))
            {
                MessageBox.Show("Нет часто продаваемых товаров.", "Информация");
                ProductsDataGrid.ItemsSource = null;
                return;
            }

            var frequentProducts = allProducts.AsEnumerable()
                .Where(row => row.Field<int>("TotalSoldQuantity") >= 10) // например, больше или равно 10
                .CopyToDataTable();

            if (!frequentProducts.Columns.Contains("OrderQuantity"))
            {
                frequentProducts.Columns.Add("OrderQuantity", typeof(int));
                foreach (DataRow row in frequentProducts.Rows)
                {
                    row["OrderQuantity"] = 0;
                }
            }

            productsTable = frequentProducts;
            ProductsDataGrid.ItemsSource = productsTable.DefaultView;
            UpdateTotalPrice();
        }


        private void LoadInfrequentProducts()
        {
            var allProducts = Queries.GetProductsForSale(adminUsername);

            if (allProducts == null || !allProducts.Columns.Contains("TotalSoldQuantity"))
            {
                MessageBox.Show("Нет нечасто продаваемых товаров.", "Информация");
                ProductsDataGrid.ItemsSource = null;
                return;
            }

            var infrequentProducts = allProducts.AsEnumerable()
                .Where(row => row.Field<int>("TotalSoldQuantity") < 10) 
                .CopyToDataTable();

            if (!infrequentProducts.Columns.Contains("OrderQuantity"))
            {
                infrequentProducts.Columns.Add("OrderQuantity", typeof(int));
                foreach (DataRow row in infrequentProducts.Rows)
                {
                    row["OrderQuantity"] = 0;
                }
            }

            productsTable = infrequentProducts;
            ProductsDataGrid.ItemsSource = productsTable.DefaultView;
            UpdateTotalPrice();
        }
    }
}
