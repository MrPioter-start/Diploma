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
        public bool IsStockSufficient { get; private set; } = true;

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
                OrderQuantity = row.Field<int>("Quantity"),
                Price = row.Field<decimal>("Price"),
                Total = row.Field<decimal>("Price") * row.Field<int>("Quantity")
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
            if (!IsOrderQuantityValid(selectedProducts))
            {
                IsConfirmed = false;
                throw new InvalidOperationException("Недостаточно товара на складе.");
            }

            var selectedProductsForSaving = selectedProducts
                .Where(row => row.Field<int>("Quantity") > 0)
                .ToList();

            if (!selectedProductsForSaving.Any())
            {
                IsConfirmed = false;
                throw new InvalidOperationException("Нет выбранных товаров для оформления заказа.");
            }

            bool quantitiesOk = UpdateQuantities(selectedProductsForSaving);
            if (!quantitiesOk)
            {
                IsConfirmed = false;
                throw new InvalidOperationException("Недостаточно товара на складе.");
            }

            Console.WriteLine("[LOG] Сохраняем заказ в AddSaleOrder. Проверка на дублирование списания");

            Queries.UpdateCashAmount(totalAmount, "Заказ", adminUsername);
            decimal currentTotalOrders = Queries.GetCustomerTotalOrders(customerEmail);
            Queries.UpdateCustomerTotalOrders(customerEmail, currentTotalOrders + totalAmount);
        }



        private bool UpdateQuantities(List<DataRow> selectedProducts)
        {
            foreach (var row in selectedProducts)
            {
                string productName = row.Field<string>("ProductName");
                int quantity = row.Field<int>("Quantity");

                int currentQuantity = GetCurrentProductQuantity(productName);
                int newQuantity = currentQuantity - quantity;

                Console.WriteLine($"[LOG] Товар: {productName} | Остаток: {currentQuantity} | Заказ: {quantity} | Новый остаток: {newQuantity}");

                if (newQuantity < 0)
                {
                    MessageBox.Show($"Недостаточно товара '{productName}' на складе.", "Ошибка");
                    IsStockSufficient = false;
                    return false;
                }
            }

            foreach (var row in selectedProducts)
            {
                string productName = row.Field<string>("ProductName");
                int quantity = row.Field<int>("Quantity");
                int currentQuantity = GetCurrentProductQuantity(productName);
                int newQuantity = currentQuantity - quantity;

                Queries.UpdateProductQuantity(productName, newQuantity);
            }

            return true;
        }




        private int GetCurrentProductQuantity(string productName)
        {
            string query = @"SELECT Quantity FROM Products WHERE Name = @ProductName";

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
                int quantity = row.Field<int>("Quantity");

                int currentQuantity = GetCurrentProductQuantity(productName);
                if (quantity > currentQuantity)
                {
                    MessageBox.Show($"Недостаточно товара '{productName}' на складе. Требуется: {quantity}, доступно: {currentQuantity}.", "Ошибка");
                    return false;
                }
            }

            return true;
        }
    }
}
