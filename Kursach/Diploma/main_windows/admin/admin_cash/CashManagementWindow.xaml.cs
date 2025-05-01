using Kursach.Database;
using Kursach.main_windows.admin;
using System.Windows;
using System.Windows.Controls;
namespace Kursach.main_windows
{
    /// <summary>
    /// Логика взаимодействия для CashManagementWindow.xaml
    /// </summary>
    public partial class CashManagementWindow : Window
    {
        private string adminUsername;

        public CashManagementWindow(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
        }

        private void ExecuteOperation_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму.", "Ошибка");
                return;
            }

            string operationType = (OperationTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Пополнение";

            try
            {
                if (operationType == "Снятие")
                {
                    decimal currentCash = Queries.GetCurrentCashAmount(adminUsername);
                    if (currentCash < amount)
                    {
                        MessageBox.Show("Недостаточно средств в кассе.", "Ошибка");
                        return;
                    }

                    amount *= -1; 
                }

                Queries.UpdateCashAmount(amount, operationType, adminUsername);
                MessageBox.Show("Операция выполнена успешно.", "Успех");
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void OpenCashHistory_Click(object sender, RoutedEventArgs e)
        {
            var cashHistoryWindow = new CashHistoryWindow(adminUsername);
            cashHistoryWindow.ShowDialog();
        }

        private void OpenOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            var orderHistoryWindow = new ReturnHistoryWindow(adminUsername);
            orderHistoryWindow.ShowDialog();
        }
    }
}
