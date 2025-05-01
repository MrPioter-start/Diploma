using System.Windows;
using System.Data;
using System.Linq;
using Kursach.Database;
using System.Windows.Controls;

namespace Kursach.main_windows.admin
{
    public partial class ReturnWindow : Window
    {
        private int saleId;
        private DataTable returnItemsTable;
        private string userUsername;

        public ReturnWindow(int saleId, string userUsername)
        {
            InitializeComponent();
            this.saleId = saleId;
            LoadReturnItems();
            this.userUsername = userUsername;
        }

        private void LoadReturnItems()
        {
            returnItemsTable = Queries.GetSaleDetails(saleId);
            foreach (DataRow row in returnItemsTable.Rows)
            {
                row["ReturnQuantity"] = 0;
            }
            ReturnItemsDataGrid.ItemsSource = returnItemsTable.DefaultView;
        }

        private void IncreaseReturnQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string productName)
            {
                var row = returnItemsTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("ProductName") == productName);
                if (row != null)
                {
                    int returnQuantity = row.Field<int>("ReturnQuantity");
                    int purchasedQuantity = row.Field<int>("Quantity");

                    if (returnQuantity < purchasedQuantity)
                    {
                        row.SetField("ReturnQuantity", returnQuantity + 1);
                    }
                }
            }
        }

        private void DecreaseReturnQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string productName)
            {
                var row = returnItemsTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("ProductName") == productName);
                if (row != null)
                {
                    int returnQuantity = row.Field<int>("ReturnQuantity");

                    if (returnQuantity > 0)
                    {
                        row.SetField("ReturnQuantity", returnQuantity - 1);
                    }
                }
            }
        }

        private void ConfirmReturn_Click(object sender, RoutedEventArgs e)
        {
            var rowsToReturn = returnItemsTable.AsEnumerable()
                .Where(row => row.Field<int>("ReturnQuantity") > 0)
                .ToList();

            if (rowsToReturn.Count == 0)
            {
                MessageBox.Show("Нет товаров для возврата.", "Ошибка");
                return;
            }

            try
            {
                decimal totalRefundAmount = 0;

                foreach (var row in rowsToReturn)
                {
                    string productName = row.Field<string>("ProductName");
                    int returnQuantity = row.Field<int>("ReturnQuantity");
                    decimal soldPrice = row.Field<decimal>("Price"); 
                    decimal refundAmount = soldPrice * returnQuantity;

                    decimal currentCash = Queries.GetCurrentCashAmount(userUsername);
                    if (currentCash < refundAmount)
                    {
                        MessageBox.Show("Недостаточно средств в кассе для возврата.", "Ошибка");
                        return;
                    }

                    totalRefundAmount += refundAmount;

                    Queries.ReturnProduct(productName, returnQuantity, saleId);
                    Queries.AddReturn(saleId, productName, returnQuantity, userUsername);
                }

                Queries.UpdateCashAmount(-totalRefundAmount, "Возврат", userUsername);

                MessageBox.Show("Товар успешно возвращен!", "Успех");
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при возврате: {ex.Message}", "Ошибка");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}