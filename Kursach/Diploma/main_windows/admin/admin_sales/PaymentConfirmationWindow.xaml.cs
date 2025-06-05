using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using Kursach.Database.WarehouseApp.Database;

namespace Kursach.main_windows.admin
{
    public partial class PaymentConfirmationWindow : Window
    {
        public bool IsConfirmed { get; private set; } = false;
        private decimal discountedTotal;
        private List<DataRow> selectedProducts;
        private decimal paymentAmount;

        public PaymentConfirmationWindow(List<DataRow> selectedProducts, decimal originalTotal, decimal discountedTotal, decimal paymentAmount)
        {
            InitializeComponent();

            this.selectedProducts = selectedProducts;
            this.discountedTotal = discountedTotal;
            this.paymentAmount = paymentAmount;

            // Защищаемся от возможных лишних пробелов и символов в названиях колонок
            if (selectedProducts.Count > 0)
            {
                FixColumnNames(selectedProducts[0].Table);
            }

            var orderDetails = selectedProducts.Select(row => new
            {
                ProductID = GetSafeField<int>(row, "ProductID"),
                Name = GetSafeField<string>(row, "Name", "Неизвестно"),
                Brand = GetSafeField<string>(row, "Brand", "Неизвестно"),
                CategoryName = GetSafeField<string>(row, "CategoryName", "Неизвестно"),
                OrderQuantity = GetSafeField<int>(row, "OrderQuantity", 0),
                Price = GetSafeField<decimal>(row, "Price", 0m),
                Total = GetSafeField<decimal>(row, "Price", 0m) * GetSafeField<int>(row, "OrderQuantity", 0)
            }).ToList();

            OrderDetailsDataGrid.ItemsSource = orderDetails;

            OriginalTotalTextBlock.Text = $"Итого без скидки: {originalTotal:F2} byn";
            DiscountTextBlock.Text = $"Скидка: {(originalTotal - discountedTotal):F2} byn";
            TotalPriceTextBlock.Text = $"Итого с учетом скидки: {discountedTotal:F2} byn";

            var activePromotions = GetActivePromotions(selectedProducts);

            PromotionsTextBlock.Text = GetPromotionDescription(activePromotions);

            if (paymentAmount >= discountedTotal)
            {
                PaymentAmountTextBlock.Text = $"{paymentAmount:F2} byn";
                ChangeTextBlock.Text = $"{(paymentAmount - discountedTotal):F2} byn";
            }
            else
            {
                PaymentAmountTextBlock.Text = $"{paymentAmount:F2} byn";
                ChangeTextBlock.Text = "Недостаточно средств";
            }
        }

        private void FixColumnNames(DataTable table)
        {
            // Убираем пробелы и обратные слэши из имен колонок
            foreach (DataColumn col in table.Columns)
            {
                string newName = col.ColumnName.Trim().Replace("\\", "");
                if (newName != col.ColumnName)
                {
                    col.ColumnName = newName;
                }
            }
        }

        private T GetSafeField<T>(DataRow row, string columnName, T defaultValue = default!)
        {
            if (row.Table.Columns.Contains(columnName) && !row.IsNull(columnName))
            {
                try
                {
                    return row.Field<T>(columnName);
                }
                catch
                {
                    // Если тип не совпадает, вернуть default
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (paymentAmount < discountedTotal)
            {
                MessageBox.Show("Внесенная сумма меньше стоимости заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsConfirmed = true;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private List<Promotion> GetActivePromotions(List<DataRow> selectedProducts)
        {
            var activePromotions = new List<Promotion>();

            // Загружаем все активные акции единожды, чтобы не дергать БД на каждый товар
            string query = @"
        SELECT 
            p.PromotionID,
            p.PromotionName,
            pr.TargetType,
            pr.TargetValue,
            p.DiscountPercentage
        FROM 
            Promotions p
        INNER JOIN 
            PromotionRules pr ON p.PromotionID = pr.PromotionID
        WHERE 
            @Today BETWEEN p.StartDate AND p.EndDate";

            var promotions = DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Today", DateTime.Today);
            }).AsEnumerable().Select(row => new Promotion
            {
                PromotionID = row.Field<int>("PromotionID"),
                PromotionName = row.Field<string>("PromotionName"),
                TargetType = row.Field<string>("TargetType"),
                TargetValue = row.Field<string>("TargetValue"),
                DiscountPercentage = row.Field<decimal>("DiscountPercentage")
            }).ToList();

            foreach (var product in selectedProducts)
            {
                string productName = GetSafeField<string>(product, "Name", "").Trim().ToLower();
                string brand = GetSafeField<string>(product, "Brand", "").Trim().ToLower();
                string category = GetSafeField<string>(product, "CategoryName", "").Trim().ToLower();

                foreach (var promotion in promotions)
                {
                    bool isApplicable = false;

                    string targetValue = promotion.TargetValue?.Trim().ToLower() ?? "";

                    switch (promotion.TargetType)
                    {
                        case "Товар":
                            isApplicable = productName == targetValue;
                            break;

                        case "Бренд":
                            isApplicable = brand == targetValue;
                            break;

                        case "Категория":
                            isApplicable = category == targetValue;
                            break;
                    }

                    if (isApplicable && !activePromotions.Any(p => p.PromotionID == promotion.PromotionID))
                    {
                        activePromotions.Add(promotion);
                    }
                }
            }

            return activePromotions;
        }


        private string GetPromotionDescription(List<Promotion> promotions)
        {
            if (promotions.Count == 0)
            {
                return "В заказе нет акций.";
            }

            var description = new StringBuilder();
            description.AppendLine("В заказе участвуют следующие акции:");

            foreach (var promotion in promotions)
            {
                description.AppendLine($"- {promotion.PromotionName} ({promotion.TargetType}: {promotion.TargetValue}) ({promotion.DiscountPercentage:F2}%)");
            }

            return description.ToString();
        }
    }
}
