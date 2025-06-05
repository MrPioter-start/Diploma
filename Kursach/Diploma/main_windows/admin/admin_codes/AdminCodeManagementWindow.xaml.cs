using Kursach.Database;
using System;
using System.Windows;

namespace Kursach.main_windows
{
    /// <summary>
    /// Логика взаимодействия для AdminCodeManagementWindow.xaml
    /// </summary>
    public partial class AdminCodeManagementWindow : Window
    {
        private string adminUsername;
        private const int ManagerRoleID = 2;

        public AdminCodeManagementWindow(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
        }

        private void CreateOrUpdateCode_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Введите код.", "Ошибка");
                return;
            }

            try
            {
                if (Queries.IsCodeExistsForAdmin(ManagerRoleID, adminUsername))
                {
                    MessageBox.Show("Код уже существует. Нажмите 'Изменить код'.", "Информация");
                    UpdateCodeButton.Visibility = Visibility.Visible;
                    CreateCodeButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Queries.SaveAccessCode(code, ManagerRoleID, adminUsername);
                    MessageBox.Show("Код успешно сохранен", "Успех");
                }

                CodeTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании кода: {ex.Message}", "Ошибка");
            }
        }

        private void UpdateCode_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Введите код.", "Ошибка");
                return;
            }

            try
            {
                if (!Queries.IsCodeExistsForAdmin(ManagerRoleID, adminUsername))
                {
                    MessageBox.Show("Код для менеджера не найден. Сначала создайте код.", "Ошибка");
                    return;
                }

                Queries.UpdateAccessCode(code, ManagerRoleID, adminUsername);
                MessageBox.Show("Код успешно обновлён", "Успех");

                CodeTextBox.Clear();
                UpdateCodeButton.Visibility = Visibility.Collapsed;
                CreateCodeButton.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении кода: {ex.Message}", "Ошибка");
            }
        }


        private void ViewCodes_Click(object sender, RoutedEventArgs e)
        {
            var viewCodesWindow = new ViewCodesWindow(adminUsername);
            viewCodesWindow.ShowDialog();
        }

        private void UserManagement(object sender, RoutedEventArgs e)
        {
            var userManagement = new UserManagementWindow(adminUsername);
            userManagement.ShowDialog();
        }
    }
}
