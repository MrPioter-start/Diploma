using Kursach.Database;
using Kursach.main_windows.admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

namespace Diploma.main_windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private bool isLoading = false;
        private string adminUsername;
        private bool isUpdatingComboBox = false;

        public OrdersWindow(string adminUsername)
        {
            isLoading = true;

            InitializeComponent();
            this.adminUsername = adminUsername;
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = Queries.GetOrdersByAdmin(adminUsername);
            OrdersDataGrid.ItemsSource = orders.DefaultView;
            isLoading = false;
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdatingComboBox || isLoading) return;

            if (sender is ComboBox comboBox && comboBox.DataContext is DataRowView row)
            {
                try
                {
                    int orderId = Convert.ToInt32(row["OrderID"]);
                    string newStatus = comboBox.SelectedItem?.ToString();
                    string currentStatus = row["Status"]?.ToString();

                    if (string.IsNullOrEmpty(newStatus))
                    {
                        MessageBox.Show("Статус не выбран.", "Ошибка");
                        return;
                    }

                    if (currentStatus == "Завершен" || currentStatus == "Отменен")
                    {
                        isUpdatingComboBox = true;
                        comboBox.SelectedItem = currentStatus;
                        isUpdatingComboBox = false;
                        MessageBox.Show("Этот заказ уже завершён или отменён. Нельзя изменить его статус.", "Информация");
                        return;
                    }

                    if (newStatus == "Завершен")
                    {
                        isUpdatingComboBox = true;
                        comboBox.SelectedItem = currentStatus;
                        isUpdatingComboBox = false;
                        MessageBox.Show("Статус 'Завершен' нельзя выбрать вручную.", "Информация");
                        return;
                    }

                    string customerEmail = row["Email"].ToString();
                    decimal totalAmount = Convert.ToDecimal(row["TotalAmount"]);

                    string customerName = row.Row.Table.Columns.Contains("ClientName")
                      ? row["ClientName"].ToString()
                      : "клиент";


                    if (newStatus == "Доставлен")
                    {
                        var orderDetails = Queries.GetOrderDetails(orderId);

                        var confirmationWindow = new OrderConfirmationWindow2(
                            orderDetails.AsEnumerable().ToList(),
                            totalAmount,
                            adminUsername,
                            customerEmail);

                        confirmationWindow.ShowDialog();

                        isUpdatingComboBox = true;
                        if (confirmationWindow.IsConfirmed && confirmationWindow.IsStockSufficient)
                        {
                            Queries.UpdateOrderStatus(orderId, "Завершен");
                            row["Status"] = "Завершен";
                            comboBox.SelectedItem = "Завершен";
                            MessageBox.Show("Оплата подтверждена! Заказ завершён.", "Успех");

                            SendStatusEmail(customerEmail, orderId.ToString(), "Завершен", totalAmount, customerName);
                        }
                        else
                        {
                            Queries.UpdateOrderStatus(orderId, "Собран");
                            row["Status"] = "Собран";
                            comboBox.SelectedItem = "Собран";
                            MessageBox.Show("Оплата отменена. Заказ возвращён в статус 'Собран'.", "Информация");
                        }
                        isUpdatingComboBox = false;
                        return;
                    }

                    Queries.UpdateOrderStatus(orderId, newStatus);
                    row["Status"] = newStatus;

                    if (newStatus == "Отменен")
                    {
                        isUpdatingComboBox = true;
                        comboBox.IsEnabled = false;
                        isUpdatingComboBox = false;
                    }

                    SendStatusEmail(customerEmail, orderId.ToString(), newStatus, totalAmount, customerName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }



        private void StatusComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is DataRowView row)
            {
                string currentStatus = row["Status"]?.ToString();

                isUpdatingComboBox = true;
                comboBox.SelectedItem = currentStatus;

                if (currentStatus == "Завершен" || currentStatus == "Отменен")
                {
                    comboBox.IsEnabled = false;
                }

                isUpdatingComboBox = false;
            }
        }

        private void SendStatusEmail(string toEmail, string orderId, string status, decimal totalAmount, string customerName)
        {
            try
            {
                var fromAddress = new MailAddress("playingcsgo61@gmail.com", "MyApp");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "zjqn iplw nqof lnnu";
                string subject = $"Обновление статуса вашего заказа #{orderId}";

                string body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; background-color: #f6f9fc; margin: 0; padding: 20px;'>
            <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
                <h1 style='color: #0078D7;'>Спасибо за покупку в Магазине GoldenBrush</h1>
                <p>Здравствуйте, <strong>{customerName}</strong>,</p>
                <p>Статус вашего заказа <strong>#{orderId}</strong> был обновлен на:</p>
                <h2 style='color: #28a745;'>{status}</h2>

                <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                    <tr style='background-color: #0078D7; color: white;'>
                        <th style='padding: 10px; text-align: left;'>Информация о заказе</th>
                        <th style='padding: 10px; text-align: right;'>Сумма</th>
                    </tr>
                    <tr>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd;'>Общая сумма заказа:</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: right;'>{totalAmount:F2} BYN</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px;'>Дата обновления:</td>
                        <td style='padding: 10px; text-align: right;'>{DateTime.Now:dd.MM.yyyy HH:mm}</td>
                    </tr>
                </table>

                <p style='margin-top: 30px;'>Если у вас есть вопросы или нужна помощь, свяжитесь с нашей службой поддержки:</p>
                <p><a href='mailto:support@magazinxyz.com' style='color: #0078D7; text-decoration: none;'>pitranufriev61@gmail.com</a></p>

                <hr style='margin-top: 40px; border: none; border-top: 1px solid #eee;' />

                <p style='font-size: 0.8em; color: #888;'>Это автоматическое сообщение, пожалуйста, не отвечайте на него.</p>
            </div>
        </body>
        </html>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке почты: {ex.Message}", "Ошибка");
            }
        }
    }
}
