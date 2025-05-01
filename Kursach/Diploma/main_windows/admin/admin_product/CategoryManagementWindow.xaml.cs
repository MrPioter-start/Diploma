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

namespace Kursach.main_windows.admin.admin_product
{
    /// <summary>
    /// Логика взаимодействия для CategoryManagementWindow.xaml
    /// </summary>
    public partial class CategoryManagementWindow : Window
    {
        private string createdBy;

        public CategoryManagementWindow(string username)
        {
            InitializeComponent();
            this.createdBy = username;
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = CategoryNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Введите название категории.", "Ошибка");
                return;
            }

            try
            {
                Queries.AddCategory(categoryName, createdBy);
                MessageBox.Show("Категория успешно добавлена.", "Успех");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении категории: {ex.Message}", "Ошибка");
            }
        }
    }
}
