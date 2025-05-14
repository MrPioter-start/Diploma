using Kursach.Database.WarehouseApp.Database;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Kursach.Database
{
    public static class Queries
    {

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

        public static void UpdateOrderStatus(int orderId, string newStatus)
        {
            string query = @"
        UPDATE Orders
        SET Status = @NewStatus
        WHERE OrderID = @OrderID";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@NewStatus", newStatus);
                command.Parameters.AddWithValue("@OrderID", orderId);
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
            o.OrderID,
            c.Name AS ClientName,
            c.Email,
            c.ContactInfo,
            o.TotalAmount,
            o.Status,
            o.OrderTime
        FROM Orders o
        INNER JOIN Customers c ON o.CustomerID = c.CustomerID
        WHERE o.AdminUsername = @AdminUsername";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
        }
        public static DataTable GetOrders()
        {
            string query = @"
        SELECT OrderID, ClientName, Email, ContactInfo, TotalAmount, Status
        FROM Orders";

            return DatabaseHelper.ExecuteQuery(query, command => { });
        }

        
        public static DataTable GetOrderDetails(int orderId)
        {
            string query = @"
        SELECT 
            ProductName,
            Quantity AS OrderQuantity,
            Price
        FROM 
            OrderDetails
        WHERE 
            OrderID = @OrderID";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@OrderID", orderId);
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

        //public static void AddSale(DataTable selectedProducts, decimal total, string email, string clientName)
        //{
        //    string insertSaleQuery = @"
        //INSERT INTO Sales (UserUsername, Total, SaleTime, ClientEmail, ClientName)
        //VALUES (@UserUsername, @Total, @SaleTime, @ClientEmail, @ClientName);
        //SELECT SCOPE_IDENTITY();";

        //    int saleId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertSaleQuery, command =>
        //    {
        //        command.Parameters.AddWithValue("@UserUsername", "Admin"); // Замените на актуального пользователя
        //        command.Parameters.AddWithValue("@Total", total);
        //        command.Parameters.AddWithValue("@SaleTime", DateTime.Now);
        //        command.Parameters.AddWithValue("@ClientEmail", email);
        //        command.Parameters.AddWithValue("@ClientName", clientName);
        //    }));

        //    foreach (DataRow row in selectedProducts.Rows)
        //    {
        //        string insertSaleDetailsQuery = @"
        //    INSERT INTO SaleDetails (SaleID, ProductName, Quantity, Price)
        //    VALUES (@SaleID, @ProductName, @Quantity, @Price)";

        //        DatabaseHelper.ExecuteNonQuery(insertSaleDetailsQuery, command =>
        //        {
        //            command.Parameters.AddWithValue("@SaleID", saleId);
        //            command.Parameters.AddWithValue("@ProductName", row.Field<string>("Name"));
        //            command.Parameters.AddWithValue("@Quantity", row.Field<int>("OrderQuantity"));
        //            command.Parameters.AddWithValue("@Price", row.Field<decimal>("Price"));
        //        });
        //    }
        //}

        public static void AddReport(string reportType, DateTime periodStart, DateTime periodEnd, string generatedBy)
        {
            string query = @"
        INSERT INTO Reports (
            ReportType,
            GeneratedDate,
            PeriodStart,
            PeriodEnd,
            GeneratedBy
        )
        VALUES (
            @ReportType,
            GETDATE(),
            @PeriodStart,
            @PeriodEnd,
            @GeneratedBy
        )";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@ReportType", reportType);
                command.Parameters.AddWithValue("@PeriodStart", periodStart);
                command.Parameters.AddWithValue("@PeriodEnd", periodEnd);
                command.Parameters.AddWithValue("@GeneratedBy", generatedBy);
            });
        }

        public static DataTable GetReports(string generatedBy)
        {
            string query = @"
        SELECT 
            ReportID,
            ReportType,
            GeneratedDate,
            PeriodStart,
            PeriodEnd
        FROM Reports
        WHERE GeneratedBy = @GeneratedBy";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@GeneratedBy", generatedBy);
            });
        }

        public static DataTable GetSalesReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
        SELECT 
            s.SaleTime AS [Дата продажи],
            s.Total AS [Общая сумма],
            STUFF((
                SELECT CHAR(13) + CHAR(10) + ' - ' + sd.ProductName
                FROM SaleDetails sd
                WHERE sd.SaleID = s.SaleID
                  AND sd.Quantity > 0 -- Исключаем товары с нулевым количеством продаж
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 3, '') AS [Проданные позиции]
        FROM Sales s
        WHERE s.SaleTime BETWEEN @StartDate AND @EndDate
          AND s.UserUsername = @AdminUsername -- Фильтрация по администратору
          AND EXISTS (
              SELECT 1
              FROM SaleDetails sd
              WHERE sd.SaleID = s.SaleID
                AND sd.Quantity > 0 -- Проверяем, что есть хотя бы один товар с ненулевым количеством
          )";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername); // Добавляем параметр
            });
        }

        public static DataTable GetCostsReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
        SELECT 
            Timestamp AS [Дата затрат],
            OperationType AS [Тип затрат],
            Amount AS [Сумма]
        FROM CashTransactions
        WHERE Timestamp BETWEEN @StartDate AND @EndDate
          AND AdminUsername = @AdminUsername -- Фильтрация по администратору
          AND OperationType IN ('Снятие', 'Возврат') -- Учитываем только нужные типы операций
        ORDER BY Timestamp";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername); // Добавляем параметр
            });
        }

        public static DataTable GetOrderDynamicsReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
        SELECT 
            CAST(s.SaleTime AS DATE) AS [Дата],
            COUNT(s.SaleID) AS [Количество заказов]
        FROM Sales s
        JOIN SaleDetails sd ON s.SaleID = sd.SaleID
        WHERE s.SaleTime BETWEEN @StartDate AND @EndDate
          AND s.UserUsername = @AdminUsername -- Фильтрация по администратору
          AND sd.Quantity > 0 -- Исключаем товары с нулевым количеством продаж
        GROUP BY CAST(s.SaleTime AS DATE)
        ORDER BY [Дата]";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername); // Добавляем параметр
            });
        }

        public static DataTable GetPopularProductsReport(DateTime startDate, DateTime endDate, string adminUsername)
        {
            string query = @"
        SELECT TOP 10
            sd.ProductName AS [Название товара],
            SUM(sd.Quantity) AS [Количество продаж]
        FROM SaleDetails sd
        JOIN Sales s ON sd.SaleID = s.SaleID
        WHERE s.SaleTime BETWEEN @StartDate AND @EndDate
          AND s.UserUsername = @AdminUsername -- Фильтрация по администратору
          AND sd.Quantity > 0 -- Исключаем товары с нулевым количеством продаж
        GROUP BY sd.ProductName
        ORDER BY SUM(sd.Quantity) DESC";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername); // Добавляем параметр
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

        [Obsolete]
        public static DataTable GetProducts(string adminUsername)
        {
            string query = @"
        SELECT 
            ProductID, 
            Name, 
            Categories.CategoryName, 
            Price, 
            Quantity, 
            Brand
        FROM 
            Products
        INNER JOIN 
            Categories ON Products.CategoryID = Categories.CategoryID
        WHERE 
            Products.CreatedBy = @CreatedBy";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }

        public static void ApplyPromotions(DataTable productsTable)
        {
            try
            {
                string promotionsQuery = @"
            SELECT 
                p.PromotionID,
                p.DiscountPercentage,
                pr.TargetType,
                pr.TargetValue
            FROM 
                Promotions p
            JOIN 
                PromotionRules pr ON p.PromotionID = pr.PromotionID
            WHERE 
                GETDATE() BETWEEN p.StartDate AND p.EndDate";

                var promotions = DatabaseHelper.ExecuteQuery(promotionsQuery, command => { });

                foreach (DataRow promotionRow in promotions.Rows)
                {
                    decimal discountPercentage = Convert.ToDecimal(promotionRow["DiscountPercentage"]);
                    string targetType = promotionRow["TargetType"].ToString();
                    string targetValue = promotionRow["TargetValue"].ToString();

                    foreach (DataRow productRow in productsTable.Rows)
                    {
                        bool applyDiscount = false;

                        switch (targetType)
                        {
                            case "Product":
                                if (productRow["Name"].ToString() == targetValue)
                                    applyDiscount = true;
                                break;
                            case "Brand":
                                if (productRow["Brand"].ToString() == targetValue)
                                    applyDiscount = true;
                                break;
                            case "Category":
                                if (productRow["CategoryName"].ToString() == targetValue)
                                    applyDiscount = true;
                                break;
                        }

                        if (applyDiscount)
                        {
                            decimal originalPrice = Convert.ToDecimal(productRow["Price"]);
                            decimal discountedPrice = originalPrice * (1 - discountPercentage / 100);
                            productRow["Price"] = Math.Round(discountedPrice, 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при применении акций: {ex.Message}");
            }
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
            ProductName,
            Quantity,
            Price,
            0 AS ReturnQuantity
        FROM SaleDetails
        WHERE SaleID = @SaleID";

            return DatabaseHelper.ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@SaleID", saleId);
            });
        }

        [Obsolete]
        public static void AddReturn(int saleId, string productName, int quantity, string adminUsername)
        {
            string query = @"
        INSERT INTO Returns (SaleID, ProductName, ReturnedQuantity, ReturnTime, AdminUsername) 
        VALUES (@SaleID, @ProductName, @ReturnedQuantity, @ReturnTime, @AdminUsername)";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@SaleID", saleId);
                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@ReturnedQuantity", quantity);
                command.Parameters.AddWithValue("@ReturnTime", DateTime.Now);
                command.Parameters.AddWithValue("@AdminUsername", adminUsername);
            });
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
        public static void ReturnProduct(string productName, int returnQuantity, int saleId)
        {
            string updateProductQuery = @"
        UPDATE Products 
        SET Quantity = Quantity + @ReturnQuantity 
        WHERE Name = @ProductName";

            DatabaseHelper.ExecuteNonQuery(updateProductQuery, command =>
            {
                command.Parameters.AddWithValue("@ReturnQuantity", returnQuantity);
                command.Parameters.AddWithValue("@ProductName", productName);
            });

            string updateSaleDetailsQuery = @"
        UPDATE SaleDetails 
        SET Quantity = Quantity - @ReturnQuantity 
        WHERE SaleID = @SaleID AND ProductName = @ProductName";

            DatabaseHelper.ExecuteNonQuery(updateSaleDetailsQuery, command =>
            {
                command.Parameters.AddWithValue("@ReturnQuantity", returnQuantity);
                command.Parameters.AddWithValue("@SaleID", saleId);
                command.Parameters.AddWithValue("@ProductName", productName);
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
                    command.Parameters.AddWithValue("@ProductName", row.Field<string>("ProductName"));
                    command.Parameters.AddWithValue("@Quantity", row.Field<int>("OrderQuantity")); 
                    command.Parameters.AddWithValue("@Price", row.Field<decimal>("Price"));
                });
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

        [Obsolete]
        public static void DeleteProduct(string productName, string adminUsername)
        {
            string query = @"
        DELETE FROM Products 
        WHERE Name = @Name AND CreatedBy = @CreatedBy";

            DatabaseHelper.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@Name", productName);
                command.Parameters.AddWithValue("@CreatedBy", adminUsername);
            });
        }


        [Obsolete]
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
