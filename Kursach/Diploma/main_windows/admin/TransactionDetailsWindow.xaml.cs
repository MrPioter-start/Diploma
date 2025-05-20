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

namespace Diploma.main_windows.admin
{
    /// <summary>
    /// Логика взаимодействия для TransactionDetailsWindow.xaml
    /// </summary>
    public partial class TransactionDetailsWindow : Window
    {
        public TransactionDetailsWindow(DataTable transactionDetails)
        {
            InitializeComponent();

            TransactionDetailsDataGrid.ItemsSource = transactionDetails.DefaultView;
        }
    }
}
