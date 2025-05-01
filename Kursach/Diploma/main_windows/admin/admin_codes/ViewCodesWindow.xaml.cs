using Kursach.Database.WarehouseApp.Database;
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

namespace Kursach.main_windows
{
    /// <summary>
    /// Логика взаимодействия для ViewCodesWindow.xaml
    /// </summary>
    public partial class ViewCodesWindow : Window
    {
        private string adminUsername;

        public ViewCodesWindow(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
            LoadCodes();
        }

        private void LoadCodes()
        {
            string query = @"
                SELECT Roles.RoleName AS Role, Code, CreatedAt 
                FROM AccessCodes 
                INNER JOIN Roles ON AccessCodes.RoleID = Roles.RoleID 
                WHERE CreatedBy = @CreatedBy";

            DataTable codesTable = DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });

            if (codesTable.Rows.Count > 1)
            {
                codesTable = codesTable.AsEnumerable().Take(2).CopyToDataTable();
            }

            CodesDataGrid.ItemsSource = codesTable.DefaultView;
        }
    }
}
