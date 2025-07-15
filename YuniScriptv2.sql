USE master;
GO

-- Tạo cơ sở dữ liệu YUni
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'YUni')
BEGIN
    CREATE DATABASE YUni;
END
GO

USE YUni;
GO

-- 1. Tạo bảng Role trước
CREATE TABLE Role (
    roleId INT PRIMARY KEY IDENTITY(1,1),
    roleName VARCHAR(255) NOT NULL UNIQUE,
    description TEXT,
    createdAt DATETIME DEFAULT GETDATE(),
    updatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 2. Tạo bảng Users với roleId là INT (foreign key)
CREATE TABLE Users (
    userId UNIQUEIDENTIFIER PRIMARY KEY,
    fullName VARCHAR(255) NOT NULL,
    userName VARCHAR(255) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    DoB DATE CHECK (DoB <= DATEADD(YEAR, -13, GETDATE())),
    passwordHash VARCHAR(255) NOT NULL,
    lastLogin DATETIME,
    img VARCHAR(255),
    isVerified BIT DEFAULT 0,
    roleId INT,
    createdAt DATETIME DEFAULT GETDATE(),
    updatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Role FOREIGN KEY (roleId) REFERENCES Role(roleId)
);
GO


-- 5. Tạo bảng PaymentMethods
CREATE TABLE PaymentMethods (
    methodId UNIQUEIDENTIFIER PRIMARY KEY,
    methodName VARCHAR(255) NOT NULL UNIQUE,
    isActive BIT DEFAULT 1
);
GO

-- 6. Tạo bảng FinancialAccounts
CREATE TABLE FinancialAccounts (
    accountId UNIQUEIDENTIFIER PRIMARY KEY,
    accountName VARCHAR(255) NOT NULL,
    balance DECIMAL(15,2) DEFAULT 0,
    currencyCode CHAR(3) DEFAULT 'VND',
    userId UNIQUEIDENTIFIER,
    isDefault BIT DEFAULT 0,
    FOREIGN KEY (userId) REFERENCES Users(userId)
);
GO

-- 7. Tạo bảng ExpensesCategories
CREATE TABLE ExpensesCategories (
    exCId UNIQUEIDENTIFIER PRIMARY KEY,
    categoryName VARCHAR(255) NOT NULL UNIQUE,
    description TEXT
);
GO

-- 8. Tạo bảng Expenses
CREATE TABLE Expenses (
    expensesId UNIQUEIDENTIFIER PRIMARY KEY,
    amount DECIMAL(15,2) NOT NULL CHECK (amount > 0),
    description TEXT,
    createdDate DATETIME DEFAULT GETDATE(),
    type VARCHAR(10) NOT NULL CHECK (type IN ('income', 'expense')),
    frequency VARCHAR(10) DEFAULT 'once' CHECK (frequency IN ('once', 'daily', 'weekly', 'monthly')),
    nextDueDate DATE,
    exCId UNIQUEIDENTIFIER,
    accountId UNIQUEIDENTIFIER,
    userId UNIQUEIDENTIFIER,
    FOREIGN KEY (exCId) REFERENCES ExpensesCategories(exCId),
    FOREIGN KEY (accountId) REFERENCES FinancialAccounts(accountId),
    FOREIGN KEY (userId) REFERENCES Users(userId)
);
GO

-- 9. Tạo bảng PaymentGateways
CREATE TABLE PaymentGateways (
    gatewayId UNIQUEIDENTIFIER PRIMARY KEY,
    gatewayName VARCHAR(255) NOT NULL UNIQUE,
    apiKey VARCHAR(255),
    isActive BIT DEFAULT 1
);
GO

-- 10. Tạo bảng Discounts
CREATE TABLE Discounts (
    discountId UNIQUEIDENTIFIER PRIMARY KEY,
    discountName VARCHAR(255) NOT NULL,
    discountPercentage DECIMAL(5,2) CHECK (discountPercentage BETWEEN 0 AND 100),
    isActive BIT DEFAULT 1
);
GO

-- 11. Tạo bảng MembershipPlans
CREATE TABLE MembershipPlans (
    mPId UNIQUEIDENTIFIER PRIMARY KEY,
    planName VARCHAR(255) NOT NULL,
    price DECIMAL(15,2) NOT NULL,
    durationDays INT NOT NULL
);
GO

-- 12. Tạo bảng Invoices
CREATE TABLE Invoices (
    invoiceId UNIQUEIDENTIFIER PRIMARY KEY,
    amount DECIMAL(15,2) NOT NULL,
    taxAmount DECIMAL(15,2) DEFAULT 0,
    discountAmount DECIMAL(15,2) DEFAULT 0,
    totalAmount AS (amount + taxAmount - discountAmount),
    paymentMethodId UNIQUEIDENTIFIER,
    gatewayTransactionId VARCHAR(255),
    createdDate DATETIME DEFAULT GETDATE(),
    updatedDate DATETIME DEFAULT GETDATE(),
    invoiceStatus VARCHAR(10) DEFAULT 'unpaid' CHECK (invoiceStatus IN ('paid', 'unpaid', 'pending', 'refunded')),
    userId UNIQUEIDENTIFIER,
    discountId UNIQUEIDENTIFIER,
    membershipPlanId UNIQUEIDENTIFIER,
    FOREIGN KEY (paymentMethodId) REFERENCES PaymentMethods(methodId),
    FOREIGN KEY (userId) REFERENCES Users(userId),
    FOREIGN KEY (discountId) REFERENCES Discounts(discountId),
    FOREIGN KEY (membershipPlanId) REFERENCES MembershipPlans(mPId)
);
GO

-- 13. Tạo bảng PriorityLevels
CREATE TABLE PriorityLevels (
    priorityId TINYINT PRIMARY KEY,
    levelName VARCHAR(50) NOT NULL UNIQUE,
    colorCode VARCHAR(7)
);

INSERT INTO PriorityLevels (priorityId, levelName, colorCode) VALUES 
(1, 'Urgent', '#FF0000'),
(2, 'High', '#FFA500'),
(3, 'Medium', '#FFFF00'),
(4, 'Low', '#008000');
GO

-- 14. Tạo bảng Subjects
CREATE TABLE Subjects (
    subjectId UNIQUEIDENTIFIER PRIMARY KEY,
    subjectName VARCHAR(255) NOT NULL UNIQUE,
    description TEXT
);
GO

-- 15. Tạo bảng Assignments
CREATE TABLE Assignments (
    assignmentId UNIQUEIDENTIFIER PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    dueDate DATETIME NOT NULL,
    completedDate DATETIME,
    status VARCHAR(20) DEFAULT 'not_started' CHECK (status IN ('not_started', 'in_progress', 'completed', 'overdue')),
    priorityId TINYINT DEFAULT 3,
    estimatedTime INT,
    subjectId UNIQUEIDENTIFIER,
    FOREIGN KEY (priorityId) REFERENCES PriorityLevels(priorityId),
    FOREIGN KEY (subjectId) REFERENCES Subjects(subjectId)
);
GO

-- 16. Tạo bảng EventCategories
CREATE TABLE EventCategories (
    evCategoryId UNIQUEIDENTIFIER PRIMARY KEY,
    categoryName VARCHAR(255) NOT NULL UNIQUE,
    description TEXT
);
GO

-- 17. Tạo bảng Events
CREATE TABLE Events (
    eventId UNIQUEIDENTIFIER PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    startDateTime DATETIME NOT NULL,
    endDateTime DATETIME NOT NULL,
    description TEXT,
    recurrencePattern VARCHAR(10) DEFAULT 'none' CHECK (recurrencePattern IN ('none', 'daily', 'weekly', 'monthly', 'yearly')),
    recurrenceEndDate DATE,
    location GEOGRAPHY,
    evCategoryId UNIQUEIDENTIFIER,
    userId UNIQUEIDENTIFIER,
    CHECK (endDateTime > startDateTime),
    FOREIGN KEY (evCategoryId) REFERENCES EventCategories(evCategoryId),
    FOREIGN KEY (userId) REFERENCES Users(userId)
);
GO

-- 18. Tạo bảng ReminderTemplates
CREATE TABLE ReminderTemplates (
    templateId UNIQUEIDENTIFIER PRIMARY KEY,
    templateName VARCHAR(255) NOT NULL,
    triggerType VARCHAR(20) CHECK (triggerType IN ('before_start', 'after_completion', 'fixed_time')),
    triggerValue INT
);
GO

-- 19. Tạo bảng Reminders
CREATE TABLE Reminders (
    reminderId UNIQUEIDENTIFIER PRIMARY KEY,
    reminderTime DATETIME NOT NULL,
    status VARCHAR(10) DEFAULT 'pending' CHECK (status IN ('pending', 'sent', 'failed')),
    notificationChannel VARCHAR(10) DEFAULT 'push' CHECK (notificationChannel IN ('email', 'push', 'sms')),
    eventId UNIQUEIDENTIFIER,
    assignmentId UNIQUEIDENTIFIER,
    userId UNIQUEIDENTIFIER,
    templateId UNIQUEIDENTIFIER,
    FOREIGN KEY (eventId) REFERENCES Events(eventId),
    FOREIGN KEY (assignmentId) REFERENCES Assignments(assignmentId),
    FOREIGN KEY (userId) REFERENCES Users(userId),
    FOREIGN KEY (templateId) REFERENCES ReminderTemplates(templateId)
);
GO

-- 21. Tạo bảng Goals
CREATE TABLE Goals (
    goalId UNIQUEIDENTIFIER PRIMARY KEY,
    goalName VARCHAR(255) NOT NULL,
    description TEXT,
    targetDate DATE NOT NULL,
    status VARCHAR(20) DEFAULT 'not_started' CHECK (status IN ('not_started', 'in_progress', 'completed')),
    userId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (userId) REFERENCES Users(userId)
);
GO

-- 22. Tạo bảng Budgets
CREATE TABLE Budgets (
    budgetId UNIQUEIDENTIFIER PRIMARY KEY,
    categoryId UNIQUEIDENTIFIER,
    accountId UNIQUEIDENTIFIER,
    budgetAmount DECIMAL(15,2) NOT NULL CHECK (budgetAmount >= 0),
    startDate DATE NOT NULL,
    endDate DATE NOT NULL,
    userId UNIQUEIDENTIFIER NOT NULL,
    CHECK (endDate >= startDate),
    FOREIGN KEY (categoryId) REFERENCES ExpensesCategories(exCId),
    FOREIGN KEY (accountId) REFERENCES FinancialAccounts(accountId),
    FOREIGN KEY (userId) REFERENCES Users(userId)
);
GO

-- 23. Tạo bảng Investments
CREATE TABLE Investments (
    investmentId UNIQUEIDENTIFIER PRIMARY KEY,
    investmentName VARCHAR(255) NOT NULL,
    amount DECIMAL(15,2) NOT NULL CHECK (amount > 0),
    investmentDate DATE NOT NULL,
    maturityDate DATE,
    interestRate DECIMAL(5,2),
    userId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (userId) REFERENCES Users(userId)
);
GO
INSERT INTO Role (roleName, description, createdAt, updatedAt)
VALUES 
    ('Admin', 'Administrator with full access', GETDATE(), GETDATE()),
    ('User', 'Regular user with limited access', GETDATE(), GETDATE());

ALTER TABLE Assignments
ADD userId UNIQUEIDENTIFIER;
GO

ALTER TABLE Assignments
ADD CONSTRAINT FK_Assignments_Users FOREIGN KEY (userId) REFERENCES Users(userId);
GO
ALTER TABLE Users
ADD VerificationCode VARCHAR(255) NULL,
    VerificationCodeExpiry DATETIME NULL;