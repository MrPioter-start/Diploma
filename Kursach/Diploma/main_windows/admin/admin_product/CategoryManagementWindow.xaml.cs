using Kursach.Database;
using Kursach.Database.WarehouseApp.Database;
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
                if (IsCategoryExists(categoryName))
                {
                    MessageBox.Show("Категория с таким названием уже существует.", "Ошибка");
                    return;
                }

                Queries.AddCategory(categoryName, createdBy);
                MessageBox.Show("Категория успешно добавлена.", "Успех");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении категории: {ex.Message}", "Ошибка");
            }
        }

        private bool IsCategoryExists(string categoryName)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM Categories 
        WHERE CategoryName = @CategoryName AND CreatedBy = @CreatedBy";

            int count = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@CategoryName", categoryName);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
            }) is int result ? result : 0;

            return count > 0;
        }
    }
}
