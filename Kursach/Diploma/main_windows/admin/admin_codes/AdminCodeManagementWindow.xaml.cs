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

namespace Kursach.main_windows
{
    /// <summary>
    /// Логика взаимодействия для AdminCodeManagementWindow.xaml
    /// </summary>
    public partial class AdminCodeManagementWindow : Window
    {
        private string adminUsername;

        public AdminCodeManagementWindow(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
        }

        private void CreateOrUpdateCode_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text.Trim();
        
            // Определяем роль
            int roleID = 0;
            if (RoleComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRole = selectedItem.Content.ToString();
        
                switch (selectedRole)
                {
                    case "Менеджер":
                        roleID = 2;
                        break;
                    case "Пользователь":
                        roleID = 3;
                        break;
                }
            }
        
            if (string.IsNullOrEmpty(code) || roleID == 0)
            {
                MessageBox.Show("Введите код и выберите роль.", "Ошибка");
                return;
            }
        
            try
            {
                if (Queries.IsCodeExistsForAdmin(roleID, adminUsername))
                {
                    MessageBox.Show("Код для этой роли уже существует. Нажмите 'Изменить код'.", "Информация");
                    UpdateCodeButton.Visibility = Visibility.Visible; 
                    CreateCodeButton.Visibility = Visibility.Collapsed; 
                }
                else
                {
                    Queries.SaveAccessCode(code, roleID, adminUsername);
                    MessageBox.Show($"Код успешно сохранен", "Успех");
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
        
            int roleID = 0;
            if (RoleComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRole = selectedItem.Content.ToString();
        
                switch (selectedRole)
                {
                    case "Менеджер":
                        roleID = 2;
                        break;
                    case "Пользователь":
                        roleID = 3;
                        break;
                }
            }
        
            if (string.IsNullOrEmpty(code) || roleID == 0)
            {
                MessageBox.Show("Введите код и выберите роль.", "Ошибка");
                return;
            }
        
            try
            {
                Queries.UpdateAccessCode(code, roleID, adminUsername);
                MessageBox.Show($"Код успешно сохранен", "Успех");
        
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
        
    }
}
