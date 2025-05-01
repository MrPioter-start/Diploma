using Kursach.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diploma.main_windows.admin.Reportes
{
    /// <summary>
    /// Логика взаимодействия для ReportsHistoryWindow.xaml
    /// </summary>
    public partial class ReportsHistoryWindow : Window
    {
        private string adminUsername;

        public ReportsHistoryWindow(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                DataTable reportsTable = Queries.GetReports(adminUsername);

                if (reportsTable == null || reportsTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет доступных отчетов.", "Информация");
                    return;
                }

                ReportsDataGrid.ItemsSource = reportsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
