CREATE DATABASE WarehouseDB
select * from Customers

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
    Status NVARCHAR(50) DEFAULT '����������',
    OrderTime DATETIME DEFAULT GETDATE(),
    AdminUsername NVARCHAR(50)
);

select * from PromotionRules

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
select * from LoyaltyLevels
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
ALTER TABLE CashTransactions
ADD CashID INT;

ALTER TABLE CashTransactions
ADD CONSTRAINT FK_CashTransactions_CashRegister
FOREIGN KEY (CashID) REFERENCES CashRegister(CashID);

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

INSERT INTO Roles (RoleName) VALUES ('�������������');
INSERT INTO Roles (RoleName) VALUES ('��������');
INSERT INTO Roles (RoleName) VALUES ('������������');

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
    CreatedBy NVARCHAR(50) NOT NULL, -- ������������, ������� ������ �����
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
    GeneratedDate DATETIME NOT NULL DEFAULT GETDATE(),
    PeriodStart DATETIME NOT NULL,
    PeriodEnd DATETIME NOT NULL,
    GeneratedBy NVARCHAR(50) NOT NULL, -- ������������, ������� ����������� �����
    TransactionID INT NULL, -- �����������: ����� � ���������� ����������� (���� ����� � �������� ��� �������)
    FOREIGN KEY (GeneratedBy) REFERENCES Users(Username),
    FOREIGN KEY (TransactionID) REFERENCES Transactions(TransactionID)
);
select * from Customers

ALTER TABLE Products
ADD Brand NVARCHAR(100) NULL;

-- ���������� �������
INSERT INTO Products (Name, Brand, CategoryID, Price, Quantity, Size, Composition, ShelfLife, MinStockLevel, DeliveryTime, CreatedBy)
VALUES 
-- ��������� 1: ���������
('��������� ����', 'Loreal', 34, 1500.00, 50, '30ml', '����, ��������, ��������', 365, 10, 7, 'admin'),
('����� ��� ����', 'Kerastase', 34, 2000.00, 30, '50ml', '��������� ��������', 365, 5, 7, 'admin'),
('���� ��� ������', 'Maybelline', 34, 900.00, 40, '10ml', '��������, ����', 365, 8, 7, 'admin'),
('������ �������', 'Mac', 34, 1200.00, 35, '3g', '�������� ����, ���������', 365, 6, 7, 'admin'),
('����� ����������', 'Chanel', 34, 2500.00, 20, '12g', '��������, �����', 365, 5, 7, 'admin'),

-- ��������� 2: �����������
('������� ��� ��������', 'Staleks', 35, 800.00, 25, '15cm', '�����', 730, 3, 14, 'admin'),
('����� ��� ������', 'GoldenBrush', 35, 1200.00, 20, '20cm', '������, �������', 365, 5, 7, 'admin'),
('����� ��� �������', 'BaByliss', 35, 2500.00, 15, '25cm', '������, ��������', 365, 4, 7, 'admin'),
('������ ��� �����', 'Rowenta', 35, 3000.00, 10, '30cm', '������, ��������', 365, 3, 7, 'admin'),
('������� ��� �������', 'Philips', 35, 4000.00, 8, '15cm', '�������, ������', 365, 2, 7, 'admin'),

-- ��������� 3: ���� �� �����
('���� ��� ���', 'Nivea', 36, 500.00, 50, '75ml', '�����, ��������', 365, 10, 7, 'admin'),
('����� ��� ����', 'The Body Shop', 36, 1200.00, 30, '200ml', '�����, �����', 365, 5, 7, 'admin'),
('����� ��� ����', 'Innisfree', 36, 1500.00, 25, '100ml', '��������� ��������', 365, 6, 7, 'admin'),
('��������� ��� ����', 'SkinCeuticals', 36, 4500.00, 10, '30ml', '������� C, �������������', 365, 3, 7, 'admin'),
('������ ��� ����', 'Bioderma', 36, 1800.00, 40, '250ml', '�����, ���������', 365, 8, 7, 'admin'),

-- ��������� 4: ���� �� ��������
('�������', 'Loreal', 37, 900.00, 60, '250ml', '���������, �����', 365, 12, 7, 'admin'),
('�����������', 'Pantene', 37, 800.00, 55, '200ml', '�����, ��������', 365, 10, 7, 'admin'),
('����� ��� �����', 'Kerastase', 37, 2000.00, 30, '250ml', '��������� ��������', 365, 5, 7, 'admin'),
('��������� ��� ��������', 'Moroccanoil', 37, 1500.00, 25, '100ml', '��������� �����', 365, 6, 7, 'admin'),
('����� ��� �������', 'Tigi', 37, 1200.00, 40, '150ml', '��������, �����', 365, 8, 7, 'admin'),

-- ��������� 5: ������������� ������
('������ �����', 'Oral-B', 38, 300.00, 100, NULL, '�������, ������', 365, 20, 7, 'admin'),
('������ �����', 'Colgate', 38, 200.00, 150, '100ml', '������, �������', 365, 30, 7, 'admin'),
('����������', 'Rexona', 38, 400.00, 80, '150ml', '�������� ����������', 365, 15, 7, 'admin'),
('����', 'Dove', 38, 150.00, 200, '100g', '�����, ���������', 365, 50, 7, 'admin'),
('������� ������ �������', 'Head & Shoulders', 38, 600.00, 70, '250ml', '����, ���������', 365, 10, 7, 'admin');

INSERT INTO Categories (CategoryName, CreatedBy)
VALUES 
('���������', 'admin'),
('�����������', 'admin'),
('���� �� �����', 'admin'),
('���� �� ��������', 'admin'),
('������������� ������', 'admin');

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
DELETE FROM Users

ALTER TABLE Customers
ADD  PersonalDiscount DECIMAL(5, 2) DEFAULT 0;

ALTER TABLE Customers
ADD LoyaltyLevelID INT;
ALTER TABLE Customers
ADD CONSTRAINT FK_Customers_LoyaltyLevels
FOREIGN KEY (LoyaltyLevelID) REFERENCES LoyaltyLevels(LoyaltyLevelID);

CREATE TABLE LoyaltyLevels (
    LoyaltyLevelID INT PRIMARY KEY,
    LevelName NVARCHAR(50),
    MinOrderAmount DECIMAL(18, 2),
    DiscountPercentage DECIMAL(5, 2)
);

ALTER TABLE LoyaltyLevels
ADD LoyaltyLevelID INT IDENTITY(1,1) PRIMARY KEY;

ALTER TABLE LoyaltyLevels
ADD AdminUsername NVARCHAR(50);

ALTER TABLE LoyaltyLevels
ADD CONSTRAINT FK_LoyaltyLevels_Users
FOREIGN KEY (AdminUsername) REFERENCES Users(Username);

Delete from LoyaltyLevels

INSERT INTO LoyaltyLevels (LevelName, MinOrderAmount, DiscountPercentage, AdminUsername) VALUES
('���������', 1000, 5.00, 'admin'),
('����������', 4000, 10.00, 'admin'),
('�������', 7000, 15.00, 'admin');
INSERT INTO LoyaltyLevels (LevelName, MinOrderAmount, DiscountPercentage, AdminUsername)
VALUES ('Gold', 1000, 10, 'admin');
select * from LoyaltyLevels
select * from Customers
CREATE TABLE PromotionRules (
    RuleID INT IDENTITY(1,1) PRIMARY KEY,
    PromotionID INT NOT NULL, -- ������ �� �����
    TargetType NVARCHAR(50) NOT NULL, -- ��� ����: 'Product', 'Brand', 'Category'
    TargetValue NVARCHAR(255) NOT NULL, -- �������� ���� (��������, �������� ������, ������ ��� ���������)
    FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID)
);

ALTER TABLE dbo.Customers
ADD CONSTRAINT FK_Customers_LoyaltyLevels
    FOREIGN KEY (LoyaltyLevelID)
    REFERENCES dbo.LoyaltyLevels (LoyaltyLevelID)
    ON UPDATE CASCADE
    ON DELETE SET NULL;  -- ��� NO ACTION / RESTRICT � ����������� �� ������-������
GO

select * from PromotionRules
select * from Promotions

Delete from PromotionRules
Delete from Promotions

SELECT * FROM Products WHERE Brand = 'Loreal';
SELECT * FROM Products WHERE Name = '��������� ����';
SELECT * FROM Categories WHERE CategoryName = '���������';

DROP TABLE Sales;
DROP TABLE SalesDetails;
DROP TABLE Orders;
DROP TABLE OrderDetails;
select * from SaleDetails

ALTER TABLE Returns
DROP CONSTRAINT FK_Returns_Sales;

Drop table Returns
DROP TABLE SaleDetails;
DROP TABLE Sales;
DROP TABLE OrderDetails;
DROP TABLE Orders;

CREATE TABLE Returns (
    ReturnID INT PRIMARY KEY IDENTITY(1,1),
    TransactionID INT NOT NULL, 
    ProductName NVARCHAR(100) NOT NULL,
    ReturnedQuantity INT NOT NULL,
    ReturnTime DATETIME DEFAULT GETDATE(),
    AdminUsername NVARCHAR(50) NOT NULL,
);

ALTER TABLE Returns
ADD CONSTRAINT FK_Returns_Transactions
FOREIGN KEY (TransactionID) REFERENCES Transactions(TransactionID);

CREATE TABLE Transactions (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),
    Type NVARCHAR(50) NOT NULL CHECK (Type IN ('�������', '�����')),
    CustomerID INT NULL,
    Status NVARCHAR(50) NULL,
    CreatedBy NVARCHAR(50) NOT NULL,
    Total DECIMAL(18, 2) NOT NULL,
    TransactionTime DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Username)
);


delete from TransactionDetails
delete from Transactions
delete from Returns

select * from TransactionDetails
select * from Customers

CREATE TABLE TransactionDetails (
    DetailID INT PRIMARY KEY IDENTITY(1,1),
    TransactionID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (TransactionID) REFERENCES Transactions(TransactionID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

select * from Reports