using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Kursach.Database.WarehouseApp.Database;

namespace Diploma.main_windows.admin.Promotions
{
    public partial class LoyaltyLevelsWindow : Window
    {
        private DataTable loyaltyLevelsTable;
        private DataRow editingRow = null;

        public LoyaltyLevelsWindow()
        {
            InitializeComponent();
            LoadLoyaltyLevels();
        }

        private void LoadLoyaltyLevels()
        {
            try
            {
                string query = "SELECT LoyaltyLevelID, LevelName, MinOrderAmount, DiscountPercentage FROM LoyaltyLevels";
                loyaltyLevelsTable = DatabaseHelper.ExecuteQuery(query, null);
                loyaltyLevelsTable.PrimaryKey = new[] { loyaltyLevelsTable.Columns["LoyaltyLevelID"] };
                LoyaltyLevelsDataGrid.ItemsSource = loyaltyLevelsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке уровней: " + ex.Message);
            }
        }

        // Удалён метод AddRow_Click — не добавляем пустые строки напрямую

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (LoyaltyLevelsDataGrid.SelectedItem is DataRowView rowView)
            {
                if (MessageBox.Show("Удалить выбранный уровень лояльности?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    rowView.Row.Delete();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (DataRow row in loyaltyLevelsTable.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        string insertQuery = @"INSERT INTO LoyaltyLevels (LevelName, MinOrderAmount, DiscountPercentage)
                                               VALUES (@name, @min, @discount)";
                        DatabaseHelper.ExecuteNonQuery(insertQuery, cmd =>
                        {
                            cmd.Parameters.AddWithValue("@name", row["LevelName"]);
                            cmd.Parameters.AddWithValue("@min", Convert.ToDecimal(row["MinOrderAmount"]));
                            cmd.Parameters.AddWithValue("@discount", Convert.ToDecimal(row["DiscountPercentage"]));
                        });
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        string updateQuery = @"UPDATE LoyaltyLevels 
                                               SET LevelName = @name, MinOrderAmount = @min, DiscountPercentage = @discount
                                               WHERE LoyaltyLevelID = @id";
                        DatabaseHelper.ExecuteNonQuery(updateQuery, cmd =>
                        {
                            cmd.Parameters.AddWithValue("@name", row["LevelName"]);
                            cmd.Parameters.AddWithValue("@min", Convert.ToDecimal(row["MinOrderAmount"]));
                            cmd.Parameters.AddWithValue("@discount", Convert.ToDecimal(row["DiscountPercentage"]));
                            cmd.Parameters.AddWithValue("@id", row["LoyaltyLevelID"]);
                        });
                    }
                    else if (row.RowState == DataRowState.Deleted)
                    {
                        string deleteQuery = "DELETE FROM LoyaltyLevels WHERE LoyaltyLevelID = @id";
                        DatabaseHelper.ExecuteNonQuery(deleteQuery, cmd =>
                        {
                            cmd.Parameters.AddWithValue("@id", row["LoyaltyLevelID", DataRowVersion.Original]);
                        });
                    }
                }

                MessageBox.Show("Изменения сохранены.");
                LoadLoyaltyLevels(); // Перезагрузить
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void AddLevel_Click(object sender, RoutedEventArgs e)
        {
            editingRow = null;
            LevelNameTextBox.Text = "";
            MinOrderTotalTextBox.Text = "";
            DiscountPercentageTextBox.Text = "";
            LevelForm.Visibility = Visibility.Visible;
        }

        private void EditLevel_Click(object sender, RoutedEventArgs e)
        {
            if (LoyaltyLevelsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                editingRow = selectedRow.Row;
                LevelNameTextBox.Text = editingRow["LevelName"].ToString();
                MinOrderTotalTextBox.Text = editingRow["MinOrderAmount"].ToString();
                DiscountPercentageTextBox.Text = editingRow["DiscountPercentage"].ToString();
                LevelForm.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Выберите уровень для редактирования.");
            }
        }

        private void DeleteLevel_Click(object sender, RoutedEventArgs e)
        {
            if (LoyaltyLevelsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                if (MessageBox.Show("Удалить выбранный уровень лояльности?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string deleteQuery = "DELETE FROM LoyaltyLevels WHERE LoyaltyLevelID = @id";
                    DatabaseHelper.ExecuteNonQuery(deleteQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@id", selectedRow["LoyaltyLevelID"]);
                    });

                    LoadLoyaltyLevels();
                }
            }
            else
            {
                MessageBox.Show("Выберите уровень для удаления.");
            }
        }

        private void CloseLevelForm_Click(object sender, RoutedEventArgs e)
        {
            LevelForm.Visibility = Visibility.Collapsed;
        }

        private void SaveLevelButton_Click(object sender, RoutedEventArgs e)
        {
            string name = LevelNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название уровня.");
                return;
            }

            if (!decimal.TryParse(MinOrderTotalTextBox.Text, out decimal minOrder))
            {
                MessageBox.Show("Введите корректное значение порога заказа.");
                return;
            }

            if (!decimal.TryParse(DiscountPercentageTextBox.Text, out decimal discount))
            {
                MessageBox.Show("Введите корректное значение скидки.");
                return;
            }

            try
            {
                string duplicateCheckQuery = @"SELECT COUNT(*) FROM LoyaltyLevels 
                                       WHERE LevelName = @name" +
                                               (editingRow != null ? " AND LoyaltyLevelID != @id" : "");

                int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(duplicateCheckQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    if (editingRow != null)
                        cmd.Parameters.AddWithValue("@id", editingRow["LoyaltyLevelID"]);
                }));

                if (count > 0)
                {
                    MessageBox.Show("Уровень с таким названием уже существует.");
                    return;
                }

                if (editingRow == null)
                {
                    string insertQuery = @"INSERT INTO LoyaltyLevels (LevelName, MinOrderAmount, DiscountPercentage)
                                   VALUES (@name, @min, @discount)";
                    DatabaseHelper.ExecuteNonQuery(insertQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@min", minOrder);
                        cmd.Parameters.AddWithValue("@discount", discount);
                    });
                }
                else
                {
                    string updateQuery = @"UPDATE LoyaltyLevels 
                                   SET LevelName = @name, MinOrderAmount = @min, DiscountPercentage = @discount
                                   WHERE LoyaltyLevelID = @id";
                    DatabaseHelper.ExecuteNonQuery(updateQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@min", minOrder);
                        cmd.Parameters.AddWithValue("@discount", discount);
                        cmd.Parameters.AddWithValue("@id", editingRow["LoyaltyLevelID"]);
                    });
                }

                LoadLoyaltyLevels();
                LevelForm.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

    }
}
