using Diploma.main_windows;
using Kursach.Database;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
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
            InitializeComponent();
            this.adminUsername = adminUsername;
            isDiscountInPercent = false;

            LoadProducts();
        }

        private void LoadProducts()
        {
            // Загружаем товары из базы данных
            productsTable = Queries.GetProducts(adminUsername);
            if (productsTable == null || productsTable.Rows.Count == 0)
            {
                MessageBox.Show("Нет доступных товаров.", "Информация");
                return;
            }

            // Добавляем колонку для количества заказа, если её нет
            if (!productsTable.Columns.Contains("OrderQuantity"))
            {
                productsTable.Columns.Add("OrderQuantity", typeof(int));
                foreach (DataRow row in productsTable.Rows)
                {
                    row["OrderQuantity"] = 0;
                }
            }

            // Применяем акции к товарам
            Queries.ApplyPromotions(productsTable);

            // Отображаем товары в DataGrid
            ProductsDataGrid.ItemsSource = productsTable.DefaultView;

            // Обновляем общую стоимость
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

        private decimal CalculateTotalPrice(List<DataRow> selectedProducts)
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

        private void UpdateQuantities(List<DataRow> selectedProducts)
        {
            foreach (var row in selectedProducts)
            {
                string productName = row.Field<string>("Name");
                int newQuantity = row.Field<int>("Quantity") - row.Field<int>("OrderQuantity");
                Queries.UpdateProductQuantity(productName, newQuantity);
            }
        }

        private void ResetForm()
        {
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
            string searchQuery = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                ProductsDataGrid.ItemsSource = productsTable.DefaultView;
                return;
            }

            var filteredRows = productsTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(field => field.ToString().ToLower().Contains(searchQuery)))
                .CopyToDataTable();

            ProductsDataGrid.ItemsSource = filteredRows.DefaultView;
        }

        private void OrderQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
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
            var selectedProducts = productsTable.AsEnumerable()
                .Where(row => row.Field<int>("OrderQuantity") > 0)
                .ToList();

            if (selectedProducts.Count == 0)
            {
                MessageBox.Show("Нет выбранных товаров.", "Ошибка");
                return;
            }

            decimal originalTotal = CalculateTotalPrice(selectedProducts);
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

            var paymentWindow = new PaymentConfirmationWindow(selectedProducts,originalTotal, discountedTotal, paymentAmount);
            paymentWindow.ShowDialog();

            if (paymentWindow.IsConfirmed)
            {
                UpdateQuantities(selectedProducts);
                Queries.AddSale(selectedProducts, discountedTotal, adminUsername);
                Queries.UpdateCashAmount(discountedTotal, "Продажа", adminUsername); 
                MessageBox.Show("Заказ успешно оплачен!", "Успех");
                ResetForm();
            }
            else
            {
                MessageBox.Show("Оплата отменена.", "Информация");
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
                        selectedProducts.Columns.Add("Name", typeof(string));
                        selectedProducts.Columns.Add("Brand", typeof(string));
                        selectedProducts.Columns.Add("OrderQuantity", typeof(int));
                        selectedProducts.Columns.Add("Price", typeof(decimal));

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            string productName = worksheet.Cells[row, 4].Text;
                            string brand = worksheet.Cells[row, 5].Text;
                            string quantityStr = worksheet.Cells[row, 6].Text;

                            if (!int.TryParse(quantityStr, out int quantity))
                            {
                                continue;
                            }

                            var productRow = productsTable.AsEnumerable()
                                .FirstOrDefault(p => p.Field<string>("Name") == productName);

                            if (productRow != null)
                            {
                                decimal price = productRow.Field<decimal>("Price");
                                selectedProducts.Rows.Add(productName, brand, quantity, price);
                            }
                        }

                        var orderConfirmationWindow = new OrderConfirmationWindow(clientInfo, selectedProducts,adminUsername);
                        orderConfirmationWindow.ShowDialog();
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
                        worksheet.Cells[2, 3].Value = "+79991234567";
                        worksheet.Cells[2, 4].Value = "Пример товара 1";
                        worksheet.Cells[2, 5].Value = "Brand";
                        worksheet.Cells[2, 6].Value = 1;
                        worksheet.Cells[2, 7].Value = "Без комментариев";

                        worksheet.Cells[3, 4].Value = "Пример товара 2";
                        worksheet.Cells[3, 5].Value = "Brand";
                        worksheet.Cells[3, 6].Value = 2;
                        worksheet.Cells[3, 7].Value = "Дополнительный комментарий";

                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();
                        worksheet.Column(6).AutoFit();
                        worksheet.Column(7).AutoFit();

                        FileInfo fileInfo = new FileInfo(dialog.FileName);
                        package.SaveAs(fileInfo);
                    }

                    MessageBox.Show("Шаблон успешно сохранен.", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании шаблона: {ex.Message}", "Ошибка");
                }
            }
        }
    }
}