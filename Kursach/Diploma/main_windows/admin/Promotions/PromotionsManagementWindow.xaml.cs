using Kursach.Database.WarehouseApp.Database;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Diploma.main_windows.admin.Promotions
{
    /// <summary>
    /// Логика взаимодействия для PromotionsManagementWindow.xaml
    /// </summary>
    public partial class PromotionsManagementWindow : Window
    {
        private string adminUsername;

        public PromotionsManagementWindow(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
            LoadPromotions();
        }

        private void LoadPromotions()
        {
            try
            {
                string query = @"
                    SELECT 
                        PromotionID,
                        PromotionName,
                        DiscountPercentage,
                        StartDate,
                        EndDate,
                        CreatedBy
                    FROM 
                        Promotions";

                var promotionsTable = DatabaseHelper.ExecuteQuery(query, command => { });
                PromotionsDataGrid.ItemsSource = promotionsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке акций: {ex.Message}", "Ошибка");
            }
        }

        private void AddPromotion_Click(object sender, RoutedEventArgs e)
        {
            PromotionForm.Visibility = Visibility.Visible;
        }

        private void SavePromotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string promotionName = PromotionNameTextBox.Text.Trim();
                decimal discountPercentage = Convert.ToDecimal(DiscountPercentageTextBox.Text);
                DateTime startDate = StartDatePicker.SelectedDate.Value;
                DateTime endDate = EndDatePicker.SelectedDate.Value;

                // Проверяем, выбран ли тип цели акции
                if (TargetTypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип цели акции.", "Ошибка");
                    return;
                }

                string targetType = (TargetTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                string targetValue = TargetValueTextBox.Text.Trim();

                if (string.IsNullOrEmpty(promotionName) || discountPercentage <= 0 || startDate >= endDate || string.IsNullOrEmpty(targetType) || string.IsNullOrEmpty(targetValue))
                {
                    MessageBox.Show("Проверьте введенные данные.", "Ошибка");
                    return;
                }

                // Проверяем, существует ли целевая сущность
                bool isValidTarget = false;

                switch (targetType)
                {
                    case "Товар":
                        isValidTarget = ProductExists(targetValue);
                        break;
                    case "Бренд":
                        isValidTarget = BrandExists(targetValue);
                        break;
                    case "Категория":
                        isValidTarget = CategoryExists(targetValue);
                        break;
                }

                if (!isValidTarget)
                {
                    MessageBox.Show($"Указанная {targetType} не существует.", "Ошибка");
                    return;
                }

                // Добавляем акцию
                string promotionQuery = @"
            INSERT INTO Promotions (PromotionName, DiscountPercentage, StartDate, EndDate, CreatedBy)
            VALUES (@PromotionName, @DiscountPercentage, @StartDate, @EndDate, @CreatedBy);
            SELECT SCOPE_IDENTITY();";

                int promotionId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(promotionQuery, command =>
                {
                    command.Parameters.AddWithValue("@PromotionName", promotionName);
                    command.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@CreatedBy", adminUsername);
                }));

                // Добавляем правило акции
                string ruleQuery = @"
            INSERT INTO PromotionRules (PromotionID, TargetType, TargetValue)
            VALUES (@PromotionID, @TargetType, @TargetValue)";

                DatabaseHelper.ExecuteNonQuery(ruleQuery, command =>
                {
                    command.Parameters.AddWithValue("@PromotionID", promotionId);
                    command.Parameters.AddWithValue("@TargetType", targetType);
                    command.Parameters.AddWithValue("@TargetValue", targetValue);
                });

                MessageBox.Show("Акция успешно добавлена.", "Успех");
                LoadPromotions();
                PromotionForm.Visibility = Visibility.Collapsed;
            }
            catch (FormatException)
            {
                MessageBox.Show("Некорректный формат данных. Проверьте введенные значения.", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении акции: {ex.Message}", "Ошибка");
            }
        }

        private void EditPromotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PromotionsDataGrid.SelectedItem is DataRowView selectedRow)
                {
                    int promotionId = Convert.ToInt32(selectedRow["PromotionID"]);

                    // Загружаем данные акции из таблицы Promotions
                    string promotionQuery = @"
                SELECT 
                    PromotionName, 
                    DiscountPercentage, 
                    StartDate, 
                    EndDate 
                FROM 
                    Promotions 
                WHERE 
                    PromotionID = @PromotionID";

                    var promotionData = DatabaseHelper.ExecuteQuery(promotionQuery, command =>
                    {
                        command.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    if (promotionData.Rows.Count == 0)
                    {
                        MessageBox.Show("Акция не найдена.", "Ошибка");
                        return;
                    }

                    // Заполняем поля формы данными из таблицы Promotions
                    DataRow promotionRow = promotionData.Rows[0];
                    PromotionNameTextBox.Text = promotionRow["PromotionName"].ToString();
                    DiscountPercentageTextBox.Text = promotionRow["DiscountPercentage"].ToString();
                    StartDatePicker.SelectedDate = Convert.ToDateTime(promotionRow["StartDate"]);
                    EndDatePicker.SelectedDate = Convert.ToDateTime(promotionRow["EndDate"]);

                    // Загружаем правила акции из таблицы PromotionRules
                    string ruleQuery = @"
                SELECT 
                    TargetType, 
                    TargetValue 
                FROM 
                    PromotionRules 
                WHERE 
                    PromotionID = @PromotionID";

                    var ruleData = DatabaseHelper.ExecuteQuery(ruleQuery, command =>
                    {
                        command.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    if (ruleData.Rows.Count > 0)
                    {
                        // Заполняем поля "Применить к" и "Значение"
                        DataRow ruleRow = ruleData.Rows[0];
                        string targetType = ruleRow["TargetType"].ToString();
                        string targetValue = ruleRow["TargetValue"].ToString();

                        // Устанавливаем значение в ComboBox "Применить к"
                        foreach (ComboBoxItem item in TargetTypeComboBox.Items)
                        {
                            if (item.Content.ToString() == targetType)
                            {
                                TargetTypeComboBox.SelectedItem = item;
                                break;
                            }
                        }

                        // Устанавливаем значение в TextBox "Значение"
                        TargetValueTextBox.Text = targetValue;
                    }

                    // Показываем форму редактирования акции
                    PromotionForm.Visibility = Visibility.Visible;

                    // Обновляем кнопку сохранения
                    SavePromotionButton.Click -= SavePromotion_Click;
                    SavePromotionButton.Click += (s, ev) => UpdatePromotion(promotionId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании акции: {ex.Message}", "Ошибка");
            }
        }

        private void UpdatePromotion(int promotionId)
        {
            try
            {
                string promotionName = PromotionNameTextBox.Text.Trim();
                decimal discountPercentage = Convert.ToDecimal(DiscountPercentageTextBox.Text);
                DateTime startDate = StartDatePicker.SelectedDate.Value;
                DateTime endDate = EndDatePicker.SelectedDate.Value;
                string targetType = (TargetTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                string targetValue = TargetValueTextBox.Text.Trim();

                // Обновляем данные акции в таблице Promotions
                string updatePromotionQuery = @"
            UPDATE Promotions 
            SET 
                PromotionName = @PromotionName, 
                DiscountPercentage = @DiscountPercentage, 
                StartDate = @StartDate, 
                EndDate = @EndDate 
            WHERE 
                PromotionID = @PromotionID";

                DatabaseHelper.ExecuteNonQuery(updatePromotionQuery, command =>
                {
                    command.Parameters.AddWithValue("@PromotionID", promotionId);
                    command.Parameters.AddWithValue("@PromotionName", promotionName);
                    command.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                });

                // Обновляем правила акции в таблице PromotionRules
                string updateRuleQuery = @"
            UPDATE PromotionRules 
            SET 
                TargetType = @TargetType, 
                TargetValue = @TargetValue 
            WHERE 
                PromotionID = @PromotionID";

                DatabaseHelper.ExecuteNonQuery(updateRuleQuery, command =>
                {
                    command.Parameters.AddWithValue("@PromotionID", promotionId);
                    command.Parameters.AddWithValue("@TargetType", targetType);
                    command.Parameters.AddWithValue("@TargetValue", targetValue);
                });

                MessageBox.Show("Акция успешно обновлена.", "Успех");
                LoadPromotions();
                PromotionForm.Visibility = Visibility.Collapsed;
            }
            catch (FormatException)
            {
                MessageBox.Show("Некорректный формат данных. Проверьте введенные значения.", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении акции: {ex.Message}", "Ошибка");
            }
        }

        private void DeletePromotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PromotionsDataGrid.SelectedItem is DataRowView selectedRow)
                {
                    int promotionId = Convert.ToInt32(selectedRow["PromotionID"]);

                    string deleteRulesQuery = @"
                DELETE FROM PromotionRules
                WHERE PromotionID = @PromotionID";

                    DatabaseHelper.ExecuteNonQuery(deleteRulesQuery, command =>
                    {
                        command.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    string deletePromotionQuery = @"
                DELETE FROM Promotions
                WHERE PromotionID = @PromotionID";

                    DatabaseHelper.ExecuteNonQuery(deletePromotionQuery, command =>
                    {
                        command.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    MessageBox.Show("Акция успешно удалена.", "Успех");
                    LoadPromotions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении акции: {ex.Message}", "Ошибка");
            }
        }

        public static bool BrandExists(string brandName)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM Products 
        WHERE LOWER(LTRIM(RTRIM(Brand)) COLLATE SQL_Latin1_General_CP1_CI_AS) = LOWER(LTRIM(RTRIM(@BrandName)))";

            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@BrandName", brandName);
            }) is int count && count > 0;
        }

        public static bool ProductExists(string productName)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM Products 
        WHERE LOWER(LTRIM(RTRIM(Name)) COLLATE SQL_Latin1_General_CP1_CI_AS) = LOWER(LTRIM(RTRIM(@ProductName)))";

            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@ProductName", productName);
            }) is int count && count > 0;
        }

        public static bool CategoryExists(string categoryName)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM Categories 
        WHERE LOWER(LTRIM(RTRIM(CategoryName)) COLLATE SQL_Latin1_General_CP1_CI_AS) = LOWER(LTRIM(RTRIM(@CategoryName)))";

            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@CategoryName", categoryName);
            }) is int count && count > 0;
        }
    }
}