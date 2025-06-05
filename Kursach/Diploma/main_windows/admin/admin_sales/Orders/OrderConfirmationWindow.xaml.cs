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

        public string LoyaltyLevel { get; private set; } = "Нет уровня";
        public decimal LoyaltyDiscount { get; private set; } = 0m;
        public string ClientName { get; set; }
        public string ContactInfo { get; set; }
        public string Email { get; set; }
        private List<Promotion> appliedPromotions;

        public OrderConfirmationWindow(dynamic clientInfo, DataTable selectedProducts, string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
            _clientInfo = clientInfo;
            _selectedProducts = selectedProducts;

            EnsureColumns();
            DataContext = this;

            ClientName = _clientInfo.ClientName;
            ContactInfo = _clientInfo.ContactInfo;
            Email = _clientInfo.Email;

            OrderProductsDataGrid.ItemsSource = _selectedProducts.DefaultView;

            LoadCustomerLoyaltyInfo();
            ApplyDiscountsToProducts();
            CalculateAndDisplayTotal();
            LoadPromotionsDescription();
        }

        private void EnsureColumns()
        {
            if (!_selectedProducts.Columns.Contains("Total"))
                _selectedProducts.Columns.Add("Total", typeof(decimal));
            if (!_selectedProducts.Columns.Contains("DiscountedPrice"))
                _selectedProducts.Columns.Add("DiscountedPrice", typeof(decimal));

            foreach (DataRow row in _selectedProducts.Rows)
            {
                int qty = Convert.ToInt32(row["OrderQuantity"]);
                decimal price = Convert.ToDecimal(row["Price"]);
                // Изначально DiscountedPrice = та же цена из таблицы (в ней уже лежит промо-цена, если товар акционный)
                row["DiscountedPrice"] = price;
                row["Total"] = Math.Round(price * qty, 2);
            }
        }

        private void LoadCustomerLoyaltyInfo()
        {
            int id = Queries.GetCustomerIdByEmail(_clientInfo.Email);
            if (id == 0) return;

            var dt = Queries.GetCustomerLoyaltyInfo(id);
            if (dt.Rows.Count == 0) return;

            decimal sum = dt.Rows[0].Field<decimal>("TotalOrders");
            int? curLvl = dt.Rows[0].Field<int?>("LoyaltyLevelID");
            string curName = dt.Rows[0].Field<string>("LevelName");
            decimal curDisc = dt.Rows[0].Field<decimal?>("DiscountPercentage") ?? 0m;

            var dtLvl = Queries.GetLoyaltyLevelByTotalOrders(sum);
            if (dtLvl.Rows.Count > 0)
            {
                int newLvl = dtLvl.Rows[0].Field<int>("LoyaltyLevelID");
                string newName = dtLvl.Rows[0].Field<string>("LevelName");
                decimal newDisc = dtLvl.Rows[0].Field<decimal>("DiscountPercentage");

                if (curLvl != newLvl)
                {
                    Queries.UpdateCustomerLoyaltyLevel(id, newLvl);
                    LoyaltyLevel = newName;
                    LoyaltyDiscount = newDisc;
                }
                else
                {
                    LoyaltyLevel = curName;
                    LoyaltyDiscount = curDisc;
                }
            }
        }

        private void ApplyDiscountsToProducts()
        {
            // Берём цену из колонки Price (в ней уже учтены акции), и сразу считаем лояльностную скидку
            foreach (DataRow row in _selectedProducts.Rows)
            {
                int qty = row.Field<int>("OrderQuantity");
                decimal basePrice = row.Field<decimal>("Price");      // здесь уже промо-цена, если есть
                decimal finalPrice = Math.Round(basePrice * (1 - LoyaltyDiscount / 100m), 2);
                row["DiscountedPrice"] = finalPrice;
                row["Total"] = Math.Round(finalPrice * qty, 2);
            }
        }

        private void CalculateAndDisplayTotal()
        {
            decimal total = _selectedProducts.AsEnumerable()
                .Sum(r => r.Field<int>("OrderQuantity") * r.Field<decimal>("DiscountedPrice"));

            TotalAmountTextBlock.Text = $"{total:F2} BYN";
            if (LoyaltyDiscount > 0)
                TotalAmountTextBlock.Text += $" (со скидкой уровня лояльности {LoyaltyDiscount:F2}%)";
        }

        private List<Promotion> GetActivePromotions()
        {
            // Этот метод больше нужен только для описания акций, но уже не влияет на цены
            string query = @"
SELECT p.PromotionID, pr.TargetType, pr.TargetValue, p.DiscountPercentage, p.PromotionName
FROM Promotions p
JOIN PromotionRules pr ON p.PromotionID = pr.PromotionID
WHERE @Today BETWEEN p.StartDate AND p.EndDate";
            return DatabaseHelper.ExecuteQuery(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@Today", DateTime.Today);
            }).AsEnumerable().Select(r => new Promotion
            {
                PromotionID = r.Field<int>("PromotionID"),
                TargetType = r.Field<string>("TargetType"),
                TargetValue = r.Field<string>("TargetValue"),
                DiscountPercentage = r.Field<decimal>("DiscountPercentage"),
                PromotionName = r.Field<string>("PromotionName")
            }).ToList();
        }

        private void LoadPromotionsDescription()
        {
            // Просто собираем описание, не трогаем расчёт
            appliedPromotions = GetAppliedPromotionsForProducts(_selectedProducts);
            PromotionsDescriptionTextBlock.Text = GetPromotionDescription(appliedPromotions);
        }

        private List<Promotion> GetAppliedPromotionsForProducts(DataTable products)
        {
            var active = GetActivePromotions();
            var used = new List<Promotion>();

            foreach (DataRow row in products.Rows)
            {
                string name = row["Name"].ToString();
                string brand = row["Brand"].ToString();
                string category = products.Columns.Contains("CategoryName")
                    ? row["CategoryName"].ToString()
                    : "";

                foreach (var promo in active)
                {
                    bool ok = promo.TargetType switch
                    {
                        "Товар" => promo.TargetValue == name,
                        "Бренд" => promo.TargetValue == brand,
                        "Категория" => promo.TargetValue == category,
                        _ => false
                    };
                    if (ok && !used.Any(x => x.PromotionID == promo.PromotionID))
                        used.Add(promo);
                }
            }
            return used;
        }

        private string GetPromotionDescription(List<Promotion> promotions)
        {
            if (promotions.Count == 0) return "В заказе нет акций.";
            var sb = new StringBuilder("В заказе участвуют следующие акции:\n");
            promotions.ForEach(p => sb.AppendLine(
                $"- {p.PromotionName} ({p.TargetType}: {p.TargetValue}) — {p.DiscountPercentage:F2}%"));
            return sb.ToString();
        }

        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = _clientInfo.Email;
                var cust = Queries.GetCustomerByEmail(email);
                if (cust.Rows.Count == 0)
                    Queries.AddOrUpdateCustomer(_clientInfo.ClientName, email, _clientInfo.ContactInfo, adminUsername);
                int custId = Queries.GetCustomerIdByEmail(email);

                decimal finalTotal = _selectedProducts.AsEnumerable()
                    .Sum(r => r.Field<int>("OrderQuantity") * r.Field<decimal>("DiscountedPrice"));

                // Формируем детали по колонке DiscountedPrice
                var rows = _selectedProducts.AsEnumerable().Select(r =>
                {
                    var nr = r.Table.NewRow();
                    nr.ItemArray = r.ItemArray.Clone() as object[];
                    nr["Price"] = r.Field<decimal>("DiscountedPrice");
                    return nr;
                }).ToList();

                Queries.AddTransactionOrder(rows, finalTotal, adminUsername, custId, "Оформлен", "Заказ");
                MessageBox.Show($"Заказ оформлен. Итог: {finalTotal:F2} BYN", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
