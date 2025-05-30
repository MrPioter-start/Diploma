using Diploma.main_windows.admin.admin_product;
using Diploma.main_windows.admin.Promotions;
using Diploma.main_windows.admin.Reportes;
using Diploma.main_windows.admin;
using Diploma.main_windows;
using Kursach.Database;
using Kursach.main_windows.admin;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kursach.main_windows
{
    public partial class ManagerWindow : Window
    {
        private string adminUsername;
        private DataTable allTransactionsTable;

        public ManagerWindow(string managerUsername)
        {
            InitializeComponent();
            this.adminUsername = Queries.GetAdminForManager(managerUsername);
            InitializeCashRegister(adminUsername);
            LoadTransactionsHistory();
            LoadCashAmount();
        }
        private void InitializeCashRegister(string userUsername)
        {
            decimal currentCash = Queries.GetCurrentCashAmount(userUsername);
            if (currentCash == 0)
            {
                Queries.UpdateCashAmount(0, "Инициализация", userUsername);
            }
        }
        private void LoadCashAmount()
        {
            decimal currentCash = Queries.GetCurrentCashAmount(adminUsername);
            CashAmountTextBlock.Text = $"{currentCash:F2} byn";
            CashAmountTextBlock.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        private void OpenCashManagement_Click(object sender, RoutedEventArgs e)
        {
            var cashWindow = new CashManagementWindow(adminUsername);
            cashWindow.Closed += (s, ev) => Dispatcher.Invoke(LoadCashAmount);
            cashWindow.ShowDialog();
        }
        private void LoadTransactionsHistory()
        {
            try
            {
                allTransactionsTable = Queries.GetTransactionsHistory(adminUsername);

                if (allTransactionsTable == null || allTransactionsTable.Rows.Count == 0)
                {
                    SalesHistoryDataGrid.ItemsSource = null;
                    return;
                }

                ApplyTransactionTypeFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных о транзакциях: {ex.Message}", "Ошибка");
            }
        }

        private void ApplyTransactionTypeFilter()
        {
            if (allTransactionsTable == null) return;

            string selectedType = (TransactionTypeFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (selectedType == "Все" || string.IsNullOrEmpty(selectedType))
            {
                SalesHistoryDataGrid.ItemsSource = allTransactionsTable.DefaultView;
            }
            else
            {
                DataView filteredView = new DataView(allTransactionsTable)
                {
                    RowFilter = $"Type = '{selectedType}'"
                };
                SalesHistoryDataGrid.ItemsSource = filteredView;
            }
        }

        private void TransactionTypeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyTransactionTypeFilter();
        }

        private void OpenProductMenu(object sender, RoutedEventArgs e)
        {
            var productMenu = new ProductManagementWindow(adminUsername);
            productMenu.ShowDialog();
        }

        private void OpenSalesMenu(object sender, RoutedEventArgs e)
        {
            var salesWindow = new SalesWindow(adminUsername);
            salesWindow.Closed += (s, ev) => LoadCashAmount();
            salesWindow.ShowDialog();
            LoadTransactionsHistory();
        }

        private void SalesHistoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SalesHistoryDataGrid.SelectedItem is DataRowView selectedRow)
            {
                try
                {
                    int transactionId = Convert.ToInt32(selectedRow["TransactionID"]);

                    DataTable transactionDetails = Queries.GetTransactionDetails(transactionId);

                    var detailsWindow = new TransactionDetailsWindow(transactionDetails, transactionId, adminUsername);
                    detailsWindow.Closed += (s, ev) => LoadCashAmount();
                    detailsWindow.Closed += (s, ev) => LoadTransactionsHistory();
                    detailsWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии окна деталей: {ex.Message}", "Ошибка");
                }
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string helpFilePath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "main_windows",
                    "manager",
                    "Help",
                    "help.html"
                );

                if (System.IO.File.Exists(helpFilePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = helpFilePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Файл справки не найден.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            var Orders = new OrdersWindow(adminUsername);
            Orders.Closed += (s, ev) => Dispatcher.Invoke(LoadCashAmount);
            Orders.Closed += (s, ev) => Dispatcher.Invoke(LoadTransactionsHistory);
            Orders.ShowDialog();
        }
    }
}