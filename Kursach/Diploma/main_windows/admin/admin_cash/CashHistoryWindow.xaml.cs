using System.Data;
using System.Drawing;
using System.IO;
using System.Windows;
using Kursach.Database;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Xceed.Document.NET;
using Xceed.Words.NET;




namespace Kursach.main_windows.admin
{
    public partial class CashHistoryWindow : Window
    {
        private string adminUsername;

        public CashHistoryWindow(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
            LoadCashHistory();
        }

        private void LoadCashHistory()
        {
            DataTable cashHistoryTable = Queries.GetCashTransactions(adminUsername);
            CashHistoryDataGrid.ItemsSource = cashHistoryTable.DefaultView;
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "CashHistory.xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                try
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("История операций кассы");

                        worksheet.Cells[1, 1].Value = "Тип операции";
                        worksheet.Cells[1, 2].Value = "Сумма";
                        worksheet.Cells[1, 3].Value = "Время";
                        worksheet.Cells[1, 4].Value = "Пользователь";

                        using (var range = worksheet.Cells[1, 1, 1, 4])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FF00A651"));
                            range.Style.Font.Color.SetColor(Color.White);
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.White);
                        }

                        int rowIndex = 2;
                        foreach (DataRowView row in CashHistoryDataGrid.ItemsSource)
                        {
                            string operationType = row["OperationType"].ToString();
                            decimal amount = Convert.ToDecimal(row["Amount"]);
                            DateTime timestamp = Convert.ToDateTime(row["Timestamp"]);
                            string adminUsername = row["AdminUsername"].ToString();

                            Color rowColor = Color.White;
                            Color borderAmountColor = Color.White;

                            switch (operationType)
                            {
                                case "Пополнение":
                                    rowColor = ColorTranslator.FromHtml("#FF00A651");
                                    borderAmountColor = Color.Green;
                                    break;
                                case "Продажа":
                                    rowColor = ColorTranslator.FromHtml("#FF00A651");
                                    borderAmountColor = Color.Green;
                                    break;
                                case "Снятие":
                                case "Возврат":
                                    rowColor = ColorTranslator.FromHtml("#FFD4AF37");
                                    borderAmountColor = Color.Red;
                                    break;
                                case "Инициализация":
                                    rowColor = ColorTranslator.FromHtml("#FF4D4D4D");
                                    borderAmountColor = Color.Gray;
                                    break;
                            }

                            using (var rowRange = worksheet.Cells[rowIndex, 1, rowIndex, 4])
                            {
                                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rowRange.Style.Fill.BackgroundColor.SetColor(rowColor);
                                rowRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White);
                            }

                            if (amount < 0)
                            {
                                worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Red);
                            }
                            else if (amount > 0)
                            {
                                worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Green);
                            }

                            worksheet.Cells[rowIndex, 3].Value = timestamp;
                            worksheet.Cells[rowIndex, 3].Style.Numberformat.Format = "dd.MM.yyyy HH:mm:ss";
                            worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                            worksheet.Cells[rowIndex, 1].Value = operationType;
                            worksheet.Cells[rowIndex, 2].Value = $"{amount:F2} ₽";
                            worksheet.Cells[rowIndex, 4].Value = adminUsername;
                            rowIndex++;
                        }

                        for (int i = 1; i <= 4; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        FileInfo fileInfo = new FileInfo(dialog.FileName);
                        package.SaveAs(fileInfo);

                        MessageBox.Show("Данные успешно экспортированы в Excel!", "Успех");
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка: Файл уже открыт. Закройте его и повторите попытку.\n{ex.Message}", "Ошибка");
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
                FileName = "CashHistory.docx",
                Filter = "Word files (*.docx)|*.docx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (DocX document = DocX.Create(dialog.FileName))
                    {
                        Paragraph title = document.InsertParagraph("История операций кассы");
                        title.FontSize(16).Bold()
                            .Color(Xceed.Drawing.Color.Black); 

                        if (CashHistoryDataGrid.Items.Count == 0)
                        {
                            MessageBox.Show("Нет данных для экспорта.", "Информация");
                            return;
                        }

                        Table table = document.AddTable(CashHistoryDataGrid.Items.Count + 1, 4);
                        table.Alignment = Alignment.center;

                        table.Rows[0].Cells[0].Paragraphs[0].Append("Тип операции")
                            .FontSize(12).Bold()
                            .Color(Xceed.Drawing.Color.Black);
                        table.Rows[0].Cells[1].Paragraphs[0].Append("Сумма")
                            .FontSize(12).Bold()
                            .Color(Xceed.Drawing.Color.Black);
                        table.Rows[0].Cells[2].Paragraphs[0].Append("Время")
                            .FontSize(12).Bold()
                            .Color(Xceed.Drawing.Color.Black);
                        table.Rows[0].Cells[3].Paragraphs[0].Append("Пользователь")
                            .FontSize(12).Bold()
                            .Color(Xceed.Drawing.Color.Black);

                        int rowIndex = 1;
                        foreach (DataRowView row in CashHistoryDataGrid.ItemsSource)
                        {
                            string operationType = row["OperationType"].ToString();
                            decimal amount = Convert.ToDecimal(row["Amount"]);
                            DateTime timestamp = Convert.ToDateTime(row["Timestamp"]);
                            string adminUsername = row["AdminUsername"].ToString();

                            Xceed.Drawing.Color cellColor = Xceed.Drawing.Color.White;
                            Xceed.Drawing.Color textColor = Xceed.Drawing.Color.White;

                            switch (operationType)
                            {
                                case "Пополнение":
                                    cellColor = new Xceed.Drawing.Color(0xFF00A651u); 
                                    break;
                                case "Продажа":
                                    cellColor = new Xceed.Drawing.Color(0xFF00A651u);
                                    break;
                                case "Снятие":
                                case "Возврат":
                                    cellColor = new Xceed.Drawing.Color(0xFFD4AF37u);
                                    textColor = Xceed.Drawing.Color.Black; 
                                    break;
                                case "Инициализация":
                                    cellColor = new Xceed.Drawing.Color(0xFF4D4D4Du); 
                                    break;
                            }

                            table.Rows[rowIndex].Cells[0].Paragraphs[0].Append(operationType)
                                .FontSize(12)
                                .Color(textColor); 
                            table.Rows[rowIndex].Cells[1].Paragraphs[0].Append($"{amount:F2} ₽")
                                .FontSize(12)
                                .Color(textColor);
                            table.Rows[rowIndex].Cells[2].Paragraphs[0].Append(timestamp.ToString("dd.MM.yyyy HH:mm:ss"))
                                .FontSize(12)
                                .Color(textColor);
                            table.Rows[rowIndex].Cells[3].Paragraphs[0].Append(adminUsername)
                                .FontSize(12)
                                .Color(textColor);

                            foreach (var cell in table.Rows[rowIndex].Cells)
                            {
                                cell.FillColor = cellColor;
                            }

                            rowIndex++;
                        }

                        document.InsertTable(table);
                        document.Save();
                        MessageBox.Show("Данные успешно экспортированы в Word!", "Успех");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

    }

}