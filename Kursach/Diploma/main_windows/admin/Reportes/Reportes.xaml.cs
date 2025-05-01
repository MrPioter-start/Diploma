using Kursach.Database;
using Microsoft.Win32;
using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Xceed.Words.NET;

namespace Diploma.main_windows.admin.Reportes
{
    /// <summary>
    /// Логика взаимодействия для Reportes.xaml
    /// </summary>
    public partial class Reportes : Window
    {
        private string CurrentAdminUsername;

        public Reportes(string adminUsername)
        {
            InitializeComponent();
            CurrentAdminUsername = adminUsername;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            ReportDataGrid.ItemsSource = null;

            try
            {
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;
                string reportType = (ReportTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (startDate > endDate)
                {
                    MessageBox.Show("Начальная дата не может быть позже конечной.", "Ошибка");
                    return;
                }

                DataTable reportData = null;

                switch (reportType)
                {
                    case "Продажи":
                        reportData = Queries.GetSalesReport(startDate, endDate, CurrentAdminUsername);
                        break;
                    case "Затраты":
                        reportData = Queries.GetCostsReport(startDate, endDate, CurrentAdminUsername);
                        break;
                    case "Популярные товары":
                        reportData = Queries.GetPopularProductsReport(startDate, endDate, CurrentAdminUsername);
                        break;
                    case "Динамика заказов":
                        reportData = Queries.GetOrderDynamicsReport(startDate, endDate, CurrentAdminUsername);
                        break;
                    default:
                        MessageBox.Show("Выберите тип отчета.", "Ошибка");
                        return;
                }

                if (reportData == null || reportData.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для выбранного периода.", "Информация");
                    return;
                }

                Queries.AddReport(
                reportType: reportType,
                periodStart: startDate,
                periodEnd: endDate,
                generatedBy: CurrentAdminUsername
                );
                ReportDataGrid.Items.Clear();

                ReportDataGrid.ItemsSource = null;
                ReportDataGrid.ItemsSource = reportData.DefaultView;

                MessageBox.Show("Отчет успешно сформирован.", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "Report.xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                try
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Отчет");
                        var data = ReportDataGrid.ItemsSource as DataView;

                        // Заголовки
                        for (int i = 0; i < data.Table.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = data.Table.Columns[i].ColumnName;
                        }

                        // Данные
                        for (int row = 0; row < data.Count; row++)
                        {
                            for (int col = 0; col < data.Table.Columns.Count; col++)
                            {
                                worksheet.Cells[row + 2, col + 1].Value = data[row][col];
                            }
                        }

                        for (int i = 1; i <= data.Table.Columns.Count; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        FileInfo fileInfo = new FileInfo(dialog.FileName);
                        package.SaveAs(fileInfo);
                    }

                    MessageBox.Show("Отчет успешно экспортирован в Excel!", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "Report.docx",
                Filter = "Word files (*.docx)|*.docx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (DocX document = DocX.Create(dialog.FileName))
                    {
                        document.InsertParagraph("Отчет").FontSize(16).Bold().Color(Xceed.Drawing.Color.White);

                        var data = ReportDataGrid.ItemsSource as DataView;
                        var table = document.AddTable(data.Count + 1, data.Table.Columns.Count);

                        for (int i = 0; i < data.Table.Columns.Count; i++)
                        {
                            table.Rows[0].Cells[i].Paragraphs[0].Append(data.Table.Columns[i].ColumnName).Bold();
                        }

                        for (int row = 0; row < data.Count; row++)
                        {
                            for (int col = 0; col < data.Table.Columns.Count; col++)
                            {
                                table.Rows[row + 1].Cells[col].Paragraphs[0].Append(data[row][col].ToString());
                            }
                        }

                        document.InsertTable(table);
                        document.Save();
                    }

                    MessageBox.Show("Отчет успешно экспортирован в Word!", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "Report.pdf",
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (var document = new PdfDocument())
                    {
                        var page = document.AddPage();
                        var gfx = XGraphics.FromPdfPage(page);
                        var font = new XFont("Arial", 12);

                        var data = ReportDataGrid.ItemsSource as DataView;
                        int y = 50;

                        for (int i = 0; i < data.Table.Columns.Count; i++)
                        {
                            gfx.DrawString(data.Table.Columns[i].ColumnName, font, XBrushes.Black, new XRect(50 + i * 150, y, 150, 20), XStringFormats.TopLeft);
                        }

                        y += 20;

                        foreach (DataRowView row in data)
                        {
                            for (int col = 0; col < data.Table.Columns.Count; col++)
                            {
                                gfx.DrawString(row[col].ToString(), font, XBrushes.Black, new XRect(50 + col * 150, y, 150, 20), XStringFormats.TopLeft);
                            }
                            y += 20;
                        }

                        document.Save(dialog.FileName);
                    }

                    MessageBox.Show("Отчет успешно экспортирован в PDF!", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void History_of_reportes(object sender, RoutedEventArgs e)
        {
            var reportsHistory = new ReportsHistoryWindow(CurrentAdminUsername);
            reportsHistory.ShowDialog();
        }
    }
}
