using System.Windows;
using System.Collections.Generic;
using System.Data;

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

            var orderDetails = selectedProducts.Select(row => new
            {
                ProductID = row.Field<int>("ProductID"),
                Name = row.Field<string?>("Name") ?? "Неизвестно",
                Quantity = row.Field<int>("Quantity"),
                Price = row.Field<decimal>("Price"),
                Total = row.Field<decimal>("Price") * row.Field<int>("Quantity")
            }).ToList();

            OrderDetailsDataGrid.ItemsSource = orderDetails;

            OriginalTotalTextBlock.Text = $"Итого без скидки: {originalTotal:F2} byn";
            DiscountTextBlock.Text = $"Скидка: {(originalTotal - discountedTotal):F2} byn";
            TotalPriceTextBlock.Text = $"Итого с учетом скидки: {discountedTotal:F2} byn";

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
    }
}