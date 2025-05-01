using Kursach.Database;
using Kursach.main_windows.admin;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Kursach.main_windows
{
    public partial class ManagerWindow : Window
    {
        private string adminUsername;

        public ManagerWindow(string managerUsername)
        {
            InitializeComponent();
            this.adminUsername = Queries.GetAdminForManager(managerUsername); 
            LoadCashAmount();
            LoadSalesHistory();
        }

        private void LoadCashAmount()
        {
            decimal amount = Queries.GetCurrentCashAmount(adminUsername); 
            CashAmountTextBlock.Text = $"{amount:F2} ₽";
        }

        private void OpenCashManagement_Click(object sender, RoutedEventArgs e)
        {
            var cashWindow = new CashManagementWindow(adminUsername);
            cashWindow.Closed += (s, ev) => LoadCashAmount();
            cashWindow.ShowDialog();
        }

        private void LoadSalesHistory()
        {
            DataTable salesTable = Queries.GetSalesHistory(adminUsername);
            SalesHistoryDataGrid.ItemsSource = salesTable.DefaultView;
        }

        private void OpenSalesMenu_Click(object sender, RoutedEventArgs e)
        {
            var salesWindow = new SalesWindow(adminUsername);
            salesWindow.Closed += (s, ev) => LoadCashAmount();
            salesWindow.ShowDialog();
            LoadSalesHistory();
        }

        private void SalesHistoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SalesHistoryDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int saleId = Convert.ToInt32(selectedRow["SaleID"]);

                var returnWindow = new ReturnWindow(saleId, adminUsername);
                returnWindow.Closed += (s, ev) => LoadCashAmount();
                if (returnWindow.ShowDialog() == true)
                {
                    LoadSalesHistory();
                }
            }
        }

    }
}