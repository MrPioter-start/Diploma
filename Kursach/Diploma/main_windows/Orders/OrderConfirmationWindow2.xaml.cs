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
        private string customerEmail;

        public OrderConfirmationWindow2(List<DataRow> selectedProducts, decimal totalAmount, string adminUsername, string customerEmail)
        {
            InitializeComponent();
            this.selectedProducts = selectedProducts;
            this.totalAmount = totalAmount;
            this.adminUsername = adminUsername;
            this.customerEmail = customerEmail;

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
                IsConfirmed = false;
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
                if (!IsOrderQuantityValid(selectedProducts))
                {
                    throw new InvalidOperationException("Недостаточно товара на складе.");
                }

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

                decimal currentTotalOrders = Queries.GetCustomerTotalOrders(customerEmail);

                Queries.UpdateCustomerTotalOrders(customerEmail, currentTotalOrders + totalAmount);

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

        private bool IsOrderQuantityValid(List<DataRow> selectedProducts)
        {
            foreach (var row in selectedProducts)
            {
                string productName = row.Field<string>("ProductName");
                int orderQuantity = row.Field<int>("OrderQuantity");

                int currentQuantity = GetCurrentProductQuantity(productName);

                if (orderQuantity > currentQuantity)
                {
                    MessageBox.Show($"Недостаточно товара '{productName}' на складе. Требуется: {orderQuantity}, доступно: {currentQuantity}.", "Ошибка");
                    return false;
                }
            }

            return true;
        }
    }
}