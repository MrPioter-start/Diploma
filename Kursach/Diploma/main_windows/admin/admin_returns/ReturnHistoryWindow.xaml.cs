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

namespace Kursach.main_windows.admin
{
    /// <summary>
    /// Логика взаимодействия для ReturnHistoryWindow.xaml
    /// </summary>
    public partial class ReturnHistoryWindow : Window
    {
        private string adminUsername;

        public ReturnHistoryWindow(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
            LoadReturnHistory();
        }

        private void LoadReturnHistory()
        {
            DataTable returnHistoryTable = Queries.GetReturnHistory(adminUsername);
            ReturnsDataGrid.ItemsSource = returnHistoryTable.DefaultView;
        }
    }
}
