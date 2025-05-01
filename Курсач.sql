CREATE DATABASE WarehouseDB

CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    AccessCode NVARCHAR(50),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

CREATE TABLE Warehouses (
    WarehouseID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    ManagerID INT,
    FOREIGN KEY (ManagerID) REFERENCES Users(UserID)
);

CREATE TABLE Stock (
    StockID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    WarehouseID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID)
);

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    Status NVARCHAR(50) DEFAULT 'Оформление',
    OrderTime DATETIME DEFAULT GETDATE(),
    AdminUsername NVARCHAR(50)
);

ALTER TABLE Orders
ADD CustomerID INT NULL;

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Customers
FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID);

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Users
FOREIGN KEY (AdminUsername) REFERENCES Users(Username);

ALTER TABLE Orders
ADD TotalAmount DECIMAL(18, 2) DEFAULT 0;

select * from Sales
select * from SaleDetails
select * from Customers
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    ProductName NVARCHAR(255),
    Quantity INT,
    Price DECIMAL(18, 2),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255),
    Email NVARCHAR(255) UNIQUE,
    ContactInfo NVARCHAR(255),
    TotalOrders DECIMAL(18, 2) DEFAULT 0,
    AdminUsername NVARCHAR(50),
    FOREIGN KEY (AdminUsername) REFERENCES Users(Username)
);

CREATE TABLE AccessCodes (
    CodeID INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) NOT NULL UNIQUE,
    RoleID INT NOT NULL,
    CreatedBy NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

CREATE TABLE Returns (
    ReturnID INT PRIMARY KEY IDENTITY(1,1),
    SaleID INT NOT NULL, 
    ProductName NVARCHAR(100) NOT NULL,
    ReturnedQuantity INT NOT NULL,
    ReturnTime DATETIME DEFAULT GETDATE(),
    AdminUsername NVARCHAR(50) NOT NULL,
    FOREIGN KEY (SaleID) REFERENCES Sales(SaleID)
);


CREATE TABLE Sales (
    SaleID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    SoldBy NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    SaleDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE SaleDetails (
    DetailID INT PRIMARY KEY IDENTITY(1,1),
    SaleID INT NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (SaleID) REFERENCES Sales(SaleID)
);

CREATE TABLE CashRegister (
    CashID INT PRIMARY KEY IDENTITY(1,1),
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    LastUpdate DATETIME DEFAULT GETDATE()
);

CREATE TABLE CashTransactions (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),
    Amount DECIMAL(18,2) NOT NULL,
    OperationType NVARCHAR(50) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    AdminUsername NVARCHAR(50) NOT NULL,
    FOREIGN KEY (AdminUsername) REFERENCES Users(Username)
);


CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,          
    CategoryID INT NOT NULL,               
    Price DECIMAL(18, 2) NOT NULL,          
    Quantity INT NOT NULL,                
    CreatedBy NVARCHAR(50) NOT NULL,      
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID), 
    FOREIGN KEY (CreatedBy) REFERENCES Users(Username)   
);

select * from Users
SELECT * FROM Sales;
SELECT * FROM CashTransactions;

INSERT INTO Roles (RoleName) VALUES ('Администратор');
INSERT INTO Roles (RoleName) VALUES ('Менеджер');
INSERT INTO Roles (RoleName) VALUES ('Пользователь');

INSERT INTO Users (Username, PasswordHash, RoleID)
VALUES ('admin2', '52575821', 1);

INSERT INTO Users (Username, PasswordHash, RoleID, AccessCode)
VALUES ('manager1', 'hashed_password_here', 2, '1234');

ALTER TABLE Users ADD AccessCodeID INT NULL;
ALTER TABLE Users ADD CONSTRAINT FK_Users_AccessCodes FOREIGN KEY (AccessCodeID) REFERENCES AccessCodes(CodeID);

SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Products';

ALTER TABLE Products ADD Category NVARCHAR(100);
ALTER TABLE Products ADD Quantity INT;
ALTER TABLE Products ADD CreatedBy NVARCHAR(100);

DROP TABLE Warehouses;

CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);

drop table Products
drop table Categories
drop table Sales
drop table Products

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'Username';

SELECT * FROM Returns WHERE AdminUsername = 'admin';

ALTER TABLE Categories ADD CreatedBy NVARCHAR(50);
ALTER TABLE Categories
ADD CONSTRAINT FK_Categories_Users
FOREIGN KEY (CreatedBy) REFERENCES Users(Username);

SELECT Username, AccessCode FROM Users WHERE Username = 'sasha';
ALTER TABLE Users DROP COLUMN AccessCode;

ALTER TABLE Sales 
DROP COLUMN AdminUsername; 

ALTER TABLE Sales 
ADD UserUsername NVARCHAR(50) NOT NULL;

ALTER TABLE Sales 
ADD CONSTRAINT FK_Sales_User 
FOREIGN KEY (UserUsername) REFERENCES Users(Username); 

ALTER TABLE Sales 
ADD UserUsername NVARCHAR(50) NOT NULL;

ALTER TABLE Sales 
ADD CONSTRAINT DF_UserUsername DEFAULT 'default_user' FOR UserUsername;

ALTER TABLE CashRegister 
ADD UserUsername NVARCHAR(50) NOT NULL; 

ALTER TABLE CashRegister 
ADD CONSTRAINT FK_CashRegister_User 
FOREIGN KEY (UserUsername) REFERENCES Users(Username); 

ALTER TABLE CashTransactions 
ADD CONSTRAINT FK_CashTransactions_User 
FOREIGN KEY (AdminUsername) REFERENCES Users(Username); 

SELECT * FROM CashRegister WHERE UserUsername = 'admin';

SELECT * FROM CashTransactions WHERE AdminUsername = 'admin';

ALTER TABLE Users
ADD Preferences NVARCHAR(MAX) NULL,
    LoyaltyLevel INT DEFAULT 0,
    PersonalDiscount DECIMAL(5,2) DEFAULT 0.00;

ALTER TABLE Products
ADD Size NVARCHAR(50) NULL,
    Composition NVARCHAR(MAX) NULL,
    ShelfLife INT NOT NULL DEFAULT 0,
    MinStockLevel INT NOT NULL DEFAULT 0,
    DeliveryTime INT NOT NULL DEFAULT 0;

ALTER TABLE CashRegister
ADD LastUpdate DATETIME NOT NULL DEFAULT GETDATE();

ALTER TABLE Returns
ADD ReturnReason NVARCHAR(MAX) NULL;

CREATE TABLE Promotions (
    PromotionID INT IDENTITY(1,1) PRIMARY KEY,
    PromotionName NVARCHAR(100) NOT NULL,
    DiscountPercentage DECIMAL(5,2) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    CreatedBy NVARCHAR(50) NOT NULL, -- Пользователь, который создал акцию
    FOREIGN KEY (CreatedBy) REFERENCES Users(Username)
);

CREATE TABLE ProductPromotions (
    ProductPromotionID INT IDENTITY(1,1) PRIMARY KEY,
    PromotionID INT NOT NULL,
    ProductID INT NOT NULL,
    FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE Reports (
    ReportID INT IDENTITY(1,1) PRIMARY KEY,
    ReportType NVARCHAR(50) NOT NULL,
    GeneratedDate DATETIME NOT NULL,
    PeriodStart DATETIME NOT NULL,
    PeriodEnd DATETIME NOT NULL,
    GeneratedBy NVARCHAR(50) NOT NULL, -- Пользователь, который сформировал отчет
    SaleID INT NULL, -- Опционально: связь с конкретной продажей (если отчет о продажах)
    FOREIGN KEY (GeneratedBy) REFERENCES Users(Username),
    FOREIGN KEY (SaleID) REFERENCES Sales(SaleID)
);

ALTER TABLE Products
ADD Brand NVARCHAR(100) NULL;

-- Добавление товаров
INSERT INTO Products (Name, Brand, CategoryID, Price, Quantity, Size, Composition, ShelfLife, MinStockLevel, DeliveryTime, CreatedBy)
VALUES 
-- Категория 1: Косметика
('Тональный крем', 'Loreal', 21, 1500.00, 50, '30ml', 'Вода, минералы, пигменты', 365, 10, 7, 'admin'),
('Маска для лица', 'Kerastase', 21, 2000.00, 30, '50ml', 'Экстракты растений', 365, 5, 7, 'admin'),
('Тушь для ресниц', 'Maybelline', 21, 900.00, 40, '10ml', 'Пигменты, воск', 365, 8, 7, 'admin'),
('Помада матовая', 'Mac', 21, 1200.00, 35, '3g', 'Пчелиный воск, красители', 365, 6, 7, 'admin'),
('Пудра компактная', 'Chanel', 21, 2500.00, 20, '12g', 'Минералы, тальк', 365, 5, 7, 'admin'),

-- Категория 2: Инструменты
('Ножницы для маникюра', 'Staleks', 22, 800.00, 25, '15cm', 'Сталь', 730, 3, 14, 'admin'),
('Фреза для ногтей', 'GoldenBrush', 22, 1200.00, 20, '20cm', 'Металл, пластик', 365, 5, 7, 'admin'),
('Щипцы для завивки', 'BaByliss', 22, 2500.00, 15, '25cm', 'Металл, керамика', 365, 4, 7, 'admin'),
('Плойка для волос', 'Rowenta', 22, 3000.00, 10, '30cm', 'Металл, керамика', 365, 3, 7, 'admin'),
('Машинка для стрижки', 'Philips', 22, 4000.00, 8, '15cm', 'Пластик, металл', 365, 2, 7, 'admin'),

-- Категория 3: Уход за кожей
('Крем для рук', 'Nivea', 23, 500.00, 50, '75ml', 'Масла, витамины', 365, 10, 7, 'admin'),
('Скраб для тела', 'The Body Shop', 23, 1200.00, 30, '200ml', 'Сахар, масла', 365, 5, 7, 'admin'),
('Маска для лица', 'Innisfree', 23, 1500.00, 25, '100ml', 'Экстракты растений', 365, 6, 7, 'admin'),
('Сыворотка для кожи', 'SkinCeuticals', 23, 4500.00, 10, '30ml', 'Витамин C, антиоксиданты', 365, 3, 7, 'admin'),
('Лосьон для тела', 'Bioderma', 23, 1800.00, 40, '250ml', 'Масла, экстракты', 365, 8, 7, 'admin'),

-- Категория 4: Уход за волосами
('Шампунь', 'Loreal', 24, 900.00, 60, '250ml', 'Экстракты, масла', 365, 12, 7, 'admin'),
('Кондиционер', 'Pantene', 24, 800.00, 55, '200ml', 'Масла, протеины', 365, 10, 7, 'admin'),
('Маска для волос', 'Kerastase', 24, 2000.00, 30, '250ml', 'Экстракты растений', 365, 5, 7, 'admin'),
('Сыворотка для кончиков', 'Moroccanoil', 24, 1500.00, 25, '100ml', 'Аргановое масло', 365, 6, 7, 'admin'),
('Спрей для укладки', 'Tigi', 24, 1200.00, 40, '150ml', 'Полимеры, масла', 365, 8, 7, 'admin'),

-- Категория 5: Гигиенические товары
('Зубная щетка', 'Oral-B', 25, 300.00, 100, NULL, 'Пластик, щетина', 365, 20, 7, 'admin'),
('Зубная паста', 'Colgate', 25, 200.00, 150, '100ml', 'Фторид, кальций', 365, 30, 7, 'admin'),
('Дезодорант', 'Rexona', 25, 400.00, 80, '150ml', 'Алюминий хлоргидрат', 365, 15, 7, 'admin'),
('Мыло', 'Dove', 25, 150.00, 200, '100g', 'Масла, экстракты', 365, 50, 7, 'admin'),
('Шампунь против перхоти', 'Head & Shoulders', 25, 600.00, 70, '250ml', 'Цинк, экстракты', 365, 10, 7, 'admin');

INSERT INTO Categories (CategoryName, CreatedBy)
VALUES 
('Косметика', 'admin'),
('Инструменты', 'admin'),
('Уход за кожей', 'admin'),
('Уход за волосами', 'admin'),
('Гигиенические товары', 'admin');

SELECT * FROM Categories;
SELECT * FROM Products

DROP TABLE Promotions
DROP TABLE Reports
DELETE FROM Reports
DELETE FROM CashTransactions 
DELETE FROM CashRegister
DELETE FROM Returns
DELETE FROM Sales
DELETE FROM SaleDetails
DELETE FROM Products
DELETE FROM Categories
DELETE FROM Returns
DELETE FROM Customers
DELETE FROM Orders
DELETE FROM OrderDetails

