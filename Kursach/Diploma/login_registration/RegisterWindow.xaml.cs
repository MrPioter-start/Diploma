using Kursach.Database;
using System;
using System.Collections.Generic;
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

namespace Kursach.login_registration
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private string selectedRole;

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Hide();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string accessCode = AccessCodeTextBox.Text.Trim();

            int roleID = 0;
            if (RoleComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRole = selectedItem.Content.ToString();

                switch (selectedRole)
                {
                    case "Администратор":
                        roleID = 1;
                        break;
                    case "Менеджер":
                        roleID = 2;
                        break;
                }
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка");
                return;
            }

            try
            {
                Queries.RegisterUser(username, password, roleID, roleID == 1 ? null : accessCode);
                MessageBox.Show($"Пользователь {username} успешно зарегистрирован!", "Успех");

                var loginWindow = new LoginWindow(); 
                loginWindow.Show();
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка");
            }
        }
        
        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedRole = selectedItem.Content.ToString();

                bool showCodeInput = selectedRole == "Менеджер";
                CodeLabel.Visibility = showCodeInput ? Visibility.Visible : Visibility.Collapsed;
                AccessCodeTextBox.Visibility = showCodeInput ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Close_app(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
