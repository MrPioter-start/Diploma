using System.Windows;
using System.Data;
using System.Linq;
using Kursach.Database;
using System.Windows.Controls;
using System.Transactions;

namespace Kursach.main_windows.admin
{
    public partial class ReturnWindow : Window
    {
        private int saleId;
        private DataTable returnItemsTable;
        private string userUsername;

        public ReturnWindow(int saleId, DataTable transactionDetails, string userUsername)
        {
            InitializeComponent();
            this.saleId = saleId;
            this.returnItemsTable = PrepareReturnItemsTable(transactionDetails);
            this.userUsername = userUsername;

            ReturnItemsDataGrid.ItemsSource = returnItemsTable.DefaultView;
        }

        private DataTable PrepareReturnItemsTable(DataTable transactionDetails)
        {
            var table = transactionDetails.Clone();
            table.Columns.Add("ReturnQuantity", typeof(int));

            foreach (DataRow row in transactionDetails.Rows)
            {
                var newRow = table.NewRow();
                newRow.ItemArray = row.ItemArray;
                newRow["ReturnQuantity"] = 0;
                table.Rows.Add(newRow);
            }

            return table;
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
            try
            {
                var rowsToReturn = returnItemsTable.AsEnumerable()
                    .Where(row => row.Field<int>("ReturnQuantity") > 0)
                    .ToList();

                if (rowsToReturn.Count == 0)
                {
                    MessageBox.Show("Нет товаров для возврата.", "Ошибка");
                    return;
                }

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

                    Queries.UpdateTransactionTotal(saleId);

                    Queries.ReturnProduct(productName, returnQuantity, saleId);
                    Queries.AddReturn(saleId, productName, returnQuantity, userUsername);
                }

                Queries.UpdateCashAmount(-totalRefundAmount, "Возврат", userUsername);

                MessageBox.Show("Товар успешно возвращен!", "Успех");

                this.DialogResult = true;

                Queries.UpdateTransactionTotal(saleId);
                decimal updatedTotal = Queries.GetTransactionTotal(saleId);

                if (updatedTotal == 0)
                {
                    Queries.UpdateOrderStatus(saleId, "Возвращен");
                }

                this.Close();
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