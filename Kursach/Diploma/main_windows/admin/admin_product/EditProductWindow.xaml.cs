using Kursach.Database;
using System.Data;
using System.Windows;

namespace Kursach.main_windows.admin
{
    /// <summary>
    /// Логика взаимодействия для EditProductWindow.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        private string originalName;
        private string createdBy;

        public EditProductWindow(string name, string category, decimal price, int quantity,
                         string size, string composition, string shelfLife, string deliveryTime, string adminUsername,
                         string brand, int minStockLevel)
        {
            InitializeComponent();
            this.originalName = name;
            this.createdBy = adminUsername;

            NameTextBox.Text = name;
            PriceTextBox.Text = price.ToString();
            QuantityTextBox.Text = quantity.ToString();
            SizeTextBox.Text = size;
            CompositionTextBox.Text = composition;
            ShelfLifeTextBox.Text = shelfLife;
            DeliveryTimeTextBox.Text = deliveryTime;
            BrandTextBox.Text = brand; 
            MinStockLevelTextBox.Text = minStockLevel.ToString(); 

            LoadCategories(category);
        }
        private void LoadCategories(string selectedCategory)
        {
            DataTable categoriesTable = Queries.GetCategories(createdBy);
            CategoryComboBox.ItemsSource = categoriesTable.DefaultView;
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            CategoryComboBox.SelectedValuePath = "CategoryID";

            // Выбор текущей категории
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                foreach (DataRowView row in categoriesTable.DefaultView)
                {
                    if (row["CategoryName"].ToString() == selectedCategory)
                    {
                        CategoryComboBox.SelectedItem = row;
                        break;
                    }
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string newName = NameTextBox.Text.Trim();
            string newSize = SizeTextBox.Text.Trim();
            string newComposition = CompositionTextBox.Text.Trim();
            string newBrand = BrandTextBox.Text.Trim();

            if (CategoryComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка");
                return;
            }

            int categoryID = (int)CategoryComboBox.SelectedValue;

            if (!decimal.TryParse(PriceTextBox.Text, out decimal newPrice) || newPrice <= 0)
            {
                MessageBox.Show("Введите корректную цену.", "Ошибка");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int newQuantity) || newQuantity < 0)
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Введите название товара.", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(newBrand))
            {
                MessageBox.Show("Введите бренд товара.", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(newSize))
            {
                MessageBox.Show("Введите размер товара.", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(newComposition))
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

            if (!int.TryParse(MinStockLevelTextBox.Text, out int newMinStockLevel) || newMinStockLevel < 0)
            {
                MessageBox.Show("Введите корректное минимальное количество.", "Ошибка");
                return;
            }

            try
            {
                Queries.UpdateProduct(
                    originalName: originalName,
                    newName: newName,
                    categoryID: categoryID,
                    price: newPrice,
                    quantity: newQuantity,
                    size: newSize,
                    composition: newComposition,
                    shelfLife: shelfLifeValue.ToString(),
                    deliveryTime: deliveryTimeValue.ToString(),
                    createdBy: createdBy,
                    minStockLevel: newMinStockLevel,
                    brand: newBrand
                );

                MessageBox.Show("Товар успешно обновлен.", "Успех");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении товара: {ex.Message}", "Ошибка");
            }
        }

    }
}
