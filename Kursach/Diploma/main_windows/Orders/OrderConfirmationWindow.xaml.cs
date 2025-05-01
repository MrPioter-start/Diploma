using Kursach.Database;
using System;
using System.Data;
using System.Windows;

namespace Diploma.main_windows
{
    public partial class OrderConfirmationWindow : Window
    {
        private dynamic _clientInfo;
        private DataTable _selectedProducts;
        string adminUsername;

        public OrderConfirmationWindow(dynamic clientInfo, DataTable selectedProducts, string adminUsername)
        {
            InitializeComponent();

            this.adminUsername = adminUsername;
            _clientInfo = clientInfo;
            _selectedProducts = selectedProducts;

            DataContext = _clientInfo;
            OrderProductsDataGrid.ItemsSource = _selectedProducts.DefaultView;
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
                    Queries.AddCustomer(_clientInfo.ClientName, clientEmail, _clientInfo.ContactInfo, adminUsername);
                }

                int customerId = Queries.GetCustomerIdByEmail(clientEmail);

                var selectedProductsList = _selectedProducts.AsEnumerable().ToList();

                Queries.AddSale(selectedProductsList, totalAmount, customerId, adminUsername);

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