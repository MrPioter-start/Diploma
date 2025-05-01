using Kursach.Database;
using Kursach.Database.WarehouseApp.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace Diploma.main_windows
{
    public partial class OrderConfirmationWindow2 : Window
    {
        public bool IsConfirmed { get; private set; } = false;
        private decimal totalAmount;
        private List<DataRow> selectedProducts;
        private string adminUsername;

        public OrderConfirmationWindow2(List<DataRow> selectedProducts, decimal totalAmount, string adminUsername)
        {
            InitializeComponent();
            this.selectedProducts = selectedProducts;
            this.totalAmount = totalAmount;
            this.adminUsername = adminUsername;

            var orderDetails = selectedProducts.Select(row => new
            {
                ProductName = row.Field<string>("ProductName"),
                OrderQuantity = row.Field<int>("OrderQuantity"),
                Price = row.Field<decimal>("Price"),
                Total = row.Field<decimal>("Price") * row.Field<int>("OrderQuantity")
            }).ToList();

            OrderDetailsDataGrid.ItemsSource = orderDetails;

            TotalPriceTextBlock.Text = $"Итого: {totalAmount:F2} byn";
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveOrderToDatabase();
                IsConfirmed = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подтверждении заказа: {ex.Message}", "Ошибка");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private void SaveOrderToDatabase()
        {
            try
            {
                var selectedProductsForSaving = selectedProducts
                    .Where(row => row.Field<int>("OrderQuantity") > 0)
                    .ToList();

                if (!selectedProductsForSaving.Any())
                {
                    throw new InvalidOperationException("Нет выбранных товаров для оформления заказа.");
                }

                Queries.AddSaleOrder(selectedProductsForSaving, totalAmount, adminUsername);

                UpdateQuantities(selectedProductsForSaving);

                Queries.UpdateCashAmount(totalAmount, "Продажа", adminUsername);

                MessageBox.Show("Заказ успешно добавлен в продажи!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка");
            }
        }

        private void UpdateQuantities(List<DataRow> selectedProducts)
        {
            foreach (var row in selectedProducts)
            {
                string productName = row.Field<string>("ProductName");
                int orderQuantity = row.Field<int>("OrderQuantity");

                int currentQuantity = GetCurrentProductQuantity(productName);

                int newQuantity = currentQuantity - orderQuantity;

                if (newQuantity < 0)
                {
                    MessageBox.Show($"Недостаточно товара '{productName}' на складе.", "Ошибка");
                    continue;
                }

                Queries.UpdateProductQuantity(productName, newQuantity);
            }
        }

        private int GetCurrentProductQuantity(string productName)
        {
            string query = @"
        SELECT Quantity 
        FROM Products 
        WHERE Name = @ProductName";

            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@ProductName", productName);
            }) is int quantity ? quantity : 0;
        }
    }
}