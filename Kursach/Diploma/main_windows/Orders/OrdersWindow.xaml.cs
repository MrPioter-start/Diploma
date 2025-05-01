using Kursach.Database;
using Kursach.main_windows.admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diploma.main_windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private string adminUsername;

        public OrdersWindow(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = Queries.GetOrdersByAdmin(adminUsername);
            OrdersDataGrid.ItemsSource = orders.DefaultView;
        }

        private void StatusComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is DataRowView row)
            {
                try
                {
                    int orderId = Convert.ToInt32(row["OrderID"]);
                    string newStatus = comboBox.SelectedItem?.ToString();

                    if (string.IsNullOrEmpty(newStatus))
                    {
                        MessageBox.Show("Статус не выбран.", "Ошибка");
                        return;
                    }

                    Queries.UpdateOrderStatus(orderId, newStatus);

                    if (newStatus == "Доставка")
                    {
                        decimal totalAmount = Convert.ToDecimal(row["TotalAmount"]);

                        var orderDetails = Queries.GetOrderDetails(orderId);

                        if (orderDetails == null || orderDetails.Rows.Count == 0)
                        {
                            MessageBox.Show("Нет данных о товарах в заказе.", "Ошибка");
                            return;
                        }

                        var orderDetailsList = orderDetails.AsEnumerable().ToList();

                        var confirmationWindow = new OrderConfirmationWindow2(
                            orderDetailsList,
                            totalAmount,
                            adminUsername);

                        confirmationWindow.ShowDialog();

                        if (confirmationWindow.IsConfirmed)
                        {
                            MessageBox.Show("Оплата успешно подтверждена!", "Успех");
                        }
                        else
                        {
                            MessageBox.Show("Оплата отменена.", "Информация");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке статуса заказа: {ex.Message}", "Ошибка");
                }
            }
        }
    }
}
