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

namespace Kursach.main_windows
{
    /// <summary>
    /// Логика взаимодействия для UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        private string adminUsername;

        public UserManagementWindow(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
            LoadUsers();
        }

        private void LoadUsers()
        {
            DataTable usersTable = Queries.GetUsersByAdmin(adminUsername);

            UsersDataGrid.ItemsSource = usersTable.DefaultView;
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string username = selectedRow["Username"].ToString();

                try
                {
                    Queries.DeleteUser(username);
                    MessageBox.Show($"Пользователь {username} успешно удален.", "Успех");

                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка");
                }
            }
        }
    }
}
