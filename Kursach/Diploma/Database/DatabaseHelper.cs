using System;
using System.Data;
using System.Data.SqlClient;

namespace Kursach.Database
{
    namespace WarehouseApp.Database
    {
        public static class DatabaseHelper
        {
            private static string connectionString = "Server=KLARDEAD;Database=WarehouseDB;Trusted_Connection=True;";

            [Obsolete]
            public static SqlConnection OpenConnection()
            {
                SqlConnection connection = new SqlConnection(connectionString);
                try
                {
                    connection.Open();
                    return connection;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при подключении к базе данных: {ex.Message}");
                    throw;
                }
            }

            [Obsolete]
            public static void CloseConnection(SqlConnection connection)
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            [Obsolete]
            public static void ExecuteNonQuery(string query, Action<SqlCommand> configureCommand)
            {
                using (SqlConnection connection = OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        configureCommand?.Invoke(command);
                        command.ExecuteNonQuery();
                    }
                }
            }

            [Obsolete]
            public static object ExecuteScalar(string query, Action<SqlCommand> configureCommand)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        configureCommand?.Invoke(command);
                        return command.ExecuteScalar();
                    }
                }
            }

            [Obsolete]
            public static DataTable ExecuteQuery(string query, Action<SqlCommand> configureCommand)
            {
                DataTable table = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        configureCommand?.Invoke(command);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(table); 
                        }
                    }
                }
                return table;
            }
        }
    }
}
