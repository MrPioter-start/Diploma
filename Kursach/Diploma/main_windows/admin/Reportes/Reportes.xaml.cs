using Kursach.Database;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Diploma.main_windows.admin.Reportes
{
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

                DataTable reportData = reportType switch
                {
                    "Продажи" => Queries.GetSalesReport(startDate, endDate, CurrentAdminUsername),
                    "Затраты" => Queries.GetCostsReport(startDate, endDate, CurrentAdminUsername),
                    "Популярные товары" => Queries.GetPopularProductsReport(startDate, endDate, CurrentAdminUsername),
                    "Динамика заказов" => Queries.GetOrderDynamicsReport(startDate, endDate, CurrentAdminUsername),
                    _ => null
                };

                if (reportData == null || reportData.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для выбранного периода.", "Информация");
                    return;
                }

                // Удаляем строки с нулевыми или пустыми количествами
                foreach (DataColumn col in reportData.Columns)
                {
                    if (col.ColumnName.ToLower().Contains("колич"))
                    {
                        for (int i = reportData.Rows.Count - 1; i >= 0; i--)
                        {
                            var val = reportData.Rows[i][col];
                            if (val == DBNull.Value || Convert.ToDecimal(val) == 0)
                                reportData.Rows.RemoveAt(i);
                        }
                        break;
                    }
                }

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для отображения после фильтрации нулевых значений.", "Информация");
                    return;
                }

                Queries.AddReport(reportType, startDate, endDate, CurrentAdminUsername);
                ReportDataGrid.ItemsSource = reportData.DefaultView;

                // Форматирование чисел и дат
                foreach (var col in ReportDataGrid.Columns.OfType<DataGridTextColumn>())
                {
                    var header = col.Header.ToString().ToLower();
                    if (header.Contains("дата") || header.Contains("время"))
                        col.Binding.StringFormat = "dd.MM.yyyy";
                    else if (header.Contains("стоим") || header.Contains("сумм") || header.Contains("колич") || header.Contains("цен"))
                        col.Binding.StringFormat = "F2";
                }

                MessageBox.Show("Отчет успешно сформирован.", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var data = ReportDataGrid.ItemsSource as DataView;
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта в Excel.", "Информация");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "Report.xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true) return;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Отчет");

                string reportType = (ReportTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Отчет";
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

                worksheet.Cells[1, 1].Value = $"Отчет по {reportType.ToLower()} с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                var titleRange = worksheet.Cells[1, 1, 1, data.Table.Columns.Count];
                titleRange.Merge = true;
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.Size = 14;
                titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(1).Height = 24;

                // Заголовки
                for (int i = 0; i < data.Table.Columns.Count; i++)
                {
                    var cell = worksheet.Cells[2, i + 1];
                    cell.Value = data.Table.Columns[i].ColumnName;
                    cell.Style.Font.Bold = true;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.WrapText = false; // ОТКЛЮЧАЕМ перенос, пусть ширина подстроится
                }

                // Данные
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < data.Table.Columns.Count; col++)
                    {
                        var cell = worksheet.Cells[row + 3, col + 1];
                        var val = data[row][col];
                        cell.Value = val switch
                        {
                            DateTime dt => dt.ToString("dd.MM.yyyy"),
                            double d => Math.Round(d, 2),
                            decimal dec => Math.Round(dec, 2),
                            _ => val
                        };
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        cell.Style.WrapText = false;
                    }
                }

                // Автоширина и ограничение максимальной ширины
                for (int i = 1; i <= data.Table.Columns.Count; i++)
                {
                    worksheet.Column(i).AutoFit();
                    if (worksheet.Column(i).Width > 40)
                        worksheet.Column(i).Width = 40;
                }

                package.SaveAs(new FileInfo(dialog.FileName));
                MessageBox.Show("Отчет успешно экспортирован в Excel!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }



        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            var data = ReportDataGrid.ItemsSource as DataView;
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта в Word.", "Информация");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "Report.docx",
                Filter = "Word files (*.docx)|*.docx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                using var document = DocX.Create(dialog.FileName);
                string reportType = (ReportTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Отчет";
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

                document.InsertParagraph($"Отчет по {reportType.ToLower()} с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}")
                        .FontSize(16).Bold().Color(Xceed.Drawing.Color.DarkSlateBlue)
                        .Alignment = Alignment.center;

                var table = document.AddTable(data.Count + 1, data.Table.Columns.Count);
                table.Design = TableDesign.TableGrid;

                // Заголовки
                for (int i = 0; i < data.Table.Columns.Count; i++)
                {
                    var cell = table.Rows[0].Cells[i];
                    cell.Paragraphs[0].Append(data.Table.Columns[i].ColumnName)
                                      .Bold()
                                      .FontSize(12)
                                      .Color(Xceed.Drawing.Color.White);
                    cell.FillColor = Xceed.Drawing.Color.DarkSlateBlue;
                }

                // Данные
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < data.Table.Columns.Count; col++)
                    {
                        var val = data[row][col];
                        string text = val switch
                        {
                            DateTime dt => dt.ToString("dd.MM.yyyy"),
                            double d => d.ToString("F2"),
                            decimal dec => dec.ToString("F2"),
                            _ => val?.ToString() ?? ""
                        };

                        var cell = table.Rows[row + 1].Cells[col];
                        cell.Paragraphs[0].Append(text)
                                          .FontSize(11)
                                          .SpacingAfter(2);
                    }
                }

                table.AutoFit = AutoFit.Contents;
                document.InsertParagraph().InsertTableAfterSelf(table);

                document.Save();
                MessageBox.Show("Отчет успешно экспортирован в Word!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }


        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            var data = ReportDataGrid.ItemsSource as DataView;
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта в PDF.", "Информация");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "Report.pdf",
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                GlobalFontSettings.FontResolver = new CustomFontResolver();
                using var document = new PdfDocument();
                var page = document.AddPage();
                page.Orientation = PdfSharp.PageOrientation.Landscape;
                var gfx = XGraphics.FromPdfPage(page);

                var font = new XFont("Arial", 10);
                var boldFont = new XFont("Arial", 14);
                var headerFont = new XFont("Arial", 14);

                double margin = 40;
                double y = margin;
                double x = margin;

                string reportType = (ReportTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Отчет";
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

                gfx.DrawString($"Отчет по '{reportType.ToLower()}' с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}",
                    headerFont, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
                y += 30;

                double colWidth = (page.Width - 2 * margin) / data.Table.Columns.Count;
                double rowHeight = 20;

                // Заголовки
                for (int i = 0; i < data.Table.Columns.Count; i++)
                {
                    gfx.DrawRectangle(XPens.Black, x + i * colWidth, y, colWidth, rowHeight);
                    gfx.DrawString(data.Table.Columns[i].ColumnName, boldFont, XBrushes.Black,
                        new XRect(x + i * colWidth + 2, y + 3, colWidth - 4, rowHeight), XStringFormats.TopLeft);
                }

                y += rowHeight;

                foreach (DataRowView row in data)
                {
                    double maxRowHeight = rowHeight;

                    for (int col = 0; col < data.Table.Columns.Count; col++)
                    {
                        string text = FormatCell(row[col]);
                        var size = gfx.MeasureString(text, font);
                        var lines = Math.Ceiling(size.Width / colWidth);
                        double heightNeeded = lines * rowHeight;

                        if (heightNeeded > maxRowHeight)
                            maxRowHeight = heightNeeded;
                    }

                    for (int col = 0; col < data.Table.Columns.Count; col++)
                    {
                        string text = FormatCell(row[col]);

                        gfx.DrawRectangle(XPens.Black, x + col * colWidth, y, colWidth, maxRowHeight);
                        gfx.DrawString(text, font, XBrushes.Black,
                            new XRect(x + col * colWidth + 2, y + 3, colWidth - 4, maxRowHeight), XStringFormats.TopLeft);
                    }

                    y += maxRowHeight;

                    if (y + maxRowHeight > page.Height - margin)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        y = margin;
                    }
                }

                document.Save(dialog.FileName);
                MessageBox.Show("Отчет успешно экспортирован в PDF!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }

            string FormatCell(object val)
            {
                return val switch
                {
                    DateTime dt => dt.ToString("dd.MM.yyyy HH:mm"),
                    double d => d.ToString("F2"),
                    decimal dec => dec.ToString("F2"),
                    _ => val?.ToString() ?? ""
                };
            }
        }

        private void History_of_reportes(object sender, RoutedEventArgs e)
        {
            new ReportsHistoryWindow(CurrentAdminUsername).ShowDialog();
        }
    }
}
