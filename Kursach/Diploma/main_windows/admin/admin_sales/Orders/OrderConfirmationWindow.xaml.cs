using Kursach.Database;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using Kursach.Database.WarehouseApp.Database;

namespace Diploma.main_windows
{
    public partial class OrderConfirmationWindow : Window
    {
        private dynamic _clientInfo;
        private DataTable _selectedProducts;
        private string adminUsername;
        private List<Promotion> appliedPromotions;

        public OrderConfirmationWindow(dynamic clientInfo, DataTable selectedProducts, string adminUsername)
        {
            InitializeComponent();

            this.adminUsername = adminUsername;
            _clientInfo = clientInfo;
            _selectedProducts = selectedProducts;

            EnsureTotalColumnExistsAndUpdate();

            DataContext = _clientInfo;
            OrderProductsDataGrid.ItemsSource = _selectedProducts.DefaultView;

            CalculateAndDisplayTotal();
            LoadPromotionsDescription();
        }

        private void EnsureTotalColumnExistsAndUpdate()
        {
            if (!_selectedProducts.Columns.Contains("Total"))
                _selectedProducts.Columns.Add("Total", typeof(decimal));

            foreach (DataRow row in _selectedProducts.Rows)
            {
                int quantity = Convert.ToInt32(row["OrderQuantity"]);
                decimal price = Convert.ToDecimal(row["Price"]);
                row["Total"] = Math.Round(quantity * price, 2);
            }
        }


        private void CalculateAndDisplayTotal()
        {
            decimal total = _selectedProducts.AsEnumerable()
                .Sum(row => row.Field<int>("OrderQuantity") * row.Field<decimal>("Price"));

            TotalAmountTextBlock.Text = $"{total} byn";
        }

        private void LoadPromotionsDescription()
        {
            appliedPromotions = GetAppliedPromotionsForProducts(_selectedProducts);
            string description = GetPromotionDescription(appliedPromotions);
            PromotionsDescriptionTextBlock.Text = description;
        }

        private List<Promotion> GetAppliedPromotionsForProducts(DataTable products)
        {
            var activePromotions = GetActivePromotions();
            var usedPromotions = new List<Promotion>();

            foreach (DataRow product in products.Rows)
            {
                string name = product["Name"].ToString();
                string brand = product["Brand"].ToString();
                string category = product.Table.Columns.Contains("CategoryName") ? product["CategoryName"].ToString() : "";

                foreach (var promo in activePromotions)
                {
                    bool isApplicable = promo.TargetType switch
                    {
                        "Товар" => promo.TargetValue == name,
                        "Бренд" => promo.TargetValue == brand,
                        "Категория" => promo.TargetValue == category,
                        _ => false
                    };

                    if (isApplicable && !usedPromotions.Any(p => p.PromotionID == promo.PromotionID))
                        usedPromotions.Add(promo);
                }
            }

            return usedPromotions;
        }

        private List<Promotion> GetActivePromotions()
        {
            string query = @"
SELECT 
    p.PromotionID,
    pr.TargetType,
    pr.TargetValue,
    p.DiscountPercentage,
    p.PromotionName
FROM 
    Promotions p
INNER JOIN 
    PromotionRules pr ON p.PromotionID = pr.PromotionID
WHERE 
    @Today BETWEEN p.StartDate AND p.EndDate";

            return DatabaseHelper.ExecuteQuery(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@Today", DateTime.Today);
            }).AsEnumerable().Select(row => new Promotion
            {
                PromotionID = row.Field<int>("PromotionID"),
                TargetType = row.Field<string>("TargetType"),
                TargetValue = row.Field<string>("TargetValue"),
                DiscountPercentage = row.Field<decimal>("DiscountPercentage"),
                PromotionName = row.Field<string>("PromotionName")
            }).ToList();
        }

        private string GetPromotionDescription(List<Promotion> promotions)
        {
            if (promotions == null || promotions.Count == 0)
                return "В заказе нет акций.";

            var sb = new StringBuilder();
            sb.AppendLine("В заказе участвуют следующие акции:");
            foreach (var promo in promotions)
            {
                sb.AppendLine($"- {promo.PromotionName} ({promo.TargetType}: {promo.TargetValue}) ({promo.DiscountPercentage:F2}%)");
            }

            return sb.ToString();
        }

        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string clientEmail = _clientInfo.Email;
                decimal totalAmount = CalculateTotal();

                var existingCustomer = Queries.GetCustomerByEmail(clientEmail);
                if (existingCustomer.Rows.Count == 0)
                {
                    Queries.AddOrUpdateCustomer(_clientInfo.ClientName, clientEmail, _clientInfo.ContactInfo, adminUsername);
                }

                int customerId = Queries.GetCustomerIdByEmail(clientEmail);
                var selectedProductsList = _selectedProducts.AsEnumerable().ToList();

                Queries.AddTransactionOrder(selectedProductsList, totalAmount, adminUsername, customerId, "Оформлен", "Заказ");

                MessageBox.Show("Заказ успешно оформлен!", "Успех");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка");
            }
        }

        private decimal CalculateTotal()
        {
            return _selectedProducts.AsEnumerable()
                .Sum(row => row.Field<int>("OrderQuantity") * row.Field<decimal>("Price"));
        }
    }
}
