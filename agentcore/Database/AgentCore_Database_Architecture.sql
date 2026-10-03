-- ============================================================================
-- AGENTCORE: ENTERPRISE COMMODITY BROKER & COMMISSION AGENT ERP
-- COMPLETE DATABASE ARCHITECTURE & SCHEMA SPECIFICATION
-- Developed by City Computers Marot (03447436314)
-- ============================================================================
-- NOTE: THIS IS THE OFFICIAL CANONICAL DATABASE DESIGN FOR AGENTCORE ERP.
-- DO NOT DEVIATE FROM THIS SCHEMA ARCHITECTURE.
-- ============================================================================

CREATE DATABASE [AgentCore_Broker_ERP];
GO
USE [AgentCore_Broker_ERP];
GO

-- ----------------------------------------------------------------------------
-- 1. SYSTEM SECURITY & USER ACCESS MANAGEMENT
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[Users] (
    [UserID] smallint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserName] varchar(30) NOT NULL UNIQUE,
    [Password] varchar(50) NOT NULL,
    [Narration] varchar(100) NULL,
    [IsAdministrator] bit NOT NULL DEFAULT 0,
    [IsEnabled] bit NOT NULL DEFAULT 1
);
GO

CREATE TABLE [dbo].[Project_Tasks] (
    [TaskKey] varchar(50) NOT NULL PRIMARY KEY,
    [TaskName] varchar(50) NOT NULL,
    [TaskGroup] varchar(60) NOT NULL,
    [IsAutoPost] bit NOT NULL DEFAULT 0,
    [IsPosting] bit NOT NULL DEFAULT 0,
    [Edition] varchar(10) NULL
);
GO

CREATE TABLE [dbo].[UsersTasks] (
    [UserID] smallint NOT NULL,
    [TaskKey] varchar(50) NOT NULL,
    CONSTRAINT [PK_UsersTasks] PRIMARY KEY ([UserID], [TaskKey]),
    CONSTRAINT [FK_UsersTasks_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserID]) ON DELETE CASCADE,
    CONSTRAINT [FK_UsersTasks_Tasks] FOREIGN KEY ([TaskKey]) REFERENCES [dbo].[Project_Tasks]([TaskKey]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[UserHistory] (
    [LogID] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserID] smallint NOT NULL,
    [Description] varchar(255) NOT NULL,
    [LogTime] datetime NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE [dbo].[Project_Registry] (
    [RegistryKey] varchar(50) NOT NULL PRIMARY KEY,
    [ParentRegistryKey] varchar(50) NULL,
    [Narration] varchar(100) NULL,
    [Value] varchar(255) NULL,
    [IsEditable] bit NOT NULL DEFAULT 1
);
GO

CREATE TABLE [dbo].[PortSettings] (
    [RegistryKey] varchar(50) NOT NULL PRIMARY KEY,
    [RegistryValue] varchar(50) NOT NULL,
    [Description] varchar(100) NULL
);
GO

-- ----------------------------------------------------------------------------
-- 2. COMPANY & BRANCH MASTER CONFIGURATION
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[Branch] (
    [BranchID] int NOT NULL PRIMARY KEY,
    [CompanyName] nvarchar(250) NOT NULL,
    [CompanyAddress] nvarchar(500) NULL,
    [CompanyCity] nvarchar(100) NULL,
    [CompanyPhone1] nvarchar(50) NULL,
    [CompanyPhone2] nvarchar(50) NULL,
    [CompanyMobileNo] nvarchar(50) NULL,
    [CompanyNTN] nvarchar(50) NULL,
    [CompanySTN] nvarchar(50) NULL,
    [CompanyWeb] nvarchar(150) NULL,
    [CompanyEmail] nvarchar(150) NULL,
    [CompanyFax] nvarchar(50) NULL,
    [CompanyDescription] nvarchar(500) NULL,
    [ImageData] varbinary(max) NULL,
    [FooterInUrdu] nvarchar(500) NULL,
    [FooterInEnglish] nvarchar(500) NULL,
    [FooterBrokerage] nvarchar(500) NULL,
    [FooterPestro] nvarchar(500) NULL,
    [FooterCrop] nvarchar(500) NULL,
    [CashAccountNo] varchar(10) NULL
);
GO

CREATE TABLE [dbo].[CompanyLogos] (
    [ImageID] smallint NOT NULL PRIMARY KEY,
    [ImageData] varbinary(max) NULL,
    [FooterInUrdu] nvarchar(500) NULL,
    [FooterInEnglish] varchar(500) NULL
);
GO

CREATE TABLE [dbo].[Companies] (
    [CompanyID] varchar(5) NOT NULL PRIMARY KEY,
    [CompanyName] varchar(100) NOT NULL
);
GO

CREATE TABLE [dbo].[Towns] (
    [TownID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TownName] varchar(100) NOT NULL
);
GO

CREATE TABLE [dbo].[Sectors] (
    [SectorID] smallint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TownID] int NOT NULL,
    [SectorName] varchar(100) NOT NULL,
    CONSTRAINT [FK_Sectors_Towns] FOREIGN KEY ([TownID]) REFERENCES [dbo].[Towns]([TownID])
);
GO

CREATE TABLE [dbo].[Units] (
    [UnitID] tinyint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UnitName] varchar(30) NOT NULL,
    [Multiplier] decimal(10, 4) NOT NULL DEFAULT 1.0000
);
GO

-- ----------------------------------------------------------------------------
-- 3. FINANCIAL ACCOUNTING & GENERAL LEDGER
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[ChartOfAccounts] (
    [AccountNo] varchar(10) NOT NULL PRIMARY KEY,
    [UserID] smallint NOT NULL DEFAULT 1,
    [AccountName] varchar(100) NOT NULL,
    [AccountType] varchar(20) NOT NULL,
    [AccountDepth] numeric(3, 0) NULL DEFAULT 1,
    [Narration] varchar(200) NULL,
    [ParentAccountNo] varchar(10) NULL,
    [OpeningDebit] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [OpeningCredit] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [AdjustedDebit] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [AdjustedCredit] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [IsDetailed] bit NOT NULL DEFAULT 1,
    [IsLocked] bit NOT NULL DEFAULT 0,
    [IsPosted] bit NOT NULL DEFAULT 0,
    [IsEditable] bit NOT NULL DEFAULT 1,
    [BalFlag] bit NOT NULL DEFAULT 1,
    [PLFlag] varchar(1) NOT NULL DEFAULT 'N',
    [ExpFlag] bit NOT NULL DEFAULT 0,
    [ExpenseLimit] decimal(18, 2) NULL
);
GO

CREATE TABLE [dbo].[AccountsBalances] (
    [AccountNo] varchar(10) NOT NULL PRIMARY KEY,
    [Debit] decimal(18, 2) NULL DEFAULT 0.00,
    [Credit] decimal(18, 2) NULL DEFAULT 0.00,
    [Bal] decimal(18, 2) NULL DEFAULT 0.00,
    [BalType] varchar(2) NULL
);
GO

CREATE TABLE [dbo].[CreditVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL PRIMARY KEY,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL DEFAULT 1,
    [IsPosted] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[CreditVouchersBody] (
    [SerialNo] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [Narration] varchar(255) NULL,
    [Credit] decimal(18, 2) NOT NULL
);
GO

CREATE TABLE [dbo].[DebitVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL PRIMARY KEY,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL DEFAULT 1,
    [IsPosted] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[DebitVouchersBody] (
    [SerialNo] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [Narration] varchar(255) NULL,
    [Debit] decimal(18, 2) NOT NULL
);
GO

CREATE TABLE [dbo].[JournalVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL PRIMARY KEY,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL DEFAULT 1,
    [IsPosted] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[JournalVouchersBody] (
    [SerialNo] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [Narration] varchar(255) NULL,
    [Debit] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [Credit] decimal(18, 2) NOT NULL DEFAULT 0.00
);
GO

-- ----------------------------------------------------------------------------
-- 4. BANKING & CHEQUE MANAGEMENT
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[BankCheques] (
    [VoucherNo] numeric(10, 0) NOT NULL PRIMARY KEY,
    [VoucherDate] smalldatetime NOT NULL,
    [ChequeNo] varchar(30) NOT NULL,
    [ChequeDate] smalldatetime NULL,
    [ReconcileDate] smalldatetime NULL,
    [BankAccountNo] varchar(10) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [UserNo] smallint NOT NULL,
    [Amount] decimal(18, 2) NOT NULL,
    [ReceivedBy] varchar(100) NULL,
    [Narration] varchar(200) NULL,
    [IsLost] bit NULL DEFAULT 0,
    [IsPosted] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[BankChequesBody] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [Amount] decimal(18, 2) NOT NULL
);
GO

CREATE TABLE [dbo].[BankChequeBookHistory] (
    [AccountNo] varchar(10) NOT NULL,
    [ChequeNo] varchar(50) NOT NULL,
    [EntryDate] smalldatetime NOT NULL,
    CONSTRAINT [PK_BankChequeBookHistory] PRIMARY KEY ([AccountNo], [ChequeNo])
);
GO

CREATE TABLE [dbo].[BankDeposit] (
    [VoucherNo] numeric(10, 0) NOT NULL PRIMARY KEY,
    [VoucherDate] smalldatetime NOT NULL,
    [IsCheque] bit NOT NULL DEFAULT 0,
    [ChequeNo] varchar(30) NULL,
    [ChequeDate] smalldatetime NULL,
    [SlipNo] varchar(30) NULL,
    [DepositedDate] smalldatetime NULL,
    [ReconcileDate] smalldatetime NULL,
    [BankAccountNo] varchar(10) NOT NULL,
    [AccountNo] varchar(10) NULL,
    [UserNo] smallint NOT NULL,
    [Amount] decimal(18, 2) NOT NULL,
    [DepositedBy] varchar(100) NULL,
    [Narration] varchar(200) NULL,
    [IsBounced] bit NOT NULL DEFAULT 0,
    [IsPosted] bit NOT NULL DEFAULT 0
);
GO

-- ----------------------------------------------------------------------------
-- 5. STAKEHOLDERS (PARTIES, GROUPS & SALESMEN)
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[PartyGroups] (
    [PartyGroupID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [GroupName] varchar(100) NOT NULL
);
GO

CREATE TABLE [dbo].[Parties] (
    [PartyID] varchar(10) NOT NULL PRIMARY KEY,
    [PartyName] varchar(100) NOT NULL,
    [ContactPerson] varchar(100) NULL,
    [SectorID] smallint NULL,
    [Address] varchar(250) NOT NULL,
    [City] varchar(100) NULL,
    [Phone1] varchar(25) NULL,
    [Phone2] varchar(25) NULL,
    [Mobile] varchar(25) NULL,
    [Fax] varchar(25) NULL,
    [EMail] varchar(100) NULL,
    [IsCustomer] bit NOT NULL DEFAULT 0,
    [IsVendor] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[Salesmen] (
    [SalesManId] varchar(10) NOT NULL PRIMARY KEY,
    [Address] varchar(200) NOT NULL,
    [City] varchar(100) NOT NULL,
    [HomePhone] varchar(25) NULL,
    [Mobile] varchar(25) NULL,
    [EMail] varchar(100) NULL,
    [Qualification] varchar(50) NULL,
    [Designation] varchar(50) NULL,
    [JoiningDate] datetime NULL,
    [BasicSalary] decimal(18, 2) NULL,
    [TravelExpense] decimal(18, 2) NULL,
    [VehicleAlloted] varchar(50) NULL,
    [Commission] decimal(6, 2) NULL,
    [IncomTax] decimal(6, 2) NULL,
    [OtherTax] decimal(6, 2) NULL,
    [IsMarried] bit NULL
);
GO

-- ----------------------------------------------------------------------------
-- 6. PRODUCTS & INVENTORY DEFINITIONS
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[ProductGroups] (
    [CompanyID] varchar(5) NOT NULL,
    [GroupID] tinyint NOT NULL,
    [GroupName] varchar(100) NOT NULL,
    CONSTRAINT [PK_ProductGroups] PRIMARY KEY ([CompanyID], [GroupID])
);
GO

CREATE TABLE [dbo].[Products] (
    [ProductID] varchar(10) NOT NULL PRIMARY KEY,
    [CompanyID] varchar(5) NOT NULL,
    [GroupID] tinyint NULL,
    [ProductName] varchar(100) NOT NULL,
    [UnitID] tinyint NOT NULL,
    [PurchasePrice] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [SalePrice] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [PurDiscRatio] decimal(6, 2) NOT NULL DEFAULT 0.00,
    [SaleDiscRatio] decimal(6, 2) NOT NULL DEFAULT 0.00,
    [PurDiscValue] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [SaleDiscValue] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [PurSTRatio] decimal(6, 2) NOT NULL DEFAULT 0.00,
    [SaleSTRatio] decimal(6, 2) NOT NULL DEFAULT 0.00,
    [IsCrop] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[CurrentStock] (
    [ProductID] varchar(10) NOT NULL PRIMARY KEY,
    [Qty] decimal(18, 4) NOT NULL DEFAULT 0.0000,
    [Cost] decimal(18, 4) NOT NULL DEFAULT 0.0000
);
GO

CREATE TABLE [dbo].[CurrentStockPestro] (
    [ProductId] varchar(10) NOT NULL,
    [BatchNo] varchar(30) NOT NULL,
    [Quantity] int NOT NULL DEFAULT 0,
    [Cost] decimal(18, 4) NOT NULL DEFAULT 0.0000,
    [ExpiryDate] smalldatetime NOT NULL,
    CONSTRAINT [PK_CurrentStockPestro] PRIMARY KEY ([ProductId], [BatchNo])
);
GO

-- ----------------------------------------------------------------------------
-- 7. MANDI BROKERAGE & CONTRACTS CONSOLE
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[Contract] (
    [ContractID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CDate] datetime NOT NULL DEFAULT GETDATE(),
    [PartyID] varchar(10) NOT NULL,
    [BrokerID] varchar(10) NULL,
    [Refree] varchar(100) NULL,
    [VehicalCharges] decimal(18, 2) NULL,
    [OtherExpence] decimal(18, 2) NULL,
    [Advance] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [IsReached] bit NOT NULL DEFAULT 0,
    [BrokerCommission] decimal(18, 2) NULL,
    [TotalGross] decimal(18, 2) NULL,
    [IsSaleContract] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[ContractBody] (
    [ContractID] int NOT NULL,
    [ProductID] varchar(10) NOT NULL,
    [TruckQty] int NULL,
    [BagsQty] int NULL,
    [WeightPerBag] decimal(18, 4) NOT NULL,
    [RatePerBag] decimal(18, 2) NULL,
    [RatePerMound] decimal(18, 2) NULL,
    [Bardana] int NULL,
    [TotalValue] decimal(18, 2) NOT NULL,
    [TotalKgs] decimal(18, 4) NOT NULL,
    [MoundStd] decimal(10, 4) NOT NULL DEFAULT 40.0000
);
GO

CREATE TABLE [dbo].[ContractInvoice] (
    [ContractPurId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContractID] int NULL,
    [CDate] datetime NULL,
    [PartyID] varchar(10) NOT NULL,
    [BrokerID] varchar(10) NULL,
    [Refree] varchar(100) NULL,
    [IsReached] bit NOT NULL DEFAULT 0,
    [IsPay] bit NULL DEFAULT 0,
    [IsSaleInvoice] bit NOT NULL DEFAULT 0,
    [VehicalCharges] decimal(18, 2) NULL,
    [OtherExpenceMinus] decimal(18, 2) NULL,
    [OtherExpencePlus] decimal(18, 2) NULL,
    [Discount] decimal(18, 2) NULL,
    [Advance] decimal(18, 2) NULL,
    [Received] decimal(18, 2) NULL,
    [CommType] varchar(20) NULL,
    [CommissionRatio] decimal(18, 2) NULL,
    [CommissionValue] decimal(18, 2) NULL,
    [CurDate] datetime NULL
);
GO

CREATE TABLE [dbo].[ContractInvoiceBody] (
    [ContractPurId] int NOT NULL,
    [ProductID] varchar(10) NOT NULL,
    [TruckQty] int NULL,
    [BagsQty] int NULL,
    [WeightPerBag] decimal(18, 4) NULL,
    [RateType] varchar(20) NULL,
    [RatePerMound] decimal(18, 2) NULL,
    [Bardana] int NULL,
    [TotalValue] decimal(18, 2) NULL,
    [TotalKgs] decimal(18, 4) NULL,
    [MoundStd] decimal(10, 4) NULL DEFAULT 40.0000
);
GO

CREATE TABLE [dbo].[BrokrageShortage] (
    [InvoiceID] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [InvoiceDate] smalldatetime NOT NULL,
    [BrokragePurID] int NOT NULL,
    [BrokrageSaleID] int NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [CustomerID] varchar(10) NOT NULL,
    [PurchaseRate] decimal(18, 2) NULL,
    [SaleRate] decimal(18, 2) NULL,
    [ShortageInKgs] decimal(18, 4) NOT NULL,
    [CommissionWeight] decimal(18, 4) NULL,
    [CommissionAmount] decimal(18, 2) NULL
);
GO

-- ----------------------------------------------------------------------------
-- 8. CROPS WEIGHMENT & TRANSACTIONS (KACHA / PAKKA ARTH)
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[CropsWeighments] (
    [WeighmentID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [WeighmentDate] smalldatetime NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [ProductID] varchar(10) NOT NULL,
    [Multiplier] decimal(10, 4) NOT NULL DEFAULT 1.0000,
    [SupplyMode] varchar(50) NULL,
    [SupplyModeQty] smallint NULL,
    [InitialGrossWeight] decimal(18, 4) NOT NULL,
    [TareWeight] decimal(18, 4) NOT NULL,
    [GrossWeight] decimal(18, 4) NOT NULL,
    [Deduction] decimal(18, 4) NOT NULL DEFAULT 0.0000,
    [Shortage] decimal(18, 4) NOT NULL DEFAULT 0.0000,
    [UserID] smallint NOT NULL DEFAULT 1,
    [Remarks] varchar(200) NULL,
    [IsFinalized] bit NOT NULL DEFAULT 0,
    [IsPosted] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[CropsPurchaseHeader] (
    [CropPurchaseID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PurchaseDate] smalldatetime NOT NULL,
    [PurContractID] int NULL,
    [VendorID] varchar(10) NOT NULL,
    [ComRatio] decimal(6, 2) NOT NULL DEFAULT 0.00,
    [ExpPlus] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [ExpMinus] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [UserID] smallint NOT NULL DEFAULT 1,
    [Remarks] varchar(255) NULL,
    [IsPosted] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[CropsPurchaseBody] (
    [SERIALNO] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CropPurchaseID] int NOT NULL,
    [WeighmentID] int NOT NULL,
    [Weight] decimal(18, 4) NOT NULL,
    [Price] decimal(18, 2) NOT NULL
);
GO

-- ----------------------------------------------------------------------------
-- 9. PESTRO AGROCHEMICALS & INVOICING
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[PurchasesPest] (
    [PurchaseId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [LastUpdateDate] smalldatetime NOT NULL,
    [EntryDate] smalldatetime NOT NULL,
    [BillNo] varchar(30) NULL,
    [BillDate] smalldatetime NULL,
    [OrderID] smallint NULL,
    [PreviousCredit] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [Expenses] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [PaidAmount] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [VendorId] varchar(10) NOT NULL,
    [IsPosted] bit NOT NULL DEFAULT 0,
    [IsLocal] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[PurchasesBatch] (
    [PurchaseId] int NOT NULL,
    [ProductId] varchar(10) NOT NULL,
    [BatchNo] varchar(30) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(18, 4) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0
);
GO

CREATE TABLE [dbo].[SalesPest] (
    [SaleId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [SaleType] varchar(10) NOT NULL,
    [ExternalId] int NOT NULL,
    [SalesManId] varchar(10) NULL,
    [CustomerId] varchar(10) NOT NULL,
    [LastUpdateDate] smalldatetime NOT NULL,
    [SaleDate] smalldatetime NOT NULL,
    [IsCashInvoice] bit NOT NULL DEFAULT 0,
    [IsWarranted] bit NOT NULL DEFAULT 0,
    [PreviousDebit] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [IsAutoBatch] bit NOT NULL DEFAULT 0,
    [ReceivedAmount] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [SpecialDiscount] decimal(18, 2) NOT NULL DEFAULT 0.00,
    [DueDate] smalldatetime NOT NULL,
    [IsPrinted] bit NOT NULL DEFAULT 0,
    [Remarks] varchar(255) NULL,
    [IsPosted] bit NOT NULL DEFAULT 0,
    [SSInvNo] int NULL,
    [IsInvoiceClaimable] bit NULL DEFAULT 0,
    [CustomerName] varchar(100) NULL,
    [PhaseNo] varchar(20) NULL
);
GO

CREATE TABLE [dbo].[SalesBatch] (
    [SaleId] int NOT NULL,
    [ProductId] varchar(10) NOT NULL,
    [BatchNo] varchar(30) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(18, 4) NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0
);
GO

-- ----------------------------------------------------------------------------
-- 10. TASK SCHEDULER & RECURRING REMINDERS
-- ----------------------------------------------------------------------------

CREATE TABLE [dbo].[TasksToDo] (
    [SubjectId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserNo] smallint NOT NULL DEFAULT 1,
    [Subject] varchar(100) NOT NULL,
    [Description] varchar(500) NULL,
    [DueDateTime] smalldatetime NULL,
    [Priority] varchar(20) NULL,
    [Status] varchar(30) NULL,
    [RemindDateTime] smalldatetime NULL,
    [Sound] bit NULL DEFAULT 1
);
GO

CREATE TABLE [dbo].[Promises] (
    [PromiseNo] numeric(10, 0) IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [IsPayment] bit NOT NULL DEFAULT 0,
    [CurrentDate] smalldatetime NOT NULL,
    [PromiseDate] smalldatetime NOT NULL,
    [Narration] varchar(255) NULL,
    [Amount] decimal(18, 2) NOT NULL,
    [PartyNo] varchar(10) NOT NULL,
    [UserNo] smallint NOT NULL DEFAULT 1
);
GO

-- ============================================================================
-- END OF AGENTCORE DATABASE ARCHITECTURE SPECIFICATION
-- ============================================================================
