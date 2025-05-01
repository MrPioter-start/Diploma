using Kursach.Database;
using Kursach.Database.WarehouseApp.Database;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.main_windows
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private int? accessCodeID;
        private DataTable originalTable;

        public UserWindow(string username)
        {
            InitializeComponent();
            this.accessCodeID = Queries.GetUserAccessCodeID(username); 
            LoadProducts(username);
        }

        private void LoadProducts(string username)
        {
            int? accessCodeID = Queries.GetUserAccessCodeID(username);
            if (!accessCodeID.HasValue)
            {
                MessageBox.Show("Код доступа не найден. Обратитесь к администратору.", "Ошибка");
                return;
            }

            string adminUsername = GetAdminByAccessCodeID(accessCodeID.Value);
            if (string.IsNullOrEmpty(adminUsername))
            {
                MessageBox.Show("Код доступа не связан с администратором.", "Ошибка");
                return;
            }

            originalTable = Queries.GetProductsByAdmin(adminUsername); 
            ProductsDataGrid.ItemsSource = originalTable.DefaultView;

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                ProductsDataGrid.ItemsSource = originalTable.DefaultView;
                return;
            }

            var filteredRows = originalTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(field => field.ToString().ToLower().Contains(searchQuery)))
                .ToList(); 

            if (filteredRows.Count == 0)
            {
                ProductsDataGrid.ItemsSource = null; 
                return;
            }

            DataTable filteredTable = filteredRows.CopyToDataTable();
            ProductsDataGrid.ItemsSource = filteredTable.DefaultView;
        }

        private string GetAdminByAccessCodeID(int accessCodeID)
        {
            string query = @"
            SELECT CreatedBy 
            FROM AccessCodes 
            WHERE CodeID = @CodeID";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@CodeID", accessCodeID);
            });

            return result?.ToString();
        }

    }
}
