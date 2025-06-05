using Kursach.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing;
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
    /// Логика взаимодействия для AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private string createdBy;

        public AddProductWindow(string adminUsername)
        {
            InitializeComponent();
            this.createdBy = adminUsername;
            LoadCategories();
        }

        private void LoadCategories()
        {
            DataTable categoriesTable = Queries.GetCategories(createdBy);
            CategoryComboBox.ItemsSource = categoriesTable.DefaultView;
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            CategoryComboBox.SelectedValuePath = "CategoryID";
        }

        [Obsolete]
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string size = SizeTextBox.Text.Trim();
            string composition = CompositionTextBox.Text.Trim();
            string brand = BrandTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название товара.", "Ошибка");
                return;
            }

            if (CategoryComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(brand))
            {
                MessageBox.Show("Введите бренд товара.", "Ошибка");
                return;
            }

            int categoryID = (int)CategoryComboBox.SelectedValue;

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число).", "Ошибка");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество (неотрицательное число).", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(size))
            {
                MessageBox.Show("Введите размер товара.", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(composition))
            {
                MessageBox.Show("Введите состав товара.", "Ошибка");
                return;
            }

            if (!int.TryParse(ShelfLifeTextBox.Text, out int shelfLifeValue) || shelfLifeValue < 0)
            {
                MessageBox.Show("Введите корректный срок годности (неотрицательное число).", "Ошибка");
                return;
            }

            if (!int.TryParse(DeliveryTimeTextBox.Text, out int deliveryTimeValue) || deliveryTimeValue < 0)
            {
                MessageBox.Show("Введите корректное время доставки (неотрицательное число).", "Ошибка");
                return;
            }

            if (!int.TryParse(MinStockLevelTextBox.Text, out int minStockLevel) || minStockLevel < 0)
            {
                MessageBox.Show("Введите корректное минимальное количество.", "Ошибка");
                return;
            }

            try
            {
                Queries.AddProduct(
                    name: name,
                    categoryID: categoryID,
                    price: price,
                    quantity: quantity,
                    size: size,
                    composition: composition,
                    shelfLife: shelfLifeValue.ToString(),
                    deliveryTime: deliveryTimeValue.ToString(),
                    createdBy: createdBy,
                    minStockLevel: minStockLevel,
                    brand: brand
                );

                MessageBox.Show("Товар успешно добавлен.", "Успех");

                NameTextBox.Clear();
                PriceTextBox.Clear();
                QuantityTextBox.Clear();
                BrandTextBox.Text = "";
                MinStockLevelTextBox.Clear();
                SizeTextBox.Clear();
                CompositionTextBox.Clear();
                ShelfLifeTextBox.Clear();
                DeliveryTimeTextBox.Clear();
                CategoryComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка");
            }
        }
    }
}
