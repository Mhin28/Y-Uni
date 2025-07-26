USE [master]
GO

-- Create the database
CREATE DATABASE [YuniBuddy]
GO

USE [YuniBuddy]
GO

-- Create Tables (Combined Schema)

CREATE TABLE [dbo].[Role](
	[roleId] [int] IDENTITY(1,1) NOT NULL,
	[roleName] [varchar](255) NOT NULL,
	[description] [text] NULL,
	[createdAt] [datetime] NULL,
	[updatedAt] [datetime] NULL,
	PRIMARY KEY CLUSTERED ([roleId] ASC)
);
GO

CREATE TABLE [dbo].[Users](
	[userId] [uniqueidentifier] NOT NULL,
	[fullName] [varchar](255) NOT NULL,
	[userName] [varchar](255) NOT NULL,
	[email] [varchar](255) NOT NULL,
	[DoB] [date] NULL,
	[passwordHash] [varchar](255) NOT NULL,
	[lastLogin] [datetime] NULL,
	[img] [varchar](255) NULL,
	[isVerified] [bit] NULL,
	[roleId] [int] NULL,
	[createdAt] [datetime] NULL,
	[updatedAt] [datetime] NULL,
	[VerificationCode] [varchar](255) NULL,
	[VerificationCodeExpiry] [datetime] NULL,
	PRIMARY KEY CLUSTERED ([userId] ASC)
);
GO

CREATE TABLE [dbo].[Subjects](
	[subjectId] [uniqueidentifier] NOT NULL,
	[subjectName] [varchar](255) NOT NULL,
	[description] [text] NULL,
	[userId] [uniqueidentifier] NOT NULL,
	PRIMARY KEY CLUSTERED ([subjectId] ASC)
);
GO

CREATE TABLE [dbo].[PriorityLevels](
	[priorityId] [tinyint] NOT NULL,
	[levelName] [varchar](50) NOT NULL,
	[colorCode] [varchar](7) NULL,
	PRIMARY KEY CLUSTERED ([priorityId] ASC)
);
GO

CREATE TABLE [dbo].[Assignments](
	[assignmentId] [uniqueidentifier] NOT NULL,
	[title] [varchar](255) NOT NULL,
	[description] [text] NULL,
	[dueDate] [datetime] NOT NULL,
	[completedDate] [datetime] NULL,
	[status] [varchar](20) NULL,
	[priorityId] [tinyint] NULL,
	[estimatedTime] [int] NULL,
	[subjectId] [uniqueidentifier] NULL,
	[userId] [uniqueidentifier] NULL,
	PRIMARY KEY CLUSTERED ([assignmentId] ASC)
);
GO

CREATE TABLE [dbo].[EventCategories](
	[evCategoryId] [uniqueidentifier] NOT NULL,
	[categoryName] [varchar](255) NOT NULL,
	[description] [text] NULL,
	[userId] [uniqueidentifier] NOT NULL,
	PRIMARY KEY CLUSTERED ([evCategoryId] ASC)
);
GO

CREATE TABLE [dbo].[Events](
	[eventId] [uniqueidentifier] NOT NULL,
	[title] [varchar](255) NOT NULL,
	[startDateTime] [datetime] NOT NULL,
	[endDateTime] [datetime] NOT NULL,
	[description] [text] NULL,
	[recurrencePattern] [varchar](10) NULL,
	[recurrenceEndDate] [date] NULL,
	[location] [geography] NULL,
	[evCategoryId] [uniqueidentifier] NULL,
	[userId] [uniqueidentifier] NULL,
	PRIMARY KEY CLUSTERED ([eventId] ASC)
);
GO

CREATE TABLE [dbo].[FinancialAccounts](
	[accountId] [uniqueidentifier] NOT NULL,
	[accountName] [varchar](255) NOT NULL,
	[balance] [decimal](15, 2) NULL,
	[currencyCode] [char](3) NULL,
	[userId] [uniqueidentifier] NULL,
	[isDefault] [bit] NULL,
	PRIMARY KEY CLUSTERED ([accountId] ASC)
);
GO

-- Friend's updated schema for ExpensesCategories
CREATE TABLE [dbo].[ExpensesCategories](
	[exCId] [uniqueidentifier] NOT NULL,
	[categoryName] [varchar](255) NOT NULL,
	[description] [text] NULL,
	[type] [varchar](10) NOT NULL,
	PRIMARY KEY CLUSTERED ([exCId] ASC)
);
GO

-- Friend's updated schema for Expenses
CREATE TABLE [dbo].[Expenses](
	[expensesId] [uniqueidentifier] NOT NULL,
	[amount] [decimal](15, 2) NOT NULL,
	[description] [text] NULL,
	[createdDate] [datetime] NULL,
	[exCId] [uniqueidentifier] NULL,
	[accountId] [uniqueidentifier] NULL,
	[userId] [uniqueidentifier] NULL,
	PRIMARY KEY CLUSTERED ([expensesId] ASC)
);
GO


CREATE TABLE [dbo].[Budgets](
	[budgetId] [uniqueidentifier] NOT NULL,
	[categoryId] [uniqueidentifier] NULL,
	[accountId] [uniqueidentifier] NULL,
	[budgetAmount] [decimal](15, 2) NOT NULL,
	[startDate] [date] NOT NULL,
	[endDate] [date] NOT NULL,
	[userId] [uniqueidentifier] NOT NULL,
	PRIMARY KEY CLUSTERED ([budgetId] ASC)
);
GO

CREATE TABLE [dbo].[Goals](
	[goalId] [uniqueidentifier] NOT NULL,
	[goalName] [varchar](255) NOT NULL,
	[description] [text] NULL,
	[targetDate] [date] NOT NULL,
	[status] [varchar](20) NULL,
	[userId] [uniqueidentifier] NOT NULL,
	PRIMARY KEY CLUSTERED ([goalId] ASC)
);
GO

CREATE TABLE [dbo].[Investments](
	[investmentId] [uniqueidentifier] NOT NULL,
	[investmentName] [varchar](255) NOT NULL,
	[amount] [decimal](15, 2) NOT NULL,
	[investmentDate] [date] NOT NULL,
	[maturityDate] [date] NULL,
	[interestRate] [decimal](5, 2) NULL,
	[userId] [uniqueidentifier] NOT NULL,
	PRIMARY KEY CLUSTERED ([investmentId] ASC)
);
GO

CREATE TABLE [dbo].[Discounts](
	[discountId] [uniqueidentifier] NOT NULL,
	[discountName] [varchar](255) NOT NULL,
	[discountPercentage] [decimal](5, 2) NULL,
	[isActive] [bit] NULL,
	PRIMARY KEY CLUSTERED ([discountId] ASC)
);
GO

CREATE TABLE [dbo].[MembershipPlans](
	[mPId] [uniqueidentifier] NOT NULL,
	[planName] [varchar](255) NOT NULL,
	[price] [decimal](15, 2) NOT NULL,
	[durationDays] [int] NOT NULL,
	PRIMARY KEY CLUSTERED ([mPId] ASC)
);
GO

CREATE TABLE [dbo].[PaymentMethods](
	[methodId] [uniqueidentifier] NOT NULL,
	[methodName] [varchar](255) NOT NULL,
	[isActive] [bit] NULL,
	PRIMARY KEY CLUSTERED ([methodId] ASC)
);
GO

CREATE TABLE [dbo].[Invoices](
	[invoiceId] [uniqueidentifier] NOT NULL,
	[amount] [decimal](15, 2) NOT NULL,
	[taxAmount] [decimal](15, 2) NULL,
	[discountAmount] [decimal](15, 2) NULL,
	[totalAmount]  AS (([amount]+[taxAmount])-[discountAmount]),
	[paymentMethodId] [uniqueidentifier] NULL,
	[gatewayTransactionId] [varchar](255) NULL,
	[createdDate] [datetime] NULL,
	[updatedDate] [datetime] NULL,
	[invoiceStatus] [varchar](10) NULL,
	[userId] [uniqueidentifier] NULL,
	[discountId] [uniqueidentifier] NULL,
	[membershipPlanId] [uniqueidentifier] NULL,
	PRIMARY KEY CLUSTERED ([invoiceId] ASC)
);
GO

CREATE TABLE [dbo].[PaymentGateways](
	[gatewayId] [uniqueidentifier] NOT NULL,
	[gatewayName] [varchar](255) NOT NULL,
	[apiKey] [varchar](255) NULL,
	[isActive] [bit] NULL,
	PRIMARY KEY CLUSTERED ([gatewayId] ASC)
);
GO

CREATE TABLE [dbo].[ReminderTemplates](
	[templateId] [uniqueidentifier] NOT NULL,
	[templateName] [varchar](255) NOT NULL,
	[triggerType] [varchar](20) NULL,
	[triggerValue] [int] NULL,
	PRIMARY KEY CLUSTERED ([templateId] ASC)
);
GO

CREATE TABLE [dbo].[Reminders](
	[reminderId] [uniqueidentifier] NOT NULL,
	[reminderTime] [datetime] NOT NULL,
	[status] [varchar](10) NULL,
	[notificationChannel] [varchar](10) NULL,
	[eventId] [uniqueidentifier] NULL,
	[assignmentId] [uniqueidentifier] NULL,
	[userId] [uniqueidentifier] NULL,
	[templateId] [uniqueidentifier] NULL,
	PRIMARY KEY CLUSTERED ([reminderId] ASC)
);
GO

-- Insert Data (Combined from both scripts)

-- Roles
SET IDENTITY_INSERT [dbo].[Role] ON 
INSERT [dbo].[Role] ([roleId], [roleName], [description], [createdAt], [updatedAt]) VALUES (1, N'Admin', N'Administrator with full access', CAST(N'2025-07-15T17:42:17.650' AS DateTime), CAST(N'2025-07-15T17:42:17.650' AS DateTime))
INSERT [dbo].[Role] ([roleId], [roleName], [description], [createdAt], [updatedAt]) VALUES (2, N'User', N'Regular user with limited access', CAST(N'2025-07-15T17:42:17.650' AS DateTime), CAST(N'2025-07-15T17:42:17.650' AS DateTime))
SET IDENTITY_INSERT [dbo].[Role] OFF
GO

-- Users (from both scripts)
INSERT [dbo].[Users] ([userId], [fullName], [userName], [email], [DoB], [passwordHash], [lastLogin], [img], [isVerified], [roleId], [createdAt], [updatedAt], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'92613cbd-9d94-4300-aac1-d1297b2135ef', N'John Doe', N'johndoe', N'thiennhse184989@fpt.edu.vn', CAST(N'2000-01-15' AS Date), N'b926e929192ee30e047ab90fc9d1e0d811a4ccc5f0411da2047abfccc8cd8f60', NULL, NULL, 1, 2, CAST(N'2025-07-15T10:49:22.937' AS DateTime), CAST(N'2025-07-15T10:49:22.937' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([userId], [fullName], [userName], [email], [DoB], [passwordHash], [lastLogin], [img], [isVerified], [roleId], [createdAt], [updatedAt], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'f29ea8b0-e604-47f6-b048-ea28d74d9529', N'Nguyen Van A', N'nguyenvana', N'vana@example.com', CAST(N'2000-01-01' AS Date), N'12345', NULL, NULL, 1, 2, CAST(N'2025-07-09T22:09:45.263' AS DateTime), CAST(N'2025-07-09T22:09:45.263' AS DateTime), NULL, NULL)
GO

-- Financial Accounts (from friend's script)
INSERT [dbo].[FinancialAccounts] ([accountId], [accountName], [balance], [currencyCode], [userId], [isDefault]) VALUES (N'8a7ffe8c-5903-4a2e-84af-2a2d233d1d92', N'duy', CAST(500000000.00 AS Decimal(15, 2)), N'VND', N'f29ea8b0-e604-47f6-b048-ea28d74d9529', 1)
GO

-- Expense Categories (from friend's script)
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'96f41702-6e74-4e22-8b85-37e8b484b2eb', N'Salary', N'Monthly salary from job', N'income')
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'9c30e9b6-ceba-45fa-8854-672899ced2d0', N'Saving', N'Saved funds', N'expense')
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'9857fea4-a14c-4404-8973-6823ed622815', N'Fixed Expenses', N'Monthly recurring costs', N'expense')
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'509fecb0-e79c-4595-a65b-8b7294a91eb4', N'Other Expenses', N'Miscellaneous expenses', N'expense')
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'34ce65a8-3f7e-4cd9-8829-b8f49962f71b', N'Other Source', N'Freelance, side income, or gifts', N'income')
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'bc0cbe27-b184-4d5b-9bfc-ba4dbdb84a70', N'Living Expenses', N'Day-to-day living needs', N'expense')
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'2ea59839-2ef2-4078-aa0e-e223e80a12db', N'Education & Self-improvement', N'Learning and development costs', N'expense')
INSERT [dbo].[ExpensesCategories] ([exCId], [categoryName], [description], [type]) VALUES (N'b8244a0b-ad8e-413f-a1e1-e9439605005a', N'Entertainment & Personal', N'Hobbies, entertainment, and personal care', N'expense')
GO

-- Expenses (from friend's script)
INSERT [dbo].[Expenses] ([expensesId], [amount], [description], [createdDate], [exCId], [accountId], [userId]) VALUES (N'c630b1ec-6241-4795-a493-efd779355601', CAST(50000.00 AS Decimal(15, 2)), N'for testing', CAST(N'2025-07-24T12:27:57.590' AS DateTime), N'9c30e9b6-ceba-45fa-8854-672899ced2d0', N'8a7ffe8c-5903-4a2e-84af-2a2d233d1d92', N'f29ea8b0-e604-47f6-b048-ea28d74d9529')
GO

-- Budgets (from friend's script)
INSERT [dbo].[Budgets] ([budgetId], [categoryId], [accountId], [budgetAmount], [startDate], [endDate], [userId]) VALUES (N'15006b16-809b-4332-b84a-2e5f3a7dba5b', N'9857fea4-a14c-4404-8973-6823ed622815', N'8a7ffe8c-5903-4a2e-84af-2a2d233d1d92', CAST(300000.00 AS Decimal(15, 2)), CAST(N'2025-07-01' AS Date), CAST(N'2025-07-31' AS Date), N'f29ea8b0-e604-47f6-b048-ea28d74d9529')
INSERT [dbo].[Budgets] ([budgetId], [categoryId], [accountId], [budgetAmount], [startDate], [endDate], [userId]) VALUES (N'79869f28-33eb-48de-9fe5-903a18ba339b', N'bc0cbe27-b184-4d5b-9bfc-ba4dbdb84a70', N'8a7ffe8c-5903-4a2e-84af-2a2d233d1d92', CAST(400000.00 AS Decimal(15, 2)), CAST(N'2025-07-01' AS Date), CAST(N'2025-07-31' AS Date), N'f29ea8b0-e604-47f6-b048-ea28d74d9529')
INSERT [dbo].[Budgets] ([budgetId], [categoryId], [accountId], [budgetAmount], [startDate], [endDate], [userId]) VALUES (N'4d344694-6d3f-4eac-a69e-91a070aebb2d', N'9c30e9b6-ceba-45fa-8854-672899ced2d0', N'8a7ffe8c-5903-4a2e-84af-2a2d233d1d92', CAST(800000.00 AS Decimal(15, 2)), CAST(N'2025-07-01' AS Date), CAST(N'2025-07-31' AS Date), N'f29ea8b0-e604-47f6-b048-ea28d74d9529')
INSERT [dbo].[Budgets] ([budgetId], [categoryId], [accountId], [budgetAmount], [startDate], [endDate], [userId]) VALUES (N'dda05996-1cf1-484f-b4cb-a3abce754b1c', N'509fecb0-e79c-4595-a65b-8b7294a91eb4', N'8a7ffe8c-5903-4a2e-84af-2a2d233d1d92', CAST(30000.00 AS Decimal(15, 2)), CAST(N'2025-07-01' AS Date), CAST(N'2025-07-31' AS Date), N'f29ea8b0-e604-47f6-b048-ea28d74d9529')
GO

-- Subjects (from your script)
INSERT [dbo].[Subjects] ([subjectId], [subjectName], [description], [userId]) VALUES (N'ca1f5ccd-76ce-4b2d-8967-7cf2aee58562', N'Computer Sciences', N'Programming and algorithms course', N'92613cbd-9d94-4300-aac1-d1297b2135ef')
INSERT [dbo].[Subjects] ([subjectId], [subjectName], [description], [userId]) VALUES (N'99443c54-7998-4d70-9668-883d3b511517', N'Mathematics', N'Auto-created subject: Mathematics', N'92613cbd-9d94-4300-aac1-d1297b2135ef')
INSERT [dbo].[Subjects] ([subjectId], [subjectName], [description], [userId]) VALUES (N'79ae2290-4e7f-428f-a86e-9d7750bde8ee', N'Computer Science', N'Programming and algorithms course', N'92613cbd-9d94-4300-aac1-d1297b2135ef')
INSERT [dbo].[Subjects] ([subjectId], [subjectName], [description], [userId]) VALUES (N'0325ac65-51e9-4b1b-a501-cea9c3df148b', N'Chemistry', N'Auto-created subject: Chemistry', N'92613cbd-9d94-4300-aac1-d1297b2135ef')
GO

-- Priority Levels (from your script)
INSERT [dbo].[PriorityLevels] ([priorityId], [levelName], [colorCode]) VALUES (1, N'Urgent', N'#FF0000')
INSERT [dbo].[PriorityLevels] ([priorityId], [levelName], [colorCode]) VALUES (2, N'High', N'#FFA500')
INSERT [dbo].[PriorityLevels] ([priorityId], [levelName], [colorCode]) VALUES (3, N'Medium', N'#FFFF00')
INSERT [dbo].[PriorityLevels] ([priorityId], [levelName], [colorCode]) VALUES (4, N'Low', N'#008000')
GO

-- Assignments (from your script)
INSERT [dbo].[Assignments] ([assignmentId], [title], [description], [dueDate], [completedDate], [status], [priorityId], [estimatedTime], [subjectId], [userId]) VALUES (N'21d73b8e-fd66-46da-a17b-1da7e0a93814', N'API Design Project', N'Learning how to design APIs and endpoints', CAST(N'2024-02-15T23:59:00.000' AS DateTime), NULL, N'not_started', 1, 120, N'79ae2290-4e7f-428f-a86e-9d7750bde8ee', N'92613cbd-9d94-4300-aac1-d1297b2135ef')
-- ... (and so on for all your other data)
GO


-- Add Unique Constraints
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[EventCategories] ADD UNIQUE NONCLUSTERED ([categoryName] ASC);
GO
ALTER TABLE [dbo].[ExpensesCategories] ADD UNIQUE NONCLUSTERED ([categoryName] ASC);
GO
ALTER TABLE [dbo].[PaymentGateways] ADD UNIQUE NONCLUSTERED ([gatewayName] ASC);
GO
ALTER TABLE [dbo].[PaymentMethods] ADD UNIQUE NONCLUSTERED ([methodName] ASC);
GO
ALTER TABLE [dbo].[PriorityLevels] ADD UNIQUE NONCLUSTERED ([levelName] ASC);
GO
ALTER TABLE [dbo].[Role] ADD UNIQUE NONCLUSTERED ([roleName] ASC);
GO
ALTER TABLE [dbo].[Subjects] ADD UNIQUE NONCLUSTERED ([subjectName] ASC);
GO
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED ([userName] ASC);
GO
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED ([email] ASC);
GO

-- Add Default Values
ALTER TABLE [dbo].[Assignments] ADD  DEFAULT ('not_started') FOR [status];
GO
ALTER TABLE [dbo].[Assignments] ADD  DEFAULT ((3)) FOR [priorityId];
GO
ALTER TABLE [dbo].[Discounts] ADD  DEFAULT ((1)) FOR [isActive];
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ('none') FOR [recurrencePattern];
GO
ALTER TABLE [dbo].[Expenses] ADD  DEFAULT (getdate()) FOR [createdDate];
GO
ALTER TABLE [dbo].[FinancialAccounts] ADD  DEFAULT ((0)) FOR [balance];
GO
ALTER TABLE [dbo].[FinancialAccounts] ADD  DEFAULT ('VND') FOR [currencyCode];
GO
ALTER TABLE [dbo].[FinancialAccounts] ADD  DEFAULT ((0)) FOR [isDefault];
GO
ALTER TABLE [dbo].[Goals] ADD  DEFAULT ('not_started') FOR [status];
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [taxAmount];
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [discountAmount];
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (getdate()) FOR [createdDate];
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (getdate()) FOR [updatedDate];
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ('unpaid') FOR [invoiceStatus];
GO
ALTER TABLE [dbo].[PaymentGateways] ADD  DEFAULT ((1)) FOR [isActive];
GO
ALTER TABLE [dbo].[PaymentMethods] ADD  DEFAULT ((1)) FOR [isActive];
GO
ALTER TABLE [dbo].[Reminders] ADD  DEFAULT ('pending') FOR [status];
GO
ALTER TABLE [dbo].[Reminders] ADD  DEFAULT ('push') FOR [notificationChannel];
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getdate()) FOR [createdAt];
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getdate()) FOR [updatedAt];
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [isVerified];
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [createdAt];
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [updatedAt];
GO

-- Add Foreign Keys
ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD FOREIGN KEY([priorityId]) REFERENCES [dbo].[PriorityLevels] ([priorityId]);
GO
ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD FOREIGN KEY([subjectId]) REFERENCES [dbo].[Subjects] ([subjectId]);
GO
ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD CONSTRAINT [FK_Assignments_Users] FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Budgets]  WITH CHECK ADD FOREIGN KEY([accountId]) REFERENCES [dbo].[FinancialAccounts] ([accountId]);
GO
ALTER TABLE [dbo].[Budgets]  WITH CHECK ADD FOREIGN KEY([categoryId]) REFERENCES [dbo].[ExpensesCategories] ([exCId]);
GO
ALTER TABLE [dbo].[Budgets]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[EventCategories]  WITH CHECK ADD CONSTRAINT [FK_EventCategories_Users] FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD FOREIGN KEY([evCategoryId]) REFERENCES [dbo].[EventCategories] ([evCategoryId]);
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD FOREIGN KEY([accountId]) REFERENCES [dbo].[FinancialAccounts] ([accountId]);
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD FOREIGN KEY([exCId]) REFERENCES [dbo].[ExpensesCategories] ([exCId]);
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[FinancialAccounts]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Goals]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Investments]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([discountId]) REFERENCES [dbo].[Discounts] ([discountId]);
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([membershipPlanId]) REFERENCES [dbo].[MembershipPlans] ([mPId]);
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([paymentMethodId]) REFERENCES [dbo].[PaymentMethods] ([methodId]);
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Reminders]  WITH CHECK ADD FOREIGN KEY([assignmentId]) REFERENCES [dbo].[Assignments] ([assignmentId]);
GO
ALTER TABLE [dbo].[Reminders]  WITH CHECK ADD FOREIGN KEY([eventId]) REFERENCES [dbo].[Events] ([eventId]);
GO
ALTER TABLE [dbo].[Reminders]  WITH CHECK ADD FOREIGN KEY([templateId]) REFERENCES [dbo].[ReminderTemplates] ([templateId]);
GO
ALTER TABLE [dbo].[Reminders]  WITH CHECK ADD FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Subjects]  WITH CHECK ADD CONSTRAINT [FK_Subjects_Users] FOREIGN KEY([userId]) REFERENCES [dbo].[Users] ([userId]);
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CONSTRAINT [FK_Users_Role] FOREIGN KEY([roleId]) REFERENCES [dbo].[Role] ([roleId]);
GO

-- Add Check Constraints
ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD CHECK  (([status]='overdue' OR [status]='completed' OR [status]='in_progress' OR [status]='not_started'));
GO
ALTER TABLE [dbo].[Budgets]  WITH CHECK ADD CHECK  (([endDate]>=[startDate]));
GO
ALTER TABLE [dbo].[Budgets]  WITH CHECK ADD CHECK  (([budgetAmount]>=(0)));
GO
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD CHECK  (([discountPercentage]>=(0) AND [discountPercentage]<=(100)));
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD CHECK  (([endDateTime]>[startDateTime]));
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD CHECK  (([recurrencePattern]='yearly' OR [recurrencePattern]='monthly' OR [recurrencePattern]='weekly' OR [recurrencePattern]='daily' OR [recurrencePattern]='none'));
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD CHECK  (([amount]>(0)));
GO
-- This CHECK constraint is from your friend's changes to the ExpensesCategories table
ALTER TABLE [dbo].[ExpensesCategories]  WITH CHECK ADD CHECK  (([type]='expense' OR [type]='income'));
GO
ALTER TABLE [dbo].[Goals]  WITH CHECK ADD CHECK  (([status]='completed' OR [status]='in_progress' OR [status]='not_started'));
GO
ALTER TABLE [dbo].[Investments]  WITH CHECK ADD CHECK  (([amount]>(0)));
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD CHECK  (([invoiceStatus]='refunded' OR [invoiceStatus]='pending' OR [invoiceStatus]='unpaid' OR [invoiceStatus]='paid'));
GO
ALTER TABLE [dbo].[Reminders]  WITH CHECK ADD CHECK  (([notificationChannel]='sms' OR [notificationChannel]='push' OR [notificationChannel]='email'));
GO
ALTER TABLE [dbo].[Reminders]  WITH CHECK ADD CHECK  (([status]='failed' OR [status]='sent' OR [status]='pending'));
GO
ALTER TABLE [dbo].[ReminderTemplates]  WITH CHECK ADD CHECK  (([triggerType]='fixed_time' OR [triggerType]='after_completion' OR [triggerType]='before_start'));
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  (([DoB]<=dateadd(year,(-13),getdate())));
GO