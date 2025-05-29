using Kursach.Database;
using Kursach.main_windows.admin;
using System.Data;
using System.Transactions;
using System.Windows;

namespace Diploma.main_windows.admin
{
    public partial class TransactionDetailsWindow : Window
    {
        private readonly DataTable _transactionDetails;
        private string admin;
        private int transactionId;

        public TransactionDetailsWindow(DataTable transactionDetails, int transactionId, string admin)
        {
            InitializeComponent();
            this.admin = admin;

            this.transactionId = transactionId;
            _transactionDetails = transactionDetails;

            TransactionDetailsDataGrid.ItemsSource = transactionDetails.DefaultView;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string type = Queries.GetTransactionType(transactionId);
                string status = Queries.GetTransactionStatus(transactionId);

                if (type == "Заказ" && status != "Завершен")
                {
                    MessageBox.Show("Возврат доступен только для завершённых заказов.", "Отказ");
                    return;
                }

                if (TransactionDetailsDataGrid.ItemsSource == null)
                {
                    MessageBox.Show("Нет данных для возврата.", "Ошибка");
                    return;
                }

                var transactionDetails = ((DataView)TransactionDetailsDataGrid.ItemsSource).ToTable();

                if (transactionDetails.Rows.Count == 0)
                {
                    MessageBox.Show("Нет товаров для возврата.", "Ошибка");
                    return;
                }

                var returnWindow = new ReturnWindow(transactionId, transactionDetails, admin);

                if (returnWindow.ShowDialog() == true)
                {
                    var updatedDetails = Queries.GetTransactionDetails(transactionId);
                    _transactionDetails.Clear();
                    foreach (DataRow row in updatedDetails.Rows)
                        _transactionDetails.ImportRow(row);

                    TransactionDetailsDataGrid.ItemsSource = _transactionDetails.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна возврата: {ex.Message}", "Ошибка");
            }
        }
    }
}