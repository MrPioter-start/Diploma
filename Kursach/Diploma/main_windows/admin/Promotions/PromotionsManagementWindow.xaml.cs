using Kursach.Database.WarehouseApp.Database;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Diploma.main_windows.admin.Promotions
{
    public partial class PromotionsManagementWindow : Window
    {
        private readonly string adminUsername;
        private int? editingPromotionId = null;

        public PromotionsManagementWindow(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;

            TargetTypeComboBox.SelectionChanged += TargetTypeComboBox_SelectionChanged;

            LoadPromotions();
            PromotionForm.Visibility = Visibility.Collapsed;
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
            FROM Promotions
            WHERE CreatedBy = @CreatedBy"; 

                var promotionsTable = DatabaseHelper.ExecuteQuery(query, command =>
                {
                    command.Parameters.AddWithValue("@CreatedBy", adminUsername);
                });

                PromotionsDataGrid.ItemsSource = promotionsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке акций: {ex.Message}", "Ошибка");
            }
        }

        private void AddPromotion_Click(object sender, RoutedEventArgs e)
        {
            editingPromotionId = null;
            ClearPromotionForm();
            PromotionForm.Visibility = Visibility.Visible;
        }

        private bool isSaving = false;

        private async void SavePromotionButton_Click(object sender, RoutedEventArgs e)
        {
            if (isSaving) return;

            isSaving = true;
            SavePromotionButton.IsEnabled = false;

            try
            {
                if (editingPromotionId.HasValue)
                {
                    UpdatePromotion(editingPromotionId.Value);
                }
                else
                {
                    AddPromotion();
                }
            }
            finally
            {
                isSaving = false;
                SavePromotionButton.IsEnabled = true;
            }
        }


        private void AddPromotion()
        {
            try
            {
                if (!ValidatePromotionForm(out string promotionName, out decimal discountPercentage, out DateTime startDate, out DateTime endDate, out string targetType, out string targetValue))
                    return;

                // Проверка дубликатов должна быть ДО вставки
                if (IsDuplicatePromotion(promotionName, targetType, targetValue, startDate, endDate))
                {
                    MessageBox.Show("Такая акция уже существует.", "Ошибка");
                    return;
                }

                if (!IsValidTarget(targetType, targetValue))
                {
                    MessageBox.Show($"Указанная {targetType} не существует.", "Ошибка");
                    return;
                }

                string insertPromotionQuery = @"
                    INSERT INTO Promotions (PromotionName, DiscountPercentage, StartDate, EndDate, CreatedBy)
                    VALUES (@PromotionName, @DiscountPercentage, @StartDate, @EndDate, @CreatedBy);
                    SELECT SCOPE_IDENTITY();";

                var result = DatabaseHelper.ExecuteScalar(insertPromotionQuery, command =>
                {
                    command.Parameters.AddWithValue("@PromotionName", promotionName);
                    command.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@CreatedBy", adminUsername);
                });

                int promotionId = Convert.ToInt32(result);

                string insertRuleQuery = @"
                    INSERT INTO PromotionRules (PromotionID, TargetType, TargetValue)
                    VALUES (@PromotionID, @TargetType, @TargetValue)";

                DatabaseHelper.ExecuteNonQuery(insertRuleQuery, command =>
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
                    editingPromotionId = promotionId;

                    string promotionQuery = @"
                        SELECT PromotionName, DiscountPercentage, StartDate, EndDate
                        FROM Promotions
                        WHERE PromotionID = @PromotionID";

                    var promotionData = DatabaseHelper.ExecuteQuery(promotionQuery, command =>
                    {
                        command.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    if (promotionData.Rows.Count == 0)
                    {
                        MessageBox.Show("Акция не найдена.", "Ошибка");
                        return;
                    }

                    var promotionRow = promotionData.Rows[0];
                    PromotionNameTextBox.Text = promotionRow["PromotionName"].ToString();
                    DiscountPercentageTextBox.Text = promotionRow["DiscountPercentage"].ToString();
                    StartDatePicker.SelectedDate = Convert.ToDateTime(promotionRow["StartDate"]);
                    EndDatePicker.SelectedDate = Convert.ToDateTime(promotionRow["EndDate"]);

                    string ruleQuery = @"
                        SELECT TargetType, TargetValue
                        FROM PromotionRules
                        WHERE PromotionID = @PromotionID";

                    var ruleData = DatabaseHelper.ExecuteQuery(ruleQuery, command =>
                    {
                        command.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    if (ruleData.Rows.Count > 0)
                    {
                        var ruleRow = ruleData.Rows[0];
                        string targetType = ruleRow["TargetType"].ToString();
                        string targetValue = ruleRow["TargetValue"].ToString();

                        foreach (ComboBoxItem item in TargetTypeComboBox.Items)
                        {
                            if (item.Content.ToString() == targetType)
                            {
                                TargetTypeComboBox.SelectedItem = item;
                                break;
                            }
                        }

                        TargetValueComboBox.SelectedItem = null; 
                        foreach (var obj in TargetValueComboBox.Items)
                        {
                            if (obj.ToString() == targetValue)
                            {
                                TargetValueComboBox.SelectedItem = obj;
                                break;
                            }
                        }
                    }

                    PromotionForm.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Выберите акцию для редактирования.", "Внимание");
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
                if (!ValidatePromotionForm(out string promotionName, out decimal discountPercentage, out DateTime startDate, out DateTime endDate, out string targetType, out string targetValue))
                    return;

                if (!IsValidTarget(targetType, targetValue))
                {
                    MessageBox.Show($"Указанная {targetType} не существует.", "Ошибка");
                    return;
                }

                if (IsDuplicatePromotionExceptCurrent(promotionId, promotionName, targetType, targetValue, startDate, endDate))
                {
                    MessageBox.Show("Такая акция уже существует.", "Ошибка");
                    return;
                }

                string updatePromotionQuery = @"
                    UPDATE Promotions
                    SET PromotionName = @PromotionName,
                        DiscountPercentage = @DiscountPercentage,
                        StartDate = @StartDate,
                        EndDate = @EndDate
                    WHERE PromotionID = @PromotionID";

                DatabaseHelper.ExecuteNonQuery(updatePromotionQuery, command =>
                {
                    command.Parameters.AddWithValue("@PromotionID", promotionId);
                    command.Parameters.AddWithValue("@PromotionName", promotionName);
                    command.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                });

                string updateRuleQuery = @"
                    UPDATE PromotionRules
                    SET TargetType = @TargetType,
                        TargetValue = @TargetValue
                    WHERE PromotionID = @PromotionID";

                DatabaseHelper.ExecuteNonQuery(updateRuleQuery, command =>
                {
                    command.Parameters.AddWithValue("@PromotionID", promotionId);
                    command.Parameters.AddWithValue("@TargetType", targetType);
                    command.Parameters.AddWithValue("@TargetValue", targetValue);
                });

                MessageBox.Show("Акция успешно обновлена.", "Успех");
                LoadPromotions();
                PromotionForm.Visibility = Visibility.Collapsed;
                editingPromotionId = null;
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

                    string deleteRulesQuery = "DELETE FROM PromotionRules WHERE PromotionID = @PromotionID";
                    DatabaseHelper.ExecuteNonQuery(deleteRulesQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    string deletePromotionQuery = "DELETE FROM Promotions WHERE PromotionID = @PromotionID";
                    DatabaseHelper.ExecuteNonQuery(deletePromotionQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@PromotionID", promotionId);
                    });

                    MessageBox.Show("Акция успешно удалена.", "Успех");
                    LoadPromotions();
                }
                else
                {
                    MessageBox.Show("Выберите акцию для удаления.", "Внимание");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении акции: {ex.Message}", "Ошибка");
            }
        }

        private void ClearPromotionForm()
        {
            PromotionNameTextBox.Text = "";
            DiscountPercentageTextBox.Text = "";
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            TargetTypeComboBox.SelectedItem = null;
            TargetValueComboBox.Items.Clear();
        }

        private bool ValidatePromotionForm(out string promotionName, out decimal discountPercentage, out DateTime startDate, out DateTime endDate, out string targetType, out string targetValue)
        {
            promotionName = PromotionNameTextBox.Text.Trim();
            targetType = (TargetTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            targetValue = TargetValueComboBox.SelectedItem?.ToString();

            discountPercentage = 0;
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            if (string.IsNullOrEmpty(promotionName))
            {
                MessageBox.Show("Введите название акции.", "Ошибка");
                return false;
            }

            if (!decimal.TryParse(DiscountPercentageTextBox.Text, out discountPercentage) || discountPercentage < 0 || discountPercentage > 100)
            {
                MessageBox.Show("Введите процент скидки от 0 до 100.", "Ошибка");
                return false;
            }


            if (!StartDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату начала акции.", "Ошибка");
                return false;
            }
            startDate = StartDatePicker.SelectedDate.Value;

            if (!EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату окончания акции.", "Ошибка");
                return false;
            }
            endDate = EndDatePicker.SelectedDate.Value;

            if (endDate < startDate)
            {
                MessageBox.Show("Дата окончания акции не может быть раньше даты начала.", "Ошибка");
                return false;
            }

            if (string.IsNullOrEmpty(targetType))
            {
                MessageBox.Show("Выберите тип цели акции.", "Ошибка");
                return false;
            }

            if (string.IsNullOrEmpty(targetValue))
            {
                MessageBox.Show("Выберите значение цели акции.", "Ошибка");
                return false;
            }

            return true;
        }

        private void ClosePromotionForm_Click(object sender, RoutedEventArgs e)
        {
            PromotionForm.Visibility = Visibility.Collapsed;
        }


        private void TargetTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TargetValueComboBox.Items.Clear();

            if (TargetTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string targetType = selectedItem.Content.ToString();
                string query = targetType switch
                {
                    "Товар" => "SELECT DISTINCT Name FROM Products",
                    "Бренд" => "SELECT DISTINCT Brand FROM Products",
                    "Категория" => "SELECT DISTINCT CategoryName FROM Categories",
                    _ => null
                };

                if (query != null)
                {
                    var resultTable = DatabaseHelper.ExecuteQuery(query, cmd => { });
                    foreach (DataRow row in resultTable.Rows)
                    {
                        string val = row[0]?.ToString();
                        if (!string.IsNullOrEmpty(val))
                            TargetValueComboBox.Items.Add(val);
                    }
                }
            }
        }

        private bool IsDuplicatePromotion(string promotionName, string targetType, string targetValue, DateTime startDate, DateTime endDate)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Promotions p
                JOIN PromotionRules r ON p.PromotionID = r.PromotionID
                WHERE 
                    p.PromotionName = @PromotionName AND
                    r.TargetType = @TargetType AND
                    r.TargetValue = @TargetValue AND
                    p.StartDate = @StartDate AND
                    p.EndDate = @EndDate";

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@PromotionName", promotionName);
                cmd.Parameters.AddWithValue("@TargetType", targetType);
                cmd.Parameters.AddWithValue("@TargetValue", targetValue);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
            }));

            return count > 0;
        }

        private bool IsDuplicatePromotionExceptCurrent(int promotionId, string promotionName, string targetType, string targetValue, DateTime startDate, DateTime endDate)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Promotions p
                JOIN PromotionRules r ON p.PromotionID = r.PromotionID
                WHERE 
                    p.PromotionID <> @PromotionID AND
                    p.PromotionName = @PromotionName AND
                    r.TargetType = @TargetType AND
                    r.TargetValue = @TargetValue AND
                    p.StartDate = @StartDate AND
                    p.EndDate = @EndDate";

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@PromotionID", promotionId);
                cmd.Parameters.AddWithValue("@PromotionName", promotionName);
                cmd.Parameters.AddWithValue("@TargetType", targetType);
                cmd.Parameters.AddWithValue("@TargetValue", targetValue);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
            }));

            return count > 0;
        }

        private bool IsValidTarget(string targetType, string targetValue)
        {
            string query = targetType switch
            {
                "Товар" => "SELECT COUNT(*) FROM Products WHERE Name = @TargetValue",
                "Бренд" => "SELECT COUNT(*) FROM Products WHERE Brand = @TargetValue",
                "Категория" => "SELECT COUNT(*) FROM Categories WHERE CategoryName = @TargetValue",
                _ => null
            };

            if (query == null) return false;

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@TargetValue", targetValue);
            }));

            return count > 0;
        }
    }
}
