-- ============================================================================
-- AUTOMATIC SCHEMA SYNCHRONIZATION FROM ATUPDATE.DLL MASTER SPECIFICATION
-- Target Database: [MyMandi_2025-26]
-- Generated Date: 2026-10-11 12:28:27
-- Total Tables to Create: 78
-- ============================================================================
USE [MyMandi_2025-26];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AccountsBalances')
BEGIN
    CREATE TABLE [dbo].[AccountsBalances] (
    [AccountNo] varchar(10) NOT NULL,
    [Debit] decimal(14, 2) NULL,
    [Credit] decimal(14, 2) NULL,
    [Bal] decimal(14, 2) NULL,
    [BalType] varchar(2) NULL
    );
    PRINT 'Created table: [AccountsBalances]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BankChequeBookHistory')
BEGIN
    CREATE TABLE [dbo].[BankChequeBookHistory] (
    [AccountNo] varchar(10) NOT NULL,
    [ChequeNo] varchar(50) NOT NULL,
    [EntryDate] smalldatetime NOT NULL,
    [IsCancelled] bit NULL,
    [Remarks] varchar(150) NULL,
    [CancelDate] smalldatetime NULL
    );
    PRINT 'Created table: [BankChequeBookHistory]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BankCheques')
BEGIN
    CREATE TABLE [dbo].[BankCheques] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [ChequeNo] varchar(15) NOT NULL,
    [ChequeDate] smalldatetime NULL,
    [ReconcileDate] smalldatetime NULL,
    [BankAccountNo] varchar(10) NOT NULL,
    [AccountNo] varchar(10) NULL,
    [UserNo] smallint NOT NULL,
    [Amount] decimal(11, 2) NOT NULL,
    [ReceivedBy] nvarchar(1000) NULL,
    [Narration] varchar(1000) NULL,
    [IsLost] bit NULL,
    [IsPosted] bit NOT NULL,
    [WHTAx] numeric(9, 0) NULL,
    [PMFlag] varchar(1) NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [WHTtoBank] bit NULL,
    [WHTtoPayee] bit NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_BankCheques] PRIMARY KEY CLUSTERED ([VoucherNo] ASC, [VoucherDate] ASC)
    );
    PRINT 'Created table: [BankCheques]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BankChequesBody')
BEGIN
    CREATE TABLE [dbo].[BankChequesBody] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [Amount] decimal(11, 2) NOT NULL
    );
    PRINT 'Created table: [BankChequesBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BankDeposit')
BEGIN
    CREATE TABLE [dbo].[BankDeposit] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [IsCheque] bit NOT NULL,
    [ChequeNo] varchar(15) NULL,
    [ChequeDate] smalldatetime NULL,
    [SlipNo] varchar(15) NULL,
    [DepositedDate] smalldatetime NULL,
    [ReconcileDate] smalldatetime NULL,
    [BankAccountNo] varchar(10) NOT NULL,
    [AccountNo] varchar(10) NULL,
    [UserNo] smallint NOT NULL,
    [Amount] decimal(11, 2) NOT NULL,
    [DepositedBy] varchar(30) NULL,
    [Narration] varchar(1000) NULL,
    [IsBounced] bit NOT NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_BankDeposit] PRIMARY KEY CLUSTERED ([VoucherNo] ASC, [VoucherDate] ASC)
    );
    PRINT 'Created table: [BankDeposit]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Branch')
BEGIN
    CREATE TABLE [dbo].[Branch] (
    [BranchID] int NOT NULL,
    [CompanyName] nvarchar(1000) NOT NULL,
    [CompanyAddress] nvarchar(1000) NULL,
    [CompanyCity] nvarchar(1000) NULL,
    [CompanyPhone1] nvarchar(1000) NULL,
    [CompanyPhone2] nvarchar(1000) NULL,
    [CompanyMobileNo] nvarchar(1000) NULL,
    [CompanyNTN] nvarchar(1000) NULL,
    [CompanySTN] nvarchar(1000) NULL,
    [CompanyWeb] nvarchar(1000) NULL,
    [CompanyEmail] nvarchar(1000) NULL,
    [CompanyFax] nvarchar(1000) NULL,
    [CompanyDescription] nvarchar(1000) NULL,
    [ImageData] image NULL,
    [FooterInUrdu] nvarchar(1000) NULL,
    [FooterInEnglish] nvarchar(1000) NULL,
    [FooterBrokerage] nvarchar(1000) NULL,
    [FooterPestro] nvarchar(1000) NULL,
    [FooterCrop] nvarchar(1000) NULL,
    [CashAccountNo] varchar(10) NULL,
    CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED ([BranchID] ASC)
    );
    PRINT 'Created table: [Branch]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BrokerageGatePass')
BEGIN
    CREATE TABLE [dbo].[BrokerageGatePass] (
    [GatePassID] int NOT NULL,
    [GatePassDate] datetime NULL,
    [PartyID] varchar(10) NOT NULL,
    [CustomerID] varchar(10) NULL,
    [IsSale] bit NOT NULL,
    [Narration] varchar(500) NULL,
    [GatePassNo] varchar(15) NULL,
    [BillNo] varchar(10) NULL,
    [SGPID] int NULL,
    [PGPID] int NULL,
    CONSTRAINT [PK_BrokerageGatePass] PRIMARY KEY CLUSTERED ([GatePassID] ASC)
    );
    PRINT 'Created table: [BrokerageGatePass]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BrokerageGatePassBody')
BEGIN
    CREATE TABLE [dbo].[BrokerageGatePassBody] (
    [GatePassID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [VehicleNo] varchar(20) NULL,
    [BagsQty] decimal(18, 2) NULL,
    [PQty] decimal(18, 2) NULL,
    [WeightPerBag] decimal(18, 2) NULL,
    [RatePerMound] decimal(10, 2) NULL,
    [TotalKgs] decimal(18, 2) NULL,
    [IsSale] bit NULL,
    [IsDeleted] bit NULL,
    [SerialNo] bigint NULL,
    [MoundStd] decimal(7, 3) NULL,
    [SGPID] int NULL,
    [PGPID] int NULL
    );
    PRINT 'Created table: [BrokerageGatePassBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BrokrageShortage')
BEGIN
    CREATE TABLE [dbo].[BrokrageShortage] (
    [InvoiceID] bigint NOT NULL,
    [InvoiceDate] smalldatetime NOT NULL,
    [BrokragePurID] int NOT NULL,
    [BrokrageSaleID] int NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [CustomerID] varchar(10) NOT NULL,
    [PurchaseRate] decimal(11, 5) NULL,
    [SaleRate] decimal(11, 5) NULL,
    [ShortageInKgs] numeric(18, 0) NOT NULL,
    [CommissionWeight] numeric(5, 2) NULL,
    [CommissionAmount] numeric(18, 0) NULL,
    [Narration] varchar(200) NULL
    );
    PRINT 'Created table: [BrokrageShortage]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CashNoteDetail')
BEGIN
    CREATE TABLE [dbo].[CashNoteDetail] (
    [SerialNo] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [VoucherType] varchar(3) NOT NULL,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [R5000] decimal(11, 0) NOT NULL,
    [R1000] decimal(11, 0) NOT NULL,
    [R500] decimal(11, 0) NOT NULL,
    [R100] decimal(11, 0) NOT NULL,
    [R50] decimal(11, 0) NOT NULL,
    [R20] decimal(11, 0) NOT NULL,
    [R10] decimal(11, 0) NOT NULL,
    [R5] decimal(11, 0) NOT NULL,
    [R2] decimal(11, 0) NOT NULL,
    [R1] decimal(11, 0) NOT NULL,
    CONSTRAINT [PK_CashNoteDetail] PRIMARY KEY CLUSTERED ([SerialNo] ASC)
    );
    PRINT 'Created table: [CashNoteDetail]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChartOfAccounts')
BEGIN
    CREATE TABLE [dbo].[ChartOfAccounts] (
    [AccountNo] varchar(10) NOT NULL,
    [UserID] smallint NOT NULL,
    [AccountType] varchar(11) NOT NULL,
    [AccountDepth] numeric(3, 0) NULL,
    [Narration] varchar(50) NULL,
    [ParentAccountNo] varchar(10) NULL,
    [OpeningDebit] decimal(13, 2) NOT NULL DEFAULT ((0)),
    [OpeningCredit] decimal(13, 2) NOT NULL DEFAULT ((0)),
    [AdjustedDebit] decimal(13, 2) NOT NULL DEFAULT ((0)),
    [AdjustedCredit] decimal(13, 2) NOT NULL DEFAULT ((0)),
    [IsDetailed] bit NOT NULL,
    [IsLocked] bit NOT NULL DEFAULT ((0)),
    [IsPosted] bit NOT NULL DEFAULT ((0)),
    [IsEditable] bit NOT NULL DEFAULT ((1)),
    [BalFlag] bit NOT NULL DEFAULT ((0)),
    [PLFlag] varchar(1) NOT NULL DEFAULT (' '),
    [ExpFlag] bit NOT NULL DEFAULT ((0)),
    [ExpenseLimit] decimal(13, 2) NULL,
    [AccountName] varchar(150) NULL,
    [IsHidden] bit NULL,
    [Bank] nvarchar(50) NULL,
    [AccountNameUrdu] nvarchar(250) NULL,
    [CropSeason] varchar(5) NULL,
    CONSTRAINT [PK_ChartOfAccounts] PRIMARY KEY CLUSTERED ([AccountNo] ASC)
    );
    PRINT 'Created table: [ChartOfAccounts]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CompanyLogos')
BEGIN
    CREATE TABLE [dbo].[CompanyLogos] (
    [ImageID] smallint NOT NULL,
    [ImageData] image NULL,
    [FooterInUrdu] nvarchar(1000) NULL,
    [FooterInEnglish] varchar(250) NULL,
    [FooterBrokerage] nvarchar(1000) NULL,
    [FooterPestro] nvarchar(1000) NULL,
    [FooterCrop] nvarchar(1000) NULL
    );
    PRINT 'Created table: [CompanyLogos]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComRecords')
BEGIN
    CREATE TABLE [dbo].[ComRecords] (
    [SrNo] numeric(18, 0) IDENTITY(1,1) NOT NULL,
    [FromNo] varchar(20) NULL,
    [ToNo] varchar(20) NULL,
    [Body] nvarchar(3500) NULL,
    [SourceType] varchar(50) NULL,
    [ComTime] datetime NOT NULL,
    [IsSMS] bit NOT NULL,
    [IsProcessed] bit NOT NULL,
    [UnitID] varchar(20) NULL,
    [IsWebRequest] bit NULL
    );
    PRINT 'Created table: [ComRecords]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComSettings')
BEGIN
    CREATE TABLE [dbo].[ComSettings] (
    [ComID] tinyint NOT NULL,
    [ComPortName] varchar(20) NOT NULL,
    [ComPortNo] tinyint NOT NULL,
    [ControlNo] varchar(15) NOT NULL,
    [ServiceNo] varchar(15) NOT NULL,
    [BaudRate] varchar(30) NULL,
    [SMSIndication] varchar(30) NULL,
    [CSet] varchar(30) NULL,
    [IsSMSOnly] bit NULL
    );
    PRINT 'Created table: [ComSettings]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Contract')
BEGIN
    CREATE TABLE [dbo].[Contract] (
    [ContractID] int NOT NULL,
    [CDate] datetime NULL,
    [PartyID] varchar(10) NOT NULL,
    [BrokerID] varchar(10) NULL,
    [Refree] varchar(30) NULL,
    [VehicalCharges] decimal(18, 0) NULL,
    [OtherExpence] decimal(18, 0) NULL,
    [Advance] decimal(18, 0) NOT NULL,
    [IsReached] bit NOT NULL,
    [BrokerCommission] decimal(18, 0) NULL,
    [TotalGross] numeric(18, 0) NULL,
    [IsSaleContract] bit NOT NULL,
    [Narration] varchar(200) NULL,
    [SaleContractId] int NULL,
    [PurContractId] int NULL,
    [IsCompleted] bit NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL,
    [IDBranchPur] bigint NULL,
    [IDBranchSale] bigint NULL,
    CONSTRAINT [PK_Contract] PRIMARY KEY CLUSTERED ([ContractID] ASC)
    );
    PRINT 'Created table: [Contract]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContractBody')
BEGIN
    CREATE TABLE [dbo].[ContractBody] (
    [ContractID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [TruckQty] int NULL,
    [BagsQty] int NULL,
    [WeightPerBag] decimal(20, 0) NULL,
    [RatePerBag] nvarchar(50) NULL,
    [RatePerMound] int NULL,
    [Bardana] int NULL,
    [TotalValue] decimal(20, 0) NULL,
    [TotalKgs] decimal(20, 0) NULL,
    [MoundStd] smallint NULL,
    [ExtraWeight] decimal(12, 2) NULL
    );
    PRINT 'Created table: [ContractBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContractInvoice')
BEGIN
    CREATE TABLE [dbo].[ContractInvoice] (
    [ContractPurId] int NOT NULL,
    [ContractID] int NULL,
    [CDate] datetime NULL,
    [PartyID] varchar(10) NOT NULL,
    [BrokerID] varchar(10) NULL,
    [Refree] varchar(30) NULL,
    [IsReached] bit NOT NULL,
    [IsPay] bit NULL,
    [IsSaleInvoice] bit NOT NULL,
    [VehicalCharges] decimal(18, 2) NULL,
    [OtherExpenceMinus] decimal(18, 2) NULL,
    [OtherExpencePlus] decimal(18, 2) NULL,
    [Discount] decimal(18, 2) NULL,
    [Advance] decimal(18, 2) NULL,
    [Received] decimal(18, 2) NULL,
    [CommType] varchar(20) NULL,
    [CommissionRatio] decimal(18, 5) NULL,
    [CommissionValue] decimal(18, 2) NULL,
    [CurDate] datetime NULL,
    [Narration] varchar(500) NULL,
    [PurArhat] decimal(18, 4) NULL,
    [PurMazdori] decimal(18, 4) NULL,
    [PurDalali] decimal(18, 4) NULL,
    [PurPulliGhya] decimal(18, 4) NULL,
    [PurComm] decimal(18, 4) NULL,
    [PurLoading] decimal(18, 4) NULL,
    [PurUnLoading] decimal(18, 4) NULL,
    [SaleComm] decimal(18, 8) NULL,
    [SaleMazdori] decimal(18, 4) NULL,
    [SaleMarketFees] decimal(18, 4) NULL,
    [SaleSootli] decimal(18, 4) NULL,
    [SaleDalali] decimal(18, 4) NULL,
    [SaleManshyana] decimal(18, 4) NULL,
    [SaleLoading] decimal(18, 4) NULL,
    [SaleUnLoading] decimal(18, 4) NULL,
    [PurId] int NULL,
    [SellerId] varchar(10) NULL,
    [AddExpInAmount] bit NULL,
    [LoadingPerBag] bit NULL,
    [WHTax] decimal(18, 4) NULL,
    [GatePassNo] varchar(15) NULL,
    [BillNo] varchar(10) NULL,
    [MazdoriPerBag] bit NULL,
    [NetValue] decimal(18, 2) NULL,
    [AddArhat] bit NULL,
    [AddDalali] bit NULL,
    [AddMazdori] bit NULL,
    [AddPuliGhya] bit NULL,
    [AddCommission] bit NULL,
    [AddManshyana] bit NULL,
    [AddLoading] bit NULL,
    [AddUnLoading] bit NULL,
    [AddMarketFees] bit NULL,
    [AddSootli] bit NULL,
    [AddWHTax] bit NULL,
    [MinusArhat] bit NULL,
    [MinusDalali] bit NULL,
    [MinusMazdori] bit NULL,
    [MinusPuliGhya] bit NULL,
    [MinusCommission] bit NULL,
    [MinusManshyana] bit NULL,
    [MinusLoading] bit NULL,
    [MinusUnLoading] bit NULL,
    [MinusMarketFees] bit NULL,
    [MinusSootli] bit NULL,
    [MinusWHTax] bit NULL,
    [UnLoadingPerBag] bit NULL,
    [IsAdvanceSale] bit NULL,
    [AddVehicleCharges] bit NULL,
    [MinusVehicleCharges] bit NULL,
    [IsApplyPartyMarkup] bit NULL,
    [PurChangrai] decimal(18, 4) NULL,
    [AddChangrai] bit NULL,
    [MinusChangrai] bit NULL,
    [AddTelePhoneExp] decimal(18, 2) NULL,
    [AddInsuranceExp] decimal(18, 2) NULL,
    [AddFuelExp] decimal(18, 2) NULL,
    [AddStitchingExp] decimal(18, 2) NULL,
    [RefCommRatio] decimal(18, 4) NULL,
    [RefCommValue] decimal(18, 5) NULL,
    [AddWHTKg] bit NULL,
    [MinusWHTKg] bit NULL,
    [WHTKg] decimal(18, 5) NULL,
    [RefCommType] varchar(20) NULL,
    [WHTinKg] bit NULL,
    [AddBankCharges] bit NULL,
    [MinusBankCharges] bit NULL,
    [BankCharges] decimal(9, 5) NULL,
    [BankChargesAmount] decimal(18, 2) NULL,
    [BankChargesInKg] bit NULL,
    [MazdoriPercent] bit NULL,
    [MultiEntryID] varchar(20) NULL,
    [isBardana] bit NULL,
    [MazdooriValue] decimal(18, 2) NULL,
    [MunshanaValue] decimal(18, 2) NULL,
    [SotooliValue] decimal(18, 2) NULL,
    [TotalCommision] decimal(18, 2) NULL,
    [SMazdooriValue] decimal(18, 2) NULL,
    [SMunshanaValue] decimal(18, 2) NULL,
    [SSotooliValue] decimal(18, 2) NULL,
    [STotalCommision] decimal(18, 2) NULL,
    [PurMazdoriAmount] decimal(18, 4) NULL,
    [Bardana] decimal(18, 4) NULL,
    [CommissionAmount] decimal(18, 4) NULL,
    [SootaliAmount] decimal(18, 4) NULL,
    [ManshyanaAmount] decimal(18, 4) NULL,
    [PrevBalance] decimal(18, 0) NULL,
    [FinalBalance] decimal(18, 0) NULL,
    [Stitching] decimal(18, 0) NULL,
    [RentGodown] decimal(18, 0) NULL,
    [ChokiDara] decimal(18, 0) NULL,
    [ChartsCharges] decimal(18, 0) NULL,
    [BagUsage] decimal(18, 0) NULL,
    [ShortageG] decimal(18, 0) NULL,
    [TractorExp] decimal(18, 0) NULL,
    [Sotli] decimal(18, 0) NULL,
    [PurchaseID] int NULL,
    [SaleID] int NULL,
    [BagVendorID] varchar(10) NULL,
    [PurMarketFee] decimal(18, 4) NULL,
    [AddPurMarketFee] decimal(18, 4) NULL,
    [MinusPurMarketFee] decimal(18, 4) NULL,
    [PMazdoriWOShort] bit NULL,
    [PCarriageWOShort] bit NULL,
    [PLoadingWOShort] bit NULL,
    [PUnLoadingWOShort] bit NULL,
    [SMazdoriWOShort] bit NULL,
    [SLoadingWOShort] bit NULL,
    [SUnLoadingWOShort] bit NULL,
    [DMCommmission] bit NULL,
    [PurAnjuman] decimal(18, 4) NULL,
    [SaleAnjuman] decimal(18, 4) NULL,
    [IsShortInvoice] bit NOT NULL DEFAULT ((0)),
    [NarrationUrdu] nvarchar(1000) NULL,
    [FPData] image NULL,
    [MunshyanaPerMnd] bit NULL,
    [MunshyanaPer] decimal(18, 2) NULL,
    [KantaExpense] decimal(18, 2) NULL,
    [SepVehicalCharges] bit NULL,
    [IsMultiNew] bit NULL,
    [VehicleChargesAcc] varchar(10) NULL,
    [ChangraiAcc] varchar(10) NULL,
    [DalaliAcc] varchar(10) NULL,
    [MazdooriAcc] varchar(10) NULL,
    [IsMultiNewMulti] bit NULL,
    [WHTDamiRatio] decimal(10, 2) NULL,
    [WHTDamiValue] decimal(10, 2) NULL,
    [PurBagUsage] decimal(10, 4) NULL,
    [PurBagUsagePerBag] bit NULL,
    [PurBagUsagePercent] bit NULL,
    [VehicleChargesRatio] decimal(18, 5) NULL,
    [VehicleChargesPerBag] bit NULL,
    [VehicleChargesPerMnd] bit NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NOT NULL,
    [IDBranchPur] bigint NULL,
    [IDBranchSale] bigint NULL,
    [IDBranchMultiID] varchar(20) NULL,
    [CharityRatio] decimal(10, 4) NULL,
    [CharityValue] decimal(18, 4) NULL,
    [IsCharityPer] bit NULL,
    [IsCharityMnd] bit NULL,
    CONSTRAINT [PK_ContractInvoice] PRIMARY KEY CLUSTERED ([ContractPurId] ASC)
    );
    PRINT 'Created table: [ContractInvoice]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContractInvoiceBody')
BEGIN
    CREATE TABLE [dbo].[ContractInvoiceBody] (
    [ContractPurId] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [TruckQty] int NULL,
    [BagsQty] decimal(18, 2) NULL,
    [WeightPerBag] decimal(18, 2) NULL,
    [RateType] varchar(20) NULL,
    [RatePerMound] decimal(10, 2) NULL,
    [Bardana] decimal(18, 4) NULL,
    [TotalValue] decimal(19, 4) NULL,
    [TotalKgs] decimal(18, 2) NULL,
    [IsSaleInvoice] bit NULL,
    [ExtraWeight] decimal(12, 2) NULL,
    [IsDeleted] bit NULL,
    [VehicleNo] varchar(20) NULL,
    [Shortage] decimal(6, 2) NULL,
    [SerialNo] bigint NULL,
    [MoundStd] decimal(7, 3) NULL,
    [PQty] decimal(18, 2) NULL,
    [SaleID] int NULL,
    [PurchaseID] int NULL,
    [IsWithShortage] bit NULL,
    [Deduction] decimal(6, 2) NULL,
    [PurID] int NULL,
    [ShortageGPer] decimal(18, 2) NULL,
    [ShortageGType] varchar(5) NULL,
    [ContractID] int NULL
    );
    PRINT 'Created table: [ContractInvoiceBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditVouchers')
BEGIN
    CREATE TABLE [dbo].[CreditVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_CreditVouchers] PRIMARY KEY CLUSTERED ([VoucherNo] ASC, [VoucherDate] ASC)
    );
    PRINT 'Created table: [CreditVouchers]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditVouchersBody')
BEGIN
    CREATE TABLE [dbo].[CreditVouchersBody] (
    [SerialNo] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [Narration] varchar(1000) NULL,
    [Credit] decimal(25, 2) NULL,
    CONSTRAINT [PK_CreditVouchersBody] PRIMARY KEY CLUSTERED ([SerialNo] ASC)
    );
    PRINT 'Created table: [CreditVouchersBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsPurchaseBody')
BEGIN
    CREATE TABLE [dbo].[CropsPurchaseBody] (
    [SERIALNO] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [CropPurchaseID] int NOT NULL,
    [WeighmentID] int NOT NULL,
    [Weight] decimal(11, 4) NOT NULL,
    [Price] decimal(11, 5) NULL,
    [ProductID] varchar(5) NULL,
    [Multiplier] decimal(7, 3) NULL,
    CONSTRAINT [PK_CropsPurchaseBody] PRIMARY KEY CLUSTERED ([SERIALNO] ASC)
    );
    PRINT 'Created table: [CropsPurchaseBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsPurchaseContracts')
BEGIN
    CREATE TABLE [dbo].[CropsPurchaseContracts] (
    [PurContractID] int NOT NULL,
    [ContractDate] smalldatetime NOT NULL,
    [ValidUpTo] smalldatetime NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [SupplyMode] varchar(20) NULL,
    [SupplyModeQty] smallint NULL,
    [Weight] decimal(11, 4) NOT NULL,
    [Price] decimal(11, 2) NOT NULL,
    [ExpPlus] decimal(18, 2) NOT NULL,
    [ExpMinus] decimal(18, 2) NOT NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [UserID] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [ProductID] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsPurchaseContracts] PRIMARY KEY CLUSTERED ([PurContractID] ASC)
    );
    PRINT 'Created table: [CropsPurchaseContracts]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsPurchaseHeader')
BEGIN
    CREATE TABLE [dbo].[CropsPurchaseHeader] (
    [CropPurchaseID] int NOT NULL,
    [PurchaseDate] smalldatetime NOT NULL,
    [PurContractID] int NULL,
    [VendorID] varchar(10) NOT NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [ExpPlus] decimal(18, 5) NULL,
    [ExpMinus] decimal(18, 5) NULL,
    [UserID] smallint NOT NULL,
    [Remarks] varchar(500) NULL,
    [IsPosted] bit NOT NULL,
    [IsApplyPartyMarkup] bit NULL,
    [PaidAmount] decimal(30, 10) NULL,
    [VendorIDCom] varchar(10) NULL,
    [VendorComRatio] decimal(4, 2) NULL,
    [ComAmount] decimal(18, 0) NULL,
    [ComPerPerc] bit NULL,
    [ComPerMound] bit NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [MillExp] decimal(18, 2) NULL,
    [OtherExp] decimal(18, 2) NULL,
    [FreightExp] decimal(18, 2) NULL,
    [KantaExp] decimal(18, 2) NULL,
    [BrokerageExp] decimal(18, 2) NULL,
    [MarketFeesExp] decimal(18, 2) NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL,
    [BagQty] decimal(10, 0) NULL,
    [BagRate] decimal(10, 5) NULL,
    [BagAmount] decimal(10, 5) NULL,
    CONSTRAINT [PK_CropsPurchaseHeader] PRIMARY KEY CLUSTERED ([CropPurchaseID] ASC)
    );
    PRINT 'Created table: [CropsPurchaseHeader]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsSaleBody')
BEGIN
    CREATE TABLE [dbo].[CropsSaleBody] (
    [SERIALNO] int IDENTITY(1,1) NOT NULL,
    [CropSaleID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [SupplyMode] varchar(20) NULL,
    [SupplyModeQty] smallint NULL,
    [Deduction] decimal(11, 4) NOT NULL,
    [Weight] decimal(11, 4) NOT NULL,
    [Price] decimal(11, 5) NULL,
    [IsDeleted] bit NOT NULL,
    [IsStockFromWeighment] bit NULL,
    [Narration] varchar(99) NULL,
    [Cost] decimal(18, 6) NOT NULL DEFAULT ((0)),
    [WeighmentID] int NULL,
    [LotNo] varchar(10) NULL,
    CONSTRAINT [PK_CropsSaleBody] PRIMARY KEY CLUSTERED ([SERIALNO] ASC)
    );
    PRINT 'Created table: [CropsSaleBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsSaleContracts')
BEGIN
    CREATE TABLE [dbo].[CropsSaleContracts] (
    [CropSaleContractID] int NOT NULL,
    [ContractDate] smalldatetime NOT NULL,
    [ValidUpTo] smalldatetime NOT NULL,
    [CustomerID] varchar(10) NOT NULL,
    [SupplyMode] varchar(20) NULL,
    [SupplyModeQty] smallint NULL,
    [Price] decimal(11, 2) NOT NULL,
    [ExpPlus] decimal(18, 2) NOT NULL,
    [ExpMinus] decimal(18, 2) NOT NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [UserID] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [ProductId] varchar(5) NULL,
    [Weight] decimal(11, 4) NOT NULL DEFAULT ((0)),
    [ComIsMinus] bit NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsSaleContracts] PRIMARY KEY CLUSTERED ([CropSaleContractID] ASC)
    );
    PRINT 'Created table: [CropsSaleContracts]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsSaleHeader')
BEGIN
    CREATE TABLE [dbo].[CropsSaleHeader] (
    [CropSaleID] int NOT NULL,
    [SaleDate] smalldatetime NOT NULL,
    [CropSaleContractID] int NULL,
    [CustomerID] varchar(10) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [WHTRatio] decimal(4, 2) NOT NULL,
    [ExpPlus] decimal(18, 5) NULL,
    [ExpMinus] decimal(18, 5) NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [UserID] smallint NOT NULL,
    [Remarks] varchar(500) NULL,
    [IsPosted] bit NOT NULL,
    [IsStockFromWeighment] bit NULL,
    [IsApplyPartyMarkup] bit NULL,
    [PaidAmount] decimal(30, 10) NULL,
    [ComAmount] decimal(18, 0) NULL,
    [ComPerPerc] bit NULL,
    [ComPerMound] bit NULL,
    [ComIsMinus] bit NULL,
    [VehicleNo] varchar(20) NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [MillExp] decimal(18, 2) NULL,
    [OtherExp] decimal(18, 2) NULL,
    [FreightExp] decimal(18, 2) NULL,
    [KantaExp] decimal(18, 2) NULL,
    [BrokerageExp] decimal(18, 2) NULL,
    [MarketFeesExp] decimal(18, 2) NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL,
    [BagQty] decimal(10, 0) NULL,
    [BagRate] decimal(10, 5) NULL,
    [BagAmount] decimal(10, 5) NULL,
    CONSTRAINT [PK_CropsSaleHeader] PRIMARY KEY CLUSTERED ([CropSaleID] ASC)
    );
    PRINT 'Created table: [CropsSaleHeader]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsSalesWeighments')
BEGIN
    CREATE TABLE [dbo].[CropsSalesWeighments] (
    [WeighmentID] int NOT NULL,
    [WeighmentDate] smalldatetime NOT NULL,
    [CustomerID] varchar(10) NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [SupplyMode] varchar(20) NULL,
    [SupplyModeQty] smallint NULL,
    [InitialGrossWeight] decimal(11, 4) NOT NULL,
    [TareWeight] decimal(8, 2) NOT NULL,
    [GrossWeight] decimal(11, 4) NOT NULL,
    [Deduction] decimal(6, 2) NOT NULL,
    [Shortage] decimal(7, 2) NOT NULL,
    [UserID] smallint NOT NULL,
    [Remarks] varchar(50) NULL,
    [IsFinalized] bit NOT NULL,
    [IsPosted] bit NOT NULL,
    [VehicleNo] varchar(20) NULL,
    [IniGrossWeightDate] datetime NULL,
    [TareWeightDate] datetime NULL,
    [isAuto] bit NULL,
    [MainExeWeighment] bit NULL,
    [IsAutoGross] varchar(10) NULL,
    [IsAutoTare] varchar(10) NULL,
    [LotNo] varchar(10) NULL,
    [GatePassNo] int NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsSalesWeighments] PRIMARY KEY CLUSTERED ([WeighmentID] ASC)
    );
    PRINT 'Created table: [CropsSalesWeighments]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsWeighments')
BEGIN
    CREATE TABLE [dbo].[CropsWeighments] (
    [WeighmentID] int NOT NULL,
    [WeighmentDate] smalldatetime NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [SupplyMode] varchar(20) NULL,
    [SupplyModeQty] smallint NULL,
    [InitialGrossWeight] decimal(11, 4) NOT NULL,
    [TareWeight] decimal(8, 2) NOT NULL,
    [GrossWeight] decimal(11, 4) NOT NULL,
    [Deduction] decimal(6, 2) NOT NULL,
    [Shortage] decimal(7, 2) NOT NULL,
    [UserID] smallint NOT NULL,
    [Remarks] varchar(50) NULL,
    [IsFinalized] bit NOT NULL,
    [IsPosted] bit NOT NULL,
    [VehicleNo] varchar(20) NULL,
    [IniGrossWeightDate] datetime NULL,
    [TareWeightDate] datetime NULL,
    [isAuto] bit NULL,
    [MainExeWeighment] bit NULL,
    [IsAutoGross] varchar(10) NULL,
    [IsAutoTare] varchar(10) NULL,
    [GatePassNo] int NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsWeighments] PRIMARY KEY CLUSTERED ([WeighmentID] ASC)
    );
    PRINT 'Created table: [CropsWeighments]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CropsWeighmentsSupplyModes')
BEGIN
    CREATE TABLE [dbo].[CropsWeighmentsSupplyModes] (
    [WeighmentID] int NOT NULL,
    [SupplyMode] varchar(20) NOT NULL,
    [SupplyModeQty] int NOT NULL,
    [Deduction] decimal(10, 2) NOT NULL,
    [TTLDeduction] decimal(10, 2) NOT NULL,
    [IsPurWeighment] bit NOT NULL,
    CONSTRAINT [PK_CropsWeighmentsSupplyModes] PRIMARY KEY CLUSTERED ([WeighmentID] ASC, [SupplyMode] ASC, [IsPurWeighment] ASC)
    );
    PRINT 'Created table: [CropsWeighmentsSupplyModes]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CurrentStock')
BEGIN
    CREATE TABLE [dbo].[CurrentStock] (
    [ProductID] varchar(5) NOT NULL,
    [Qty] decimal(11, 2) NOT NULL,
    [Cost] decimal(18, 6) NOT NULL,
    CONSTRAINT [PK_CurrentStock] PRIMARY KEY CLUSTERED ([ProductID] ASC)
    );
    PRINT 'Created table: [CurrentStock]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CurrentStockPestro')
BEGIN
    CREATE TABLE [dbo].[CurrentStockPestro] (
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL
    );
    PRINT 'Created table: [CurrentStockPestro]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DebitVouchers')
BEGIN
    CREATE TABLE [dbo].[DebitVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_DebitVouchers] PRIMARY KEY CLUSTERED ([VoucherNo] ASC, [VoucherDate] ASC)
    );
    PRINT 'Created table: [DebitVouchers]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DebitVouchersBody')
BEGIN
    CREATE TABLE [dbo].[DebitVouchersBody] (
    [SerialNo] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [Narration] varchar(1000) NULL,
    [Debit] decimal(25, 2) NULL,
    CONSTRAINT [PK_DebitVouchersBody] PRIMARY KEY CLUSTERED ([SerialNo] ASC)
    );
    PRINT 'Created table: [DebitVouchersBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExpenseType')
BEGIN
    CREATE TABLE [dbo].[ExpenseType] (
    [ExpanseType] varchar(50) NULL,
    [DamiType] varchar(15) NULL,
    [DamiAmount] decimal(6, 5) NULL,
    [Dami] varchar(1) NULL,
    [WHTaxAmount] decimal(6, 3) NULL,
    [WHTax] varchar(1) NULL,
    [ArhatType] varchar(15) NULL,
    [ArhatAmount] decimal(6, 5) NULL,
    [Arhat] varchar(1) NULL,
    [ChangraiAmount] decimal(6, 3) NULL,
    [Changrai] varchar(1) NULL,
    [DalaliAmount] decimal(6, 3) NULL,
    [Dalali] varchar(1) NULL,
    [MarketAmount] decimal(6, 3) NULL,
    [Market] varchar(1) NULL,
    [MazdooriType] varchar(15) NULL,
    [MazdooriAmount] decimal(6, 3) NULL,
    [Mazdoori] varchar(1) NULL,
    [ChangraiMAmount] decimal(6, 3) NULL,
    [ChangraiM] varchar(1) NULL,
    [ComisionAmount] decimal(6, 3) NULL,
    [Comision] varchar(1) NULL,
    [MunshayanaAmount] decimal(6, 3) NULL,
    [Munshayana] varchar(1) NULL,
    [LoadingType] varchar(15) NULL,
    [LoadingAmount] decimal(6, 3) NULL,
    [Loading] varchar(1) NULL,
    [UnLoadingType] varchar(15) NULL,
    [UnLoadingAmount] decimal(6, 3) NULL,
    [UnLoading] varchar(1) NULL,
    [BankAmount] decimal(6, 3) NULL,
    [Bank] varchar(1) NULL,
    [DamiTypeS] varchar(15) NULL,
    [DamiAmountS] decimal(6, 5) NULL,
    [DamiS] varchar(1) NULL,
    [WHTaxAmountS] decimal(6, 3) NULL,
    [WHTaxS] varchar(1) NULL,
    [ArhatTypeS] varchar(15) NULL,
    [ArhatAmountS] decimal(6, 5) NULL,
    [ArhatS] varchar(1) NULL,
    [ChangraiAmountS] decimal(6, 3) NULL,
    [ChangraiS] varchar(1) NULL,
    [DalaliAmountS] decimal(6, 3) NULL,
    [DalaliS] varchar(1) NULL,
    [MarketAmountS] decimal(6, 3) NULL,
    [MarketS] varchar(1) NULL,
    [MazdooriTypeS] varchar(15) NULL,
    [MazdooriAmountS] decimal(6, 3) NULL,
    [MazdooriS] varchar(1) NULL,
    [ChangraiMAmountS] decimal(18, 2) NULL,
    [ChangraiMS] varchar(1) NULL,
    [ComisionAmountS] decimal(6, 3) NULL,
    [ComisionS] varchar(1) NULL,
    [MunshayanaAmountS] decimal(6, 3) NULL,
    [MunshayanaS] varchar(1) NULL,
    [LoadingTypeS] varchar(15) NULL,
    [LoadingAmountS] decimal(6, 3) NULL,
    [LoadingS] varchar(1) NULL,
    [UnLoadingTypeS] varchar(15) NULL,
    [UnLoadingAmountS] decimal(6, 3) NULL,
    [UnLoadingS] varchar(1) NULL,
    [BankAmountS] decimal(6, 3) NULL,
    [BankS] varchar(1) NULL,
    [SootaliAmountS] decimal(6, 3) NULL,
    [SootaliS] varchar(1) NULL,
    [PMazdoriWOShort] bit NULL,
    [PCarriageWOShort] bit NULL,
    [PLoadingWOShort] bit NULL,
    [PUnLoadingWOShort] bit NULL,
    [DMCommmission] bit NULL,
    [BankChargesInKg] bit NULL,
    [RefCommType] varchar(20) NULL,
    [RefCommRatio] decimal(14, 4) NULL,
    [OtherExpenceMinus] decimal(14, 2) NULL,
    [OtherExpencePlus] decimal(14, 2) NULL,
    [Stitching] decimal(18, 4) NULL,
    [RentGodown] decimal(18, 4) NULL,
    [ChokiDara] decimal(18, 4) NULL,
    [ChartsCharges] decimal(18, 4) NULL,
    [BagUsage] decimal(18, 4) NULL,
    [ShortageG] decimal(18, 4) NULL,
    [TractorExp] decimal(18, 4) NULL,
    [Sotli] decimal(18, 4) NULL,
    [AddTelePhoneExp] decimal(18, 4) NULL,
    [AddStitching] bit NULL,
    [AddRentGodown] bit NULL,
    [AddChokiDara] bit NULL,
    [AddChartsCharges] bit NULL,
    [AddBagUsage] bit NULL,
    [AddShortageG] bit NULL,
    [AddTractorExp] bit NULL,
    [AddSotli] bit NULL,
    [PlusTelePhoneExp] bit NULL,
    [SMazdoriWOShort] bit NULL,
    [SCarriageWOShort] bit NULL,
    [SLoadingWOShort] bit NULL,
    [SUnLoadingWOShort] bit NULL,
    [DMCommmissionS] bit NULL,
    [SBankChargesInKg] bit NULL,
    [RefCommTypeS] varchar(20) NULL,
    [RefCommRatioS] decimal(14, 4) NULL,
    [OtherExpenceMinusS] decimal(14, 2) NULL,
    [OtherExpencePlusS] decimal(14, 2) NULL,
    [StitchingS] decimal(18, 4) NULL,
    [RentGodownS] decimal(18, 4) NULL,
    [ChokiDaraS] decimal(18, 4) NULL,
    [ChartsChargesS] decimal(18, 4) NULL,
    [BagUsageS] decimal(18, 4) NULL,
    [ShortageGS] decimal(18, 4) NULL,
    [TractorExpS] decimal(18, 4) NULL,
    [AddTelePhoneExpS] decimal(18, 4) NULL,
    [AddStitchingS] bit NULL,
    [AddRentGodownS] bit NULL,
    [AddChokiDaraS] bit NULL,
    [AddChartsChargesS] bit NULL,
    [AddBagUsageS] bit NULL,
    [AddShortageGS] bit NULL,
    [AddTractorExpS] bit NULL,
    [AddSotliS] bit NULL,
    [PlusTelePhoneExpS] bit NULL,
    [KantaExpenseP] decimal(18, 2) NULL,
    [KantaExpenseS] decimal(18, 2) NULL,
    [BagUsageSPer] varchar(1) NULL,
    [MunshianaPer] varchar(1) NULL,
    [VehicleChargesPer] varchar(1) NULL,
    [IsPurchase] bit NULL
    );
    PRINT 'Created table: [ExpenseType]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'History')
BEGIN
    CREATE TABLE [dbo].[History] (
    [HistoryId] int IDENTITY(1,1) NOT NULL,
    [TimeKey] smalldatetime NULL,
    [ProcessName] varchar(20) NOT NULL,
    [ProcessId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNoOld] varchar(20) NULL,
    [QuantityOld] int NULL DEFAULT ((0)),
    [CostOld] decimal(12, 5) NULL,
    [BatchNoNew] varchar(20) NULL,
    [QuantityNew] int NULL DEFAULT ((0)),
    [CostNew] decimal(12, 5) NULL
    );
    PRINT 'Created table: [History]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Insurance')
BEGIN
    CREATE TABLE [dbo].[Insurance] (
    [SrNo] int IDENTITY(1,1) NOT NULL,
    [InsuranceCompanyName] varchar(50) NOT NULL,
    [CoverNote] varchar(50) NOT NULL,
    [InsuredValue] int NOT NULL,
    [NetPremium] int NOT NULL,
    [FromDate] smalldatetime NOT NULL,
    [ToDate] smalldatetime NOT NULL,
    [PolicyNo] varchar(30) NULL,
    [Category] varchar(30) NULL,
    CONSTRAINT [PK_Insurance] PRIMARY KEY CLUSTERED ([SrNo] ASC)
    );
    PRINT 'Created table: [Insurance]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'JournalVouchers')
BEGIN
    CREATE TABLE [dbo].[JournalVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_JournalVouchers] PRIMARY KEY CLUSTERED ([VoucherNo] ASC, [VoucherDate] ASC)
    );
    PRINT 'Created table: [JournalVouchers]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'JournalVouchersBody')
BEGIN
    CREATE TABLE [dbo].[JournalVouchersBody] (
    [SerialNo] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [Narration] varchar(1000) NULL,
    [Debit] decimal(25, 2) NULL,
    [Credit] decimal(25, 2) NULL,
    CONSTRAINT [PK_JournalVouchersBody] PRIMARY KEY CLUSTERED ([SerialNo] ASC)
    );
    PRINT 'Created table: [JournalVouchersBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MultiBody')
BEGIN
    CREATE TABLE [dbo].[MultiBody] (
    [MultiID] bigint NOT NULL,
    [PurchaseID] int NOT NULL,
    [SaleID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [MDate] smalldatetime NOT NULL,
    [IndentNo] nvarchar(20) NOT NULL,
    [TokenNo] nvarchar(20) NOT NULL,
    [VehicleNo] nvarchar(20) NOT NULL,
    [VehicleAccNo] varchar(10) NULL,
    [GWeight] decimal(10, 3) NOT NULL,
    [TWeight] decimal(10, 3) NOT NULL,
    [NWeight] decimal(10, 3) NOT NULL,
    [MRate] decimal(10, 2) NOT NULL,
    [GRate] decimal(10, 2) NOT NULL,
    [AmountP] decimal(20, 2) NOT NULL,
    [AmountS] decimal(20, 2) NOT NULL,
    [TPTM] decimal(10, 2) NOT NULL,
    [TPTEx] decimal(10, 2) NOT NULL,
    [IsVL] bit NOT NULL,
    [NetAmountP] decimal(20, 2) NOT NULL,
    [NetAmountS] decimal(20, 2) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [IsVLG] bit NULL,
    [SrNo] int NULL,
    CONSTRAINT [PK_MultiBody] PRIMARY KEY CLUSTERED ([MultiID] ASC, [ProductID] ASC, [IndentNo] ASC)
    );
    PRINT 'Created table: [MultiBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MultiInvoice')
BEGIN
    CREATE TABLE [dbo].[MultiInvoice] (
    [MultiID] bigint NOT NULL,
    [MultiDate] smalldatetime NOT NULL,
    [PurchaseID] int NOT NULL,
    [SaleID] int NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [CustomerID] varchar(10) NOT NULL,
    [GrossAmountP] decimal(20, 2) NOT NULL,
    [GrossAmountS] decimal(20, 2) NOT NULL,
    [NetAmountP] decimal(20, 2) NOT NULL,
    [NetAmountS] decimal(20, 2) NOT NULL,
    [IsMultiNew] bit NULL,
    [PLSAccount] varchar(10) NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL DEFAULT ((1)),
    [IDBranch] bigint NULL
    );
    PRINT 'Created table: [MultiInvoice]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OpeningBatchPestro')
BEGIN
    CREATE TABLE [dbo].[OpeningBatchPestro] (
    [SerialID] int IDENTITY(1,1) NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [Quantity] decimal(10, 3) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [IsDeleted] bit NOT NULL
    );
    PRINT 'Created table: [OpeningBatchPestro]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OpeningStock')
BEGIN
    CREATE TABLE [dbo].[OpeningStock] (
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [Qty] decimal(18, 5) NOT NULL,
    [Bonus] decimal(7, 2) NOT NULL,
    [Price] decimal(18, 5) NOT NULL,
    [DiscRatio] decimal(4, 2) NOT NULL,
    [DiscValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalesTax] decimal(9, 2) NOT NULL,
    [Cost] decimal(18, 6) NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OpeningStock] PRIMARY KEY CLUSTERED ([ProductID] ASC)
    );
    PRINT 'Created table: [OpeningStock]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OpeningStockPestro')
BEGIN
    CREATE TABLE [dbo].[OpeningStockPestro] (
    [ProductId] varchar(5) NOT NULL,
    [Quantity] decimal(10, 3) NOT NULL,
    [Bonus] decimal(10, 3) NOT NULL,
    [Price] decimal(18, 2) NULL,
    [DiscRatio1] decimal(4, 2) NOT NULL,
    [DiscRatio2] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalestax] decimal(11, 2) NOT NULL,
    [IsDeleted] bit NOT NULL
    );
    PRINT 'Created table: [OpeningStockPestro]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PortSettings')
BEGIN
    CREATE TABLE [dbo].[PortSettings] (
    [RegistryKey] varchar(50) NOT NULL,
    [RegistryValue] varchar(20) NOT NULL,
    [Description] varchar(50) NULL
    );
    PRINT 'Created table: [PortSettings]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Project_Registry')
BEGIN
    CREATE TABLE [dbo].[Project_Registry] (
    [RegistryKey] varchar(40) NOT NULL,
    [ParentRegistryKey] varchar(40) NULL,
    [Narration] nvarchar(3000) NULL,
    [Value] nvarchar(3000) NULL,
    [IsEditable] bit NOT NULL DEFAULT ((0)),
    CONSTRAINT [PK_Project_Registry] PRIMARY KEY CLUSTERED ([RegistryKey] ASC)
    );
    PRINT 'Created table: [Project_Registry]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Project_Tasks')
BEGIN
    CREATE TABLE [dbo].[Project_Tasks] (
    [TaskKey] varchar(50) NOT NULL,
    [TaskName] varchar(50) NOT NULL,
    [TaskGroup] varchar(60) NOT NULL DEFAULT (''),
    [IsAutoPost] bit NOT NULL,
    [IsPosting] bit NOT NULL,
    [Edition] varchar(3) NULL,
    CONSTRAINT [PK_Project_Tasks] PRIMARY KEY CLUSTERED ([TaskKey] ASC)
    );
    PRINT 'Created table: [Project_Tasks]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Purchase')
BEGIN
    CREATE TABLE [dbo].[Purchase] (
    [PurchaseID] int NOT NULL,
    [PurchaseDate] smalldatetime NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [BillNo] varchar(10) NULL,
    [BillDate] smalldatetime NULL,
    [ExpPlus] decimal(9, 0) NULL,
    [ExpMinus] decimal(9, 0) NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [PaidAmount] decimal(9, 2) NOT NULL,
    [Narration] varchar(100) NULL,
    [IsPosted] bit NOT NULL,
    [SpecialDiscount] numeric(5, 0) NULL,
    [IsApplyPartyMarkup] bit NULL,
    [SaleID] int NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [BankTax] decimal(10, 2) NULL,
    [IsMulti] bit NULL,
    [Freight] decimal(10, 2) NULL,
    [IsFreightPlus] bit NULL,
    [VehicleNo] nvarchar(50) NULL,
    [CropSeason] varchar(5) NULL,
    CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED ([PurchaseID] ASC)
    );
    PRINT 'Created table: [Purchase]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseBody')
BEGIN
    CREATE TABLE [dbo].[PurchaseBody] (
    [PurchaseID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [Qty] decimal(7, 2) NOT NULL,
    [Bonus] decimal(7, 2) NOT NULL,
    [Price] decimal(15, 5) NULL,
    [DiscRatio] decimal(4, 2) NOT NULL,
    [DiscValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalesTax] decimal(11, 2) NOT NULL,
    [Cost] decimal(18, 6) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [PaidAmount] decimal(18, 2) NULL,
    CONSTRAINT [PK_PurchaseBody] PRIMARY KEY CLUSTERED ([PurchaseID] ASC, [ProductID] ASC)
    );
    PRINT 'Created table: [PurchaseBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchasesBatch')
BEGIN
    CREATE TABLE [dbo].[PurchasesBatch] (
    [PurchaseId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [PurchasesBatch]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchasesBodyPest')
BEGIN
    CREATE TABLE [dbo].[PurchasesBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [PurchaseId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL DEFAULT ((0)),
    [Bonus] smallint NOT NULL DEFAULT ((0)),
    [Price] decimal(20, 4) NULL DEFAULT ((0)),
    [DiscRatio1] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [DiscRatio2] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [STValue] decimal(7, 2) NOT NULL DEFAULT ((0)),
    [STRatio] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [TTLValue] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [TTLSalestax] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [PurchasesBodyPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchasesPest')
BEGIN
    CREATE TABLE [dbo].[PurchasesPest] (
    [PurchaseId] int NOT NULL,
    [LastUpdateDate] smalldatetime NOT NULL DEFAULT (getdate()),
    [EntryDate] smalldatetime NOT NULL,
    [BillNo] varchar(10) NULL DEFAULT ('0'),
    [BillDate] smalldatetime NULL,
    [OrderID] smallint NULL,
    [PreviousCredit] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [Expenses] decimal(11, 2) NOT NULL,
    [PaidAmount] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [VendorId] varchar(10) NOT NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL DEFAULT ((0)),
    [IsLocal] bit NOT NULL DEFAULT ((0)),
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL,
    [Narration] nvarchar(1000) NULL
    );
    PRINT 'Created table: [PurchasesPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurPendings')
BEGIN
    CREATE TABLE [dbo].[PurPendings] (
    [PurchaseId] smallint NOT NULL,
    [PurchaseDate] smalldatetime NOT NULL,
    [BillNo] varchar(10) NULL,
    [BillDate] smalldatetime NULL,
    [VendorId] varchar(10) NOT NULL,
    [OrderID] smallint NULL,
    [TimeKey] timestamp NULL,
    [IsPending] tinyint NOT NULL,
    [PreviousCredit] decimal(11, 2) NOT NULL,
    [CropSeason] varchar(5) NULL
    );
    PRINT 'Created table: [PurPendings]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurPendingsBatch')
BEGIN
    CREATE TABLE [dbo].[PurPendingsBatch] (
    [PurchaseId] smallint NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(10) NOT NULL DEFAULT (''),
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL
    );
    PRINT 'Created table: [PurPendingsBatch]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurPendingsBody')
BEGIN
    CREATE TABLE [dbo].[PurPendingsBody] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [PurchaseId] smallint NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL DEFAULT ((0)),
    [Bonus] smallint NOT NULL DEFAULT ((0)),
    [Price] decimal(20, 4) NULL DEFAULT ((0)),
    [DiscRatio1] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [DiscRatio2] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [STValue] decimal(7, 2) NOT NULL DEFAULT ((0)),
    [STRatio] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [TTLValue] decimal(9, 2) NOT NULL DEFAULT ((0)),
    [TTLSalestax] decimal(9, 2) NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [PurPendingsBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurReturns')
BEGIN
    CREATE TABLE [dbo].[PurReturns] (
    [PurReturnID] int NOT NULL,
    [PurchaseID] int NULL,
    [PurReturnDate] smalldatetime NOT NULL,
    [VendorID] varchar(10) NOT NULL,
    [ExpPlus] decimal(6, 2) NOT NULL,
    [ExpMinus] decimal(6, 2) NOT NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [ReceivedAmount] decimal(9, 2) NOT NULL,
    [Narration] varchar(100) NULL,
    [IsPosted] bit NOT NULL,
    [IsApplyPartyMarkup] bit NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL,
    CONSTRAINT [PK_PurReturns] PRIMARY KEY CLUSTERED ([PurReturnID] ASC)
    );
    PRINT 'Created table: [PurReturns]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurReturnsBatch')
BEGIN
    CREATE TABLE [dbo].[PurReturnsBatch] (
    [PReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [Quantity] int NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Cost] decimal(12, 5) NULL,
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [PurReturnsBatch]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurReturnsBody')
BEGIN
    CREATE TABLE [dbo].[PurReturnsBody] (
    [PurReturnID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [Qty] decimal(7, 2) NOT NULL,
    [Bonus] decimal(7, 2) NOT NULL,
    [Price] decimal(11, 2) NOT NULL,
    [DiscRatio] decimal(4, 2) NOT NULL,
    [DiscValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalesTax] decimal(11, 2) NOT NULL,
    [Cost] decimal(18, 6) NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_PurReturnsBody] PRIMARY KEY CLUSTERED ([PurReturnID] ASC, [ProductID] ASC)
    );
    PRINT 'Created table: [PurReturnsBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurReturnsBodyPest')
BEGIN
    CREATE TABLE [dbo].[PurReturnsBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [PReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL DEFAULT ((0)),
    [Bonus] smallint NOT NULL DEFAULT ((0)),
    [Price] decimal(20, 4) NULL DEFAULT ((0)),
    [DiscRatio1] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [DiscRatio2] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [STValue] decimal(7, 2) NOT NULL DEFAULT ((0)),
    [STRatio] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [TTLValue] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [TTLSalestax] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [PurReturnsBodyPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurReturnsPest')
BEGIN
    CREATE TABLE [dbo].[PurReturnsPest] (
    [PReturnId] int NOT NULL,
    [PurchaseId] int NULL,
    [LastUpdateDate] smalldatetime NOT NULL DEFAULT (getdate()),
    [ReturnDate] smalldatetime NOT NULL,
    [PreviousCredit] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [ReceivedAmount] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [VendorId] varchar(10) NOT NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL DEFAULT ((0)),
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL
    );
    PRINT 'Created table: [PurReturnsPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalePendings')
BEGIN
    CREATE TABLE [dbo].[SalePendings] (
    [SaleId] int NOT NULL,
    [IsPending] tinyint NOT NULL,
    [SaleDate] smalldatetime NOT NULL,
    [IsCashInvoice] bit NOT NULL DEFAULT ((0)),
    [IsWarranted] bit NOT NULL DEFAULT ((0)),
    [CustomerId] varchar(10) NOT NULL,
    [TimeKey] timestamp NULL,
    [SalesManNo] varchar(10) NULL,
    [IsShortStockInvoice] bit NOT NULL DEFAULT ((0)),
    [IsAutoBatch] bit NOT NULL DEFAULT ((0)),
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL
    );
    PRINT 'Created table: [SalePendings]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalePendingsBatch')
BEGIN
    CREATE TABLE [dbo].[SalePendingsBatch] (
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(10) NOT NULL,
    [Quantity] int NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL
    );
    PRINT 'Created table: [SalePendingsBatch]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalePendingsBody')
BEGIN
    CREATE TABLE [dbo].[SalePendingsBody] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL DEFAULT ((0)),
    [Bonus] smallint NOT NULL DEFAULT ((0)),
    [Price] decimal(20, 4) NULL DEFAULT ((0)),
    [DiscRatio1] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [DiscRatio2] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [STValue] decimal(7, 2) NOT NULL DEFAULT ((0)),
    [STRatio] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [IsSaleOnTp] bit NOT NULL DEFAULT ((0)),
    [TTLValue] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [TTLSalestax] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [SalePendingsBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaleReturns')
BEGIN
    CREATE TABLE [dbo].[SaleReturns] (
    [SaleReturnID] int NOT NULL,
    [SaleReturnDate] smalldatetime NOT NULL,
    [SaleID] int NULL,
    [CustomerID] varchar(10) NOT NULL,
    [SectorID] smallint NOT NULL,
    [ExpPlus] decimal(10, 2) NULL,
    [ExpMinus] decimal(10, 2) NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [PaidAmount] decimal(9, 2) NOT NULL,
    [SpecialDiscount] decimal(9, 2) NOT NULL,
    [Narration] varchar(500) NULL,
    [IsPosted] bit NOT NULL,
    [IsApplyPartyMarkup] bit NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL,
    CONSTRAINT [PK_SaleReturns] PRIMARY KEY CLUSTERED ([SaleReturnID] ASC)
    );
    PRINT 'Created table: [SaleReturns]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaleReturnsBatch')
BEGIN
    CREATE TABLE [dbo].[SaleReturnsBatch] (
    [SReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [SaleReturnsBatch]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaleReturnsBody')
BEGIN
    CREATE TABLE [dbo].[SaleReturnsBody] (
    [SaleReturnID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [Qty] decimal(7, 2) NOT NULL,
    [Bonus] decimal(7, 2) NOT NULL,
    [Price] decimal(11, 2) NOT NULL,
    [DiscRatio] decimal(4, 2) NOT NULL,
    [DiscValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalesTax] decimal(11, 2) NOT NULL,
    [Cost] decimal(18, 6) NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SaleReturnsBody] PRIMARY KEY CLUSTERED ([SaleReturnID] ASC, [ProductID] ASC)
    );
    PRINT 'Created table: [SaleReturnsBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaleReturnsBodyPest')
BEGIN
    CREATE TABLE [dbo].[SaleReturnsBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [SReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL DEFAULT ((0)),
    [Bonus] smallint NOT NULL DEFAULT ((0)),
    [Price] decimal(20, 4) NULL DEFAULT ((0)),
    [DiscRatio1] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [DiscRatio2] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [STValue] decimal(7, 2) NOT NULL DEFAULT ((0)),
    [STRatio] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [TTLValue] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [TTLSalestax] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [SaleReturnsBodyPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SaleReturnsPest')
BEGIN
    CREATE TABLE [dbo].[SaleReturnsPest] (
    [SReturnId] int NOT NULL,
    [SaleId] int NULL,
    [LastUpdateDate] smalldatetime NOT NULL,
    [ReturnDate] smalldatetime NOT NULL,
    [PreviousDebit] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [PaidAmount] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [SpecialDiscount] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [CustomerId] varchar(10) NOT NULL,
    [IsUndelivered] bit NOT NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL DEFAULT ((0)),
    [SalesManId] varchar(10) NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL
    );
    PRINT 'Created table: [SaleReturnsPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sales')
BEGIN
    CREATE TABLE [dbo].[Sales] (
    [SaleID] int NOT NULL,
    [SaleDate] smalldatetime NOT NULL,
    [CustomerID] varchar(10) NOT NULL,
    [SectorID] smallint NOT NULL,
    [ExpPlus] decimal(10, 2) NULL,
    [ExpMinus] decimal(10, 2) NULL,
    [ComRatio] decimal(4, 2) NOT NULL,
    [ReceivedAmount] decimal(9, 2) NOT NULL,
    [SpecialDiscount] decimal(9, 2) NOT NULL,
    [Narration] varchar(500) NULL,
    [IsPosted] bit NOT NULL,
    [IsApplyPartyMarkup] bit NULL,
    [CustomerIDCom] varchar(10) NULL,
    [CustomerComRatio] decimal(4, 2) NULL,
    [PurchaseID] int NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [FPData] image NULL,
    [IsMulti] bit NULL,
    [Freight] decimal(10, 2) NULL,
    [IsFreightPlus] bit NULL,
    [VehicleNo] nvarchar(50) NULL,
    [VoucherNoDr] numeric(10, 0) NULL,
    [VoucherNoCr] numeric(10, 0) NULL,
    [Amount] decimal(11, 2) NULL,
    [WHTRatio] decimal(11, 2) NULL,
    [WHTAmount] decimal(11, 2) NULL,
    [WHTPlus] bit NULL,
    [CropSeason] varchar(5) NULL,
    CONSTRAINT [PK_Sales] PRIMARY KEY CLUSTERED ([SaleID] ASC)
    );
    PRINT 'Created table: [Sales]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesBatch')
BEGIN
    CREATE TABLE [dbo].[SalesBatch] (
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] decimal(12, 5) NULL,
    [Cost] decimal(12, 5) NULL,
    [IsDeleted] bit NOT NULL DEFAULT ((0))
    );
    PRINT 'Created table: [SalesBatch]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesBody')
BEGIN
    CREATE TABLE [dbo].[SalesBody] (
    [SaleID] int NOT NULL,
    [ProductID] varchar(5) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    [Qty] decimal(7, 2) NOT NULL,
    [Bonus] decimal(7, 2) NOT NULL,
    [Price] decimal(15, 5) NULL,
    [DiscRatio] decimal(4, 2) NOT NULL,
    [DiscValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalesTax] decimal(11, 2) NOT NULL,
    [Cost] decimal(18, 6) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [Narration] varchar(99) NULL,
    [SrNo] int NOT NULL DEFAULT ((0)),
    [PurchaseID] int NULL,
    CONSTRAINT [PK_SalesBody] PRIMARY KEY CLUSTERED ([SaleID] ASC, [ProductID] ASC, [SrNo] ASC)
    );
    PRINT 'Created table: [SalesBody]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesBodyPest')
BEGIN
    CREATE TABLE [dbo].[SalesBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [IsSaleOnTp] bit NOT NULL DEFAULT ((0)),
    [Quantity] decimal(12, 5) NULL,
    [Bonus] smallint NOT NULL DEFAULT ((0)),
    [Price] decimal(20, 4) NULL DEFAULT ((0)),
    [DiscRatio1] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [DiscRatio2] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [STValue] decimal(7, 2) NOT NULL DEFAULT ((0)),
    [STRatio] decimal(4, 2) NOT NULL DEFAULT ((0)),
    [TTLValue] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [TTLSalestax] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [IsDeleted] bit NOT NULL DEFAULT ((0)),
    [IsAdvance] bit NULL DEFAULT ((0))
    );
    PRINT 'Created table: [SalesBodyPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesPest')
BEGIN
    CREATE TABLE [dbo].[SalesPest] (
    [SaleId] int NOT NULL,
    [SaleType] varchar(1) NOT NULL,
    [ExternalId] int NOT NULL,
    [SalesManId] varchar(10) NULL,
    [CustomerId] varchar(10) NOT NULL,
    [LastUpdateDate] smalldatetime NOT NULL DEFAULT (getdate()),
    [SaleDate] smalldatetime NOT NULL,
    [IsCashInvoice] bit NOT NULL DEFAULT ((0)),
    [IsWarranted] bit NOT NULL DEFAULT ((0)),
    [PreviousDebit] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [IsAutoBatch] bit NOT NULL DEFAULT ((0)),
    [ReceivedAmount] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [SpecialDiscount] decimal(11, 2) NOT NULL DEFAULT ((0)),
    [DueDate] smalldatetime NOT NULL,
    [IsPrinted] bit NOT NULL DEFAULT ((0)),
    [Remarks] varchar(96) NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL DEFAULT ((0)),
    [SSInvNo] int NULL,
    [IsInvoiceClaimable] bit NULL,
    [CustomerName] varchar(100) NULL,
    [PhaseNo] varchar(10) NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [FPData] image NULL,
    [VoucherNoDr] numeric(10, 0) NULL,
    [VoucherNoCr] numeric(10, 0) NULL,
    [Amount] decimal(11, 2) NULL,
    [WHTRatio] decimal(11, 2) NULL,
    [WHTAmount] decimal(11, 2) NULL,
    [WHTPlus] bit NULL,
    [CropSeason] varchar(5) NULL
    );
    PRINT 'Created table: [SalesPest]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TasksRepeated')
BEGIN
    CREATE TABLE [dbo].[TasksRepeated] (
    [RepeatedTaskId] int NOT NULL,
    [UserNo] smallint NOT NULL,
    [Subject] varchar(30) NULL,
    [Description] varchar(200) NULL,
    [Priority] varchar(6) NULL DEFAULT ('Normal'),
    [RemindTime] smalldatetime NULL,
    [RepeatPattern] varchar(7) NULL,
    [DailyGap] tinyint NULL,
    [WeekGap] tinyint NULL,
    [Monday] bit NULL,
    [Tuesday] bit NULL,
    [Wednesday] bit NULL,
    [Thursday] bit NULL,
    [Friday] bit NULL,
    [Saturday] bit NULL,
    [Sunday] bit NULL,
    [MonthGap] tinyint NULL,
    [MonthDay] tinyint NULL,
    [YearDate] smalldatetime NULL,
    [StartingDate] smalldatetime NULL,
    [EndingDate] smalldatetime NULL,
    CONSTRAINT [PK_TasksRepeated] PRIMARY KEY CLUSTERED ([RepeatedTaskId] ASC)
    );
    PRINT 'Created table: [TasksRepeated]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TasksToDo')
BEGIN
    CREATE TABLE [dbo].[TasksToDo] (
    [SubjectId] int NOT NULL,
    [UserNo] smallint NOT NULL,
    [Subject] varchar(30) NOT NULL,
    [Description] varchar(200) NULL,
    [DueDateTime] smalldatetime NULL,
    [Priority] varchar(6) NULL,
    [Status] varchar(25) NULL,
    [RemindDateTime] smalldatetime NULL,
    [Sound] bit NULL DEFAULT ((1)),
    CONSTRAINT [PK_TasksToDo] PRIMARY KEY CLUSTERED ([SubjectId] ASC)
    );
    PRINT 'Created table: [TasksToDo]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_VuAccVouchersAll')
BEGIN
    CREATE TABLE [dbo].[tbl_VuAccVouchersAll] (
    [VoucherType] varchar(4) NULL,
    [VoucherNo] int NULL,
    [VoucherDate] smalldatetime NULL,
    [ChequeNo] varchar(50) NULL,
    [ChequeDate] smalldatetime NULL,
    [DepositedDate] smalldatetime NULL,
    [ReconcileDate] smalldatetime NULL,
    [AccountNo] varchar(10) NULL,
    [Narration] nvarchar(4000) NULL,
    [Debit] decimal(18, 2) NULL,
    [Credit] decimal(18, 2) NULL,
    [IsApplyPartyMarkup] bit NULL,
    [CropSeason] varchar(5) NULL
    );
    PRINT 'Created table: [tbl_VuAccVouchersAll]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserHistory')
BEGIN
    CREATE TABLE [dbo].[UserHistory] (
    [UserID] smallint NOT NULL,
    [Description] nvarchar(1000) NULL,
    [LogTime] datetime NOT NULL
    );
    PRINT 'Created table: [UserHistory]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UsersTasks')
BEGIN
    CREATE TABLE [dbo].[UsersTasks] (
    [UserID] smallint NOT NULL,
    [TaskKey] varchar(50) NOT NULL
    );
    PRINT 'Created table: [UsersTasks]';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ZNarration')
BEGIN
    CREATE TABLE [dbo].[ZNarration] (
    [VoucherID] bigint NOT NULL,
    [VoucherType] varchar(10) NOT NULL,
    [Narration] nvarchar(3000) NULL,
    [TTLValue] decimal(25, 2) NULL
    );
    PRINT 'Created table: [ZNarration]';
END
GO

