using Kursach.Database;
using Kursach.main_windows.admin;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.main_windows
{
    /// <summary>
    /// Логика взаимодействия для SalesWindowManager.xaml
    /// </summary>
    public partial class SalesWindowManager : Window
    {
        private DataTable productsTable;
        private string adminUsername;
        private decimal discountValue = 0;
        private bool isDiscountInPercent = false;

        public SalesWindowManager(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
            isDiscountInPercent = false;

            LoadProducts();
        }

        private void LoadProducts()
        {
            productsTable = Queries.GetProducts(adminUsername);
            if (productsTable == null || productsTable.Rows.Count == 0)
            {
                MessageBox.Show("Нет доступных товаров.", "Информация");
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

            var paymentWindow = new PaymentConfirmationWindow(selectedProducts, originalTotal, discountedTotal, paymentAmount);
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
    }
}
