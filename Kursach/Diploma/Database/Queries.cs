using Kursach.Database.WarehouseApp.Database;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Kursach.Database
{
    public static class Queries
    {
        public static void AddTransaction(
    List<DataRow> selectedProducts,
    decimal total,
    string adminUsername,
    int? customerID = null,
    string status = null,
    string type = "Локальная покупка")
        {
            string transactionQuery = @"
        INSERT INTO Transactions (Type, CustomerID, Status, CreatedBy, Total)
        VALUES (@Type, @CustomerID, @Status, @CreatedBy, @Total);
        SELECT SCOPE_IDENTITY();";

            int transactionId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(transactionQuery, command =>
            {
                command.Parameters.AddWithValue("@Type", type); // Тип: "Продажа" или "Заказ"
                command.Parameters.AddWithValue("@CustomerID", (object)customerID ?? DBNull.Value); // ID клиента (если есть)
                command.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value); // Статус (например, "Оформление")
                command.Parameters.AddWithValue("@CreatedBy", adminUsername); // Администратор
                command.Parameters.AddWithValue("@Total", total); // Общая сумма
            }));

            foreach (var row in selectedProducts)
            {
                string detailQuery = @"
            INSERT INTO TransactionDetails (TransactionID, ProductID, Quantity, Price)
            VALUES (@TransactionID, @ProductID, @Quantity, @Price)";

                DatabaseHelper.ExecuteNonQuery(detailQuery, command =>
                {
                    command.Parameters.AddWithValue("@TransactionID", transactionId); // ID транзакции
                    command.Parameters.AddWithValue("@ProductID", row.Field<int>("ProductID")); // ID товара
                    command.Parameters.AddWithValue("@Quantity", row.Field<int>("OrderQuantity")); // Количество
                    command.Parameters.AddWithValue("@Price", row.Field<decimal>("Price")); // Цена
                });

            }
        }

        public static string GetTransactionStatus(int transactionId)
        {
            string query = "SELECT Status FROM Transactions WHERE TransactionID = @TransactionID";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
            });

            return result?.ToString() ?? string.Empty;
        }

        public static string GetTransactionType(int transactionId)
        {
            string query = "SELECT Type FROM Transactions WHERE TransactionID = @TransactionID";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
            });

            return result?.ToString() ?? string.Empty;
        }



        public static DataTable GetOrders(string adminUsername)
        {
            string query = @"
        SELECT 
            t.TransactionID AS OrderID,
            c.ClientName,
            c.Email,
            c.ContactInfo,
            t.Total AS TotalAmount,
            t.Status
        FROM 
            Transactions t
        JOIN 
            Customers c ON t.CustomerID = c.CustomerID
        WHERE 
            t.Type = 'Заказ' AND t.CreatedBy = @AdminUsername
        ORDER BY 
            t.TransactionTime DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }

        public static int AddOrUpdateCustomer(string name, string email, string contactInfo, string adminUsername)
        {
            string query = @"
IF NOT EXISTS (SELECT 1 FROM Customers WHERE Email = @Email AND AdminUsername = @AdminUsername)
BEGIN
    INSERT INTO Customers (Name, Email, ContactInfo, TotalOrders, AdminUsername)
    VALUES (@Name, @Email, @ContactInfo, 0, @AdminUsername);
END
ELSE
BEGIN
    UPDATE Customers
    SET Name = @Name, ContactInfo = @ContactInfo
    WHERE Email = @Email AND AdminUsername = @AdminUsername;
END

SELECT CustomerID FROM Customers WHERE Email = @Email AND AdminUsername = @AdminUsername;";

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@ContactInfo", contactInfo);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername); // Добавляем параметр AdminUsername
            }));
        }

        public static void AddTransactionOrder(
    List<DataRow> selectedProducts,
    decimal total,
    string adminUsername,
    int? customerID = null,
    string status = "Оформлен",
    string type = "Заказ")
        {
            string transactionQuery = @"
        INSERT INTO Transactions (Type, CustomerID, Status, CreatedBy, Total)
        VALUES (@Type, @CustomerID, @Status, @CreatedBy, @Total);
        SELECT SCOPE_IDENTITY();";

            int transactionId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(transactionQuery, command =>
            {
                command.Parameters.AddWithValue("@Type", type); // Тип: "Продажа" или "Заказ"
                command.Parameters.AddWithValue("@CustomerID", (object)customerID ?? DBNull.Value); // ID клиента (если есть)
                command.Parameters.AddWithValue("@Status", status); // Статус заказа
                command.Parameters.AddWithValue("@CreatedBy", adminUsername); // Администратор
                command.Parameters.AddWithValue("@Total", total); // Общая сумма
            }));

            foreach (var row in selectedProducts)
            {
                string detailQuery = @"
            INSERT INTO TransactionDetails (TransactionID, ProductID, Quantity, Price)
            VALUES (@TransactionID, @ProductID, @Quantity, @Price)";

                DatabaseHelper.ExecuteNonQuery(detailQuery, command =>
                {
                    command.Parameters.AddWithValue("@TransactionID", transactionId); // ID транзакции
                    command.Parameters.AddWithValue("@ProductID", row.Field<int>("ProductID")); // ID товара
                    command.Parameters.AddWithValue("@Quantity", row.Field<int>("OrderQuantity")); // Количество
                    command.Parameters.AddWithValue("@Price", row.Field<decimal>("Price")); // Цена
                });
            }
        }

        public static void UpdateCashAmount(decimal amount, string operationType, string adminUsername, int? transactionId = null)
        {
            string query = @"
        IF NOT EXISTS (SELECT 1 FROM CashRegister WHERE UserUsername = @UserUsername)
        BEGIN
            INSERT INTO CashRegister (UserUsername, Amount, LastUpdate)
            VALUES (@UserUsername, @Amount, GETDATE());
        END
        ELSE
        BEGIN
            UPDATE CashRegister
            SET Amount = Amount + @Amount, LastUpdate = GETDATE()
            WHERE UserUsername = @UserUsername;
        END

        INSERT INTO CashTransactions (Amount, OperationType, AdminUsername, TransactionID)
        VALUES (@Amount, @OperationType, @AdminUsername, @TransactionID);";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@OperationType", operationType);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
                command.Parameters.AddWithValue("@TransactionID", (object)transactionId ?? DBNull.Value);
            });
        }

        public static DataTable GetTransactionsHistory(string adminUsername)
        {
            if (string.IsNullOrWhiteSpace(adminUsername))
            {
                throw new ArgumentException("Имя пользователя администратора не может быть пустым.");
            }

            string query = @"
SELECT 
    t.TransactionID,
    t.Type,
    CASE 
        WHEN c.CustomerID IS NULL THEN 'Локально' -- Если клиент отсутствует
        ELSE c.Name -- Имя клиента
    END AS CustomerName,
    CASE 
        WHEN t.Status IS NULL THEN 'Завершен' -- Если статус отсутствует
        ELSE t.Status
    END AS Status,
    t.Total,
    t.TransactionTime,
    u.Username AS CreatedBy -- Имя администратора, который создал транзакцию
FROM 
    Transactions t
LEFT JOIN 
    Customers c ON t.CustomerID = c.CustomerID -- LEFT JOIN для обработки случаев без клиента
INNER JOIN 
    Users u ON t.CreatedBy = u.Username
WHERE 
    t.CreatedBy = @CreatedBy
ORDER BY 
    t.TransactionTime DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        public static DataTable GetTransactionDetails(int transactionId)
        {
            string query = @"
        SELECT 
            p.Name AS ProductName,
            td.Quantity,
            td.Price
        FROM 
            TransactionDetails td
        JOIN 
            Products p ON td.ProductID = p.ProductID
        WHERE 
            td.TransactionID = @TransactionID";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
            });
        }

        public static decimal GetLoyaltyDiscount(string customerEmail)
        {
            string query = @"
        SELECT 
            MAX(LoyaltyLevelID) AS LoyaltyLevelID,
            DiscountPercentage
        FROM 
            LoyaltyLevels
        WHERE 
            MinOrderAmount <= (SELECT TotalOrders FROM Customers WHERE Email = @CustomerEmail)";

            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@CustomerEmail", customerEmail);
            }) is decimal discount ? discount : 0;
        }

        public static decimal GetPersonalDiscount(string customerEmail)
        {
            string query = @"
        SELECT PersonalDiscount 
        FROM Customers 
        WHERE Email = @CustomerEmail";

            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@CustomerEmail", customerEmail);
            }) is decimal discount ? discount : 0;
        }

        public static void UpdateProductQuantity(string productName, int newQuantity)
        {
            string query = @"
        UPDATE Products 
        SET Quantity = @NewQuantity 
        WHERE Name = @ProductName";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@NewQuantity", newQuantity);
                command.Parameters.AddWithValue("@ProductName", productName);
            });
        }

        public static DataTable GetProductsBySalesFrequency(string adminUsername)
        {
            if (string.IsNullOrEmpty(adminUsername))
            {
                throw new ArgumentException("Имя пользователя администратора не может быть пустым.");
            }

            string query = @"
SELECT 
    p.ProductID, 
    p.Name, 
    p.Brand,
    c.CategoryName, 
    p.Price,
    p.Quantity,
    p.Size,
    p.Composition,
    p.ShelfLife,
    p.DeliveryTime,
    p.MinStockLevel,
    ISNULL(SUM(td.Quantity), 0) AS TotalSoldQuantity
FROM 
    Products p
INNER JOIN 
    Categories c ON p.CategoryID = c.CategoryID
LEFT JOIN 
    TransactionDetails td ON p.ProductID = td.ProductID
WHERE 
    p.CreatedBy = @CreatedBy
GROUP BY 
    p.ProductID, 
    p.Name, 
    p.Brand, 
    c.CategoryName, 
    p.Price, 
    p.Quantity, 
    p.Size, 
    p.Composition, 
    p.ShelfLife, 
    p.DeliveryTime, 
    p.MinStockLevel
ORDER BY 
    TotalSoldQuantity DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        public static decimal CalculateTotalDiscount(string customerEmail)
        {
            decimal loyaltyDiscount = GetLoyaltyDiscount(customerEmail);

            string query = @"
        SELECT PersonalDiscount 
        FROM Customers 
        WHERE Email = @CustomerEmail";

            decimal personalDiscount = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@CustomerEmail", customerEmail);
            }) is decimal discount ? discount : 0;

            return loyaltyDiscount + personalDiscount;
        }

        public static void UpdateCustomerTotalOrders(string customerEmail, decimal newTotalOrders)
        {
            string query = @"
        UPDATE Customers
        SET TotalOrders = @NewTotalOrders
        WHERE Email = @Email";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@NewTotalOrders", newTotalOrders);
                command.Parameters.AddWithValue("@Email", customerEmail);
            });
        }
        public static decimal GetCustomerTotalOrders(string customerEmail)
        {
            string query = @"
        SELECT TotalOrders 
        FROM Customers 
        WHERE Email = @Email";

            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Email", customerEmail);
            }) is decimal totalOrders ? totalOrders : 0;
        }

        public static decimal GetTransactionTotal(int transactionId)
        {
            string query = "SELECT Total FROM Transactions WHERE TransactionID = @TransactionID";
            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
            });

            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        public static void UpdateOrderStatus(int transactionId, string newStatus)
        {
            string query = @"
        UPDATE Transactions 
        SET Status = @NewStatus
        WHERE TransactionID = @TransactionID";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
                command.Parameters.AddWithValue("@NewStatus", newStatus);
            });
        }
        public static int GetCustomerIdByUsername(string username)
        {
            string query = @"
        SELECT CustomerID
        FROM Customers
        WHERE Username = @Username"; // Используйте соответствующее поле

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
            });

            if (result == null)
            {
                throw new Exception("Клиент с указанным именем пользователя не найден.");
            }

            return Convert.ToInt32(result);
        }

        public static void AddSale(List<DataRow> selectedProducts, decimal total, string userUsername, string adminUsername)
        {
            int customerId = GetCustomerIdByUsername(userUsername);

            string insertSaleQuery = @"
        INSERT INTO Sales (UserUsername, Total, SaleTime, AdminUsername)
        VALUES (@UserUsername, @Total, @SaleTime, @AdminUsername);
        SELECT SCOPE_IDENTITY();";

            int saleId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertSaleQuery, command =>
            {
                command.Parameters.AddWithValue("@UserUsername", userUsername);
                command.Parameters.AddWithValue("@Total", total);
                command.Parameters.AddWithValue("@SaleTime", DateTime.Now);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername); 
            }));

            foreach (var row in selectedProducts)
            {
                string insertSaleDetailsQuery = @"
        INSERT INTO SaleDetails (SaleID, ProductName, Quantity, Price)
        VALUES (@SaleID, @ProductName, @Quantity, @Price)";

                DatabaseHelper.ExecuteNonQuery(insertSaleDetailsQuery, command =>
                {
                    command.Parameters.AddWithValue("@SaleID", saleId);
                    command.Parameters.AddWithValue("@ProductName", row.Field<string>("Name"));
                    command.Parameters.AddWithValue("@Quantity", row.Field<int>("OrderQuantity"));
                    command.Parameters.AddWithValue("@Price", row.Field<decimal>("Price"));
                });
            }

            UpdateCustomerTotalOrders(customerId, total);
        }
        public static void AddSale(List<DataRow> selectedProducts, decimal totalAmount, int customerId, string adminUsername)
        {
            string insertOrderQuery = @"
        INSERT INTO Orders (CustomerID, TotalAmount, Status, AdminUsername)
        VALUES (@CustomerID, @TotalAmount, 'Оформление', @AdminUsername)
        SELECT SCOPE_IDENTITY();";

            int orderId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertOrderQuery, command =>
            {
                command.Parameters.AddWithValue("@CustomerID", customerId);
                command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            }));

            foreach (var row in selectedProducts)
            {
                string productName = row.Field<string>("Name");
                int quantity = row.Field<int>("OrderQuantity");
                decimal price = row.Field<decimal>("Price");

                string insertOrderDetailsQuery = @"
            INSERT INTO OrderDetails (OrderID, ProductName, Quantity, Price)
            VALUES (@OrderID, @ProductName, @Quantity, @Price)";

                DatabaseHelper.ExecuteNonQuery(insertOrderDetailsQuery, command =>
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@Price", price);
                });
            }
        }
        public static DataTable GetCustomerByEmail(string email)
        {
            string query = @"
        SELECT *
        FROM Customers
        WHERE Email = @Email";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Email", email);
            });
        }
        public static int GetCustomerIdByEmail(string email)
        {
            string query = @"
        SELECT CustomerID
        FROM Customers
        WHERE Email = @Email";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Email", email);
            });

            if (result == null)
            {
                throw new Exception("Клиент с указанным email не найден.");
            }

            return Convert.ToInt32(result);
        }
        public static void UpdateCustomerTotalOrders(int customerId, decimal totalAmount)
        {
            string query = @"
        UPDATE Customers
        SET TotalOrders = TotalOrders + @TotalAmount
        WHERE CustomerID = @CustomerID";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CustomerID", customerId);
                command.Parameters.AddWithValue("@TotalAmount", totalAmount);
            });
        }
        public static void AddCustomer(string name, string email, string contactInfo, string adminUsername)
        {
            string query = @"
        INSERT INTO Customers (Name, Email, ContactInfo, AdminUsername)
        VALUES (@Name, @Email, @ContactInfo, @AdminUsername)";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@ContactInfo", contactInfo);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }
        public static void RegisterUser(string username, string password, int roleID, string adminUsername, string accessCode = null)
        {
            string hashedPassword = PasswordHelper.HashPassword(password);

            if (roleID != 1)
            {
                if (!IsAccessCodeExists(accessCode, roleID))
                {
                    throw new ArgumentException("Неверный или несуществующий код доступа.");
                }

                int? accessCodeID = GetAccessCodeID(accessCode, roleID);
                if (accessCodeID == null)
                {
                    throw new ArgumentException("Неверный код доступа.");
                }
            }

            string query = @"
        INSERT INTO Users (Username, PasswordHash, RoleID, AccessCodeID, AdminUsername)
        VALUES (@Username, @PasswordHash, @RoleID, @AccessCodeID, @AdminUsername)";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                command.Parameters.AddWithValue("@RoleID", roleID);
                command.Parameters.AddWithValue("@AccessCodeID", accessCode != null ? (int?)GetAccessCodeID(accessCode, roleID) : null);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }
        public static void AddSale(DataTable selectedProducts, decimal totalAmount, string clientEmail, string adminUsername)
        {
            int customerId = GetCustomerIdByEmail(clientEmail);

            string insertOrderQuery = @"
        INSERT INTO Orders (CustomerID, TotalAmount, Status, AdminUsername)
        VALUES (@CustomerID, @TotalAmount, 'Оформление', @AdminUsername)
        SELECT SCOPE_IDENTITY();";

            int orderId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertOrderQuery, command =>
            {
                command.Parameters.AddWithValue("@CustomerID", customerId);
                command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            }));

            foreach (DataRow row in selectedProducts.Rows)
            {
                string productName = row.Field<string>("Name");
                int quantity = row.Field<int>("OrderQuantity");
                decimal price = row.Field<decimal>("Price");

                string insertOrderDetailsQuery = @"
            INSERT INTO OrderDetails (OrderID, ProductName, Quantity, Price)
            VALUES (@OrderID, @ProductName, @Quantity, @Price)";

                DatabaseHelper.ExecuteNonQuery(insertOrderDetailsQuery, command =>
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@Price", price);
                });
            }
        }
        public static DataTable GetOrdersByAdmin(string adminUsername)
        {
            string query = @"
        SELECT 
            t.TransactionID AS OrderID,
            c.Name AS ClientName,
            c.Email,
            c.ContactInfo,
            t.Total AS TotalAmount,
            t.Status,
            t.TransactionTime
        FROM 
            Transactions t
        LEFT JOIN 
            Customers c ON t.CustomerID = c.CustomerID
        WHERE 
            t.Type = 'Заказ' AND 
            t.CreatedBy = @AdminUsername
        ORDER BY 
            t.TransactionTime DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }

        public static DataTable GetOrderDetails(int transactionId)
        {
            string query = @"
SELECT 
    td.ProductID,
    p.Name AS ProductName,
    td.Quantity,
    td.Price
FROM 
    TransactionDetails td
INNER JOIN 
    Products p ON td.ProductID = p.ProductID
WHERE 
    td.TransactionID = @TransactionID";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
            });
        }


        public static DataTable GetProductRecommendations(string username, int currentSaleID)
        {
            string query = @"
        SELECT TOP 5 
            p.Name, 
            p.CategoryName, 
            p.Price, 
            p.Quantity
        FROM Products p
        WHERE p.CategoryID IN (
            SELECT DISTINCT pr.CategoryID
            FROM Sales s
            JOIN SaleDetails sd ON s.SaleID = sd.SaleID
            JOIN Products pr ON sd.ProductName = pr.Name
            WHERE s.UserUsername = @UserUsername
        )
        AND p.Quantity > 0
        AND p.Name NOT IN (
            SELECT ProductName 
            FROM SaleDetails 
            WHERE SaleID = @CurrentSaleID
        )
        ORDER BY p.Quantity DESC;";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@UserUsername", username);
                command.Parameters.AddWithValue("@CurrentSaleID", currentSaleID);
            });
        }
        public static void AddReport(string reportType, DateTime periodStart, DateTime periodEnd, string generatedBy, int? transactionId = null)
        {
            string query = @"
        INSERT INTO Reports (
            ReportType,
            GeneratedDate,
            PeriodStart,
            PeriodEnd,
            GeneratedBy,
            TransactionID
        )
        VALUES (
            @ReportType,
            GETDATE(),
            @PeriodStart,
            @PeriodEnd,
            @GeneratedBy,
            @TransactionID
        )";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@ReportType", reportType);
                command.Parameters.AddWithValue("@PeriodStart", periodStart);
                command.Parameters.AddWithValue("@PeriodEnd", periodEnd);
                command.Parameters.AddWithValue("@GeneratedBy", generatedBy);
                command.Parameters.AddWithValue("@TransactionID", (object)transactionId ?? DBNull.Value);
            });
        }

        public static DataTable GetReports(string adminUsername)
        {
            string query = @"
        SELECT 
            ReportID,
            ReportType,
            GeneratedDate,
            PeriodStart,
            PeriodEnd,
            GeneratedBy,
            TransactionID
        FROM 
            Reports
        WHERE 
            GeneratedBy = @AdminUsername
        ORDER BY 
            GeneratedDate DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }

        public static DataTable GetSalesReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
SELECT 
    t.Total AS 'Сумма продажи',
    t.TransactionTime AS 'Время продажи',
    CONCAT(p.Name, ' ', p.Brand) AS 'Наименование товара',
    td.Quantity AS 'Количество',
    td.Price AS 'Цена за единицу'
FROM 
    Transactions t
LEFT JOIN 
    TransactionDetails td ON t.TransactionID = td.TransactionID
LEFT JOIN 
    Products p ON td.ProductID = p.ProductID
WHERE 
    (t.Type = 'Локальная покупка' OR t.Type = 'Заказ') AND
    t.CreatedBy = @AdminUsername AND 
    t.TransactionTime BETWEEN @StartDate AND @EndDate";


            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
            });
        }

        public static DataTable GetCostsReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
        SELECT 
            OperationType AS 'Тип операции',
            Amount AS 'Сумма',
            Timestamp AS 'Дата'
        FROM 
            CashTransactions
        WHERE 
            AdminUsername = @AdminUsername AND 
            Timestamp BETWEEN @StartDate AND @EndDate AND 
            OperationType IN ('Снятие', 'Возврат')";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
            });
        }

        public static DataTable GetOrderDynamicsReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
        SELECT 
            CAST(t.TransactionTime AS DATE) AS 'Дата продаж',
            COUNT(*) AS 'Всего продаж',
            SUM(t.Total) AS 'Сумма'
        FROM 
            Transactions t
        WHERE 
            t.Type = 'Заказ' AND 
            t.CreatedBy = @AdminUsername AND 
            t.TransactionTime BETWEEN @StartDate AND @EndDate
        GROUP BY 
            CAST(t.TransactionTime AS DATE)
        ORDER BY 
            CAST(t.TransactionTime AS DATE) ASC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
            });
        }

        public static DataTable GetPopularProductsReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
SELECT 
    p.Name AS 'Название',
    p.Brand AS 'Бренд',
    SUM(td.Quantity) AS 'Всего продано'
FROM 
    TransactionDetails td
INNER JOIN 
    Transactions t ON td.TransactionID = t.TransactionID
INNER JOIN 
    Products p ON td.ProductID = p.ProductID
WHERE 
    t.Type = 'Локальная покупка' AND 
    t.CreatedBy = @AdminUsername AND 
    t.TransactionTime BETWEEN @StartDate AND @EndDate 
GROUP BY 
    p.Name,
    p.Brand
HAVING 
    SUM(td.Quantity) > 0
ORDER BY 
    SUM(td.Quantity) DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
            });
        }


        public static DataTable GetCustomerLoyaltyInfo(int customerId)
        {
            string query = @"
        SELECT 
            c.TotalOrders,
            c.LoyaltyLevelID,
            l.LevelName,
            l.DiscountPercentage
        FROM Customers c
        LEFT JOIN LoyaltyLevels l ON c.LoyaltyLevelID = l.LoyaltyLevelID
        WHERE c.CustomerID = @CustomerID";

            return DatabaseHelper.ExecuteQuery(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
            });
        }

        public static DataTable GetLoyaltyLevelByTotalOrders(decimal totalOrders)
        {
            string query = @"
        SELECT TOP 1 LoyaltyLevelID, LevelName, DiscountPercentage
        FROM LoyaltyLevels
        WHERE @TotalOrders >= MinOrderAmount
        ORDER BY MinOrderAmount DESC";

            return DatabaseHelper.ExecuteQuery(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@TotalOrders", totalOrders);
            });
        }



        public static void UpdateCustomerLoyaltyLevel(int customerId, int loyaltyLevelId)
        {
            string query = "UPDATE Customers SET LoyaltyLevelID = @LoyaltyLevelID WHERE CustomerID = @CustomerID";
            DatabaseHelper.ExecuteNonQuery(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@LoyaltyLevelID", loyaltyLevelId);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
            });
        }





        [Obsolete]
        public static DataTable GetProductsForPromotion(int promotionId)
        {
            string query = @"SELECT p.ProductID, p.Name, p.Price 
                     FROM Products p
                     JOIN ProductPromotions pp ON p.ProductID = pp.ProductID
                     WHERE pp.PromotionID = @PromotionID";
            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@PromotionID", promotionId);
            });
        }



        [Obsolete]
        public static DataTable GetLowStockProducts(string adminUsername)
        {
            string query = @"
        SELECT 
            p.Name AS ProductName, 
            c.CategoryName, 
            p.Quantity, 
            p.MinStockLevel,    
            p.Brand
        FROM Products p
        INNER JOIN Categories c ON p.CategoryID = c.CategoryID
        WHERE p.CreatedBy = @CreatedBy AND p.Quantity <= p.MinStockLevel";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        [Obsolete]
        public static string GetAdminForManager(string managerUsername)
        {
            string query = @"
                SELECT AccessCodes.CreatedBy 
                FROM Users 
                INNER JOIN AccessCodes ON Users.AccessCodeID = AccessCodes.CodeID 
                WHERE Users.Username = @ManagerUsername";
            return DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@ManagerUsername", managerUsername);
            })?.ToString();
        }

        public static DataTable GetProductsForSale(string adminUsername)
        {
            if (string.IsNullOrWhiteSpace(adminUsername))
            {
                throw new ArgumentException("Имя пользователя администратора не может быть пустым.");
            }

            string query = @"
SELECT 
    Products.ProductID,
    Products.Name, 
    Categories.CategoryName, 
    Products.Brand, 
    Products.Price, 
    Products.Quantity,
    ISNULL(SUM(TransactionDetails.Quantity), 0) AS TotalSoldQuantity
FROM 
    Products
INNER JOIN 
    Categories ON Products.CategoryID = Categories.CategoryID
LEFT JOIN 
    TransactionDetails ON Products.ProductID = TransactionDetails.ProductID
WHERE 
    Products.CreatedBy = @CreatedBy
GROUP BY 
    Products.ProductID,
    Products.Name, 
    Categories.CategoryName, 
    Products.Brand, 
    Products.Price, 
    Products.Quantity
ORDER BY 
    TotalSoldQuantity DESC";


            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }


        [Obsolete]
        public static DataTable GetProducts(string adminUsername)
        {
            if (string.IsNullOrWhiteSpace(adminUsername))
            {
                throw new ArgumentException("Имя пользователя администратора не может быть пустым.");
            }

            string query = @"
SELECT 
    ProductID, 
    Name, 
    Brand,
    Categories.CategoryName, 
    Price,
    Quantity,
    Size,
    Composition,
    ShelfLife,
    DeliveryTime,
    MinStockLevel
FROM 
    Products
INNER JOIN 
    Categories ON Products.CategoryID = Categories.CategoryID
WHERE 
    Products.CreatedBy = @CreatedBy
ORDER BY 
    ProductID DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }


        [Obsolete]
        public static void UpdateProduct(string originalName, string newName, int categoryID, decimal price, int quantity,
                                  string size, string composition, string shelfLife, string deliveryTime, string createdBy)
        {
            string query = @"
        UPDATE Products
        SET 
            Name = @NewName,
            CategoryID = @CategoryID,
            Price = @Price,
            Quantity = @Quantity,
            Size = @Size,
            Composition = @Composition,
            ShelfLife = @ShelfLife,
            DeliveryTime = @DeliveryTime
        WHERE Name = @OriginalName AND CreatedBy = @CreatedBy";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@OriginalName", originalName);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@CategoryID", categoryID);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Size", size);
                command.Parameters.AddWithValue("@Composition", composition);
                command.Parameters.AddWithValue("@ShelfLife", shelfLife);
                command.Parameters.AddWithValue("@DeliveryTime", deliveryTime);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
            });
        }

        [Obsolete]
        public static decimal GetCurrentCashAmount(string adminUsername)
        {
            string query = @"
        SELECT TOP 1 Amount 
        FROM CashRegister 
        WHERE UserUsername = @AdminUsername 
        ORDER BY LastUpdate DESC";
            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername); 
            });
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        [Obsolete]
        public static void UpdateCashAmount(decimal amount, string operationType, string adminUsername)
        {
            decimal currentAmount = GetCurrentCashAmount(adminUsername);
            if (currentAmount + amount < 0)
            {
                throw new Exception("Недостаточно средств в кассе для выполнения операции.");
            }

            string query = @"
        IF NOT EXISTS (SELECT 1 FROM CashRegister WHERE UserUsername = @AdminUsername)
        BEGIN
            INSERT INTO CashRegister (UserUsername, Amount, LastUpdate) 
            VALUES (@AdminUsername, @Amount, GETDATE());
        END
        ELSE
        BEGIN
            UPDATE CashRegister 
            SET Amount = Amount + @Amount, LastUpdate = GETDATE() 
            WHERE UserUsername = @AdminUsername;
        END
        INSERT INTO CashTransactions (Amount, OperationType, AdminUsername) 
        VALUES (@Amount, @OperationType, @AdminUsername)";
            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@OperationType", operationType);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }

        [Obsolete]
        public static DataTable GetCashTransactions(string userUsername)
        {
            string query = @"
        SELECT 
            OperationType,
            Amount,
            Timestamp,
            AdminUsername
        FROM CashTransactions
        WHERE AdminUsername = @UserUsername
        ORDER BY Timestamp DESC"; 

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@UserUsername", userUsername);
            });
        }

        [Obsolete]
        public static DataTable GetReturnHistory(string adminUsername)
        {
            string query = @"
        SELECT 
            ReturnID,
            ProductName,
            ReturnedQuantity,
            ReturnTime,
            AdminUsername
        FROM Returns
        WHERE AdminUsername = @AdminUsername
        ORDER BY ReturnTime DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }

        [Obsolete]
        public static DataTable GetSaleDetails(int saleId)
        {
            string query = @"
SELECT 
    td.ProductID,
    p.Name AS ProductName,
    td.Quantity,
    p.Price
FROM 
    TransactionDetails td
INNER JOIN 
    Products p ON td.ProductID = p.ProductID
WHERE 
    td.TransactionID = @TransactionID";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", saleId);
            });
        }

        [Obsolete]
        public static void AddReturn(int transactionId, string productName, int quantity, string adminUsername)
        {
            string query = @"
    INSERT INTO Returns (TransactionID, ProductName, ReturnedQuantity, ReturnTime, AdminUsername) 
    VALUES (@TransactionID, @ProductName, @ReturnedQuantity, @ReturnTime, @AdminUsername)";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@ReturnedQuantity", quantity);
                command.Parameters.AddWithValue("@ReturnTime", DateTime.Now);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }


        private static void UpdateCashAmount(int returnQuantity, string productName, string adminUsername)
        {
            decimal productPrice = GetProductPrice(productName);

            decimal refundAmount = productPrice * returnQuantity;

            Queries.UpdateCashAmount(-refundAmount, "Возврат", adminUsername);
        }

        public static void UpdateTransactionTotal(int transactionId)
        {
            string query = @"
UPDATE Transactions
SET Total = (
    SELECT SUM(Quantity * Price)
    FROM TransactionDetails
    WHERE TransactionID = @TransactionID
)
WHERE TransactionID = @TransactionID";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
            });
        }


        private static decimal GetProductPrice(string productName)
        {
            string query = @"
SELECT Price 
FROM Products 
WHERE Name = @ProductName";

            return Convert.ToDecimal(DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@ProductName", productName);
            }));
        }

        private static bool TransactionExists(int transactionId)
        {
            string query = @"
SELECT COUNT(*) 
FROM Transactions 
WHERE TransactionID = @TransactionID";

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@TransactionID", transactionId);
            })) > 0;
        }

        public static void ReturnProduct(string productName, int returnQuantity, int transactionId)
        {
            // Получаем ProductID
            int productId = GetProductIdByName(productName);
            if (productId == -1)
            {
                throw new Exception($"Товар с именем {productName} не найден.");
            }

            string updateProductQuery = @"
UPDATE Products 
SET Quantity = Quantity + @ReturnQuantity 
WHERE ProductID = @ProductID";

            DatabaseHelper.ExecuteNonQuery(updateProductQuery, command =>
            {
                command.Parameters.AddWithValue("@ReturnQuantity", returnQuantity);
                command.Parameters.AddWithValue("@ProductID", productId);
            });

            string updateTransactionDetailsQuery = @"
UPDATE TransactionDetails 
SET Quantity = Quantity - @ReturnQuantity 
WHERE TransactionID = @TransactionID AND ProductID = @ProductID";

            DatabaseHelper.ExecuteNonQuery(updateTransactionDetailsQuery, command =>
            {
                command.Parameters.AddWithValue("@ReturnQuantity", returnQuantity);
                command.Parameters.AddWithValue("@TransactionID", transactionId);
                command.Parameters.AddWithValue("@ProductID", productId);
            });

            UpdateTransactionTotal(transactionId);
        }

        private static int GetProductIdByName(string productName)
        {
            string query = "SELECT ProductID FROM Products WHERE Name = @ProductName";
            object result = DatabaseHelper.ExecuteScalar(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ProductName", productName);
            });

            return result != null ? Convert.ToInt32(result) : -1;
        }


        [Obsolete]
        public static DataTable GetSalesHistory(string adminUsername)
        {
            string query = @"  
            SELECT   
                s.SaleID,  
                s.SaleTime,  
                CASE   
                    WHEN (s.Total - ISNULL((  
                        SELECT SUM(rd.ReturnedQuantity * sd.Price)   
                        FROM Returns rd   
                        JOIN SaleDetails sd ON rd.SaleID = sd.SaleID   
                        WHERE rd.SaleID = s.SaleID  
                    ), 0)) < 0 THEN 0  
                    ELSE (s.Total - ISNULL((  
                        SELECT SUM(rd.ReturnedQuantity * sd.Price)   
                        FROM Returns rd   
                        JOIN SaleDetails sd ON rd.SaleID = sd.SaleID   
                        WHERE rd.SaleID = s.SaleID  
                    ), 0))  
                END AS AdjustedTotal,  
                s.UserUsername,  
                (SELECT STRING_AGG(sd.ProductName + ' (' + CAST(sd.Quantity AS NVARCHAR) + ' шт.)', ', ')   
                 FROM SaleDetails sd   
                 WHERE sd.SaleID = s.SaleID) AS ProductsList  
            FROM Sales s  
            WHERE s.UserUsername = @UserUsername
            ORDER BY s.SaleTime DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@UserUsername", adminUsername);
            });
        }

        [Obsolete]
        public static bool ValidateUser(string username, string passwordHash)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            });
            return Convert.ToInt32(result) > 0;
        }

        [Obsolete]
        public static void AddSale(List<DataRow> selectedProducts, decimal total, string userUsername)
        {
            string insertSaleQuery = @"
                INSERT INTO Sales (UserUsername, Total, SaleTime)
                VALUES (@UserUsername, @Total, @SaleTime);
                SELECT SCOPE_IDENTITY();";

            int saleId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertSaleQuery, command =>
            {
                command.Parameters.AddWithValue("@UserUsername", userUsername);
                command.Parameters.AddWithValue("@Total", total);
                command.Parameters.AddWithValue("@SaleTime", DateTime.Now);
            }));

            foreach (var row in selectedProducts)
            {
                string insertSaleDetailsQuery = @"
                INSERT INTO SaleDetails (SaleID, ProductName, Quantity, Price)
                VALUES (@SaleID, @ProductName, @Quantity, @Price)";

                DatabaseHelper.ExecuteNonQuery(insertSaleDetailsQuery, command =>
                {
                    command.Parameters.AddWithValue("@SaleID", saleId);
                    command.Parameters.AddWithValue("@ProductName", row.Field<string>("Name"));
                    command.Parameters.AddWithValue("@Quantity", row.Field<int>("OrderQuantity"));
                    command.Parameters.AddWithValue("@Price", row.Field<decimal>("Price"));
                });
            }
        }

        public static void AddSaleOrder(List<DataRow> selectedProducts, decimal total, string userUsername)
        {
            try
            {
                string insertTransactionQuery = @"
            INSERT INTO Transactions (Type, Total, CreatedBy, TransactionTime)
            VALUES (@Type, @Total, @CreatedBy, @TransactionTime);
            SELECT SCOPE_IDENTITY();";

                int transactionId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertTransactionQuery, command =>
                {
                    command.Parameters.AddWithValue("@Type", "Продажа");
                    command.Parameters.AddWithValue("@Total", total);
                    command.Parameters.AddWithValue("@CreatedBy", userUsername);
                    command.Parameters.AddWithValue("@TransactionTime", DateTime.Now);
                }));

                foreach (var row in selectedProducts)
                {
                    string insertTransactionDetailsQuery = @"
                INSERT INTO TransactionDetails (TransactionID, ProductID, Quantity, Price)
                VALUES (@TransactionID, @ProductID, @Quantity, @Price)";

                    DatabaseHelper.ExecuteNonQuery(insertTransactionDetailsQuery, command =>
                    {
                        command.Parameters.AddWithValue("@TransactionID", transactionId);
                        command.Parameters.AddWithValue("@ProductID", row.Field<int>("ProductID"));  // <= теперь есть
                        command.Parameters.AddWithValue("@Quantity", row.Field<int>("Quantity"));    // <= правильное имя
                        command.Parameters.AddWithValue("@Price", row.Field<decimal>("Price"));
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заказа: {ex.Message}", "Ошибка");
            }
        }


        [Obsolete]
        private static bool IsAccessCodeExists(string accessCode, int roleID)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM AccessCodes 
        WHERE Code = @Code AND RoleID = @RoleID";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Code", accessCode);
                command.Parameters.AddWithValue("@RoleID", roleID);
            });

            return Convert.ToInt32(result) > 0;
        }

        [Obsolete]
        public static void RegisterUser(string username, string password, int roleID, string accessCode = null)
        {
            string hashedPassword = PasswordHelper.HashPassword(password);

            if (roleID != 1) 
            {
                if (!IsAccessCodeExists(accessCode, roleID))
                {
                    throw new ArgumentException("Неверный или несуществующий код доступа.");
                }

                int? accessCodeID = GetAccessCodeID(accessCode, roleID);
                if (accessCodeID == null)
                {
                    throw new ArgumentException("Неверный код доступа.");
                }
            }

            string query = @"
        INSERT INTO Users (Username, PasswordHash, RoleID, AccessCodeID) 
        VALUES (@Username, @PasswordHash, @RoleID, @AccessCodeID)";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                command.Parameters.AddWithValue("@RoleID", roleID);
                command.Parameters.AddWithValue("@AccessCodeID", roleID == 1 ? DBNull.Value : (object)GetAccessCodeID(accessCode, roleID));
            });
        }

        [Obsolete]
        public static string GetUserPasswordHash(string username)
        {
            string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
            });

            return result?.ToString();
        }

        [Obsolete]
        private static int? GetAccessCodeID(string code, int roleID)
        {
            string query = @"
        SELECT CodeID 
        FROM AccessCodes 
        WHERE Code = @Code AND RoleID = @RoleID";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Code", code);
                command.Parameters.AddWithValue("@RoleID", roleID);
            });

            return result == null ? (int?)null : Convert.ToInt32(result);
        }

        [Obsolete]
        public static void SaveAccessCode(string code, int roleID, string createdBy)
        {
            string query = @"
        INSERT INTO AccessCodes (Code, RoleID, CreatedBy) 
        VALUES (@Code, @RoleID, @CreatedBy)";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Code", code);
                command.Parameters.AddWithValue("@RoleID", roleID);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
            });
        }

        [Obsolete]
        public static bool IsCodeExistsForAdmin(int roleID, string adminUsername)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM AccessCodes 
        WHERE RoleID = @RoleID AND CreatedBy = @CreatedBy";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@RoleID", roleID);
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });

            return Convert.ToInt32(result) > 0;
        }

        [Obsolete]
        public static void UpdateAccessCode(string newCode, int roleID, string adminUsername)
        {
            string query = @"
        UPDATE AccessCodes 
        SET Code = @Code 
        WHERE RoleID = @RoleID AND CreatedBy = @CreatedBy";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Code", newCode);
                command.Parameters.AddWithValue("@RoleID", roleID);
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        [Obsolete]
        public static string? GetUserRole(string username)
        {
            string query = @"
        SELECT Roles.RoleName 
        FROM Users 
        INNER JOIN Roles ON Users.RoleID = Roles.RoleID 
        WHERE Users.Username = @Username";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
            });

            return result?.ToString();
        }

        [Obsolete]
        public static bool ValidateAccessCode(string code, int roleID)
        {
            string query = "SELECT COUNT(*) FROM AccessCodes WHERE Code = @Code AND RoleID = @RoleID";
            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Code", code);
                command.Parameters.AddWithValue("@RoleID", roleID);
            });
            return Convert.ToInt32(result) > 0;
        }

        [Obsolete]
        public static DataTable GetUsersByAdmin(string adminUsername)
        {
            string query = @"
        SELECT Users.Username, Roles.RoleName 
        FROM Users 
        INNER JOIN AccessCodes ON Users.AccessCodeID = AccessCodes.CodeID 
        INNER JOIN Roles ON Users.RoleID = Roles.RoleID 
        WHERE AccessCodes.CreatedBy = @CreatedBy";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        [Obsolete]
        public static void DeleteUser(string username)
        {
            string query = "DELETE FROM Users WHERE Username = @Username";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
            });
        }

        [Obsolete]
        public static void AddPromotion(string promotionName, decimal discountPercentage, DateTime startDate, DateTime endDate, string createdBy)
        {
            string query = @"INSERT INTO Promotions (PromotionName, DiscountPercentage, StartDate, EndDate, CreatedBy) 
                     VALUES (@PromotionName, @DiscountPercentage, @StartDate, @EndDate, @CreatedBy)";
            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@PromotionName", promotionName);
                command.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
            });
        }

        [Obsolete]
        public static void AddProductToPromotion(int promotionId, int productId)
        {
            string query = @"INSERT INTO ProductPromotions (PromotionID, ProductID) 
                     VALUES (@PromotionID, @ProductID)";
            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@PromotionID", promotionId);
                command.Parameters.AddWithValue("@ProductID", productId);
            });
        }

        [Obsolete]
        public static void GenerateReport(string reportType, DateTime periodStart, DateTime periodEnd, string generatedBy, int? saleId = null)
        {
            string query = @"INSERT INTO Reports (ReportType, GeneratedDate, PeriodStart, PeriodEnd, GeneratedBy, SaleID) 
                     VALUES (@ReportType, GETDATE(), @PeriodStart, @PeriodEnd, @GeneratedBy, @SaleID)";
            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@ReportType", reportType);
                command.Parameters.AddWithValue("@PeriodStart", periodStart);
                command.Parameters.AddWithValue("@PeriodEnd", periodEnd);
                command.Parameters.AddWithValue("@GeneratedBy", generatedBy);
                command.Parameters.AddWithValue("@SaleID", saleId.HasValue ? (object)saleId.Value : DBNull.Value);
            });
        }

        [Obsolete]
        public static void AddProduct(string name, int categoryID, decimal price, int quantity,
                             string size, string composition, string shelfLife, string deliveryTime,
                             string createdBy, int minStockLevel, string brand)
        {
            string query = @"
        INSERT INTO Products (
            Name, CategoryID, Price, Quantity, Size, Composition, ShelfLife, DeliveryTime, CreatedBy, MinStockLevel, Brand
        )
        VALUES (
            @Name, @CategoryID, @Price, @Quantity, @Size, @Composition, @ShelfLife, @DeliveryTime, @CreatedBy, @MinStockLevel, @Brand
        )";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@CategoryID", categoryID);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Size", size);
                command.Parameters.AddWithValue("@Composition", composition);
                command.Parameters.AddWithValue("@ShelfLife", shelfLife);
                command.Parameters.AddWithValue("@DeliveryTime", deliveryTime);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
                command.Parameters.AddWithValue("@MinStockLevel", minStockLevel);
                command.Parameters.AddWithValue("@Brand", brand);
            });
        }

        public static void DeleteProduct(int productId, string adminUsername)
        {
            string deleteQuery = @"
    DELETE FROM Products 
    WHERE ProductID = @ProductID AND CreatedBy = @CreatedBy";

            DatabaseHelper.ExecuteNonQuery(deleteQuery, command =>
            {
                command.Parameters.AddWithValue("@ProductID", productId);
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }


        [Obsolete]
        public static void AddCategory(string categoryName, string createdBy)
        {
            string query = @"
        INSERT INTO Categories (CategoryName, CreatedBy) 
        VALUES (@CategoryName, @CreatedBy)";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CategoryName", categoryName);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
            });
        }

        [Obsolete]
        public static DataTable GetCategories(string adminUsername)
        {
            string query = @"
        SELECT CategoryID, CategoryName 
        FROM Categories 
        WHERE CreatedBy = @CreatedBy";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        [Obsolete]
        public static void UpdateProduct(string originalName, string newName, int categoryID, decimal price, int quantity,
                                string size, string composition, string shelfLife, string deliveryTime,
                                string createdBy, int minStockLevel, string brand)
        {
            string query = @"
        UPDATE Products
        SET 
            Name = @NewName,
            CategoryID = @CategoryID,
            Price = @Price,
            Quantity = @Quantity,
            Size = @Size,
            Composition = @Composition,
            ShelfLife = @ShelfLife,
            DeliveryTime = @DeliveryTime,
            MinStockLevel = @MinStockLevel,
            Brand = @Brand
        WHERE Name = @OriginalName AND CreatedBy = @CreatedBy";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@OriginalName", originalName);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@CategoryID", categoryID);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Size", size);
                command.Parameters.AddWithValue("@Composition", composition);
                command.Parameters.AddWithValue("@ShelfLife", shelfLife);
                command.Parameters.AddWithValue("@DeliveryTime", deliveryTime);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
                command.Parameters.AddWithValue("@MinStockLevel", minStockLevel);
                command.Parameters.AddWithValue("@Brand", brand);
            });
        }

        [Obsolete]
        public static DataTable GetProductsByAdmin(string adminUsername)
        {
            string query = @"
        SELECT Products.Name, Categories.CategoryName, Products.Price, Products.Quantity 
        FROM Products 
        INNER JOIN Categories ON Products.CategoryID = Categories.CategoryID 
        WHERE Products.CreatedBy = @CreatedBy";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        [Obsolete]
        public static int? GetUserAccessCodeID(string username)
        {
            string query = @"
        SELECT AccessCodeID 
        FROM Users 
        WHERE Username = @Username";

            object result = DatabaseHelper.ExecuteScalar(query, command =>
            {
                command.Parameters.AddWithValue("@Username", username);
            });

            return result != null ? Convert.ToInt32(result) : (int?)null;
        }
    }
    
}
