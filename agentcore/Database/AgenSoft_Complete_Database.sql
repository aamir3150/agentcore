/*******************************************************************************
 * AGENSOFT COMPLETE DATABASE DESIGN & DDL SCRIPT
 * 
 * Database:       AgenSoft25261
 * Source Server:  .\SQL2014
 * Extracted At:   2026-11-10 09:03:10
 * Total Tables:   89
 * Architecture:   Legacy VB6 Mandi Management System (AgenSoft)
 * Integrity:      Application-Managed (No Foreign Key Constraints in Engine)
 *******************************************************************************/

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'AgenSoft25261')
BEGIN
    CREATE DATABASE [AgenSoft25261];
END
GO

USE [AgenSoft25261];
GO

-- =============================================================================
-- TABLE: [dbo].[AccountsBalances]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[AccountsBalances]', N'U') IS NOT NULL
    DROP TABLE [dbo].[AccountsBalances];
GO

CREATE TABLE [dbo].[AccountsBalances] (
    [AccountNo] varchar(10) NOT NULL,
    [Debit] decimal(14, 2) NULL,
    [Credit] decimal(14, 2) NULL,
    [Bal] decimal(14, 2) NULL,
    [BalType] varchar(2) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[BankChequeBookHistory]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[BankChequeBookHistory]', N'U') IS NOT NULL
    DROP TABLE [dbo].[BankChequeBookHistory];
GO

CREATE TABLE [dbo].[BankChequeBookHistory] (
    [AccountNo] varchar(10) NOT NULL,
    [ChequeNo] varchar(50) NOT NULL,
    [EntryDate] smalldatetime NOT NULL,
    [IsCancelled] bit NULL,
    [Remarks] varchar(150) NULL,
    [CancelDate] smalldatetime NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[BankCheques]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[BankCheques]', N'U') IS NOT NULL
    DROP TABLE [dbo].[BankCheques];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_BankCheques] PRIMARY KEY CLUSTERED ([VoucherNo], [VoucherDate])
);
GO

-- =============================================================================
-- TABLE: [dbo].[BankChequesBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[BankChequesBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[BankChequesBody];
GO

CREATE TABLE [dbo].[BankChequesBody] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [Amount] decimal(11, 2) NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[BankDeposit]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[BankDeposit]', N'U') IS NOT NULL
    DROP TABLE [dbo].[BankDeposit];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_BankDeposit] PRIMARY KEY CLUSTERED ([VoucherNo], [VoucherDate])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Branch]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Branch]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Branch];
GO

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
    CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED ([BranchID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[BrokerageGatePass]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[BrokerageGatePass]', N'U') IS NOT NULL
    DROP TABLE [dbo].[BrokerageGatePass];
GO

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
    CONSTRAINT [PK_BrokerageGatePass] PRIMARY KEY CLUSTERED ([GatePassID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[BrokerageGatePassBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[BrokerageGatePassBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[BrokerageGatePassBody];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[BrokrageShortage]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[BrokrageShortage]', N'U') IS NOT NULL
    DROP TABLE [dbo].[BrokrageShortage];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[CashNoteDetail]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CashNoteDetail]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CashNoteDetail];
GO

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
    CONSTRAINT [PK_CashNoteDetail] PRIMARY KEY CLUSTERED ([SerialNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[ChartOfAccounts]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ChartOfAccounts]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ChartOfAccounts];
GO

CREATE TABLE [dbo].[ChartOfAccounts] (
    [AccountNo] varchar(10) NOT NULL,
    [UserID] smallint NOT NULL,
    [AccountType] varchar(11) NOT NULL,
    [AccountDepth] numeric(3, 0) NULL,
    [Narration] varchar(50) NULL,
    [ParentAccountNo] varchar(10) NULL,
    [OpeningDebit] decimal(13, 2) NOT NULL,
    [OpeningCredit] decimal(13, 2) NOT NULL,
    [AdjustedDebit] decimal(13, 2) NOT NULL,
    [AdjustedCredit] decimal(13, 2) NOT NULL,
    [IsDetailed] bit NOT NULL,
    [IsLocked] bit NOT NULL,
    [IsPosted] bit NOT NULL,
    [IsEditable] bit NOT NULL,
    [BalFlag] bit NOT NULL,
    [PLFlag] varchar(1) NOT NULL,
    [ExpFlag] bit NOT NULL,
    [ExpenseLimit] decimal(13, 2) NULL,
    [AccountName] varchar(150) NULL,
    [IsHidden] bit NULL,
    [Bank] nvarchar(50) NULL,
    [AccountNameUrdu] nvarchar(250) NULL,
    [CropSeason] varchar(5) NULL,
    CONSTRAINT [PK_ChartOfAccounts] PRIMARY KEY CLUSTERED ([AccountNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Companies]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Companies]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Companies];
GO

CREATE TABLE [dbo].[Companies] (
    [CompanyID] varchar(2) NOT NULL,
    [CompanyName] varchar(30) NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([CompanyID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CompanyLogos]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CompanyLogos]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CompanyLogos];
GO

CREATE TABLE [dbo].[CompanyLogos] (
    [ImageID] smallint NOT NULL,
    [ImageData] image NULL,
    [FooterInUrdu] nvarchar(1000) NULL,
    [FooterInEnglish] varchar(250) NULL,
    [FooterBrokerage] nvarchar(1000) NULL,
    [FooterPestro] nvarchar(1000) NULL,
    [FooterCrop] nvarchar(1000) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[ComRecords]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ComRecords]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ComRecords];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[ComSettings]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ComSettings]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ComSettings];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[Contract]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Contract]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Contract];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL,
    [IDBranchPur] bigint NULL,
    [IDBranchSale] bigint NULL,
    CONSTRAINT [PK_Contract] PRIMARY KEY CLUSTERED ([ContractID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[ContractBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ContractBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ContractBody];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[ContractInvoice]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ContractInvoice]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ContractInvoice];
GO

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
    [IsShortInvoice] bit NOT NULL,
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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NOT NULL,
    [IDBranchPur] bigint NULL,
    [IDBranchSale] bigint NULL,
    [IDBranchMultiID] varchar(20) NULL,
    [CharityRatio] decimal(10, 4) NULL,
    [CharityValue] decimal(18, 4) NULL,
    [IsCharityPer] bit NULL,
    [IsCharityMnd] bit NULL,
    CONSTRAINT [PK_ContractInvoice] PRIMARY KEY CLUSTERED ([ContractPurId])
);
GO

-- =============================================================================
-- TABLE: [dbo].[ContractInvoiceBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ContractInvoiceBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ContractInvoiceBody];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[CreditVouchers]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CreditVouchers]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CreditVouchers];
GO

CREATE TABLE [dbo].[CreditVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL,
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_CreditVouchers] PRIMARY KEY CLUSTERED ([VoucherNo], [VoucherDate])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CreditVouchersBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CreditVouchersBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CreditVouchersBody];
GO

CREATE TABLE [dbo].[CreditVouchersBody] (
    [SerialNo] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [Narration] varchar(1000) NULL,
    [Credit] decimal(25, 2) NULL,
    CONSTRAINT [PK_CreditVouchersBody] PRIMARY KEY CLUSTERED ([SerialNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsPurchaseBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsPurchaseBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsPurchaseBody];
GO

CREATE TABLE [dbo].[CropsPurchaseBody] (
    [SERIALNO] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [CropPurchaseID] int NOT NULL,
    [WeighmentID] int NOT NULL,
    [Weight] decimal(11, 4) NOT NULL,
    [Price] decimal(11, 5) NULL,
    [ProductID] varchar(5) NULL,
    [Multiplier] decimal(7, 3) NULL,
    CONSTRAINT [PK_CropsPurchaseBody] PRIMARY KEY CLUSTERED ([SERIALNO])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsPurchaseContracts]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsPurchaseContracts]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsPurchaseContracts];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsPurchaseContracts] PRIMARY KEY CLUSTERED ([PurContractID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsPurchaseHeader]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsPurchaseHeader]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsPurchaseHeader];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL,
    [BagQty] decimal(10, 0) NULL,
    [BagRate] decimal(10, 5) NULL,
    [BagAmount] decimal(10, 5) NULL,
    CONSTRAINT [PK_CropsPurchaseHeader] PRIMARY KEY CLUSTERED ([CropPurchaseID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsSaleBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsSaleBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsSaleBody];
GO

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
    [Cost] decimal(18, 6) NOT NULL,
    [WeighmentID] int NULL,
    [LotNo] varchar(10) NULL,
    CONSTRAINT [PK_CropsSaleBody] PRIMARY KEY CLUSTERED ([SERIALNO])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsSaleContracts]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsSaleContracts]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsSaleContracts];
GO

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
    [Weight] decimal(11, 4) NOT NULL,
    [ComIsMinus] bit NULL,
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsSaleContracts] PRIMARY KEY CLUSTERED ([CropSaleContractID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsSaleHeader]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsSaleHeader]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsSaleHeader];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL,
    [BagQty] decimal(10, 0) NULL,
    [BagRate] decimal(10, 5) NULL,
    [BagAmount] decimal(10, 5) NULL,
    CONSTRAINT [PK_CropsSaleHeader] PRIMARY KEY CLUSTERED ([CropSaleID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsSalesWeighments]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsSalesWeighments]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsSalesWeighments];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsSalesWeighments] PRIMARY KEY CLUSTERED ([WeighmentID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsWeighments]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsWeighments]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsWeighments];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL,
    CONSTRAINT [PK_CropsWeighments] PRIMARY KEY CLUSTERED ([WeighmentID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CropsWeighmentsSupplyModes]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CropsWeighmentsSupplyModes]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CropsWeighmentsSupplyModes];
GO

CREATE TABLE [dbo].[CropsWeighmentsSupplyModes] (
    [WeighmentID] int NOT NULL,
    [SupplyMode] varchar(20) NOT NULL,
    [SupplyModeQty] int NOT NULL,
    [Deduction] decimal(10, 2) NOT NULL,
    [TTLDeduction] decimal(10, 2) NOT NULL,
    [IsPurWeighment] bit NOT NULL,
    CONSTRAINT [PK_CropsWeighmentsSupplyModes] PRIMARY KEY CLUSTERED ([WeighmentID], [SupplyMode], [IsPurWeighment])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CurrentStock]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CurrentStock]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CurrentStock];
GO

CREATE TABLE [dbo].[CurrentStock] (
    [ProductID] varchar(5) NOT NULL,
    [Qty] decimal(11, 2) NOT NULL,
    [Cost] decimal(18, 6) NOT NULL,
    CONSTRAINT [PK_CurrentStock] PRIMARY KEY CLUSTERED ([ProductID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[CurrentStockPestro]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[CurrentStockPestro]', N'U') IS NOT NULL
    DROP TABLE [dbo].[CurrentStockPestro];
GO

CREATE TABLE [dbo].[CurrentStockPestro] (
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[DebitVouchers]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[DebitVouchers]', N'U') IS NOT NULL
    DROP TABLE [dbo].[DebitVouchers];
GO

CREATE TABLE [dbo].[DebitVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL,
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_DebitVouchers] PRIMARY KEY CLUSTERED ([VoucherNo], [VoucherDate])
);
GO

-- =============================================================================
-- TABLE: [dbo].[DebitVouchersBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[DebitVouchersBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[DebitVouchersBody];
GO

CREATE TABLE [dbo].[DebitVouchersBody] (
    [SerialNo] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [Narration] varchar(1000) NULL,
    [Debit] decimal(25, 2) NULL,
    CONSTRAINT [PK_DebitVouchersBody] PRIMARY KEY CLUSTERED ([SerialNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[ExpenseType]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ExpenseType]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ExpenseType];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[History]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[History]', N'U') IS NOT NULL
    DROP TABLE [dbo].[History];
GO

CREATE TABLE [dbo].[History] (
    [HistoryId] int IDENTITY(1,1) NOT NULL,
    [TimeKey] smalldatetime NULL,
    [ProcessName] varchar(20) NOT NULL,
    [ProcessId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNoOld] varchar(20) NULL,
    [QuantityOld] int NULL,
    [CostOld] decimal(12, 5) NULL,
    [BatchNoNew] varchar(20) NULL,
    [QuantityNew] int NULL,
    [CostNew] decimal(12, 5) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[Insurance]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Insurance]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Insurance];
GO

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
    CONSTRAINT [PK_Insurance] PRIMARY KEY CLUSTERED ([SrNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[JournalVouchers]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[JournalVouchers]', N'U') IS NOT NULL
    DROP TABLE [dbo].[JournalVouchers];
GO

CREATE TABLE [dbo].[JournalVouchers] (
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [UserNo] smallint NOT NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [VoucherTime] datetime NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL,
    [BranchID] int NOT NULL,
    [IDBranch] bigint NOT NULL,
    CONSTRAINT [PK_JournalVouchers] PRIMARY KEY CLUSTERED ([VoucherNo], [VoucherDate])
);
GO

-- =============================================================================
-- TABLE: [dbo].[JournalVouchersBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[JournalVouchersBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[JournalVouchersBody];
GO

CREATE TABLE [dbo].[JournalVouchersBody] (
    [SerialNo] numeric(10, 0) IDENTITY(1,1) NOT NULL,
    [AccountNo] varchar(10) NOT NULL,
    [VoucherNo] numeric(10, 0) NOT NULL,
    [VoucherDate] smalldatetime NOT NULL,
    [Narration] varchar(1000) NULL,
    [Debit] decimal(25, 2) NULL,
    [Credit] decimal(25, 2) NULL,
    CONSTRAINT [PK_JournalVouchersBody] PRIMARY KEY CLUSTERED ([SerialNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[MultiBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[MultiBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[MultiBody];
GO

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
    CONSTRAINT [PK_MultiBody] PRIMARY KEY CLUSTERED ([MultiID], [ProductID], [IndentNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[MultiInvoice]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[MultiInvoice]', N'U') IS NOT NULL
    DROP TABLE [dbo].[MultiInvoice];
GO

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
    [BranchID] int NOT NULL,
    [IDBranch] bigint NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[OpeningBatchPestro]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[OpeningBatchPestro]', N'U') IS NOT NULL
    DROP TABLE [dbo].[OpeningBatchPestro];
GO

CREATE TABLE [dbo].[OpeningBatchPestro] (
    [SerialID] int IDENTITY(1,1) NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [Quantity] decimal(10, 3) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[OpeningStock]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[OpeningStock]', N'U') IS NOT NULL
    DROP TABLE [dbo].[OpeningStock];
GO

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
    CONSTRAINT [PK_OpeningStock] PRIMARY KEY CLUSTERED ([ProductID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[OpeningStockPestro]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[OpeningStockPestro]', N'U') IS NOT NULL
    DROP TABLE [dbo].[OpeningStockPestro];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[Parties]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Parties]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Parties];
GO

CREATE TABLE [dbo].[Parties] (
    [PartyID] varchar(10) NOT NULL,
    [SectorID] smallint NULL,
    [Phone1] varchar(15) NULL,
    [Phone2] varchar(15) NULL,
    [Mobile] varchar(20) NULL,
    [Fax] varchar(20) NULL,
    [IsCustomer] bit NOT NULL,
    [IsVendor] bit NOT NULL,
    [PartyGroupID] int NULL,
    [STNNo] varchar(50) NULL,
    [NTNNo] varchar(50) NULL,
    [PartyName] varchar(150) NULL,
    [Address] varchar(200) NULL,
    [City] varchar(50) NULL,
    [EMail] varchar(100) NULL,
    [ContactPerson] varchar(150) NULL,
    [BankAccountNo] varchar(150) NULL,
    [NameUrdu] nvarchar(200) NULL,
    CONSTRAINT [PK_Parties] PRIMARY KEY CLUSTERED ([PartyID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[PartyGroups]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PartyGroups]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PartyGroups];
GO

CREATE TABLE [dbo].[PartyGroups] (
    [PartyGroupID] int NOT NULL,
    [GroupName] varchar(100) NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PortSettings]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PortSettings]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PortSettings];
GO

CREATE TABLE [dbo].[PortSettings] (
    [RegistryKey] varchar(50) NOT NULL,
    [RegistryValue] varchar(20) NOT NULL,
    [Description] varchar(50) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[ProductGroups]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ProductGroups]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ProductGroups];
GO

CREATE TABLE [dbo].[ProductGroups] (
    [CompanyID] varchar(2) NOT NULL,
    [GroupID] tinyint NOT NULL,
    [GroupName] varchar(25) NOT NULL,
    CONSTRAINT [PK_ProductGroups] PRIMARY KEY CLUSTERED ([CompanyID], [GroupID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Products]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Products]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Products];
GO

CREATE TABLE [dbo].[Products] (
    [ProductID] varchar(5) NOT NULL,
    [CompanyID] varchar(2) NOT NULL,
    [GroupID] tinyint NULL,
    [ProductName] nvarchar(150) NULL,
    [UnitID] tinyint NOT NULL,
    [PurchasePrice] decimal(9, 2) NOT NULL,
    [SalePrice] decimal(9, 2) NOT NULL,
    [PurDiscRatio] decimal(4, 2) NOT NULL,
    [SaleDiscRatio] decimal(4, 2) NOT NULL,
    [PurDiscValue] decimal(6, 2) NOT NULL,
    [SaleDiscValue] decimal(6, 2) NOT NULL,
    [PurSTRatio] decimal(4, 2) NOT NULL,
    [SaleSTRatio] decimal(4, 2) NOT NULL,
    [IsCrop] bit NOT NULL,
    [IsBroker] bit NULL,
    [PerBagKg] decimal(9, 5) NULL,
    [SaleAcNo] varchar(10) NULL,
    [PurAcNo] varchar(10) NULL,
    [NameUrdu] nvarchar(200) NULL,
    [IsPest] bit NULL,
    [IsGen] bit NULL,
    [CommAcc] varchar(10) NULL,
    [ExpAcc] varchar(10) NULL,
    [BarCode] nvarchar(30) NULL,
    [MinQty] decimal(10, 3) NULL,
    [MaxQty] decimal(10, 3) NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Project_Registry]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Project_Registry]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Project_Registry];
GO

CREATE TABLE [dbo].[Project_Registry] (
    [RegistryKey] varchar(40) NOT NULL,
    [ParentRegistryKey] varchar(40) NULL,
    [Narration] nvarchar(3000) NULL,
    [Value] nvarchar(3000) NULL,
    [IsEditable] bit NOT NULL,
    CONSTRAINT [PK_Project_Registry] PRIMARY KEY CLUSTERED ([RegistryKey])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Project_Tasks]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Project_Tasks]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Project_Tasks];
GO

CREATE TABLE [dbo].[Project_Tasks] (
    [TaskKey] varchar(50) NOT NULL,
    [TaskName] varchar(50) NOT NULL,
    [TaskGroup] varchar(60) NOT NULL,
    [IsAutoPost] bit NOT NULL,
    [IsPosting] bit NOT NULL,
    [Edition] varchar(3) NULL,
    CONSTRAINT [PK_Project_Tasks] PRIMARY KEY CLUSTERED ([TaskKey])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Promises]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Promises]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Promises];
GO

CREATE TABLE [dbo].[Promises] (
    [PromiseNo] numeric(10, 0) NOT NULL,
    [IsPayment] bit NOT NULL,
    [CurrentDate] smalldatetime NOT NULL,
    [PromiseDate] smalldatetime NOT NULL,
    [Narration] varchar(50) NULL,
    [Amount] decimal(11, 2) NOT NULL,
    [PartyNo] varchar(10) NOT NULL,
    [UserNo] smallint NOT NULL,
    [IsFullFilled] bit NOT NULL,
    [InducedByID] int NULL,
    CONSTRAINT [PK_Promises] PRIMARY KEY CLUSTERED ([PromiseNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Purchase]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Purchase]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Purchase];
GO

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
    CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED ([PurchaseID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurchaseBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurchaseBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurchaseBody];
GO

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
    CONSTRAINT [PK_PurchaseBody] PRIMARY KEY CLUSTERED ([PurchaseID], [ProductID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurchasesBatch]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurchasesBatch]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurchasesBatch];
GO

CREATE TABLE [dbo].[PurchasesBatch] (
    [PurchaseId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurchasesBodyPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurchasesBodyPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurchasesBodyPest];
GO

CREATE TABLE [dbo].[PurchasesBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [PurchaseId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL,
    [Bonus] smallint NOT NULL,
    [Price] decimal(20, 4) NULL,
    [DiscRatio1] decimal(4, 2) NOT NULL,
    [DiscRatio2] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalestax] decimal(11, 2) NOT NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurchasesPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurchasesPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurchasesPest];
GO

CREATE TABLE [dbo].[PurchasesPest] (
    [PurchaseId] int NOT NULL,
    [LastUpdateDate] smalldatetime NOT NULL,
    [EntryDate] smalldatetime NOT NULL,
    [BillNo] varchar(10) NULL,
    [BillDate] smalldatetime NULL,
    [OrderID] smallint NULL,
    [PreviousCredit] decimal(11, 2) NOT NULL,
    [Expenses] decimal(11, 2) NOT NULL,
    [PaidAmount] decimal(11, 2) NOT NULL,
    [VendorId] varchar(10) NOT NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL,
    [IsLocal] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL,
    [Narration] nvarchar(1000) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurPendings]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurPendings]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurPendings];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[PurPendingsBatch]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurPendingsBatch]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurPendingsBatch];
GO

CREATE TABLE [dbo].[PurPendingsBatch] (
    [PurchaseId] smallint NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(10) NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurPendingsBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurPendingsBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurPendingsBody];
GO

CREATE TABLE [dbo].[PurPendingsBody] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [PurchaseId] smallint NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL,
    [Bonus] smallint NOT NULL,
    [Price] decimal(20, 4) NULL,
    [DiscRatio1] decimal(4, 2) NOT NULL,
    [DiscRatio2] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [TTLValue] decimal(9, 2) NOT NULL,
    [TTLSalestax] decimal(9, 2) NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurReturns]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurReturns]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurReturns];
GO

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
    CONSTRAINT [PK_PurReturns] PRIMARY KEY CLUSTERED ([PurReturnID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurReturnsBatch]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurReturnsBatch]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurReturnsBatch];
GO

CREATE TABLE [dbo].[PurReturnsBatch] (
    [PReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [Quantity] int NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Cost] decimal(12, 5) NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurReturnsBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurReturnsBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurReturnsBody];
GO

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
    CONSTRAINT [PK_PurReturnsBody] PRIMARY KEY CLUSTERED ([PurReturnID], [ProductID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurReturnsBodyPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurReturnsBodyPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurReturnsBodyPest];
GO

CREATE TABLE [dbo].[PurReturnsBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [PReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL,
    [Bonus] smallint NOT NULL,
    [Price] decimal(20, 4) NULL,
    [DiscRatio1] decimal(4, 2) NOT NULL,
    [DiscRatio2] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalestax] decimal(11, 2) NOT NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[PurReturnsPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[PurReturnsPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[PurReturnsPest];
GO

CREATE TABLE [dbo].[PurReturnsPest] (
    [PReturnId] int NOT NULL,
    [PurchaseId] int NULL,
    [LastUpdateDate] smalldatetime NOT NULL,
    [ReturnDate] smalldatetime NOT NULL,
    [PreviousCredit] decimal(11, 2) NOT NULL,
    [ReceivedAmount] decimal(11, 2) NOT NULL,
    [VendorId] varchar(10) NOT NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SalePendings]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SalePendings]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SalePendings];
GO

CREATE TABLE [dbo].[SalePendings] (
    [SaleId] int NOT NULL,
    [IsPending] tinyint NOT NULL,
    [SaleDate] smalldatetime NOT NULL,
    [IsCashInvoice] bit NOT NULL,
    [IsWarranted] bit NOT NULL,
    [CustomerId] varchar(10) NOT NULL,
    [TimeKey] timestamp NULL,
    [SalesManNo] varchar(10) NULL,
    [IsShortStockInvoice] bit NOT NULL,
    [IsAutoBatch] bit NOT NULL,
    [FPData] image NULL,
    [CropSeason] varchar(5) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SalePendingsBatch]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SalePendingsBatch]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SalePendingsBatch];
GO

CREATE TABLE [dbo].[SalePendingsBatch] (
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(10) NOT NULL,
    [Quantity] int NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SalePendingsBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SalePendingsBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SalePendingsBody];
GO

CREATE TABLE [dbo].[SalePendingsBody] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL,
    [Bonus] smallint NOT NULL,
    [Price] decimal(20, 4) NULL,
    [DiscRatio1] decimal(4, 2) NOT NULL,
    [DiscRatio2] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [IsSaleOnTp] bit NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalestax] decimal(11, 2) NOT NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SaleReturns]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SaleReturns]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SaleReturns];
GO

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
    CONSTRAINT [PK_SaleReturns] PRIMARY KEY CLUSTERED ([SaleReturnID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[SaleReturnsBatch]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SaleReturnsBatch]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SaleReturnsBatch];
GO

CREATE TABLE [dbo].[SaleReturnsBatch] (
    [SReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] int NOT NULL,
    [Cost] decimal(12, 5) NOT NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SaleReturnsBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SaleReturnsBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SaleReturnsBody];
GO

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
    CONSTRAINT [PK_SaleReturnsBody] PRIMARY KEY CLUSTERED ([SaleReturnID], [ProductID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[SaleReturnsBodyPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SaleReturnsBodyPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SaleReturnsBodyPest];
GO

CREATE TABLE [dbo].[SaleReturnsBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [SReturnId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [Quantity] int NOT NULL,
    [Bonus] smallint NOT NULL,
    [Price] decimal(20, 4) NULL,
    [DiscRatio1] decimal(4, 2) NOT NULL,
    [DiscRatio2] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalestax] decimal(11, 2) NOT NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SaleReturnsPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SaleReturnsPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SaleReturnsPest];
GO

CREATE TABLE [dbo].[SaleReturnsPest] (
    [SReturnId] int NOT NULL,
    [SaleId] int NULL,
    [LastUpdateDate] smalldatetime NOT NULL,
    [ReturnDate] smalldatetime NOT NULL,
    [PreviousDebit] decimal(11, 2) NOT NULL,
    [PaidAmount] decimal(11, 2) NOT NULL,
    [SpecialDiscount] decimal(11, 2) NOT NULL,
    [CustomerId] varchar(10) NOT NULL,
    [IsUndelivered] bit NOT NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL,
    [SalesManId] varchar(10) NULL,
    [NarrationUrdu] nvarchar(1000) NULL,
    [CropSeason] varchar(5) NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[Sales]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Sales]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Sales];
GO

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
    CONSTRAINT [PK_Sales] PRIMARY KEY CLUSTERED ([SaleID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[SalesBatch]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SalesBatch]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SalesBatch];
GO

CREATE TABLE [dbo].[SalesBatch] (
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [BatchNo] varchar(20) NOT NULL,
    [ExpiryDate] smalldatetime NOT NULL,
    [Quantity] decimal(12, 5) NULL,
    [Cost] decimal(12, 5) NULL,
    [IsDeleted] bit NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SalesBody]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SalesBody]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SalesBody];
GO

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
    [SrNo] int NOT NULL,
    [PurchaseID] int NULL,
    CONSTRAINT [PK_SalesBody] PRIMARY KEY CLUSTERED ([SaleID], [ProductID], [SrNo])
);
GO

-- =============================================================================
-- TABLE: [dbo].[SalesBodyPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SalesBodyPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SalesBodyPest];
GO

CREATE TABLE [dbo].[SalesBodyPest] (
    [SerialNo] int IDENTITY(1,1) NOT NULL,
    [SaleId] int NOT NULL,
    [ProductId] varchar(5) NOT NULL,
    [IsSaleOnTp] bit NOT NULL,
    [Quantity] decimal(12, 5) NULL,
    [Bonus] smallint NOT NULL,
    [Price] decimal(20, 4) NULL,
    [DiscRatio1] decimal(4, 2) NOT NULL,
    [DiscRatio2] decimal(4, 2) NOT NULL,
    [STValue] decimal(7, 2) NOT NULL,
    [STRatio] decimal(4, 2) NOT NULL,
    [TTLValue] decimal(11, 2) NOT NULL,
    [TTLSalestax] decimal(11, 2) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [IsAdvance] bit NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[Salesmen]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Salesmen]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Salesmen];
GO

CREATE TABLE [dbo].[Salesmen] (
    [SalesManId] varchar(10) NOT NULL,
    [Address] varchar(80) NOT NULL,
    [City] varchar(30) NOT NULL,
    [HomePhone] varchar(15) NULL,
    [Mobile] varchar(15) NULL,
    [EMail] varchar(40) NULL,
    [Qualification] varchar(15) NULL,
    [Designation] varchar(15) NULL,
    [JoiningDate] datetime NULL,
    [BasicSalary] int NULL,
    [TravelExpense] int NULL,
    [VehicleAlloted] varchar(15) NULL,
    [Commission] decimal(6, 2) NULL,
    [IncomTax] decimal(6, 2) NULL,
    [OtherTax] decimal(6, 2) NULL,
    [IsMarried] bit NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[SalesPest]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[SalesPest]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SalesPest];
GO

CREATE TABLE [dbo].[SalesPest] (
    [SaleId] int NOT NULL,
    [SaleType] varchar(1) NOT NULL,
    [ExternalId] int NOT NULL,
    [SalesManId] varchar(10) NULL,
    [CustomerId] varchar(10) NOT NULL,
    [LastUpdateDate] smalldatetime NOT NULL,
    [SaleDate] smalldatetime NOT NULL,
    [IsCashInvoice] bit NOT NULL,
    [IsWarranted] bit NOT NULL,
    [PreviousDebit] decimal(11, 2) NOT NULL,
    [IsAutoBatch] bit NOT NULL,
    [ReceivedAmount] decimal(11, 2) NOT NULL,
    [SpecialDiscount] decimal(11, 2) NOT NULL,
    [DueDate] smalldatetime NOT NULL,
    [IsPrinted] bit NOT NULL,
    [Remarks] varchar(96) NULL,
    [TimeKey] timestamp NULL,
    [IsPosted] bit NOT NULL,
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
GO

-- =============================================================================
-- TABLE: [dbo].[Sectors]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Sectors]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Sectors];
GO

CREATE TABLE [dbo].[Sectors] (
    [SectorID] smallint NOT NULL,
    [TownID] int NOT NULL,
    [SectorName] varchar(30) NOT NULL,
    CONSTRAINT [PK_Sectors] PRIMARY KEY CLUSTERED ([SectorID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[TasksRepeated]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[TasksRepeated]', N'U') IS NOT NULL
    DROP TABLE [dbo].[TasksRepeated];
GO

CREATE TABLE [dbo].[TasksRepeated] (
    [RepeatedTaskId] int NOT NULL,
    [UserNo] smallint NOT NULL,
    [Subject] varchar(30) NULL,
    [Description] varchar(200) NULL,
    [Priority] varchar(6) NULL,
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
    CONSTRAINT [PK_TasksRepeated] PRIMARY KEY CLUSTERED ([RepeatedTaskId])
);
GO

-- =============================================================================
-- TABLE: [dbo].[TasksToDo]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[TasksToDo]', N'U') IS NOT NULL
    DROP TABLE [dbo].[TasksToDo];
GO

CREATE TABLE [dbo].[TasksToDo] (
    [SubjectId] int NOT NULL,
    [UserNo] smallint NOT NULL,
    [Subject] varchar(30) NOT NULL,
    [Description] varchar(200) NULL,
    [DueDateTime] smalldatetime NULL,
    [Priority] varchar(6) NULL,
    [Status] varchar(25) NULL,
    [RemindDateTime] smalldatetime NULL,
    [Sound] bit NULL,
    CONSTRAINT [PK_TasksToDo] PRIMARY KEY CLUSTERED ([SubjectId])
);
GO

-- =============================================================================
-- TABLE: [dbo].[tbl_VuAccVouchersAll]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[tbl_VuAccVouchersAll]', N'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_VuAccVouchersAll];
GO

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
GO

-- =============================================================================
-- TABLE: [dbo].[Towns]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Towns]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Towns];
GO

CREATE TABLE [dbo].[Towns] (
    [TownID] int NOT NULL,
    [TownName] varchar(30) NOT NULL,
    CONSTRAINT [PK_Towns] PRIMARY KEY CLUSTERED ([TownID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[Units]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Units]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Units];
GO

CREATE TABLE [dbo].[Units] (
    [UnitID] tinyint NOT NULL,
    [UnitName] varchar(15) NOT NULL,
    [Multiplier] decimal(7, 3) NOT NULL,
    CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED ([UnitID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[UserHistory]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[UserHistory]', N'U') IS NOT NULL
    DROP TABLE [dbo].[UserHistory];
GO

CREATE TABLE [dbo].[UserHistory] (
    [UserID] smallint NOT NULL,
    [Description] nvarchar(1000) NULL,
    [LogTime] datetime NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[Users]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

CREATE TABLE [dbo].[Users] (
    [UserID] smallint NOT NULL,
    [UserName] varchar(30) NOT NULL,
    [Password] varchar(20) NOT NULL,
    [Narration] varchar(50) NULL,
    [IsAdministrator] bit NOT NULL,
    [IsEnabled] bit NOT NULL,
    [FPData] binary(2000) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID])
);
GO

-- =============================================================================
-- TABLE: [dbo].[UsersTasks]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[UsersTasks]', N'U') IS NOT NULL
    DROP TABLE [dbo].[UsersTasks];
GO

CREATE TABLE [dbo].[UsersTasks] (
    [UserID] smallint NOT NULL,
    [TaskKey] varchar(50) NOT NULL
);
GO

-- =============================================================================
-- TABLE: [dbo].[ZNarration]
-- =============================================================================
IF OBJECT_ID(N'[dbo].[ZNarration]', N'U') IS NOT NULL
    DROP TABLE [dbo].[ZNarration];
GO

CREATE TABLE [dbo].[ZNarration] (
    [VoucherID] bigint NOT NULL,
    [VoucherType] varchar(10) NOT NULL,
    [Narration] nvarchar(3000) NULL,
    [TTLValue] decimal(25, 2) NULL
);
GO


-- =============================================================================
-- SECTION 2: USER-DEFINED FUNCTIONS (44 Functions)
-- (Required by Views and Stored Procedures)
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Function: [dbo].[RegistryValue]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[RegistryValue]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[RegistryValue];
IF OBJECT_ID(N'[dbo].[RegistryValue]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[RegistryValue];
IF OBJECT_ID(N'[dbo].[RegistryValue]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[RegistryValue];
GO

CREATE FUNCTION dbo.RegistryValue (@RegistryKey Varchar(50))

RETURNS Varchar(70)  AS  

BEGIN 

    Declare @Value Varchar(70)

    Select @Value = isnull(Value,'') From Project_Registry Where RegistryKey = @RegistryKey

    Return @Value

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[Value]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[Value]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[Value];
IF OBJECT_ID(N'[dbo].[Value]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[Value];
IF OBJECT_ID(N'[dbo].[Value]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[Value];
GO

CREATE FUNCTION dbo.Value (@RegistryKey Varchar(50))



RETURNS Varchar(70)  AS  



BEGIN 



    Declare @Value Varchar(70)



    Select @Value = Value From Project_Registry Where RegistryKey = @RegistryKey



    Return @Value



END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[DefaultMinDate]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[DefaultMinDate]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[DefaultMinDate];
IF OBJECT_ID(N'[dbo].[DefaultMinDate]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[DefaultMinDate];
IF OBJECT_ID(N'[dbo].[DefaultMinDate]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[DefaultMinDate];
GO

CREATE FUNCTION [dbo].[DefaultMinDate]()

RETURNS SMALLDATETIME 

 

AS



BEGIN 



    DECLARE



        @RtnValue AS smalldatetime

    SELECT @RtnValue = MIN(ChoosenDate)

FROM (

    SELECT MIN(VoucherDate)        AS ChoosenDate         FROM CreditVouchers        

UNION ALL

    SELECT MIN(VoucherDate)        AS ChoosenDate         FROM DebitVouchers        

UNION ALL

    SELECT MIN(VoucherDate)        AS ChoosenDate         FROM JournalVouchers         

UNION ALL

    SELECT MIN(VoucherDate)        AS ChoosenDate         FROM BankCheques        

UNION ALL

    SELECT MIN(VoucherDate)        AS ChoosenDate         FROM BankDeposit        

UNION ALL

    SELECT MIN(ReconcileDate)      AS ChoosenDate         FROM BankCheques        

UNION ALL

    SELECT MIN(ReconcileDate)      AS ChoosenDate         FROM BankDeposit        

UNION ALL

    SELECT MIN(DepositedDate)      AS ChoosenDate         FROM BankDeposit        

UNION ALL

    SELECT MIN(WeighmentDate)      AS ChoosenDate         FROM CropsWeighments    

UNION ALL

    SELECT MIN(ContractDate)      AS ChoosenDate         FROM CropsPurchaseContracts

UNION ALL

    SELECT MIN(PurchaseDate)      AS ChoosenDate         FROM CropsPurchaseHeader    

UNION ALL

    SELECT MIN(ContractDate)      AS ChoosenDate         FROM CropsSaleContracts

UNION ALL

    SELECT MIN(SaleDate)          AS ChoosenDate         FROM CropsSaleHeader

UNION ALL

    SELECT MIN(PurchaseDate)      AS ChoosenDate         FROM Purchase

UNION ALL

    SELECT MIN(PurReturnDate)      AS ChoosenDate         FROM PurReturns

UNION ALL

    SELECT MIN(SaleReturnDate)      AS ChoosenDate         FROM SaleReturns

UNION ALL

    SELECT MIN(SaleDate)          AS ChoosenDate         FROM Sales

UNION ALL

    SELECT MIN(CDate)          AS ChoosenDate         FROM ContractInvoice

) MAXDate

   IF @RtnValue IS NULL



    SELECT @RtnValue=CAST('01/01/2006' AS SMALLDATETIME)

--SELECT @RtnValue

    RETURN @RtnValue



END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[DefaultMaxDate]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[DefaultMaxDate]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[DefaultMaxDate];
IF OBJECT_ID(N'[dbo].[DefaultMaxDate]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[DefaultMaxDate];
IF OBJECT_ID(N'[dbo].[DefaultMaxDate]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[DefaultMaxDate];
GO

CREATE FUNCTION [dbo].[DefaultMaxDate]()

RETURNS SMALLDATETIME 

 

AS



BEGIN 



    DECLARE



        @RtnValue AS smalldatetime

    SELECT @RtnValue = MAX(ChoosenDate)

FROM (

    SELECT MAX(VoucherDate)        AS ChoosenDate         FROM CreditVouchers        

UNION ALL

    SELECT MAX(VoucherDate)        AS ChoosenDate         FROM DebitVouchers        

UNION ALL

    SELECT MAX(VoucherDate)        AS ChoosenDate         FROM JournalVouchers         

UNION ALL

    SELECT MAX(VoucherDate)        AS ChoosenDate         FROM BankCheques        

UNION ALL

    SELECT MAX(VoucherDate)        AS ChoosenDate         FROM BankDeposit        

UNION ALL

    SELECT MAX(ReconcileDate)      AS ChoosenDate         FROM BankCheques        

UNION ALL

    SELECT MAX(ReconcileDate)      AS ChoosenDate         FROM BankDeposit        

UNION ALL

    SELECT MAX(DepositedDate)      AS ChoosenDate         FROM BankDeposit        

UNION ALL

    SELECT MAX(WeighmentDate)      AS ChoosenDate         FROM CropsWeighments    

UNION ALL

    SELECT MAX(ContractDate)      AS ChoosenDate         FROM CropsPurchaseContracts

UNION ALL

    SELECT MAX(PurchaseDate)      AS ChoosenDate         FROM CropsPurchaseHeader    

UNION ALL

    SELECT MAX(ContractDate)      AS ChoosenDate         FROM CropsSaleContracts

UNION ALL

    SELECT MAX(SaleDate)          AS ChoosenDate         FROM CropsSaleHeader

UNION ALL

    SELECT MAX(PurchaseDate)      AS ChoosenDate         FROM Purchase

UNION ALL

    SELECT MAX(PurReturnDate)      AS ChoosenDate         FROM PurReturns

UNION ALL

    SELECT MAX(SaleReturnDate)      AS ChoosenDate         FROM SaleReturns

UNION ALL

    SELECT MAX(SaleDate)          AS ChoosenDate         FROM Sales

UNION ALL

    SELECT MAX(CDate)          AS ChoosenDate         FROM ContractInvoice

) MAXDate

   IF @RtnValue IS NULL



    SELECT @RtnValue=CAST('01/01/2006' AS SMALLDATETIME)

--SELECT @RtnValue

    RETURN @RtnValue



END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[KgsToMounds]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[KgsToMounds]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[KgsToMounds];
IF OBJECT_ID(N'[dbo].[KgsToMounds]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[KgsToMounds];
IF OBJECT_ID(N'[dbo].[KgsToMounds]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[KgsToMounds];
GO

CREATE FUNCTION dbo.KgsToMounds (@Kgs Decimal(18,9), @Unit Decimal(6,3))

RETURNS Varchar(30)  AS  

BEGIN 

    Declare @Value Varchar(30)

    Set @Value = cast(cast(@Kgs/@Unit as bigint) as varchar) + ' - ' + cast(cast(Round(((@Kgs/@Unit)-cast(@Kgs/@Unit as bigint))*@Unit,0) as bigint) as varchar)

    Return @Value

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunTTLCost]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunTTLCost]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunTTLCost];
IF OBJECT_ID(N'[dbo].[FunTTLCost]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLCost];
IF OBJECT_ID(N'[dbo].[FunTTLCost]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLCost];
GO

CREATE FUNCTION dbo.FunTTLCost (@Multiplier decimal(7,3), @Qty decimal(11,2), @Bonus decimal(11,2), @Price Decimal(11,2), @DiscRatio Decimal(4,2), @DiscValue Decimal(7,2), @STRatio Decimal(4,2), @STValue Decimal(7,2) )  



RETURNS Decimal(9,2)



AS



BEGIN 



    DECLARE @TTLSalestax Decimal(18,6), @TTLValue decimal(18,6), @TTLCost decimal(18,6)



    SET @TTLValue =  ( @Price * @Qty * (1 - @DiscRatio/100)) - (@DiscValue) 



    SET @TTLSalestax =  ((@Qty + @Bonus) * @STValue)  +  (@TTLValue * @STRatio/100) 



    SET @TTLCost = (@TTLValue + @TTLSalestax) / ((@Qty + @Bonus) * @Multiplier) 

    

    --RETURN  ((( @Price * @Qty * (1 - @DiscRatio/100)) - (@DiscValue) ) + (((@Qty + @Bonus) * @STValue)  +  ((( @Price * @Qty * (1 - @DiscRatio/100)) - (@DiscValue) ) * @STRatio/100) )) / ((@Qty + @Bonus) * @Multiplier) 



    RETURN @TTLCost

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunTTLDiscValue]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunTTLDiscValue]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunTTLDiscValue];
IF OBJECT_ID(N'[dbo].[FunTTLDiscValue]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLDiscValue];
IF OBJECT_ID(N'[dbo].[FunTTLDiscValue]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLDiscValue];
GO

CREATE FUNCTION dbo.FunTTLDiscValue (@Multiplier decimal(7,3), @Qty decimal(11,2), @Bonus decimal(11,2), @Price Decimal(11,2), @DiscRatio Decimal(4,2), @DiscValue Decimal(7,2) )  



RETURNS Decimal(9,2)



AS



BEGIN 



    DECLARE @TTLDiscValue Decimal(11,2)



    if @DiscRatio>0 or @DiscValue>0 

        SET @TTLDiscValue = ROUND( (@Price * @Qty) - (( @Price * @Qty * (1 - @DiscRatio/100)) - (@DiscValue)) , 2)

    else 

    

        SET @TTLDiscValue = 0



    RETURN @TTLDiscValue

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunTTLSalesTax]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunTTLSalesTax]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunTTLSalesTax];
IF OBJECT_ID(N'[dbo].[FunTTLSalesTax]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLSalesTax];
IF OBJECT_ID(N'[dbo].[FunTTLSalesTax]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLSalesTax];
GO

CREATE FUNCTION dbo.FunTTLSalesTax (@Multiplier decimal(7,3), @Qty decimal(11,2), @Bonus decimal(11,2), @Price Decimal(11,2), @DiscRatio Decimal(4,2), @DiscValue Decimal(7,2), @STRatio Decimal(4,2), @STValue Decimal(7,2) )  



RETURNS Decimal(9,2)



AS



BEGIN 



    DECLARE @TTLSalestax Decimal(11,2), @TTLValue decimal(11,2)



    SET @TTLValue = ROUND( ( @Price * @Qty * (1 - @DiscRatio/100)) - (@DiscValue) , 2)



    SET @TTLSalestax = Round( ((@Qty + @Bonus) * @STValue)  +  (@TTLValue * @STRatio/100) ,2)



    RETURN @TTLSalestax

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunTTLValue]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunTTLValue]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunTTLValue];
IF OBJECT_ID(N'[dbo].[FunTTLValue]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLValue];
IF OBJECT_ID(N'[dbo].[FunTTLValue]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunTTLValue];
GO

CREATE FUNCTION dbo.FunTTLValue (@Multiplier decimal(7,3), @Qty decimal(11,2), @Bonus decimal(11,2), @Price Decimal(11,2), @DiscRatio Decimal(4,2), @DiscValue Decimal(7,2) )  



RETURNS Decimal(9,2)



AS



BEGIN 



    DECLARE @TTLValue Decimal(11,2)



    SET @TTLValue = ROUND( ( @Price * @Qty * (1 - @DiscRatio/100)) - (@DiscValue) , 2)



    RETURN @TTLValue

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPestroCost]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPestroCost]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPestroCost];
IF OBJECT_ID(N'[dbo].[FunGetPestroCost]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPestroCost];
IF OBJECT_ID(N'[dbo].[FunGetPestroCost]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPestroCost];
GO

CREATE FUNCTION [dbo].FunGetPestroCost (@ProductID varchar(5), @BatchNo as varchar(20))  



RETURNS Decimal(18,6)



AS



BEGIN 



    DECLARE @Cost decimal(18,6)

    SET @COST = 0

BEGIN



SELECT  @COST =   



CASE WHEN (SUM(PP.TTLValue) / SUM(PP.QTY)) = 0 THEN AVG(Products.PurchasePrice) ELSE (SUM(PP.TTLValue) / SUM(PP.QTY)) END 

FROM         (SELECT     ProductId, BatchNo, SUM(Quantity) AS QTY, SUM(Quantity * Cost) AS TTLValue

                       FROM          OpeningBatchPestro

                       GROUP BY ProductId, BatchNo

                       UNION ALL

                       SELECT     ProductId, BatchNo, SUM(Quantity) AS Expr1, SUM(Quantity * Cost) AS Expr2

                       FROM         PurchasesBatch

                       GROUP BY ProductId, BatchNo

                       UNION ALL

                       SELECT     ProductId, BatchNo, - SUM(Quantity) AS Expr1, - SUM(Quantity * Cost) AS Expr2

                       FROM         PurReturnsBatch

                       GROUP BY ProductId, BatchNo) AS PP INNER JOIN

                      Products ON PP.ProductId = Products.ProductID

                      Where 1=1 AND PP.ProductId = @ProductID and PP.BatchNo = @BatchNo

                      GROUP BY PP.ProductId, PP.BatchNo

END    

    RETURN @Cost

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPestroExpiry]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPestroExpiry]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPestroExpiry];
IF OBJECT_ID(N'[dbo].[FunGetPestroExpiry]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPestroExpiry];
IF OBJECT_ID(N'[dbo].[FunGetPestroExpiry]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPestroExpiry];
GO

CREATE FUNCTION [dbo].[FunGetPestroExpiry] (@ProductID varchar(5), @BatchNo as varchar(20))  



RETURNS smalldatetime



AS



BEGIN 



    DECLARE @Expiry smalldatetime

    SET @Expiry =  '1/1/2000'

BEGIN



SELECT  @Expiry =   



MAX(ExpiryDate) 

FROM         (SELECT     ExpiryDate

                       FROM          OpeningBatchPestro

                       WHERE      (ProductId = @ProductID) AND (BatchNo = @BatchNo)

                       UNION ALL

                       SELECT     ExpiryDate

                       FROM         PurchasesBatch

                       WHERE     (ProductId = @ProductID) AND (BatchNo = @BatchNo)

                        UNION ALL

                       SELECT     ExpiryDate

                       FROM         SaleReturnsBatch

                       WHERE     (ProductId = @ProductID) AND (BatchNo = @BatchNo)

                       ) AS PPP

END    

    RETURN @Expiry

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetCropCost]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetCropCost]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropCost];
IF OBJECT_ID(N'[dbo].[FunGetCropCost]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropCost];
IF OBJECT_ID(N'[dbo].[FunGetCropCost]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropCost];
GO

CREATE FUNCTION [FunGetCropCost] (@ProductID INT)  



RETURNS Decimal(18,6)



AS



BEGIN 



    DECLARE @Cost decimal(18,6)

    SET @COST = 0

BEGIN



SELECT  @COST =   

--SUM(TEMP.TTLValue) / SUM(TEMP.QTY)

CASE WHEN (SUM(TEMP.TTLValue) / SUM(TEMP.QTY)) = 0 THEN AVG(Products.PurchasePrice) ELSE (SUM(TEMP.TTLValue) / SUM(TEMP.QTY)) END 

FROM         (SELECT     SUM((CropsPurchaseBody.Price / CropsPurchaseBody.Multiplier * CropsPurchaseBody.Weight) ) AS TTLValue, 

                                              SUM(CropsPurchaseBody.Weight) AS QTY, CropsPurchaseBody.ProductID

                       FROM          CropsPurchaseBody INNER JOIN

                                              CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

                       GROUP BY CropsPurchaseBody.ProductID

                       UNION ALL

                       SELECT     SUM(TTLValue) AS TTLValue, SUM(Qty * Multiplier) AS QTY, ProductID

                       FROM         OpeningStock

                       GROUP BY ProductID) AS TEMP INNER JOIN

                      Products ON TEMP.ProductID = Products.ProductID

WHERE     (temp.ProductID = @ProductID)



  GROUP BY TEMP.ProductID  

END    

    RETURN @Cost

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetCropCostDW]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetCropCostDW]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropCostDW];
IF OBJECT_ID(N'[dbo].[FunGetCropCostDW]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropCostDW];
IF OBJECT_ID(N'[dbo].[FunGetCropCostDW]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropCostDW];
GO

CREATE FUNCTION FunGetCropCostDW (@ProductID INT,@ToDate as smalldatetime)  



RETURNS Decimal(18,6)



AS



BEGIN 



    DECLARE @Cost decimal(18,6)

    SET @COST = 0

BEGIN



SELECT  @COST =   

--SUM(TEMP.TTLValue) / SUM(TEMP.QTY)

CASE WHEN (SUM(TEMP.TTLValue) / SUM(TEMP.QTY)) = 0 THEN AVG(Products.PurchasePrice) ELSE (SUM(TEMP.TTLValue) / SUM(TEMP.QTY)) END 

FROM         (SELECT     SUM((CropsPurchaseBody.Price / CropsPurchaseBody.Multiplier * CropsPurchaseBody.Weight) ) AS TTLValue, 

                                              SUM(CropsPurchaseBody.Weight) AS QTY, CropsPurchaseBody.ProductID

                       FROM          CropsPurchaseBody INNER JOIN

                                              CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

                       Where CropsPurchaseHeader.PurchaseDate <= @ToDate

                       GROUP BY CropsPurchaseBody.ProductID

                       UNION ALL

                       SELECT     SUM(TTLValue) AS TTLValue, SUM(Qty * Multiplier) AS QTY, ProductID

                       FROM         OpeningStock

                       GROUP BY ProductID) AS TEMP INNER JOIN

                      Products ON TEMP.ProductID = Products.ProductID

WHERE     (temp.ProductID = @ProductID)



  GROUP BY TEMP.ProductID  

END    

    RETURN @Cost

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetCropPurchaseNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetCropPurchaseNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropPurchaseNarration];
IF OBJECT_ID(N'[dbo].[FunGetCropPurchaseNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropPurchaseNarration];
IF OBJECT_ID(N'[dbo].[FunGetCropPurchaseNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropPurchaseNarration];
GO

CREATE    Function FunGetCropPurchaseNarration(

    @CropPurchaseId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY





FOR Select Products.ProductName+':'+cast(cast(CropsPurchaseBody.Weight/CropsPurchaseBody.Multiplier as int) as varchar) + '-' + cast(cast(Round(((CropsPurchaseBody.Weight/CropsPurchaseBody.Multiplier)-cast(CropsPurchaseBody.Weight/CropsPurchaseBody.Multiplier as int))*CropsPurchaseBody.Multiplier,0) as int) as varchar) +'Mnd-' + Cast(Cast(CropsPurchaseBody.Weight AS Decimal(18,0)) AS Varchar)+'Kgs @'+ cast(cast(CropsPurchaseBody.Price as decimal(11,2)) as varchar)+'. Rs.'+ cast(cast(CropsPurchaseBody.Weight/CropsPurchaseBody.Multiplier*CropsPurchaseBody.Price  as decimal(18,2)) as varchar)  as Remarks From CropsPurchaseBody INNER JOIN CropsWeighments ON CropsWeighments.WeighmentId = CropsPurchaseBody.WeighmentId INNER JOIN Products ON Products.ProductId = CropsWeighments.ProductId Where CropsPurchaseBody.CropPurchaseId=@CropPurchaseId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetCropSaleNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetCropSaleNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropSaleNarration];
IF OBJECT_ID(N'[dbo].[FunGetCropSaleNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropSaleNarration];
IF OBJECT_ID(N'[dbo].[FunGetCropSaleNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCropSaleNarration];
GO

CREATE    Function FunGetCropSaleNarration(

    @CropSaleId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR Select Products.ProductName+':'+cast(cast(CropsSaleBody.Weight/CropsSaleBody.Multiplier as int) as varchar) + '-' + cast(cast(Round(((CropsSaleBody.Weight/CropsSaleBody.Multiplier)-cast(CropsSaleBody.Weight/CropsSaleBody.Multiplier as int))*CropsSaleBody.Multiplier,0) as int) as varchar)+'Mnd-' + Cast(Cast(CropsSaleBody.Weight AS Decimal(18,0)) AS Varchar)+'Kgs @'+cast(cast(CropsSaleBody.Price as decimal(11,2)) as varchar)+'. Rs.'+ cast(cast(CropsSaleBody.Weight/CropsSaleBody.Multiplier*CropsSaleBody.Price  as decimal(18,2)) as varchar)   as Remarks From CropsSaleBody INNER JOIN Products ON Products.ProductId = CropsSaleBody.ProductId Where CropsSaleBody.CropSaleId=@CropSaleId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPurchaseNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPurchaseNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPurchaseNarration];
IF OBJECT_ID(N'[dbo].[FunGetPurchaseNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPurchaseNarration];
IF OBJECT_ID(N'[dbo].[FunGetPurchaseNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPurchaseNarration];
GO

CREATE    Function FunGetPurchaseNarration(

    @PurchaseId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR Select Products.ProductName+':'+cast(PurchaseBody.Qty as varchar)+'@'+cast(PurchaseBody.Price as varchar)+'. Rs.'+ cast(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax as varchar)   as Remarks From PurchaseBody INNER JOIN Products ON Products.ProductId = PurchaseBody.ProductId Where PurchaseBody.PurchaseId=@PurchaseId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetSalesNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetSalesNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetSalesNarration];
IF OBJECT_ID(N'[dbo].[FunGetSalesNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetSalesNarration];
IF OBJECT_ID(N'[dbo].[FunGetSalesNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetSalesNarration];
GO

CREATE    Function [FunGetSalesNarration]( @SaleId int)    Returns nVarchar(3990) AS   Begin   Declare @FetchRemarks nvarchar(3990)    Declare @RemarksList nvarchar(3990) set @RemarksList = ''   DECLARE curSales CURSOR READ_ONLY  FOR 

    Select Products.ProductName +':'+cast(SalesBody.Qty as varchar)+'@'+cast(SalesBody.Price as varchar) + case when SalesBody.DiscRatio > 0 then +', D%.'+  cast(SalesBody.DiscRatio as varchar) else '' end +', Rs.'+cast(SalesBody.TTLValue+SalesBody.TTLSalesTax as varchar) +', '+ IsNull(SalesBody.Narration,'')   as Remarks 

    From SalesBody INNER JOIN Products ON Products.ProductId = SalesBody.ProductId Where SalesBody.SaleId=@SaleId  OPEN curSales   FETCH NEXT FROM curSales INTO @FetchRemarks WHILE (@@fetch_status = 0)  BEGIN   set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end    set @RemarksList = @RemarksList  + @FetchRemarks    FETCH NEXT FROM curSales INTO @FetchRemarks END CLOSE curSales  DEALLOCATE curSales Return @RemarksList end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPurReturnsNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPurReturnsNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPurReturnsNarration];
IF OBJECT_ID(N'[dbo].[FunGetPurReturnsNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPurReturnsNarration];
IF OBJECT_ID(N'[dbo].[FunGetPurReturnsNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPurReturnsNarration];
GO

CREATE    Function FunGetPurReturnsNarration(

    @PurReturnId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR Select Products.ProductName+':'+cast(PurReturnsBody.Qty as varchar)+'@'+cast(PurReturnsBody.Price as varchar)+'. Rs.'+cast(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax as varchar) as Remarks From PurReturnsBody INNER JOIN Products ON Products.ProductId = PurReturnsBody.ProductId Where PurReturnsBody.PurReturnId=@PurReturnId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetSaleReturnsNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetSaleReturnsNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetSaleReturnsNarration];
IF OBJECT_ID(N'[dbo].[FunGetSaleReturnsNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetSaleReturnsNarration];
IF OBJECT_ID(N'[dbo].[FunGetSaleReturnsNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetSaleReturnsNarration];
GO

CREATE    Function FunGetSaleReturnsNarration(

    @SaleReturnId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR Select Products.ProductName+':'+cast(SaleReturnsBody.Qty as varchar)+'@'+cast(SaleReturnsBody.Price as varchar)+'. Rs.'+cast(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax as varchar)  as Remarks From SaleReturnsBody INNER JOIN Products ON Products.ProductId = SaleReturnsBody.ProductId Where SaleReturnsBody.SaleReturnId=@SaleReturnId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetContractPurchaseInvoiceNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetContractPurchaseInvoiceNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractPurchaseInvoiceNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractPurchaseInvoiceNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractPurchaseInvoiceNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractPurchaseInvoiceNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractPurchaseInvoiceNarration];
GO

CREATE    Function [FunGetContractPurchaseInvoiceNarration](@ContractPurchaseInvoiceId int)Returns nVarchar(3990) AS Begin Declare @FetchRemarks nvarchar(3990) Declare @RemarksList nvarchar(3990) set @RemarksList = '' DECLARE curSales CURSOR READ_ONLY FOR  SELECT +'Pr.'+ Products.ProductName  + (case When IsNull(ContractInvoiceBody.VehicleNo,'')<>'' Then '--VehNo.' + cast(ContractInvoiceBody.VehicleNo AS varchar) Else '' END)+ (case when Isnull(ContractInvoiceBody.TruckQty,0) > 0 then  '--T.' + cast(ContractInvoiceBody.TruckQty AS varchar) Else '' END)+ (Case when Isnull(ContractInvoiceBody.BagsQty,0) > 0 then '--B.' + cast(ContractInvoiceBody.BagsQty AS varchar) else '' end)+ (Case when Isnull(ContractInvoiceBody.PQty,0) > 0 then '--P.' + cast(ContractInvoiceBody.PQty AS varchar) else '' end)+ (case when isnull(ContractInvoiceBody.TotalKgs,0) > 0 then '--WKg.' + cast(ContractInvoiceBody.TotalKgs AS varchar) else '' end)+ '--WM.' + cast(Dbo.KgsToMounds(ContractInvoiceBody.TotalKgs,ContractInvoiceBody.MoundStd) AS varchar) + '--R.' + cast(ContractInvoiceBody.RatePerMound AS varchar) + (Case When ContractInvoiceBody.Bardana>0 THEN '--Bar.' + cast(ContractInvoiceBody.Bardana AS varchar) ELSE '' END)  + (Case When IsNull(ContractInvoice.CommType,'')<>'' Then '--CT.' + cast(ContractInvoice.CommType AS varchar) Else '' END)+ (Case When ContractInvoice.CommissionValue>0 THEN  '--CV.' + cast(ContractInvoice.CommissionValue AS varchar) ELSE '' END)+ (Case When ContractInvoice.OtherExpenceMinus>0 THEN '--Exp(-).' + cast(ContractInvoice.OtherExpenceMinus AS varchar) ELSE '' END)+ (Case When ContractInvoice.OtherExpencePlus>0 THEN '--Exp(+).' + cast(ContractInvoice.OtherExpencePlus AS varchar) ELSE '' END)+ ' ,' + case when dbo.RegistryValue('HideInvoiceNarrationInLedger') > 0 then '' else isnull(ContractInvoice.Narration,'') end AS Remarks FROM ContractInvoiceBody INNER JOIN Products ON  Products.ProductId = ContractInvoiceBody.ProductId INNER JOIN ContractInvoice ON  ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId WHERE ContractInvoiceBody.ContractPurId = @ContractPurchaseInvoiceId  And ContractInvoice.IsSaleInvoice=0 OPEN curSales FETCH NEXT FROM curSales INTO @FetchRemarks WHILE (@@fetch_status = 0) BEGIN  set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end set @RemarksList = @RemarksList  + @FetchRemarks  FETCH NEXT FROM curSales INTO @FetchRemarks END CLOSE curSales DEALLOCATE curSales  set @RemarksList = @RemarksList + ( Select (Case When VehicalCharges> 0 THEN ( '--VEx.'+ convert(varchar,VehicalCharges)) ELSE '' END) From ContractInvoice Where ContractInvoice.ContractPurId = @ContractPurchaseInvoiceId) Return @RemarksList end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetContractPurchaseNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetContractPurchaseNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractPurchaseNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractPurchaseNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractPurchaseNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractPurchaseNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractPurchaseNarration];
GO

CREATE    Function FunGetContractPurchaseNarration(

    @ContractPurchaseId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor    

--By fahad at 7/10/06

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR SELECT +'P.'+ Products.ProductName + '|T.' + cast(ContractBody.TruckQty AS varchar) 

+ '|B.' + cast(ContractBody.BagsQty AS varchar) + '|W.' + cast(ContractBody.WeightPerBag AS varchar) 

+ '|R.' + cast(ContractBody.RatePerMound AS varchar) 

    AS Remarks

FROM ContractBody INNER JOIN

    Products ON 

    Products.ProductId = ContractBody.ProductId INNER JOIN Contract ON 

    ContractBody.ContractId = Contract.ContractId

WHERE ContractBody.ContractId = @ContractPurchaseId And Contract.IsSaleContract=0



OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

























































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetContractSaleInvoiceNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetContractSaleInvoiceNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractSaleInvoiceNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractSaleInvoiceNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractSaleInvoiceNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractSaleInvoiceNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractSaleInvoiceNarration];
GO

CREATE    Function [dbo].[FunGetContractSaleInvoiceNarration](

    @ContractSaleInvoiceId int)

Returns Varchar(5000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor    

--By fahad at 7/10/06

-- =============================================

Declare @FetchRemarks varchar(1000)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR 

SELECT +'Pr.'+ Products.ProductName 

+ (case When IsNull(ContractInvoiceBody.VehicleNo,'')<>'' Then '--VehNo.' + cast(ContractInvoiceBody.VehicleNo AS varchar) Else '' END)

+ (case when isnull(ContractInvoiceBody.TruckQty,0) > 0 then '--T.' + cast(ContractInvoiceBody.TruckQty AS varchar) else '' end)

+ (case when isnull(ContractInvoiceBody.BagsQty,0) > 0 then '--B.' + cast(ContractInvoiceBody.BagsQty AS varchar) else '' end)

+ (case when isnull(ContractInvoiceBody.PQty,0) > 0 then '--B.' + cast(ContractInvoiceBody.PQty AS varchar) else '' end)

+ (case when isnull(ContractInvoiceBody.TotalKgs,0) > 0 then '--WKg.' + cast(ContractInvoiceBody.TotalKgs AS varchar) else '' end )

+ '--WM.' + cast(Dbo.KgsToMounds(ContractInvoiceBody.TotalKgs,ContractInvoiceBody.MoundStd) AS varchar) 

+ '--R.' + cast(ContractInvoiceBody.RatePerMound AS varchar) 

+ (Case When ContractInvoiceBody.Bardana>0 THEN '--Bar.' + cast(ContractInvoiceBody.Bardana AS varchar) ELSE '' END)

+ (Case When IsNull(ContractInvoice.CommType,'')<>'' Then '--CT.' + cast(ContractInvoice.CommType AS varchar) Else '' END)

+ (Case When ContractInvoice.CommissionValue>0 THEN  '--CV.' + cast(ContractInvoice.CommissionValue AS varchar) ELSE '' END)

+ (Case When ContractInvoice.OtherExpenceMinus>0 THEN '--Exp(-).' + cast(ContractInvoice.OtherExpenceMinus AS varchar) ELSE '' END)

+ (Case When ContractInvoice.OtherExpencePlus>0 THEN '--Exp(+).' + cast(ContractInvoice.OtherExpencePlus AS varchar) ELSE '' END)

+ ',' + isnull(contractinvoice.narration,'')

    AS Remarks

FROM ContractInvoiceBody INNER JOIN

    Products ON 

    Products.ProductId = ContractInvoiceBody.ProductId INNER JOIN ContractInvoice ON 

    ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

WHERE ContractInvoiceBody.ContractPurId = @ContractSaleInvoiceId  And ContractInvoice.IsSaleInvoice=1



OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales



set @RemarksList = @RemarksList + ( Select (Case When VehicalCharges> 0 THEN ( '--VEx.'+ convert(varchar,VehicalCharges)) ELSE '' END) From ContractInvoice Where ContractInvoice.ContractPurId = @ContractSaleInvoiceId)



Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetContractSalesNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetContractSalesNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractSalesNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractSalesNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractSalesNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractSalesNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractSalesNarration];
GO

CREATE    Function FunGetContractSalesNarration(

    @ContractSaleId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor    

--By fahad at 7/10/06

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR SELECT +'P.'+ Products.ProductName + '|T.' + cast(ContractBody.TruckQty AS varchar) 

+ '|B.' + cast(ContractBody.BagsQty AS varchar) + '|W.' + cast(ContractBody.WeightPerBag AS varchar) 

+ '|R.' + cast(ContractBody.RatePerMound AS varchar) 

    AS Remarks

FROM ContractBody INNER JOIN

    Products ON 

    Products.ProductId = ContractBody.ProductId INNER JOIN Contract ON 

    ContractBody.ContractId = Contract.ContractId

WHERE ContractBody.ContractId = @ContractSaleId And Contract.IsSaleContract=1



OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)



--    BEGIN



        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end



        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks



--    End



END

CLOSE curSales

DEALLOCATE curSales



Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetContractMultiInvoiceNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetContractMultiInvoiceNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractMultiInvoiceNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractMultiInvoiceNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractMultiInvoiceNarration];
IF OBJECT_ID(N'[dbo].[FunGetContractMultiInvoiceNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetContractMultiInvoiceNarration];
GO

CREATE    Function [dbo].[FunGetContractMultiInvoiceNarration](@MultiID int)

 Returns nVarchar(3990) 

 AS 

 Begin 

 Declare @FetchRemarks nvarchar(3990) 

 Declare @RemarksList nvarchar(3990) 

 

 

 set @RemarksList = '' 

 DECLARE curSales 

 CURSOR READ_ONLY FOR  

 SELECT DISTINCT 

                   --'V.No:' + PUR.VehicleNo +

                    ', Pro:' + PUR.ProductName 

                    --+ ', PWt:' + CAST(CAST(SUM(PUR.TotalKgsPur) AS

                    --   DECIMAL(10, 2)) AS varchar) + ', SWt:' + CAST(CAST(MAX(SAL.TotalKgsSale) AS DECIMAL(10, 2)) AS varchar) 

                    --  + ', PRate:' + CAST(CAST(AVG(PUR.RatePur) AS DECIMAL(10, 2)) AS varchar) + ', SRate:' + CAST(CAST(AVG(SAL.RateSale) AS DECIMAL(10, 2)) AS varchar) 

                    --  + ', PVal:'+ CAST(CAST(SUM(PUR.TotalValueP) AS DECIMAL(10, 2)) AS varchar)

                    --   + ', SVal:' +CAST(CAST(max(SAL.TotalValueS) AS DECIMAL(10, 2)) AS varchar)

                      AS Narration

                      --, CAST(CAST(SUM(PUR.TotalValueP) AS DECIMAL(10, 2)) AS varchar)TotalValueP 

                      --, CAST(CAST(SUM(SAL.TotalValueS) AS DECIMAL(10, 2)) AS varchar)TotalValueS 

FROM         MultiInvoice AS BMI INNER JOIN

                          (SELECT     CIP.ContractPurId,  CIP.PartyID AS AccountNo,  COAP.AccountName AS PartyPur, CIBP.ProductID, Pro.ProductName, CIBP.VehicleNo, SUM(CIBP.TotalKgs) 

                                                   AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) 

                                                   * CIBP.Bardana), 0) --+ MAX(ConExp.TTLExps) 

                                                   AS TotalValueP

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId INNER JOIN

                                                   Products AS Pro ON CIBP.ProductID = Pro.ProductID INNER JOIN

                                                   ChartOfAccounts AS COAP ON CIP.PartyID = COAP.AccountNo 

                                                   --INNER JOIN

                                                   --    (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

                                                   --                             - (dbo.FunGetAllExpMinusPur(ContractInvoiceBody_1.ContractPurId) /

                                                   --                                 (SELECT     COUNT(*) AS Expr1

                                                   --                                   FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                   --                                   WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                   --                                 (SELECT     COUNT(*) AS Expr1

                                                   --                                   FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                   --                                   WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                   --                             + dbo.FunGetAllExpPlusPur(ContractInvoiceBody_1.ContractPurId) /

                                                   --                                 (SELECT     COUNT(*) AS Expr1

                                                   --                                   FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                   --                                   WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                   --                                 (SELECT     COUNT(*) AS Expr1

                                                   --                                   FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                   --                                   WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                                                   --                             AS TTLExps

                                                   --      FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                   --                             ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                   --      WHERE      (ContractInvoice_1.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                   --      GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo) AS ConExp ON 

                                                   --ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                                                   

                                                   

                            WHERE      (CIP.IsSaleInvoice = 0) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.PartyID, CIP.Received, CIP.Narration, CIP.NetValue, CIP.IsApplyPartyMarkup, CIP.BagVendorID, 

                                                   CIP.NarrationUrdu, CIP.SepVehicalCharges, CIP.AddVehicleCharges, CIP.VehicalCharges, CIP.MinusVehicleCharges, CIP.MultiEntryID, 

                                                   COAP.AccountName, CIBP.ProductID, Pro.ProductName, CIBP.VehicleNo) AS PUR ON PUR.ContractPurId = BMI.PurchaseID 

                                                   INNER JOIN

                          (

                          SELECT   DISTINCT  BMIS.MultiID, PUR.VehicleNo, (PUR.TotalKgsPur) AS TotalKgsSale, (PUR.RatePur) AS RateSale, (PUR.TotalValueP) AS TotalValueS

FROM         MultiInvoice AS BMIS INNER JOIN

                          (SELECT     CIP.ContractPurId, CIP.CDate, CIBP.VehicleNo, SUM(CIBP.TotalKgs) AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, 

                                                   ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) * CIBP.Bardana), 0) --+ MAX(ConExp.TTLExps) 

                                                   AS TotalValueP

                            FROM          ContractInvoiceBody AS CIBP 

                            INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId 

                                                   --INNER JOIN

                            --                           (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

                            --                                                    - (dbo.FunGetAllExpMinusSale(ContractInvoiceBody_1.ContractPurId) /

                            --                                                        (SELECT     COUNT(*) AS Expr1

                            --                                                          FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                            --                                                          WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                            --                                                        (SELECT     COUNT(*) AS Expr1

                            --                                                          FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                            --                                                          WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                            --                                                    + dbo.FunGetAllExpPlusSale(ContractInvoiceBody_1.ContractPurId) /

                            --                                                        (SELECT     COUNT(*) AS Expr1

                            --                                                          FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                            --                                                          WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                            --                                                        (SELECT     COUNT(*) AS Expr1

                            --                                                          FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                            --                                                          WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                            --                                                    AS TTLExps

                            --                             FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                            --                                                    ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                            --                             WHERE      (ContractInvoice_1.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                            --                             GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo) AS ConExp ON 

                            --                       ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                            WHERE      (CIP.IsSaleInvoice = 1) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.VehicalCharges, CIBP.VehicleNo) AS PUR ON PUR.ContractPurId = BMIS.SALEID) AS SAL ON SAL.MultiID = BMI.MultiID

WHERE     (BMI.MultiID = 0)--@MultiID

GROUP BY BMI.MultiID, PUR.VehicleNo, PUR.ProductName

 OPEN curSales FETCH NEXT FROM curSales INTO @FetchRemarks WHILE (@@fetch_status = 0) 

 BEGIN  

 set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end 

 set @RemarksList = @RemarksList  + @FetchRemarks  

 FETCH NEXT FROM curSales INTO @FetchRemarks 

 END 

 CLOSE curSales DEALLOCATE curSales  

 

 DECLARE @ComListP as varchar(3990)

 DECLARE @ComListS as varchar(3990)

 DECLARE @OldPurPartyName varchar(1000),    @CurPurPartyName     varchar(1000),    @CurSalPartyName     varchar(1000),    @OldSalPartyName     varchar(1000)

 Set @ComListP = ''

 Set @ComListS = ''

 Declare CurComList Cursor STATIC For 

 SELECT DISTINCT PUR.PartyPur, SAL.PartySale

FROM         MultiInvoice AS BMI INNER JOIN

                          (SELECT     CIP.ContractPurId, CIP.PartyID AS AccountNo, COAP.AccountName AS PartyPur, CIBP.VehicleNo

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId INNER JOIN

                                                   ChartOfAccounts AS COAP ON CIP.PartyID = COAP.AccountNo

                            WHERE      (CIP.IsSaleInvoice = 0) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.PartyID, COAP.AccountName, CIBP.VehicleNo) AS PUR ON PUR.ContractPurId = BMI.PurchaseID INNER JOIN

                          (SELECT     CIS.ContractPurId, COAS.AccountName AS PartySale, CIBS.VehicleNo

                            FROM          ContractInvoiceBody AS CIBS INNER JOIN

                                                   ContractInvoice AS CIS ON CIBS.ContractPurId = CIS.ContractPurId INNER JOIN

                                                   ChartOfAccounts AS COAS ON CIS.SellerId = COAS.AccountNo

                            WHERE      (CIS.IsSaleInvoice = 1) AND (ISNULL(CIS.IsMultiNew, 0) = 1)

                            GROUP BY CIS.ContractPurId, COAS.AccountName, CIBS.VehicleNo) AS SAL ON SAL.ContractPurId = BMI.SaleID AND 

                      PUR.VehicleNo = SAL.VehicleNo

WHERE     (BMI.MultiID = @MultiID) AND ISNULL(BMI.IsMultiNew,0) = 1

GROUP BY BMI.MultiID, PUR.PartyPur, SAL.PartySale

For Read Only



--    RETURN @ComListP + @ComListS

    Open CurComList



    FETCH Next FROM CurComList INTO @CurPurPartyName, @CurSalPartyName

    WHILE (@@FETCH_STATUS <> -1)



        Begin



        IF (@@FETCH_STATUS = 0)



            Begin



                if @OldPurPartyName<>@CurPurPartyName



                    Select @ComListP = @ComListP 



                   Select @ComListP = ISNULL(@ComListP, '') + ISNULL(@CurPurPartyName, '') + ', ' + char(13)



                    set @OldPurPartyName=@CurPurPartyName



            --if @OldSalPartyName<>@CurSalPartyName

            --    BEGIN

   --                 Select @ComListS = @ComListS 

   

   --                Select @ComListS = ISNULL(@ComListS, '') +  ISNULL(@CurSalPartyName, '') + char(13)

                    

   --                 set @OldSalPartyName=@CurSalPartyName

   --             END

            End



        FETCH Next FROM CurComList INTO @CurPurPartyName, @CurSalPartyName



        End

        Close CurComList



    DEALLOCATE CurComList

set @ComListP = CASE WHEN ISNULL(@ComListP, '') <> '' THEN ' P.P: ' + @ComListP ELSE '' END

set @ComListS = ''--CASE WHEN ISNULL(@ComListS, '') <> '' THEN ' S.P: ' + @ComListS ELSE '' END



--RETURN @ComListP + @ComListS

 --set @RemarksList = @RemarksList 

 set @RemarksList = @RemarksList  + char(13)+@ComListP + @ComListS 

 Return @RemarksList 

 

 end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetMultiNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetMultiNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetMultiNarration];
IF OBJECT_ID(N'[dbo].[FunGetMultiNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetMultiNarration];
IF OBJECT_ID(N'[dbo].[FunGetMultiNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetMultiNarration];
GO

CREATE    Function FunGetMultiNarration(@MultiID int, @IsSale as bit)    Returns nVarchar(3990) AS   Begin   

    Declare @FetchRemarks nVarchar(3990)    

    Declare @RemarksList nVarchar(3990) set @RemarksList = ''   DECLARE curSales CURSOR READ_ONLY  FOR 

    

    Select Products.ProductName  +':'+cast(NWeight as varchar)+'@'+ case when @IsSale = 0 then cast(GRate as varchar) else cast(MRate as varchar) end  +'. Rs.'+

    case when @IsSale = 0 then  cast(AmountP as varchar) else  cast(AmountS as varchar) end   as Remarks 

     From MultiBody INNER JOIN Products 

     ON Products.ProductId = MultiBody.ProductId Where MultiBody.MultiID=@MultiID  

     

     OPEN curSales   FETCH NEXT FROM curSales INTO @FetchRemarks WHILE (@@fetch_status = 0)  BEGIN   set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end    set @RemarksList = @RemarksList  + @FetchRemarks    FETCH NEXT FROM curSales INTO @FetchRemarks END CLOSE curSales  DEALLOCATE curSales Return @RemarksList end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetBrokrageExpNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetBrokrageExpNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetBrokrageExpNarration];
IF OBJECT_ID(N'[dbo].[FunGetBrokrageExpNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetBrokrageExpNarration];
IF OBJECT_ID(N'[dbo].[FunGetBrokrageExpNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetBrokrageExpNarration];
GO

CREATE    Function [FunGetBrokrageExpNarration](

    @ContractPurId int)

Returns Varchar(4000) AS

Begin



Declare @ReturnString varchar(4000)

Declare @PartyName varchar(200)

SELECT     @PartyName = Parties.PartyName  FROM         ContractInvoice INNER JOIN    Parties ON ContractInvoice.PartyID = Parties.PartyID  WHERE     (ContractInvoice.ContractPurId = @ContractPurId)

set @ReturnString = 'Inv#.' + convert(varchar,@ContractPurId) + ', Party:' + @PartyName

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================





Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)



set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR  SELECT   DISTINCT   Products.ProductName  FROM         Products INNER JOIN   ContractInvoiceBody ON Products.ProductID = ContractInvoiceBody.ProductID  WHERE     (ContractInvoiceBody.ContractPurId = @ContractPurId)

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END



CLOSE curSales



DEALLOCATE curSales



set @ReturnString = @ReturnString + ', Products:' +  @RemarksList 



Return @ReturnString

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetAllExpPur]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetAllExpPur]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPur];
IF OBJECT_ID(N'[dbo].[FunGetAllExpPur]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPur];
IF OBJECT_ID(N'[dbo].[FunGetAllExpPur]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPur];
GO

CREATE FUNCTION dbo.FunGetAllExpPur(@InvoiceId numeric (12))

RETURNS Numeric (12) AS  

BEGIN 



Declare @Value Numeric (12)

Select @Value = 

  IsNull(Case When (C.IsPay = 0) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.AddArhat,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN C.MazdoriPerbag = 1 then (case when CB.WeightPerBag <> 0 then (Sum(CB.TotalKgs)/CB.WeightPerBag) * C.PurMazdori 

else

(SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori  end) Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddVehicleCharges,0) = 1) Then C.VehicalCharges End,0)        

+ IsNull(Case When (Isnull(C.AddMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.AddBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpencePlus,0)

--Sales

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then 

(CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddSootli,0) = 1) Then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case  When (Isnull(C.AddWHTax,0) = 1) Then (Case WHEN Isnull(C.WHTinKg,0)=1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax Else SUM(CB.TotalValue) * C.WHTax / 100 END)  End,0)

+ (CASE WHEN ISNULL(C.ChartsCharges,0) > 0 THEN  ABS(C.ChartsCharges) ELSE 0 END)

--MINUSES

-(IsNull(C.Discount,0)

+  IsNull(Case When (C.IsPay = 1) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.MinusArhat,0) = 1) Then IsNull((SUM(CB.TotalValue))  * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then 

(CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100

 WHEN C.MazdoriPerbag = 1 then (case when CB.WeightPerBag <> 0 then (Sum(CB.TotalKgs)/CB.WeightPerBag) * C.PurMazdori 

else

(SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori  end) Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

 + IsNull(Case When (Isnull(C.MinusVehicleCharges,0) = 1) Then (C.VehicalCharges) End,0)        

+ IsNull(Case When (Isnull(C.MinusMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.MinusBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpenceMinus,0)

--Sales

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then

 (CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusSootli,0) = 1) Then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case When (Isnull(C.MinusWHTax,0) = 1) Then ( C.CommissionValue * C.WHTax / 100 )  End,0)

+ (CASE WHEN ISNULL(C.ChartsCharges,0) < 0 THEN  ABS(C.ChartsCharges) ELSE 0 END))

From ContractInvoice C INNER JOIN ContractInvoiceBody CB ON C.ContractPurId = CB.ContractPurId 

Where C.ContractPurId = @InvoiceId And C.IsSaleInvoice = 0

Group By C.IsSaleInvoice, C.IsPay, C.AddArhat, C.PurArhat,C.AddChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.AddMazdori, C.AddDalali, C.PurDalali, C.AddPuliGhya, C.PurPulliGhya, 

C.PurComm, C.AddCommission, C.SaleManshyana, C.AddManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.AddLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.AddUnLoading,

C.VehicalCharges, C.AddVehicleCharges, C.SaleMarketFees, C.AddMarketFees, C.OtherExpencePlus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.AddSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.AddWHTax,C.WHTinKg, C.MazdoriPercent,C.AddBankCharges,C.BankChargesAmount, C.CommissionValue

,CB.WeightPerBag,C.ChartsCharges





, C.MinusArhat, C.PurArhat,C.MinusChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.MinusMazdori, C.MinusDalali, C.PurDalali, C.MinusPuliGhya, C.PurPulliGhya, 

C.PurComm, C.MinusCommission, C.SaleManshyana, C.MinusManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.MinusLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.MinusUnLoading,

C.VehicalCharges, C.MinusVehicleCharges, C.SaleMarketFees, C.MinusMarketFees, C.OtherExpenceMinus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.MinusSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.MinusWHTax,C.WHTinKg, C.MazdoriPercent,C.MinusBankCharges,C.BankChargesAmount, C.CommissionValue, C.Discount

,CB.WeightPerBag, C.ChartsCharges



Return IsNull(@Value,0)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetAllExpSale]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetAllExpSale]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpSale];
IF OBJECT_ID(N'[dbo].[FunGetAllExpSale]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpSale];
IF OBJECT_ID(N'[dbo].[FunGetAllExpSale]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpSale];
GO

CREATE FUNCTION [dbo].FunGetAllExpSale(@InvoiceId numeric (12))

RETURNS Numeric (12) AS  

BEGIN 



Declare @Value Numeric (12)



Select @Value = 



  IsNull(Case When (C.IsPay = 0) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.AddArhat,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddVehicleCharges,0) = 1) Then (C.VehicalCharges) End,0)        

+ IsNull(Case When (Isnull(C.AddBankCharges,0) = 1) Then  C.BankChargesAmount End,0) 

+ IsNull(Case When (Isnull(C.AddTelePhoneExp,0) = 1) Then Sum(C.AddTelePhoneExp) End,0)        

+ IsNull(Case When (Isnull(C.AddInsuranceExp,0) = 1) Then Sum(C.AddInsuranceExp) End,0)        

+ IsNull(Case When (Isnull(C.AddFuelExp,0) = 1) Then Sum(C.AddFuelExp) End,0)        

+ IsNull(Case When (Isnull(C.AddStitchingExp,0) = 1) Then Sum(C.AddStitchingExp) End,0)        

+ IsNull(Case When (Isnull(C.AddMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(C.OtherExpencePlus,0)



+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddSootli,0) = 1) Then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddWHTax,0) = 1) Then 

(Case WHEN Isnull(C.WHTinKg,0)=1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax Else SUM(CB.TotalValue) * C.WHTax / 100 END)  End,0)

--MINUS

-(IsNull(C.Discount,0)

+  IsNull(Case When (C.IsPay = 1) Then C.CommissionValue End,0)

+ IsNull(Case When (Isnull(C.MinusArhat,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100

WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)



+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusVehicleCharges,0) = 1) Then (C.VehicalCharges) End,0)        

+ IsNull(Case When (Isnull(C.MinusMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.MinusBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpenceMinus,0)

--Sales

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100 WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusSootli,0) = 1) Then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case  When (Isnull(C.MinusWHTax,0) = 1) Then (Case WHEN Isnull(C.WHTinKg,0)=1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax Else SUM(CB.TotalValue) * C.WHTax / 100 END)  End,0))

From ContractInvoice C INNER JOIN ContractInvoiceBody CB ON C.ContractPurId = CB.ContractPurId 

Where C.ContractPurId = @InvoiceId And C.IsSaleInvoice = 1

Group By C.IsSaleInvoice, C.IsPay, C.AddArhat, C.PurArhat, C.AddChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.AddMazdori, C.AddDalali, C.PurDalali, C.AddPuliGhya, C.PurPulliGhya, 

C.PurComm, C.AddCommission, C.SaleManshyana, C.AddManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.AddLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.AddUnLoading,

C.VehicalCharges, C.AddVehicleCharges,C.AddTelePhoneExp,C.AddInsuranceExp,C.AddFuelExp,C.AddStitchingExp, C.SaleMarketFees, C.AddMarketFees, C.OtherExpencePlus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.AddSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.AddWHTax,C.CommissionValue,C.WHTinKg, C.MazdoriPercent,C.AddBankCharges,C.BankChargesAmount

, C.MinusArhat, C.PurArhat,C.MinusChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.MinusMazdori, C.MinusDalali, C.PurDalali, C.MinusPuliGhya, C.PurPulliGhya, 

C.PurComm, C.MinusCommission, C.SaleManshyana, C.MinusManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.MinusLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.MinusUnLoading,

C.VehicalCharges, C.MinusVehicleCharges, C.SaleMarketFees, C.MinusMarketFees, C.OtherExpenceMinus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.MinusSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.MinusWHTax,C.CommissionValue,C.WHTinKg, C.MazdoriPercent,C.MinusBankCharges,C.BankChargesAmount, C.Discount



Return IsNull(@Value,0)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetAllExpPlusPur]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetAllExpPlusPur]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPlusPur];
IF OBJECT_ID(N'[dbo].[FunGetAllExpPlusPur]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPlusPur];
IF OBJECT_ID(N'[dbo].[FunGetAllExpPlusPur]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPlusPur];
GO

CREATE FUNCTION dbo.FunGetAllExpPlusPur(@InvoiceId numeric (12))

RETURNS Numeric (12) AS  

BEGIN 



Declare @Value Numeric (12)

Select @Value = 

  IsNull(Case When (C.IsPay = 0) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.AddArhat,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN C.MazdoriPerbag = 1 then (case when CB.WeightPerBag <> 0 then (Sum(CB.TotalKgs)/CB.WeightPerBag) * C.PurMazdori 

else

(SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori  end) Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddVehicleCharges,0) = 1) Then C.VehicalCharges End,0)        

+ IsNull(Case When (Isnull(C.AddMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.AddBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpencePlus,0)

--Sales

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then 

(CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddSootli,0) = 1) Then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case  When (Isnull(C.AddWHTax,0) = 1) Then (Case WHEN Isnull(C.WHTinKg,0)=1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax Else SUM(CB.TotalValue) * C.WHTax / 100 END)  End,0)

+ (CASE WHEN ISNULL(C.ChartsCharges,0) > 0 THEN  ABS(C.ChartsCharges) ELSE 0 END)

From ContractInvoice C INNER JOIN ContractInvoiceBody CB ON C.ContractPurId = CB.ContractPurId Where C.ContractPurId = @InvoiceId And C.IsSaleInvoice = 0

Group By C.IsSaleInvoice, C.IsPay, C.AddArhat, C.PurArhat,C.AddChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.AddMazdori, C.AddDalali, C.PurDalali, C.AddPuliGhya, C.PurPulliGhya, 

C.PurComm, C.AddCommission, C.SaleManshyana, C.AddManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.AddLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.AddUnLoading,

C.VehicalCharges, C.AddVehicleCharges, C.SaleMarketFees, C.AddMarketFees, C.OtherExpencePlus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.AddSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.AddWHTax,C.WHTinKg, C.MazdoriPercent,C.AddBankCharges,C.BankChargesAmount, C.CommissionValue

,CB.WeightPerBag,C.ChartsCharges

Return IsNull(@Value,0)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetAllExpPlusSale]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetAllExpPlusSale]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPlusSale];
IF OBJECT_ID(N'[dbo].[FunGetAllExpPlusSale]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPlusSale];
IF OBJECT_ID(N'[dbo].[FunGetAllExpPlusSale]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpPlusSale];
GO

CREATE FUNCTION [FunGetAllExpPlusSale](@InvoiceId numeric (12))

RETURNS Numeric (12) AS  

BEGIN 



Declare @Value Numeric (12)



Select @Value = 



  IsNull(Case When (C.IsPay = 0) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.AddArhat,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddVehicleCharges,0) = 1) Then (C.VehicalCharges) End,0)        

+ IsNull(Case When (Isnull(C.AddBankCharges,0) = 1) Then  C.BankChargesAmount End,0) 

+ IsNull(Case When (Isnull(C.AddTelePhoneExp,0) = 1) Then Sum(C.AddTelePhoneExp) End,0)        

+ IsNull(Case When (Isnull(C.AddInsuranceExp,0) = 1) Then Sum(C.AddInsuranceExp) End,0)        

+ IsNull(Case When (Isnull(C.AddFuelExp,0) = 1) Then Sum(C.AddFuelExp) End,0)        

+ IsNull(Case When (Isnull(C.AddStitchingExp,0) = 1) Then Sum(C.AddStitchingExp) End,0)        

+ IsNull(Case When (Isnull(C.AddMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(C.OtherExpencePlus,0)



+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddSootli,0) = 1) Then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddWHTax,0) = 1) Then 

(Case WHEN Isnull(C.WHTinKg,0)=1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax Else SUM(CB.TotalValue) * C.WHTax / 100 END)  End,0)

From ContractInvoice C INNER JOIN ContractInvoiceBody CB ON C.ContractPurId = CB.ContractPurId 

Where C.ContractPurId = @InvoiceId And C.IsSaleInvoice = 1

Group By C.IsSaleInvoice, C.IsPay, C.AddArhat, C.PurArhat, C.AddChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.AddMazdori, C.AddDalali, C.PurDalali, C.AddPuliGhya, C.PurPulliGhya, 

C.PurComm, C.AddCommission, C.SaleManshyana, C.AddManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.AddLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.AddUnLoading,

C.VehicalCharges, C.AddVehicleCharges,C.AddTelePhoneExp,C.AddInsuranceExp,C.AddFuelExp,C.AddStitchingExp, C.SaleMarketFees, C.AddMarketFees, C.OtherExpencePlus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.AddSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.AddWHTax,C.CommissionValue,C.WHTinKg, C.MazdoriPercent,C.AddBankCharges,C.BankChargesAmount





Return IsNull(@Value,0)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetAllExpMinusPur]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetAllExpMinusPur]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpMinusPur];
IF OBJECT_ID(N'[dbo].[FunGetAllExpMinusPur]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpMinusPur];
IF OBJECT_ID(N'[dbo].[FunGetAllExpMinusPur]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpMinusPur];
GO

CREATE FUNCTION dbo.FunGetAllExpMinusPur(@InvoiceId numeric (12))

RETURNS Numeric (12) AS  

BEGIN 



Declare @Value Numeric (12)

Select @Value = 

IsNull(C.Discount,0)

+  IsNull(Case When (C.IsPay = 1) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.MinusArhat,0) = 1) Then IsNull((SUM(CB.TotalValue))  * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then 

(CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100

 WHEN C.MazdoriPerbag = 1 then (case when CB.WeightPerBag <> 0 then (Sum(CB.TotalKgs)/CB.WeightPerBag) * C.PurMazdori 

else

(SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori  end) Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

 + IsNull(Case When (Isnull(C.MinusVehicleCharges,0) = 1) Then (C.VehicalCharges) End,0)        

+ IsNull(Case When (Isnull(C.MinusMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.MinusBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpenceMinus,0)

--Sales

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then

 (CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusSootli,0) = 1) Then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case When (Isnull(C.MinusWHTax,0) = 1) Then ( C.CommissionValue * C.WHTax / 100 )  End,0)

+ (CASE WHEN ISNULL(C.ChartsCharges,0) < 0 THEN  ABS(C.ChartsCharges) ELSE 0 END)

From ContractInvoice C INNER JOIN ContractInvoiceBody CB ON C.ContractPurId = CB.ContractPurId Where C.ContractPurId = @InvoiceId And C.IsSaleInvoice = 0

Group By C.IsSaleInvoice, C.IsPay, C.MinusArhat, C.PurArhat,C.MinusChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.MinusMazdori, C.MinusDalali, C.PurDalali, C.MinusPuliGhya, C.PurPulliGhya, 

C.PurComm, C.MinusCommission, C.SaleManshyana, C.MinusManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.MinusLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.MinusUnLoading,

C.VehicalCharges, C.MinusVehicleCharges, C.SaleMarketFees, C.MinusMarketFees, C.OtherExpenceMinus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.MinusSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.MinusWHTax,C.WHTinKg, C.MazdoriPercent,C.MinusBankCharges,C.BankChargesAmount, C.CommissionValue, C.Discount

,CB.WeightPerBag, C.ChartsCharges

Return IsNull(@Value,0)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetAllExpMinusSale]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetAllExpMinusSale]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpMinusSale];
IF OBJECT_ID(N'[dbo].[FunGetAllExpMinusSale]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpMinusSale];
IF OBJECT_ID(N'[dbo].[FunGetAllExpMinusSale]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetAllExpMinusSale];
GO

CREATE FUNCTION dbo.FunGetAllExpMinusSale(@InvoiceId numeric (12))

RETURNS Numeric (12) AS  

BEGIN 



Declare @Value Numeric (12)

Select @Value = 

IsNull(C.Discount,0)

+  IsNull(Case When (C.IsPay = 1) Then C.CommissionValue End,0)

+ IsNull(Case When (Isnull(C.MinusArhat,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100

WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)



+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusVehicleCharges,0) = 1) Then (C.VehicalCharges) End,0)        

+ IsNull(Case When (Isnull(C.MinusMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.MinusBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpenceMinus,0)

--Sales

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100 WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusSootli,0) = 1) Then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case  When (Isnull(C.MinusWHTax,0) = 1) Then (Case WHEN Isnull(C.WHTinKg,0)=1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax Else SUM(CB.TotalValue) * C.WHTax / 100 END)  End,0)



From ContractInvoice C INNER JOIN ContractInvoiceBody CB ON C.ContractPurId = CB.ContractPurId 

Where C.ContractPurId = @InvoiceId And C.IsSaleInvoice = 1

Group By C.IsSaleInvoice, C.IsPay, C.MinusArhat, C.PurArhat,C.MinusChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.MinusMazdori, C.MinusDalali, C.PurDalali, C.MinusPuliGhya, C.PurPulliGhya, 

C.PurComm, C.MinusCommission, C.SaleManshyana, C.MinusManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.MinusLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.MinusUnLoading,

C.VehicalCharges, C.MinusVehicleCharges, C.SaleMarketFees, C.MinusMarketFees, C.OtherExpenceMinus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.MinusSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.MinusWHTax,C.CommissionValue,C.WHTinKg, C.MazdoriPercent,C.MinusBankCharges,C.BankChargesAmount, C.Discount



Return IsNull(@Value,0)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunPhrCurrentDebit]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunPhrCurrentDebit]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunPhrCurrentDebit];
IF OBJECT_ID(N'[dbo].[FunPhrCurrentDebit]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunPhrCurrentDebit];
IF OBJECT_ID(N'[dbo].[FunPhrCurrentDebit]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunPhrCurrentDebit];
GO

CREATE    Function [dbo].[FunPhrCurrentDebit] (@AccNo as varchar(10),@FromDate as smalldatetime) returns decimal(20,2)

AS

BEGIN

Declare @RetData as decimal(20,0);

SELECT   @RetData =  IsNull(Sum(Debit),0) - IsNull(SUm(Credit),0)

FROM         (



SELECT     OpeningDebit DEBIT, OpeningCredit CREDIT

FROM         ChartOfAccounts

WHERE AccountNo = @AccNo

UNION ALL

SELECT     ISNULL(SUM(OpeningStock.TTLValue),0) AS Debit, 0 AS Credit

FROM         OpeningStock INNER JOIN

                      Products ON OpeningStock.ProductID = Products.ProductID

WHERE     (ISNULL(Products.PurAcNo, '') = @AccNo)

UNION ALL

SELECT 0           AS DEBIT,SUM(CREDIT) AS CREDIT    FROM CREDITVOUCHERSBODY    WHERE (VOUCHERDATE <= @FROMDATE)        AND AccountNo = @AccNo

UNION ALL

         (SELECT SUM(DEBIT)  AS DEBIT,0           AS CREDIT    FROM DEBITVOUCHERSBODY     WHERE (VOUCHERDATE <= @FROMDATE)    And AccountNo = @AccNo)



    

        /* -------------- JOURNAL VOUCHERS --------------- */                                                                                                    

        UNION ALL

         (SELECT SUM(DEBIT)  AS DEBIT,SUM(CREDIT) AS CREDIT    FROM JOURNALVOUCHERSBODY WHERE (VOUCHERDATE <= @FROMDATE)    And AccountNo = @AccNo)



        /* -------------- BANK CHEQUES     --------------- */                                                                                                    

        UNION ALL

        

SELECT     sum(ISNULL(BankChequesBody.Amount, BankCheques.Amount)) AS DEBIT, 0 AS CREDIT

FROM         BankCheques LEFT OUTER JOIN

                      BankChequesBody ON BankCheques.VoucherNo = BankChequesBody.VoucherNo AND 

                      BankCheques.VoucherDate = BankChequesBody.VoucherDate

WHERE     (BankCheques.IsPosted = 1) AND (BankCheques.IsLost = 0)

and ISNULL(BankChequesBody.AccountNo, BankCheques.AccountNo) = @AccNo and BankCheques.VoucherDate <= @FROMDATE

        /* -------------- BANK Deposits    --------------- */                                                                                                                

        UNION ALL

         

SELECT     0 AS Debit, SUM(Amount) AS CREDIT

FROM         BankDeposit

WHERE     (IsPosted = 1) AND (IsBounced = 0) AND (ISNULL(IsCheque, 0) = 1) AND (AccountNo = @AccNo) AND (VoucherDate <= @FROMDATE)



union all

/*********** INVENTORY PURCHASE NET AMOUNT Credit, 0 AS IsApplyPartyMarkup TO VENDOR ACCOUNT *************/

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     Purchase.PaidAmount AS Debit, (SUM(PurchaseBody.TTLValue + PurchaseBody.TTLSalesTax) - ISNULL(Purchase.ExpMinus, 0) 

                                              + ISNULL(Purchase.ExpPlus, 0) + (CASE WHEN ISNULL(Purchase.IsFreightPlus, 0) = 1 THEN ISNULL(Purchase.Freight, 0) 

                                              ELSE - 1 * ISNULL(Purchase.Freight, 0) END)) + SUM(PurchaseBody.TTLValue + PurchaseBody.TTLSalesTax) * ISNULL(Purchase.ComRatio, 

                                              0) / 100 AS Credit

                       FROM          PurchaseBody INNER JOIN

                                              Purchase ON PurchaseBody.PurchaseID = Purchase.PurchaseID

                       WHERE      (Purchase.IsPosted = 1) AND (Purchase.PurchaseDate <= @FROMDATE) AND (Purchase.VendorID = @AccNo)

                       GROUP BY Purchase.PurchaseID, Purchase.PaidAmount, Purchase.ExpMinus, Purchase.IsFreightPlus, Purchase.Freight, Purchase.ComRatio, 

                                              Purchase.ExpPlus) AS IP

union all                                              

/*********** PRODUCT PURCHASERETURN NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     (SUM(PurReturnsBody.TTLValue + PurReturnsBody.TTLSalesTax) - ISNULL(PurReturns.ExpMinus, 0) + ISNULL(PurReturns.ExpPlus, 0)) 

                                              + SUM(PurReturnsBody.TTLValue + PurReturnsBody.TTLSalesTax) * ISNULL(PurReturns.ComRatio, 0) / 100 AS Debit, 

                                              PurReturns.ReceivedAmount AS Credit

                       FROM          PurReturnsBody INNER JOIN

                                              PurReturns ON PurReturnsBody.PurReturnID = PurReturns.PurReturnID

                       WHERE      (PurReturns.IsPosted = 1) AND (PurReturns.PurReturnDate <= @FROMDATE) AND (PurReturns.VendorID = @AccNo)

                       GROUP BY PurReturns.ComRatio, PurReturns.ExpPlus, PurReturns.ExpMinus, PurReturns.VendorID, PurReturns.ReceivedAmount, 

                                              PurReturns.NarrationUrdu,PurReturns.PurReturnID) AS IR

UNION ALL

/*********** PRODUCT Sales NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     CASE WHEN isnull(Sales.CustomerIDCom, 0) = 0 THEN SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) - ISNULL(Sales.ExpMinus, 0) 

                                              + ISNULL(Sales.ExpPlus, 0) + (SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio, 0) / 100) 

                                              ELSE (SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) - ISNULL(Sales.ExpMinus, 0) + ISNULL(Sales.ExpPlus, 0) 

                                              + (SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio, 0) / 100)) * (100 - Sales.CustomerComRatio) 

                                              / 100 END + (CASE WHEN ISNULL(Sales.IsFreightPlus, 0) = 1 THEN ISNULL(Sales.Freight, 0) ELSE - 1 * ISNULL(Sales.Freight, 0) END) 

                                              AS Debit, Sales.ReceivedAmount + Sales.SpecialDiscount + SUM(ISNULL(MI.VL, 0)) AS Credit

                       FROM          SalesBody INNER JOIN

                                              Sales ON SalesBody.SaleID = Sales.SaleID LEFT OUTER JOIN

                                                  (SELECT     MultiBody.SaleID, MultiBody.ProductID, SUM(ISNULL(MultiBody.TPTEx, 0) + ISNULL(MultiBody.TPTM, 0)) AS VL

                                                    FROM          MultiBody INNER JOIN

                                                                           Sales ON MultiBody.SaleID = Sales.SaleID

                                                    WHERE      (1 = 1) AND (ISNULL(MultiBody.IsVL, 0) = 0) AND (ISNULL(MultiBody.IsVLG, 0) = 0)

                                                    GROUP BY MultiBody.SaleID, MultiBody.ProductID

                                                    HAVING      (SUM(ISNULL(MultiBody.TPTEx, 0) + ISNULL(MultiBody.TPTM, 0)) > 0)) AS MI ON SalesBody.ProductID = MI.ProductID AND 

                                              MI.SaleID = Sales.SaleID

                       WHERE      (Sales.IsPosted = 1) AND (Sales.SaleDate <= @FROMDATE) AND (Sales.CustomerID = @AccNo)

                       GROUP BY Sales.SaleID, Sales.CustomerIDCom, Sales.CustomerComRatio, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, 

                                              Sales.ExpPlus, Sales.ExpMinus, Sales.CustomerID, Sales.ReceivedAmount, Sales.SpecialDiscount, Sales.IsApplyPartyMarkup, 

                                              Sales.NarrationUrdu, Sales.IsFreightPlus, Sales.Freight, Sales.VehicleNo) AS SI                                              

UNION ALL

/*********** PRODUCT SaleReturns NET AMOUNT Credit, 0 AS IsApplyPartyMarkup TO VENDOR ACCOUNT *************/

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     SaleReturns.PaidAmount + SaleReturns.SpecialDiscount AS Debit, (SUM(SaleReturnsBody.TTLValue + SaleReturnsBody.TTLSalesTax) 

                                              - ISNULL(SaleReturns.ExpMinus, 0) + ISNULL(SaleReturns.ExpPlus, 0)) 

                                              + SUM(SaleReturnsBody.TTLValue + SaleReturnsBody.TTLSalesTax) * ISNULL(SaleReturns.ComRatio, 0) / 100 AS Credit, 

                                              0 AS IsApplyPartyMarkup

                       FROM          SaleReturnsBody INNER JOIN

                                              SaleReturns ON SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID

                       WHERE      (SaleReturns.IsPosted = 1) AND (SaleReturns.SaleReturnDate <= @FROMDATE) AND (SaleReturns.CustomerID = @AccNo)

                       GROUP BY SaleReturns.ComRatio, SaleReturns.CustomerID, SaleReturns.PaidAmount, SaleReturns.ExpMinus, SaleReturns.ExpPlus, 

                                              SaleReturns.SpecialDiscount, SaleReturns.NarrationUrdu,SaleReturns.SaleReturnID) AS SR                                              

UNION ALL

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     ContractInvoice.Received AS Debit, 

                                              ROUND(ISNULL(ContractInvoice.NetValue - (CASE WHEN dbo.RegistryValue('StopApplyGrossToPartyAccount') = 1 AND 

                                              isnull(ContractInvoice.BagVendorID, '') = '' THEN SUM(ContractInvoiceBody.TotalValue) 

                                              + SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) WHEN ISNULL(ContractInvoice.BagVendorID, '') 

                                              <> '' THEN SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) ELSE 0 END) + ContractInvoice.Received, 0), 0) 

                                              + ISNULL((CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges, 0) 

                                              = 1 THEN (CASE WHEN ContractInvoice.AddVehicleCharges = 1 THEN - VehicalCharges WHEN MinusVehicleCharges = 1 THEN VehicalCharges

                                               END) ELSE 0 END), 0) AS Credit

                       FROM          ContractInvoiceBody INNER JOIN

                                              ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 0) AND (ContractInvoice.CDate <= @FROMDATE) AND (ContractInvoice.PartyID = @AccNo)

                       GROUP BY ContractInvoice.PartyID, ContractInvoice.Received, ContractInvoice.NetValue, ContractInvoice.BagVendorID, 

                                              ContractInvoice.SepVehicalCharges, ContractInvoice.AddVehicleCharges, ContractInvoice.VehicalCharges, 

                                              ContractInvoice.MinusVehicleCharges,ContractInvoice.ContractPurId) AS CPI                                              

UNION ALL

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     (CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges, 0) 

                                              = 1 THEN (CASE WHEN ContractInvoice.MinusVehicleCharges = 1 THEN VehicalCharges ELSE 0 END) ELSE 0 END) AS Debit, 

                                              (CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges, 0) 

                                              = 1 THEN (CASE WHEN ContractInvoice.AddVehicleCharges = 1 THEN VehicalCharges ELSE 0 END) ELSE 0 END) AS Credit

                       FROM          ContractInvoiceBody INNER JOIN

                                              ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.SepVehicalCharges, 0) = 1) AND (ContractInvoice.CDate <= @FROMDATE) AND 

                                              (ContractInvoice.PartyID = @AccNo)

                       GROUP BY ContractInvoice.PartyID, ContractInvoice.Received, ContractInvoice.NetValue, ContractInvoice.SepVehicalCharges, 

                                            ContractInvoice.ContractPurId,  ContractInvoice.AddVehicleCharges, ContractInvoice.VehicalCharges, ContractInvoice.MinusVehicleCharges) AS CPIV

UNION ALL

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     ROUND(ISNULL(ContractInvoice.NetValue - (CASE WHEN dbo.RegistryValue('StopApplyGrossToPartyAccount') 

                                              = 1 THEN SUM(ContractInvoiceBody.TotalValue) + SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) ELSE 0 END), 0) 

                                              + ContractInvoice.Received, 0) + ISNULL((CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges, 0) 

                                              = 1 THEN (CASE WHEN ContractInvoice.AddVehicleCharges = 1 THEN - VehicalCharges WHEN MinusVehicleCharges = 1 THEN VehicalCharges

                                               END) ELSE 0 END), 0) AS Debit, ContractInvoice.Received AS Credit

                       FROM          ContractInvoiceBody INNER JOIN

                                              ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 1) AND (ContractInvoice.CDate <= @FROMDATE) AND (ISNULL(ContractInvoice.SellerId, ContractInvoice.PartyID) = @AccNo)

                       GROUP BY ContractInvoice.SellerId, ContractInvoice.Received, ContractInvoice.NetValue, ContractInvoice.PartyID, ContractInvoice.SepVehicalCharges, 

                                              ContractInvoice.ContractPurId,ContractInvoice.AddVehicleCharges, ContractInvoice.VehicalCharges, ContractInvoice.MinusVehicleCharges) AS CSI                                              

UNION ALL

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     (CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges, 0) 

                                              = 1 THEN (CASE WHEN ContractInvoice.AddVehicleCharges = 1 THEN VehicalCharges ELSE 0 END) ELSE 0 END) AS Debit, 

                                              (CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges, 0) 

                                              = 1 THEN (CASE WHEN ContractInvoice.MinusVehicleCharges = 1 THEN VehicalCharges ELSE 0 END) ELSE 0 END) AS Credit

                       FROM          ContractInvoiceBody INNER JOIN

                                              ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.SepVehicalCharges, 0) = 1) AND (ContractInvoice.CDate <= @FROMDATE) AND 

                                              (ISNULL(ContractInvoice.SellerId, ContractInvoice.PartyID) = @AccNo)

                       GROUP BY ContractInvoice.SellerId, ContractInvoice.Received, ContractInvoice.NetValue, ContractInvoice.PartyID, ContractInvoice.SepVehicalCharges, 

                                             ContractInvoice.ContractPurId, ContractInvoice.AddVehicleCharges, ContractInvoice.VehicalCharges, ContractInvoice.MinusVehicleCharges) AS CSIV                                              

UNION ALL



SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (

SELECT   Purchase.PaidAmount + SUM(ISNULL(vl,0)) AS Debit, SUM(MI.AmountP)- ISNULL(Purchase.ExpMinus, 0) + ISNULL(Purchase.ExpPlus, 0) + (SUM(MI.AmountP) * IsNull(Purchase.ComRatio,0) / 100)AS Credit

FROM Purchase INNER JOIN ((SELECT     MultiBody.PurchaseID, CASE WHEN ISNULL(MultiBody.IsVLG, 0) = 1 THEN SUM(ISNULL(MultiBody.TPTEx, 0)) 

ELSE 0 END + CASE WHEN SUM(ISNULL(MultiBody.TPTM, 0)) > 0 THEN SUM(ISNULL(MultiBody.TPTM, 0)) ELSE 0 END AS VL, 

SUM(MultiBody.AmountP) AS AmountP, MultiBody.MDate, MultiBody.MultiID, MultiBody.IsVLG

FROM         MultiBody INNER JOIN

Purchase ON MultiBody.PurchaseID = Purchase.PurchaseID

WHERE     (1 = 1)  

GROUP BY MultiBody.IsVLG, MultiBody.PurchaseID, MultiBody.MDate, MultiBody.MultiID)) MI ON MI.PurchaseID = Purchase.PurchaseID 

WHERE     Purchase.IsPosted = 1 AND MDate <= @FROMDATE AND Purchase.VendorID  = @AccNo

GROUP BY Purchase.ComRatio, Purchase.VendorID, Purchase.PaidAmount, Purchase.ExpMinus, Purchase.ExpPlus

)MIP                                              

UNION ALL

SELECT     SUM(Debit) AS Debit, SUM(Credit) AS Credit

FROM         (SELECT     (SUM(MI.AmountS) - ISNULL(Sales.ExpMinus, 0) + ISNULL(Sales.ExpPlus, 0)) + SUM(MI.AmountS) * ISNULL(Sales.ComRatio, 0) 

                                              / 100 AS Debit, Sales.ReceivedAmount + Sales.SpecialDiscount + SUM(ISNULL(MI.VL, 0)) AS Credit

                       FROM          Sales INNER JOIN

                                                  (SELECT     MultiBody.SaleID, CASE WHEN (ISNULL(MultiBody.IsVL, 0) = 0 AND ISNULL(MultiBody.IsVLG, 0) = 0) 

                                                                           THEN SUM(ISNULL(MultiBody.TPTEx, 0) + ISNULL(MultiBody.TPTM, 0)) ELSE 0 END AS VL, MultiBody.MultiID, 

                                                                           MultiBody.MDate, SUM(MultiBody.AmountS) AS AmountS

                                                    FROM          MultiBody INNER JOIN

                                                                           Sales ON MultiBody.SaleID = Sales.SaleID

                                                    WHERE      (1 = 1)

                                                    GROUP BY MultiBody.SaleID, MultiBody.MultiID, MultiBody.MDate, MultiBody.IsVL, MultiBody.IsVLG) AS MI ON 

                                              MI.SaleID = Sales.SaleID

                       WHERE      (Sales.IsPosted = 1) AND (MI.MDate <= @FROMDATE) AND (Sales.CustomerID = @AccNo)

                       GROUP BY MI.MultiID, MI.MDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, Sales.ExpPlus, Sales.ExpMinus, Sales.CustomerID, 

                                              Sales.ReceivedAmount, Sales.SpecialDiscount, Sales.IsApplyPartyMarkup, Sales.NarrationUrdu) AS MIS

UNION ALL

SELECT     SUM(DEBIT) AS Debit, SUM(CREDIT) AS Credit

FROM         (SELECT     P.PaidAmount AS DEBIT, ROUND(PB.TTLValue + PB.TTLSalestax, 0) AS CREDIT

                       FROM          PurchasesPest AS P INNER JOIN

                                                  (SELECT     PurchaseId, SUM(TTLValue) AS TTLValue, SUM(TTLSalestax) AS TTLSalestax

                                                    FROM          PurchasesBodyPest

                                                    GROUP BY PurchaseId) AS PB ON P.PurchaseId = PB.PurchaseId

                       WHERE      (1 = 1) AND (P.EntryDate <= @FROMDATE) AND (P.VendorId = @AccNo)) AS PPI                                              

UNION ALL

SELECT     SUM(DEBIT) AS Debit, SUM(CREDIT) AS Credit

FROM         (SELECT     PRB.TTLValue + PRB.TTLSalestax AS DEBIT, PR.ReceivedAmount AS CREDIT

                       FROM          PurReturnsPest AS PR INNER JOIN

                                                  (SELECT     PReturnId, SUM(TTLValue) AS TTLValue, SUM(TTLSalestax) AS TTLSalestax

                                                    FROM          PurReturnsBodyPest

                                                    GROUP BY PReturnId) AS PRB ON PR.PReturnId = PRB.PReturnId

                       WHERE      (1 = 1) AND (PR.ReturnDate <= @FROMDATE) AND (PR.VendorId = @AccNo)) AS PPR                       

                       

UNION ALL

/*********** PRODUCT Sales Pest NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/

SELECT     SUM(DEBIT) AS Debit, SUM(CREDIT) AS Credit

FROM         (

SELECT round(TTLValue+TTLSalestax-SpecialDiscount,0) AS DEBIT, ReceivedAmount AS CREDIT

FROM SalesPest S INNER JOIN (SELECT SaleId, Sum(TTLValue) TTLValue,  Sum(TTLSalestax) TTLSalestax FROM SalesBodyPest GROUP BY SaleId) SB ON S.SaleId = SB.SaleId

WHERE      (1 = 1) AND (S.SaleDate <= @FROMDATE) AND (S.CustomerId = @AccNo)      

)PSI

UNION ALL

/*********** PRODUCT SaleReturns Pest NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/

SELECT     SUM(DEBIT) AS Debit, SUM(CREDIT) AS Credit

FROM         (

SELECT PaidAmount+SpecialDiscount AS DEBIT, TTLValue+TTLSalestax AS CREDIT

FROM SaleReturnsPest SR INNER JOIN (SELECT SReturnId, Sum(TTLValue) TTLValue, Sum(TTLSalestax) TTLSalestax FROM SaleReturnsBodyPest GROUP BY SReturnId) SRB ON SR.SReturnId = SRB.SReturnId

WHERE      (1 = 1) AND (SR.ReturnDate <= @FROMDATE) AND (SR.CustomerId = @AccNo) 

)PSR               



UNION ALL



/*********** CROP PURCHASE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/

SELECT     SUM(DEBIT) AS Debit, SUM(CREDIT) AS Credit

FROM         (

SELECT     ISNULL(CropsPurchaseHeader.PaidAmount, 0) AS Debit, 

                     round( CASE WHEN isnull(CropsPurchaseHeader.VendorIDCom, 0) = 0 THEN (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) 

                      * CropsPurchaseBody.Price)) - ISNULL(CropsPurchaseHeader.ExpMinus, 0) + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 

                      + (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN iSNULL(ComAmount, 0) ELSE (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) 

                      * CropsPurchaseBody.Price) * IsNull(CropsPurchaseHeader.ComRatio, 0) / 100) END) 

                      ELSE ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 

                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) + (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN iSNULL(ComAmount, 0) 

                      ELSE (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price) * IsNull(CropsPurchaseHeader.ComRatio, 0) 

                      / 100) END)) * (100 - CropsPurchaseHeader.VendorComRatio) / 100 END ,0)AS  Credit

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID

WHERE     (CropsPurchaseHeader.IsPosted = 1) AND CropsPurchaseHeader.PurchaseDate  <= @FROMDATE AND CropsPurchaseHeader.VendorID  = @AccNo



GROUP BY CropsPurchaseHeader.ComAmount, CropsPurchaseHeader.ComPerMound, 

                      CropsPurchaseHeader.IsPosted, 

                      CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio, 

                      CropsPurchaseHeader.CropPurchaseID,CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks, CropsPurchaseHeader.PaidAmount,NarrationUrdu

                      )CP

                      

UNION ALL



/*********** CROP Sale NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/

SELECT     SUM(DEBIT) AS Debit, SUM(CREDIT) AS Credit

FROM         (

SELECT     ROUND(((SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier * CropsSaleBody.Price) - ISNULL(CropsSaleHeader.ExpMinus, 0) + ISNULL(CropsSaleHeader.ExpPlus, 0) 

                      + CASE WHEN ISNULL(CropsSaleHeader.ComIsMinus, 0) = 1 THEN - (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN ISNULL(ComAmount, 0) 

                      ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 

                      * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) ELSE (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN ISNULL(ComAmount, 0) 

                      ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 

                      * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) END) + SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier * CropsSaleBody.Price) * ISNULL(CropsSaleHeader.STRatio, 0) / 100) 

                      - SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier * CropsSaleBody.Price) * ISNULL(CropsSaleHeader.WHTRatio, 

                      0) / 100, 0) AS Debit, ISNULL(CropsSaleHeader.PaidAmount, 0) AS Credit

                      

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID LEFT OUTER JOIN

                      CropsSalesWeighments ON CropsSaleBody.WeighmentID = CropsSalesWeighments.WeighmentID

WHERE     (CropsSaleHeader.IsPosted = 1) AND CropsSaleHeader.SaleDate <= @FROMDATE AND CropsSaleHeader.CustomerID = @AccNo

GROUP BY CropsSaleHeader.CustomerID, CropsSaleHeader.STRatio, CropsSaleHeader.WHTRatio, CropsSaleHeader.ExpMinus, CropsSaleHeader.ExpPlus, 

                      CropsSaleHeader.Remarks, CropsSaleHeader.PaidAmount, CropsSaleHeader.ComRatio, CropsSaleHeader.ComAmount, 

                     CropsSaleHeader.CropSaleID, CropsSaleHeader.ComPerMound, CropsSaleHeader.ComIsMinus, CropsSaleHeader.NarrationUrdu, CropsSalesWeighments.LotNo                      

                      )CS

                      )TTT

return @retData

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[CashInHand]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[CashInHand]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[CashInHand];
IF OBJECT_ID(N'[dbo].[CashInHand]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[CashInHand];
IF OBJECT_ID(N'[dbo].[CashInHand]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[CashInHand];
GO

CREATE FUNCTION dbo.CashInHand (@AccountNo Varchar(10))

RETURNS Decimal(18,2) AS  

BEGIN 

    Declare @Value Decimal (18,2), @Value2 Decimal (18,2)

    Select @Value = Sum(OpeningDebit-OpeningCredit) from ChartOfaccounts  Where AccountNo = '111'

    Select @Value2 = Sum(Debit-Credit) from VuAccVouchersAll Where AccountNo = '111'

    Return IsNull(@Value,0)+IsNull(@Value2,0)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[fn_AllData]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[fn_AllData]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[fn_AllData];
IF OBJECT_ID(N'[dbo].[fn_AllData]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[fn_AllData];
IF OBJECT_ID(N'[dbo].[fn_AllData]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[fn_AllData];
GO

CREATE FUNCTION [dbo].[fn_AllData]()

    RETURNS @Results TABLE

    (

        VoucherType nvarchar(5),

    VoucherNo bigint,

    VoucherDate smalldatetime,

    ChequeNo nvarchar(25),

    ChequeDate smalldatetime,

    DepositedDate smalldatetime,

    ReconcileDate smalldatetime,

    AccountNo nvarchar(12), 

    Narration nvarchar(1000),

    Debit decimal(25,5),

    Credit decimal(25,5), 

    IsApplyPartyMarkup bit

    --,    CropSeason varchar(5)

    )

AS

BEGIN

    INSERT INTO @Results 

    SELECT VoucherType, VoucherNo, VoucherDate, ChequeNo, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, Debit, Credit, IsApplyPartyMarkup FROM VuAccVouchersOthers 

    union all 

    SELECT VoucherType, VoucherNo, VoucherDate, ChequeNo, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, Debit, Credit, IsApplyPartyMarkup FROM VuAccVouchersOthersExtension

    RETURN 

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[fn_AllDataNewID]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[fn_AllDataNewID]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[fn_AllDataNewID];
IF OBJECT_ID(N'[dbo].[fn_AllDataNewID]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[fn_AllDataNewID];
IF OBJECT_ID(N'[dbo].[fn_AllDataNewID]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[fn_AllDataNewID];
GO

CREATE FUNCTION [dbo].[fn_AllDataNewID]()

    RETURNS @Results TABLE

    (

        VoucherType nvarchar(5),

    VoucherNo bigint,

    VoucherDate smalldatetime,

    ChequeNo nvarchar(25),

    ChequeDate smalldatetime,

    DepositedDate smalldatetime,

    ReconcileDate smalldatetime,

    AccountNo nvarchar(12), 

    Narration nvarchar(1000),

    Debit decimal(25,5),

    Credit decimal(25,5), 

    IsApplyPartyMarkup bit

    )

AS

BEGIN

    INSERT INTO @Results SELECT * FROM VuAccVouchersOthersNewIDs union all SELECT * FROM VuAccVouchersOthersExtensionNewIDs

    RETURN 



END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunBatchesString]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunBatchesString]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunBatchesString];
IF OBJECT_ID(N'[dbo].[FunBatchesString]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunBatchesString];
IF OBJECT_ID(N'[dbo].[FunBatchesString]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunBatchesString];
GO

CREATE FUNCTION FunBatchesString 

    (@InvoiceNo int, @ProductID varchar(10), @InvoiceType varchar(15))

RETURNS varchar(100)

AS

BEGIN

    Declare @BatchList varchar(100)

    Declare @tempVar varchar(20)

    set @BatchList = ''

if upper(@InvoiceType)=upper('Sales')

    Declare curBatchList cursor local fast_forward

    for select batchno +'('+ convert(varchar, expirydate, 3) +')' as batchno from salesbatch where Saleid = @InvoiceNo and Productid = @Productid

else if upper(@InvoiceType)=upper('SaleReturns')

    Declare curBatchList cursor local fast_forward

    for select batchno +'('+ convert(varchar, expirydate, 3) +')' as batchno from SaleReturnsBatch where SReturnId = @InvoiceNo and Productid = @Productid

else if upper(@InvoiceType)=upper('PurchaseReturns')

    Declare curBatchList cursor local fast_forward

    for select batchno +'('+ convert(varchar, expirydate, 3) +')' as batchno from PurReturnsBatch where PReturnId = @InvoiceNo and Productid = @Productid

else if upper(@InvoiceType)=upper('Purchase')

    Declare curBatchList cursor local fast_forward

    for select batchno +'('+ convert(varchar, expirydate, 3) +')' as batchno from PurchasesBatch where PurchaseId = @InvoiceNo and Productid = @Productid

else

    Declare curBatchList cursor local fast_forward

    for select batchno +'('+ convert(varchar, expirydate, 3) +')' as batchno from CurrentStockPestro where Productid = @Productid



    open curbatchlist

    if @@cursor_rows = 0

        goto finalize

    fetch next from curBatchList into @tempVar

    while @@Fetch_Status = 0

    begin

        set @BatchList = @BatchList + @TempVar + ', '

        fetch next from curBatchList into @tempVar

        

    end 

    close curbatchlist

    deallocate curbatchlist



finalize:

    if len(@batchlist) = 0

        return @batchlist

    return substring(@batchList,1,len(@batchlist)-1)

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunComListToStr]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunComListToStr]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunComListToStr];
IF OBJECT_ID(N'[dbo].[FunComListToStr]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunComListToStr];
IF OBJECT_ID(N'[dbo].[FunComListToStr]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunComListToStr];
GO

CREATE FUNCTION dbo.FunComListToStr (@ProcessType as varchar(15), @ProcessId as int)  



RETURNS varchar(1000)  



--WITH ENCRYPTION



AS 

                             

BEGIN 



    DECLARE @ComList as varchar(1000)



    DECLARE



        @OldProductName varchar(50),



        @CurPhaseNo     varchar(10),



        @CurProductName     varchar(50),



        @CurPacking     varchar(30),



        @CurQuantity    varchar(30),



        @CurPrice  decimal(9,2),



        @CurDics       decimal(7,2),



        @CurSTax       decimal(7,2)





/*



1- Purchases



2- Sales



3- PurchaseReturns



4- SaleReturns



*/



    Set @ComList = ''



/*******************************************************************************************/



/*****************Added Phase No. By Fahad*****************************************/

IF UPPER(@ProcessType) = 'PURCHASE' 



    Declare CurComList Cursor STATIC For SELECT    Null as PhaseNo, Products.ProductName, '1 PC' as Packing, PurchasesBody.Quantity, PurchasesBody.Price, PurchasesBody.DiscRatio1, PurchasesBody.STRatio FROM         PurchasesBodyPest PurchasesBody INNER JOIN Products ON PurchasesBody.ProductId = Products.ProductId WHERE     (PurchasesBody.PurchaseId =  @ProcessId) For Read Only



ELSE IF UPPER(@ProcessType) = 'PURCHASERETURN' 



    Declare CurComList Cursor STATIC For SELECT     Null as PhaseNo, Products.ProductName, '1 PC' as Packing, PurReturnsBody.Quantity, PurReturnsBody.Price, PurReturnsBody.DiscRatio1, PurReturnsBody.STRatio FROM         PurReturnsBodyPest PurReturnsBody INNER JOIN Products ON PurReturnsBody.ProductId = Products.ProductId WHERE     (PurReturnsBody.PReturnId = @ProcessId) For Read Only



ELSE IF UPPER(@ProcessType) = 'SALE' 



    Declare CurComList Cursor STATIC For SELECT   (Select PhaseNo From SalesPest Where SaleId = SalesBody.SaleId) As PhaseNo,   Products.ProductName, '1 PC' as Packing, SalesBody.Quantity, SalesBody.Price, SalesBody.DiscRatio1, SalesBody.STRatio FROM         SalesBodyPest SalesBody INNER JOIN Products ON SalesBody.ProductId = Products.ProductId WHERE     (SalesBody.SaleId = @ProcessId) For Read Only



ELSE IF UPPER(@ProcessType) = 'SALERETURN' 



    Declare CurComList Cursor STATIC For SELECT     ' ' as PhaseNo, Products.ProductName, '1 PC' as Packing, SaleReturnsBody.Quantity, SaleReturnsBody.Price, SaleReturnsBody.DiscRatio1, SaleReturnsBody.STRatio FROM         SaleReturnsBodyPest SaleReturnsBody INNER JOIN Products ON SaleReturnsBody.ProductId = Products.ProductId WHERE     (SaleReturnsBody.SReturnId = @ProcessId) For Read Only



ELSE 



    RETURN @ComList



/*******************************************************************************************/



    Open CurComList



    FETCH Next FROM CurComList INTO @CurPhaseNo, @CurProductName, @CurPacking, @CurQuantity, @CurPrice, @CurDics, @CurSTax



    



    WHILE (@@FETCH_STATUS <> -1)



        Begin



        IF (@@FETCH_STATUS = 0)



            Begin



                if @OldProductName<>@CurProductName



                    Select @ComList = @ComList 



                   Select @ComList = ISNULL(@ComList, '') + CASE WHEN @CurPhaseNo <> '' THEN '(PH: ' + @CurPhaseNo + ')' ELSE '' END + ISNULL(@CurProductName, '') +'-'+ ISNULL(@CurPacking,'') +' x '+ ISNULL(cast(@CurQuantity as varchar(10)),'') +' @ '+ ISNULL(cast(@CurPrice as varchar(10)),'') + CASE WHEN ISNULL(@CurDics, 0) <>0 THEN ' (DS: ' + ISNULL(Cast(@CurDics as varchar(10)), '') + ')' ELSE '' END + CASE WHEN ISNULL(@CurSTax, 0)<>0 THEN 'ST: ' + ISNULL(cast(@CurSTax as varchar(10)), '') + ')' ELSE '' END  + char(13)



                    set @OldProductName=@CurProductName



            End



        FETCH Next FROM CurComList INTO @CurPhaseNo, @CurProductName, @CurPacking, @CurQuantity, @CurPrice, @CurDics, @CurSTax



        End



/*****************End By Fahad*****************************************/    



--    IF @@CURSOR_ROWS > 0



--        Select @ComList = LEFT(@ComList, (LEN(@ComList)-1))



    Close CurComList



    DEALLOCATE CurComList



RETURN @ComList



END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetCost]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetCost]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetCost];
IF OBJECT_ID(N'[dbo].[FunGetCost]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCost];
IF OBJECT_ID(N'[dbo].[FunGetCost]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetCost];
GO

CREATE FUNCTION [FunGetCost] (@ProductID varchar(10),@Date as smalldatetime)  



RETURNS Decimal(18,2)



AS



BEGIN 



    DECLARE @Cost decimal(18,2)

    SET @COST = 0

BEGIN



SELECT  @COST =   

 CASE WHEN SUM(qty) > 0 THEN SUM(Value) / SUM(Qty) ELSE 0 END 

FROM         (SELECT     SUM(TTLValue + TTLSalesTax) AS Value, SUM(Qty * Multiplier) AS Qty

                       FROM          OpeningStock

                       WHERE      (ProductID = @ProductID)

                       UNION ALL

                       SELECT     SUM(PurchaseBody.TTLValue + PurchaseBody.TTLSalesTax) AS Value, SUM(PurchaseBody.Qty * PurchaseBody.Multiplier) AS Qty

                       FROM         PurchaseBody INNER JOIN

                                             Purchase ON PurchaseBody.PurchaseID = Purchase.PurchaseID

                       WHERE     (PurchaseBody.ProductID = @ProductID) AND (Purchase.PurchaseDate <= @Date)

                       UNION ALL

                       SELECT     - ISNULL(SUM(PurReturnsBody.TTLValue + PurReturnsBody.TTLSalesTax), 0) AS Value, 

                                             - ISNULL(SUM(PurReturnsBody.Qty * PurReturnsBody.Multiplier), 0) AS Qty

                       FROM         PurReturnsBody INNER JOIN

                                             PurReturns ON PurReturnsBody.PurReturnID = PurReturns.PurReturnID

                       WHERE     (PurReturnsBody.ProductID = @ProductID) AND (PurReturns.PurReturnDate <= @Date)

                       union all

                       SELECT     SUM(ContractInvoiceBody.TotalValue) AS TotalValue, SUM(ContractInvoiceBody.TotalKgs) AS TotalKgs

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId



                       where ContractInvoice.CDate <= @Date and ContractInvoiceBody.ProductID = @ProductID

                       and  ContractInvoice.IsSaleInvoice = 0 AND ContractInvoice.MultiEntryID IS NULL



                       ) AS GP

END    

    RETURN @Cost

END

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetJVNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetJVNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetJVNarration];
IF OBJECT_ID(N'[dbo].[FunGetJVNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetJVNarration];
IF OBJECT_ID(N'[dbo].[FunGetJVNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetJVNarration];
GO

CREATE    Function FunGetJVNarration(        @VID int, @AccountNo varchar(10)) 

Returns Varchar(3990) AS    Begin   

Declare @FetchRemarks varchar(3990)  

Declare @RemarksList varchar(3990)  set @RemarksList = ''   DECLARE curSales CURSOR READ_ONLY  FOR 

--Select Products.ProductName  +':'+cast(PurchasesBodyPest.Quantity+PurchasesBodyPest.Bonus as varchar)+'@'+cast(PurchasesBodyPest.Price as varchar)

--+'. Rs.'+cast(PurchasesBodyPest.TTLValue+PurchasesBodyPest.TTLSalesTax as varchar)  as Remarks From PurchasesBodyPest INNER JOIN Products 

--ON Products.ProductId = PurchasesBodyPest.ProductId Where PurchasesBodyPest.PurchaseId=@PurId   





SELECT    JV.AccountNo +':'+ COA.AccountName

FROM         JournalVouchersBody AS JV INNER JOIN

                      ChartOfAccounts AS COA ON JV.AccountNo = COA.AccountNo

Where JV.AccountNo <> @AccountNo and JV.VoucherNo = @VID





  

OPEN curSales   FETCH NEXT FROM curSales INTO @FetchRemarks WHILE (@@fetch_status = 0)  



BEGIN           

set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end            

set @RemarksList = @RemarksList  + @FetchRemarks    

FETCH NEXT FROM curSales INTO @FetchRemarks END CLOSE curSales  DEALLOCATE curSales Return @RemarksList end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPPNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPPNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPPNarration];
IF OBJECT_ID(N'[dbo].[FunGetPPNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPPNarration];
IF OBJECT_ID(N'[dbo].[FunGetPPNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPPNarration];
GO

CREATE    Function FunGetPPNarration(

    @PurId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR Select Products.ProductName+':'+cast(PurchasesBodyPest.Quantity+PurchasesBodyPest.Bonus as varchar)+'@'+cast(PurchasesBodyPest.Price as varchar)+'. Rs.'+cast(PurchasesBodyPest.TTLValue+PurchasesBodyPest.TTLSalesTax as varchar)  as Remarks From PurchasesBodyPest INNER JOIN Products ON Products.ProductId = PurchasesBodyPest.ProductId Where PurchasesBodyPest.PurchaseId=@PurId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPPRNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPPRNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPPRNarration];
IF OBJECT_ID(N'[dbo].[FunGetPPRNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPPRNarration];
IF OBJECT_ID(N'[dbo].[FunGetPPRNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPPRNarration];
GO

CREATE    Function FunGetPPRNarration(

    @PurReturnId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR Select Products.ProductName+':'+cast(PurReturnsBodyPest.Quantity+PurReturnsBodyPest.Bonus as varchar)+'@'+cast(PurReturnsBodyPest.Price as varchar)+'. Rs.'+cast(PurReturnsBodyPest.TTLValue+PurReturnsBodyPest.TTLSalesTax as varchar)  as Remarks From PurReturnsBodyPest INNER JOIN Products ON Products.ProductId = PurReturnsBodyPest.ProductId Where PurReturnsBodyPest.PReturnId=@PurReturnId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPSNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPSNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPSNarration];
IF OBJECT_ID(N'[dbo].[FunGetPSNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPSNarration];
IF OBJECT_ID(N'[dbo].[FunGetPSNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPSNarration];
GO

CREATE    Function [FunGetPSNarration](        @SaleId int)    Returns nVarchar(3990) AS   Begin   Declare @FetchRemarks nVarchar(3990)    Declare @RemarksList nVarchar(3990) set @RemarksList = ''   DECLARE curSales CURSOR READ_ONLY  FOR 

  SELECT     Products.ProductName + ':' + CAST(SalesBodyPest.Quantity + SalesBodyPest.Bonus AS varchar) + '@' + CAST(SalesBodyPest.Price AS varchar) 

                      + CASE WHEN SalesBodyPest.DiscRatio1 > 0 THEN ', D%: ' + CAST(SalesBodyPest.DiscRatio1 AS VARCHAR) ELSE '' END + '. Rs.' + CAST(SalesBodyPest.TTLValue + SalesBodyPest.TTLSalestax AS varchar) AS Remarks

FROM         SalesBodyPest INNER JOIN

                      Products ON Products.ProductID = SalesBodyPest.ProductId

                      Where SalesBodyPest.SaleId=@SaleId    

  OPEN curSales   FETCH NEXT FROM curSales INTO @FetchRemarks WHILE (@@fetch_status = 0)  BEGIN           set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end            set @RemarksList = @RemarksList  + @FetchRemarks    FETCH NEXT FROM curSales INTO @FetchRemarks END CLOSE curSales  DEALLOCATE curSales Return @RemarksList end

GO

-- -----------------------------------------------------------------------------
-- Function: [dbo].[FunGetPSRNarration]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[FunGetPSRNarration]', N'FN') IS NOT NULL DROP FUNCTION [dbo].[FunGetPSRNarration];
IF OBJECT_ID(N'[dbo].[FunGetPSRNarration]', N'IF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPSRNarration];
IF OBJECT_ID(N'[dbo].[FunGetPSRNarration]', N'TF') IS NOT NULL DROP FUNCTION [dbo].[FunGetPSRNarration];
GO

CREATE    Function FunGetPSRNarration(

    @SaleReturnId int)

Returns Varchar(4000) AS

Begin

-- =============================================

-- Declare and using a READ_ONLY cursor

-- =============================================

Declare @FetchRemarks varchar(100)

Declare @RemarksList varchar(4000)

set @RemarksList = ''

DECLARE curSales CURSOR

READ_ONLY

FOR Select Products.ProductName+':'+cast(SaleReturnsBodyPest.Quantity+SaleReturnsBodyPest.Bonus as varchar)+'@'+cast(SaleReturnsBodyPest.Price as varchar)+'. Rs.'+cast(SaleReturnsBodyPest.TTLValue+SaleReturnsBodyPest.TTLSalesTax as varchar)  as Remarks From SaleReturnsBodyPest INNER JOIN Products ON Products.ProductId = SaleReturnsBodyPest.ProductId Where SaleReturnsBodyPest.SReturnId=@SaleReturnId 

OPEN curSales

FETCH NEXT FROM curSales INTO @FetchRemarks

WHILE (@@fetch_status = 0)

BEGIN

--    IF (@@fetch_status <> -2)

--    BEGIN

        set @RemarksList = @RemarksList  + case when @RemarksList = '' then '' else ', ' end

        set @RemarksList = @RemarksList  + @FetchRemarks 

FETCH NEXT FROM curSales INTO @FetchRemarks

--    End

END

CLOSE curSales

DEALLOCATE curSales

Return @RemarksList

end

GO


-- =============================================================================
-- VIEWS DEFINITION
-- =============================================================================
-- View: VuAccVouchersOtherAccounts
IF OBJECT_ID(N'[dbo].[VuAccVouchersOtherAccounts]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOtherAccounts];
GO
CREATE view VuAccVouchersAccounts 


 


 As


 


   /* -------------- CREDIT VOUCHERS ---------------- */


 


   select 'CV' as VOUCHERTYPE,CREDITVOUCHERS.VOUCHERNO,CREDITVOUCHERS.VOUCHERDATE,'' as CHEQUENO,null as ChequeDate, null as DepositedDate, Null as ReconcileDate, CREDITVOUCHERSBODY.ACCOUNTNO ,CREDITVOUCHERSBODY.NARRATION, 0 as DEBIT,CREDITVOUCHERSBODY.CREDIT from


 


     CREDITVOUCHERS join CREDITVOUCHERSBODY on CREDITVOUCHERS.VOUCHERNO=CREDITVOUCHERSBODY.VOUCHERNO


 


     and CREDITVOUCHERS.VOUCHERDATE=CREDITVOUCHERSBODY.VOUCHERDATE where CREDITVOUCHERS.ISPOSTED=1 


 


 UNION ALL


 


   select 'CV' as VOUCHERTYPE,CREDITVOUCHERS.VOUCHERNO,CREDITVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, dbo.RegistryValue('AccCashAccount') as ACCOUNTNO,'Cash received' as NARRATION,


 


     SUM(CREDITVOUCHERSBODY.CREDIT) as DEBIT,0 as CREDIT from


 


     CREDITVOUCHERS join CREDITVOUCHERSBODY on


 


     CREDITVOUCHERS.VOUCHERNO=CREDITVOUCHERSBODY.VOUCHERNO


 


     and CREDITVOUCHERS.VOUCHERDATE=CREDITVOUCHERSBODY.VOUCHERDATE where


 


     CREDITVOUCHERS.ISPOSTED=1


 


     group by CREDITVOUCHERS.VOUCHERNO,CREDITVOUCHERS.VOUCHERDATE


 


 UNION ALL


 


   /* -------------- DEBIT VOUCHERS ---------------- */


 


   select 'DV' as VOUCHERTYPE,DEBITVOUCHERS.VOUCHERNO,DEBITVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, DEBITVOUCHERSBody.ACCOUNTNO, DEBITVOUCHERSBody.NARRATION, DEBITVOUCHERSBody.DEBIT ,0 as CREDIT from


 


     DEBITVOUCHERSBody join DEBITVOUCHERS on


 


     DEBITVOUCHERS.VOUCHERNO=DEBITVOUCHERSBody.VOUCHERNO


 


     and DEBITVOUCHERS.VOUCHERDATE=DEBITVOUCHERSBody.VOUCHERDATE where


 


     DEBITVOUCHERS.ISPOSTED=1 


 


 UNION ALL


 


   select 'DV' as VOUCHERTYPE,DEBITVOUCHERS.VOUCHERNO,DEBITVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, dbo.RegistryValue('AccCashAccount') as ACCOUNTNO,'Cash paid' as NARRATION,


 


     0 as DEBIT,


 


     SUM(DEBITVOUCHERSBody.DEBIT) as CREDIT from


 


     DEBITVOUCHERSBody join DEBITVOUCHERS on


 


     DEBITVOUCHERSBody.VOUCHERNO=DEBITVOUCHERS.VOUCHERNO


 


     and DEBITVOUCHERS.VOUCHERDATE=DEBITVOUCHERSBody.VOUCHERDATE where


 


     DEBITVOUCHERS.ISPOSTED=1


 


     group by DEBITVOUCHERS.VOUCHERNO,DEBITVOUCHERS.VOUCHERDATE


 


 UNION ALL


 


   /* -------------- JOURNAL VOUCHERS ---------------- */


 


   select 'JV' as VOUCHERTYPE,JOURNALVOUCHERS.VOUCHERNO,JOURNALVOUCHERS.VOUCHERDATE,'' as CHEQUENO, null as ChequeDate, null as DepositedDate, Null as ReconcileDate, JOURNALVOUCHERSBODY.ACCOUNTNO,JOURNALVOUCHERSBODY.NARRATION,


 


     JOURNALVOUCHERSBODY.DEBIT,JOURNALVOUCHERSBODY.CREDIT from


 


     JOURNALVOUCHERS join JOURNALVOUCHERSBODY on


 


     JOURNALVOUCHERS.VOUCHERNO=JOURNALVOUCHERSBODY.VOUCHERNO


 


     and JOURNALVOUCHERS.VOUCHERDATE=JOURNALVOUCHERSBODY.VOUCHERDATE where


 


     JOURNALVOUCHERS.ISPOSTED=1


 


 UNION ALL


 


   /* -------------- BANK CHEQUES ---------------- */


 


   select 'BC' as VOUCHERTYPE,BANKCHEQUES.VOUCHERNO,BANKCHEQUES.VOUCHERDATE,BANKCHEQUES.CHEQUENO,      ChequeDate, null as DepositedDate, ReconcileDate,


 


     BANKCHEQUES.AccountNo,BANKCHEQUES.NARRATION,BANKCHEQUES.AMOUNT as DEBIT,0 as CREDIT from


 


     BANKCHEQUES where


 


     BANKCHEQUES.ISPOSTED=1 and BANKCHEQUES.ISLOST=0


 


 UNION ALL


 


   select 'BC' as VOUCHERTYPE,BANKCHEQUES.VOUCHERNO,BANKCHEQUES.VOUCHERDATE,BANKCHEQUES.CHEQUENO,      ChequeDate, null as DepositedDate, ReconcileDate,


 


     BANKCHEQUES.BANKACCOUNTNO,BANKCHEQUES.NARRATION,0 as DEBIT,BANKCHEQUES.AMOUNT as CREDIT from


 


     BANKCHEQUES where


 


     BANKCHEQUES.ISPOSTED=1 and BANKCHEQUES.ISLOST=0 


 


 UNION ALL


 


   /* -------------- BANK Deposits ---------------- */


 


   select 'BD' as VOUCHERTYPE,BANKDEPOSIT.VOUCHERNO,BANKDeposit.VOUCHERDATE,BANKDeposit.SlipNo,        ChequeDate, DepositedDate, ReconcileDate,


 


     IsNull(BANKDeposit.AccountNo,'111'), BANKDeposit.NARRATION,0 as Debit,BANKDeposit.AMOUNT as CREDIT from


 


     BANKDeposit where


 


     BANKDeposit.ISPOSTED=1 and BANKDeposit.ISBounced=0 


 


 UNION ALL


 


   select 'BD' as VOUCHERTYPE,BANKDEPOSIT.VOUCHERNO,BANKDeposit.VOUCHERDATE,BANKDeposit.SlipNo,


 


     null as ChequeDate, DepositedDate, ReconcileDate,


 


     BANKDeposit.BankAccountNo,BANKDeposit.NARRATION,BANKDeposit.AMOUNT as DEBIT,0 as CREDIT from


 


     BANKDeposit where


 


     BANKDeposit.ISPOSTED=1 and BANKDeposit.ISBounced=0
GO

CREATE VIEW [dbo].[VuAccVouchersOtherAccounts]  AS 



/* ------Pur Mazdoori Acc--- */

SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      SUM(Credit) AS Credit, IsApplyPartyMarkup, CropSeason

FROM         (SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, MazdooriAcc AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                                              ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) 

                                              = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.PurMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori ELSE round(((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.PurMazdori, 0) END WHEN ISNULL(PMazdoriWOShort, 0) 

                                              > 0 THEN ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori ELSE ((ContractInvoiceBody.TotalKgs) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori END, 0) AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                                              AS IsApplyPartyMarkup, ContractInvoice.CropSeason

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE   ISNULL(MazdooriAcc,'') <> '' AND   (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusMazdori, 0) = 0) AND (ISNULL(ContractInvoice.AddMazdori, 0) = 0) AND (ContractInvoice.PurMazdori > 0)  AND ContractInvoice.IsShortInvoice = 0  and ISNULL(IsMultiNew,0) = 0)

                      AS tmp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup,AccountNo, CropSeason



UNION ALL



SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      SUM(Credit) AS Credit, IsApplyPartyMarkup, CropSeason

FROM         (SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57002' AS AccountNo, ContractInvoice.Narration, ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) 

                                              = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.PurMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori ELSE round(((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.PurMazdori, 0) END WHEN ISNULL(PMazdoriWOShort, 0) 

                                              > 0 THEN ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori ELSE ((ContractInvoiceBody.TotalKgs) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori END, 0) AS Debit, 

                                              0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                                              AS IsApplyPartyMarkup, ContractInvoice.CropSeason

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE   ISNULL(MazdooriAcc,'') <> '' AND   (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusMazdori, 0) = 0) AND (ISNULL(ContractInvoice.AddMazdori, 0) = 0) AND (ContractInvoice.PurMazdori > 0)  AND ContractInvoice.IsShortInvoice = 0  and ISNULL(IsMultiNew,0) = 0)

                      AS tmp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup, CropSeason



UNION ALL

/* ------Pur Vehicle Charges Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, VehicleChargesAcc AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND( ContractInvoice.VehicalCharges ,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 0) And (Isnull(ContractInvoice.AddVehicleCharges,0) = 0) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

and ISNULL(VehicleChargesAcc,'') <> ''

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup,VehicleChargesAcc, ContractInvoice.CropSeason

HAVING      (ContractInvoice.VehicalCharges <> 0)



UNION ALL

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57009' AS AccountNo, ContractInvoice.Narration, 

                      ROUND( ContractInvoice.VehicalCharges ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 0) And (Isnull(ContractInvoice.AddVehicleCharges,0) = 0) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

and ISNULL(VehicleChargesAcc,'') <> ''

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup,VehicleChargesAcc, ContractInvoice.CropSeason

HAVING      (ContractInvoice.VehicalCharges <> 0)



UNION ALL

/* ------Pur Changrai Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, ChangraiAcc AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100,0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusChangrai,0) = 0)And (Isnull(ContractInvoice.AddChangrai,0) = 0)  and ISNULL(IsMultiNew,0) = 0

AND ISNULL(ChangraiAcc,'') <> ''

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, IsApplyPartyMarkup,ChangraiAcc, ContractInvoice.CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)



UNION ALL

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57013' AS AccountNo, ContractInvoice.Narration, 

                      round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100,0)  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusChangrai,0) = 0)And (Isnull(ContractInvoice.AddChangrai,0) = 0)  and ISNULL(IsMultiNew,0) = 0

AND ISNULL(ChangraiAcc,'') <> ''

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, IsApplyPartyMarkup, ContractInvoice.CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)





UNION ALL



/* ------Pur Dalali Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, DalaliAcc AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 ,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusDalali,0) = 0)  And (Isnull(ContractInvoice.AddDalali,0) = 0)

AND ISNULL(DalaliAcc,'') <> ''

 GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup,DalaliAcc, ContractInvoice.CropSeason



UNION ALL



/* ------Pur Dalali Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57003' AS AccountNo, ContractInvoice.Narration, 

                      round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusDalali,0) = 0)  And (Isnull(ContractInvoice.AddDalali,0) = 0)

AND ISNULL(DalaliAcc,'') <> ''

 GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup, ContractInvoice.CropSeason





UNION ALL

/* ------Sale Mazdoori Acc--- */





SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit)AS Debit, 

                      round(SUM(Credit),0)AS Credit, IsApplyPartyMarkup, CropSeason

FROM         (SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, MazdooriAcc AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                                              

                                              ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN  CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.SaleMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleMazdori END 

                                              when isnull(SMazdoriWOShort,0) > 0 then

                                              ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori

                                              

                                              ELSE ((ContractInvoiceBody.TotalKgs) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori END, 2)AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                                              AS IsApplyPartyMarkup, ContractInvoice.CropSeason

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE    ISNULL(MazdooriAcc,'') <> '' AND  (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.AddMazdori, 0) = 1) AND ((isnull(ContractInvoice.SaleMazdori,0) > 0)) and IsShortInvoice =  0 and ISNULL(IsMultiNew,0) = 0 --AND (ISNULL(ContractInvoice.AddMazdori, 0) = 1)

                       )

                      AS temp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup, CropSeason



UNION ALL



SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit)AS Debit, 

                      round(SUM(Credit),0)AS Credit, IsApplyPartyMarkup, CropSeason

FROM         (SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, MazdooriAcc AS AccountNo, ContractInvoice.Narration, ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN  CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.SaleMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleMazdori END 

                                              when isnull(SMazdoriWOShort,0) > 0 then

                                              ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori

                                              

                                              ELSE ((ContractInvoiceBody.TotalKgs) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori END, 2) AS Debit, 

                                              

                                              0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                                              AS IsApplyPartyMarkup, ContractInvoice.CropSeason

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE    ISNULL(MazdooriAcc,'') <> '' AND  (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.MinusMazdori, 0) = 1) AND ((isnull(ContractInvoice.SaleMazdori,0) > 0)) and IsShortInvoice =  0 and ISNULL(IsMultiNew,0) = 0 --AND (ISNULL(ContractInvoice.AddMazdori, 0) = 0)

                       )

                      AS temp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup, CropSeason



UNION ALL



/* ------Sale Dalali Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, DalaliAcc AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND( SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 ,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddDalali,0) = 0) and ISNULL(IsMultiNew,0) = 0

AND ISNULL(DalaliAcc,'') <> '' And (Isnull(ContractInvoice.MinusDalali,0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup,DalaliAcc, ContractInvoice.CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58003' AS AccountNo, ContractInvoice.Narration, 

                      ROUND( SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 ,0) AS Debit, 0  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddDalali,0) = 0) and ISNULL(IsMultiNew,0) = 0

AND ISNULL(DalaliAcc,'') <> '' And (Isnull(ContractInvoice.MinusDalali,0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup, ContractInvoice.CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)





UNION ALL

/*-----------------*/





/* ------Sale VehicalCharges  Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, VehicleChargesAcc AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ContractInvoice.VehicalCharges AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 0) AND IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

AND ISNULL(VehicleChargesAcc,'') <> '' And (Isnull(ContractInvoice.AddVehicleCharges,0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup,VehicleChargesAcc, ContractInvoice.CropSeason

HAVING      (ContractInvoice.VehicalCharges <> 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58010' AS AccountNo, ContractInvoice.Narration, 

                      ContractInvoice.VehicalCharges AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, ContractInvoice.CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 0) AND IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

AND ISNULL(VehicleChargesAcc,'') <> '' And (Isnull(ContractInvoice.AddVehicleCharges,0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup, ContractInvoice.CropSeason

HAVING      (ContractInvoice.VehicalCharges <> 0)
GO

-- View: VuAccVouchersOthers
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthers]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthers];
GO
CREATE VIEW [dbo].[VuAccVouchersOthers]  AS 

/*************** BROKERAGE Shortage Adjustment********************************************/





SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, VendorID AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, ROUND(PurchaseRate * (ShortageInKgs/37.324), 4) AS Debit, 0 AS Credit, 

                      0 AS IsApplyPartyMarkup, NULL CropSeason 

FROM         BrokrageShortage



UNION ALL



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, CustomerID AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, 0 AS Debit, ROUND(SaleRate *((ShortageInKgs-ISNULL(CommissionWeight,0))/37.324), 4) AS Credit, 

                      0 AS IsApplyPartyMarkup, NULL CropSeason





FROM         BrokrageShortage 



UNION ALL



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, dbo.RegistryValue('BrokrageShortageRevenue') AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, 0 AS Debit, ROUND(SaleRate * (CommissionWeight/37.324), 4) AS Credit, 

                      0 AS IsApplyPartyMarkup, NULL CropSeason

FROM         BrokrageShortage

Where ISNULL(CommissionWeight,0) > 0



/*****************************************************************************************/



UNION ALL



/*************** BROKERAGE PURCHASE EXPENSES********************************************/



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '46' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.CommissionValue)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0) AND ContractInvoice.IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)



UNION ALL



/***Purchase Discount income***/

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '5124' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.Discount)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0) AND ContractInvoice.IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup,  CropSeason



HAVING      (SUM(ContractInvoice.Discount) > 0)



UNION ALL



/***********/



/* ------Pur Arhat Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '47' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 ,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId







WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusArhat,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '47' AS AccountNo, ContractInvoice.Narration, 

                     round( SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100  ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddArhat,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL

/*--------------------*/



/* ------Pur-With Holding Tax Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '2301' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      round((ContractInvoice.CommissionValue * Isnull(ContractInvoice.WHTax, 0) / 100),0) AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

Where (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddWHTax,0) = 0) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.WHTax,  CropSeason

HAVING      Isnull(ContractInvoice.WHTax, 0) > 0



UNION ALL





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 





                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '2301' AS AccountNo, ContractInvoice.Narration, round((ContractInvoice.CommissionValue * Isnull(ContractInvoice.WHTax, 0) / 100),0) AS Debit, 

                      0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId





Where (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddWHTax,0) = 1) and ISNULL(IsMultiNew,0) = 0





GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.WHTax,  CropSeason

HAVING      Isnull(ContractInvoice.WHTax, 0) > 0



UNION ALL



/*--------------------*/

/* ------Pur Changrai Acc--- */





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.ChangraiAcc,'') <> '' THEN ContractInvoice.ChangraiAcc ELSE '57013' END AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ROUND(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100, 0) AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusChangrai, 0) = 1) AND (ISNULL(ContractInvoice.IsMultiNew, 0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.ChangraiAcc,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.ChangraiAcc,'') <> '' THEN ContractInvoice.ChangraiAcc ELSE '57013' END AS AccountNo, ContractInvoice.Narration, 

                      round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100,0)  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddChangrai,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, IsApplyPartyMarkup,ContractInvoice.ChangraiAcc,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)



UNION ALL



/* ------Pur Sootli Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57020' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      round((SUM(ContractInvoiceBody.BagsQty) + SUM(ISNULL(ContractInvoiceBody.PQty, 0))) * ContractInvoice.SaleSootli ,0) AS Credit,

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusSootli, 0) = 1) AND ContractInvoice.IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli, ContractInvoice.IsApplyPartyMarkup,  CropSeason



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57020' AS AccountNo, ContractInvoice.Narration, 

                      round((SUM(ContractInvoiceBody.BagsQty) + SUM(ISNULL(ContractInvoiceBody.PQty, 0))) * ContractInvoice.SaleSootli ,0) AS Debit, 0 AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusSootli, 0) = 0) AND ContractInvoice.IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli, ContractInvoice.IsApplyPartyMarkup,  CropSeason

having ContractInvoice.SaleSootli > 0



UNION ALL



/* ------Pur Mazdoori Acc--- */



SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      SUM(Credit) AS Credit, IsApplyPartyMarkup,  CropSeason

FROM         (SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.MazdooriAcc,'') <> '' THEN ContractInvoice.MazdooriAcc ELSE '57002' END AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                                              ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) 

                                              = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.PurMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori ELSE round(((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.PurMazdori, 0) END WHEN ISNULL(PMazdoriWOShort, 0) 

                                              > 0 THEN ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori ELSE ((ContractInvoiceBody.TotalKgs) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori END, 0) AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                                              AS IsApplyPartyMarkup,  CropSeason

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusMazdori, 0) = 1) AND (ContractInvoice.PurMazdori > 0)  AND ContractInvoice.IsShortInvoice = 0  and ISNULL(IsMultiNew,0) = 0)

                      AS tmp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup,  CropSeason



UNION ALL

SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      SUM(Credit) AS Credit, IsApplyPartyMarkup,  CropSeason

FROM         (SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.MazdooriAcc,'') <> '' THEN ContractInvoice.MazdooriAcc ELSE '57002' END AS AccountNo, ContractInvoice.Narration, 

                      ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) 

                      = 1 THEN (ContractInvoiceBody.TotalValue) 

                      * ContractInvoice.PurMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                      = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                      * ContractInvoice.PurMazdori ELSE round(((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                      > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.PurMazdori, 0) END WHEN ISNULL(PMazdoriWOShort, 0) 

                      > 0 THEN ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) / ContractInvoiceBody.MoundStd) 

                      * ContractInvoice.PurMazdori ELSE ((ContractInvoiceBody.TotalKgs) / ContractInvoiceBody.MoundStd) 

                      * ContractInvoice.PurMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) * ContractInvoice.PurMazdori END, 0) 

                      AS Debit, 0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup ,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.AddMazdori, 0) = 1) AND (ContractInvoice.PurMazdori > 0) AND 

                      (ContractInvoice.IsShortInvoice = 0) AND (ISNULL(ContractInvoice.IsMultiNew, 0) = 0)) AS tmp



GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup,  CropSeason





/* --------- */



UNION ALL



/* ------Pur Dalali Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.DalaliAcc,'') <> '' THEN ContractInvoice.DalaliAcc ELSE '57003' END AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 ,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusDalali,0) = 1) 



 GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup, ContractInvoice.DalaliAcc,  CropSeason







HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 > 0) 



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.DalaliAcc,'') <> '' THEN ContractInvoice.DalaliAcc ELSE '57003' END AS AccountNo, ContractInvoice.Narration, 

                      round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddDalali,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup, ContractInvoice.DalaliAcc,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 > 0) 



UNION ALL

/*---------------*/



/* ------Pur Bank Charges Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57016' AS AccountNo, Narration, 0 AS Debit, round(BankChargesAmount,0) AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(MinusBankCharges, 0) = 1) AND (ISNULL(BankChargesAmount, 0) > 0)

GROUP BY ContractPurId, CDate, Narration, PurPulliGhya, IsApplyPartyMarkup, BankChargesAmount,  CropSeason



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57016' AS AccountNo, Narration, round(BankChargesAmount,0) AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(AddBankCharges, 0) = 1) AND (ISNULL(BankChargesAmount, 0) > 0)



/*---------------*/

UNION ALL

/* ------ Pur Ref Commission Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57017' AS AccountNo, Narration, round(RefCommValue,0) AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(RefCommValue, 0) > 0)





UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, round(RefCommValue,0) AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(RefCommValue, 0) > 0)





UNION ALL



/* ------Pur Carriage Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57004' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, 

                      CASE WHEN ISNULL(PCarriageWOShort,0) > 0 THEN 

                      round((SUM(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya ,0)

                      

                      ELSE

                      round((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya ,0) END AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusPuliGhya,0) = 1) --AND ISNULL(ContractInvoice.PurPulliGhya,0) > 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurPulliGhya, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup,PCarriageWOShort,  CropSeason

HAVING      ((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya > 0)







UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57004' AS AccountNo, ContractInvoice.Narration, 

                      CASE WHEN ISNULL(PCarriageWOShort,0) > 0 THEN 

                      round((SUM(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya ,0)

                      

                      ELSE

                      round((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya ,0) END AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddPuliGhya,0) = 1)  AND ISNULL(ContractInvoice.PurPulliGhya,0) > 0 GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurPulliGhya, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup,PCarriageWOShort,  CropSeason

HAVING      ((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya > 0)



UNION ALL

/*--------------*/



/* ------Pur Commission Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4205' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND(CASE WHEN isnull(ContractInvoice.DMCommmission, 0) > 0 THEN (SUM(ContractInvoiceBody.TotalValue) 

                      + ContractInvoice.CommissionValue + ISNULL(ContractInvoice.AddPurMarketFee, 0) + abs(ISNULL(ContractInvoice.MinusPurMarketFee, 0))) 

                      * ContractInvoice.PurComm / 100 ELSE SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 END, 0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0)  And (Isnull(ContractInvoice.MinusCommission,0) = 1) and IsShortInvoice = 0  and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurComm, IsApplyPartyMarkup,ContractInvoice.DMCommmission, ContractInvoice.AddPurMarketFee, ContractInvoice.MinusPurMarketFee, ContractInvoice.CommissionValue,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 > 0)



UNION ALL





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4205' AS AccountNo, ContractInvoice.Narration, 

                      ROUND(CASE WHEN isnull(ContractInvoice.DMCommmission, 0) > 0 THEN (SUM(ContractInvoiceBody.TotalValue) 

                      + ContractInvoice.CommissionValue + ISNULL(ContractInvoice.AddPurMarketFee, 0) + abs(ISNULL(ContractInvoice.MinusPurMarketFee, 0))) 

                      * ContractInvoice.PurComm / 100 ELSE SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 END, 0) AS Debit, 0 AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.AddCommission, 0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurComm, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.DMCommmission, ContractInvoice.AddPurMarketFee, ContractInvoice.MinusPurMarketFee, ContractInvoice.CommissionValue,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 > 0)



UNION ALL

--/*-------------*/

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '57028' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



case when  Isnull(ContractInvoice.AddTelePhoneExp,0)  > 0 then Isnull(ContractInvoice.AddTelePhoneExp,0) else 0 end AS Debit, 



              case when  Isnull(ContractInvoice.AddTelePhoneExp,0) <  0 then abs(Isnull(ContractInvoice.AddTelePhoneExp,0)) else 0 end AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

,  CropSeason

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 0 AND Isnull(ContractInvoice.AddTelePhoneExp,0) <> 0 and ISNULL(IsMultiNew,0) = 0

                      

                      

/* ------Pur Manshyana Acc--- */

union all

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57006' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, CASE WHEN ISNULL(ContractInvoice.MunshyanaPerMnd,0) = 0 THEN ROUND(SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana, 0)

                     ELSE ROUND(SUM(ContractInvoice.MunshyanaPer), 0) END   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusManshyana,0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup,ContractInvoice.MunshyanaPerMnd,  CropSeason

HAVING      (SUM(ContractInvoice.SaleManshyana) > 0)--HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57006' AS AccountNo, ContractInvoice.Narration, 

                     CASE WHEN ISNULL(ContractInvoice.MunshyanaPerMnd,0) = 0 THEN ROUND(SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana, 0)

                     ELSE ROUND(SUM(ContractInvoice.MunshyanaPer), 0) END  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddManshyana,0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup,ContractInvoice.MunshyanaPerMnd,  CropSeason

HAVING      (SUM(ContractInvoice.SaleManshyana) > 0)







/* ------Pur Kanta Expense Acc--- */

union all

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57029' AS AccountNo, ContractInvoice.Narration, 

                      case when SUM(KantaExpense)> 0 then abs(round(SUM(KantaExpense),0)) else 0 end AS Debit, case when SUM(KantaExpense)< 0 then abs(round(SUM(KantaExpense),0)) else 0 end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoice.KantaExpense) <> 0)





UNION ALL

/*-----------------*/



/* ------Pur Loading Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57007' AS AccountNo, ContractInvoice.Narration, 

          0 AS Debit, round( case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.PurLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        when isnull(PLoadingWOShort,0) > 0 then

         (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading

         else 

          (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading end ,0)

          AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup,PLoadingWOShort,  CropSeason

HAVING      (ContractInvoice.PurLoading > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57007' AS AccountNo, ContractInvoice.Narration, 

         ROUND( case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.PurLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        when isnull(PLoadingWOShort,0) > 0 then

         (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading end ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup,PLoadingWOShort,  CropSeason

HAVING      (ContractInvoice.PurLoading > 0)



UNION ALL

/*--------------*/



/* ------Pur Un-Loading Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57008' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit,ROUND(  case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.PurUnLoading * (Sum(ContractInvoiceBody.BagsQty)+ Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        when isnull(PUnLoadingWOShort,0) > 0 then

         (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading  end ,0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusUnLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurUnLoading, ContractInvoice.UnLoadingPerBag, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup,PUnLoadingWOShort,  CropSeason

HAVING      (ContractInvoice.PurUnLoading > 0)

 

UNION ALL 



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57008' AS AccountNo, ContractInvoice.Narration, 

                     ROUND(  case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.PurUnLoading * (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        when isnull(PUnLoadingWOShort,0) > 0 then

         (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading  end  ,0) AS Debit,  0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddUnLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurUnLoading, ContractInvoice.UnLoadingPerBag, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup,PUnLoadingWOShort,  CropSeason

HAVING      (ContractInvoice.PurUnLoading > 0)

 

UNION ALL 

/*--------------*/



/* ------Pur Vehicle Charges Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.VehicleChargesAcc,'') <> '' THEN ContractInvoice.VehicleChargesAcc ELSE '57009' END AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND( ContractInvoice.VehicalCharges ,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup, ContractInvoice.VehicleChargesAcc,  CropSeason

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.VehicleChargesAcc,'') <> '' THEN ContractInvoice.VehicleChargesAcc ELSE '57009' END AS AccountNo, ContractInvoice.Narration, 

                     ROUND(  ContractInvoice.VehicalCharges ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddVehicleCharges,0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup, ContractInvoice.VehicleChargesAcc,  CropSeason

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL





/* ------Pur Market Fees Acc--- */





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57010' AS AccountNo, ContractInvoice.Narration, 

                      ROUND( ISNULL(ContractInvoice.AddPurMarketFee, 0),0) AS Debit, ROUND( abs(ISNULL(ContractInvoice.MinusPurMarketFee, 0)),0) AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, 

                      ContractInvoice.IsApplyPartyMarkup, ContractInvoice.AddPurMarketFee, ContractInvoice.MinusPurMarketFee,  CropSeason

HAVING      (ISNULL(ContractInvoice.AddPurMarketFee, 0) > 0) OR

                      abs(ISNULL(ContractInvoice.MinusPurMarketFee, 0)) > 0







UNION ALL

/*-----------------*/



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4102' AS AccountNo, ContractInvoice.Narration, 

                      (CASE WHEN IsPay=0 THEN SUM(ContractInvoice.CommissionValue) ELSE 0 END) AS Debit, (CASE WHEN IsPay=1 THEN SUM(ContractInvoice.CommissionValue) ELSE 0 END)   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, IsPay, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)









UNION ALL /*************** BROKERAGE SALE EXPENSES********************************************/



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4103' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.CommissionValue)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 0) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)







UNION ALL

/*****Discount On Sale exp******/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '48' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.Discount) AS Debit, 0  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 0) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0





GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoice.Discount) > 0)





UNION ALL

/****************/







SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4103' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.CommissionValue) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)



/* ------Sale Kanta Expense Acc--- */

union all

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58025' AS AccountNo, ContractInvoice.Narration, 

                      case when SUM(KantaExpense)< 0 then abs(round(SUM(KantaExpense),0)) else 0 end AS Debit, case when SUM(KantaExpense)> 0 then abs(round(SUM(KantaExpense),0)) else 0 end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice 

--INNER JOIN ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,  IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoice.KantaExpense) <> 0)



UNION ALL

/* ------Sale Bank Charges Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58016' AS AccountNo, Narration, 0 AS Debit, BankChargesAmount AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(BankChargesAmount, 0) > 0) AND (ISNULL(AddBankCharges, 0) = 1) and ISNULL(IsMultiNew,0) = 0



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58016' AS AccountNo, Narration, BankChargesAmount AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(BankChargesAmount, 0) > 0) AND (ISNULL(MinusBankCharges, 0) = 1) and ISNULL(IsMultiNew,0) = 0

/*--------------------*/

UNION ALL

/* ------Sale Ref Commission Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1)  AND (ISNULL(RefCommValue, 0) > 0) and ISNULL(IsMultiNew,0) = 0



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(RefCommValue, 0) > 0) and ISNULL(IsMultiNew,0) = 0



/*--------------------*/

UNION ALL

/* ------Sale Market Fees Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58001' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND( SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees ,0)AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddMarketFees,0) = 1) and ISNULL(IsMultiNew,0) = 0



GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58001' AS AccountNo, ContractInvoice.Narration, 

                      ROUND( SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusMarketFees,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup,  CropSeason

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL

/*--------------------*/



/* ------Sale Mazdoori Acc--- */





SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit)AS Debit, 

                      round(SUM(Credit),0)AS Credit, IsApplyPartyMarkup,  CropSeason

FROM         (SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58002' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                                              

                                              ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN  CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.SaleMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleMazdori END 

                                              when isnull(SMazdoriWOShort,0) > 0 then

                                              ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori

                                              

                                              ELSE ((ContractInvoiceBody.TotalKgs) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori END, 2)AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                                              AS IsApplyPartyMarkup,  CropSeason

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE     ISNULL(MazdooriAcc,'') = '' AND  (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.AddMazdori, 0) = 1) AND ((isnull(ContractInvoice.SaleMazdori,0) > 0)) and IsShortInvoice =  0 and ISNULL(IsMultiNew,0) = 0)

                      AS temp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup,  CropSeason



union all

SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      round(SUM(Credit),0) AS Credit, IsApplyPartyMarkup,  CropSeason

FROM         (

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58002' AS AccountNo, ContractInvoice.Narration, 

                      

                      ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN  CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.SaleMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleMazdori END 

                                              when isnull(SMazdoriWOShort,0) > 0 then

                                              ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori

                                              

                                              ELSE ((ContractInvoiceBody.TotalKgs) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori END, 2) AS Debit, 

                      0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE    ISNULL(MazdooriAcc,'') = '' AND  (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.MinusMazdori, 0) = 1) and (isnull(ContractInvoice.SaleMazdori,0) > 0) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

) 

                      AS temp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup,  CropSeason



UNION ALL

/*-------------------*/



/* ------Sale Dalali Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58003' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND( SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 ,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

,  CropSeason FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddDalali,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58003' AS AccountNo, ContractInvoice.Narration, 

                      ROUND( SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

,  CropSeason FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusDalali,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)



UNION ALL

/*---------------------*/



/* ------Sale Sootli Acc--- */

SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) 

                      / COUNT(VoucherNo) AS Debit, SUM(Credit) / COUNT(VoucherNo) AS Credit, IsApplyPartyMarkup,  CropSeason 

FROM         (SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58004' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                                              SSoot.TSaleSootli AS Credit, 0 AS IsApplyPartyMarkup,  CropSeason 









                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId INNER JOIN

                                                  (SELECT     VoucherNo, SUM(TSaleSootli) AS TSaleSootli

                                                    FROM          (SELECT     ContractInvoice.ContractPurId AS VoucherNo, ROUND(CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                                                                                   = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                                                                                   * ContractInvoice.SaleSootli ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                                                                                    0) > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleSootli END, 0) 

                                                                                                   AS TSaleSootli

                                                                            FROM          ContractInvoice INNER JOIN

                                                                                                   ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId) AS tmp

                                                    GROUP BY VoucherNo) AS SSoot ON SSoot.VoucherNo = ContractInvoice.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.AddSootli, 0) = 1) AND (ContractInvoice.SaleSootli > 0) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0) AS SSotli

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup,  CropSeason 



UNION ALL



SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      SUM(Credit) AS Credit, IsApplyPartyMarkup,  CropSeason 

FROM         (SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58004' AS AccountNo, ContractInvoice.Narration, 

                                              ROUND(CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) = 0 THEN ((ContractInvoiceBody.BagsQty) 

                                              + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleSootli ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleSootli END, 0) AS Debit, 0 AS Credit, 

                                              0 AS IsApplyPartyMarkup,  CropSeason 

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.MinusSootli, 0) = 1) AND (ContractInvoice.SaleSootli > 0) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0) AS Temp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup,  CropSeason 

UNION ALL





/*------------------*/



/* ------Sale Commission Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4206' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ROUND(CASE WHEN isnull(ContractInvoice.DMCommmission, 0) > 0 THEN (SUM(ContractInvoiceBody.TotalValue) 

                      + isnull(ContractInvoice.CommissionValue, 0) + isnull(ROUND( SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees ,0), 0)) 

                      * ContractInvoice.SaleComm / 100 ELSE SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 END, 0) AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.AddCommission, 0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleComm, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.SaleMarketFees, ContractInvoice.DMCommmission,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4206' AS AccountNo, ContractInvoice.Narration, 

                      ROUND(CASE WHEN isnull(ContractInvoice.DMCommmission, 0) > 0 THEN (SUM(ContractInvoiceBody.TotalValue) 

                      + isnull(ContractInvoice.CommissionValue, 0) + isnull(ROUND( SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees ,0), 0)) 

                      * ContractInvoice.SaleComm / 100 ELSE SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 END, 0)  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusCommission,0) = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleComm, IsApplyPartyMarkup,ContractInvoice.CommissionValue,  ContractInvoice.SaleMarketFees, ContractInvoice.DMCommmission,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 > 0)



UNION ALL

/*---------------------*/



/* ------Sale Manshyana Acc--- */



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58006' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, CASE WHEN ISNULL(ContractInvoice.MunshyanaPerMnd,0) = 0 THEN ROUND(SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana, 0)

                     ELSE ROUND((ContractInvoice.MunshyanaPer), 0) END AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

                     

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddManshyana,0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,  IsApplyPartyMarkup, ContractInvoice.SaleManshyana,ContractInvoice.MunshyanaPerMnd,  CropSeason ,ContractInvoice.MunshyanaPer

HAVING      (SUM(ContractInvoice.SaleManshyana) <> 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58006' AS AccountNo, ContractInvoice.Narration, 

                     CASE WHEN ISNULL(ContractInvoice.MunshyanaPerMnd,0) = 0 THEN ROUND(SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana, 0)

                     ELSE ROUND((ContractInvoice.MunshyanaPer), 0) END AS Debit, 0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 

                      0) AS IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.MinusManshyana, 0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, 

                      ContractInvoice.IsApplyPartyMarkup, ContractInvoice.MunshyanaPerMnd,  CropSeason ,ContractInvoice.MunshyanaPer

HAVING      (SUM(ContractInvoice.SaleManshyana) <> 0)



UNION ALL





/*----------------------*/



/* ------Sale Loading Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58007' AS AccountNo, ContractInvoice.Narration, 

        0 AS Debit, case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.SaleLoading* (Sum(ContractInvoiceBody.BagsQty)+ Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        

        when isnull(SLoadingWOShort,0) > 0 then

        (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading 

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup,  CropSeason 

,SLoadingWOShort

HAVING      (ContractInvoice.SaleLoading > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58007' AS AccountNo, ContractInvoice.Narration, 

        case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.SaleLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        when isnull(SLoadingWOShort,0) > 0 then

        (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading 

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading end AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup,  CropSeason 

,SLoadingWOShort

HAVING      (ContractInvoice.SaleLoading > 0)



UNION ALL

/*---------------------*/











/* ------Sale Un-Loading Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58008' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.SaleUnLoading * (Sum(ContractInvoiceBody.BagsQty)+ Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        when isnull(SUnLoadingWOShort,0) > 0 then

        (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading 

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading end   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddUnLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleUnLoading, ContractInvoice.UnLoadingPerBag, IsApplyPartyMarkup,SUnLoadingWOShort,  CropSeason 

HAVING      (ContractInvoice.SaleUnLoading > 0)

 

UNION ALL 





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58008' AS AccountNo, ContractInvoice.Narration, 

                      case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.SaleUnLoading * (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        when isnull(SUnLoadingWOShort,0) > 0 then

        (Sum(ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading 

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading end AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusUnLoading,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleUnLoading, ContractInvoice.UnLoadingPerBag, IsApplyPartyMarkup,  CropSeason 

,SUnLoadingWOShort

HAVING      (ContractInvoice.SaleUnLoading > 0)

 

UNION ALL 



































/*--------------------*/







/* ------Sale WH-Tax  Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND( 

Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax

Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END,0)

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTax,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration,

 ROUND( 

 Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax

Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END,0)



AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTax,0) = 1)

And (Isnull(ContractInvoice.IsMultiNew,0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, 

SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg  

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTKg,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTKg,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)





UNION ALL

/*-----------------*/



/* ------Sale VehicalCharges  Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.VehicleChargesAcc,'')<> '' THEN ContractInvoice.VehicleChargesAcc ELSE '58010' END  AS AccountNo, ContractInvoice.Narration, 

                      ContractInvoice.VehicalCharges AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 1) AND IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup,ContractInvoice.VehicleChargesAcc,  CropSeason 

HAVING      (ContractInvoice.VehicalCharges > 0)



 

UNION ALL





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(ContractInvoice.VehicleChargesAcc,'')<> '' THEN ContractInvoice.VehicleChargesAcc ELSE '58010' END AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ContractInvoice.VehicalCharges AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

                      

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.AddVehicleCharges, 0) = 1) AND (ContractInvoice.IsShortInvoice = 0) AND 

                      (ISNULL(ContractInvoice.IsMultiNew, 0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, 

                      ContractInvoice.IsApplyPartyMarkup, ContractInvoice.VehicleChargesAcc,  CropSeason 

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL

/*------ Telephone, Insurance, Fule and Stitching Acc-------------------*/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58011' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ContractInvoice.AddTelePhoneExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  And (Isnull(ContractInvoice.AddTelePhoneExp,0) = 1) and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.AddTelePhoneExp, 

                      ContractInvoice.IsApplyPartyMarkup,  CropSeason 

HAVING      (ContractInvoice.AddTelePhoneExp > 0)















UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58012' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                       ContractInvoice.AddInsuranceExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 



FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddInsuranceExp,0) = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,  ContractInvoice.AddInsuranceExp, 

                      ContractInvoice.IsApplyPartyMarkup,  CropSeason 

HAVING      ( ContractInvoice.AddInsuranceExp > 0) AND ISNULL(ContractInvoice.AddInsuranceExp,0) > 0





UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58013' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddFuelExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddFuelExp,0) = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddFuelExp, 

                      ContractInvoice.IsApplyPartyMarkup,  CropSeason 

HAVING      ( ContractInvoice.AddFuelExp > 0)



UNION ALL









SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58014' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddStitchingExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddStitchingExp,0) = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddStitchingExp, 

                      ContractInvoice.IsApplyPartyMarkup,  CropSeason 

HAVING      ( ContractInvoice.AddStitchingExp > 0)



UNION ALL



----SALE Exp Stitching

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58014' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.Stitching, 0) < 0 then Abs(ISNULL(ContractInvoice.Stitching, 0)) Else 0 End  AS Debit,



Case When ISNULL(ContractInvoice.Stitching, 0) > 0 then ISNULL(ContractInvoice.Stitching, 0) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)   and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.Stitching, IsApplyPartyMarkup,  CropSeason 

HAVING      (ABS(ISNULL(ContractInvoice.Stitching, 0)) > 0)





UNION ALL

-----SALE Exp  Rent Godown

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58019' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.RentGodown, 0) < 0 then Abs(ISNULL(ContractInvoice.RentGodown, 0)) Else 0 End  AS Debit,

Case When ISNULL(ContractInvoice.RentGodown, 0) > 0 then ISNULL(ContractInvoice.RentGodown, 0) Else 0 End   AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.RentGodown, IsApplyPartyMarkup,  CropSeason 

HAVING      (ABS(ISNULL(ContractInvoice.RentGodown, 0)) > 0)





UNION ALL

------SALE Exp  ChokiDara

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58020' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ChokiDara, 0) < 0 then Abs(ISNULL(ContractInvoice.ChokiDara, 0)) Else 0 End  AS Debit,

Case When ISNULL(ContractInvoice.ChokiDara, 0) > 0 then ISNULL(ContractInvoice.ChokiDara, 0) Else 0 End   AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0



GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ChokiDara, IsApplyPartyMarkup,  CropSeason 

HAVING      (ABS(ISNULL(ContractInvoice.ChokiDara, 0)) > 0)

/************************************************************************/



UNION ALL

--- SALE Exp  ChartCharges

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58021' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ChartsCharges, 0) < 0 then Abs(ISNULL(ContractInvoice.ChartsCharges, 0)) Else 0 End    AS Debit,

Case When ISNULL(ContractInvoice.ChartsCharges, 0) > 0 then ISNULL(ContractInvoice.ChartsCharges, 0) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ChartsCharges, IsApplyPartyMarkup,  CropSeason 

HAVING      (ABS(ISNULL(ContractInvoice.ChartsCharges, 0)) <> 0)



UNION ALL

-----SALE Exp  Bag Usage

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58022' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.BagUsage, 0) < 0 then Abs(ISNULL(ContractInvoice.BagUsage, 0)) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.BagUsage, 0) > 0 then ISNULL(ContractInvoice.BagUsage, 0) Else 0 End  AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.BagUsage, IsApplyPartyMarkup,  CropSeason 

HAVING      (ABS(ISNULL(ContractInvoice.BagUsage, 0)) > 0)



UNION ALL

-------- SALE Exp  Shortage

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58023' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ShortageG, 0) < 0 then Abs(ISNULL(ContractInvoice.ShortageG, 0)) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.ShortageG, 0) > 0 then ISNULL(ContractInvoice.ShortageG, 0) Else 0 End  AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ShortageG, IsApplyPartyMarkup,  CropSeason 

HAVING      (ABS(ISNULL(ContractInvoice.ShortageG, 0)) > 0)



UNION ALL

---- SALE Exp  Tractor Exp

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58024' AS AccountNo, ContractInvoice.Narration,   

 Case When ISNULL(ContractInvoice.TractorExp, 0) < 0 then Abs(ISNULL(ContractInvoice.TractorExp, 0)) Else 0 End  AS Debit,

Case When ISNULL(ContractInvoice.TractorExp, 0) > 0 then ISNULL(ContractInvoice.TractorExp, 0) Else 0 End    AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.TractorExp, IsApplyPartyMarkup,  CropSeason 

HAVING      (ABS(ISNULL(ContractInvoice.TractorExp, 0)) > 0)



UNION ALL

/***********/



/* ------Pur Arhat Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4204' AS AccountNo, ContractInvoice.Narration, 

                      ROUND(CASE WHEN isnull(ContractInvoice.DMCommmission, 0) > 0 THEN (SUM(ContractInvoiceBody.TotalValue) 

                      + ROUND(SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees, 0) + ContractInvoice.CommissionValue) 

                      * ContractInvoice.PurArhat / 100 ELSE SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 END, 0)  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusArhat,0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, IsApplyPartyMarkup,ContractInvoice.SaleMarketFees, ContractInvoice.CommissionValue, ContractInvoice.DMCommmission,  CropSeason 



--HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4204' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ROUND(CASE WHEN isnull(ContractInvoice.DMCommmission, 0) > 0 THEN (SUM(ContractInvoiceBody.TotalValue) 

                      + ROUND(SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees, 0) + ContractInvoice.CommissionValue) 

                      * ContractInvoice.PurArhat / 100 ELSE SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 END, 0) AS Credit, 





                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.AddArhat, 0) = 1) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.SaleMarketFees, ContractInvoice.CommissionValue, ContractInvoice.DMCommmission,  CropSeason 

--HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL 

/* ------SALE-DAMI-With Holding Tax Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '2304' AS AccountNo, ContractInvoice.Narration, round((ContractInvoice.CommissionValue * Isnull(ContractInvoice.WHTDamiRatio, 0) / 100),0) AS Debit, 

                      0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

Where (ContractInvoice.IsSaleInvoice = 1)  and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.WHTDamiRatio,  CropSeason 

HAVING      Isnull(ContractInvoice.WHTDamiRatio, 0) > 0



UNION ALL

/*************** PRODUCT GENERAL OPENING ********************************************/



/****** PRODUCT OPENING GROSS VALUE DEBIT TO PRODUCT OPENING ACCOUNT  *********/



--SELECT     'OP' AS VOUCHERTYPE, 1 AS VoucherNo, '1/1/1900' AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

--                      AS ReconcileDate, CASE WHEN isnull(Products.PurAcNo, '') = '' THEN dbo.RegistryValue('ATOpeningAccount') 

--                      ELSE Products.PurAcNo END AS AccountNo, 'Opening Value' AS Narration, SUM(OpeningStock.TTLValue) AS Debit, 0 AS Credit, 

--                      1 AS IsApplyPartyMarkup

--FROM         OpeningStock INNER JOIN

--                      Products ON OpeningStock.ProductID = Products.ProductID

--Group by Products.PurAcNo

--UNION ALL

--

--

--

--

--

--/****** PRODUCT PURCHASE SALESTAX VALUE DEBIT TO PRODUCT PURCHASE STAX ACCOUNT  *********/

--

--SELECT     'OP' AS VOUCHERTYPE, 1 AS VoucherNo, 

--

--                      dbo.DefaultMinDate() AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

--

--                      AS ReconcileDate, dbo.RegistryValue('ATOpeningSTaxAccount') AS AccountNo, 'Opening STax' AS Narration, 

--

--                      SUM(TTLSalesTax) AS Debit, 

--

--              0 AS Credit, 1 as IsApplyPartyMarkup

--

--FROM OpeningStock

--

--UNION ALL 

/*************** PRODUCT GENERAL PURCHASE ********************************************/



/****** PRODUCT PURCHASE GROSS VALUE DEBIT TO PRODUCT PURCHASE ACCOUNT  *********/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, case when ISNULL(Products.PurAcNo, '') = '' then dbo.RegistryValue('ATPurchaseAccount') else Products.PurAcNo end 

 AS AccountNo, ISNULL(Purchase.Narration, ' ') AS Narration, 

                      SUM(PurchaseBody.TTLValue)

                      --+ SUM(ISNULL(vl,0)) 

                      AS Debit, 0 AS Credit, ISNULL(Purchase.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup, CropSeason 

FROM         PurchaseBody INNER JOIN

                      Purchase ON PurchaseBody.PurchaseID = Purchase.PurchaseID INNER JOIN



                      Products ON PurchaseBody.ProductID = Products.ProductID

                      LEFT OUTER JOIN ((SELECT   MultiBody.PurchaseID, MultiBody.pRODUCTID, CASE WHEN SUM(ISNULL(MultiBody.TPTM,0)) > 0 THEN SUM(ISNULL(MultiBody.TPTM,0)) END AS VL

                      

FROM         MultiBody INNER JOIN

                      Purchase ON MultiBody.PurchaseID = Purchase.PurchaseID

WHERE 1=1 --AND ISNULL(MultiBody.IsVL,0) = 1

GROUP BY MultiBody.IsVL,MultiBody.PurchaseID, MultiBody.pRODUCTID)) MI ON MI.PurchaseID = Purchase.PurchaseID AND MI.pRODUCTID = PurchaseBody.ProductID

WHERE     (Purchase.IsPosted = 1)

GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.IsApplyPartyMarkup, Products.PurAcNo, CropSeason



UNION ALL



/****** PRODUCT PURCHASE SALESTAX VALUE DEBIT TO PRODUCT PURCHASE STAX ACCOUNT  *********/





SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseSTaxAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      SUM(PurchaseBody.TTLSalesTax) AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM PurchaseBody INNER JOIN

















    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, IsApplyPartyMarkup, CropSeason



UNION ALL



/******* PRODUCT PURCHASE EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpMinusAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      0 AS Debit, Purchase.ExpMinus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Purchase 



WHERE     Purchase.IsPosted = 1



UNION ALL



/******* PRODUCT PURCHASE EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpPlusAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      Purchase.ExpPlus AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Purchase 



WHERE     Purchase.IsPosted = 1



UNION ALL



/********** PRODUCT PURCHASE COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/







SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseComAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax) * IsNull(Purchase.ComRatio,0) / 100 AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.ComRatio, IsApplyPartyMarkup, CropSeason



UNION ALL



/*********** PRODUCT PURCHASE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, Purchase.VendorID AS AccountNo, IsNull(Purchase.Narration,' ') +', '+ ISNULL(NarrationuRDU,N'') AS Narration, 



                      Purchase.PaidAmount + SUM(ISNULL(vl,0)) AS Debit, 



              SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax)



                      - ISNULL(Purchase.ExpMinus, 0) 



                      + ISNULL(Purchase.ExpPlus, 0) 



              + (SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax) * IsNull(Purchase.ComRatio,0) / 100)

              --+ SUM(ISNULL(vl,0)) 

              + (case when ISNULL(Purchase.IsFreightPlus,0) = 1 THEN ISNULL(Purchase.Freight,0) ELSE -1 * ISNULL(Purchase.Freight,0) END)

               AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID

 LEFT OUTER JOIN ((SELECT   MultiBody.PurchaseID, MultiBody.pRODUCTID, CASE WHEN ISNULL(MultiBody.IsVLG,0) = 1 OR SUM(ISNULL(MultiBody.TPTM,0)) > 0 THEN  SUM(ISNULL(MultiBody.TPTEx,0)+ISNULL(MultiBody.TPTM,0)) ELSE 0 END AS VL

                      

FROM         MultiBody INNER JOIN

                      Purchase ON MultiBody.PurchaseID = Purchase.PurchaseID

WHERE 1=1 --AND ISNULL(MultiBody.IsVL,0) = 1

GROUP BY MultiBody.IsVLG,MultiBody.IsVL,MultiBody.PurchaseID, MultiBody.pRODUCTID)) MI ON MI.PurchaseID = Purchase.PurchaseID AND MI.pRODUCTID = PurchaseBody.ProductID

WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.ComRatio, Purchase.VendorID, Purchase.PaidAmount, Purchase.ExpMinus, Purchase.ExpPlus, IsApplyPartyMarkup,NarrationuRDU,Purchase.IsFreightPlus,Purchase.Freight

, CropSeason

UNION ALL



/******* PRODUCT PURCHASE PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      0 AS Debit, Purchase.PaidAmount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Purchase 



WHERE     Purchase.IsPosted = 1



/******* PRODUCT PURCHASE BANK CHARGES CREDIT TO BANK CHARGES ACCOUNT 52102 ************/

UNION ALL

SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '52102' AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      0 AS Debit, ISNULL(Purchase.BankTax,0) AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Purchase 



WHERE     Purchase.IsPosted = 1

/******* PRODUCT PURCHASE FREIGHT CHARGES TO FREIGHT CHARGES ACCOUNT 52102 ************/

UNION ALL

SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '52061' AS AccountNo, 'PURCHASE FREIGHT CHARGES ' + IsNull(Purchase.Narration,'') AS Narration, 



                      (case when ISNULL(Purchase.IsFreightPlus,0) = 1 THEN ISNULL(Purchase.Freight,0) ELSE 0 END) AS Debit, (case when ISNULL(Purchase.IsFreightPlus,0) = 0 THEN ISNULL(Purchase.Freight,0) ELSE 0 END) AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Purchase 



WHERE     Purchase.IsPosted = 1

/********************* END PRODUCT GENERAL PURCHASE *************************************/

UNION ALL

/******************** PRODUCT GENERAL PURCHASE RETURN *************************************/

/****** PRODUCT PURCHASERETURN GROSS VALUE CREDIT TO PRODUCT PURCHASERETURN ACCOUNT  *********/





SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, case when ISNULL(Products.PurAcNo, '') = '' then dbo.RegistryValue('ATPurchaseReturnAccount') else Products.PurAcNo end 

 AS AccountNo, 

                      ISNULL(PurReturns.Narration, ' ') AS Narration, 0 AS Debit, SUM(PurReturnsBody.TTLValue) AS Credit, ISNULL(PurReturns.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason 

FROM         PurReturnsBody INNER JOIN

                      PurReturns ON PurReturnsBody.PurReturnID = PurReturns.PurReturnID INNER JOIN

                      Products ON PurReturnsBody.ProductID = Products.ProductID

WHERE     (PurReturns.IsPosted = 1)

GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.IsApplyPartyMarkup, Products.PurAcNo, CropSeason



UNION ALL



/****** PRODUCT PURCHASERETURN STAX VALUE CREDIT TO PRODUCT PURCHASERETURNSTAX ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnSTAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(PurReturnsBody.TTLSalesTax) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, IsApplyPartyMarkup, CropSeason



UNION ALL



/****** PRODUCT PURCHASERETURN EXPMINUS DEBIT TO PRODUCT EXPMINUS ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnExpMinusAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      PurReturns.ExpMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM PurReturns 



WHERE     PurReturns.IsPosted = 1







UNION ALL



/****** PRODUCT PURCHASERETURN EXPPLUS CREDIT TO PRODUCT EXPPLUS ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnExpPlusAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              PurReturns.ExpPlus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM PurReturns 









WHERE     PurReturns.IsPosted = 1



UNION ALL



/****** PRODUCT PURCHASERETURN COMMISSION VALUE CREDIT TO PRODUCT PURCHASERETURN COMMISSON ACCOUNT  *********/ 

SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnComAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax) * IsNull(PurReturns.ComRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.ComRatio, IsApplyPartyMarkup, CropSeason



UNION ALL



/*********** PRODUCT PURCHASERETURN NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 

                       PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, PurReturns.VendorID AS AccountNo, IsNull(PurReturns.Narration,' ')+', '+ ISNULL(NarrationuRDU,N'')  AS Narration, 



              SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax)



                      - ISNULL(PurReturns.ExpMinus, 0) 

                       + ISNULL(PurReturns.ExpPlus, 0) 



              + (SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax) * IsNull(PurReturns.ComRatio,0) / 100) AS Debit,







                      PurReturns.ReceivedAmount AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.ComRatio, PurReturns.ExpPlus, PurReturns.ExpMinus, PurReturns.VendorID, PurReturns.ReceivedAmount, IsApplyPartyMarkup,NarrationuRDU

, CropSeason

UNION ALL



/****** PRODUCT PURCHASERETURN RECEIVED AMOUNT DEBIT TO CASH ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      PurReturns.ReceivedAmount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM PurReturns 







WHERE     PurReturns.IsPosted = 1



/********************* END PRODUCT GENERAL PURCHASE RETURN *************************************/

UNION ALL

/********************* PRODUCT GENERAL SALES *************************************/

/****** PRODUCT Sales GROSS VALUE CREDIT TO PRODUCT Sales ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, 







case when ISNULL(Products.SaleAcNo,'') = '' then dbo.RegistryValue('ATSalesAccount') else Products.SaleAcNo end AS AccountNo, ISNULL(Sales.Narration, ' ') AS Narration, 0 AS Debit, 

                      round(SUM(SalesBody.TTLValue),0)

                      

                     --+SUM(ISNULL(MI.VL,0))

                       AS Credit, ISNULL(Sales.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

                       

FROM         SalesBody INNER JOIN

                      Sales ON SalesBody.SaleID = Sales.SaleID INNER JOIN

                      Products ON SalesBody.ProductID = Products.ProductID





                      

                      LEFT OUTER JOIN ((SELECT   MultiBody.SaleID, MultiBody.pRODUCTID, CASE WHEN ISNULL(MultiBody.IsVL,0) = 0 OR SUM(ISNULL(MultiBody.TPTM,0)) > 0 THEN SUM(ISNULL(MultiBody.TPTEx,0)+ISNULL(MultiBody.TPTM,0)) ELSE 0 END AS VL

                      

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID

WHERE 1=1 --AND ISNULL(MultiBody.IsVL,0) = 1

GROUP BY MultiBody.IsVL,MultiBody.SaleID, MultiBody.pRODUCTID)) MI ON MI.SaleID = Sales.SaleID AND MI.pRODUCTID = SalesBody.ProductID

WHERE     (Sales.IsPosted = 1)

GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.IsApplyPartyMarkup, Products.SaleAcNo, CropSeason



UNION ALL



/****** PRODUCT Sales STAX VALUE CREDIT TO PRODUCT SalesSTAX ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                       AS ReconcileDate, dbo.RegistryValue('ATSalesSTaxAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(SalesBody.TTLSalesTax) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup, CropSeason





UNION ALL



/****** PRODUCT Sales EXPMINUS DEBIT TO PRODUCT EXPMINUS ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpMinusAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.ExpMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Sales 



WHERE     Sales.IsPosted = 1



UNION ALL

 /****** PRODUCT Sales EXPPLUS CREDIT TO PRODUCT EXPPLUS ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpPlusAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 

                       0 AS Debit, 



              Sales.ExpPlus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Sales 



WHERE     Sales.IsPosted = 1



UNION ALL



/****** PRODUCT Sales COMMISSION VALUE CREDIT TO PRODUCT Sales COMMISSON ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesComAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, IsApplyPartyMarkup, CropSeason



UNION ALL



/****** PRODUCT Sales SpecialDiscount DEBIT TO PRODUCT SALES SpecialDiscount ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, dbo.RegistryValue('ATSalesSpecialDiscountAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.SpecialDiscount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Sales 







WHERE     Sales.IsPosted = 1



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, Sales.CustomerID AS AccountNo, ISNULL(Sales.Narration, ' ') + ', ' + ISNULL(Sales.NarrationUrdu, N'') 

                      AS Narration, CASE WHEN isnull(Sales.CustomerIDCom, 0) = 0 THEN SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) - ISNULL(Sales.ExpMinus, 

                      0) + ISNULL(Sales.ExpPlus, 0) + (SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio, 0) / 100) 

                      ELSE (SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) - ISNULL(Sales.ExpMinus, 0) + ISNULL(Sales.ExpPlus, 0) 

                      + (SUM(SalesBody.TTLValue + SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio, 0) / 100)) * (100 - Sales.CustomerComRatio) 

                      / 100 END --+ SUM(isnull(MI.VL,0)) 

                      + (case when ISNULL(Sales.IsFreightPlus,0) = 1 THEN ISNULL(Sales.Freight,0) ELSE -1 * ISNULL(Sales.Freight,0) END)

                      AS Debit, 

                      Sales.ReceivedAmount + Sales.SpecialDiscount + SUM(isnull(MI.VL,0)) AS Credit, 

                      ISNULL(Sales.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup,  CropSeason 

FROM         SalesBody INNER JOIN

                      Sales ON SalesBody.SaleID = Sales.SaleID LEFT OUTER JOIN

                          (SELECT   MultiBody.SaleID, MultiBody.pRODUCTID, SUM(ISNULL(MultiBody.TPTEx,0)+ISNULL(MultiBody.TPTM,0)) AS VL

                      

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID

WHERE 1=1 AND ISNULL(MultiBody.IsVL,0) = 0 and  ISNULL(MultiBody.IsVLG,0) = 0

GROUP BY MultiBody.SaleID, MultiBody.pRODUCTID

having SUM(ISNULL(MultiBody.TPTEx,0)+ISNULL(MultiBody.TPTM,0)) > 0

) AS MI ON SalesBody.ProductID = MI.ProductID AND MI.SaleID = Sales.SaleID

WHERE     (Sales.IsPosted = 1)

GROUP BY Sales.SaleID, Sales.CustomerIDCom, Sales.CustomerComRatio, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, Sales.ExpPlus, 

                      Sales.ExpMinus, Sales.CustomerID, Sales.ReceivedAmount, Sales.SpecialDiscount, Sales.IsApplyPartyMarkup, Sales.NarrationUrdu, 

                      Sales.PurchaseID,Sales.IsFreightPlus,Sales.Freight, CropSeason

                      

UNION ALL



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, Sales.CustomerIDCom AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax)



                      - ISNULL(Sales.ExpMinus, 0) 



                      + ISNULL(Sales.ExpPlus, 0) 



              + (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100)) * Sales.CustomerComRatio / 100 



AS Debit

,Sales.ReceivedAmount AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 





FROM SalesBody INNER JOIN

    Sales ON 



    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1 and isnull(Sales.CustomerIDCom,0)<>0



GROUP BY Sales.SaleID, Sales.CustomerIDCom, Sales.CustomerComRatio, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, Sales.ExpPlus, Sales.ExpMinus, Sales.CustomerID, Sales.ReceivedAmount, Sales.SpecialDiscount, IsApplyPartyMarkup

, CropSeason

UNION ALL



/****** PRODUCT Sales RECEIVEDAMOUNT DEBIT TO CASH ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo,  

                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.ReceivedAmount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Sales 



WHERE     Sales.IsPosted = 1

UNION ALL



/****** PRODUCT Sales FREIGHT TO FREIGHT ACCOUNT  *********/





SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo,  

                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '52061' AS AccountNo,'SALE FREIGHT CHARGES ' + IsNull(Sales.Narration,'')  AS Narration, 



























                      (case when ISNULL(Sales.IsFreightPlus,0) = 0 THEN ISNULL(Sales.Freight,0) ELSE 0 END) AS Debit, 



              (case when ISNULL(Sales.IsFreightPlus,0) = 1 THEN ISNULL(Sales.Freight,0) ELSE 0 END) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM Sales 



WHERE     Sales.IsPosted = 1

UNION ALL

/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleAccount') AS AccountNo, Sales.Narration AS Narration, 



              SUM(SalesBody.Cost*((SalesBody.Qty+SalesBody.Bonus))) AS Debit,



                      0 AS Credit, 0 AS IsApplyPartyMarkup ,  CropSeason 

 FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID







WHERE     Sales.IsPosted = 1  AND Sales.SaleID > 0



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, CropSeason



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSoldStockValueAccount') AS AccountNo, Sales.Narration AS Narration, 



              0 AS Debit,



                      SUM(SalesBody.Cost*((SalesBody.Qty+SalesBody.Bonus))) AS Credit, 0 AS IsApplyPartyMarkup ,  CropSeason 

 FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID







WHERE     Sales.IsPosted = 1  AND Sales.SaleID > 0



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, CropSeason

UNION ALL

/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/



SELECT    'MI' AS VOUCHERTYPE, MultiBody.MultiID, MultiBody.MDate

, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              SUM(MultiBody.AmountP) / SUM(MultiBody.NWeight) * SUM(MultiBody.NWeight) AS Debit, 0 Credit, 0 IsApplyPartyMarkup,  CropSeason 

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID

WHERE     (1 = 1)

GROUP BY MultiBody.MultiID, MultiBody.MDatE, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup, CropSeason



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/ 

SELECT    'MI' AS VOUCHERTYPE, MultiBody.MultiID, MultiBody.MDate

, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSoldStockValueAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              0 AS Debit, SUM(MultiBody.AmountP) / SUM(MultiBody.NWeight) * SUM(MultiBody.NWeight) Credit,0 IsApplyPartyMarkup,  CropSeason 

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID

WHERE     (1 = 1)

GROUP BY MultiBody.MultiID, MultiBody.MDatE, Sales.IsPosted, Sales.Narration,  IsApplyPartyMarkup, CropSeason







/********************* END PRODUCT GENERAL SALES *************************************/

UNION ALL

/********************* PRODUCT GENERAL SALES RETURN *************************************/



/****** PRODUCT SaleReturns GROSS VALUE DEBIT TO PRODUCT SaleReturns ACCOUNT  *********/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 



case when ISNULL(Products.SaleAcNo,'') = '' then dbo.RegistryValue('ATSaleReturnsAccount') else Products.SaleAcNo end 

AS AccountNo, 

                      ISNULL(SaleReturns.Narration, ' ') AS Narration, SUM(SaleReturnsBody.TTLValue) AS Debit, 0 AS Credit, ISNULL(SaleReturns.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup,  CropSeason 

FROM         SaleReturnsBody INNER JOIN

                      SaleReturns ON SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID INNER JOIN

                      Products ON SaleReturnsBody.ProductID = Products.ProductID

WHERE     (SaleReturns.IsPosted = 1)

GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.IsApplyPartyMarkup, 



                      Products.SaleAcNo, CropSeason

UNION ALL



/****** PRODUCT SaleReturns SALESTAX VALUE DEBIT TO PRODUCT SaleReturns STAX ACCOUNT  *********/







SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsSTAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SUM(SaleReturnsBody.TTLSalesTax) AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup, CropSeason



UNION ALL



/******* PRODUCT SaleReturns EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsExpMinusAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      0 AS Debit, SaleReturns.ExpMinus AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/******* PRODUCT SaleReturns EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsExpPlusAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SaleReturns.ExpPlus AS Debit, 0 AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL





/********** PRODUCT SaleReturns COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 





                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsComAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 







                      SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax) * IsNull(SaleReturns.ComRatio,0) / 100 AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.ComRatio, IsApplyPartyMarkup

, CropSeason

UNION ALL



/******* PRODUCT SaleReturns PAIDAMOUNT CREDIT TO SALESRETURNS SPECIALDISOUNT ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsSpecialDiscountAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 





                      0 AS Debit, SaleReturns.SpecialDiscount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 













                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, SaleReturns.CustomerID AS AccountNo, IsNull(SaleReturns.Narration,' ')+', '+ISNULL(NarrationuRDU,N'')  AS Narration, 



                      SaleReturns.PaidAmount+SaleReturns.SpecialDiscount AS Debit, 



              SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax)



                      - ISNULL(SaleReturns.ExpMinus, 0) 



                      + ISNULL(SaleReturns.ExpPlus, 0) 







              + (SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax) * IsNull(SaleReturns.ComRatio,0) / 100) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.ComRatio, SaleReturns.CustomerID, SaleReturns.PaidAmount, SaleReturns.ExpMinus, SaleReturns.ExpPlus, SaleReturns.SpecialDiscount, IsApplyPartyMarkup,NarrationuRDU

, CropSeason

UNION ALL



/******* PRODUCT SaleReturns PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 















                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      0 AS Debit, SaleReturns.PaidAmount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT DEBIT TO COSTOFGOODSaleReturns ACCOUNT *************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleReturnAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



              0 AS Debit,



                      SUM(SaleReturnsBody.Cost*((SaleReturnsBody.Qty+SaleReturnsBody.Bonus)*SaleReturnsBody.Multiplier)) AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                      ,  CropSeason 

FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup

, CropSeason

UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT DEBIT TO COSTOFGOODSaleReturns ACCOUNT *************/





SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnStockValueAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



              SUM(SaleReturnsBody.Cost*((SaleReturnsBody.Qty+SaleReturnsBody.Bonus)*SaleReturnsBody.Multiplier)) AS Debit,



                      0 AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                      ,  CropSeason 

FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1







GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup

, CropSeason



    /********************* END PRODUCT GENERAL SALES RETURN *************************************/









UNION ALL

/********************* CONTRACT INVOICE SALES *************************************/

/****** CONTRACT Sales GROSS VALUE CREDIT TO Sales ACCOUNT  *********/

SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate,



CASE WHEN ISNULL(Products.SaleAcNo,'') = '' THEN  dbo.RegistryValue('ATSalesAccount') ELSE Products.SaleAcNo END  AS AccountNo, 

                      ISNULL(ContractInvoice.Narration, '') AS Narration, 0 AS Debit, ROUND(SUM(ContractInvoiceBody.TotalValue),0) AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup, CropSeason 

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId INNER JOIN

                      Products ON ContractInvoiceBody.ProductID = Products.ProductID

WHERE     (ContractInvoice.IsSaleInvoice = 1) AND (dbo.RegistryValue('StopApplyGrossToPartyAccount') = 0) and ISNULL(IsMultiNew,0) = 0

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice, ContractInvoice.Narration, ContractInvoice.Received, 

                      ContractInvoice.IsApplyPartyMarkup, Products.SaleAcNo, CropSeason 

UNION ALL

/*************** BARDANA VALUE AS CREDIT TO SALE BARDANA ACCOUNT ********************************************/

    SELECT 'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, 

    NULL AS DepositedDate, NULL AS ReconcileDate,  '415' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

        0 AS Debit, round(Sum(Isnull(ContractInvoiceBody.PQty,0) * ContractInvoiceBody.Bardana) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana),0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

    , CropSeason FROM ContractInvoiceBody INNER JOIN  ContractInvoice ON ContractInvoiceBody.ContractPurID = ContractInvoice.ContractPurID



    WHERE ContractInvoice.IsSaleInvoice=1 and isnull(ContractInvoice.isBardana,1) = 1 and dbo.RegistryValue('StopApplyGrossToPartyAccount')=0 and ISNULL(IsMultiNew,0) = 0

    

    GROUP BY ContractInvoice.ContractPurID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice,ContractInvoice.Narration, IsApplyPartyMarkup, CropSeason 



HAVING  Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana)  > 0 or Sum(Isnull(ContractInvoiceBody.PQty,0) * ContractInvoiceBody.Bardana) > 0



UNION ALL

/**************** CONTRACT SALE INVOICE EXP PLUS AS CREDIT TO SALES EXP PLUS  ACCOUNT  *********/

SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpPlusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                       0 AS Debit, 



              Isnull(ContractInvoice.OtherExpencePlus,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 AND  Isnull(ContractInvoice.OtherExpencePlus,0) > 0 and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0







UNION ALL

/**************** CONTRACT SALE INVOICE TELEPHONE,INSURANCE,FUEL,STITCHING EXP Credit To  ACCOUNTs  *********/



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58011' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                     case when Isnull(ContractInvoice.AddTelePhoneExp,0) < 0 then  abs(Isnull(ContractInvoice.AddTelePhoneExp,0)) else 0 end AS Debit, 



              case when Isnull(ContractInvoice.AddTelePhoneExp,0) > 0 then  Isnull(ContractInvoice.AddTelePhoneExp,0) else 0 end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

              , CropSeason 

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 AND Isnull(ContractInvoice.AddTelePhoneExp,0) <> 0 and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 





                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58012' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 



              Isnull(ContractInvoice.AddInsuranceExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

              , CropSeason 

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 AND ISNULL(ContractInvoice.AddInsuranceExp,0) > 0 and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58013' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 



              Isnull(ContractInvoice.AddFuelExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

              , CropSeason 

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 AND Isnull(ContractInvoice.AddFuelExp,0) > 0 and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58014' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 







              Isnull(ContractInvoice.AddStitchingExp,0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

              , CropSeason 

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 AND Isnull(ContractInvoice.AddStitchingExp,0)   > 0 and IsShortInvoice = 0 and ISNULL(IsMultiNew,0) = 0







/*************************************************************************************************************/





UNION ALL

/**************** CONTRACT SALE INVOICE EXP MINUS AS DEBIT TO SALES EXP MINUS  ACCOUNT  *********/





SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpMinusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      ContractInvoice.OtherExpenceMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

              , CropSeason 

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 AND IsNULL(ContractInvoice.OtherExpenceMinus,0) > 0 and ISNULL(IsMultiNew,0) = 0





UNION ALL





/*********** CONTRACT SALE Invoice NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/

SELECT 'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



    AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, isnull(ContractInvoice.SellerID,ContractInvoice.PartyID) AS AccountNo, 

    

    IsNull(ContractInvoice.Narration,'')+', '+ ISNULL(NarrationUrdu,N'') AS Narration,     

ROUND(ISNULL(ContractInvoice.NetValue -(case when dbo.RegistryValue('StopApplyGrossToPartyAccount')=1 then 



SUM(ContractInvoiceBody.TotalValue) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) else 0 end), 0) 

+ ContractInvoice.Received ,0 ) 

+ 

ISNULL((CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges,0)  = 1 THEN 

(case when ContractInvoice.AddVehicleCharges  = 1 then -VehicalCharges when MinusVehicleCharges = 1 then VehicalCharges end)

else

0 

end),0)

 AS Debit, ContractInvoice.Received As Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 , CropSeason 

    FROM ContractInvoiceBody INNER JOIN ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId



    WHERE (ContractInvoice.IsSaleInvoice = 1)  AND (ISNULL(ContractInvoice.IsMultiNew, 0) = 0)

    GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.SellerID, ContractInvoice.Received, 



    ContractInvoice.Narration, ContractInvoice.NetValue, IsApplyPartyMarkup,ContractInvoice.PartyID,NarrationUrdu

    ,ContractInvoice.SepVehicalCharges,ContractInvoice.AddVehicleCharges  ,VehicalCharges, CropSeason 

    ,MinusVehicleCharges



UNION ALL

SELECT 'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



    AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, isnull(ContractInvoice.SellerID,ContractInvoice.PartyID) AS AccountNo, 

    

    'Vehicle Charges: '+ IsNull(ContractInvoice.Narration,'')+', '+ ISNULL(NarrationUrdu,N'') AS Narration,     

(CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges,0)  = 1 THEN 

(case when ContractInvoice.AddVehicleCharges  = 1 then VehicalCharges else 0 end)

else

0 

end)

 AS Debit, (CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges,0)  = 1 THEN 

(case when ContractInvoice.MinusVehicleCharges  = 1 then VehicalCharges else 0 end)

else

0 

end) As Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

, CropSeason 

    FROM ContractInvoiceBody INNER JOIN ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId



    WHERE (ContractInvoice.IsSaleInvoice = 1) and ISNULL(ContractInvoice.SepVehicalCharges,0)  = 1 --and ISNULL(IsMultiNew,0) = 0

    GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.SellerID, ContractInvoice.Received, 



    ContractInvoice.Narration, ContractInvoice.NetValue, IsApplyPartyMarkup,ContractInvoice.PartyID,NarrationUrdu

    ,ContractInvoice.SepVehicalCharges,ContractInvoice.AddVehicleCharges  ,VehicalCharges

    ,MinusVehicleCharges, CropSeason 



UNION ALL

/********************* CONTRACT Sales Invoice RECEIVEDAMOUNT as DEBIT TO CASH ACCOUNT  *********/



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, 





                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo,IsNull(ContractInvoice.Narration ,'') AS Narration, 



                      ContractInvoice.Received AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

              , CropSeason 

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1  AND ISNULL(ContractInvoice.Received,0) > 0



/********************* END CONTRACT INVOICE SALES**********************/
GO

-- View: VuAccVouchersOthersCrop
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersCrop]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersCrop];
GO
CREATE VIEW [dbo].[VuAccVouchersOthersCrop]  AS 



/*************** CROPS PURCHASE ********************************************/



/****** CROP PURCHASE GROSS VALUE DEBIT TO CROP PURCHASE ACCOUNT  *********/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, CropsPurchaseHeader.PurchaseDate AS VoucherDate, 

                      '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, case when ISNULL(Products.PurAcNo, '') = '' then dbo.RegistryValue('ATCropPurchaseAccount') else Products.PurAcNo end 

                      AS AccountNo, ISNULL(CropsPurchaseHeader.Remarks, ' ') +', '+ ISNULL(NarrationUrdu,N'') AS Narration, 

                      ROUND(SUM(CropsPurchaseBody.Weight / CropsWeighments.Multiplier * CropsPurchaseBody.Price),0) AS Debit,  0 AS Credit, 

                      ISNULL(CropsPurchaseHeader.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup, CropSeason 

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     (CropsPurchaseHeader.IsPosted = 1)

GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.Remarks, 

                      CropsPurchaseHeader.IsApplyPartyMarkup, Products.PurAcNo,NarrationUrdu, CropSeason 

UNION ALL



/******* CROP PURCHASE EXPMINUS CREDIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      case when ISNULL(Products.ExpAcc, '') = '' then dbo.RegistryValue('ATCropPurchaseExpMinusAccount') else Products.ExpAcc end 

                       AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ')  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.ExpMinus 



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN



                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID





WHERE     CropsPurchaseHeader.IsPosted = 1 AND (CropsPurchaseHeader.ExpMinus) <> 0

UNION ALL



/******* CROP PURCHASE OtherExp CREDIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropPurchaseExpMinusAccount') 

                       AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ')  AS Narration, 0 AS Debit, 



                      ISNULL(CropsPurchaseHeader.OtherExp,0)



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN



                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID





WHERE     CropsPurchaseHeader.IsPosted = 1 AND (ISNULL(CropsPurchaseHeader.OtherExp,0)) <> 0

UNION ALL



/******* CROP PURCHASE EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      case when ISNULL(Products.ExpAcc, '') = '' then dbo.RegistryValue('ATCropPurchaseExpPlusAccount') else Products.ExpAcc end AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



                      CropsPurchaseHeader.ExpPlus 

                       AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1

UNION ALL



/******* CROP PURCHASE MillExp CREDIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropPurchaseExpPlusAccount') AS AccountNo, 'Crop Mill Expense: ' AS Narration, 



                     0 

                       AS Debit, ISNULL(CropsPurchaseHeader.MillExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.MillExp,0) <> 0

UNION ALL



/******* CROP PURCHASE PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ')  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.PaidAmount



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1



UNION ALL



/*************** BARDANA VALUE AS DEBIT TO PURCHASE BARDANA ACCOUNT ********************************************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '11327' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,'Crop Purchase Bags ')  AS Narration, ISNULL(CropsPurchaseHeader.BagAmount,0) AS Debit, 



                      0



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.BagAmount,0) <> 0

UNION ALL



/********** CROP PURCHASE COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, CropsPurchaseHeader.PurchaseDate AS VoucherDate, 

                      '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 

                      case when ISNULL(Products.CommAcc, '') = '' then dbo.RegistryValue('ATCropPurchaseComAccount') else Products.CommAcc end --dbo.RegistryValue('ATCropPurchaseComAccount') 

                      AS AccountNo, ISNULL(CropsPurchaseHeader.Remarks, ' ') AS Narration, CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN ISNULL(ComAmount, 0) 

                      ELSE ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) * IsNull(CropsPurchaseHeader.ComRatio, 0) 

                      / 100) END AS Debit, 0 AS Credit, ISNULL(CropsPurchaseHeader.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup, CropsPurchaseHeader.CropSeason 

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN



                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID

                      INNER JOIN Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     (CropsPurchaseHeader.IsPosted = 1)

GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.ComRatio, 

                      CropsPurchaseHeader.Remarks, CropsPurchaseHeader.IsApplyPartyMarkup, CropsPurchaseHeader.ComAmount, Products.CommAcc, CropsPurchaseHeader.ComPerMound

                       , CropsPurchaseHeader.CropSeason 

UNION ALL



/*********** CROP PURCHASE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, CropsPurchaseHeader.PurchaseDate AS VoucherDate, 

                      '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CropsPurchaseHeader.VendorID AS AccountNo, 

                      ISNULL(CropsPurchaseHeader.Remarks, ' ')+', '+ ISNULL(NarrationUrdu,N'') AS Narration, ISNULL(CropsPurchaseHeader.PaidAmount, 0) AS Debit, 

                     round( CASE WHEN isnull(CropsPurchaseHeader.VendorIDCom, 0) = 0 THEN (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) 

                      * CropsPurchaseBody.Price)) - ISNULL(CropsPurchaseHeader.ExpMinus, 0) + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 

                      + (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN iSNULL(ComAmount, 0) ELSE (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) 

                      * CropsPurchaseBody.Price) * IsNull(CropsPurchaseHeader.ComRatio, 0) / 100) END) 

                      ELSE ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 

                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) + (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN iSNULL(ComAmount, 0) 

                      ELSE (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price) * IsNull(CropsPurchaseHeader.ComRatio, 0) 

                      / 100) END)) * (100 - CropsPurchaseHeader.VendorComRatio) / 100 END ,0)

-ROUND((ISNULL(CropsPurchaseHeader.MillExp,0)+ ISNULL(CropsPurchaseHeader.OtherExp,0)+ ISNULL(CropsPurchaseHeader.FreightExp,0)+ ISNULL(CropsPurchaseHeader.KantaExp,0)+ ISNULL(CropsPurchaseHeader.BrokerageExp,0)+ ISNULL(CropsPurchaseHeader.MarketFeesExp,0)),0)

+ISNULL(CropsPurchaseHeader.BagAmount,0)                      

                      AS  Credit, ISNULL(CropsPurchaseHeader.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup, CropsPurchaseHeader.CropSeason 

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID

WHERE     (CropsPurchaseHeader.IsPosted = 1)



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.ComAmount, CropsPurchaseHeader.ComPerMound, 

                      CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.IsApplyPartyMarkup, 

                      CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio, 

                      CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks, CropsPurchaseHeader.PaidAmount,NarrationUrdu

, CropsPurchaseHeader.MillExp, CropsPurchaseHeader.OtherExp, CropsPurchaseHeader.FreightExp, CropsPurchaseHeader.KantaExp, CropsPurchaseHeader.BrokerageExp, CropsPurchaseHeader.MarketFeesExp, CropsPurchaseHeader.CropSeason 

,CropsPurchaseHeader.BagAmount

UNION ALL





SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, CropsPurchaseHeader.VendorIDCom AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                   round(   ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 



                    - ISNULL(CropsPurchaseHeader.PaidAmount, 0) 

            



                      + (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price))



                      * IsNull(CropsPurchaseHeader.ComRatio,0) / 100) * CropsPurchaseHeader.VendorComRatio / 100  ,0) AS Credit



, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

, CropsPurchaseHeader.CropSeason 

FROM CropsPurchaseBody INNER JOIN

    CropsPurchaseHeader ON 



    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

     INNER JOIN



    CropsWeighments ON 

    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID



WHERE     CropsPurchaseHeader.IsPosted = 1 and isnull(CropsPurchaseHeader.VendorIDCom,0) <> 0



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted, IsApplyPartyMarkup



              ,CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio 



              , CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks,CropsPurchaseHeader.PaidAmount

              , CropsPurchaseHeader.CropSeason 

UNION ALL



/******* CROP PURCHASE BrokerageExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      '57017' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



CASE WHEN ISNULL(CropsPurchaseHeader.BrokerageExp,0) < 0 THEN ABS(ISNULL(CropsPurchaseHeader.BrokerageExp,0)) ELSE 0 END

                       AS Debit, CASE WHEN ISNULL(CropsPurchaseHeader.BrokerageExp,0) > 0 THEN ISNULL(CropsPurchaseHeader.BrokerageExp,0) ELSE 0 END AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

, CropsPurchaseHeader.CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.BrokerageExp,0) <> 0



UNION ALL



/******* CROP PURCHASE KantaExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      '57029' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



0

                       AS Debit, ISNULL(CropsPurchaseHeader.KantaExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

, CropsPurchaseHeader.CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.KantaExp,0) <> 0



UNION ALL



/******* CROP PURCHASE FreightExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      '57009' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



0

                       AS Debit, ISNULL(CropsPurchaseHeader.FreightExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                       , CropsPurchaseHeader.CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.FreightExp,0) <> 0



UNION ALL



/******* CROP PURCHASE MarketFeesExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      '57010' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



0

                       AS Debit, ISNULL(CropsPurchaseHeader.MarketFeesExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                       , CropsPurchaseHeader.CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.MarketFeesExp,0) <> 0



/********************* END CROP PURCHASE *************************************/

UNION ALL

/*************** CROPS SALES ********************************************/  /****** CROP SALES GROSS VALUE CREDIT TO CROP SALE ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 

case when isnull(Products.SaleAcNo,'') = ''  then dbo.RegistryValue('ATCropSaleAccount') else Products.SaleAcNo end AS AccountNo, 

                      ISNULL(CropsSaleHeader.Remarks, ' ') AS Narration, 0 AS Debit, ROUND(SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier * CropsSaleBody.Price),0) AS Credit, ISNULL(CropsSaleHeader.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

                     , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID

WHERE     (CropsSaleHeader.IsPosted = 1)

GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDate, CropsSaleHeader.IsPosted, CropsSaleHeader.Remarks, 

                      CropsSaleHeader.IsApplyPartyMarkup, Products.SaleAcNo, CropsSaleHeader.CropSeason 

UNION ALL



/******* CROP SALE EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      case when isnull(Products.ExpAcc,'') = ''  then dbo.RegistryValue('ATCropSaleExpMinusAccount') else Products.ExpAcc end --dbo.RegistryValue('ATCropSaleExpMinusAccount') 

                      AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ')  AS Narration, 



                      CropsSaleHeader.ExpMinus AS Debit, 

 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

  , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID



WHERE     CropsSaleHeader.IsPosted = 1

UNION ALL



/******* CROP SALE COST OF GOOD SOLD ACCOUNT  ************/

SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleAccount') AS AccountNo, 

                      ISNULL(CropsSaleHeader.Remarks, ' ') AS Narration, CropsSaleBody.Weight *  CropsSaleBody.Cost AS Debit, 0 AS Credit, ISNULL(CropsSaleHeader.IsApplyPartyMarkup, 

                      0) AS IsApplyPartyMarkup , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID

WHERE     (CropsSaleHeader.IsPosted = 1)

UNION ALL

SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, dbo.RegistryValue('ATCropSoldStockValueAccount') AS AccountNo, 

                      ISNULL(CropsSaleHeader.Remarks, ' ') AS Narration, 0 AS Debit, CropsSaleBody.Weight *  CropsSaleBody.Cost  AS Credit, ISNULL(CropsSaleHeader.IsApplyPartyMarkup, 

                      0) AS IsApplyPartyMarkup , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID

WHERE     (CropsSaleHeader.IsPosted = 1)

UNION ALL

/******* CROP SALE EXPPLUS CREDIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      case when isnull(Products.ExpAcc,'') = ''  then dbo.RegistryValue('ATCropSaleExpPlusAccount') else Products.ExpAcc end AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      CropsSaleHeader.ExpPlus

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

   , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/******* CROP SALE RECEIVEDAMOUNT DEBIT TO CASH ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ')  AS Narration, 



                      CropsSaleHeader.PaidAmount AS Debit, 

                       0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                        , CropsSaleHeader.CropSeason 

FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/******* CROP SALE BAGS AMOUNT CREDIT TO BAGS ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '11327' AS AccountNo, IsNull(CropsSaleHeader.Remarks,'Crop Sale Bags ')  AS Narration, 



                      0 AS Debit, 

                       CropsSaleHeader.BagAmount AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                        , CropsSaleHeader.CropSeason 

FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1 and ISNULL(CropsSaleHeader.BagAmount,0) <> 0



UNION ALL



/********** CROP SALE STAX CREDIT TO STAX ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleSTaxAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.STRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                       , CropsSaleHeader.CropSeason 

FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.STRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup , CropsSaleHeader.CropSeason 



UNION ALL



/********** CROP SALE WHTAX CREDIT TO WHTAX ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleWHTaxAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.WHTRatio,0) / 100 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                       , CropsSaleHeader.CropSeason 

FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.WHTRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup , CropsSaleHeader.CropSeason 



UNION ALL

/********** CROP SALE Commision Credit Account 4203  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 

                      case when isnull(Products.CommAcc,'') = ''  then dbo.RegistryValue('ATCropSaleComAccount') else Products.CommAcc end--dbo.RegistryValue('ATCropSaleComAccount') 

                      AS AccountNo, 

                      ISNULL(CropsSaleHeader.Remarks, ' ') AS Narration, CASE WHEN ISNULL(CropsSaleHeader.ComIsMinus, 0) 

                      = 1 THEN (CASE WHEN ISNULl(ComPermound, 0) = 1 THEN ISNULL(ComAmount, 0) ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) ELSE 0 END AS Debit, 

                      CASE WHEN ISNULL(CropsSaleHeader.ComIsMinus, 0) = 0 THEN (CASE WHEN ISNULl(ComPermound, 0) = 1 THEN ISNULL(ComAmount, 0) 

                      ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 

                      * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) ELSE 0 END AS Credit, ISNULL(CropsSaleHeader.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup , CropsSaleHeader.CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID

                      

WHERE     (CropsSaleHeader.IsPosted = 1)

GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDate, CropsSaleHeader.IsPosted, CropsSaleHeader.WHTRatio, CropsSaleHeader.Remarks, 

                      CropsSaleHeader.IsApplyPartyMarkup, CropsSaleHeader.ComRatio, CropsSaleHeader.ComAmount, CropsSaleHeader.ComPerMound, 

                      CropsSaleHeader.ComIsMinus,CommAcc , CropsSaleHeader.CropSeason 





UNION ALL



/*********** CROP Sale NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/

SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CropsSaleHeader.CustomerID AS AccountNo, ISNULL(CropsSaleHeader.Remarks, 

                      ' ') + ', ' + ISNULL(CropsSaleHeader.NarrationUrdu, N'') + ', Lot#'+isnull(CropsSalesWeighments.LotNo,'') AS Narration, ROUND(((SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier * CropsSaleBody.Price) - ISNULL(CropsSaleHeader.ExpMinus, 0) + ISNULL(CropsSaleHeader.ExpPlus, 0) 

                      + CASE WHEN ISNULL(CropsSaleHeader.ComIsMinus, 0) = 1 THEN - (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN ISNULL(ComAmount, 0) 

                      ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 

                      * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) ELSE (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN ISNULL(ComAmount, 0) 

                      ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 

                      * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) END) + SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier * CropsSaleBody.Price) * ISNULL(CropsSaleHeader.STRatio, 0) / 100) 

                      - SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier * CropsSaleBody.Price) * ISNULL(CropsSaleHeader.WHTRatio, 

                      0) / 100, 0)

                      -ROUND((ISNULL(CropsSaleHeader.MillExp,0)+ ISNULL(CropsSaleHeader.OtherExp,0)+ ISNULL(CropsSaleHeader.FreightExp,0)+ ISNULL(CropsSaleHeader.KantaExp,0)+ ISNULL(CropsSaleHeader.BrokerageExp,0)+ ISNULL(CropsSaleHeader.MarketFeesExp,0)),0)

                      +ISNULL(CropsSaleHeader.BagAmount,0)

                       AS Debit, ISNULL(CropsSaleHeader.PaidAmount, 0) AS Credit, ISNULL(CropsSaleHeader.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

                       , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID LEFT OUTER JOIN

                      CropsSalesWeighments ON CropsSaleBody.WeighmentID = CropsSalesWeighments.WeighmentID

WHERE     (CropsSaleHeader.IsPosted = 1)

GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDate, CropsSaleHeader.IsPosted, CropsSaleHeader.IsApplyPartyMarkup, 

                      CropsSaleHeader.CustomerID, CropsSaleHeader.STRatio, CropsSaleHeader.WHTRatio, CropsSaleHeader.ExpMinus, CropsSaleHeader.ExpPlus, 

                      CropsSaleHeader.Remarks, CropsSaleHeader.PaidAmount, CropsSaleHeader.ComRatio, CropsSaleHeader.ComAmount, 

                      CropsSaleHeader.ComPerMound, CropsSaleHeader.ComIsMinus, CropsSaleHeader.NarrationUrdu, CropsSalesWeighments.LotNo

, CropsSaleHeader.MillExp, CropsSaleHeader.OtherExp, CropsSaleHeader.FreightExp, CropsSaleHeader.KantaExp, CropsSaleHeader.BrokerageExp, CropsSaleHeader.MarketFeesExp

 , CropsSaleHeader.CropSeason ,CropsSaleHeader.BagAmount

UNION ALL

/******* CROP SALE OtherExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropSaleExpMinusAccount') AS AccountNo, 'Crop Sale Other Expenses' AS Narration, 

                      ISNULL(CropsSaleHeader.OtherExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

   , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.OtherExp,0) <> 0



UNION ALL

/******* CROP SALE MillExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropSaleExpMinusAccount') AS AccountNo, 'Crop Sale Mill Expenses' AS Narration, 

                      ISNULL(CropsSaleHeader.MillExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

   , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.MillExp,0) <> 0



UNION ALL

/******* CROP SALE KantaExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58025' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                      ISNULL(CropsSaleHeader.KantaExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

   , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.KantaExp,0) <> 0



UNION ALL

/******* CROP SALE BrokerageExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58017' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                     case when ISNULL(CropsSaleHeader.BrokerageExp,0) > 0 then ISNULL(CropsSaleHeader.BrokerageExp,0) else 0 end AS Debit, 



                      case when ISNULL(CropsSaleHeader.BrokerageExp,0) < 0 then ABS(ISNULL(CropsSaleHeader.BrokerageExp,0)) else 0 end

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

   , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.BrokerageExp,0) <> 0





UNION ALL

/******* CROP SALE MarketFeesExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58001' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                     ISNULL(CropsSaleHeader.MarketFeesExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

   , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.MarketFeesExp,0) <> 0





UNION ALL

/******* CROP SALE FreightExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58010' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                     ISNULL(CropsSaleHeader.FreightExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

   , CropsSaleHeader.CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.FreightExp,0) <> 0





/********************* END CROP Sale *************************************/
GO

-- View: VuAccVouchersOthersExtendedCrop
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersExtendedCrop]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersExtendedCrop];
GO
CREATE VIEW [dbo].[VuAccVouchersOthersExtendedCrop]  AS 



/*************** CROPS PURCHASE ********************************************/



/****** CROP PURCHASE GROSS VALUE DEBIT TO CROP PURCHASE ACCOUNT  *********/







SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, CropsPurchaseHeader.PurchaseDate AS VoucherDate, 

                      '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, isnull( Products.PurAcNo,dbo.RegistryValue('ATCropPurchaseAccount') )



                      AS AccountNo, dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID) AS Narration, 

                      SUM(CropsPurchaseBody.Weight / CropsWeighments.Multiplier * CropsPurchaseBody.Price) AS Debit, 0 AS Credit, 0 AS IsApplyPartyMarkup 

              ,  CropSeason         

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     (CropsPurchaseHeader.IsPosted = 1)

GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.Remarks, 

                      Products.PurAcNo ,  CropSeason 

UNION ALL



/******* CROP PURCHASE EXPMINUS Credit, 0 AS IsApplyPartyMarkup TO EXPENSES MINUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, case when ISNULL(Products.ExpAcc, '') = '' then dbo.RegistryValue('ATCropPurchaseExpMinusAccount') else Products.ExpAcc end  AS AccountNo, dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID)  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.ExpMinus



                       AS Credit, 0 AS IsApplyPartyMarkup

                        ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

                      

WHERE     CropsPurchaseHeader.IsPosted = 1



UNION ALL



/******* CROP PURCHASE EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT    DISTINCT 'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, case when ISNULL(Products.ExpAcc, '') = '' then dbo.RegistryValue('ATCropPurchaseExpPlusAccount') else Products.ExpAcc end

                       AS AccountNo, dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID) AS Narration, 



                      CropsPurchaseHeader.ExpPlus



                       AS Debit, 0 AS Credit, 0 AS IsApplyPartyMarkup

                        ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID







WHERE     CropsPurchaseHeader.IsPosted = 1



UNION ALL



/******* CROP PURCHASE PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID)  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.PaidAmount



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                        ,  CropSeason 

FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1

UNION ALL



/*************** BARDANA VALUE AS DEBIT TO PURCHASE BARDANA ACCOUNT ********************************************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '11327' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,'Crop Purchase Bags ')  AS Narration, ISNULL(CropsPurchaseHeader.BagAmount,0) AS Debit, 



                      0



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, CropSeason 



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.BagAmount,0) <> 0

UNION ALL



/********** CROP PURCHASE COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, CropsPurchaseHeader.PurchaseDate AS VoucherDate, 

                      '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(Products.CommAcc, '') 

                      = '' THEN dbo.RegistryValue('ATCropPurchaseComAccount') ELSE Products.CommAcc END AS AccountNo, 

                      dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID) AS Narration, CASE WHEN ISNULL(ComPerMound, 0) 

                      = 1 THEN ISNULL(ComAmount, 0) ELSE ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 

                      * IsNull(CropsPurchaseHeader.ComRatio, 0) / 100) END AS Debit, 0 AS Credit, 0 AS IsApplyPartyMarkup ,  CropSeason 

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     (CropsPurchaseHeader.IsPosted = 1)

GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.ComRatio, 

                      CropsPurchaseHeader.Remarks, CropsPurchaseHeader.ComAmount, CropsPurchaseHeader.ComPerMound, Products.CommAcc ,  CropSeason 

UNION ALL



/*********** CROP PURCHASE NET AMOUNT Credit, 0 AS IsApplyPartyMarkup TO VENDOR ACCOUNT *************/





--SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



--                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



-- AS ReconcileDate, CropsPurchaseHeader.VendorID AS AccountNo, dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID)

--+ Case When ISNULL(CropsPurchaseHeader.ExpPlus, 0) > 0 then ',(Exp+):'+ Cast(Cast(ISNULL(CropsPurchaseHeader.ExpPlus, 0) AS Decimal(18,2)) AS Varchar) Else '' End 



--+ Case When ISNULL(CropsPurchaseHeader.ExpMinus, 0) > 0 then ',(Exp-):'+ Cast(Cast(ISNULL(CropsPurchaseHeader.ExpMinus, 0) AS Decimal(18,2)) AS Varchar) Else '' End 



--+ Case When  IsNull(CropsPurchaseHeader.ComRatio,0) > 0 then ',Com:'+Cast(Cast((Case When ISNULL(ComPerMound, 0) = 1 then iSNULL(ComAmount, 0)Else SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)  * IsNull(CropsPurchaseHeader.ComRatio,0) / 100 END)AS Decimal(18,2))AS Varchar) Else '' End 



--+', '+ ISNULL(CropsPurchaseHeader.Remarks,N'')

--+', '+ ISNULL(NarrationUrdu,N'')  AS Narration, ISNULL(CropsPurchaseHeader.PaidAmount, 0) AS Debit, 



--case when isnull(CropsPurchaseHeader.VendorIDCom,0) = 0 then



--                      (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



--                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



--                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 

--+ (Case When ISNULL(ComPerMound, 0) = 1 then iSNULL(ComAmount, 0)Else

--(SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)  * IsNull(CropsPurchaseHeader.ComRatio,0) / 100)

--End  )

--else



--                      ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



--                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



--                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 

--+ (Case When ISNULL(ComPerMound, 0) = 1 then iSNULL(ComAmount, 0)Else

--(SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)  * IsNull(CropsPurchaseHeader.ComRatio,0) / 100) End) )  

--* (100-CropsPurchaseHeader.VendorComRatio) / 100 

-- -ROUND((ISNULL(CropsPurchaseHeader.MillExp,0)+ ISNULL(CropsPurchaseHeader.OtherExp,0)+ ISNULL(CropsPurchaseHeader.FreightExp,0)+ ISNULL(CropsPurchaseHeader.KantaExp,0)+ ISNULL(CropsPurchaseHeader.BrokerageExp,0)+ ISNULL(CropsPurchaseHeader.MarketFeesExp,0)),0)



--end AS Credit



--, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



--FROM CropsPurchaseBody INNER JOIN

--    CropsPurchaseHeader ON 

--    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

--     INNER JOIN

--    CropsWeighments ON 

--    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID



--WHERE     CropsPurchaseHeader.IsPosted = 1



--GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.ComPerMound, CropsPurchaseHeader.ComAmount, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted, IsApplyPartyMarkup



--              ,CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio 



--              , CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks,CropsPurchaseHeader.PaidAmount

--,NarrationUrdu,CropsPurchaseHeader.Remarks

--, CropsPurchaseHeader.MillExp, CropsPurchaseHeader.OtherExp, CropsPurchaseHeader.FreightExp, CropsPurchaseHeader.KantaExp, CropsPurchaseHeader.BrokerageExp, CropsPurchaseHeader.MarketFeesExp



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, CropsPurchaseHeader.PurchaseDate AS VoucherDate, 

                      '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CropsPurchaseHeader.VendorID AS AccountNo, 

                      dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID)

+ Case When ISNULL(CropsPurchaseHeader.ExpPlus, 0) > 0 then ',(Exp+):'+ Cast(Cast(ISNULL(CropsPurchaseHeader.ExpPlus, 0) AS Decimal(18,2)) AS Varchar) Else '' End 



+ Case When ISNULL(CropsPurchaseHeader.ExpMinus, 0) > 0 then ',(Exp-):'+ Cast(Cast(ISNULL(CropsPurchaseHeader.ExpMinus, 0) AS Decimal(18,2)) AS Varchar) Else '' End 



+ Case When  IsNull(CropsPurchaseHeader.ComRatio,0) > 0 then ',Com:'+Cast(Cast((Case When ISNULL(ComPerMound, 0) = 1 then iSNULL(ComAmount, 0)Else SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)  * IsNull(CropsPurchaseHeader.ComRatio,0) / 100 END)AS Decimal(18,2))AS Varchar) Else '' End 



+', '+ ISNULL(CropsPurchaseHeader.Remarks,N'')

+', '+ ISNULL(NarrationUrdu,N'') AS Narration, ISNULL(CropsPurchaseHeader.PaidAmount, 0) AS Debit, 

                     round( CASE WHEN isnull(CropsPurchaseHeader.VendorIDCom, 0) = 0 THEN (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) 

                      * CropsPurchaseBody.Price)) - ISNULL(CropsPurchaseHeader.ExpMinus, 0) + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 

                      + (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN iSNULL(ComAmount, 0) ELSE (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) 

                      * CropsPurchaseBody.Price) * IsNull(CropsPurchaseHeader.ComRatio, 0) / 100) END) 

                      ELSE ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 

                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) + (CASE WHEN ISNULL(ComPerMound, 0) = 1 THEN iSNULL(ComAmount, 0) 

                      ELSE (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price) * IsNull(CropsPurchaseHeader.ComRatio, 0) 

                      / 100) END)) * (100 - CropsPurchaseHeader.VendorComRatio) / 100 END ,0)

-ROUND((ISNULL(CropsPurchaseHeader.MillExp,0)+ ISNULL(CropsPurchaseHeader.OtherExp,0)+ ISNULL(CropsPurchaseHeader.FreightExp,0)+ ISNULL(CropsPurchaseHeader.KantaExp,0)+ ISNULL(CropsPurchaseHeader.BrokerageExp,0)+ ISNULL(CropsPurchaseHeader.MarketFeesExp,0)),0)

+ISNULL(CropsPurchaseHeader.BagAmount,0)                      

                      AS  Credit, ISNULL(CropsPurchaseHeader.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup ,  CropSeason 

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID

WHERE     (CropsPurchaseHeader.IsPosted = 1)



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.ComAmount, CropsPurchaseHeader.ComPerMound, 

                      CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.IsApplyPartyMarkup, 

                      CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio, 

                      CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks, CropsPurchaseHeader.PaidAmount,NarrationUrdu

, CropsPurchaseHeader.MillExp, CropsPurchaseHeader.OtherExp, CropsPurchaseHeader.FreightExp, CropsPurchaseHeader.KantaExp, CropsPurchaseHeader.BrokerageExp, CropsPurchaseHeader.MarketFeesExp

 ,  CropSeason ,CropsPurchaseHeader.BagAmount





UNION ALL





SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, CropsPurchaseHeader.VendorIDCom AS AccountNo, dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID) AS Narration, 0 AS Debit, 



                      ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 



                    - ISNULL(CropsPurchaseHeader.PaidAmount, 0) 

            



                      + (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price))



                      * IsNull(CropsPurchaseHeader.ComRatio,0) / 100) * CropsPurchaseHeader.VendorComRatio / 100  AS Credit



, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 ,  CropSeason 

FROM CropsPurchaseBody INNER JOIN

    CropsPurchaseHeader ON 

    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

     INNER JOIN

    CropsWeighments ON 

    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID



WHERE     CropsPurchaseHeader.IsPosted = 1 and isnull(CropsPurchaseHeader.VendorIDCom,0) <> 0



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted, IsApplyPartyMarkup



              ,CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio 



              , CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks,CropsPurchaseHeader.PaidAmount

 ,  CropSeason 

UNION ALL



/******* CROP PURCHASE OtherExp CREDIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropPurchaseExpMinusAccount') 

                       AS AccountNo,'Other Exp: ' + dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID)  AS Narration, 0 AS Debit, 



                      ISNULL(CropsPurchaseHeader.OtherExp,0)



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN



                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID





WHERE     CropsPurchaseHeader.IsPosted = 1 AND (ISNULL(CropsPurchaseHeader.OtherExp,0)) <> 0

UNION ALL



/******* CROP PURCHASE MillExp CREDIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropPurchaseExpPlusAccount') AS AccountNo, 'Crop Mill Expense: '+ dbo.FunGetCropPurchaseNarration(CropsPurchaseHeader.CropPurchaseID) AS Narration, 



                     0 

                       AS Debit, ISNULL(CropsPurchaseHeader.MillExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.MillExp,0) <> 0



UNION ALL



/******* CROP PURCHASE BrokerageExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      '57017' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



CASE WHEN ISNULL(CropsPurchaseHeader.BrokerageExp,0) < 0 THEN ABS(ISNULL(CropsPurchaseHeader.BrokerageExp,0)) ELSE 0 END

                       AS Debit, CASE WHEN ISNULL(CropsPurchaseHeader.BrokerageExp,0) > 0 THEN ISNULL(CropsPurchaseHeader.BrokerageExp,0) ELSE 0 END AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.BrokerageExp,0) <> 0



UNION ALL



/******* CROP PURCHASE KantaExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      '57029' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



0

                       AS Debit, ISNULL(CropsPurchaseHeader.KantaExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.KantaExp,0) <> 0



UNION ALL



/******* CROP PURCHASE FreightExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, 

                      '57009' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



0

                       AS Debit, ISNULL(CropsPurchaseHeader.FreightExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.FreightExp,0) <> 0



UNION ALL



/******* CROP PURCHASE MarketFeesExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT   DISTINCT  'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      '57010' AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



0

                       AS Debit, ISNULL(CropsPurchaseHeader.MarketFeesExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                        ,  CropSeason 

FROM        CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     CropsPurchaseHeader.IsPosted = 1 AND ISNULL(CropsPurchaseHeader.MarketFeesExp,0) <> 0



/********************* END CROP PURCHASE *************************************/

UNION ALL

/*************** CROPS SALES ********************************************/ 



/****** CROP SALES GROSS VALUE Credit, 0 AS IsApplyPartyMarkup TO CROP SALE ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 





case when isnull(Products.SaleAcNo,'') = '' then dbo.RegistryValue('ATCropSaleAccount') else Products.SaleAcNo end AS AccountNo, 

                      dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID)

                      +',Bags:' + cast(sum(CropsSaleBody.SupplyModeQty) as varchar)

+',V.No:' + CropsSaleHeader.VehicleNo

 AS Narration, 0 AS Debit, SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier * CropsSaleBody.Price) AS Credit, 0 AS IsApplyPartyMarkup ,  CropSeason 

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID

WHERE     (CropsSaleHeader.IsPosted = 1)

GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDate, CropsSaleHeader.IsPosted, CropsSaleHeader.Remarks, Products.SaleAcNo,CropsSaleHeader.VehicleNo ,  CropSeason 



UNION ALL



/******* CROP SALE EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/





SELECT   DISTINCT  'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, 

                      case when isnull(Products.ExpAcc,'') = ''  then dbo.RegistryValue('ATCropSaleExpMinusAccount') else Products.ExpAcc end --dbo.RegistryValue('ATCropSaleExpMinusAccount') 

                      AS AccountNo, dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID)   AS Narration, 



                      CropsSaleHeader.ExpMinus AS Debit, 

 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN



                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/******* CROP SALE BAGS AMOUNT CREDIT TO BAGS ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '11327' AS AccountNo, IsNull(CropsSaleHeader.Remarks,'Crop Sale Bags ')  AS Narration, 



                      0 AS Debit, 

                       CropsSaleHeader.BagAmount AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                        , CropsSaleHeader.CropSeason 

FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1 and ISNULL(CropsSaleHeader.BagAmount,0) <> 0

UNION ALL



/******* CROP SALE EXPPLUS Credit, 0 AS IsApplyPartyMarkup TO EXPENSES PLUS ACCOUNT  ************/

SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      case when isnull(Products.ExpAcc,'') = ''  then dbo.RegistryValue('ATCropSaleExpPlusAccount') else Products.ExpAcc end AS AccountNo, 

                      dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID) AS Narration, 0 AS Debit, 



                      CropsSaleHeader.ExpPlus

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/******* CROP SALE RECEIVEDAMOUNT DEBIT TO CASH ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID)  AS Narration, 



                      CropsSaleHeader.PaidAmount AS Debit, 

                       0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/********** CROP SALE STAX Credit, 0 AS IsApplyPartyMarkup TO STAX ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleSTaxAccount') AS AccountNo, dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID) AS Narration, 0 AS Debit, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.STRatio,0) / 100 AS Credit, 0 AS IsApplyPartyMarkup ,  CropSeason 



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.STRatio, CropsSaleHeader.Remarks ,  CropSeason 



UNION ALL



/********** CROP SALE WHTAX Credit, 0 AS IsApplyPartyMarkup TO WHTAX ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleWHTaxAccount') AS AccountNo, dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID) AS Narration, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.WHTRatio,0) / 100 AS Debit, 0 AS Credit, 0 AS IsApplyPartyMarkup ,  CropSeason 



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.WHTRatio, CropsSaleHeader.Remarks ,  CropSeason 





UNION ALL

/*********** CROP Sale Commision Credit Account 4203 *************/

SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, case when isnull(Products.CommAcc,'') = ''  then dbo.RegistryValue('ATCropSaleComAccount') else Products.CommAcc end--dbo.RegistryValue('ATCropSaleComAccount') 

                       AS AccountNo,  dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID) AS Narration, 



CASE WHEN ISNULL(CropsSaleHeader.ComIsMinus, 0) 

                      = 1 THEN (CASE WHEN ISNULl(ComPermound, 0) = 1 THEN ISNULL(ComAmount, 0) ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) ELSE 0 END AS Debit, 

                      CASE WHEN ISNULL(CropsSaleHeader.ComIsMinus, 0) = 0 THEN (CASE WHEN ISNULl(ComPermound, 0) = 1 THEN ISNULL(ComAmount, 0) 

                      ELSE ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 

                      * IsNull(CropsSaleHeader.ComRatio, 0) / 100)) END) ELSE 0 END AS Credit

                      , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID

    INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.WHTRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup, CropsSaleHeader.ComRatio, CropsSaleHeader.ComAmount, CropsSaleHeader.ComPerMound

                      ,CropsSaleHeader.ComIsMinus,Products.CommAcc ,  CropSeason 



UNION ALL

/*********** CROP Sale NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, CropsSaleHeader.CustomerID AS AccountNo, 

          CASE WHEN    ISNULL(CropsSalesWeighments.LotNo,'') = '' THEN         dbo.FunGetCropSaleNarration(CropsSaleHeader.CropSaleID) ELSE 

          (Select Products.ProductName +':'+cast(cast((CropsSaleBody.Weight-ISNULL(CropsSaleBody.Deduction,0))/CropsSaleBody.Multiplier as int) as varchar) + '-' + cast(cast(Round((((CropsSaleBody.Weight-ISNULL(CropsSaleBody.Deduction,0))/CropsSaleBody.Multiplier)-cast((CropsSaleBody.Weight-ISNULL(CropsSaleBody.Deduction,0))/CropsSaleBody.Multiplier as int))*CropsSaleBody.Multiplier,0) as int) as varchar)+'Mnd-' + Cast(Cast((CropsSaleBody.Weight-ISNULL(CropsSaleBody.Deduction,0)) AS Decimal(18,0)) AS Varchar)+'Kgs @'+cast(cast(CropsSaleBody.Price as decimal(11,2)) as varchar)   as Remarks  From CropsSaleBody INNER JOIN Products ON Products.ProductId = CropsSaleBody.ProductId 

          Where CropsSaleBody.CropSaleId=CropsSaleHeader.CropSaleID AND CropsSalesWeighments.LotNo = CropsSaleBody.LotNo AND CropsSalesWeighments.WeighmentID = CropsSaleBody.WeighmentID) 

          END +

 Case When ISNULL(CropsSaleHeader.ExpMinus, 0)  > 0 then +',(Exp-):'+Cast(Cast(ISNULL(CropsSaleHeader.ExpMinus, 0) AS Decimal(18,2)) AS Varchar) Else '' End  

+ Case When ISNULL(CropsSaleHeader.ExpPlus, 0)  > 0 then +',(Exp+):'+Cast(Cast(ISNULL(CropsSaleHeader.ExpPlus, 0) AS Decimal(18,2)) AS Varchar) Else '' End  

+ Case when  IsNull(CropsSaleHeader.STRatio,0) > 0 then +',ST:'+ Cast(Cast((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.STRatio,0) / 100) AS Decimal(18,2) ) AS Varchar) Else '' End 

+ Case When IsNull(CropsSaleHeader.WHTRatio,0) > 0 then +',WHT:'+ Cast(Cast((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)  * IsNull(CropsSaleHeader.WHTRatio,0) / 100) AS Decimal(18,2)) AS Varchar) Else '' End 

+ Case When IsNull(CropsSaleHeader.ComRatio,0) > 0 then +',Com:'+ Cast(Cast((Case When ISNULL(ComPerMound, 0) = 1 then ISNULL(ComAmount, 0) Else ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio,0) / 100)) End ) AS Decimal(18,2)) AS Varchar) Else '' End  

+',Bags:' + ISNULL(cast((sum(CropsSaleBody.SupplyModeQty)) as varchar),'')

+',V.No:' + ISNULL(CropsSaleHeader.VehicleNo,'')

+ CASE WHEN isnull(CropsSalesWeighments.LotNo,'') <> '' THEN ', Lot#'+isnull(CropsSalesWeighments.LotNo,'') ELSE '' END

+', '+ ISNULL(CropsSaleHeader.Remarks,N'') +', '+ ISNULL(NarrationUrdu,N'') 



 AS Narration, 



                      (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      - ISNULL(CropsSaleHeader.ExpMinus, 0) 

            

                      + case when ISNULL(CropsSaleHeader.ComIsMinus, 0) = 1 then   -(Case When ISNULL(ComPerMound, 0) = 1 then ISNULL(ComAmount, 0) Else ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio,0) / 100)) End)

                     else

                      (Case When ISNULL(ComPerMound, 0) = 1 then ISNULL(ComAmount, 0) Else ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio,0) / 100)) End)

                      end

--(SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio,0) / 100)



                      + ISNULL(CropsSaleHeader.ExpPlus, 0) 



                      + (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 



                      * IsNull(CropsSaleHeader.STRatio,0) / 100)



                      - (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 



                      * IsNull(CropsSaleHeader.WHTRatio,0) / 100)

                      -ROUND((ISNULL(CropsSaleHeader.MillExp,0)+ ISNULL(CropsSaleHeader.OtherExp,0)+ ISNULL(CropsSaleHeader.FreightExp,0)+ ISNULL(CropsSaleHeader.KantaExp,0)+ ISNULL(CropsSaleHeader.BrokerageExp,0)+ ISNULL(CropsSaleHeader.MarketFeesExp,0)),0)

                      +ISNULL(CropsSaleHeader.BagAmount,0)

                       AS Debit,  ISNULL(CropsSaleHeader.PaidAmount, 0)  AS Credit, 0 AS IsApplyPartyMarkup ,  CropSeason 

FROM CropsSaleBody INNER JOIN    CropsSaleHeader ON  CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID

LEFT OUTER JOIN

                      CropsSalesWeighments ON CropsSaleBody.WeighmentID = CropsSalesWeighments.WeighmentID

 WHERE     CropsSaleHeader.IsPosted = 1  

 GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted, CropsSaleHeader.CustomerID, CropsSaleHeader.STRatio, CropsSaleHeader.WHTRatio , CropsSaleHeader.ExpMinus, CropsSaleHeader.ExpPlus, CropsSaleHeader.Remarks,CropsSaleHeader.PaidAmount, CropsSaleHeader.ComRatio, CropsSaleHeader.ComAmount, CropsSaleHeader.ComPerMound,CropsSaleHeader.ComIsMinus,CropsSaleHeader.VehicleNo,NarrationUrdu, CropsSalesWeighments.LotNo,CropsSalesWeighments.WeighmentID ,CropsSaleHeader.Remarks

, CropsSaleHeader.MillExp, CropsSaleHeader.OtherExp, CropsSaleHeader.FreightExp, CropsSaleHeader.KantaExp, CropsSaleHeader.BrokerageExp, CropsSaleHeader.MarketFeesExp ,  CropSeason ,CropsSaleHeader.BagAmount

UNION ALL

/******* CROP SALE OtherExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropSaleExpMinusAccount') AS AccountNo, 'Crop Sale Other Expenses' AS Narration, 

                      ISNULL(CropsSaleHeader.OtherExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.OtherExp,0) <> 0



UNION ALL

/******* CROP SALE MillExp DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      dbo.RegistryValue('ATCropSaleExpMinusAccount') AS AccountNo, 'Crop Sale Mill Expenses' AS Narration, 

                      ISNULL(CropsSaleHeader.MillExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.MillExp,0) <> 0



UNION ALL

/******* CROP SALE KantaExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58025' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                      ISNULL(CropsSaleHeader.KantaExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.KantaExp,0) <> 0



UNION ALL

/******* CROP SALE BrokerageExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58017' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                     case when ISNULL(CropsSaleHeader.BrokerageExp,0) > 0 then ISNULL(CropsSaleHeader.BrokerageExp,0) else 0 end AS Debit, 



                      case when ISNULL(CropsSaleHeader.BrokerageExp,0) < 0 then ABS(ISNULL(CropsSaleHeader.BrokerageExp,0)) else 0 end

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.BrokerageExp,0) <> 0





UNION ALL

/******* CROP SALE MarketFeesExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58001' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                     ISNULL(CropsSaleHeader.MarketFeesExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.MarketFeesExp,0) <> 0





UNION ALL

/******* CROP SALE FreightExp DEBIT ACCOUNT  ************/



SELECT  DISTINCT   'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, 

                      '58010' AS AccountNo, 'Crop Sale Kanta Expenses' AS Narration, 

                     ISNULL(CropsSaleHeader.FreightExp,0) AS Debit, 



                      0

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup ,  CropSeason 



FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID 



WHERE     CropsSaleHeader.IsPosted = 1 AND ISNULL(CropsSaleHeader.FreightExp,0) <> 0

/********************* END CROP Sale *************************************/
GO

-- View: VuAccVouchersOthersExtensionNewIDs
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersExtensionNewIDs]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersExtensionNewIDs];
GO
CREATE VIEW [dbo].VuAccVouchersOthersExtensionNewIDs



AS





-- extension Of View "VuAccVouchersOthers"

 -------- BrokerAge Purchase Expanses....--------

 --- SHORTAGE Purchase Exp



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57027' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ShortageG, 0) > 0 then ISNULL(ContractInvoice.ShortageG, 0) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.ShortageG, 0) < 0 then Abs(ISNULL(ContractInvoice.ShortageG, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ShortageG, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.ShortageG, 0)) > 0)



UNION ALL

--- Tractor Purchase Exp

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57028' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.TractorExp, 0) > 0 then ISNULL(ContractInvoice.TractorExp, 0) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.TractorExp, 0) < 0 then Abs(ISNULL(ContractInvoice.TractorExp, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.TractorExp, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.TractorExp, 0)) > 0)



UNION ALL

----- Sotli Purchase Exp

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL                       AS ChequeDate,

 NULL AS DepositedDate, NULL AS ReconcileDate, '57020' AS AccountNo, ContractInvoice.Narration,                       Case When ISNULL(ContractInvoice.Sotli, 0) > 0 then ISNULL(ContractInvoice.Sotli, 0) Else 0 End   AS Debit,Case When ISNULL(ContractInvoice.Sotli, 0) < 0 then Abs(ISNULL(ContractInvoice.Sotli, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.Sotli, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.Sotli, 0)) > 0)



UNION ALL

------ Stitching Purchase Exp

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57021' AS AccountNo, ContractInvoice.Narration,   

                    Case When ISNULL(ContractInvoice.Stitching, 0) > 0 then ISNULL(ContractInvoice.Stitching, 0) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.Stitching, 0) < 0 then Abs(ISNULL(ContractInvoice.Stitching, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.Stitching, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.Stitching, 0)) > 0)

UNION ALL

-- Rent Godown Purchase Ep

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57022' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.RentGodown, 0) > 0 then ISNULL(ContractInvoice.RentGodown, 0) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.RentGodown, 0) < 0 then Abs(ISNULL(ContractInvoice.RentGodown, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.RentGodown, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.RentGodown, 0)) > 0)



UNION ALL 

----Choki DARA Purchase Exp

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57023' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ChokiDara, 0) > 0 then ISNULL(ContractInvoice.ChokiDara, 0) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.ChokiDara, 0) < 0 then Abs(ISNULL(ContractInvoice.ChokiDara, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ChokiDara, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.ChokiDara, 0)) > 0)

UNION ALL

---Chart Charges Purchase Exp 

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57024' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ChartsCharges, 0) > 0 then ISNULL(ContractInvoice.ChartsCharges, 0) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.ChartsCharges, 0) < 0 then Abs(ISNULL(ContractInvoice.ChartsCharges, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ChartsCharges, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.ChartsCharges, 0)) > 0)

UNION ALL

--- Bag USAGE Purchase Exp

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57025' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.BagUsage, 0) > 0 then ISNULL(ContractInvoice.BagUsage, 0) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.BagUsage, 0) < 0 then Abs(ISNULL(ContractInvoice.BagUsage, 0)) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.BagUsage, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.BagUsage, 0)) > 0)



UNION ALL



--EXTENSION

SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(Products.PurAcNo, '') 

                      = '' THEN dbo.RegistryValue('ATPurchaseAccount') ELSE Products.PurAcNo END AS AccountNo, 'Bardana Value, Qty:' + CAST(

cast(sum(ContractInvoiceBody.BagsQty) as int) as Varchar) + ', Rate: ' + CAST(

convert(decimal(5,2),avg(ContractInvoiceBody.Bardana)) as varchar) AS Narration, 

                      SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) AS Debit, 0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

                      

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId INNER JOIN

                      Products ON ContractInvoiceBody.ProductID = Products.ProductID

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (dbo.RegistryValue('StopApplyGrossToPartyAccount') = 0)

and isnull(ContractInvoice.BagVendorID,'') <> '' 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice, ContractInvoice.Narration, ContractInvoice.Received, 

                      ContractInvoice.IsApplyPartyMarkup, Products.PurAcNo,ContractInvoice.BagVendorID

having  SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) > 0





UNION ALL

/*********** CONTRACT PURCHASE INVOICE BARDANA AMOUNT CREDIT TO BAG VENDOR ACCOUNT *************/

SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, ContractInvoice.BagVendorID AS AccountNo, 'Bardana Value, Qty:' + CAST(

cast(sum(ContractInvoiceBody.BagsQty) as int) as Varchar) + ', Rate: ' + CAST(

convert(decimal(5,2),avg(ContractInvoiceBody.Bardana)) as varchar)

                      AS Narration, 0 AS Debit,  SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0)

AND  ISNULL(ContractInvoice.BagVendorID, '') 

                      <> ''

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.PartyID, ContractInvoice.Received, ContractInvoice.Narration, 

                      ContractInvoice.NetValue, ContractInvoice.IsApplyPartyMarkup, ContractInvoice.BagVendorID

HAVING SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) > 0









/********************* END Brokerage Purchase Exp**********************/

/********************* END View **********************/
GO

-- View: VuAccVouchersOthersMulti
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersMulti]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersMulti];
GO
CREATE VIEW [dbo].[VuAccVouchersOthersMulti]  AS 



  SELECT     'MI' AS VOUCHERTYPE, MI.MultiID AS VoucherNo, MI.MDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, case when ISNULL(MI.PurAcNo, '') = '' then dbo.RegistryValue('ATPurchaseAccount') else MI.PurAcNo end 

 AS AccountNo, ISNULL(Purchase.Narration, ' ') AS Narration, 

                      SUM(MI.AmountP)

                      --+ SUM(ISNULL(vl,0)) 

                      AS Debit, 0 AS Credit, ISNULL(Purchase.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup, null CropSeason 

FROM       

                      Purchase 

                      LEFT OUTER JOIN ((SELECT     MultiBody.PurchaseID, CASE WHEN ISNULL(MultiBody.IsVLG, 0) = 1 OR

                      SUM(ISNULL(MultiBody.TPTM, 0)) > 0 THEN SUM(ISNULL(MultiBody.TPTEx, 0) + ISNULL(MultiBody.TPTM, 0)) ELSE 0 END AS VL, 

                      SUM(MultiBody.AmountP) AS AmountP, MultiBody.MDate, MultiBody.MultiID, Products.PurAcNo

FROM         MultiBody INNER JOIN





                      Purchase ON MultiBody.PurchaseID = Purchase.PurchaseID INNER JOIN

                      Products ON MultiBody.ProductID = Products.ProductID

WHERE     (1 = 1)

GROUP BY MultiBody.IsVLG, MultiBody.PurchaseID, MultiBody.MDate, MultiBody.MultiID, Products.PurAcNo)) MI ON MI.PurchaseID = Purchase.PurchaseID

WHERE     (Purchase.IsPosted = 1)

GROUP BY MI.MultiID, MI.MDate, Purchase.IsPosted, Purchase.Narration, Purchase.IsApplyPartyMarkup, MI.PurAcNo

  having  SUM(MI.AmountP) > 0

  

  UNION ALL

SELECT     'MI' AS VOUCHERTYPE, MI.MultiID AS VoucherNo, 



                      MDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, Purchase.VendorID AS AccountNo, IsNull(Purchase.Narration,' ') +', '+ ISNULL(NarrationuRDU,N'') AS Narration, 



                      Purchase.PaidAmount 

                      + SUM(ISNULL(vl,0)) 

                      AS Debit, 



              SUM(MI.AmountP)



                      - ISNULL(Purchase.ExpMinus, 0) 



                      + ISNULL(Purchase.ExpPlus, 0) 



              + (SUM(MI.AmountP) * IsNull(Purchase.ComRatio,0) / 100)

              --+ SUM(ISNULL(vl,0)) 

               AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup, null CropSeason 



FROM 

    Purchase 

    

 INNER JOIN ((SELECT     MultiBody.PurchaseID, CASE WHEN ISNULL(MultiBody.IsVLG, 0) = 1 THEN SUM(ISNULL(MultiBody.TPTEx, 0)) 

                      ELSE 0 END + CASE WHEN SUM(ISNULL(MultiBody.TPTM, 0)) > 0 THEN SUM(ISNULL(MultiBody.TPTM, 0)) ELSE 0 END AS VL, 

                      SUM(MultiBody.AmountP) AS AmountP, MultiBody.MDate, MultiBody.MultiID, MultiBody.IsVLG

FROM         MultiBody INNER JOIN

                      Purchase ON MultiBody.PurchaseID = Purchase.PurchaseID

WHERE     (1 = 1)

GROUP BY MultiBody.IsVLG, MultiBody.PurchaseID, MultiBody.MDate, MultiBody.MultiID)) MI ON MI.PurchaseID = Purchase.PurchaseID 

WHERE     Purchase.IsPosted = 1



GROUP BY MI.MultiID , MI.MDate , Purchase.IsPosted, Purchase.Narration, Purchase.ComRatio, Purchase.VendorID, Purchase.PaidAmount, Purchase.ExpMinus, Purchase.ExpPlus, IsApplyPartyMarkup,NarrationuRDU

  having  SUM(MI.AmountP) > 0

  

union all

  SELECT     'MI' AS VOUCHERTYPE, MI.MultiID AS VoucherNo, MI.MDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, 



case when ISNULL(MI.SaleAcNo,'') = '' then dbo.RegistryValue('ATSalesAccount') else MI.SaleAcNo end AS AccountNo, ISNULL(Sales.Narration, ' ') AS Narration, 0 AS Debit, 

                      round(SUM(MI.AmountS),0)

                      

                     --+SUM(ISNULL(MI.VL,0))

                       AS Credit, ISNULL(Sales.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup, null CropSeason 

                       

FROM        

                      Sales 



                      

                      INNER JOIN ((SELECT     MultiBody.SaleID, CASE WHEN ISNULL(MultiBody.IsVLG, 0) = 1 OR

                      SUM(ISNULL(MultiBody.TPTM, 0)) > 0 THEN SUM(ISNULL(MultiBody.TPTEx, 0) + ISNULL(MultiBody.TPTM, 0)) ELSE 0 END AS VL, 

                      SUM(MultiBody.AmountS) AS AmountS, MultiBody.MDate, MultiBody.MultiID, Products.SaleAcNo

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID INNER JOIN

                      Products ON MultiBody.ProductID = Products.ProductID

WHERE     (1 = 1)

GROUP BY MultiBody.IsVLG, MultiBody.SaleID, MultiBody.MDate, MultiBody.MultiID, Products.SaleAcNo)) MI ON MI.SaleID = Sales.SaleID 

WHERE     (Sales.IsPosted = 1)

GROUP BY MI.MultiID , MI.MDate , Sales.IsPosted, Sales.Narration, Sales.IsApplyPartyMarkup, MI.SaleAcNo

UNION ALL





SELECT     'MI' AS VOUCHERTYPE, MI.MultiID AS VoucherNo,  MI.MDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, Sales.CustomerID AS AccountNo, ISNULL(Sales.Narration, ' ') + ', ' + ISNULL(Sales.NarrationUrdu, N'') 

                      AS Narration, 

                     SUM(MI.AmountS) - ISNULL(Sales.ExpMinus, 

                      0) + ISNULL(Sales.ExpPlus, 0) + (SUM(MI.AmountS) * IsNull(Sales.ComRatio, 0) / 100) 

                      

                      AS Debit, 

                      Sales.ReceivedAmount + Sales.SpecialDiscount + SUM(isnull(MI.VL,0)) AS Credit, 

                      ISNULL(Sales.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup, null CropSeason 

FROM        

                      Sales INNER JOIN

                          (SELECT     MultiBody.SaleID, CASE WHEN (ISNULL(MultiBody.IsVL,0) = 0 and  ISNULL(MultiBody.IsVLG,0) = 0) THEN SUM(ISNULL(MultiBody.TPTEx, 0) + ISNULL(MultiBody.TPTM, 0)) ELSE 0 END AS VL, MultiBody.MultiID, MultiBody.MDate, 

                      SUM(MultiBody.AmountS)AmountS

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID

WHERE     (1 = 1) 

GROUP BY MultiBody.SaleID, MultiBody.MultiID, MultiBody.MDate,MultiBody.IsVL,MultiBody.IsVLG



) AS MI ON  MI.SaleID = Sales.SaleID

WHERE     (Sales.IsPosted = 1)

GROUP BY MI.MultiID , MI.MDate , Sales.IsPosted, Sales.Narration, Sales.ComRatio, Sales.ExpPlus, 

                      Sales.ExpMinus, Sales.CustomerID, Sales.ReceivedAmount, Sales.SpecialDiscount, Sales.IsApplyPartyMarkup, Sales.NarrationUrdu





    /********************* START PRODUCT GENERAL MULTI SUGARCANE*************************************/



UNION ALL

SELECT     'MI' AS VOUCHERTYPE, MultiBody.MultiID, Sales.SaleDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate,

                       MultiBody.VehicleAccNo, 

                      'Multi Inv. Sugarcane: Customer:' + ChartOfAccounts.AccountName  + ', Ident=' + MultiBody.IndentNo + ', Token:' + MultiBody.TokenNo + ', GW:' + CAST(CAST(MultiBody.GWeight AS float) 

                      AS varchar(10)) + ', GW-Mnd:' + dbo.KgsToMounds(CAST(MultiBody.GWeight AS decimal(18,9)),MultiBody.Multiplier)  AS NARRATION, 0 AS DEBIT, SUM(ISNULL(MultiBody.TPTEx, 0)) AS CREDIT, 0 AS IsApplyPartyMarkup

                      , null CropSeason 

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID INNER JOIN

                      ChartOfAccounts ON Sales.CustomerID = ChartOfAccounts.AccountNo

WHERE     (1 = 1) AND (ISNULL(MultiBody.IsVL, 0) = 1 OR

                      ISNULL(MultiBody.IsVLG, 0) = 1)

GROUP BY MultiBody.MultiID, Sales.SaleDate, MultiBody.VehicleAccNo, MultiBody.IsVL, MultiBody.IndentNo, MultiBody.TokenNo, MultiBody.GWeight, 

                      ChartOfAccounts.AccountName,MultiBody.Multiplier



UNION ALL



SELECT     'MI' AS VOUCHERTYPE, MultiBody.MultiID, Sales.SaleDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate,

                       '52061' AS Expr1, 

                      'Multi Inv. Sugarcane: Ident=' + MultiBody.IndentNo + ', Token:' + MultiBody.TokenNo + ', GW:' + CAST(CAST(MultiBody.GWeight AS float) 

                      AS varchar(10)) + ', VL#' + MultiBody.VehicleNo  AS Expr2, 

                      --SUM(ISNULL(MultiBody.TPTEx, 0)) 



CASE WHEN (ISNULL(MultiBody.IsVLG,0) = 0) THEN SUM(ISNULL(MultiBody.TPTEx, 0)) ELSE 0 END

                      AS DEBIT

                      , 0 AS CREDIT, 0 AS IsApplyPartyMarkup

                      , null CropSeason 

FROM         MultiBody INNER JOIN

                      Sales ON MultiBody.SaleID = Sales.SaleID

WHERE     (1 = 1) AND (ISNULL(MultiBody.IsVLG, 0) = 0)

GROUP BY MultiBody.MultiID, Sales.SaleDate, MultiBody.VehicleAccNo, MultiBody.IsVL, MultiBody.IndentNo, MultiBody.TokenNo, MultiBody.GWeight, 

                      MultiBody.VehicleNo,MultiBody.IsVLG

    

    /********************* END PRODUCT GENERAL MULTI SUGARCANE *************************************/

UNION ALL

/* ------Sale WH-Tax  Acc--- 

SELECT     'BMI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ROUND( 

Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax

Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END,0)

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTax,0) = 1) and ISNULL(IsMultiNew,0) = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL



SELECT     'BMI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration,

 0



AS Debit, ROUND( 

 Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax

Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTax,0) = 1) and ISNULL(IsMultiNew,0) = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL



SELECT     'BMI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, 

SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg  

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTKg,0) = 1) and ISNULL(IsMultiNew,0) = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)



UNION ALL



SELECT     'BMI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup,  CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTKg,0) = 1) and ISNULL(IsMultiNew,0) = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup,  CropSeason 

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)





UNION ALL

*/

/*********** CONTRACT PURCHASE INVOICE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/





SELECT     'BMI' AS VOUCHERTYPE, BMI.MultiID VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, ContractInvoice.PartyID AS AccountNo, 'Brokerage Multi Purchase' + ISNULL(ContractInvoice.Narration, '') +', '+ ISNULL(NarrationUrdu,N'') 

                      AS Narration, ContractInvoice.Received  AS Debit, round(ISNULL(ContractInvoice.NetValue - (CASE WHEN dbo.RegistryValue('StopApplyGrossToPartyAccount') = 1 AND 

                      isnull(ContractInvoice.BagVendorID, '') = '' THEN SUM(ContractInvoiceBody.TotalValue) 

                      + SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) WHEN ISNULL(ContractInvoice.BagVendorID, '') 

                      <> '' THEN SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) ELSE 0 END) + ContractInvoice.Received, 0),0) 

                      

                      + 

ISNULL((CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges,0)  = 1 THEN 

(case when ContractInvoice.AddVehicleCharges  = 1 then -VehicalCharges when MinusVehicleCharges = 1 then VehicalCharges end)

else

0 

end),0)

                      AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

INNER JOIN (SELECT MultiID, PurchaseID FROM MultiInvoice) BMI ON BMI.PurchaseID = ContractInvoice.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.IsMultiNew, 0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.PartyID, ContractInvoice.Received, ContractInvoice.Narration, 

                      ContractInvoice.NetValue, ContractInvoice.IsApplyPartyMarkup, ContractInvoice.BagVendorID,NarrationUrdu

,ContractInvoice.SepVehicalCharges,ContractInvoice.AddVehicleCharges  ,VehicalCharges

    ,MinusVehicleCharges , ContractInvoice.CropSeason ,BMI.MultiID

UNION ALL





/*********** CONTRACT SALE Invoice NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/

SELECT 'BMI' AS VOUCHERTYPE, BMI.MultiID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



    AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, isnull(ContractInvoice.SellerID,ContractInvoice.PartyID) AS AccountNo, 

    

    'Brokerage Multi Sale' + IsNull(ContractInvoice.Narration,'')+', '+ ISNULL(NarrationUrdu,N'') AS Narration,     

ROUND(ISNULL(ContractInvoice.NetValue -(case when dbo.RegistryValue('StopApplyGrossToPartyAccount')=1 then 

SUM(ContractInvoiceBody.TotalValue) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) else 0 end), 0) 

+ ContractInvoice.Received ,0 ) 

+ 

ISNULL((CASE WHEN ISNULL(ContractInvoice.SepVehicalCharges,0)  = 1 THEN 

(case when ContractInvoice.AddVehicleCharges  = 1 then -VehicalCharges when MinusVehicleCharges = 1 then VehicalCharges end)

else

0 

end),0)

 AS Debit, ContractInvoice.Received As Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

 , CropSeason 

    FROM ContractInvoiceBody INNER JOIN ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

    LEFT JOIN (SELECT MultiID, SaleID FROM MultiInvoice) BMI ON BMI.SaleID = ContractInvoice.ContractPurId

    WHERE (ContractInvoice.IsSaleInvoice = 1)  AND (ISNULL(ContractInvoice.IsMultiNew, 0) = 1)

    GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.SellerID, ContractInvoice.Received, 



    ContractInvoice.Narration, ContractInvoice.NetValue, IsApplyPartyMarkup,ContractInvoice.PartyID,NarrationUrdu

    ,ContractInvoice.SepVehicalCharges,ContractInvoice.AddVehicleCharges  ,VehicalCharges, CropSeason ,BMI.MultiID

    ,MinusVehicleCharges



/*

UNION ALL





SELECT DISTINCT 

                      'BMI' AS VOUCHERTYPE, BMI.MultiID AS VoucherNo, BMI.MultiDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(BMI.PLSAccount,'') = '' THEN '40001' ELSE BMI.PLSAccount END AS AccountNo,  'Profit Brokerage Multi Inovice' AS Narration, CASE WHEN (SAL.TotalValueS) - (PUR.TotalValueP) 

                      < 0 THEN ABS((SAL.TotalValueS) - (PUR.TotalValueP)) ELSE 0 END AS DEBIT, CASE WHEN (SAL.TotalValueS) - (PUR.TotalValueP) 

                      > 0 THEN (SAL.TotalValueS) - (PUR.TotalValueP) ELSE 0 END AS CREDIT, 0 AS IsApplyPartyMarkup, BMI.CropSeason 

FROM         MultiInvoice AS BMI INNER JOIN

(

SELECT     BMIP.MultiID, PUR.VehicleNo, SUM(PUR.TotalKgsPur) AS TotalKgsPur, AVG(PUR.RatePur) AS RatePur, SUM(PUR.TotalValueP) AS TotalValueP

FROM        

                          (SELECT     CIP.ContractPurId, CIP.CDate, CIBP.VehicleNo, SUM(CIBP.TotalKgs) AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, 

                                                   ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) * CIBP.Bardana), 0) + MAX(ConExp.TTLExps) 

                                                   AS TotalValueP

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId INNER JOIN

                                                       (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

                                                                                 (dbo.FunGetAllExpPur(ContractInvoiceBody_1.ContractPurId) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusPur(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2



                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo) AS ConExp ON 

                                                   ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                            WHERE      (CIP.IsSaleInvoice = 0) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.VehicalCharges, CIBP.VehicleNo) AS PUR 

                            INNER JOIN (SELECT DISTINCT MultiID, PurchaseID FROM MultiInvoice ) AS BMIP ON PUR.ContractPurId = BMIP.PurchaseID

GROUP BY BMIP.MultiID, PUR.VehicleNo

)PUR ON PUR.MultiID = BMI.MultiID

INNER JOIN (

SELECT   DISTINCT  BMIS.MultiID, PUR.VehicleNo, (PUR.TotalKgsPur) AS TotalKgsSAL, (PUR.RatePur) AS RateSAL, (PUR.TotalValueP) AS TotalValueS

FROM        -- MultiInvoice AS BMIS INNER JOIN

                          (SELECT     CIP.ContractPurId, CIP.CDate, CIBP.VehicleNo, SUM(CIBP.TotalKgs) AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, 

                                                   ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) * CIBP.Bardana), 0) + MAX(ConExp.TTLExps) 

                                                   AS TotalValueP

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId INNER JOIN

                                                       (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

                                                                                 (dbo.FunGetAllExpSale(ContractInvoiceBody_1.ContractPurId) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusSale(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo) AS ConExp ON 

                                                   ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                            WHERE      (CIP.IsSaleInvoice = 1) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.VehicalCharges, CIBP.VehicleNo) AS PUR --ON PUR.ContractPurId = BMIS.SALEID

                            INNER JOIN (SELECT DISTINCT MultiID, SALEID FROM MultiInvoice ) AS BMIS ON PUR.ContractPurId = BMIS.SALEID

) SAL ON SAL.MultiID = BMI.MultiID

--GROUP BY BMI.MultiID , BMI.MultiDate





SELECT DISTINCT 

                      'BMI' AS VOUCHERTYPE, BMI.MultiID AS VoucherNo, BMI.MultiDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(BMI.PLSAccount,'') = '' THEN '40001' ELSE BMI.PLSAccount END AS AccountNo,  'Profit Brokerage Multi Inovice' AS Narration, CASE WHEN (SAL.TotalValueS) - (PUR.TotalValueP) 

                      < 0 THEN ABS((SAL.TotalValueS) - (PUR.TotalValueP)) ELSE 0 END AS DEBIT, CASE WHEN (SAL.TotalValueS) - (PUR.TotalValueP) 

                      > 0 THEN (SAL.TotalValueS) - (PUR.TotalValueP) ELSE 0 END AS CREDIT, 0 AS IsApplyPartyMarkup, BMI.CropSeason 

FROM         MultiInvoice AS BMI INNER JOIN

(

SELECT     BMIP.MultiID, PUR.VehicleNo, SUM(PUR.TotalKgsPur) AS TotalKgsPur, AVG(PUR.RatePur) AS RatePur, SUM(PUR.TotalValueP) AS TotalValueP

FROM        

                          (SELECT     CIP.ContractPurId, CIP.CDate, CIBP.VehicleNo, SUM(CIBP.TotalKgs) AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, 

                                                   ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) * CIBP.Bardana), 0) + MAX(ConExp.TTLExps) 

                                                   AS TotalValueP

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId INNER JOIN

                                                       (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

                                                                                 (ROUND(MAX(EXPP.TTLValue),0) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusPur(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2



                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                                                INNER JOIN (SELECT        VoucherID,  Narration, TTLValue

FROM            ZNarration

WHERE VoucherType = 'BMIP') EXPP ON EXPP.VoucherID = ContractInvoiceBody_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo) AS ConExp ON 

                                                   ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                            WHERE      (CIP.IsSaleInvoice = 0) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.VehicalCharges, CIBP.VehicleNo) AS PUR 

                            INNER JOIN (SELECT DISTINCT MultiID, PurchaseID FROM MultiInvoice ) AS BMIP ON PUR.ContractPurId = BMIP.PurchaseID

GROUP BY BMIP.MultiID, PUR.VehicleNo

)PUR ON PUR.MultiID = BMI.MultiID

INNER JOIN (

SELECT   DISTINCT  BMIS.MultiID, PUR.VehicleNo, (PUR.TotalKgsPur) AS TotalKgsSAL, (PUR.RatePur) AS RateSAL, (PUR.TotalValueP) AS TotalValueS

FROM        -- MultiInvoice AS BMIS INNER JOIN

                          (SELECT     CIP.ContractPurId, CIP.CDate, CIBP.VehicleNo, SUM(CIBP.TotalKgs) AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, 

                                                   ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) * CIBP.Bardana), 0) + MAX(ConExp.TTLExps) 

                                                   AS TotalValueP

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId INNER JOIN

                                                       (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

                                                                                 (MAX(ROUND(EXPS.TTLValue,0)) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusSale(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                                                INNER JOIN (SELECT        VoucherID,  Narration, TTLValue

FROM            ZNarration

WHERE VoucherType = 'BMIS') EXPS ON EXPS.VoucherID = ContractInvoice_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo) AS ConExp ON 

                                                   ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                            WHERE      (CIP.IsSaleInvoice = 1) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.VehicalCharges, CIBP.VehicleNo) AS PUR --ON PUR.ContractPurId = BMIS.SALEID

                            INNER JOIN (SELECT DISTINCT MultiID, SALEID FROM MultiInvoice ) AS BMIS ON PUR.ContractPurId = BMIS.SALEID

) SAL ON SAL.MultiID = BMI.MultiID

--delete FROM MultiInvoice WHERE MultiID IN(174,164,379,398)

*/
GO

-- View: VuAccVouchersOthersMultiPLS
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersMultiPLS]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersMultiPLS];
GO
CREATE VIEW [dbo].[VuAccVouchersOthersMultiPLS]  AS 



SELECT DISTINCT --PUR.*

                      'BMI' AS VOUCHERTYPE, BMI.MultiID AS VoucherNo, BMI.MultiDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, CASE WHEN ISNULL(BMI.PLSAccount,'') = '' THEN '40001' ELSE BMI.PLSAccount END AS AccountNo,  'Profit Brokerage Multi Inovice' AS Narration, CASE WHEN (SAL.TotalValueS) - (PUR.TotalValueP) 

                      < 0 THEN ABS((SAL.TotalValueS) - (PUR.TotalValueP))  + ISNULL(PUR.CharityValue,0)  ELSE 0 END AS DEBIT, CASE WHEN (SAL.TotalValueS) - (PUR.TotalValueP) 

                      > 0 THEN (SAL.TotalValueS) - (PUR.TotalValueP) - ISNULL(PUR.CharityValue,0) ELSE 0 END AS CREDIT, 0 AS IsApplyPartyMarkup, BMI.CropSeason 

FROM         MultiInvoice AS BMI INNER JOIN

(

SELECT     BMIP.MultiID, PUR.VehicleNo, SUM(PUR.TotalKgsPur) AS TotalKgsPur, AVG(PUR.RatePur) AS RatePur, SUM(PUR.TotalValueP) AS TotalValueP, SUM(PUR.CharityValue) AS CharityValue

FROM        

                          (SELECT     CIP.ContractPurId, CIP.CDate, CIBP.VehicleNo, SUM(CIBP.TotalKgs) AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, 

                                                   ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) * CIBP.Bardana), 0) --+ MAX(ConExp.TTLExps) 

                                                   

                                                   +(SELECT     

                                                                                 (ROUND((EXPP.TTLValue),0) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusPur(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2



                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 



                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                                                INNER JOIN (SELECT        VoucherID,  Narration, TTLValue

FROM            ZNarration

WHERE VoucherType = 'BMIP') EXPP ON EXPP.VoucherID = ContractInvoiceBody_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         AND ContractInvoiceBody_1.ContractPurId=CIP.ContractPurId AND ContractInvoiceBody_1.VehicleNo =CIBP.VehicleNo

                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo,EXPP.TTLValue)

                                                   AS TotalValueP,ISNULL(CIP.CharityValue, 0) CharityValue

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId 

--                                                   INNER JOIN

--                                                       (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

--                                                                                 (ROUND((EXPP.TTLValue),0) /

--                                                                                    (SELECT     COUNT(*) AS Expr1

--                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

--                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

--                                                                                    (SELECT     COUNT(*) AS Expr1

--                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

--                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

--                                                                                --+ dbo.FunGetAllExpPlusPur(ContractInvoiceBody_1.ContractPurId) /

--                                                                                --    (SELECT     COUNT(*) AS Expr1

--                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

--                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

--                                                                                --    (SELECT     COUNT(*) AS Expr1

--                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2



--                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

--                                                                                AS TTLExps

--                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

--                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

--                                                                                INNER JOIN (SELECT        VoucherID,  Narration, TTLValue

--FROM            ZNarration

--WHERE VoucherType = 'BMIP') EXPP ON EXPP.VoucherID = ContractInvoiceBody_1.ContractPurId

--                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

--                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo,EXPP.TTLValue) AS ConExp ON 

--                                                   ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                            WHERE      (CIP.IsSaleInvoice = 0) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.VehicalCharges, CIBP.VehicleNo,CIP.CharityValue) AS PUR 

                            INNER JOIN (SELECT DISTINCT MultiID, PurchaseID FROM MultiInvoice ) AS BMIP ON PUR.ContractPurId = BMIP.PurchaseID

GROUP BY BMIP.MultiID, PUR.VehicleNo

)PUR ON PUR.MultiID = BMI.MultiID

INNER JOIN (

SELECT   DISTINCT  BMIS.MultiID, PUR.VehicleNo, (PUR.TotalKgsPur) AS TotalKgsSAL, (PUR.RatePur) AS RateSAL, (PUR.TotalValueP) AS TotalValueS

FROM        -- MultiInvoice AS BMIS INNER JOIN

                          (

                          

SELECT     CIP.ContractPurId, CIP.CDate, CIBP.VehicleNo, SUM(CIBP.TotalKgs) AS TotalKgsPur, AVG(CIBP.RatePerMound) AS RatePur, 

                                                   ROUND(SUM(ROUND(CIBP.TotalValue, 2) + (CIBP.BagsQty + CIBP.PQty) * CIBP.Bardana), 0) ---+ (ConExp.TTLExps) 

                                                   +(SELECT      

                                                                                 ((ROUND(EXPS.TTLValue,0)) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusSale(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                                                INNER JOIN (SELECT        VoucherID,  Narration, TTLValue

FROM            ZNarration

WHERE VoucherType = 'BMIS') EXPS ON EXPS.VoucherID = ContractInvoice_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         AND ContractInvoiceBody_1.ContractPurId = CIP.ContractPurId AND ContractInvoiceBody_1.VehicleNo = CIBP.VehicleNo



                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo,EXPS.TTLValue)

                                                   AS TotalValueP

                                                   

                            FROM          ContractInvoiceBody AS CIBP INNER JOIN

                                                   ContractInvoice AS CIP ON CIBP.ContractPurId = CIP.ContractPurId 

                                                   

--                                                   INNER JOIN

--                                                       (SELECT     ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo, 

--                                                                                 ((ROUND(EXPS.TTLValue,0)) /

--                                                                                    (SELECT     COUNT(*) AS Expr1

--                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

--                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

--                                                                                    (SELECT     COUNT(*) AS Expr1

--                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

--                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

--                                                                                --+ dbo.FunGetAllExpPlusSale(ContractInvoiceBody_1.ContractPurId) /

--                                                                                --    (SELECT     COUNT(*) AS Expr1

--                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

--                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

--                                                                                --    (SELECT     COUNT(*) AS Expr1

--                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

--                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

--                                                                                AS TTLExps

--                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

--                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

--                                                                                INNER JOIN (SELECT        VoucherID,  Narration, TTLValue

--FROM            ZNarration

--WHERE VoucherType = 'BMIS') EXPS ON EXPS.VoucherID = ContractInvoice_1.ContractPurId

--                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

--                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo,EXPS.TTLValue) AS ConExp ON 

--                                                   ConExp.ContractPurId = CIP.ContractPurId AND ConExp.VehicleNo = CIBP.VehicleNo

                            WHERE      (CIP.IsSaleInvoice = 1) AND (ISNULL(CIP.IsMultiNew, 0) = 1)

                            GROUP BY CIP.ContractPurId, CIP.CDate, CIP.VehicalCharges, CIBP.VehicleNo

                            

                            ) AS PUR --ON PUR.ContractPurId = BMIS.SALEID

                            INNER JOIN (SELECT DISTINCT MultiID, SALEID FROM MultiInvoice ) AS BMIS ON PUR.ContractPurId = BMIS.SALEID

) SAL ON SAL.MultiID = BMI.MultiID --AND SAL.VehicleNo = PUR.VehicleNo
GO

-- View: VuAccVouchersOthersNewIDs
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersNewIDs]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersNewIDs];
GO
CREATE VIEW [dbo].[VuAccVouchersOthersNewIDs]



AS



/*************** BROKERAGE Shortage Adjustment********************************************/



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, VendorID AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, ROUND(PurchaseRate * (ShortageInKgs/37.324), 4) AS Debit, 0 AS Credit, 

                      0 AS IsApplyPartyMarkup

FROM         BrokrageShortage



UNION ALL



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, CustomerID AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, 0 AS Debit, ROUND(SaleRate *((ShortageInKgs-ISNULL(CommissionWeight,0))/37.324), 4) AS Credit, 

                      0 AS IsApplyPartyMarkup

FROM         BrokrageShortage 



UNION ALL



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, dbo.RegistryValue('BrokrageShortageRevenue') AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, 0 AS Debit, ROUND(SaleRate * (CommissionWeight/37.324), 4) AS Credit, 

                      0 AS IsApplyPartyMarkup

FROM         BrokrageShortage

Where ISNULL(CommissionWeight,0) > 0



/*****************************************************************************************/



UNION ALL



/*************** BROKERAGE PURCHASE EXPENSES********************************************/



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '46' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.CommissionValue)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)



UNION ALL



/***Purchase Discount income***/

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '5124' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.Discount)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.Discount) > 0)



UNION ALL

/***********/



/* ------Pur Arhat Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '47' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusArhat,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, IsApplyPartyMarkup























HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '47' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddArhat,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL

/*--------------------*/



/* ------Pur-With Holding Tax Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '2301' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      (ContractInvoice.CommissionValue * Isnull(ContractInvoice.WHTax, 0) / 100) AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

Where (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddWHTax,0) = 0)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.WHTax

HAVING      Isnull(ContractInvoice.WHTax, 0) > 0



UNION ALL





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '2301' AS AccountNo, ContractInvoice.Narration, (ContractInvoice.CommissionValue * Isnull(ContractInvoice.WHTax, 0) / 100) AS Debit, 

                      0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

Where (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddWHTax,0) = 1)



































































































































GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.WHTax

HAVING      Isnull(ContractInvoice.WHTax, 0) > 0



UNION ALL



/*--------------------*/



/* ------Pur Changrai Acc--- */





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, dbo.RegistryValue('PurChangrai') AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusChangrai,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, dbo.RegistryValue('PurChangrai') AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddChangrai,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)



UNION ALL



/* ------Pur Sootli Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57020' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      (SUM(ContractInvoiceBody.BagsQty) + SUM(ISNULL(ContractInvoiceBody.PQty, 0))) * ContractInvoice.SaleSootli AS Credit,

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusSootli, 0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli, ContractInvoice.IsApplyPartyMarkup



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57020' AS AccountNo, ContractInvoice.Narration, 

                      (SUM(ContractInvoiceBody.BagsQty) + SUM(ISNULL(ContractInvoiceBody.PQty, 0))) * ContractInvoice.SaleSootli AS Debit, 0 AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusSootli, 0) = 0)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli, ContractInvoice.IsApplyPartyMarkup



UNION ALL



/* ------Pur Mazdoori Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57002' AS AccountNo, ContractInvoice.Narration, 

    0 AS Debit, 

CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurMazdori / 100  

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.PurMazdori Else 

    (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurMazdori  End 

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusMazdori,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent

HAVING      (ContractInvoice.PurMazdori > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57002' AS AccountNo, ContractInvoice.Narration, 



    CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurMazdori / 100  

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+ SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.PurMazdori Else 

    (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurMazdori  End

 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddMazdori,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent

HAVING      (ContractInvoice.PurMazdori > 0)

/* --------- */



UNION ALL



/* ------Pur Dalali Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57003' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusDalali,0) = 1) 



 GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup







HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 > 0) 



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57003' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN



                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddDalali,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 > 0) 



UNION ALL

/*---------------*/



/* ------Pur Bank Charges Acc--- */

SELECT     'BP' AS VOUCHERTYPE, PurchaseID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57016' AS AccountNo, Narration, 0 AS Debit, BankChargesAmount AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(MinusBankCharges, 0) = 1) AND (ISNULL(BankChargesAmount, 0) > 0)

GROUP BY PurchaseID, CDate, Narration, PurPulliGhya, IsApplyPartyMarkup, BankChargesAmount



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, PurchaseID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57016' AS AccountNo, Narration, BankChargesAmount AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(AddBankCharges, 0) = 1) AND (ISNULL(BankChargesAmount, 0) > 0)



/*---------------*/

UNION ALL

/* ------ Pur Ref Commission Acc--- */

SELECT     'BP' AS VOUCHERTYPE, PurchaseID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(RefCommValue, 0) > 0)





UNION ALL



SELECT     'BP' AS VOUCHERTYPE, PurchaseID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(RefCommValue, 0) > 0)





--UNION ALL



/* SELECT     'BP' AS VOUCHERTYPE, PurchaseID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddCommission,0) = 1) AND (ISNULL(RefCommValue, 0) > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, PurchaseID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddCommission,0) = 1) AND (ISNULL(RefCommValue, 0) > 0) */





/*---------------*/

UNION ALL



/* ------Pur Carriage Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57004' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, (SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusPuliGhya,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurPulliGhya, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      ((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57004' AS AccountNo, ContractInvoice.Narration, 

                      (SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddPuliGhya,0) = 1) GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurPulliGhya, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      ((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya > 0)



UNION ALL

/*--------------*/



/* ------Pur Commission Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4205' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0)  And (Isnull(ContractInvoice.MinusCommission,0) = 1) GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 > 0)



UNION ALL







SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4205' AS AccountNo, ContractInvoice.Narration,                        SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddCommission,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 > 0)



UNION ALL

/*-------------*/



/* ------Pur Manshyana Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57006' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusManshyana,0) = 1)



GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57006' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddManshyana,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL

/*-----------------*/



/* ------Pur Loading Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57007' AS AccountNo, ContractInvoice.Narration, 

          0 AS Debit, case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.PurLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusLoading,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurLoading > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57007' AS AccountNo, ContractInvoice.Narration, 

          case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.PurLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading end  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddLoading,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurLoading > 0)



UNION ALL

/*--------------*/



/* ------Pur Un-Loading Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57008' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.PurUnLoading * (Sum(ContractInvoiceBody.BagsQty)+ Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading  end   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN



                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID



WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusUnLoading,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurUnLoading, ContractInvoice.UnLoadingPerBag, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurUnLoading > 0)

 

UNION ALL 



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57008' AS AccountNo, ContractInvoice.Narration, 

                      case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.PurUnLoading * (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading  end  AS Debit,  0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddUnLoading,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurUnLoading, ContractInvoice.UnLoadingPerBag, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurUnLoading > 0)

 

UNION ALL 

/*--------------*/



/* ------Pur Vehicle Charges Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ContractInvoice.VehicalCharges AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57009' AS AccountNo, ContractInvoice.Narration, 

                      ContractInvoice.VehicalCharges AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddVehicleCharges,0) = 1)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL



/* ------Pur Market Fees Acc--- */

--SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

--                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57010' AS AccountNo, ContractInvoice.Narration, 

--                      0 AS Debit, SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

--FROM         ContractInvoice INNER JOIN

--                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

--WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusMarketFees,0) = 1)

--GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

--HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57010' AS AccountNo, ContractInvoice.Narration , 

                      ISNULL(ContractInvoice.AddPurMarketFee, 0) AS Debit, ISNULL(ContractInvoice.MinusPurMarketFee, 0) AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, 

                      ContractInvoice.IsApplyPartyMarkup, ContractInvoice.AddPurMarketFee, ContractInvoice.MinusPurMarketFee

HAVING      (ISNULL(ContractInvoice.AddPurMarketFee, 0) > 0) OR

                      (ISNULL(ContractInvoice.MinusPurMarketFee, 0) > 0)

--

--UNION ALL

--

--SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

--                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57010' AS AccountNo, ContractInvoice.Narration, 

--                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

--FROM         ContractInvoice INNER JOIN

--                      ContractInvoiceBody ON ContractInvoice.PurchaseID = ContractInvoiceBody.PurchaseID

--WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddMarketFees,0) = 1)

--GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

--HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL

/*-----------------*/



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4102' AS AccountNo, ContractInvoice.Narration, 

                      (CASE WHEN IsPay=0 THEN SUM(ContractInvoice.CommissionValue) ELSE 0 END) AS Debit, (CASE WHEN IsPay=1 THEN SUM(ContractInvoice.CommissionValue) ELSE 0 END)   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0)

GROUP BY ContractInvoice.PurchaseID, IsPay, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)









UNION ALL /*************** BROKERAGE SALE EXPENSES********************************************/



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4103' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.CommissionValue)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 0)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)



UNION ALL



/*****Discount On Sale exp******/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '48' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.Discount) AS Debit, 0  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 0)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.Discount) > 0)



UNION ALL

/****************/







SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4103' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.CommissionValue) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)





UNION ALL

/* ------Sale Bank Charges Acc--- */

SELECT     'BS' AS VOUCHERTYPE, SaleID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58016' AS AccountNo, Narration, 0 AS Debit, BankChargesAmount AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(BankChargesAmount, 0) > 0) AND (ISNULL(AddBankCharges, 0) = 1)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, SaleID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58016' AS AccountNo, Narration, BankChargesAmount AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(BankChargesAmount, 0) > 0) AND (ISNULL(MinusBankCharges, 0) = 1)

/*--------------------*/

UNION ALL

/* ------Sale Ref Commission Acc--- */

SELECT     'BS' AS VOUCHERTYPE, SaleID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1)  AND (ISNULL(RefCommValue, 0) > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, SaleID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(RefCommValue, 0) > 0)





--UNION ALL



/*SELECT     'BS' AS VOUCHERTYPE, SaleID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(MinusCommission, 0) = 1) AND (ISNULL(RefCommValue, 0) > 0)







UNION ALL



SELECT     'BS' AS VOUCHERTYPE, SaleID AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(MinusCommission, 0) = 1) AND (ISNULL(RefCommValue, 0) > 0) */





/*--------------------*/



UNION ALL

/* ------Sale Market Fees Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58001' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddMarketFees,0) = 1)



GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58001' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusMarketFees,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL

/*--------------------*/



/* ------Sale Mazdoori Acc--- */



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58002' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit,

CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleMazdori / 100

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.SaleMazdori Else 

        (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori End  

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddMazdori,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent

HAVING      ((Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58002' AS AccountNo, ContractInvoice.Narration, 



CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleMazdori / 100

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+ SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.SaleMazdori Else 

        (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori End  

AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN



                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusMazdori,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent

HAVING      ((Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori > 0)



UNION ALL

/*-------------------*/



/* ------Sale Dalali Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58003' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddDalali,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58003' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusDalali,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)



UNION ALL

/*---------------------*/



/* ------Sale Sootli Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58004' AS AccountNo, ContractInvoice.Narration, 



                      0 AS Debit, (SUM(ContractInvoiceBody.BagsQty)+SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.SaleSootli  AS Credit, 0 AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddSootli,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli

HAVING      ((SUM(ContractInvoiceBody.BagsQty)+SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.SaleSootli > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58004' AS AccountNo, ContractInvoice.Narration, 

                      (SUM(ContractInvoiceBody.BagsQty)+SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.SaleSootli AS Debit, 0 AS Credit, 0 AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusSootli,0) = 1)



GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli

HAVING      ((SUM(ContractInvoiceBody.BagsQty)+SUM(Isnull(ContractInvoiceBody.PQty,0))) * ContractInvoice.SaleSootli > 0)



UNION ALL

/*------------------*/



/* ------Sale Commission Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4206' AS AccountNo, ContractInvoice.Narration, 



                      0 AS Debit, Round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100,0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddCommission,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 > 0)





UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4206' AS AccountNo, ContractInvoice.Narration, 

                      Round(SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusCommission,0) = 1) GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 > 0)



UNION ALL

/*---------------------*/



/* ------Sale Manshyana Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58006' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddManshyana,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58006' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusManshyana,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



/*----------------------*/



/* ------Sale Loading Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58007' AS AccountNo, ContractInvoice.Narration, 

        0 AS Debit, case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.SaleLoading* (Sum(ContractInvoiceBody.BagsQty)+ Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddLoading,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleLoading > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58007' AS AccountNo, ContractInvoice.Narration, 

        case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.SaleLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading end AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusLoading,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleLoading > 0)



UNION ALL

/*---------------------*/



/* ------Sale Un-Loading Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58008' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.SaleUnLoading * (Sum(ContractInvoiceBody.BagsQty)+ Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading end   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddUnLoading,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleUnLoading, ContractInvoice.UnLoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleUnLoading > 0)

 

UNION ALL 





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58008' AS AccountNo, ContractInvoice.Narration, 

                      case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.SaleUnLoading * (Sum(ContractInvoiceBody.BagsQty) + Sum(Isnull(ContractInvoiceBody.PQty,0)) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading end AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusUnLoading,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleUnLoading, ContractInvoice.UnLoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleUnLoading > 0)

 

UNION ALL 



/*--------------------*/







/* ------Sale WH-Tax  Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit,

Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax

Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTax,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration,

 

 Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax

Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END



AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTax,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, 

SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg  

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTKg,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTKg,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)





UNION ALL

/*-----------------*/



/* ------Sale VehicalCharges  Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58010' AS AccountNo, ContractInvoice.Narration, 

                      ContractInvoice.VehicalCharges AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup

HAVING      (ContractInvoice.VehicalCharges > 0)

 

UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58010' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ContractInvoice.VehicalCharges AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddVehicleCharges,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL

/*------ Telephone, Insurance, Fule and Stitching Acc-------------------*/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58011' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ContractInvoice.AddTelePhoneExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1)  And (Isnull(ContractInvoice.AddTelePhoneExp,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.AddTelePhoneExp, 

                      ContractInvoice.IsApplyPartyMarkup

HAVING      (ContractInvoice.AddTelePhoneExp > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58012' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                       ContractInvoice.AddInsuranceExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup



FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddInsuranceExp,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration,  ContractInvoice.AddInsuranceExp, 

                      ContractInvoice.IsApplyPartyMarkup

HAVING      ( ContractInvoice.AddInsuranceExp > 0)





UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58013' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddFuelExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddFuelExp,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddFuelExp, 

                      ContractInvoice.IsApplyPartyMarkup

HAVING      ( ContractInvoice.AddFuelExp > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58014' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddStitchingExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddStitchingExp,0) = 1)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddStitchingExp, 

                      ContractInvoice.IsApplyPartyMarkup

HAVING      ( ContractInvoice.AddStitchingExp > 0)



UNION ALL



----SALE Exp Stitching

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58014' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.Stitching, 0) < 0 then Abs(ISNULL(ContractInvoice.Stitching, 0)) Else 0 End  AS Debit,



Case When ISNULL(ContractInvoice.Stitching, 0) > 0 then ISNULL(ContractInvoice.Stitching, 0) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) 

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.Stitching, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.Stitching, 0)) > 0)





UNION ALL

-----SALE Exp  Rent Godown

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58019' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.RentGodown, 0) < 0 then Abs(ISNULL(ContractInvoice.RentGodown, 0)) Else 0 End  AS Debit,

Case When ISNULL(ContractInvoice.RentGodown, 0) > 0 then ISNULL(ContractInvoice.RentGodown, 0) Else 0 End   AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) 

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.RentGodown, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.RentGodown, 0)) > 0)





UNION ALL

------SALE Exp  ChokiDara

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58020' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ChokiDara, 0) < 0 then Abs(ISNULL(ContractInvoice.ChokiDara, 0)) Else 0 End  AS Debit,

Case When ISNULL(ContractInvoice.ChokiDara, 0) > 0 then ISNULL(ContractInvoice.ChokiDara, 0) Else 0 End   AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) 







GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ChokiDara, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.ChokiDara, 0)) > 0)

/************************************************************************/



UNION ALL

--- SALE Exp  ChartCharges

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58021' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ChartsCharges, 0) < 0 then Abs(ISNULL(ContractInvoice.ChartsCharges, 0)) Else 0 End    AS Debit,

Case When ISNULL(ContractInvoice.ChartsCharges, 0) > 0 then ISNULL(ContractInvoice.ChartsCharges, 0) Else 0 End AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) 

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ChartsCharges, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.ChartsCharges, 0)) > 0)



UNION ALL

-----SALE Exp  Bag Usage

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58022' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.BagUsage, 0) < 0 then Abs(ISNULL(ContractInvoice.BagUsage, 0)) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.BagUsage, 0) > 0 then ISNULL(ContractInvoice.BagUsage, 0) Else 0 End  AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) 

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.BagUsage, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.BagUsage, 0)) > 0)



UNION ALL

-------- SALE Exp  Shortage

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58023' AS AccountNo, ContractInvoice.Narration,   

Case When ISNULL(ContractInvoice.ShortageG, 0) < 0 then Abs(ISNULL(ContractInvoice.ShortageG, 0)) Else 0 End   AS Debit,

Case When ISNULL(ContractInvoice.ShortageG, 0) > 0 then ISNULL(ContractInvoice.ShortageG, 0) Else 0 End  AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) 

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.ShortageG, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.ShortageG, 0)) > 0)



UNION ALL

---- SALE Exp  Tractor Exp

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58024' AS AccountNo, ContractInvoice.Narration,   

 Case When ISNULL(ContractInvoice.TractorExp, 0) < 0 then Abs(ISNULL(ContractInvoice.TractorExp, 0)) Else 0 End  AS Debit,

Case When ISNULL(ContractInvoice.TractorExp, 0) > 0 then ISNULL(ContractInvoice.TractorExp, 0) Else 0 End    AS Credit,IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.SaleID = ContractInvoiceBody.SaleID

WHERE     (ContractInvoice.IsSaleInvoice = 1) 

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.TractorExp, IsApplyPartyMarkup

HAVING      (ABS(ISNULL(ContractInvoice.TractorExp, 0)) > 0)













UNION ALL 





/*************** CROPS PURCHASE ********************************************/



/****** CROP PURCHASE GROSS VALUE DEBIT TO CROP PURCHASE ACCOUNT  *********/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, CropsPurchaseHeader.PurchaseDate AS VoucherDate, 

                      '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 

case when isnull(Products.PurAcNo,'') = '' then dbo.RegistryValue('ATCropPurchaseAccount') else Products.PurAcNo end 

                      AS AccountNo, ISNULL(CropsPurchaseHeader.Remarks, ' ') AS Narration, 

                      SUM(CropsPurchaseBody.Weight / CropsWeighments.Multiplier * CropsPurchaseBody.Price) AS Debit, 0 AS Credit, 

                      ISNULL(CropsPurchaseHeader.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         CropsPurchaseBody INNER JOIN

                      CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN

                      CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID INNER JOIN

                      Products ON CropsPurchaseBody.ProductID = Products.ProductID

WHERE     (CropsPurchaseHeader.IsPosted = 1)

GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.Remarks, 

                      CropsPurchaseHeader.IsApplyPartyMarkup, Products.PurAcNo

UNION ALL



/******* CROP PURCHASE EXPMINUS CREDIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropPurchaseExpMinusAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ')  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.ExpMinus



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsPurchaseHeader 





WHERE     CropsPurchaseHeader.IsPosted = 1



UNION ALL



/******* CROP PURCHASE EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropPurchaseExpPlusAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



                      CropsPurchaseHeader.ExpPlus 

                       AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1



UNION ALL



/******* CROP PURCHASE PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ')  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.PaidAmount



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1





UNION ALL



/********** CROP PURCHASE COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropPurchaseComAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 

Case When ISNULL(ComPerMound, 0) = 1 then 

ISNULL(ComAmount, 0)



Else



( (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) * IsNull(CropsPurchaseHeader.ComRatio,0) / 100) End AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsPurchaseBody INNER JOIN

    CropsPurchaseHeader ON 

    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

     INNER JOIN

    CropsWeighments ON 

    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID

 WHERE     CropsPurchaseHeader.IsPosted = 1



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted,



                      CropsPurchaseHeader.ComRatio, CropsPurchaseHeader.Remarks, IsApplyPartyMarkup, ComAmount, ComPerMound



UNION ALL



/*********** CROP PURCHASE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo,  CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, CropsPurchaseHeader.VendorID AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration

, ISNULL(CropsPurchaseHeader.PaidAmount, 0) AS Debit, 



case when isnull(CropsPurchaseHeader.VendorIDCom,0) = 0 then



                      (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 

+ (Case When ISNULL(ComPerMound, 0) = 1 then iSNULL(ComAmount, 0)Else

(SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)  * IsNull(CropsPurchaseHeader.ComRatio,0) / 100)

End  )

else



                      ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 

+ (Case When ISNULL(ComPerMound, 0) = 1 then iSNULL(ComAmount, 0)Else

(SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)  * IsNull(CropsPurchaseHeader.ComRatio,0) / 100) End) )  

* (100-CropsPurchaseHeader.VendorComRatio) / 100 

 



end AS Credit



, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM CropsPurchaseBody INNER JOIN CropsPurchaseHeader ON CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID INNER JOIN CropsWeighments ON CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID

WHERE     CropsPurchaseHeader.IsPosted = 1

GROUP BY CropsPurchaseHeader.CropPurchaseID, ComAmount, ComPerMound, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted, IsApplyPartyMarkup

,CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio 

 , CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks,CropsPurchaseHeader.PaidAmount









UNION ALL





SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, CropsPurchaseHeader.VendorIDCom AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      ((SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 



                    - ISNULL(CropsPurchaseHeader.PaidAmount, 0) 

            



                      + (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price))



                      * IsNull(CropsPurchaseHeader.ComRatio,0) / 100) * CropsPurchaseHeader.VendorComRatio / 100  AS Credit



, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup







FROM CropsPurchaseBody INNER JOIN

    CropsPurchaseHeader ON 





    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

     INNER JOIN



    CropsWeighments ON 

    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID



WHERE     CropsPurchaseHeader.IsPosted = 1 and isnull(CropsPurchaseHeader.VendorIDCom,0) <> 0



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted, IsApplyPartyMarkup



              ,CropsPurchaseHeader.VendorIDCom, CropsPurchaseHeader.VendorComRatio, CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio 



              , CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks,CropsPurchaseHeader.PaidAmount



/********************* END CROP PURCHASE *************************************/

UNION ALL

/*************** CROPS SALES ********************************************/  /****** CROP SALES GROSS VALUE CREDIT TO CROP SALE ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 

case when isnull( Products.SaleAcNo,'') = '' then dbo.RegistryValue('ATCropSaleAccount') else Products.SaleAcNo end AS AccountNo, 

                      ISNULL(CropsSaleHeader.Remarks, ' ') AS Narration, 0 AS Debit, SUM((CropsSaleBody.Weight - CropsSaleBody.Deduction) 

                      / CropsSaleBody.Multiplier * CropsSaleBody.Price) AS Credit, ISNULL(CropsSaleHeader.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

                     

FROM         CropsSaleBody INNER JOIN

                      CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID INNER JOIN

                      Products ON CropsSaleBody.ProductID = Products.ProductID

WHERE     (CropsSaleHeader.IsPosted = 1)

GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDate, CropsSaleHeader.IsPosted, CropsSaleHeader.Remarks, 

                      CropsSaleHeader.IsApplyPartyMarkup, Products.SaleAcNo

UNION ALL



/******* CROP SALE EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleExpMinusAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ')  AS Narration, 



                      CropsSaleHeader.ExpMinus AS Debit, 

 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/******* CROP SALE EXPPLUS CREDIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleExpPlusAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      CropsSaleHeader.ExpPlus

  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/******* CROP SALE RECEIVEDAMOUNT DEBIT TO CASH ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ')  AS Narration, 



                      CropsSaleHeader.PaidAmount AS Debit, 

                       0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1





UNION ALL



/********** CROP SALE STAX CREDIT TO STAX ACCOUNT  *********/







SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleSTaxAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.STRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.STRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup



UNION ALL



/********** CROP SALE WHTAX CREDIT TO WHTAX ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleWHTaxAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.WHTRatio,0) / 100 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.WHTRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup



UNION ALL

/********** CROP SALE Commision Credit Account 4203  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleComAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 



                      0 AS Debit,

Case When ISNULl(ComPermound, 0) = 1 then 

ISNULL(ComAmount, 0)

Else

( (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio,0) / 100)) End  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.WHTRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup, CropsSaleHeader.ComRatio, CropsSaleHeader.ComAmount, CropsSaleHeader.ComPerMound





UNION ALL

/*********** CROP Sale NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, CropsSaleHeader.CustomerID AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 



                      (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      - ISNULL(CropsSaleHeader.ExpMinus, 0) 



                      + ISNULL(CropsSaleHeader.ExpPlus, 0) 

                     

                     + Case When ISNULL(ComPerMound, 0) = 1 then ISNULL(ComAmount, 0) Else ((SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio,0) / 100)) End 

--(SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) * IsNull(CropsSaleHeader.ComRatio,0) / 100)

            

                      + (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 



                      * IsNull(CropsSaleHeader.STRatio,0) / 100)



                      - (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 



                      * IsNull(CropsSaleHeader.WHTRatio,0) / 100) AS Debit,  ISNULL(CropsSaleHeader.PaidAmount, 0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN    CropsSaleHeader ON CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID WHERE     CropsSaleHeader.IsPosted = 1

GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted, IsApplyPartyMarkup , CropsSaleHeader.CustomerID, CropsSaleHeader.STRatio, CropsSaleHeader.WHTRatio 

     , CropsSaleHeader.ExpMinus, CropsSaleHeader.ExpPlus, CropsSaleHeader.Remarks,CropsSaleHeader.PaidAmount, CropsSaleHeader.ComRatio, CropsSaleHeader.ComAmount, CropsSaleHeader.ComPerMound











/********************* END CROP Sale *************************************/

UNION ALL

/*************** PRODUCT GENERAL OPENING ********************************************/



/****** PRODUCT OPENING GROSS VALUE DEBIT TO PRODUCT OPENING ACCOUNT  *********/



SELECT     'OP' AS VOUCHERTYPE, 1 AS VoucherNo, dbo.DefaultMinDate() AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, CASE WHEN isnull(Products.SaleAcNo, '') = '' THEN dbo.RegistryValue('ATOpeningAccount') 

                      ELSE Products.SaleAcNo END AS AccountNo, 'Opening Value ' + Products.ProductName AS Narration, SUM(OpeningStock.TTLValue) AS Debit, 0 AS Credit, 

                      1 AS IsApplyPartyMarkup

FROM         OpeningStock LEFT OUTER JOIN

                      Products ON OpeningStock.ProductID = Products.ProductID

group by Products.SaleAcNo,Products.ProductName



UNION ALL











/****** PRODUCT PURCHASE SALESTAX VALUE DEBIT TO PRODUCT PURCHASE STAX ACCOUNT  *********/



SELECT     'OP' AS VOUCHERTYPE, 1 AS VoucherNo, 



                      dbo.DefaultMinDate() AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATOpeningSTaxAccount') AS AccountNo, 'Opening STax' AS Narration, 



                      SUM(TTLSalesTax) AS Debit, 



              0 AS Credit, 1 as IsApplyPartyMarkup



FROM OpeningStock



UNION ALL 

/*************** PRODUCT GENERAL PURCHASE ********************************************/



/****** PRODUCT PURCHASE GROSS VALUE DEBIT TO PRODUCT PURCHASE ACCOUNT  *********/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, case when ISNULL(Products.PurAcNo, '') = '' then dbo.RegistryValue('ATPurchaseAccount') else Products.PurAcNo end 

 AS AccountNo, ISNULL(Purchase.Narration, ' ') AS Narration, 

                      SUM(PurchaseBody.TTLValue) AS Debit, 0 AS Credit, ISNULL(Purchase.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         PurchaseBody INNER JOIN

                      Purchase ON PurchaseBody.PurchaseID = Purchase.PurchaseID INNER JOIN

                      Products ON PurchaseBody.ProductID = Products.ProductID

WHERE     (Purchase.IsPosted = 1)

GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.IsApplyPartyMarkup, Products.PurAcNo



UNION ALL



/****** PRODUCT PURCHASE SALESTAX VALUE DEBIT TO PRODUCT PURCHASE STAX ACCOUNT  *********/





SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseSTAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      SUM(PurchaseBody.TTLSalesTax) AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT PURCHASE EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpMinusAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      0 AS Debit, Purchase.ExpMinus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Purchase 



WHERE     Purchase.IsPosted = 1



UNION ALL



/******* PRODUCT PURCHASE EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpPlusAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      Purchase.ExpPlus AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Purchase 



WHERE     Purchase.IsPosted = 1



UNION ALL



/********** PRODUCT PURCHASE COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/







SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseComAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax) * IsNull(Purchase.ComRatio,0) / 100 AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.ComRatio, IsApplyPartyMarkup



UNION ALL



/*********** PRODUCT PURCHASE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, Purchase.VendorID AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      Purchase.PaidAmount AS Debit, 



              SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax)



                      - ISNULL(Purchase.ExpMinus, 0) 



                      + ISNULL(Purchase.ExpPlus, 0) 



              + (SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax) * IsNull(Purchase.ComRatio,0) / 100) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.ComRatio, Purchase.VendorID, Purchase.PaidAmount, Purchase.ExpMinus, Purchase.ExpPlus, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT PURCHASE PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/





SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 







                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      0 AS Debit, Purchase.PaidAmount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Purchase 



WHERE     Purchase.IsPosted = 1



/********************* END PRODUCT GENERAL PURCHASE *************************************/

UNION ALL

/******************** PRODUCT GENERAL PURCHASE RETURN *************************************/

/****** PRODUCT PURCHASERETURN GROSS VALUE CREDIT TO PRODUCT PURCHASERETURN ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, case when ISNULL(Products.PurAcNo, '') = '' then dbo.RegistryValue('ATPurchaseReturnAccount') else Products.PurAcNo end 

 AS AccountNo, 

                      ISNULL(PurReturns.Narration, ' ') AS Narration, 0 AS Debit, SUM(PurReturnsBody.TTLValue) AS Credit, ISNULL(PurReturns.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         PurReturnsBody INNER JOIN

                      PurReturns ON PurReturnsBody.PurReturnID = PurReturns.PurReturnID INNER JOIN

                      Products ON PurReturnsBody.ProductID = Products.ProductID

WHERE     (PurReturns.IsPosted = 1)

GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.IsApplyPartyMarkup, Products.PurAcNo



UNION ALL



/****** PRODUCT PURCHASERETURN STAX VALUE CREDIT TO PRODUCT PURCHASERETURNSTAX ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnSTAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(PurReturnsBody.TTLSalesTax) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT PURCHASERETURN EXPMINUS DEBIT TO PRODUCT EXPMINUS ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnExpMinusAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      PurReturns.ExpMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturns 



WHERE     PurReturns.IsPosted = 1



UNION ALL



/****** PRODUCT PURCHASERETURN EXPPLUS CREDIT TO PRODUCT EXPPLUS ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnExpPlusAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              PurReturns.ExpPlus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturns 



WHERE     PurReturns.IsPosted = 1



UNION ALL



/****** PRODUCT PURCHASERETURN COMMISSION VALUE CREDIT TO PRODUCT PURCHASERETURN COMMISSON ACCOUNT  *********/ 



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnComAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax) * IsNull(PurReturns.ComRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.ComRatio, IsApplyPartyMarkup



UNION ALL



/*********** PRODUCT PURCHASERETURN NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 

                       PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, PurReturns.VendorID AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



              SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax)



                      - ISNULL(PurReturns.ExpMinus, 0) 

                       + ISNULL(PurReturns.ExpPlus, 0) 



              + (SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax) * IsNull(PurReturns.ComRatio,0) / 100) AS Debit,







                      PurReturns.ReceivedAmount AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.ComRatio, PurReturns.ExpPlus, PurReturns.ExpMinus, PurReturns.VendorID, PurReturns.ReceivedAmount, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT PURCHASERETURN RECEIVED AMOUNT DEBIT TO CASH ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      PurReturns.ReceivedAmount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturns 







WHERE     PurReturns.IsPosted = 1



/********************* END PRODUCT GENERAL PURCHASE RETURN *************************************/

UNION ALL

/********************* PRODUCT GENERAL SALES *************************************/

/****** PRODUCT Sales GROSS VALUE CREDIT TO PRODUCT Sales ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, 



case when ISNULL(Products.SaleAcNo,'') = '' then dbo.RegistryValue('ATSalesAccount') else Products.SaleAcNo end AS AccountNo, ISNULL(Sales.Narration, ' ') AS Narration, 0 AS Debit, 

                      SUM(SalesBody.TTLValue) AS Credit, ISNULL(Sales.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         SalesBody INNER JOIN

                      Sales ON SalesBody.SaleID = Sales.SaleID INNER JOIN

                      Products ON SalesBody.ProductID = Products.ProductID

WHERE     (Sales.IsPosted = 1)

GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.IsApplyPartyMarkup, Products.SaleAcNo



UNION ALL



/****** PRODUCT Sales STAX VALUE CREDIT TO PRODUCT SalesSTAX ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                       AS ReconcileDate, dbo.RegistryValue('ATSalesSTAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(SalesBody.TTLSalesTax) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT Sales EXPMINUS DEBIT TO PRODUCT EXPMINUS ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpMinusAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.ExpMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Sales 



WHERE     Sales.IsPosted = 1



UNION ALL

 /****** PRODUCT Sales EXPPLUS CREDIT TO PRODUCT EXPPLUS ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpPlusAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 

                       0 AS Debit, 



              Sales.ExpPlus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Sales 



WHERE     Sales.IsPosted = 1



UNION ALL



/****** PRODUCT Sales COMMISSION VALUE CREDIT TO PRODUCT Sales COMMISSON ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesComAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT Sales SpecialDiscount DEBIT TO PRODUCT SALES SpecialDiscount ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, dbo.RegistryValue('ATSalesSpecialDiscountAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.SpecialDiscount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Sales 







WHERE     Sales.IsPosted = 1



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, Sales.CustomerID AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



case when isnull(Sales.CustomerIDCom,0)=0 then

              SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax)



                      - ISNULL(Sales.ExpMinus, 0) 



                      + ISNULL(Sales.ExpPlus, 0) 



              + (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100)



else

              (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax)



                      - ISNULL(Sales.ExpMinus, 0) 



                      + ISNULL(Sales.ExpPlus, 0) 



              + (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100)) * (100-Sales.CustomerComRatio) / 100 







end AS Debit



,Sales.ReceivedAmount + Sales.SpecialDiscount  AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.CustomerIDCom, Sales.CustomerComRatio, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, Sales.ExpPlus, Sales.ExpMinus, Sales.CustomerID, Sales.ReceivedAmount, Sales.SpecialDiscount, IsApplyPartyMarkup



UNION ALL



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, Sales.CustomerIDCom AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax)



                      - ISNULL(Sales.ExpMinus, 0) 



                      + ISNULL(Sales.ExpPlus, 0) 



              + (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100)) * Sales.CustomerComRatio / 100 



AS Debit

,Sales.ReceivedAmount AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup





FROM SalesBody INNER JOIN

    Sales ON 



    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1 and isnull(Sales.CustomerIDCom,0)<>0



GROUP BY Sales.SaleID, Sales.CustomerIDCom, Sales.CustomerComRatio, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, Sales.ExpPlus, Sales.ExpMinus, Sales.CustomerID, Sales.ReceivedAmount, Sales.SpecialDiscount, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT Sales RECEIVEDAMOUNT DEBIT TO CASH ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo,  

                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.ReceivedAmount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Sales 



WHERE     Sales.IsPosted = 1



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              SUM(SalesBody.Cost*((SalesBody.Qty+SalesBody.Bonus)*SalesBody.Multiplier)) AS Debit,



                      0 AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/ 

SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSoldStockValueAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              0 AS Debit,



                      SUM(SalesBody.Cost*((SalesBody.Qty+SalesBody.Bonus)*SalesBody.Multiplier)) AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup



/********************* END PRODUCT GENERAL SALES *************************************/

UNION ALL

/********************* PRODUCT GENERAL SALES RETURN *************************************/



/****** PRODUCT SaleReturns GROSS VALUE DEBIT TO PRODUCT SaleReturns ACCOUNT  *********/





SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 



case when ISNULL(Products.SaleAcNo,'') = '' then dbo.RegistryValue('ATSaleReturnsAccount') else Products.SaleAcNo end 

AS AccountNo, 

                      ISNULL(SaleReturns.Narration, ' ') AS Narration, SUM(SaleReturnsBody.TTLValue) AS Debit, 0 AS Credit, ISNULL(SaleReturns.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         SaleReturnsBody INNER JOIN

                      SaleReturns ON SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID INNER JOIN

                      Products ON SaleReturnsBody.ProductID = Products.ProductID

WHERE     (SaleReturns.IsPosted = 1)

GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.IsApplyPartyMarkup, 

                      Products.SaleAcNo

UNION ALL



/****** PRODUCT SaleReturns SALESTAX VALUE DEBIT TO PRODUCT SaleReturns STAX ACCOUNT  *********/







SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsSTAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SUM(SaleReturnsBody.TTLSalesTax) AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT SaleReturns EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsExpMinusAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      0 AS Debit, SaleReturns.ExpMinus AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/******* PRODUCT SaleReturns EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsExpPlusAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SaleReturns.ExpPlus AS Debit, 0 AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/********** PRODUCT SaleReturns COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsComAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax) * IsNull(SaleReturns.ComRatio,0) / 100 AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.ComRatio, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT SaleReturns PAIDAMOUNT CREDIT TO SALESRETURNS SPECIALDISOUNT ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsSpecialDiscountAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      0 AS Debit, SaleReturns.SpecialDiscount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, SaleReturns.CustomerID AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SaleReturns.PaidAmount+SaleReturns.SpecialDiscount AS Debit, 



              SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax)



                      - ISNULL(SaleReturns.ExpMinus, 0) 



                      + ISNULL(SaleReturns.ExpPlus, 0) 







              + (SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax) * IsNull(SaleReturns.ComRatio,0) / 100) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.ComRatio, SaleReturns.CustomerID, SaleReturns.PaidAmount, SaleReturns.ExpMinus, SaleReturns.ExpPlus, SaleReturns.SpecialDiscount, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT SaleReturns PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 















                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      0 AS Debit, SaleReturns.PaidAmount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT DEBIT TO COSTOFGOODSaleReturns ACCOUNT *************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleReturnAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



              0 AS Debit,



                      SUM(SaleReturnsBody.Cost*((SaleReturnsBody.Qty+SaleReturnsBody.Bonus)*SaleReturnsBody.Multiplier)) AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT DEBIT TO COSTOFGOODSaleReturns ACCOUNT *************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnStockValueAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



              SUM(SaleReturnsBody.Cost*((SaleReturnsBody.Qty+SaleReturnsBody.Bonus)*SaleReturnsBody.Multiplier)) AS Debit,



                      0 AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup





    /********************* END PRODUCT GENERAL SALES RETURN *************************************/

UNION ALL

/*************** STARTING BROKERAGE FORMS********************************************/



/*************** CONTRACT INVOICE PURCHASE ********************************************/

/*************** GROSS PURCHASE VALUE AS DEBIT TO PURCHASE ACCOUNT********************************************/



    SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 

case when ISNULL(Products.PurAcNo,'') = '' then dbo.RegistryValue('ATPurchaseAccount') else Products.PurAcNo end AS AccountNo, 

                      ISNULL(ContractInvoice.Narration, '') AS Narration, SUM(ContractInvoiceBody.TotalValue)+ContractInvoice.RefCommValue AS Debit, 0 AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.PurchaseID = ContractInvoice.PurchaseID INNER JOIN

                      Products ON ContractInvoiceBody.ProductID = Products.ProductID

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (dbo.RegistryValue('StopApplyGrossToPartyAccount') = 0)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice, ContractInvoice.Narration, ContractInvoice.Received, 

                      ContractInvoice.IsApplyPartyMarkup, Products.PurAcNo,ContractInvoice.RefCommValue



UNION ALL

/*************** REF COMMISION VALUE AS DEBIT TO PURCHASE ACCOUNT********************************************/

--

--    SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

--                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, 

--case when ISNULL(Products.PurAcNo,'') = '' then dbo.RegistryValue('ATPurchaseAccount') else Products.PurAcNo end AS AccountNo, 

--                      'REF. COMMESSION' AS Narration, ContractInvoice.RefCommValue AS Debit, 0 AS Credit, 

--                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

--FROM         ContractInvoiceBody INNER JOIN

--                      ContractInvoice ON ContractInvoiceBody.PurchaseID = ContractInvoice.PurchaseID INNER JOIN

--                      Products ON ContractInvoiceBody.ProductID = Products.ProductID

--WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (dbo.RegistryValue('StopApplyGrossToPartyAccount') = 0)

--GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice, ContractInvoice.Narration, ContractInvoice.Received, 

--                      ContractInvoice.IsApplyPartyMarkup, Products.PurAcNo,ContractInvoice.RefCommValue

--

--UNION ALL

/*************** BARDANA VALUE AS DEBIT TO PURCHASE BARDANA ACCOUNT ********************************************/

--    SELECT 'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, 

--    NULL AS DepositedDate, NULL AS ReconcileDate,  '11327' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

--        Sum(Isnull(ContractInvoiceBody.PQty,0) * ContractInvoiceBody.Bardana) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



--    FROM ContractInvoiceBody INNER JOIN  ContractInvoice ON ContractInvoiceBody.PurchaseID = ContractInvoice.PurchaseID

--

--    WHERE ContractInvoice.IsSaleInvoice=0 and isnull(ContractInvoice.isBardana,1) = 1 and dbo.RegistryValue('StopApplyGrossToPartyAccount')=0

--    

--    GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice,ContractInvoice.Narration, IsApplyPartyMarkup

--HAVING  Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana)  > 0 or Sum(Isnull(ContractInvoiceBody.PQty,0) * ContractInvoiceBody.Bardana) > 0

--



SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '11327' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                      CASE WHEN ISNULL(ContractInvoice.BagVendorID, '') = '' THEN SUM(Isnull(ContractInvoiceBody.PQty, 0) * ContractInvoiceBody.Bardana) 

                      + SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) ELSE 0 END AS Debit, 0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 

                      0) AS IsApplyPartyMarkup

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.isBardana, 1) = 1) AND (dbo.RegistryValue('StopApplyGrossToPartyAccount') = 0) AND 

                      (ISNULL(ContractInvoice.BagVendorID, '') = '')

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice, ContractInvoice.Narration, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.BagVendorID

HAVING      (SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) > 0) OR

                      (SUM(ISNULL(ContractInvoiceBody.PQty, 0) * ContractInvoiceBody.Bardana) > 0)



UNION ALL

/*************** EXPENCE PLUS AS DEBIT TO PURCHASE EXP PLUS ACCOUNT ********************************************/

 SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpPlusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      ContractInvoice.OtherExpencePlus AS Debit, 0 AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 0 and ContractInvoice.OtherExpencePlus > 0



UNION ALL

/*************** EXPENCE MINUS AS CREDIT TO PURCHASE EXP MINUS ACCOUNT ********************************************/



SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpMinusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      0 AS Debit, ContractInvoice.OtherExpenceMinus AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 0 and ContractInvoice.OtherExpenceMinus > 0





UNION ALL

/*********** CONTRACT PURCHASE INVOICE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



--SELECT 'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

--

--    AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, ContractInvoice.PartyID AS AccountNo, 

--    

--    IsNull(ContractInvoice.Narration,'') AS Narration, ContractInvoice.Received AS Debit, 

--

--       ISNULL((ContractInvoice.NetValue- (case when dbo.RegistryValue('StopApplyGrossToPartyAccount')=1 then SUM(ContractInvoiceBody.TotalValue)+Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) else 0 end)   )+ContractInvoice.Received, 0) Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

--

--    FROM ContractInvoiceBody INNER JOIN ContractInvoice ON ContractInvoiceBody.PurchaseID = ContractInvoice.PurchaseID

--

--    WHERE (ContractInvoice.IsSaleInvoice = 0) GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.PartyID, ContractInvoice.Received, 

--

--    ContractInvoice.Narration, ContractInvoice.NetValue, IsApplyPartyMarkup



SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, ContractInvoice.PartyID AS AccountNo, dbo.FunGetContractPurchaseInvoiceNarration(ContractInvoice.ContractPurId) 

                      AS Narration, ContractInvoice.Received  AS Debit, ISNULL(ContractInvoice.NetValue - (CASE WHEN dbo.RegistryValue('StopApplyGrossToPartyAccount') = 1 AND 

                      isnull(ContractInvoice.BagVendorID, '') = '' THEN SUM(ContractInvoiceBody.TotalValue) 

                      + SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) WHEN ISNULL(ContractInvoice.BagVendorID, '') 

                      <> '' THEN SUM(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) ELSE 0 END) + ContractInvoice.Received, 0) AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0)

GROUP BY ContractInvoice.PurchaseID, ContractInvoice.CDate, ContractInvoice.PartyID, ContractInvoice.Received, ContractInvoice.Narration, 

                      ContractInvoice.NetValue, ContractInvoice.IsApplyPartyMarkup, ContractInvoice.BagVendorID

,ContractInvoice.ContractPurId



UNION ALL

/******* CONTRACT PURCHASE ADVANCE AMOUNT CREDIT TO CASH ACCOUNT  ************/

SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.PurchaseID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      0 AS Debit, ContractInvoice.Received AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice=0



/********************* END CONTRACT INVOICE PURCHASE*************************************/

UNION ALL

/********************* CONTRACT INVOICE SALES *************************************/

/****** CONTRACT Sales GROSS VALUE CREDIT TO Sales ACCOUNT  *********/

SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate,

case when ISNULL(Products.SaleAcNo,'') = '' then dbo.RegistryValue('ATSalesAccount') else Products.SaleAcNo end AS AccountNo, 

                      ISNULL(ContractInvoice.Narration, '') AS Narration, 0 AS Debit, SUM(ContractInvoiceBody.TotalValue) AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoiceBody INNER JOIN

                      ContractInvoice ON ContractInvoiceBody.SaleID = ContractInvoice.SaleID INNER JOIN

                      Products ON ContractInvoiceBody.ProductID = Products.ProductID

WHERE     (ContractInvoice.IsSaleInvoice = 1) AND (dbo.RegistryValue('StopApplyGrossToPartyAccount') = 0)

GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice, ContractInvoice.Narration, ContractInvoice.Received, 

                      ContractInvoice.IsApplyPartyMarkup, Products.SaleAcNo

UNION ALL



/*************** BARDANA VALUE AS CREDIT TO SALE BARDANA ACCOUNT ********************************************/

    SELECT 'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, 

    NULL AS DepositedDate, NULL AS ReconcileDate,  '415' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

        0 AS Debit, Sum(Isnull(ContractInvoiceBody.PQty,0) * ContractInvoiceBody.Bardana) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

    FROM ContractInvoiceBody INNER JOIN  ContractInvoice ON ContractInvoiceBody.SaleID = ContractInvoice.SaleID



    WHERE ContractInvoice.IsSaleInvoice=1 and isnull(ContractInvoice.isBardana,1) = 1 and dbo.RegistryValue('StopApplyGrossToPartyAccount')=0

    

    GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice,ContractInvoice.Narration, IsApplyPartyMarkup



HAVING  Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana)  > 0 or Sum(Isnull(ContractInvoiceBody.PQty,0) * ContractInvoiceBody.Bardana) > 0



UNION ALL

/**************** CONTRACT SALE INVOICE EXP PLUS AS CREDIT TO SALES EXP PLUS  ACCOUNT  *********/

SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpPlusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                       0 AS Debit, 



              Isnull(ContractInvoice.OtherExpencePlus,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1





UNION ALL

/**************** CONTRACT SALE INVOICE TELEPHONE,INSURANCE,FUEL,STITCHING EXP Credit To  ACCOUNTs  *********/



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58011' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                       0 AS Debit, 



              Isnull(ContractInvoice.AddTelePhoneExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58012' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 



              Isnull(ContractInvoice.AddInsuranceExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58013' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 



              Isnull(ContractInvoice.AddFuelExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58014' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 







              Isnull(ContractInvoice.AddStitchingExp,0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 











/*************************************************************************************************************/





UNION ALL

/**************** CONTRACT SALE INVOICE EXP MINUS AS DEBIT TO SALES EXP MINUS  ACCOUNT  *********/





SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpMinusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      ContractInvoice.OtherExpenceMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 





UNION ALL



/*********** CONTRACT SALE Invoice NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/





SELECT 'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



    AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, isnull(ContractInvoice.SellerID,ContractInvoice.PartyID) AS AccountNo, 

    

    IsNull(ContractInvoice.Narration,'') AS Narration,     

ISNULL(ContractInvoice.NetValue -(case when dbo.RegistryValue('StopApplyGrossToPartyAccount')=1 then 

SUM(ContractInvoiceBody.TotalValue) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) else 0 end), 0) + ContractInvoice.Received AS Debit, ContractInvoice.Received As Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



    FROM ContractInvoiceBody INNER JOIN ContractInvoice ON ContractInvoiceBody.SaleID = ContractInvoice.SaleID



    WHERE (ContractInvoice.IsSaleInvoice = 1)  GROUP BY ContractInvoice.SaleID, ContractInvoice.CDate, ContractInvoice.SellerID, ContractInvoice.Received, 



    ContractInvoice.Narration, ContractInvoice.NetValue, IsApplyPartyMarkup,ContractInvoice.PartyID



UNION ALL

/********************* CONTRACT Sales Invoice RECEIVEDAMOUNT as DEBIT TO CASH ACCOUNT  *********/



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.SaleID AS VoucherNo, 





                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo,IsNull(ContractInvoice.Narration ,'') AS Narration, 



                      ContractInvoice.Received AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 



/********************* END CONTRACT INVOICE SALES**********************/
GO

-- View: VuAccVouchersOthersPL
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersPL]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersPL];
GO
CREATE VIEW VuAccVouchersOthersPL



AS



/*************** OPENING Balances ********************************************/

SELECT     '' AS VoucherType, null AS VoucherNo, null AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL 

                      AS DepositedDate, NULL AS ReconcileDate, AccountNo, '' AS Narration, OpeningDebit AS DEBIT, 

                      OpeningCredit AS CREDIT, 0 AS IsApplyPartyMarkup

FROM         ChartofAccounts

UNION ALL

/*************** BROKERAGE Shortage Adjustment********************************************/



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, VendorID AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, ROUND(PurchaseRate * (ShortageInKgs/37.324), 4) AS Debit, 0 AS Credit, 

                      0 AS IsApplyPartyMarkup

FROM         BrokrageShortage



UNION ALL



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, CustomerID AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, 0 AS Debit, ROUND(SaleRate *((ShortageInKgs-ISNULL(CommissionWeight,0))/37.324), 4) AS Credit, 

                      0 AS IsApplyPartyMarkup

FROM         BrokrageShortage 



UNION ALL



SELECT     'BSH' AS VoucherType, InvoiceID AS VoucherNo, InvoiceDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, dbo.RegistryValue('BrokrageShortageRevenue') AS AccountNo, 'Brokrage Shortage Invoice: ' + Narration AS Narration, 0 AS Debit, ROUND(SaleRate * (CommissionWeight/37.324), 4) AS Credit, 

                      0 AS IsApplyPartyMarkup

FROM         BrokrageShortage

Where ISNULL(CommissionWeight,0) > 0



/*****************************************************************************************/



UNION ALL





/*************** BROKERAGE PURCHASE EXPENSES********************************************/



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '46' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.CommissionValue)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)



UNION ALL



/***Purchase Discount income***/

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '5124' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.Discount)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0) 

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.Discount) > 0)



UNION ALL

/***********/



/* ------Pur Arhat Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '47' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusArhat,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, IsApplyPartyMarkup











HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '47' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddArhat,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurArhat / 100 > 0)



UNION ALL

/*--------------------*/



/* ------Pur-With Holding Tax Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '2301' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      (ContractInvoice.CommissionValue * Isnull(ContractInvoice.WHTax, 0) / 100) AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

Where (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddWHTax,0) = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.WHTax

HAVING      Isnull(ContractInvoice.WHTax, 0) > 0



UNION ALL







SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '2301' AS AccountNo, ContractInvoice.Narration, (ContractInvoice.CommissionValue * Isnull(ContractInvoice.WHTax, 0) / 100) AS Debit, 

                      0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

Where (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddWHTax,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurArhat, ContractInvoice.IsApplyPartyMarkup, 

                      ContractInvoice.CommissionValue, ContractInvoice.WHTax

HAVING      Isnull(ContractInvoice.WHTax, 0) > 0



UNION ALL



/*--------------------*/

/* ------Pur Changrai Acc--- */





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, dbo.RegistryValue('PurChangrai') AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusChangrai,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, dbo.RegistryValue('PurChangrai') AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddChangrai,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurChangrai, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurChangrai / 100 > 0)



UNION ALL

/*--------------------*/





/* ------Pur Mazdoori Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57002' AS AccountNo, ContractInvoice.Narration, 

    0 AS Debit, 

CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurMazdori / 100  

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+SUM(ContractInvoiceBody.PQty)) * ContractInvoice.PurMazdori Else 

    (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurMazdori  End 

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN



                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId



WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusMazdori,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent



HAVING      (ContractInvoice.PurMazdori > 0)







UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57002' AS AccountNo, ContractInvoice.Narration, 



    CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurMazdori / 100  

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+ SUM(ContractInvoiceBody.PQty)) * ContractInvoice.PurMazdori Else 

    (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurMazdori  End

 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddMazdori,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent

HAVING      (ContractInvoice.PurMazdori > 0)

/* --------- */



UNION ALL



/* ------Pur Dalali Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57003' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusDalali,0) = 1) 



 GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup







HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 > 0) 



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57003' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddDalali,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurDalali, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurDalali / 100 > 0) 



UNION ALL

/*---------------*/



/* ------Pur Bank Charges Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57016' AS AccountNo, Narration, 0 AS Debit, BankChargesAmount AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup



FROM         ContractInvoice



WHERE     (IsSaleInvoice = 0) AND (ISNULL(MinusBankCharges, 0) = 1) AND (ISNULL(BankChargesAmount, 0) > 0)



GROUP BY ContractPurId, CDate, Narration, PurPulliGhya, IsApplyPartyMarkup, BankChargesAmount



UNION ALL







SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '57016' AS AccountNo, Narration, BankChargesAmount AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 



                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(AddBankCharges, 0) = 1) AND (ISNULL(BankChargesAmount, 0) > 0)



/*---------------*/

UNION ALL

/* ------ Pur Ref Commission Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(RefCommValue, 0) > 0)





UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) AND (ISNULL(RefCommValue, 0) > 0)





--UNION ALL



/* SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '57017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddCommission,0) = 1) AND (ISNULL(RefCommValue, 0) > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddCommission,0) = 1) AND (ISNULL(RefCommValue, 0) > 0) */





/*---------------*/

UNION ALL



/* ------Pur Carriage Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57004' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, (SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusPuliGhya,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurPulliGhya, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      ((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57004' AS AccountNo, ContractInvoice.Narration, 

                      (SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddPuliGhya,0) = 1) GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurPulliGhya, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      ((SUM(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurPulliGhya > 0)





UNION ALL

/*--------------*/



/* ------Pur Commission Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4205' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0)  And (Isnull(ContractInvoice.MinusCommission,0) = 1) GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 > 0)



UNION ALL







SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4205' AS AccountNo, ContractInvoice.Narration,                        SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddCommission,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.PurComm / 100 > 0)



UNION ALL

/*-------------*/



/* ------Pur Manshyana Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57006' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusManshyana,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57006' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddManshyana,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



/*-----------------*/



/* ------Pur Loading Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57007' AS AccountNo, ContractInvoice.Narration, 

          0 AS Debit, case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.PurLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(ContractInvoiceBody.PQty) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurLoading > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57007' AS AccountNo, ContractInvoice.Narration, 

          case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.PurLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(ContractInvoiceBody.PQty) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurLoading end  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoiceBody.MoundStd, ContractInvoice.Narration, ContractInvoice.PurLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurLoading > 0)



UNION ALL

/*--------------*/



/* ------Pur Un-Loading Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57008' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.PurUnLoading * (Sum(ContractInvoiceBody.BagsQty)+ Sum(ContractInvoiceBody.PQty) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading  end   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusUnLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurUnLoading, ContractInvoice.UnLoadingPerBag, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurUnLoading > 0)

 

UNION ALL 



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57008' AS AccountNo, ContractInvoice.Narration, 



                      case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.PurUnLoading * (Sum(ContractInvoiceBody.BagsQty) + Sum(ContractInvoiceBody.PQty) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.PurUnLoading  end  AS Debit,  0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddUnLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.PurUnLoading, ContractInvoice.UnLoadingPerBag, ContractInvoiceBody.MoundStd, IsApplyPartyMarkup

HAVING      (ContractInvoice.PurUnLoading > 0)

 

UNION ALL 

/*--------------*/



/* ------Pur Vehicle Charges Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ContractInvoice.VehicalCharges AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL















SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57009' AS AccountNo, ContractInvoice.Narration, 

                      ContractInvoice.VehicalCharges AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddVehicleCharges,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL



/* ------Pur Market Fees Acc--- */

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57010' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.MinusMarketFees,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '57010' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId



WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddMarketFees,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL



/*-----------------*/



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4102' AS AccountNo, ContractInvoice.Narration, 

                      (CASE WHEN IsPay=0 THEN SUM(ContractInvoice.CommissionValue) ELSE 0 END) AS Debit, (CASE WHEN IsPay=1 THEN SUM(ContractInvoice.CommissionValue) ELSE 0 END)   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0)

GROUP BY ContractInvoice.ContractPurId, IsPay, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)





UNION ALL /*************** BROKERAGE SALE EXPENSES********************************************/



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4103' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.CommissionValue)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)



UNION ALL



/*****Discount On Sale exp******/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '48' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.Discount) AS Debit, 0  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 0)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.Discount) > 0)



UNION ALL

/****************/







SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4103' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.CommissionValue) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And (ContractInvoice.IsPay = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoice.CommissionValue) > 0)





UNION ALL

/* ------Sale Bank Charges Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58016' AS AccountNo, Narration, 0 AS Debit, BankChargesAmount AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(BankChargesAmount, 0) > 0) AND (ISNULL(AddBankCharges, 0) = 1)



UNION ALL





SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58016' AS AccountNo, Narration, BankChargesAmount AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(BankChargesAmount, 0) > 0) AND (ISNULL(MinusBankCharges, 0) = 1)

/*--------------------*/

UNION ALL



/* ------Sale Ref Commission Acc--- */



SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1)  AND (ISNULL(RefCommValue, 0) > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(RefCommValue, 0) > 0)





--UNION ALL



/*SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, '58017' AS AccountNo, Narration, RefCommValue AS Debit, 0 AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(MinusCommission, 0) = 1) AND (ISNULL(RefCommValue, 0) > 0)







UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractPurId AS VoucherNo, CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                      AS ReconcileDate, BrokerID AS AccountNo, Narration, 0 AS Debit, RefCommValue AS Credit, ISNULL(IsApplyPartyMarkup, 0) 

                      AS IsApplyPartyMarkup

FROM         ContractInvoice

WHERE     (IsSaleInvoice = 1) AND (ISNULL(MinusCommission, 0) = 1) AND (ISNULL(RefCommValue, 0) > 0) */



/*--------------------*/

UNION ALL

/* ------Sale Market Fees Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58001' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddMarketFees,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58001' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusMarketFees,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleMarketFees, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.SaleMarketFees > 0)



UNION ALL



/*--------------------*/







/* ------Sale Mazdoori Acc--- */



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58002' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit,



CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleMazdori / 100

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+SUM(ContractInvoiceBody.PQty)) * ContractInvoice.SaleMazdori Else 

        (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori End  

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddMazdori,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent

HAVING      ((Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58002' AS AccountNo, ContractInvoice.Narration, 



CASE WHEN ISNULL(ContractInvoice.MazdoriPercent,0)=1 THEN SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleMazdori / 100

WHEN ContractInvoice.MazdoriPerbag = 1 then (SUM(ContractInvoiceBody.BagsQty)+ SUM(ContractInvoiceBody.PQty)) * ContractInvoice.SaleMazdori Else 

        (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori End  

AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusMazdori,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleMazdori, ContractInvoice.MazdoriPerbag, IsApplyPartyMarkup,ContractInvoice.MazdoriPercent

HAVING      ((Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori > 0)



UNION ALL

/*-------------------*/



/* ------Sale Dalali Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58003' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN





                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddDalali,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)



UNION ALL





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58003' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId



WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusDalali,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleDalali, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleDalali / 100 > 0)



UNION ALL

/*---------------------*/



/* ------Sale Sootli Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58004' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, (SUM(ContractInvoiceBody.BagsQty)+SUM(ContractInvoiceBody.PQty)) * ContractInvoice.SaleSootli  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddSootli,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli, IsApplyPartyMarkup

HAVING      ((SUM(ContractInvoiceBody.BagsQty)+SUM(ContractInvoiceBody.PQty)) * ContractInvoice.SaleSootli > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58004' AS AccountNo, ContractInvoice.Narration, 

                      (SUM(ContractInvoiceBody.BagsQty)+ SUM(ContractInvoiceBody.PQty)) * ContractInvoice.SaleSootli AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusSootli,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleSootli, IsApplyPartyMarkup

HAVING      ((SUM(ContractInvoiceBody.BagsQty)+ SUM(ContractInvoiceBody.PQty)) * ContractInvoice.SaleSootli > 0)



UNION ALL

/*------------------*/



/* ------Sale Commission Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4206' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddCommission,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 > 0)





UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '4206' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusCommission,0) = 1) GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleComm, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.SaleComm / 100 > 0)



UNION ALL

/*---------------------*/



/* ------Sale Manshyana Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58006' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddManshyana,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58006' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusManshyana,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TruckQty) * ContractInvoice.SaleManshyana > 0)



UNION ALL



/*----------------------*/



/* ------Sale Loading Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58007' AS AccountNo, ContractInvoice.Narration, 

        0 AS Debit, case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.SaleLoading* (Sum(ContractInvoiceBody.BagsQty)+ Sum(ContractInvoiceBody.PQty) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading end  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleLoading > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58007' AS AccountNo, ContractInvoice.Narration, 

        case when ContractInvoice.LoadingPerBag=1 then ContractInvoice.SaleLoading* (Sum(ContractInvoiceBody.BagsQty) + Sum(ContractInvoiceBody.PQty) )



        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleLoading end AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                       ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleLoading,ContractInvoice.LoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleLoading > 0)



UNION ALL

/*---------------------*/



/* ------Sale Un-Loading Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58008' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.SaleUnLoading * (Sum(ContractInvoiceBody.BagsQty)+ Sum(ContractInvoiceBody.PQty) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading end   AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddUnLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleUnLoading, ContractInvoice.UnLoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleUnLoading > 0)

 

UNION ALL 





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58008' AS AccountNo, ContractInvoice.Narration, 

                      case when ContractInvoice.UnLoadingPerBag=1 then ContractInvoice.SaleUnLoading * (Sum(ContractInvoiceBody.BagsQty) + Sum(ContractInvoiceBody.PQty) )

        else (Sum(ContractInvoiceBody.TotalKgs)/ContractInvoiceBody.MoundStd) * ContractInvoice.SaleUnLoading end AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusUnLoading,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoiceBody.MoundStd, ContractInvoice.SaleUnLoading, ContractInvoice.UnLoadingPerBag, IsApplyPartyMarkup

HAVING      (ContractInvoice.SaleUnLoading > 0)

 

UNION ALL 

/*--------------------*/



/* ------Sale WH-Tax  Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit,

Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax

Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTax,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg



HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL







SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration,



 

 Case When ISNULL(ContractInvoice.WHTinKg,0)=1 then SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTax



Else SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100   END





AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTax,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTax, IsApplyPartyMarkup,ContractInvoice.WHTinKg

HAVING      (SUM(ContractInvoiceBody.TotalValue) * ContractInvoice.WHTax / 100 > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, 

SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg  

AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddWHTKg,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58009' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTKg AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusWHTKg,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.WHTKg, IsApplyPartyMarkup

HAVING      (SUM(ContractInvoiceBody.TotalKgs) / 100 * ContractInvoice.WHTkg > 0)





UNION ALL

/*-----------------*/



/* ------Sale VehicalCharges  Acc--- */

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58010' AS AccountNo, ContractInvoice.Narration, 

                      ContractInvoice.VehicalCharges AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.MinusVehicleCharges,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup

HAVING      (ContractInvoice.VehicalCharges > 0)

 

UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58010' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ContractInvoice.VehicalCharges AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId



WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddVehicleCharges,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup



HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL

/*------ Telephone, Insurance, Fule and Stitching Acc-------------------*/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58011' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ContractInvoice.AddTelePhoneExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  And (Isnull(ContractInvoice.AddTelePhoneExp,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.AddTelePhoneExp, 

                      ContractInvoice.IsApplyPartyMarkup

HAVING      (ContractInvoice.AddTelePhoneExp > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58012' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                       ContractInvoice.AddInsuranceExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddInsuranceExp,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,  ContractInvoice.AddInsuranceExp, 

                      ContractInvoice.IsApplyPartyMarkup

HAVING      ( ContractInvoice.AddInsuranceExp > 0)





UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58013' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddFuelExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddFuelExp,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddFuelExp, 

                      ContractInvoice.IsApplyPartyMarkup

HAVING      ( ContractInvoice.AddFuelExp > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '58014' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddStitchingExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And (Isnull(ContractInvoice.AddStitchingExp,0) = 1)

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddStitchingExp, 



                      ContractInvoice.IsApplyPartyMarkup



HAVING      ( ContractInvoice.AddStitchingExp > 0)









/************************************************************************/













UNION ALL 





/*************** CROPS PURCHASE ********************************************/



/****** CROP PURCHASE GROSS VALUE DEBIT TO CROP PURCHASE ACCOUNT  *********/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropPurchaseAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



                      SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)



                      AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsPurchaseBody INNER JOIN

    CropsPurchaseHeader ON 

    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

     INNER JOIN

    CropsWeighments ON 

    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID



WHERE     CropsPurchaseHeader.IsPosted = 1



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PurchaseDate, CropsPurchaseHeader.IsPosted, CropsPurchaseHeader.Remarks, IsApplyPartyMarkup



UNION ALL



/******* CROP PURCHASE EXPMINUS CREDIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropPurchaseExpMinusAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ')  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.ExpMinus



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1



UNION ALL



/******* CROP PURCHASE EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropPurchaseExpPlusAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 



                      CropsPurchaseHeader.ExpPlus



                       AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1



UNION ALL



/******* CROP PURCHASE PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ')  AS Narration, 0 AS Debit, 



                      CropsPurchaseHeader.PaidAmount



                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsPurchaseHeader 



WHERE     CropsPurchaseHeader.IsPosted = 1





UNION ALL



/********** CROP PURCHASE COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, dbo.RegistryValue('ATCropPurchaseComAccount') AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 







                      (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



                      * IsNull(CropsPurchaseHeader.ComRatio,0) / 100 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsPurchaseBody INNER JOIN



    CropsPurchaseHeader ON 

    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

     INNER JOIN

    CropsWeighments ON 



    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID

 WHERE     CropsPurchaseHeader.IsPosted = 1



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted,



                      CropsPurchaseHeader.ComRatio, CropsPurchaseHeader.Remarks, IsApplyPartyMarkup



UNION ALL



/*********** CROP PURCHASE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'CP' AS VOUCHERTYPE, CropsPurchaseHeader.CropPurchaseID AS VoucherNo, 



                      CropsPurchaseHeader.PURCHASEDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, CropsPurchaseHeader.VendorID AS AccountNo, IsNull(CropsPurchaseHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price)) 



                      - ISNULL(CropsPurchaseHeader.ExpMinus, 0) 



                      + ISNULL(CropsPurchaseHeader.ExpPlus, 0) 



                    - ISNULL(CropsPurchaseHeader.PaidAmount, 0) 

            



                      + (SUM((CropsPurchaseBody.Weight / CropsWeighments.Multiplier) * CropsPurchaseBody.Price) 



                      * IsNull(CropsPurchaseHeader.ComRatio,0) / 100) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsPurchaseBody INNER JOIN

    CropsPurchaseHeader ON 

    CropsPurchaseBody.CropPurchaseID = CropsPurchaseHeader.CropPurchaseID

     INNER JOIN

    CropsWeighments ON 

    CropsPurchaseBody.WeighmentID = CropsWeighments.WeighmentID



WHERE     CropsPurchaseHeader.IsPosted = 1



GROUP BY CropsPurchaseHeader.CropPurchaseID, CropsPurchaseHeader.PURCHASEDATE, CropsPurchaseHeader.IsPosted, IsApplyPartyMarkup



              , CropsPurchaseHeader.VendorID, CropsPurchaseHeader.ComRatio 



              , CropsPurchaseHeader.ExpMinus, CropsPurchaseHeader.ExpPlus, CropsPurchaseHeader.Remarks,CropsPurchaseHeader.PaidAmount



/********************* END CROP PURCHASE *************************************/

UNION ALL

/*************** CROPS SALES ********************************************/  /****** CROP SALES GROSS VALUE CREDIT TO CROP SALE ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)



                      AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDate, CropsSaleHeader.IsPosted, CropsSaleHeader.Remarks, IsApplyPartyMarkup



UNION ALL



/******* CROP SALE EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleExpMinusAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ')  AS Narration, 



                      CropsSaleHeader.ExpMinus AS Debit, 



























                       0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1





UNION ALL



/******* CROP SALE EXPPLUS CREDIT TO EXPENSES PLUS ACCOUNT  ************/







SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleExpPlusAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      CropsSaleHeader.ExpPlus









                       AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/******* CROP SALE RECEIVEDAMOUNT DEBIT TO CASH ACCOUNT  ************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ')  AS Narration, 



                      CropsSaleHeader.PaidAmount AS Debit, 

                       0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM         CropsSaleHeader 



WHERE     CropsSaleHeader.IsPosted = 1



UNION ALL



/********** CROP SALE STAX CREDIT TO STAX ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleSTaxAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 0 AS Debit, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.STRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.STRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup



UNION ALL



/********** CROP SALE WHTAX CREDIT TO WHTAX ACCOUNT  *********/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCropSaleWHTaxAccount') AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 



                      (SUM(((CropsSaleBody.Weight-CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 



                      * IsNull(CropsSaleHeader.WHTRatio,0) / 100 AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID



WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted,



                      CropsSaleHeader.WHTRatio, CropsSaleHeader.Remarks, IsApplyPartyMarkup



UNION ALL



/*********** CROP Sale NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/



SELECT     'CS' AS VOUCHERTYPE, CropsSaleHeader.CropSaleID AS VoucherNo, 



                      CropsSaleHeader.SaleDATE AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 







                      AS ReconcileDate, CropsSaleHeader.CustomerID AS AccountNo, IsNull(CropsSaleHeader.Remarks,' ') AS Narration, 



                      (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price)) 





                      - ISNULL(CropsSaleHeader.ExpMinus, 0) 





                      + ISNULL(CropsSaleHeader.ExpPlus, 0) 



            - ISNULL(CropsSaleHeader.PaidAmount, 0) 



                      + (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 







                      * IsNull(CropsSaleHeader.STRatio,0) / 100)





                      - (SUM(((CropsSaleBody.Weight - CropsSaleBody.Deduction) / CropsSaleBody.Multiplier) * CropsSaleBody.Price) 



                      * IsNull(CropsSaleHeader.WHTRatio,0) / 100) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM CropsSaleBody INNER JOIN

    CropsSaleHeader ON 

    CropsSaleBody.CropSaleID = CropsSaleHeader.CropSaleID









WHERE     CropsSaleHeader.IsPosted = 1



GROUP BY CropsSaleHeader.CropSaleID, CropsSaleHeader.SaleDATE, CropsSaleHeader.IsPosted, IsApplyPartyMarkup



              , CropsSaleHeader.CustomerID, CropsSaleHeader.STRatio, CropsSaleHeader.WHTRatio 



              , CropsSaleHeader.ExpMinus, CropsSaleHeader.ExpPlus, CropsSaleHeader.Remarks,CropsSaleHeader.PaidAmount



/********************* END CROP Sale *************************************/

UNION ALL

/*************** PRODUCT GENERAL OPENING ********************************************/



/****** PRODUCT OPENING GROSS VALUE DEBIT TO PRODUCT OPENING ACCOUNT  *********/



SELECT     'OP' AS VOUCHERTYPE, 1 AS VoucherNo, 



                      dbo.DefaultMinDate() AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                       AS ReconcileDate, dbo.RegistryValue('ATOpeningAccount') AS AccountNo, 'Opening Value' AS Narration, 



                      SUM(TTLValue) AS Debit, 



              0 AS Credit, 1 as IsApplyPartyMarkup



FROM OpeningStock



UNION ALL











/****** PRODUCT PURCHASE SALESTAX VALUE DEBIT TO PRODUCT PURCHASE STAX ACCOUNT  *********/



SELECT     'OP' AS VOUCHERTYPE, 1 AS VoucherNo, 



                      dbo.DefaultMinDate() AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATOpeningSTaxAccount') AS AccountNo, 'Opening STax' AS Narration, 



                      SUM(TTLSalesTax) AS Debit, 



              0 AS Credit, 1 as IsApplyPartyMarkup



FROM OpeningStock



UNION ALL 

/*************** PRODUCT GENERAL PURCHASE ********************************************/



/****** PRODUCT PURCHASE GROSS VALUE DEBIT TO PRODUCT PURCHASE ACCOUNT  *********/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      SUM(PurchaseBody.TTLValue) AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT PURCHASE SALESTAX VALUE DEBIT TO PRODUCT PURCHASE STAX ACCOUNT  *********/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseSTAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 







                      SUM(PurchaseBody.TTLSalesTax) AS Debit, 





              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1





GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, IsApplyPartyMarkup





UNION ALL



/******* PRODUCT PURCHASE EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/







SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 





                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpMinusAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      0 AS Debit, Purchase.ExpMinus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Purchase 



WHERE     Purchase.IsPosted = 1



UNION ALL



/******* PRODUCT PURCHASE EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpPlusAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      Purchase.ExpPlus AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Purchase 



WHERE     Purchase.IsPosted = 1



UNION ALL



/********** PRODUCT PURCHASE COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/







SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseComAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax) * IsNull(Purchase.ComRatio,0) / 100 AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.ComRatio, IsApplyPartyMarkup



UNION ALL



/*********** PRODUCT PURCHASE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, Purchase.VendorID AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 



                      Purchase.PaidAmount AS Debit, 



              SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax)



                      - ISNULL(Purchase.ExpMinus, 0) 



                      + ISNULL(Purchase.ExpPlus, 0) 



              + (SUM(PurchaseBody.TTLValue+PurchaseBody.TTLSalesTax) * IsNull(Purchase.ComRatio,0) / 100) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurchaseBody INNER JOIN

    Purchase ON 

    PurchaseBody.PurchaseID = Purchase.PurchaseID



WHERE     Purchase.IsPosted = 1



GROUP BY Purchase.PurchaseID, Purchase.PurchaseDate, Purchase.IsPosted, Purchase.Narration, Purchase.ComRatio, Purchase.VendorID, Purchase.PaidAmount, Purchase.ExpMinus, Purchase.ExpPlus, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT PURCHASE PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'PI' AS VOUCHERTYPE, Purchase.PurchaseID AS VoucherNo, 



                      Purchase.PurchaseDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(Purchase.Narration,' ') AS Narration, 







                      0 AS Debit, Purchase.PaidAmount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Purchase 



WHERE     Purchase.IsPosted = 1



/********************* END PRODUCT GENERAL PURCHASE *************************************/

UNION ALL



/******************** PRODUCT GENERAL PURCHASE RETURN *************************************/



/****** PRODUCT PURCHASERETURN GROSS VALUE CREDIT TO PRODUCT PURCHASERETURN ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo,  

                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(PurReturnsBody.TTLValue) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturnsBody INNER JOIN

    PurReturns ON 



    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT PURCHASERETURN STAX VALUE CREDIT TO PRODUCT PURCHASERETURNSTAX ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnSTAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(PurReturnsBody.TTLSalesTax) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT PURCHASERETURN EXPMINUS DEBIT TO PRODUCT EXPMINUS ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnExpMinusAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      PurReturns.ExpMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturns 



WHERE     PurReturns.IsPosted = 1



UNION ALL



/****** PRODUCT PURCHASERETURN EXPPLUS CREDIT TO PRODUCT EXPPLUS ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnExpPlusAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              PurReturns.ExpPlus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturns 



WHERE     PurReturns.IsPosted = 1



UNION ALL



/****** PRODUCT PURCHASERETURN COMMISSION VALUE CREDIT TO PRODUCT PURCHASERETURN COMMISSON ACCOUNT  *********/ 

SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseReturnComAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax) * IsNull(PurReturns.ComRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID





WHERE     PurReturns.IsPosted = 1







GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.ComRatio, IsApplyPartyMarkup



UNION ALL





/*********** PRODUCT PURCHASERETURN NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                       PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, PurReturns.VendorID AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



              SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax)







                      - ISNULL(PurReturns.ExpMinus, 0) 

                       + ISNULL(PurReturns.ExpPlus, 0) 



              + (SUM(PurReturnsBody.TTLValue+PurReturnsBody.TTLSalesTax) * IsNull(PurReturns.ComRatio,0) / 100) AS Debit,







                      PurReturns.ReceivedAmount AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturnsBody INNER JOIN

    PurReturns ON 

    PurReturnsBody.PurReturnID = PurReturns.PurReturnID



WHERE     PurReturns.IsPosted = 1



GROUP BY PurReturns.PurReturnID, PurReturns.PurReturnDate, PurReturns.IsPosted, PurReturns.Narration, PurReturns.ComRatio, PurReturns.ExpPlus, PurReturns.ExpMinus, PurReturns.VendorID, PurReturns.ReceivedAmount, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT PURCHASERETURN RECEIVED AMOUNT DEBIT TO CASH ACCOUNT  *********/



SELECT     'PR' AS VOUCHERTYPE, PurReturns.PurReturnID AS VoucherNo, 



                      PurReturns.PurReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(PurReturns.Narration,' ') AS Narration, 



                      PurReturns.ReceivedAmount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM PurReturns 







WHERE     PurReturns.IsPosted = 1



/********************* END PRODUCT GENERAL PURCHASE RETURN *************************************/

UNION ALL

/********************* PRODUCT GENERAL SALES *************************************/

/****** PRODUCT Sales GROSS VALUE CREDIT TO PRODUCT Sales ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 















                      AS ReconcileDate, dbo.RegistryValue('ATSalesAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(SalesBody.TTLValue) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT Sales STAX VALUE CREDIT TO PRODUCT SalesSTAX ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 

                       AS ReconcileDate, dbo.RegistryValue('ATSalesSTAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(SalesBody.TTLSalesTax) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT Sales EXPMINUS DEBIT TO PRODUCT EXPMINUS ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 





                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpMinusAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.ExpMinus AS Debit, 





              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Sales 





WHERE     Sales.IsPosted = 1



UNION ALL

 /****** PRODUCT Sales EXPPLUS CREDIT TO PRODUCT EXPPLUS ACCOUNT  *********/





SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpPlusAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 

                       0 AS Debit, 



              Sales.ExpPlus AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Sales 



WHERE     Sales.IsPosted = 1



UNION ALL



/****** PRODUCT Sales COMMISSION VALUE CREDIT TO PRODUCT Sales COMMISSON ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesComAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      0 AS Debit, 



              SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT Sales SpecialDiscount DEBIT TO PRODUCT SALES SpecialDiscount ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesSpecialDiscountAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.SpecialDiscount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM Sales 







WHERE     Sales.IsPosted = 1



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO VENDOR ACCOUNT *************/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, Sales.CustomerID AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax)



                      - ISNULL(Sales.ExpMinus, 0) 



                      + ISNULL(Sales.ExpPlus, 0) 



              + (SUM(SalesBody.TTLValue+SalesBody.TTLSalesTax) * IsNull(Sales.ComRatio,0) / 100) AS Debit,



                      Sales.ReceivedAmount + Sales.SpecialDiscount AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, Sales.ComRatio, Sales.ExpPlus, Sales.ExpMinus, Sales.CustomerID, Sales.ReceivedAmount, Sales.SpecialDiscount, IsApplyPartyMarkup

 

UNION ALL



/****** PRODUCT Sales RECEIVEDAMOUNT DEBIT TO CASH ACCOUNT  *********/



SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo,  

                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



                      Sales.ReceivedAmount AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup





FROM Sales 



WHERE     Sales.IsPosted = 1





UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/





SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              SUM(SalesBody.Cost*((SalesBody.Qty+SalesBody.Bonus)*SalesBody.Multiplier)) AS Debit,



                      0 AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN



    Sales ON 

    SalesBody.SaleID = Sales.SaleID





WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup



UNION ALL



/*********** PRODUCT Sales NET AMOUNT DEBIT TO COSTOFGOODSALES ACCOUNT *************/ 

SELECT     'SI' AS VOUCHERTYPE, Sales.SaleID AS VoucherNo, 



                      Sales.SaleDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSoldStockValueAccount') AS AccountNo, IsNull(Sales.Narration,' ') AS Narration, 



              0 AS Debit,



                      SUM(SalesBody.Cost*((SalesBody.Qty+SalesBody.Bonus)*SalesBody.Multiplier)) AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SalesBody INNER JOIN

    Sales ON 

    SalesBody.SaleID = Sales.SaleID



WHERE     Sales.IsPosted = 1



GROUP BY Sales.SaleID, Sales.SaleDate, Sales.IsPosted, Sales.Narration, IsApplyPartyMarkup



/********************* END PRODUCT GENERAL SALES *************************************/

UNION ALL

/********************* PRODUCT GENERAL SALES RETURN *************************************/



/****** PRODUCT SaleReturns GROSS VALUE DEBIT TO PRODUCT SaleReturns ACCOUNT  *********/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SUM(SaleReturnsBody.TTLValue) AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup



UNION ALL



/****** PRODUCT SaleReturns SALESTAX VALUE DEBIT TO PRODUCT SaleReturns STAX ACCOUNT  *********/







SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsSTAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SUM(SaleReturnsBody.TTLSalesTax) AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT SaleReturns EXPMINUS DEBIT TO EXPENSES MINUS ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsExpMinusAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 





                      0 AS Debit, SaleReturns.ExpMinus AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup







FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1





UNION ALL



/******* PRODUCT SaleReturns EXPPLUS DEBIT TO EXPENSES PLUS ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 





                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsExpPlusAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 





                      SaleReturns.ExpPlus AS Debit, 0 AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturns 







WHERE     SaleReturns.IsPosted = 1



UNION ALL



/********** PRODUCT SaleReturns COMMISSION DEBIT TO COMMISSION ACCOUNT  *********/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsComAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax) * IsNull(SaleReturns.ComRatio,0) / 100 AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.ComRatio, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT SaleReturns PAIDAMOUNT CREDIT TO SALESRETURNS SPECIALDISOUNT ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnsSpecialDiscountAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      0 AS Debit, SaleReturns.SpecialDiscount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, SaleReturns.CustomerID AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



                      SaleReturns.PaidAmount+SaleReturns.SpecialDiscount AS Debit, 



              SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax)



                      - ISNULL(SaleReturns.ExpMinus, 0) 



                      + ISNULL(SaleReturns.ExpPlus, 0) 



              + (SUM(SaleReturnsBody.TTLValue+SaleReturnsBody.TTLSalesTax) * IsNull(SaleReturns.ComRatio,0) / 100) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, SaleReturns.ComRatio, SaleReturns.CustomerID, SaleReturns.PaidAmount, SaleReturns.ExpMinus, SaleReturns.ExpPlus, SaleReturns.SpecialDiscount, IsApplyPartyMarkup



UNION ALL



/******* PRODUCT SaleReturns PAIDAMOUNT CREDIT TO CASH ACCOUNT  ************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 















                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 







                      0 AS Debit, SaleReturns.PaidAmount AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup





FROM SaleReturns 



WHERE     SaleReturns.IsPosted = 1



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT DEBIT TO COSTOFGOODSaleReturns ACCOUNT *************/





SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATCostOfSaleReturnAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



              0 AS Debit,



                      SUM(SaleReturnsBody.Cost*((SaleReturnsBody.Qty+SaleReturnsBody.Bonus)*SaleReturnsBody.Multiplier)) AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup



UNION ALL



/*********** PRODUCT SaleReturns NET AMOUNT DEBIT TO COSTOFGOODSaleReturns ACCOUNT *************/



SELECT     'SR' AS VOUCHERTYPE, SaleReturns.SaleReturnID AS VoucherNo, 



                      SaleReturns.SaleReturnDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSaleReturnStockValueAccount') AS AccountNo, IsNull(SaleReturns.Narration,' ') AS Narration, 



              SUM(SaleReturnsBody.Cost*((SaleReturnsBody.Qty+SaleReturnsBody.Bonus)*SaleReturnsBody.Multiplier)) AS Debit,



                      0 AS Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM SaleReturnsBody INNER JOIN

    SaleReturns ON 

    SaleReturnsBody.SaleReturnID = SaleReturns.SaleReturnID



WHERE     SaleReturns.IsPosted = 1



GROUP BY SaleReturns.SaleReturnID, SaleReturns.SaleReturnDate, SaleReturns.IsPosted, SaleReturns.Narration, IsApplyPartyMarkup





    /********************* END PRODUCT GENERAL SALES RETURN *************************************/

UNION ALL

/*************** STARTING BROKERAGE FORMS********************************************/



/*************** CONTRACT INVOICE PURCHASE ********************************************/

/*************** GROSS PURCHASE VALUE AS DEBIT TO PURCHASE ACCOUNT********************************************/



    SELECT 'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, 

    NULL AS DepositedDate, NULL AS ReconcileDate,  dbo.RegistryValue('ATPurchaseAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

           SUM(ContractInvoiceBody.TotalValue) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

    FROM ContractInvoiceBody INNER JOIN  ContractInvoice ON ContractInvoiceBody.ContractPurID = ContractInvoice.ContractPurID



    WHERE ContractInvoice.IsSaleInvoice=0 and dbo.RegistryValue('StopApplyGrossToPartyAccount')=0

    

    GROUP BY ContractInvoice.ContractPurID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice,ContractInvoice.Narration, ContractInvoice.Received, IsApplyPartyMarkup



UNION ALL

/*************** BARDANA VALUE AS DEBIT TO PURCHASE BARDANA ACCOUNT ********************************************/

    SELECT 'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, 

    NULL AS DepositedDate, NULL AS ReconcileDate,  '11327' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

        Sum(ContractInvoiceBody.PQty * ContractInvoiceBody.Bardana) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

    FROM ContractInvoiceBody INNER JOIN  ContractInvoice ON ContractInvoiceBody.ContractPurID = ContractInvoice.ContractPurID



    WHERE ContractInvoice.IsSaleInvoice=0 and isnull(ContractInvoice.isBardana,1) = 1 and dbo.RegistryValue('StopApplyGrossToPartyAccount')=0

    



    GROUP BY ContractInvoice.ContractPurID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice,ContractInvoice.Narration, IsApplyPartyMarkup

HAVING  Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana)  > 0 or Sum(ContractInvoiceBody.PQty * ContractInvoiceBody.Bardana) > 0

UNION ALL



/*************** EXPENCE PLUS AS DEBIT TO PURCHASE EXP PLUS ACCOUNT ********************************************/

 SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpPlusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      ContractInvoice.OtherExpencePlus AS Debit, 0 AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 0 and ContractInvoice.OtherExpencePlus > 0



UNION ALL

/*************** EXPENCE MINUS AS CREDIT TO PURCHASE EXP MINUS ACCOUNT ********************************************/



SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATPurchaseExpMinusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      0 AS Debit, ContractInvoice.OtherExpenceMinus AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 0 and ContractInvoice.OtherExpenceMinus > 0





UNION ALL

/*********** CONTRACT PURCHASE INVOICE NET AMOUNT CREDIT TO VENDOR ACCOUNT *************/



SELECT 'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



    AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, ContractInvoice.PartyID AS AccountNo, 

    

    IsNull(ContractInvoice.Narration,'') AS Narration, ContractInvoice.Received AS Debit, 



       ISNULL((ContractInvoice.NetValue- (case when dbo.RegistryValue('StopApplyGrossToPartyAccount')=1 then SUM(ContractInvoiceBody.TotalValue)+Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) else 0 end)   )+ContractInvoice.Received, 0) Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



    FROM ContractInvoiceBody INNER JOIN ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId



    WHERE (ContractInvoice.IsSaleInvoice = 0) GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.PartyID, ContractInvoice.Received, 



    ContractInvoice.Narration, ContractInvoice.NetValue, IsApplyPartyMarkup



UNION ALL

/******* CONTRACT PURCHASE ADVANCE AMOUNT CREDIT TO CASH ACCOUNT  ************/

SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      0 AS Debit, ContractInvoice.Received AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice=0



/********************* END CONTRACT INVOICE PURCHASE*************************************/

UNION ALL

/********************* CONTRACT INVOICE SALES *************************************/



/****** CONTRACT Sales GROSS VALUE CREDIT TO Sales ACCOUNT  *********/

SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      0 AS Debit,  SUM(ContractInvoiceBody.TotalValue) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup





FROM ContractInvoiceBody INNER JOIN

    ContractInvoice ON 

    ContractInvoiceBody.ContractPurID = ContractInvoice.ContractPurID



WHERE     ContractInvoice.IsSaleInvoice = 1 and dbo.RegistryValue('StopApplyGrossToPartyAccount')=0



GROUP BY ContractInvoice.ContractPurID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice,ContractInvoice.Narration, ContractInvoice.Received, IsApplyPartyMarkup



UNION ALL

/*************** BARDANA VALUE AS CREDIT TO SALE BARDANA ACCOUNT ********************************************/

    SELECT 'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, 

    NULL AS DepositedDate, NULL AS ReconcileDate,  '415' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

        0 AS Debit, Sum(ContractInvoiceBody.PQty * ContractInvoiceBody.Bardana) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

    FROM ContractInvoiceBody INNER JOIN  ContractInvoice ON ContractInvoiceBody.ContractPurID = ContractInvoice.ContractPurID



    WHERE ContractInvoice.IsSaleInvoice=1 and isnull(ContractInvoice.isBardana,1) = 1 and dbo.RegistryValue('StopApplyGrossToPartyAccount')=0

    

    GROUP BY ContractInvoice.ContractPurID, ContractInvoice.CDate, ContractInvoice.IsSaleInvoice,ContractInvoice.Narration, IsApplyPartyMarkup



HAVING  Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana)  > 0 or Sum(ContractInvoiceBody.PQty * ContractInvoiceBody.Bardana) > 0



UNION ALL

/**************** CONTRACT SALE INVOICE EXP PLUS AS CREDIT TO SALES EXP PLUS  ACCOUNT  *********/

SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpPlusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                       0 AS Debit, 



              Isnull(ContractInvoice.OtherExpencePlus,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1





UNION ALL

/**************** CONTRACT SALE INVOICE TELEPHONE,INSURANCE,FUEL,STITCHING EXP Credit To  ACCOUNTs  *********/



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58011' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 



              Isnull(ContractInvoice.AddTelePhoneExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '58012' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 



              Isnull(ContractInvoice.AddInsuranceExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1



UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 





                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, '58013' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 





              Isnull(ContractInvoice.AddFuelExp,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1





UNION ALL



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 





                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 





                      AS ReconcileDate, '58014' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 

                       0 AS Debit, 















              Isnull(ContractInvoice.AddStitchingExp,0)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 







/*************************************************************************************************************/





UNION ALL

/**************** CONTRACT SALE INVOICE EXP MINUS AS DEBIT TO SALES EXP MINUS  ACCOUNT  *********/





SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('ATSalesExpMinusAccount') AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      ContractInvoice.OtherExpenceMinus AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 





UNION ALL



/*********** CONTRACT SALE Invoice NET AMOUNT DEBIT TO CUSTOMER ACCOUNT *************/





SELECT 'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 



    AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, ContractInvoice.PartyID AS AccountNo, 

    

    IsNull(ContractInvoice.Narration,'') AS Narration,     ISNULL(ContractInvoice.NetValue -(case when dbo.RegistryValue('StopApplyGrossToPartyAccount')=1 then SUM(ContractInvoiceBody.TotalValue) + Sum(ContractInvoiceBody.BagsQty * ContractInvoiceBody.Bardana) else 0 end)   +ContractInvoice.Received, 0) AS Debit, ContractInvoice.Received As Credit , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



    FROM ContractInvoiceBody INNER JOIN ContractInvoice ON ContractInvoiceBody.ContractPurId = ContractInvoice.ContractPurId



    WHERE (ContractInvoice.IsSaleInvoice = 1)  GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.PartyID, ContractInvoice.Received, 



    ContractInvoice.Narration, ContractInvoice.NetValue, IsApplyPartyMarkup



UNION ALL

/********************* CONTRACT Sales Invoice RECEIVEDAMOUNT as DEBIT TO CASH ACCOUNT  *********/



SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, dbo.RegistryValue('AccCashAccount') AS AccountNo,IsNull(ContractInvoice.Narration ,'') AS Narration, 



                      ContractInvoice.Received AS Debit, 



              0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 



/********************* END CONTRACT INVOICE SALES**********************/
GO

-- View: VuAccVouchersOthersShorts
IF OBJECT_ID(N'[dbo].[VuAccVouchersOthersShorts]', N'V') IS NOT NULL
    DROP VIEW [dbo].[VuAccVouchersOthersShorts];
GO
CREATE VIEW [dbo].[VuAccVouchersOthersShorts]  AS 



/*************** BROKERAGE SHORT PURCHASE ********************************************/

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '44003' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.CommissionAmount) AS Debit, 0  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0)   AND ContractInvoice.IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      (SUM(ContractInvoice.CommissionAmount) > 0)



UNION ALL

SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '59001'  AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.Discount)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 0) AND ContractInvoice.IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      (SUM(ContractInvoice.Discount) > 0)



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '59002'  AS AccountNo, ContractInvoice.Narration, 

                      round(SootaliAmount,0) AS Debit, 0 AS Credit, 

                      ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.MinusSootli, 0) = 0) AND ContractInvoice.IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SootaliAmount, ContractInvoice.IsApplyPartyMarkup , ContractInvoice.CropSeason 

having ContractInvoice.SootaliAmount <> 0

UNION ALL

SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      SUM(Credit) AS Credit, IsApplyPartyMarkup , CropSeason 

FROM         (SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '59003' AS AccountNo, ContractInvoice.Narration, 

                                              ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) 

                                              = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.PurMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori ELSE round(((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.PurMazdori, 0) END WHEN ISNULL(PMazdoriWOShort, 0) 

                                              > 0 THEN ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori ELSE ((ContractInvoiceBody.TotalKgs) / ContractInvoiceBody.MoundStd) 

                                              * ContractInvoice.PurMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.PurMazdori END, 0) AS Debit, 0 AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice.AddMazdori, 0) = 1) AND (ContractInvoice.PurMazdori > 0)  AND ContractInvoice.IsShortInvoice = 1) AS tmp



GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup , CropSeason 



UNION ALL



SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '59004' AS AccountNo, ContractInvoice.Narration, 

                     isnull(ROUND(SUM(ContractInvoice.ManshyanaAmount),0),0)  AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) And (Isnull(ContractInvoice.AddManshyana,0) = 1) and IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.SaleManshyana, IsApplyPartyMarkup,ContractInvoice.MunshyanaPerMnd , ContractInvoice.CropSeason 

HAVING      (SUM(ContractInvoice.ManshyanaAmount) > 0)



UNION ALL





SELECT     'BP' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '59005' AS AccountNo, ContractInvoice.Narration, 

                     ROUND(  ContractInvoice.VehicalCharges ,0) AS Debit, 0 AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 0) and IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges, IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      (ContractInvoice.VehicalCharges <> 0)

UNION ALL

 SELECT     'CPI' AS VOUCHERTYPE, ContractInvoice.ContractPurID AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '59006' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                      ContractInvoice.OtherExpencePlus AS Debit, 0 AS Credit              , IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup

                       , ContractInvoice.CropSeason 

FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 0 and ContractInvoice.OtherExpencePlus > 0 and IsShortInvoice = 1

UNION ALL



/*************** BROKERAGE SHORT SALE ********************************************/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '44001' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, SUM(ContractInvoice.CommissionAmount)  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) and IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      (SUM(ContractInvoice.CommissionAmount) > 0)





UNION ALL

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '44002' AS AccountNo, ContractInvoice.Narration, 

                      SUM(ContractInvoice.Discount) AS Debit, 0  AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice WHERE     (ContractInvoice.IsSaleInvoice = 1) And IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      (SUM(ContractInvoice.Discount) > 0)



UNION ALL

SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit)AS Debit, 

                      round(SUM(Credit),0)AS Credit, IsApplyPartyMarkup , CropSeason 

FROM         (SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510001' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                                              

                                              ROUND(CASE WHEN ContractInvoice.IsShortInvoice = 0 THEN  CASE WHEN ISNULL(ContractInvoice.MazdoriPercent, 0) = 1 THEN (ContractInvoiceBody.TotalValue) 

                                              * ContractInvoice.SaleMazdori / 100 WHEN ContractInvoice.MazdoriPerbag = 1 THEN CASE WHEN isnull((ContractInvoiceBody.WeightPerBag),

                                               0) = 0 THEN ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              > 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleMazdori END 

                                              when isnull(SMazdoriWOShort,0) > 0 then

                                              ((ContractInvoiceBody.TotalKgs + ContractInvoiceBody.Shortage) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori

                                              

                                              ELSE ((ContractInvoiceBody.TotalKgs) 

                                              / ContractInvoiceBody.MoundStd) * ContractInvoice.SaleMazdori END ELSE ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) 

                                              * ContractInvoice.SaleMazdori END, 2)AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) 

                                              AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice.AddMazdori, 0) = 1) AND ((isnull(ContractInvoice.SaleMazdori,0) > 0)) and IsShortInvoice = 1 )

                      AS temp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup , CropSeason 



UNION ALL



SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, SUM(Debit) AS Debit, 

                      SUM(Credit) AS Credit, IsApplyPartyMarkup , CropSeason 

FROM         (SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                                              AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510002' AS AccountNo, ContractInvoice.Narration, 

                                              0 AS Debit, 

                                              ROUND(

                                              --CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) = 0 THEN 

                                              ((ContractInvoiceBody.BagsQty) + (Isnull(ContractInvoiceBody.PQty, 0))) * ContractInvoice.SaleSootli 

                                              --ELSE ((ContractInvoiceBody.TotalKgs / CASE WHEN isnull((ContractInvoiceBody.WeightPerBag), 0) 

                                              --> 0 THEN ContractInvoiceBody.WeightPerBag ELSE 1 END)) * ContractInvoice.SaleSootli END

                                              , 0) AS Credit, 

                                              0 AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

                       FROM          ContractInvoice INNER JOIN

                                              ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE      (ContractInvoice.IsSaleInvoice = 1)  AND (ContractInvoice.SaleSootli > 0) and IsShortInvoice = 1) AS Temp

GROUP BY VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, IsApplyPartyMarkup , CropSeason 

UNION ALL





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510008' AS AccountNo, ContractInvoice.Narration, 

                     0  AS Debit, ROUND((ContractInvoice.ManshyanaAmount), 0) AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 

                      0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.IsApplyPartyMarkup,ContractInvoice.ManshyanaAmount , ContractInvoice.CropSeason 

--HAVING      (SUM(ContractInvoice.ManshyanaAmount) <> 0)



UNION ALL





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510003' AS AccountNo, ContractInvoice.Narration, 

                      0 AS Debit, ContractInvoice.VehicalCharges AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)   AND IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.VehicalCharges , IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      (ContractInvoice.VehicalCharges > 0)



UNION ALL



/*------ Telephone, Insurance, Fule and Stitching Acc-------------------*/

SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510004' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                      ContractInvoice.AddTelePhoneExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)   and IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration, ContractInvoice.AddTelePhoneExp, 

                      ContractInvoice.IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      (ContractInvoice.AddTelePhoneExp > 0)



UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510005' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                       ContractInvoice.AddInsuranceExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 



FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,  ContractInvoice.AddInsuranceExp, 

                      ContractInvoice.IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      ( ContractInvoice.AddInsuranceExp > 0) AND ISNULL(ContractInvoice.AddInsuranceExp,0) > 0





UNION ALL



SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510006' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddFuelExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1) And  IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddFuelExp, 

                      ContractInvoice.IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      ( ContractInvoice.AddFuelExp > 0)



UNION ALL





SELECT     'BS' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL 

                      AS ChequeDate, NULL AS DepositedDate, NULL AS ReconcileDate, '510007' AS AccountNo, ContractInvoice.Narration, 0 AS Debit, 

                        ContractInvoice.AddStitchingExp AS Credit, ISNULL(ContractInvoice.IsApplyPartyMarkup, 0) AS IsApplyPartyMarkup , ContractInvoice.CropSeason 

FROM         ContractInvoice INNER JOIN

                      ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

WHERE     (ContractInvoice.IsSaleInvoice = 1)  and IsShortInvoice = 1

GROUP BY ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoice.Narration,   ContractInvoice.AddStitchingExp, 

                      ContractInvoice.IsApplyPartyMarkup , ContractInvoice.CropSeason 

HAVING      ( ContractInvoice.AddStitchingExp > 0)



UNION ALL

/**************** CONTRACT SALE INVOICE EXP PLUS AS CREDIT TO SALES EXP PLUS  ACCOUNT  *********/

SELECT     'CSI' AS VOUCHERTYPE, ContractInvoice.ContractPurId AS VoucherNo, 



                      ContractInvoice.CDate AS VoucherDate, '' AS CHEQUENO, NULL AS ChequeDate, NULL AS DepositedDate, NULL 



                      AS ReconcileDate, '510009' AS AccountNo, ISNULL(ContractInvoice.Narration,'') AS Narration, 



                       0 AS Debit, 



              Isnull(ContractInvoice.OtherExpencePlus,0) AS Credit, IsNULL(IsApplyPartyMarkup,0) as IsApplyPartyMarkup , ContractInvoice.CropSeason 



FROM ContractInvoice



WHERE     ContractInvoice.IsSaleInvoice = 1 AND  Isnull(ContractInvoice.OtherExpencePlus,0) > 0 and IsShortInvoice = 1

union all

SELECT * FROM VuAccVouchersOtherAccounts
GO


-- =============================================================================
-- SECTION 4: STORED PROCEDURES (9 Procedures)
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SPAccAccountsBalances]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SPAccAccountsBalances]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SPAccAccountsBalances];
GO

CREATE PROCEDURE SPAccAccountsBalances @FromDate smalldatetime, @ToDate smalldatetime  



 AS



/* RETURN COLUMN (AccountNo, OpeningDebit, OpeningCredit, Credit, Debit) */



BEGIN



IF EXISTS(SELECT 1 FROM tempdb..sysobjects WHERE NAME LIKE '#AccountsBalances%' And type = 'U') Drop table #AccountsBalances



CREATE TABLE #AccountsBalances



(AccountNo       Varchar(10),



 OpeningDebit    Decimal(14,2), 



 OpeningCredit   Decimal(14,2), 



 Credit          Decimal(14,2), 



 Debit           Decimal(14,2))



                    /*    CALCULATE OPENING BALANCES    */



INSERT Into #AccountsBalances



SELECT ChartOfAccounts.AccountNo, 



    ChartOfAccounts.OpeningDebit + IsNULL(SUM(VuAccVouchersAll.DEBIT),0) AS OpeningDebit, 



    ChartOfAccounts.OpeningCredit + IsNULL(SUM(VuAccVouchersAll.CREDIT),0) AS OpeningCredit,0,0



    FROM ChartOfAccounts LEFT OUTER JOIN VuAccVouchersAll ON ChartOfAccounts.AccountNo = VuAccVouchersAll.AccountNo And VuAccVouchersAll.VoucherDate < @FromDate



    WHERE ChartOfAccounts .IsDetailed=1 



    GROUP BY ChartOfAccounts.AccountNo, ChartOfAccounts.OpeningDebit, ChartOfAccounts.OpeningCredit



                    /*    CALCULATE CURRENT BALANCES    */



INSERT Into #AccountsBalances



SELECT ChartOfAccounts.AccountNo, 0,0,



    IsNULL(SUM(VuAccVouchersAll.CREDIT),0) AS Credit,



    IsNULL(SUM(VuAccVouchersAll.DEBIT),0) AS Debit



    FROM ChartOfAccounts INNER JOIN VuAccVouchersAll ON ChartOfAccounts.AccountNo = VuAccVouchersAll.ACCOUNTNO



    WHERE VuAccVouchersAll.VoucherDate >= @FromDate And VuAccVouchersAll.VoucherDate <= @ToDate



    GROUP BY ChartOfAccounts.AccountNo, ChartOfAccounts.OpeningDebit, ChartOfAccounts.OpeningCredit



                    /*     POST BALANCES INTO PARENT ACCOUNTS    */



INSERT Into #AccountsBalances



SELECT ChartOfAccounts.AccountNo, SUM(#AccountsBalances.OpeningDebit) As OpeningDebit, SUM(#AccountsBalances.OpeningCredit) As OpeningCredit,



    SUM(#AccountsBalances.Debit) As Debit, SUM(#AccountsBalances.Credit) As Credit



    FROM ChartOfAccounts INNER JOIN #AccountsBalances ON ChartOfAccounts.AccountNo = LEFT(#AccountsBalances.AccountNo,LEN(ChartOfAccounts.AccountNo))



    WHERE ChartOfAccounts.IsDetailed=0



    GROUP BY ChartOfAccounts.AccountNo



SELECT * FROM #AccountsBalances ORDER BY AccountNo



END

GO

-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SPAccAccountsBalancesFinal]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SPAccAccountsBalancesFinal]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SPAccAccountsBalancesFinal];
GO

CREATE PROCEDURE [dbo].[SPAccAccountsBalancesFinal] (@FromDate smalldatetime, @ToDate smalldatetime , @AccNo varchar(10)  = '%'  )



 AS



BEGIN



IF EXISTS(SELECT 1 FROM tempdb..sysobjects WHERE NAME LIKE '#AccountsBalances%' And type = 'U') Drop table #AccountsBalances



IF EXISTS(SELECT 1 FROM tempdb..sysobjects WHERE NAME LIKE '#AllVouchers%' And type = 'U') Drop table #AllVouchers



CREATE TABLE #AccountsBalances



(AccountNo    Varchar(10),



 AccountName    Varchar(150),



 AccountType    Varchar(11),



 IsDetailed    Bit,



 PLFlag        varchar(1),



 BalFlag        bit,



 OpeningDebit    Decimal(14,2), 



 OpeningCredit    Decimal(14,2), 



 Credit        Decimal(14,2), 



 Debit        Decimal(14,2)) 



            /*    Saving All Vouchers data into table for faster result    */



Delete From tbl_VuAccVouchersAll

INSERT INTO tbl_VuAccVouchersAll SELECT     VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, ROUND(Debit,2), ROUND(Credit,2), IsApplyPartyMarkup, CropSeason FROM         VuAccVouchersAccounts where ACCOUNTNO like @AccNo

INSERT INTO tbl_VuAccVouchersAll SELECT  VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, ROUND(Debit,2), ROUND(Credit,2), IsApplyPartyMarkup, CropSeason  FROM VuAccVouchersOthers  where ACCOUNTNO like @AccNo

INSERT INTO tbl_VuAccVouchersAll SELECT  VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, ROUND(Debit,2), ROUND(Credit,2), IsApplyPartyMarkup, CropSeason  FROM VuAccVouchersOthersExtension  where ACCOUNTNO like @AccNo

INSERT INTO tbl_VuAccVouchersAll SELECT  VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, ROUND(Debit,0), ROUND(Credit,0), IsApplyPartyMarkup , NULL CropSeason FROM VuPestroVouchers  where ACCOUNTNO like @AccNo

INSERT INTO tbl_VuAccVouchersAll SELECT  VOUCHERTYPE, VoucherNo, VoucherDate, CHEQUENO, ChequeDate, DepositedDate, ReconcileDate, AccountNo, Narration, ROUND(Debit,2), ROUND(Credit,2), IsApplyPartyMarkup , CropSeason FROM VuAccVouchersOthersCrop  where ACCOUNTNO like @AccNo

Select * into #AllVouchers FROM tbl_VuAccVouchersAll





                    /*    CALCULATE OPENING BALANCES    */



INSERT Into #AccountsBalances



SELECT ChartOfAccounts.AccountNo, ChartOfAccounts.AccountName, ChartOfAccounts.AccountType, ChartOfAccounts.IsDetailed,  ChartOfAccounts.PLFlag, ChartOfAccounts.BalFlag, 



    ChartOfAccounts.OpeningDebit + SUM(IsNULL(V.DEBIT,0)) AS OpeningDebit, 



    ChartOfAccounts.OpeningCredit + SUM(IsNULL(V.CREDIT,0)) AS OpeningCredit,0 As Credit,0 As Debit



    FROM ChartOfAccounts LEFT OUTER JOIN #AllVouchers V ON ChartOfAccounts.AccountNo = V.AccountNo And isnull(V.VoucherDate,'1/1/1900') < @FromDate



    WHERE ChartOfAccounts .IsDetailed=1 



    GROUP BY ChartOfAccounts.AccountNo, ChartOfAccounts.AccountName, ChartOfAccounts.AccountType, ChartOfAccounts.IsDetailed, ChartOfAccounts.OpeningDebit, ChartOfAccounts.OpeningCredit,  ChartOfAccounts.PLFlag, ChartOfAccounts.BalFlag



                    /*    CALCULATE CURRENT BALANCES    */



INSERT Into #AccountsBalances



SELECT ChartOfAccounts.AccountNo, ChartOfAccounts.AccountName, ChartOfAccounts.AccountType, ChartOfAccounts.IsDetailed,  ChartOfAccounts.PLFlag, ChartOfAccounts.BalFlag, 0,0,



    IsNULL(SUM(V.CREDIT),0) AS Credit, IsNULL(SUM(V.DEBIT),0) AS Debit



    FROM ChartOfAccounts INNER JOIN #AllVouchers V ON ChartOfAccounts.AccountNo = V.ACCOUNTNO



    WHERE V.VoucherDate between @FromDate And @ToDate



    GROUP BY ChartOfAccounts.AccountNo, ChartOfAccounts.AccountName, ChartOfAccounts.AccountType, ChartOfAccounts.IsDetailed, ChartOfAccounts.OpeningDebit, ChartOfAccounts.OpeningCredit,  ChartOfAccounts.PLFlag, ChartOfAccounts.BalFlag



                    /*     POST BALANCES INTO PARENT ACCOUNTS    */



INSERT Into #AccountsBalances



SELECT ChartOfAccounts.AccountNo, ChartOfAccounts.AccountName, ChartOfAccounts.AccountType, ChartOfAccounts.IsDetailed, ChartOfAccounts.PLFlag, ChartOfAccounts.BalFlag, SUM(#AccountsBalances.OpeningDebit) As OpeningDebit, SUM(#AccountsBalances.OpeningCredit) As OpeningCredit,



    SUM(#AccountsBalances.Credit) As Credit, SUM(#AccountsBalances.Debit) As Debit



    FROM ChartOfAccounts INNER JOIN #AccountsBalances ON ChartOfAccounts.AccountNo = LEFT(#AccountsBalances.AccountNo,LEN(ChartOfAccounts.AccountNo))



    WHERE ChartOfAccounts.IsDetailed=0



    GROUP BY ChartOfAccounts.AccountNo, ChartOfAccounts.AccountName, ChartOfAccounts.AccountType, ChartOfAccounts.IsDetailed, ChartOfAccounts.PLFlag, ChartOfAccounts.BalFlag



SELECT AccountNo, AccountName, AccountType, IsDetailed, PLFlag, BalFlag, 



    Sum(OpeningDebit) As OpeningDebit, Sum(OpeningCredit) As OpeningCredit, case when (Sum(OpeningCredit) - Sum(OpeningDebit)) > 0 then Sum(OpeningCredit) - Sum(OpeningDebit) else (Sum(OpeningCredit) - Sum(OpeningDebit))*-1 end as OpeningBal,



    case when (Sum(OpeningCredit) - Sum(OpeningDebit)) > 0 then 'Cr' else 'Dr' end as OpeningBalType,



    Sum(Debit) As Debit, Sum(Credit) As Credit, case when (Sum(Credit) - Sum(Debit)) > 0 then Sum(Credit) - Sum(Debit) else (Sum(Credit) - Sum(Debit))*-1 end As Bal,



    case when (Sum(Credit) - Sum(Debit)) > 0 then 'Cr' else 'Dr' end As BalType,



    Sum(OpeningDebit) + Sum(Debit) As ClosingDebit, Sum(OpeningCredit) + Sum(Credit) As ClosingCredit, case when (Sum(OpeningCredit) + Sum(Credit) - Sum(OpeningDebit) - Sum(Debit)) > 0 then Sum(OpeningCredit) + Sum(Credit) - Sum(OpeningDebit) - Sum(Debit) else (Sum(OpeningCredit) + Sum(Credit) - Sum(OpeningDebit) - Sum(Debit))*-1 end As ClosingBal,





    case when (Sum(OpeningCredit) + Sum(Credit) - Sum(OpeningDebit) - Sum(Debit)) > 0 then 'Cr' else 'Dr' end As ClosingBalType







    FROM #AccountsBalances 

where AccountNo like @AccNO

    Group By  AccountNo, AccountName, AccountType, IsDetailed, PLFlag, BalFlag



END

GO

-- -----------------------------------------------------------------------------
-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SPAgenTechCashBook]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SPAgenTechCashBook]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SPAgenTechCashBook];
GO

CREATE PROCEDURE [SPAgenTechCashBook]

    @FromDate smalldatetime = '1/1/1980', 

    @ToDate   smalldatetime = '1/1/2010',

    @ProceedingZeros tinyint = 5

AS

    -- STEP 0: Getting the Opening Balance

    Select '-' as VoucherType,0 as voucherno,'-' as StrVoucherNo,@FromDate as VoucherDate,case when isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit >= 0 then isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit else 0 end as Debit,

           case when isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit < 0 then abs(isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit) else 0 end as Credit,'Opening balance' as Narration,'-' as AccountNo,'-' As AccountName From ChartofAccounts,

    (Select sum(Credit) as Debit,0 as Credit From CreditVouchersBody inner join CreditVouchers on 

    Creditvouchers.voucherno = creditvouchersbody.voucherno AND CreditVouchers.VoucherDate = CreditVouchersBody.VoucherDate

    where creditvouchers.voucherdate < @fromdate

    UNION ALL

    Select 0 as Debit,sum(Debit) as Credit From DebitVouchersBody inner join DebitVouchers on 

    Debitvouchers.voucherno = DebitVouchersbody.voucherno AND DebitVouchers.VoucherDate = DebitVouchersBody.VoucherDate

    where Debitvouchers.voucherdate < @fromdate

    UNION ALL

    Select sum(Debit) as Debit,sum(Credit) as Credit From JournalVouchersBody inner join JournalVouchers on 

    JournalVouchers.voucherno = JournalVouchersbody.voucherno AND JournalVouchers.VoucherDate = JournalVouchersBody.VoucherDate

    where JournalVouchers.voucherdate < @fromdate and JournalVouchersBody.AccountNo='111'

    UNION ALL

    SELECT isnull(sum(BANKCHEQUESBODY.AMOUNT), sum(BANKCHEQUES.AMOUNT)) AS DEBIT, 0 AS CREDIT FROM BANKCHEQUES INNER JOIN BANKCHEQUESBODY ON BANKCHEQUES.VOUCHERNO=BANKCHEQUESBODY.VOUCHERNO AND BANKCHEQUES.VOUCHERDATE=BANKCHEQUESBODY.VOUCHERDATE  WHERE (BANKCHEQUES.VOUCHERDATE < @FromDate) AND BANKCHEQUES.ISLOST=0 AND isnull(BANKCHEQUESBODY.ACCOUNTNO,BANKCHEQUES.ACCOUNTNO)='111'





UNION ALL

     SELECT case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0) else 0 end AS CREDIT

        FROM BANKCHEQUES BC WHERE BC.ISLOST=0 AND BC.ACCOUNTNO='111' AND (BC.VOUCHERDATE < @FromDate)

UNION ALL

     SELECT case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS CREDIT

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND BCB.ACCOUNTNO='111' AND (BC.VOUCHERDATE < @FromDate)



    UNION ALL

    SELECT 0 AS DEBIT, SUM(AMOUNT) AS CREDIT FROM BANKDEPOSIT WHERE (VOUCHERDATE < @FROMDATE) AND ISBounced=0 AND isnull(ACCOUNTNO,'111')='111'

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from Sales Where Saledate < @FromDate

    UNION ALL

    Select PaidAmount as Debit,0 as Credit from CropsSaleHeader Where Saledate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From Purchase Where Purchasedate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From CropsPurchaseHeader Where Purchasedate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from PurReturns Where PurReturndate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From salereturns Where SaleReturnDate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From PurchasesPest Where EntryDate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from PurReturnsPest Where ReturnDate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from SalesPest Where Saledate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From SaleReturnsPest Where ReturnDate < @FromDate

/* Changing By fahad  

----------------Getting Amount Calc... From Contracts AND Contract Invoices---------------------

    UNION ALL

    Select Advance as Debit,0 as Credit from Contract Where CDate < @FromDate AND IsSaleContract=1

    UNION ALL

    Select 0 as Debit ,Advance as Credit from Contract Where CDate < @FromDate AND IsSaleContract=0 ***/

    UNION ALL

    Select Received as Debit,0 as Credit from ContractInvoice Where CDate < @FromDate AND IsSaleInvoice=1

    UNION ALL

    Select 0 as Debit,Received  as Credit from ContractInvoice Where CDate < @FromDate AND IsSaleInvoice=0

) OpData

    Where ChartofAccounts.AccountNo = '111'

    Group by OpeningDebit,OpeningCredit

UNION ALL

    Select AllData.*,AccountName from ChartOfAccounts RIGHT OUTER JOIN

    (

    -- STEP 1: Obtaining the Entries From DebitVouchers

    Select 'DV' as VoucherType,DebitVouchers.VoucherNo,

        'DV-' + case when len(cast(debitvouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + debitvouchers.voucherno ,@ProceedingZeros) 

            else cast(debitvouchers.voucherno as varchar) end as StrVoucherNo,DebitVouchers.VoucherDate,

        0 as Debit,Debit as Credit,narration,AccountNo

        from debitvouchers Inner Join DebitVouchersBody on 

        DebitVouchers.VoucherNo = DebitVouchersBody.VoucherNo AND debitVouchers.VoucherDate = DebitVouchersBody.VoucherDate

        where DebitVouchers.voucherdate between @fromdate and @todate

UNION ALL

    -- STEP 2: Obtaining the Entries From CreditVouchers

    Select 'CV' as VoucherType,CreditVouchers.VoucherNo,

        'CV-' + case when len(cast(Creditvouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Creditvouchers.voucherno ,@ProceedingZeros) 

            else cast(Creditvouchers.voucherno as varchar) end as StrVoucherNo,CreditVouchers.VoucherDate,

        Credit as Debit,0 as Credit,narration,AccountNo

        from Creditvouchers Inner Join CreditVouchersBody on 

        CreditVouchers.VoucherNo = CreditVouchersBody.VoucherNo AND CreditVouchers.VoucherDate = CreditVouchersBody.VoucherDate

        where CreditVouchers.voucherdate between @fromdate and @todate

UNION ALL

    -- STEP 3: Obtaining the Entries From JournalVouchers

    Select 'JV' as VoucherType,JournalVouchers.VoucherNo,

        'JV-' + case when len(cast(JournalVouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + JournalVouchers.voucherno ,@ProceedingZeros) 

            else cast(JournalVouchers.voucherno as varchar) end as StrVoucherNo,JournalVouchers.VoucherDate,

        Debit as Debit,Credit as Credit,narration,AccountNo

        from JournalVouchers Inner Join JournalVouchersBody on 

        JournalVouchers.VoucherNo = JournalVouchersBody.VoucherNo AND JournalVouchers.VoucherDate = JournalVouchersBody.VoucherDate

        where JournalVouchers.voucherdate between @fromdate and @todate and JournalVouchersBody.AccountNo='111'

UNION ALL

    -- STEP 4: Obtaining the Entries From BANKCHEQUES

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        ISNULL(BCB.AMOUNT, BC.AMOUNT) AS DEBIT, 0 AS CREDIT, narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND isnull(BCB.ACCOUNTNO,BC.ACCOUNTNO)='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 

    -- WHT AMOUNT ON CASH

UNION ALL

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0) else 0 end AS CREDIT, 'WITH HOLDING TAX AMOUNT' as narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC WHERE BC.ISLOST=0 AND BC.ACCOUNTNO='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 

UNION ALL

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS CREDIT, 'WITH HOLDING TAX AMOUNT' as narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND BCB.ACCOUNTNO='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 



UNION ALL

    -- STEP 4: Obtaining the Entries From BANKDEPOSIT

     SELECT 'BD' AS VOUCHERTYPE, BD.VOUCHERNO, 

        'BD-' + case when len(cast(BD.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BD.voucherno ,@ProceedingZeros) 

            else cast(BD.voucherno as varchar) end as StrVoucherNo,BD.VoucherDate,

        0 AS DEBIT, BD.AMOUNT AS CREDIT,  narration, ISNULL(BD.AccountNo,'111') as AccountNo 

        FROM BANKDEPOSIT BD 

        WHERE BD.ISBounced=0 AND ISNULL(BD.AccountNo,'111')='111' AND (@FROMDATE IS NULL OR BD.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BD.VoucherDate <= @ToDate) 



UNION ALL

    -- STEP 5: Obtaining the Entries from Sales

    Select 'SI' as VoucherType,Sales.Saleid as VoucherNo,

        'SI-' + case when len(cast(Sales.SaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.SaleID ,@ProceedingZeros) 

            else cast(Sales.SaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        ReceivedAmount as Debit,0 as Credit,isnull(Sales.Narration,'Sale Invoice') as Narration,CustomerID

        from Sales where Saledate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 6: Obtaining the Entries from Purchases

    Select 'PI' as VoucherType,Purchase.PurchaseID as VoucherNo,

        'PI-' + case when len(cast(Purchase.PurchaseId as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.PurchaseID ,@ProceedingZeros) 

            else cast(Purchase.PurchaseID as varchar) end as StrVoucherNo,Purchase.PurchaseDate,

        0 as debit, PaidAmount as Credit,isnull(Purchase.Narration, 'Purchase Invoice') as Narration,VendorID

        from Purchase where PurchaseDate between @fromdate and @todate AND PaidAmount>0    

UNION ALL

    -- STEP 7: Obtaining the Entries from PurchaseReturns

    Select 'PR' as VoucherType,PurReturns.PurReturnID as VoucherNo,

        'PR-' + case when len(cast(PurReturns.PurReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + PurReturns.PurReturnID ,@ProceedingZeros) 

            else cast(PurReturns.PurReturnID as varchar) end as StrVoucherNo,PurReturns.PurReturnDate,

         ReceivedAmount as Debit,0 as credit, isnull(PurReturns.Narration, 'Purchase Return Invoice') as Narration,VendorID

        from PurReturns where PurReturnDate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 8: Obtaining the Entries from SaleReturns

    Select 'SR' as VoucherType,SaleReturns.SaleReturnID as VoucherNo,

        'SR-' + case when len(cast(SaleReturns.SaleReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + SaleReturns.SaleReturnID ,@ProceedingZeros) 

            else cast(SaleReturns.SaleReturnID as varchar) end as StrVoucherNo,SaleReturns.SaleReturndate,

        0 as debit, PaidAmount as Credit, isnull(SaleReturns.Narration, 'Sale Return Invoice') as Narration,CustomerID

        from SaleReturns where SaleReturnDate between @fromdate and @todate AND PaidAmount>0









UNION ALL

    -- STEP 5: Obtaining the Entries from Sales

    Select 'PSI' as VoucherType,Sales.Saleid as VoucherNo,

        'PSI-' + case when len(cast(Sales.SaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.SaleID ,@ProceedingZeros) 

            else cast(Sales.SaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        ReceivedAmount as Debit,0 as Credit,isnull(Sales.Remarks,'Pestro Sale Invoice') as Narration,CustomerID

        from SalesPest Sales where Saledate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 6: Obtaining the Entries from Purchases

    Select 'PPI' as VoucherType,Purchase.PurchaseID as VoucherNo,

        'PPI-' + case when len(cast(Purchase.PurchaseId as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.PurchaseID ,@ProceedingZeros) 

            else cast(Purchase.PurchaseID as varchar) end as StrVoucherNo,Purchase.EntryDate,

        0 as debit, PaidAmount as Credit,'Pestro Purchase Invoice' as Narration,VendorID

        from PurchasesPest Purchase where EntryDate between @fromdate and @todate AND PaidAmount>0    

UNION ALL

    -- STEP 7: Obtaining the Entries from PurchaseReturns

    Select 'PPR' as VoucherType,PurReturns.PReturnID as VoucherNo,

        'PPR-' + case when len(cast(PurReturns.PReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + PurReturns.PReturnID ,@ProceedingZeros) 

            else cast(PurReturns.PReturnID as varchar) end as StrVoucherNo,PurReturns.ReturnDate,

         ReceivedAmount as Debit,0 as credit, 'Pestro Purchase Return Invoice' as Narration,VendorID

        from PurReturnsPest PurReturns where ReturnDate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 8: Obtaining the Entries from SaleReturns

    Select 'PSR' as VoucherType,SaleReturns.SReturnID as VoucherNo,

        'PSR-' + case when len(cast(SaleReturns.SReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + SaleReturns.SReturnID ,@ProceedingZeros) 

            else cast(SaleReturns.SReturnID as varchar) end as StrVoucherNo,SaleReturns.ReturnDate,

        0 as debit, PaidAmount as Credit, 'Pestro Sale Return Invoice' as Narration,CustomerID

        from SaleReturnsPest SaleReturns where ReturnDate between @fromdate and @todate AND PaidAmount>0

/*--------Brokerage Project Calc here-----------*/

/* Changing By fahad  

    -- STEP 9: Obtaining the Entries From Contract Sale

UNION ALL

    Select 'CS' as VoucherType, Contract.ContractID as VoucherNo,

        'CS-' + case when len(cast(Contract.ContractID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Contract.ContractID ,@ProceedingZeros) 

            else cast(Contract.ContractID as varchar) end as StrVoucherNo,Contract.CDate,

        Advance as Debit,0 as Credit,'Contract Sale' as Narration, PartyID

        from Contract where CDate between @fromdate and @todate AND IsSaleContract=1

UNION ALL

    -- STEP 10: Obtaining the Entries From Contract Purchase

    Select 'CP' as VoucherType, Contract.ContractID as VoucherNo,

        'CP-' + case when len(cast(Contract.ContractID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Contract.ContractID ,@ProceedingZeros) 

            else cast(Contract.ContractID as varchar) end as StrVoucherNo,Contract.CDate,

        0 as Debit,Advance as Credit,'Contract Purchase' as Narration, PartyID

        from Contract where CDate between @fromdate and @todate AND IsSaleContract=0  */

UNION ALL

    -- STEP 11: Obtaining the Entries From Contract Invoice Sale

    Select 'CIS' as VoucherType, ContractInvoice.ContractPurID as VoucherNo,

        'CIS-' + case when len(cast(ContractInvoice.ContractPurID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + ContractInvoice.ContractPurID ,@ProceedingZeros) 

            else cast(ContractInvoice.ContractPurID as varchar) end as StrVoucherNo, ContractInvoice.CDate,

        Received as Debit,0 as Credit,'Contract Invoice Sale' as Narration, PartyID

        from ContractInvoice where CDate between @fromdate and @todate AND IsSaleInvoice=1 AND Received>0

UNION ALL

    -- STEP 12: Obtaining the Entries From Contract Invoice Purchase

    Select 'CIP' as VoucherType, ContractInvoice.ContractPurID as VoucherNo,

        'CIP-' + case when len(cast(ContractInvoice.ContractPurID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + ContractInvoice.ContractPurID ,@ProceedingZeros) 

            else cast(ContractInvoice.ContractPurID as varchar) end as StrVoucherNo, ContractInvoice.CDate,

        0 as Debit,Received as Credit,'Contract Invoice Purchase' as Narration, PartyID

        from ContractInvoice where CDate between @fromdate and @todate AND IsSaleInvoice=0 AND Received>0

UNION ALL

    -- STEP 13: Obtaining the Entries From Crop Purchase

    Select 'CP' as VoucherType,Purchase.CropPurchaseID as VoucherNo,

        'CP-' + case when len(cast(Purchase.CropPurchaseID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.CropPurchaseID ,@ProceedingZeros) 

            else cast(Purchase.CropPurchaseID as varchar) end as StrVoucherNo,Purchase.PurchaseDate,

        0 as debit, PaidAmount as Credit,isnull(Purchase.Remarks, 'Crop Purchase Invoice') as Narration,VendorID

        from CropsPurchaseHeader Purchase where PurchaseDate between @fromdate and @todate AND PaidAmount>0

UNION ALL

    -- STEP 14: Obtaining the Entries From Crop Sales

    Select 'CS' as VoucherType,Sales.CropSaleID as VoucherNo,

        'CS-' + case when len(cast(Sales.CropSaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.CropSaleID ,@ProceedingZeros) 

            else cast(Sales.CropSaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        PaidAmount as Debit,0 as Credit,isnull(Sales.Remarks,'Crop Sale Invoice') as Narration,CustomerID

        from CropsSaleHeader Sales where Saledate between @fromdate and @todate AND PaidAmount>0





    ) ALLDATA on 

    ChartOfAccounts.AccountNo = AllData.AccountNo

Order by VoucherDate, VoucherType, VoucherNo

GO

-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SPAgenTechCashBookExtended]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SPAgenTechCashBookExtended]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SPAgenTechCashBookExtended];
GO

CREATE PROCEDURE SPAgenTechCashBookExtended

    @FromDate smalldatetime = '1/1/1980', 

    @ToDate   smalldatetime = '1/1/2010',

    @ProceedingZeros tinyint = 5

AS

    -- STEP 0: Getting the Opening Balance

    Select '-' as VoucherType,0 as voucherno,'-' as StrVoucherNo,@FromDate as VoucherDate,case when isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit >= 0 then isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit else 0 end as Debit,

           case when isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit < 0 then abs(isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit) else 0 end as Credit,'Opening balance' as Narration,'-' as AccountNo,'-' As AccountName From ChartofAccounts,

    (Select sum(Credit) as Debit,0 as Credit From CreditVouchersBody inner join CreditVouchers on 

    Creditvouchers.voucherno = creditvouchersbody.voucherno AND CreditVouchers.VoucherDate = CreditVouchersBody.VoucherDate

    where creditvouchers.voucherdate < @fromdate

    UNION ALL

    Select 0 as Debit,sum(Debit) as Credit From DebitVouchersBody inner join DebitVouchers on 

    Debitvouchers.voucherno = DebitVouchersbody.voucherno AND DebitVouchers.VoucherDate = DebitVouchersBody.VoucherDate

    where Debitvouchers.voucherdate < @fromdate

    UNION ALL

    Select sum(Debit) as Debit,sum(Credit) as Credit From JournalVouchersBody inner join JournalVouchers on 

    JournalVouchers.voucherno = JournalVouchersbody.voucherno AND JournalVouchers.VoucherDate = JournalVouchersBody.VoucherDate

    where JournalVouchers.voucherdate < @fromdate and JournalVouchersBody.AccountNo='111'

    UNION ALL

    SELECT isnull(sum(BANKCHEQUESBODY.AMOUNT), sum(BANKCHEQUES.AMOUNT)) AS DEBIT, 0 AS CREDIT FROM BANKCHEQUES INNER JOIN BANKCHEQUESBODY ON BANKCHEQUES.VOUCHERNO=BANKCHEQUESBODY.VOUCHERNO AND BANKCHEQUES.VOUCHERDATE=BANKCHEQUESBODY.VOUCHERDATE  WHERE (BANKCHEQUES.VOUCHERDATE < @FromDate) AND BANKCHEQUES.ISLOST=0 AND isnull(BANKCHEQUESBODY.ACCOUNTNO,BANKCHEQUES.ACCOUNTNO)='111'

    UNION ALL

    SELECT 0 AS DEBIT, SUM(AMOUNT) AS CREDIT FROM BANKDEPOSIT WHERE (VOUCHERDATE < @FROMDATE) AND ISBounced=0 AND isnull(ACCOUNTNO,'111')='111'

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from Sales Where Saledate < @FromDate

    UNION ALL

    Select PaidAmount as Debit,0 as Credit from CropsSaleHeader Where Saledate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From Purchase Where Purchasedate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From CropsPurchaseHeader Where Purchasedate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from PurReturns Where PurReturndate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From salereturns Where SaleReturnDate < @FromDate





    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From PurchasesPest Where EntryDate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from PurReturnsPest Where ReturnDate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from SalesPest Where Saledate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From SaleReturnsPest Where ReturnDate < @FromDate

/* Changing By fahad  

----------------Getting Amount Calc... From Contracts AND Contract Invoices---------------------

    UNION ALL

    Select Advance as Debit,0 as Credit from Contract Where CDate < @FromDate AND IsSaleContract=1

    UNION ALL

    Select 0 as Debit ,Advance as Credit from Contract Where CDate < @FromDate AND IsSaleContract=0 ***/

    UNION ALL

    Select Received as Debit,0 as Credit from ContractInvoice Where CDate < @FromDate AND IsSaleInvoice=1

    UNION ALL

    Select 0 as Debit,Received  as Credit from ContractInvoice Where CDate < @FromDate AND IsSaleInvoice=0

) OpData

    Where ChartofAccounts.AccountNo = '111'

    Group by OpeningDebit,OpeningCredit

UNION ALL

    Select AllData.*,AccountName from ChartOfAccounts RIGHT OUTER JOIN

    (

    -- STEP 1: Obtaining the Entries From DebitVouchers

    Select 'DV' as VoucherType,DebitVouchers.VoucherNo,

        'DV-' + case when len(cast(debitvouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + debitvouchers.voucherno ,@ProceedingZeros) 

            else cast(debitvouchers.voucherno as varchar) end as StrVoucherNo,DebitVouchers.VoucherDate,

        0 as Debit,Debit as Credit,narration,AccountNo

        from debitvouchers Inner Join DebitVouchersBody on 

        DebitVouchers.VoucherNo = DebitVouchersBody.VoucherNo AND debitVouchers.VoucherDate = DebitVouchersBody.VoucherDate

        where DebitVouchers.voucherdate between @fromdate and @todate

UNION ALL

    -- STEP 2: Obtaining the Entries From CreditVouchers

    Select 'CV' as VoucherType,CreditVouchers.VoucherNo,

        'CV-' + case when len(cast(Creditvouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Creditvouchers.voucherno ,@ProceedingZeros) 

            else cast(Creditvouchers.voucherno as varchar) end as StrVoucherNo,CreditVouchers.VoucherDate,

        Credit as Debit,0 as Credit,narration,AccountNo

        from Creditvouchers Inner Join CreditVouchersBody on 

        CreditVouchers.VoucherNo = CreditVouchersBody.VoucherNo AND CreditVouchers.VoucherDate = CreditVouchersBody.VoucherDate

        where CreditVouchers.voucherdate between @fromdate and @todate

UNION ALL

    -- STEP 3: Obtaining the Entries From JournalVouchers

    Select 'JV' as VoucherType,JournalVouchers.VoucherNo,         'JV-' + case when len(cast(JournalVouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + JournalVouchers.voucherno ,@ProceedingZeros) 

            else cast(JournalVouchers.voucherno as varchar) end as StrVoucherNo,JournalVouchers.VoucherDate,

        Debit as Debit,Credit as Credit,narration,AccountNo

        from JournalVouchers Inner Join JournalVouchersBody on 

        JournalVouchers.VoucherNo = JournalVouchersBody.VoucherNo AND JournalVouchers.VoucherDate = JournalVouchersBody.VoucherDate

        where JournalVouchers.voucherdate between @fromdate and @todate and JournalVouchersBody.AccountNo='111'

UNION ALL

    -- STEP 4: Obtaining the Entries From BANKCHEQUES

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        ISNULL(BCB.AMOUNT, BC.AMOUNT) AS DEBIT, 0 AS CREDIT, narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND isnull(BCB.ACCOUNTNO,BC.ACCOUNTNO)='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 

    -- WHT AMOUNT ON CASH

UNION ALL

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0) else 0 end AS CREDIT, 'WITH HOLDING TAX AMOUNT' as narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC WHERE BC.ISLOST=0 AND BC.ACCOUNTNO='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 

UNION ALL

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS CREDIT, 'WITH HOLDING TAX AMOUNT' as narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND BCB.ACCOUNTNO='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 



UNION ALL

    -- STEP 4: Obtaining the Entries From BANKDEPOSIT

     SELECT 'BD' AS VOUCHERTYPE, BD.VOUCHERNO, 

        'BD-' + case when len(cast(BD.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BD.voucherno ,@ProceedingZeros) 

            else cast(BD.voucherno as varchar) end as StrVoucherNo,BD.VoucherDate,

        0 AS DEBIT, BD.AMOUNT AS CREDIT,  narration, ISNULL(BD.AccountNo,'111') as AccountNo 

        FROM BANKDEPOSIT BD WHERE BD.ISBounced=0 AND ISNULL(BD.AccountNo,'111')='111' AND (@FROMDATE IS NULL OR BD.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BD.VoucherDate <= @ToDate) 



UNION ALL

    -- STEP 5: Obtaining the Entries from Sales

    Select 'SI' as VoucherType,Sales.Saleid as VoucherNo,

        'SI-' + case when len(cast(Sales.SaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.SaleID ,@ProceedingZeros) 

            else cast(Sales.SaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        ReceivedAmount as Debit,0 as Credit, dbo.FunGetSalesNarration(Sales.SaleID) as Narration,CustomerID

        from Sales where Saledate between @fromdate and @todate

UNION ALL

    -- STEP 6: Obtaining the Entries from Purchases

    Select 'PI' as VoucherType,Purchase.PurchaseID as VoucherNo,

        'PI-' + case when len(cast(Purchase.PurchaseId as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.PurchaseID ,@ProceedingZeros) 

            else cast(Purchase.PurchaseID as varchar) end as StrVoucherNo,Purchase.PurchaseDate,

        0 as debit, PaidAmount as Credit, dbo.FunGetPurchaseNarration(Purchase.PurchaseId) as Narration,VendorID

        from Purchase where PurchaseDate between @fromdate and @todate

UNION ALL

    -- STEP 7: Obtaining the Entries from PurchaseReturns

    Select 'PR' as VoucherType,PurReturns.PurReturnID as VoucherNo,

        'PR-' + case when len(cast(PurReturns.PurReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + PurReturns.PurReturnID ,@ProceedingZeros) 

            else cast(PurReturns.PurReturnID as varchar) end as StrVoucherNo,PurReturns.PurReturnDate,

         ReceivedAmount as Debit,0 as credit, dbo.FunGetPurReturnsNarration(PurReturns.PurReturnID) as Narration,VendorID

        from PurReturns where PurReturnDate between @fromdate and @todate

UNION ALL

    -- STEP 8: Obtaining the Entries from SaleReturns

    Select 'SR' as VoucherType,SaleReturns.SaleReturnID as VoucherNo,

        'SR-' + case when len(cast(SaleReturns.SaleReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + SaleReturns.SaleReturnID ,@ProceedingZeros) 

            else cast(SaleReturns.SaleReturnID as varchar) end as StrVoucherNo,SaleReturns.SaleReturndate,

        0 as debit, PaidAmount as Credit, dbo.FunGetSaleReturnsNarration(SaleReturns.SaleReturnID) as Narration,CustomerID

        from SaleReturns where SaleReturnDate between @fromdate and @todate









UNION ALL

    -- STEP 5: Obtaining the Entries from Sales

    Select 'PSI' as VoucherType,Sales.Saleid as VoucherNo,

        'PSI-' + case when len(cast(Sales.SaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.SaleID ,@ProceedingZeros) 

            else cast(Sales.SaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        ReceivedAmount as Debit,0 as Credit,dbo.FunGetPSNarration(Sales.SaleID) as Narration,CustomerID

        from SalesPest Sales where Saledate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 6: Obtaining the Entries from Purchases

    Select 'PPI' as VoucherType,Purchase.PurchaseID as VoucherNo,

        'PPI-' + case when len(cast(Purchase.PurchaseId as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.PurchaseID ,@ProceedingZeros) 

            else cast(Purchase.PurchaseID as varchar) end as StrVoucherNo,Purchase.EntryDate,

        0 as debit, PaidAmount as Credit,dbo.FunGetPPNarration(Purchase.PurchaseID) as Narration,VendorID

        from PurchasesPest Purchase where EntryDate between @fromdate and @todate AND PaidAmount>0    

UNION ALL

    -- STEP 7: Obtaining the Entries from PurchaseReturns

    Select 'PPR' as VoucherType,PurReturns.PReturnID as VoucherNo,

        'PPR-' + case when len(cast(PurReturns.PReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + PurReturns.PReturnID ,@ProceedingZeros) 

            else cast(PurReturns.PReturnID as varchar) end as StrVoucherNo,PurReturns.ReturnDate,

         ReceivedAmount as Debit,0 as credit, dbo.FunGetPPRNarration(PurReturns.PReturnID) as Narration,VendorID

        from PurReturnsPest PurReturns where ReturnDate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 8: Obtaining the Entries from SaleReturns

    Select 'PSR' as VoucherType,SaleReturns.SReturnID as VoucherNo,

        'PSR-' + case when len(cast(SaleReturns.SReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + SaleReturns.SReturnID ,@ProceedingZeros) 

            else cast(SaleReturns.SReturnID as varchar) end as StrVoucherNo,SaleReturns.ReturnDate,

        0 as debit, PaidAmount as Credit, dbo.FunGetPSRNarration(SaleReturns.SReturnID) as Narration,CustomerID

        from SaleReturnsPest SaleReturns where ReturnDate between @fromdate and @todate AND PaidAmount>0

/*--------Brokerage Project Calc here-----------*/

    -- STEP 9: Obtaining the Entries From Contract Sale

UNION ALL

    Select 'CS' as VoucherType, Contract.ContractID as VoucherNo,

        'CS-' + case when len(cast(Contract.ContractID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Contract.ContractID ,@ProceedingZeros) 

            else cast(Contract.ContractID as varchar) end as StrVoucherNo,Contract.CDate,

        Advance as Debit,0 as Credit,'Contract Sale' as Narration, PartyID

        from Contract where CDate between @fromdate and @todate AND IsSaleContract=1

UNION ALL

    -- STEP 10: Obtaining the Entries From Contract Purchase

    Select 'CP' as VoucherType, Contract.ContractID as VoucherNo,

        'CP-' + case when len(cast(Contract.ContractID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Contract.ContractID ,@ProceedingZeros) 

            else cast(Contract.ContractID as varchar) end as StrVoucherNo,Contract.CDate,

        0 as Debit,Advance as Credit,'Contract Purchase' as Narration, PartyID         from Contract where CDate between @fromdate and @todate AND IsSaleContract=0

UNION ALL

    -- STEP 11: Obtaining the Entries From Contract Invoice Sale

    Select 'CIS' as VoucherType, ContractInvoice.ContractPurID as VoucherNo,

        'CIS-' + case when len(cast(ContractInvoice.ContractPurID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + ContractInvoice.ContractPurID ,@ProceedingZeros) 

            else cast(ContractInvoice.ContractPurID as varchar) end as StrVoucherNo, ContractInvoice.CDate,

        Received as Debit,0 as Credit,'Contract Invoice Sale' as Narration, PartyID

        from ContractInvoice where CDate between @fromdate and @todate AND IsSaleInvoice=1

UNION ALL

    -- STEP 12: Obtaining the Entries From Contract Invoice Purchase

    Select 'CIP' as VoucherType, ContractInvoice.ContractPurID as VoucherNo,

        'CIP-' + case when len(cast(ContractInvoice.ContractPurID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + ContractInvoice.ContractPurID ,@ProceedingZeros) 

            else cast(ContractInvoice.ContractPurID as varchar) end as StrVoucherNo, ContractInvoice.CDate,

        0 as Debit,Received as Credit,'Contract Invoice Purchase' as Narration, PartyID

        from ContractInvoice where CDate between @fromdate and @todate AND IsSaleInvoice=0

UNION ALL

    -- STEP 13: Obtaining the Entries From Crop Purchase

    Select 'CP' as VoucherType,Purchase.CropPurchaseID as VoucherNo,

        'CP-' + case when len(cast(Purchase.CropPurchaseID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.CropPurchaseID ,@ProceedingZeros) 

            else cast(Purchase.CropPurchaseID as varchar) end as StrVoucherNo,Purchase.PurchaseDate,

        0 as debit, PaidAmount as Credit,isnull(Purchase.Remarks, 'Crop Purchase Invoice') as Narration,VendorID

        from CropsPurchaseHeader Purchase where PurchaseDate between @fromdate and @todate AND PaidAmount>0

UNION ALL

    -- STEP 14: Obtaining the Entries From Crop Sales

    Select 'CS' as VoucherType,Sales.CropSaleID as VoucherNo,

        'CS-' + case when len(cast(Sales.CropSaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.CropSaleID ,@ProceedingZeros) 

            else cast(Sales.CropSaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        PaidAmount as Debit,0 as Credit,isnull(Sales.Remarks,'Crop Sale Invoice') as Narration,CustomerID

        from CropsSaleHeader Sales where Saledate between @fromdate and @todate AND PaidAmount>0





    ) ALLDATA on 

    ChartOfAccounts.AccountNo = AllData.AccountNo

Order by VoucherDate

GO

-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SPAgenTechCashBookNewIDs]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SPAgenTechCashBookNewIDs]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SPAgenTechCashBookNewIDs];
GO

CREATE PROCEDURE SPAgenTechCashBookNewIDs

    @FromDate smalldatetime = '1/1/1980', 

    @ToDate   smalldatetime = '1/1/2010',

    @ProceedingZeros tinyint = 5

AS

    -- STEP 0: Getting the Opening Balance

    Select '-' as VoucherType,0 as voucherno,'-' as StrVoucherNo,@FromDate as VoucherDate,case when isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit >= 0 then isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit else 0 end as Debit,

           case when isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit < 0 then abs(isnull(Sum(Debit),0)+OpeningDebit-isnull(Sum(Credit),0)-OpeningCredit) else 0 end as Credit,'Opening balance' as Narration,'-' as AccountNo,'-' As AccountName From ChartofAccounts,

    (Select sum(Credit) as Debit,0 as Credit From CreditVouchersBody inner join CreditVouchers on 

    Creditvouchers.voucherno = creditvouchersbody.voucherno AND CreditVouchers.VoucherDate = CreditVouchersBody.VoucherDate

    where creditvouchers.voucherdate < @fromdate

    UNION ALL

    Select 0 as Debit,sum(Debit) as Credit From DebitVouchersBody inner join DebitVouchers on 

    Debitvouchers.voucherno = DebitVouchersbody.voucherno AND DebitVouchers.VoucherDate = DebitVouchersBody.VoucherDate

    where Debitvouchers.voucherdate < @fromdate



    UNION ALL



    Select sum(Debit) as Debit,sum(Credit) as Credit From JournalVouchersBody inner join JournalVouchers on 

    JournalVouchers.voucherno = JournalVouchersbody.voucherno AND JournalVouchers.VoucherDate = JournalVouchersBody.VoucherDate

    where JournalVouchers.voucherdate < @fromdate and JournalVouchersBody.AccountNo='111'

    UNION ALL

    SELECT isnull(sum(BANKCHEQUESBODY.AMOUNT), sum(BANKCHEQUES.AMOUNT)) AS DEBIT, 0 AS CREDIT FROM BANKCHEQUES INNER JOIN BANKCHEQUESBODY ON BANKCHEQUES.VOUCHERNO=BANKCHEQUESBODY.VOUCHERNO AND BANKCHEQUES.VOUCHERDATE=BANKCHEQUESBODY.VOUCHERDATE  WHERE (BANKCHEQUES.VOUCHERDATE < @FromDate) AND BANKCHEQUES.ISLOST=0 AND isnull(BANKCHEQUESBODY.ACCOUNTNO,BANKCHEQUES.ACCOUNTNO)='111'





UNION ALL

     SELECT case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0) else 0 end AS CREDIT

        FROM BANKCHEQUES BC WHERE BC.ISLOST=0 AND BC.ACCOUNTNO='111' AND (BC.VOUCHERDATE < @FromDate)

UNION ALL

     SELECT case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS CREDIT

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND BCB.ACCOUNTNO='111' AND (BC.VOUCHERDATE < @FromDate)



    UNION ALL

    SELECT 0 AS DEBIT, SUM(AMOUNT) AS CREDIT FROM BANKDEPOSIT WHERE (VOUCHERDATE < @FROMDATE) AND ISBounced=0 AND isnull(ACCOUNTNO,'111')='111'

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from Sales Where Saledate < @FromDate

    UNION ALL

    Select PaidAmount as Debit,0 as Credit from CropsSaleHeader Where Saledate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From Purchase Where Purchasedate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From CropsPurchaseHeader Where Purchasedate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from PurReturns Where PurReturndate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From salereturns Where SaleReturnDate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From PurchasesPest Where EntryDate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from PurReturnsPest Where ReturnDate < @FromDate

    UNION ALL

    Select ReceivedAmount as Debit,0 as Credit from SalesPest Where Saledate < @FromDate

    UNION ALL

    Select 0 as Debit, PaidAmount as Credit From SaleReturnsPest Where ReturnDate < @FromDate

/* Changing By fahad  

----------------Getting Amount Calc... From Contracts AND Contract Invoices---------------------

    UNION ALL

    Select Advance as Debit,0 as Credit from Contract Where CDate < @FromDate AND IsSaleContract=1

    UNION ALL

    Select 0 as Debit ,Advance as Credit from Contract Where CDate < @FromDate AND IsSaleContract=0 ***/

    UNION ALL

    Select Received as Debit,0 as Credit from ContractInvoice Where CDate < @FromDate AND IsSaleInvoice=1

    UNION ALL

    Select 0 as Debit,Received  as Credit from ContractInvoice Where CDate < @FromDate AND IsSaleInvoice=0

) OpData

    Where ChartofAccounts.AccountNo = '111'

    Group by OpeningDebit,OpeningCredit

UNION ALL

    Select AllData.*,AccountName from ChartOfAccounts RIGHT OUTER JOIN

    (

    -- STEP 1: Obtaining the Entries From DebitVouchers

    Select 'DV' as VoucherType,DebitVouchers.VoucherNo,

        'DV-' + case when len(cast(debitvouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + debitvouchers.voucherno ,@ProceedingZeros) 

            else cast(debitvouchers.voucherno as varchar) end as StrVoucherNo,DebitVouchers.VoucherDate,

        0 as Debit,Debit as Credit,narration,AccountNo

        from debitvouchers Inner Join DebitVouchersBody on 



        DebitVouchers.VoucherNo = DebitVouchersBody.VoucherNo AND debitVouchers.VoucherDate = DebitVouchersBody.VoucherDate

        where DebitVouchers.voucherdate between @fromdate and @todate



UNION ALL

    -- STEP 2: Obtaining the Entries From CreditVouchers



    Select 'CV' as VoucherType,CreditVouchers.VoucherNo,

        'CV-' + case when len(cast(Creditvouchers.voucherno as varchar)) < @ProceedingZeros then 



            right(replicate('0',@ProceedingZeros) + Creditvouchers.voucherno ,@ProceedingZeros) 

            else cast(Creditvouchers.voucherno as varchar) end as StrVoucherNo,CreditVouchers.VoucherDate,

        Credit as Debit,0 as Credit,narration,AccountNo

        from Creditvouchers Inner Join CreditVouchersBody on 

        CreditVouchers.VoucherNo = CreditVouchersBody.VoucherNo AND CreditVouchers.VoucherDate = CreditVouchersBody.VoucherDate

        where CreditVouchers.voucherdate between @fromdate and @todate

UNION ALL

    -- STEP 3: Obtaining the Entries From JournalVouchers

    Select 'JV' as VoucherType,JournalVouchers.VoucherNo,

        'JV-' + case when len(cast(JournalVouchers.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + JournalVouchers.voucherno ,@ProceedingZeros) 

            else cast(JournalVouchers.voucherno as varchar) end as StrVoucherNo,JournalVouchers.VoucherDate,

        Debit as Debit,Credit as Credit,narration,AccountNo

        from JournalVouchers Inner Join JournalVouchersBody on 

        JournalVouchers.VoucherNo = JournalVouchersBody.VoucherNo AND JournalVouchers.VoucherDate = JournalVouchersBody.VoucherDate

        where JournalVouchers.voucherdate between @fromdate and @todate and JournalVouchersBody.AccountNo='111'

UNION ALL

    -- STEP 4: Obtaining the Entries From BANKCHEQUES

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        ISNULL(BCB.AMOUNT, BC.AMOUNT) AS DEBIT, 0 AS CREDIT, narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND isnull(BCB.ACCOUNTNO,BC.ACCOUNTNO)='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 

    -- WHT AMOUNT ON CASH

UNION ALL

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0) else 0 end AS CREDIT, 'WITH HOLDING TAX AMOUNT' as narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC WHERE BC.ISLOST=0 AND BC.ACCOUNTNO='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 

UNION ALL

     SELECT 'BC' AS VOUCHERTYPE, BC.VOUCHERNO, 

        'BC-' + case when len(cast(BC.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BC.voucherno ,@ProceedingZeros) 

            else cast(BC.voucherno as varchar) end as StrVoucherNo,BC.VoucherDate,

        case when isnull(PMFlag,'+')='+' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS DEBIT, case when isnull(PMFlag,'+')='-' then IsNull(BC.WHTax ,0)/(Select case when Count(BB.AccountNo)=0 then 1 else Count(BB.AccountNo) end From BankChequesBody BB Where BB.VoucherNo=BC.VOUCHERNO AND BB.VoucherDate=BC.VOUCHERDATE) else 0 end AS CREDIT, 'WITH HOLDING TAX AMOUNT' as narration, ISNULL(BC.BankAccountNo,'111') as AccountNo 

        FROM BANKCHEQUES BC INNER JOIN BANKCHEQUESBODY BCB ON BC.VOUCHERNO=BCB.VOUCHERNO AND BC.VOUCHERDATE=BCB.VOUCHERDATE WHERE BC.ISLOST=0 AND BCB.ACCOUNTNO='111' AND (@FROMDATE IS NULL OR BC.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BC.VoucherDate <= @ToDate) 



UNION ALL



    -- STEP 4: Obtaining the Entries From BANKDEPOSIT



     SELECT 'BD' AS VOUCHERTYPE, BD.VOUCHERNO, 



        'BD-' + case when len(cast(BD.voucherno as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + BD.voucherno ,@ProceedingZeros) 



            else cast(BD.voucherno as varchar) end as StrVoucherNo,BD.VoucherDate,

        0 AS DEBIT, BD.AMOUNT AS CREDIT,  narration, ISNULL(BD.AccountNo,'111') as AccountNo 

        FROM BANKDEPOSIT BD 

        WHERE BD.ISBounced=0 AND ISNULL(BD.AccountNo,'111')='111' AND (@FROMDATE IS NULL OR BD.VoucherDate >= @FromDate) AND (@TODATE IS NULL OR BD.VoucherDate <= @ToDate) 



UNION ALL

    -- STEP 5: Obtaining the Entries from Sales

    Select 'SI' as VoucherType,Sales.Saleid as VoucherNo,

        'SI-' + case when len(cast(Sales.SaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.SaleID ,@ProceedingZeros) 

            else cast(Sales.SaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        ReceivedAmount as Debit,0 as Credit,isnull(Sales.Narration,'Sale Invoice') as Narration,CustomerID

        from Sales where Saledate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 6: Obtaining the Entries from Purchases

    Select 'PI' as VoucherType,Purchase.PurchaseID as VoucherNo,

        'PI-' + case when len(cast(Purchase.PurchaseId as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.PurchaseID ,@ProceedingZeros) 

            else cast(Purchase.PurchaseID as varchar) end as StrVoucherNo,Purchase.PurchaseDate,

        0 as debit, PaidAmount as Credit,isnull(Purchase.Narration, 'Purchase Invoice') as Narration,VendorID

        from Purchase where PurchaseDate between @fromdate and @todate AND PaidAmount>0    

UNION ALL

    -- STEP 7: Obtaining the Entries from PurchaseReturns

    Select 'PR' as VoucherType,PurReturns.PurReturnID as VoucherNo,

        'PR-' + case when len(cast(PurReturns.PurReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + PurReturns.PurReturnID ,@ProceedingZeros) 

            else cast(PurReturns.PurReturnID as varchar) end as StrVoucherNo,PurReturns.PurReturnDate,

         ReceivedAmount as Debit,0 as credit, isnull(PurReturns.Narration, 'Purchase Return Invoice') as Narration,VendorID

        from PurReturns where PurReturnDate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 8: Obtaining the Entries from SaleReturns

    Select 'SR' as VoucherType,SaleReturns.SaleReturnID as VoucherNo,

        'SR-' + case when len(cast(SaleReturns.SaleReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + SaleReturns.SaleReturnID ,@ProceedingZeros) 

            else cast(SaleReturns.SaleReturnID as varchar) end as StrVoucherNo,SaleReturns.SaleReturndate,

        0 as debit, PaidAmount as Credit, isnull(SaleReturns.Narration, 'Sale Return Invoice') as Narration,CustomerID

        from SaleReturns where SaleReturnDate between @fromdate and @todate AND PaidAmount>0









UNION ALL

    -- STEP 5: Obtaining the Entries from Sales

    Select 'PSI' as VoucherType,Sales.Saleid as VoucherNo,

        'PSI-' + case when len(cast(Sales.SaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Sales.SaleID ,@ProceedingZeros) 

            else cast(Sales.SaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        ReceivedAmount as Debit,0 as Credit,isnull(Sales.Remarks,'Pestro Sale Invoice') as Narration,CustomerID

        from SalesPest Sales where Saledate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 6: Obtaining the Entries from Purchases

    Select 'PPI' as VoucherType,Purchase.PurchaseID as VoucherNo,

        'PPI-' + case when len(cast(Purchase.PurchaseId as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.PurchaseID ,@ProceedingZeros) 

            else cast(Purchase.PurchaseID as varchar) end as StrVoucherNo,Purchase.EntryDate,

        0 as debit, PaidAmount as Credit,'Pestro Purchase Invoice' as Narration,VendorID



        from PurchasesPest Purchase where EntryDate between @fromdate and @todate AND PaidAmount>0    

UNION ALL



    -- STEP 7: Obtaining the Entries from PurchaseReturns



    Select 'PPR' as VoucherType,PurReturns.PReturnID as VoucherNo,

        'PPR-' + case when len(cast(PurReturns.PReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + PurReturns.PReturnID ,@ProceedingZeros) 

            else cast(PurReturns.PReturnID as varchar) end as StrVoucherNo,PurReturns.ReturnDate,

         ReceivedAmount as Debit,0 as credit, 'Pestro Purchase Return Invoice' as Narration,VendorID

        from PurReturnsPest PurReturns where ReturnDate between @fromdate and @todate AND ReceivedAmount>0

UNION ALL

    -- STEP 8: Obtaining the Entries from SaleReturns

    Select 'PSR' as VoucherType,SaleReturns.SReturnID as VoucherNo,

        'PSR-' + case when len(cast(SaleReturns.SReturnID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + SaleReturns.SReturnID ,@ProceedingZeros) 

            else cast(SaleReturns.SReturnID as varchar) end as StrVoucherNo,SaleReturns.ReturnDate,

        0 as debit, PaidAmount as Credit, 'Pestro Sale Return Invoice' as Narration,CustomerID

        from SaleReturnsPest SaleReturns where ReturnDate between @fromdate and @todate AND PaidAmount>0

/*--------Brokerage Project Calc here-----------*/

/* Changing By fahad  

    -- STEP 9: Obtaining the Entries From Contract Sale

UNION ALL

    Select 'CS' as VoucherType, Contract.ContractID as VoucherNo,

        'CS-' + case when len(cast(Contract.ContractID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Contract.ContractID ,@ProceedingZeros) 

            else cast(Contract.ContractID as varchar) end as StrVoucherNo,Contract.CDate,

        Advance as Debit,0 as Credit,'Contract Sale' as Narration, PartyID

        from Contract where CDate between @fromdate and @todate AND IsSaleContract=1

UNION ALL

    -- STEP 10: Obtaining the Entries From Contract Purchase

    Select 'CP' as VoucherType, Contract.ContractID as VoucherNo,

        'CP-' + case when len(cast(Contract.ContractID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Contract.ContractID ,@ProceedingZeros) 

            else cast(Contract.ContractID as varchar) end as StrVoucherNo,Contract.CDate,

        0 as Debit,Advance as Credit,'Contract Purchase' as Narration, PartyID

        from Contract where CDate between @fromdate and @todate AND IsSaleContract=0  */

UNION ALL

    -- STEP 11: Obtaining the Entries From Contract Invoice Sale

    Select 'CIS' as VoucherType, ContractInvoice.SaleID as VoucherNo,

        'CIS-' + case when len(cast(ContractInvoice.SaleID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + ContractInvoice.SaleID ,@ProceedingZeros) 

            else cast(ContractInvoice.SaleID as varchar) end as StrVoucherNo, ContractInvoice.CDate,

        Received as Debit,0 as Credit,'Contract Invoice Sale' as Narration, PartyID

        from ContractInvoice where CDate between @fromdate and @todate AND IsSaleInvoice=1 AND Received>0

UNION ALL

    -- STEP 12: Obtaining the Entries From Contract Invoice Purchase

    Select 'CIP' as VoucherType, ContractInvoice.PurchaseID as VoucherNo,

        'CIP-' + case when len(cast(ContractInvoice.PurchaseID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + ContractInvoice.PurchaseID ,@ProceedingZeros) 

            else cast(ContractInvoice.PurchaseID as varchar) end as StrVoucherNo, ContractInvoice.CDate,

        0 as Debit,Received as Credit,'Contract Invoice Purchase' as Narration, PartyID

        from ContractInvoice where CDate between @fromdate and @todate AND IsSaleInvoice=0 AND Received>0

UNION ALL

    -- STEP 13: Obtaining the Entries From Crop Purchase

    Select 'CP' as VoucherType,Purchase.CropPurchaseID as VoucherNo,

        'CP-' + case when len(cast(Purchase.CropPurchaseID as varchar)) < @ProceedingZeros then 

            right(replicate('0',@ProceedingZeros) + Purchase.CropPurchaseID ,@ProceedingZeros) 

            else cast(Purchase.CropPurchaseID as varchar) end as StrVoucherNo,Purchase.PurchaseDate,

        0 as debit, PaidAmount as Credit,isnull(Purchase.Remarks, 'Crop Purchase Invoice') as Narration,VendorID

        from CropsPurchaseHeader Purchase where PurchaseDate between @fromdate and @todate AND PaidAmount>0



UNION ALL

    -- STEP 14: Obtaining the Entries From Crop Sales

    Select 'CS' as VoucherType,Sales.CropSaleID as VoucherNo,

        'CS-' + case when len(cast(Sales.CropSaleID as varchar)) < @ProceedingZeros then 



            right(replicate('0',@ProceedingZeros) + Sales.CropSaleID ,@ProceedingZeros) 

            else cast(Sales.CropSaleID as varchar) end as StrVoucherNo,Sales.SaleDate,

        PaidAmount as Debit,0 as Credit,isnull(Sales.Remarks,'Crop Sale Invoice') as Narration,CustomerID

        from CropsSaleHeader Sales where Saledate between @fromdate and @todate AND PaidAmount>0





    ) ALLDATA on 

    ChartOfAccounts.AccountNo = AllData.AccountNo

Order by VoucherDate

GO

-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SPGetNarrationExps]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SPGetNarrationExps]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SPGetNarrationExps];
GO

CREATE PROCEDURE [dbo].[SPGetNarrationExps]



 AS



/* RETURN COLUMN (AccountNo, OpeningDebit, OpeningCredit, Credit, Debit) */



BEGIN



IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[ZNarration]') AND type in (N'U'))

BEGIN

CREATE TABLE [dbo].ZNarration(

    VoucherID bigint NOT NULL,

    VoucherType varchar(10) NOT NULL,

    Narration [nvarchar](3000) NULL,

    TTLValue decimal(25,2) NULL

)

END











    delete from ZNarration

insert into ZNarration (VoucherID, VoucherType, TTLValue)

SELECT ContractPurId,'BMIS' VoucherType, SUM(TTLExps)  FROM (

SELECT     ContractInvoiceBody_1.ContractPurId, 'BMIS' VoucherType, 

                                                                                 (MAX(ROUND(EXPS.Value,0)) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusSale(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 





                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                                                INNER JOIN (SELECT        (ISNULL(CASE WHEN (C.IsPay = 0) THEN (C.CommissionValue) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddArhat, 0) = 1) THEN IsNull(SUM(CB.TotalValue) * C.PurArhat / 100, 0) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddChangrai, 0) = 1) THEN IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100, 0) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddMazdori, 0) = 1) THEN (CASE WHEN ISNULL(C.MazdoriPercent, 0) 

                         = 1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN C.MazdoriPerbag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.PurMazdori ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurMazdori END) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddDalali, 0) = 1) THEN SUM(CB.TotalValue) * C.PurDalali / 100 END, 0) + ISNULL(CASE WHEN (Isnull(C.AddPuliGhya, 0) = 1) THEN (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurPulliGhya END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddCommission, 0) = 1) THEN IsNull(SUM(CB.TotalValue) * C.PurComm / 100, 0) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddManshyana, 0) = 1) THEN SUM(CB.TruckQty) * C.SaleManshyana END, 

                         0) + ISNULL(CASE WHEN (Isnull(C.AddLoading, 0) = 1) THEN (CASE WHEN C.LoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.PurLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurLoading END) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddUnLoading, 0) = 1) THEN (CASE WHEN C.UnLoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.PurUnLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurUnLoading END) 

                         END, 0) + ISNULL(CASE WHEN (Isnull(C.AddVehicleCharges, 0) = 1) THEN (C.VehicalCharges) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddBankCharges, 0) = 1) THEN C.BankChargesAmount END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddTelePhoneExp, 0) = 1) THEN SUM(C.AddTelePhoneExp) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddInsuranceExp, 0) = 1) THEN SUM(C.AddInsuranceExp) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddFuelExp, 0) = 1) THEN SUM(C.AddFuelExp) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddStitchingExp, 0) = 1) THEN SUM(C.AddStitchingExp) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddMarketFees, 0) = 1) THEN SUM(CB.TotalKgs) / 100 * C.SaleMarketFees END, 0) + ISNULL(C.OtherExpencePlus, 0) + ISNULL(CASE WHEN (Isnull(C.AddMazdori, 0) = 1) 

                         THEN (CASE WHEN ISNULL(C.MazdoriPercent, 0) = 1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100 WHEN C.MazdoriPerbag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.SaleMazdori ELSE (SUM(CB.TotalKgs) 

                         / CB.MoundStd) * C.SaleMazdori END) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddDalali, 0) = 1) THEN SUM(CB.TotalValue) * C.SaleDalali / 100 END, 0) + ISNULL(CASE WHEN (Isnull(C.AddSootli, 0) = 1) THEN (SUM(CB.BagsQty) 

                         + SUM(CB.PQty)) * C.SaleSootli END, 0) + ISNULL(CASE WHEN (Isnull(C.AddCommission, 0) = 1) THEN IsNull(SUM(CB.TotalValue) * C.SaleComm / 100, 0) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddLoading, 0) = 1) 

                         THEN (CASE WHEN C.LoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.SaleLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.SaleLoading END) END, 0) + ISNULL(CASE WHEN (Isnull(C.AddUnLoading, 0) 

                         = 1) THEN (CASE WHEN C.UnLoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.SaleUnLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.SaleUnLoading END) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.AddWHTax, 0) = 1) THEN (CASE WHEN Isnull(C.WHTinKg, 0) = 1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax ELSE SUM(CB.TotalValue) * C.WHTax / 100 END) END, 0)) - (ISNULL(C.Discount, 0) 

                         + ISNULL(CASE WHEN (C.IsPay = 1) THEN C.CommissionValue END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusArhat, 0) = 1) THEN IsNull(SUM(CB.TotalValue) * C.PurArhat / 100, 0) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.MinusChangrai, 0) = 1) THEN IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100, 0) END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusMazdori, 0) = 1) THEN (CASE WHEN ISNULL(C.MazdoriPercent, 0) 

                         = 1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN ISNULL(C.MazdoriPercent, 0) = 1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN C.MazdoriPerbag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) 

                         * C.PurMazdori ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurMazdori END) END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusDalali, 0) = 1) THEN SUM(CB.TotalValue) * C.PurDalali / 100 END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.MinusPuliGhya, 0) = 1) THEN (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurPulliGhya END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusCommission, 0) = 1) THEN IsNull(SUM(CB.TotalValue) 

                         * C.PurComm / 100, 0) END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusManshyana, 0) = 1) THEN SUM(CB.TruckQty) * C.SaleManshyana END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusLoading, 0) = 1) 

                         THEN (CASE WHEN C.LoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.PurLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurLoading END) END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusUnLoading, 0) 

                         = 1) THEN (CASE WHEN C.UnLoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.PurUnLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.PurUnLoading END) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.MinusVehicleCharges, 0) = 1) THEN (C.VehicalCharges) END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusMarketFees, 0) = 1) THEN SUM(CB.TotalKgs) / 100 * C.SaleMarketFees END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.MinusBankCharges, 0) = 1) THEN C.BankChargesAmount END, 0) + ISNULL(C.OtherExpenceMinus, 0) + ISNULL(CASE WHEN (Isnull(C.MinusMazdori, 0) = 1) 

                         THEN (CASE WHEN ISNULL(C.MazdoriPercent, 0) = 1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100 WHEN C.MazdoriPerbag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.SaleMazdori ELSE (SUM(CB.TotalKgs) 

                         / CB.MoundStd) * C.SaleMazdori END) END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusDalali, 0) = 1) THEN SUM(CB.TotalValue) * C.SaleDalali / 100 END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusSootli, 0) = 1) 

                         THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.SaleSootli END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusCommission, 0) = 1) THEN IsNull(SUM(CB.TotalValue) * C.SaleComm / 100, 0) END, 0) 

                         + ISNULL(CASE WHEN (Isnull(C.MinusLoading, 0) = 1) THEN (CASE WHEN C.LoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.SaleLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) * C.SaleLoading END) END, 

                         0) + ISNULL(CASE WHEN (Isnull(C.MinusUnLoading, 0) = 1) THEN (CASE WHEN C.UnLoadingPerBag = 1 THEN (SUM(CB.BagsQty) + SUM(CB.PQty)) * C.SaleUnLoading ELSE (SUM(CB.TotalKgs) / CB.MoundStd) 

                         * C.SaleUnLoading END) END, 0) + ISNULL(CASE WHEN (Isnull(C.MinusWHTax, 0) = 1) THEN (CASE WHEN Isnull(C.WHTinKg, 0) = 1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax ELSE SUM(CB.TotalValue) * C.WHTax / 100 END) 

                         END, 0)) AS Value, C.ContractPurId

FROM            ContractInvoice AS C INNER JOIN

                         ContractInvoiceBody AS CB ON C.ContractPurId = CB.ContractPurId

WHERE        (C.IsSaleInvoice = 1)

GROUP BY C.IsSaleInvoice, C.IsPay, C.AddArhat, C.PurArhat, C.AddChangrai, C.PurChangrai, C.MazdoriPerBag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.AddMazdori, C.AddDalali, C.PurDalali, C.AddPuliGhya, C.PurPulliGhya, 

                         C.PurComm, C.AddCommission, C.SaleManshyana, C.AddManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.AddLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.AddUnLoading, C.VehicalCharges, 

                         C.AddVehicleCharges, C.AddTelePhoneExp, C.AddInsuranceExp, C.AddFuelExp, C.AddStitchingExp, C.SaleMarketFees, C.AddMarketFees, C.OtherExpencePlus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.AddSootli, 

                         C.SaleComm, C.SaleLoading, C.SaleUnLoading, C.WHTax, C.AddWHTax, C.CommissionValue, C.WHTinKg, C.MazdoriPercent, C.AddBankCharges, C.BankChargesAmount, C.MinusArhat, C.PurArhat, C.MinusChangrai, 

                         C.PurChangrai, C.MazdoriPerBag, C.PurMazdori, C.PurMazdori, C.MinusMazdori, C.MinusDalali, C.PurDalali, C.MinusPuliGhya, C.PurPulliGhya, C.PurComm, C.MinusCommission, C.SaleManshyana, C.MinusManshyana, 

                         C.PurLoading, C.LoadingPerBag, C.PurLoading, C.MinusLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.MinusUnLoading, C.VehicalCharges, C.MinusVehicleCharges, C.SaleMarketFees, C.MinusMarketFees, 

                         C.OtherExpenceMinus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.MinusSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading, C.WHTax, C.MinusWHTax, C.CommissionValue, C.WHTinKg, C.MazdoriPercent, 

                         C.MinusBankCharges, C.BankChargesAmount, C.Discount, C.ContractPurId) EXPS ON EXPS.ContractPurId = ContractInvoice_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 1) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo

)BISS

GROUP BY ContractPurId



insert into ZNarration (VoucherID, VoucherType, TTLValue)

SELECT ContractPurId,'BMIP' VoucherType, SUM(TTLExps)  FROM (

SELECT     ContractInvoiceBody_1.ContractPurId, 'BMIP' VoucherType, 

                                                                                (ROUND(MAX(EXPP.Value),0) /

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_4

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                    (SELECT     COUNT(*) AS Expr1

                                                                                      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo))) 

                                                                                --+ dbo.FunGetAllExpPlusPur(ContractInvoiceBody_1.ContractPurId) /

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_3

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId)) *

                                                                                --    (SELECT     COUNT(*) AS Expr1

                                                                                --      FROM          ContractInvoiceBody AS ContractInvoiceBody_2

                                                                                --      WHERE      (ContractPurId = ContractInvoiceBody_1.ContractPurId) AND (VehicleNo = ContractInvoiceBody_1.VehicleNo)) 

                                                                                AS TTLExps

                                                         FROM          ContractInvoiceBody AS ContractInvoiceBody_1 INNER JOIN

                                                                                ContractInvoice AS ContractInvoice_1 ON ContractInvoiceBody_1.ContractPurId = ContractInvoice_1.ContractPurId

                                                                                INNER JOIN (Select Value = 

  IsNull(Case When (C.IsPay = 0) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.AddArhat,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then (

CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100 WHEN C.MazdoriPerbag = 1 then (case when CB.WeightPerBag <> 0 then (Sum(CB.TotalKgs)/CB.WeightPerBag) * C.PurMazdori 

else

(SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori  end) Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddVehicleCharges,0) = 1) Then C.VehicalCharges End,0)        

+ IsNull(Case When (Isnull(C.AddMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.AddBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpencePlus,0)

--Sales

+ IsNull(Case When (Isnull(C.AddMazdori,0) = 1) Then 

(CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.AddDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.AddSootli,0) = 1) Then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.AddCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.AddLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.AddUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case  When (Isnull(C.AddWHTax,0) = 1) Then (Case WHEN Isnull(C.WHTinKg,0)=1 THEN SUM(CB.TotalKgs) / 100 * C.WHTax Else SUM(CB.TotalValue) * C.WHTax / 100 END)  End,0)

+ (CASE WHEN ISNULL(C.ChartsCharges,0) > 0 THEN  ABS(C.ChartsCharges) ELSE 0 END)

--MINUSES

-(IsNull(C.Discount,0)

+  IsNull(Case When (C.IsPay = 1) Then (C.CommissionValue) End,0)

+ IsNull(Case When (Isnull(C.MinusArhat,0) = 1) Then IsNull((SUM(CB.TotalValue))  * C.PurArhat / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusChangrai,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurChangrai / 100,0) End,0)

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then 

(CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.PurMazdori / 100

 WHEN C.MazdoriPerbag = 1 then (case when CB.WeightPerBag <> 0 then (Sum(CB.TotalKgs)/CB.WeightPerBag) * C.PurMazdori 

else

(SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurMazdori  end) Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.PurDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusPuliGhya,0) = 1) Then (SUM(CB.TotalKgs)/CB.MoundStd) * C.PurPulliGhya  End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.PurComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusManshyana,0) = 1) Then SUM(CB.TruckQty) * C.SaleManshyana End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+ SUM(CB.PQty)) * C.PurLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.PurUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.PurUnLoading  End) END,0)

 + IsNull(Case When (Isnull(C.MinusVehicleCharges,0) = 1) Then (C.VehicalCharges) End,0)        

+ IsNull(Case When (Isnull(C.MinusMarketFees,0) = 1) Then SUM(CB.TotalKgs) / 100 * C.SaleMarketFees End,0)

+ IsNull(Case When (Isnull(C.MinusBankCharges,0) = 1) Then  C.BankChargesAmount End,0)

+ IsNull(C.OtherExpenceMinus,0)

--Sales

+ IsNull(Case When (Isnull(C.MinusMazdori,0) = 1) Then

 (CASE WHEN ISNULL(C.MazdoriPercent,0)=1 THEN SUM(CB.TotalValue) * C.SaleMazdori / 100

WHEN C.MazdoriPerbag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleMazdori Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleMazdori  End) END,0)

+ IsNull(Case When (Isnull(C.MinusDalali,0) = 1) Then SUM(CB.TotalValue) * C.SaleDalali / 100 End,0)

+ IsNull(Case When (Isnull(C.MinusSootli,0) = 1) Then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleSootli End,0)

+ IsNull(Case When (Isnull(C.MinusCommission,0) = 1) Then IsNull(SUM(CB.TotalValue) * C.SaleComm / 100,0)  End,0)

+ IsNull(Case When (Isnull(C.MinusLoading,0) = 1) Then (CASE WHEN C.LoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleLoading  End) END,0)

+ IsNull(Case When (Isnull(C.MinusUnLoading,0) = 1) Then (CASE WHEN C.UnLoadingPerBag = 1 then (SUM(CB.BagsQty)+SUM(CB.PQty)) * C.SaleUnLoading Else 

    (Sum(CB.TotalKgs)/CB.MoundStd) * C.SaleUnLoading  End) END,0)

+ IsNull(

Case When (Isnull(C.MinusWHTax,0) = 1) Then ( C.CommissionValue * C.WHTax / 100 )  End,0)

+ (CASE WHEN ISNULL(C.ChartsCharges,0) < 0 THEN  ABS(C.ChartsCharges) ELSE 0 END)),C.ContractPurId

From ContractInvoice C INNER JOIN ContractInvoiceBody CB ON C.ContractPurId = CB.ContractPurId 

Where  C.IsSaleInvoice = 0

Group By C.ContractPurId,C.IsSaleInvoice, C.IsPay, C.AddArhat, C.PurArhat,C.AddChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.AddMazdori, C.AddDalali, C.PurDalali, C.AddPuliGhya, C.PurPulliGhya, 

C.PurComm, C.AddCommission, C.SaleManshyana, C.AddManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.AddLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.AddUnLoading,

C.VehicalCharges, C.AddVehicleCharges, C.SaleMarketFees, C.AddMarketFees, C.OtherExpencePlus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.AddSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.AddWHTax,C.WHTinKg, C.MazdoriPercent,C.AddBankCharges,C.BankChargesAmount, C.CommissionValue

,CB.WeightPerBag,C.ChartsCharges





, C.MinusArhat, C.PurArhat,C.MinusChangrai, C.PurChangrai, C.MazdoriPerbag, C.PurMazdori, CB.MoundStd, C.PurMazdori, C.MinusMazdori, C.MinusDalali, C.PurDalali, C.MinusPuliGhya, C.PurPulliGhya, 

C.PurComm, C.MinusCommission, C.SaleManshyana, C.MinusManshyana, C.PurLoading, C.LoadingPerBag, C.PurLoading, C.MinusLoading, C.PurUnLoading, C.UnLoadingPerBag, C.PurUnLoading, C.MinusUnLoading,

C.VehicalCharges, C.MinusVehicleCharges, C.SaleMarketFees, C.MinusMarketFees, C.OtherExpenceMinus, C.SaleMazdori, C.SaleDalali, C.SaleSootli, C.MinusSootli, C.SaleComm, C.SaleLoading, C.SaleUnLoading,

C.WHTax, C.MinusWHTax,C.WHTinKg, C.MazdoriPercent,C.MinusBankCharges,C.BankChargesAmount, C.CommissionValue, C.Discount

,CB.WeightPerBag, C.ChartsCharges) EXPP ON EXPP.ContractPurId = ContractInvoiceBody_1.ContractPurId

                                                         WHERE      (ContractInvoice_1.IsSaleInvoice = 0) AND (ISNULL(ContractInvoice_1.IsMultiNew, 0) = 1)

                                                         GROUP BY ContractInvoiceBody_1.ContractPurId, ContractInvoiceBody_1.VehicleNo



)BIPP

GROUP BY ContractPurId



END

GO

-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SPRptProductLedgerBrk]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SPRptProductLedgerBrk]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SPRptProductLedgerBrk];
GO

CREATE PROCEDURE [dbo].SPRptProductLedgerBrk (@FromDate smalldatetime, @ToDate smalldatetime , @ProductID varchar(10))



 AS



BEGIN



IF EXISTS(SELECT 1 FROM tempdb..sysobjects WHERE NAME LIKE '#VuProductLedgerBrk%' And type = 'U') Drop table #VuProductLedgerBrk

IF EXISTS(SELECT 1 FROM tempdb..sysobjects WHERE NAME LIKE '#VUProductCost%' And type = 'U') Drop table #VUProductCost





SELECT * INTO #VUProductCost

FROM (

SELECT   SUM(Value) Value, SUM(Qty) Qty, CASE WHEN SUM(qty) > 0 THEN SUM(Value) / SUM(Qty) ELSE 0 END AS Cost, ProductID, CDate

FROM         (SELECT     '' AS CDate, ProductID, SUM(TTLValue + TTLSalesTax) AS Value, SUM(Qty * Multiplier) AS Qty

                       FROM          OpeningStock

                       GROUP BY ProductID

                       UNION ALL

                       SELECT     ContractInvoice.CDate, ContractInvoiceBody.ProductID, SUM(ContractInvoiceBody.TotalValue) AS TotalValue, 

                                             SUM(ContractInvoiceBody.TotalKgs) AS TotalKgs

                       FROM         ContractInvoice INNER JOIN

                                             ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                       WHERE     (ContractInvoice.IsSaleInvoice = 0) AND (ContractInvoice.MultiEntryID IS NULL)

                       GROUP BY ContractInvoiceBody.ProductID, ContractInvoice.CDate) AS TTP

WHERE        (ProductID = @ProductID)                           

                   GROUP BY TTP.ProductID, TTP.CDate

                   )CCPP

                   

SELECT * INTO #VuProductLedgerBrk

 FROM (

SELECT        InvType, InvNo, ContractPurId, InvDate, ProID, TotalKgs, TotalValue, IsIN,

                             CAST((SELECT 

                         --AVG(COST),

                         CASE WHEN SUM(qty) > 0 THEN SUM(Value) / SUM(Qty) ELSE AVG(COST) END

                          FROM #VUProductCost WHERE  ProductID = TTP.ProID AND CDate <= TTP.InvDate

                          ) 

                         AS decimal(36,6))AS Cost, 0.00 Cost2

FROM            (SELECT        'Opening' AS InvType, 0 AS InvNo, 0 AS ContractPurId, '1/1/2000' AS InvDate, ProductID ProID, SUM(Qty * Multiplier) AS TotalKgs, SUM(TTLValue + TTLSalesTax) AS TotalValue, 1 AS IsIN

                          FROM            OpeningStock

                          GROUP BY ProductID

                          UNION ALL

                          SELECT        'Purchase' AS InvType, ContractInvoice.PurchaseID AS InvNo, ContractInvoice.ContractPurId, ContractInvoice.CDate AS InvDate, ContractInvoiceBody.ProductID , ContractInvoiceBody.TotalKgs, 

                                                   ContractInvoiceBody.TotalValue, 1 AS IsIN

                          FROM            ContractInvoice INNER JOIN

                                                   ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                          WHERE        (ContractInvoice.IsSaleInvoice = 0) AND ContractInvoice.MultiEntryID IS NULL

                          UNION ALL

                          SELECT        'Sale' AS InvType, ContractInvoice.SaleID, ContractInvoice.ContractPurId, ContractInvoice.CDate, ContractInvoiceBody.ProductID, ContractInvoiceBody.TotalKgs, ContractInvoiceBody.TotalValue, 0 AS IsIN

                          FROM            ContractInvoice INNER JOIN

                                                   ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId

                          WHERE        (ContractInvoice.IsSaleInvoice = 1) AND ContractInvoice.MultiEntryID IS NULL) AS TTP

WHERE        (ProID = @ProductID)               

)VUL

















SELECT     TT.InvType, TT.InvNo, TT.ContractPurId, TT.InvDate, TT.ProductID, TT.TTLKGDr, TT.TTLKGCr, TT.TTLValDr, 

TT.TTLKGCr * TT.Cost TTLValCrOnCost, 

TTLValCr, TT.Cost, 

                      Products.ProductName, CAST(Cost2 AS decimal(36,5)) Cost2

 --, sum(TTLKGDr-TTLKGCr) over ( order by InvDate,ContractPurId) TTLQtyBal

--, sum(TTLValDr-(TT.TTLKGCr * TT.Cost)) over ( order by InvDate,ContractPurId) TTLValueBal

--,case when (sum(TTLKGDr-TTLKGCr) over ( order by InvDate,ContractPurId)) <> 0 then (sum(TTLValDr-(TT.TTLKGCr * TT.Cost)) over ( order by InvDate,ContractPurId)) /(sum(TTLKGDr-TTLKGCr) over ( order by InvDate,ContractPurId)) else 0 end as Cost 

FROM         (SELECT     InvType, InvNo, ContractPurId, InvDate, ProductID, CASE WHEN (TTLKGDr - TTLKGCr) > 0 THEN (TTLKGDr - TTLKGCr) 

                                              ELSE 0 END AS TTLKGDr, CASE WHEN (TTLKGDr - TTLKGCr) < 0 THEN ABS(TTLKGDr - TTLKGCr) ELSE 0 END AS TTLKGCr, 

                                              CASE WHEN (TTLValDr - TTLValCr) > 0 THEN (TTLValDr - TTLValCr) ELSE 0 END AS TTLValDr, CASE WHEN (TTLValDr - TTLValCr) 

                                              < 0 THEN ABS(TTLValDr - TTLValCr) ELSE 0 END AS TTLValCr, Cost,Cost2

                       FROM          (SELECT     'OpeningStock' AS InvType, 0 AS InvNo, 0 AS ContractPurId, @FromDate AS InvDate, ProID ProductID, 

                                                                      SUM(CASE WHEN IsIN = 1 THEN TotalKgs ELSE 0 END) AS TTLKGDr, SUM(CASE WHEN IsIN = 0 THEN TotalKgs ELSE 0 END) 

                                                                      AS TTLKGCr, SUM(CASE WHEN IsIN = 1 THEN TotalValue ELSE 0 END) AS TTLValDr, 

                                                                      SUM(CASE WHEN IsIN = 0 THEN TotalValue ELSE 0 END) AS TTLValCr, AVG(Cost) AS Cost, AVG(Cost2)Cost2

                                               FROM          #VuProductLedgerBrk

                                               WHERE      (ProID = @ProductID) AND (InvDate < @FromDate)

                                               GROUP BY ProID) AS OP

                       UNION ALL

                       SELECT     InvType, InvNo, ContractPurId, InvDate, ProductID, CASE WHEN (TTLKGDr - TTLKGCr) > 0 THEN (TTLKGDr - TTLKGCr) 

                                             ELSE 0 END AS TTLKGDr, CASE WHEN (TTLKGDr - TTLKGCr) < 0 THEN ABS(TTLKGDr - TTLKGCr) ELSE 0 END AS TTLKGCr, 

                                             CASE WHEN (TTLValDr - TTLValCr) > 0 THEN (TTLValDr - TTLValCr) ELSE 0 END AS TTLValDr, CASE WHEN (TTLValDr - TTLValCr) 

                                             < 0 THEN ABS(TTLValDr - TTLValCr) ELSE 0 END AS TTLValCr, Cost,Cost2

                       FROM         (SELECT     InvType, InvNo, ContractPurId, InvDate, ProID ProductID, (CASE WHEN IsIN = 1 THEN TotalKgs ELSE 0 END) AS TTLKGDr, 

                                                                     (CASE WHEN IsIN = 0 THEN TotalKgs ELSE 0 END) AS TTLKGCr, (CASE WHEN IsIN = 1 THEN TotalValue ELSE 0 END) 

                                                                     AS TTLValDr, (CASE WHEN IsIN = 0 THEN TotalValue ELSE 0 END) AS TTLValCr, Cost,Cost2

                                              FROM          #VuProductLedgerBrk

                                              WHERE      (ProID = @ProductID) AND (InvDate BETWEEN @FromDate AND @ToDate)) AS CURR) AS TT INNER JOIN

                      Products ON TT.ProductID = Products.ProductID

                      ORDER BY InvDate ASC,ContractPurId ASC

                      

END

GO

-- -----------------------------------------------------------------------------
-- Stored Procedure: [dbo].[SpAgenTechNetWeighment]
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'[dbo].[SpAgenTechNetWeighment]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[SpAgenTechNetWeighment];
GO

CREATE PROCEDURE SpAgenTechNetWeighment @CropPurchaseNo Numeric(10), @VendorID Varchar(10)



 As    



Begin



    If @CropPurchaseNo IS NOT NULL AND @VendorID IS NOT NULL



    Begin



        SELECT 



            CROPSWEIGHMENTS.WEIGHMENTID, 



            CROPSWEIGHMENTS.WEIGHMENTDATE, 



            CROPSWEIGHMENTS.VendorID, 



            CROPSWEIGHMENTS.SUPPLYMODE, 



            CROPSWEIGHMENTS.SUPPLYMODEQTY, 



            CROPSPURCHASEBODY.WEIGHT As NetWeight, 



            CROPSPURCHASEBODY.PRICE



        FROM CROPSWEIGHMENTS  LEFT OUTER JOIN



            CROPSPURCHASEBODY ON 



            CROPSWEIGHMENTS.WEIGHMENTID = CROPSPURCHASEBODY.WEIGHMENTID AND CROPSPURCHASEBODY.CROPPurchaseID = @CropPurchaseNo



        Where CROPSWEIGHMENTS.VendorID = @VendorID AND CROPSWEIGHMENTS.IsPosted = 1  AND CROPSWEIGHMENTS.IsFinalized = 1



        GROUP BY 





            CROPSWEIGHMENTS.WEIGHMENTID, 



            CROPSWEIGHMENTS.WEIGHMENTDATE, 



            CROPSWEIGHMENTS.VendorID, 



            CROPSWEIGHMENTS.SUPPLYMODE, 



            CROPSWEIGHMENTS.SUPPLYMODEQTY, 



            CROPSPURCHASEBODY.SerialNo,



            CROPSPURCHASEBODY.WEIGHT,



            CROPSPURCHASEBODY.PRICE,



                    CROPSWEIGHMENTS.GROSSWEIGHT,



                    CROPSWEIGHMENTS.DEDUCTION,



                    CROPSWEIGHMENTS.SHORTAGE



        HAVING (CROPSWEIGHMENTS.GROSSWEIGHT - CROPSWEIGHMENTS.DEDUCTION - CROPSWEIGHMENTS.SHORTAGE - IsNULL((Select Sum(PB1.Weight) FROM CROPSPURCHASEBODY PB1 WHERE PB1.CROPPurchaseID <> @CropPurchaseNo And WEIGHMENTID = CROPSWEIGHMENTS.WEIGHMENTID),0)) > 0



        UNION



        SELECT distinct



            CROPSWEIGHMENTS.WEIGHMENTID, 



            CROPSWEIGHMENTS.WEIGHMENTDATE, 



            CROPSWEIGHMENTS.VendorID, 



            CROPSWEIGHMENTS.SUPPLYMODE, 



            CROPSWEIGHMENTS.SUPPLYMODEQTY, 



            (CROPSWEIGHMENTS.GROSSWEIGHT - CROPSWEIGHMENTS.DEDUCTION - CROPSWEIGHMENTS.SHORTAGE - IsNULL((Select Sum(PB1.Weight) FROM CROPSPURCHASEBODY PB1 WHERE WEIGHMENTID = CROPSWEIGHMENTS.WEIGHMENTID),0))  AS NetWeight, 



            Null As PRICE



        FROM CROPSWEIGHMENTS 



             LEFT OUTER JOIN CROPSPURCHASEBODY ON 



            CROPSWEIGHMENTS.WEIGHMENTID = CROPSPURCHASEBODY.WEIGHMENTID



        Where CROPSWEIGHMENTS.IsPosted = 1 AND CROPSWEIGHMENTS.IsFinalized = 1



        GROUP BY 





            CROPSWEIGHMENTS.WEIGHMENTID, 



            CROPSWEIGHMENTS.WEIGHMENTDATE, 



            CROPSWEIGHMENTS.VendorID, 



            CROPSWEIGHMENTS.SUPPLYMODE, 



            CROPSWEIGHMENTS.SUPPLYMODEQTY, 



            CROPSPURCHASEBODY.SerialNo,



                    CROPSWEIGHMENTS.GROSSWEIGHT,



                    CROPSWEIGHMENTS.DEDUCTION,



                    CROPSWEIGHMENTS.SHORTAGE



        HAVING (CROPSWEIGHMENTS.GROSSWEIGHT - CROPSWEIGHMENTS.DEDUCTION - CROPSWEIGHMENTS.SHORTAGE - IsNULL((Select Sum(PB1.Weight) FROM CROPSPURCHASEBODY PB1 WHERE WEIGHMENTID = CROPSWEIGHMENTS.WEIGHMENTID),0) > 0) 



    END



    /************  All Weighments  ***************************/



    If @CropPurchaseNo IS NULL AND @VendorID IS NULL



    Begin



        SELECT distinct



            CROPSWEIGHMENTS.WEIGHMENTID, 



            CROPSWEIGHMENTS.WEIGHMENTDATE, 



            CROPSWEIGHMENTS.VendorID, 



            CROPSWEIGHMENTS.SUPPLYMODE, 



            CROPSWEIGHMENTS.SUPPLYMODEQTY, 



            (CROPSWEIGHMENTS.GROSSWEIGHT - CROPSWEIGHMENTS.DEDUCTION - CROPSWEIGHMENTS.SHORTAGE - IsNULL((Select Sum(PB1.Weight) FROM CROPSPURCHASEBODY PB1 WHERE WEIGHMENTID = CROPSWEIGHMENTS.WEIGHMENTID),0))  AS NetWeight, 



            Null As PRICE



        FROM CROPWEIGHMENTS INNER JOIN



            SUPPLIERS ON CROPWEIGHMENTS.VendorID = SUPPLIERS.VendorID INNER JOIN 



            CROPSWEIGHMENTS ON 



            CROPWEIGHMENTS.WEIGHMENTID = CROPSWEIGHMENTS.WEIGHMENTID



             LEFT OUTER JOIN



            BAGWEIGHT ON 



            CROPSWEIGHMENTS.WEIGHMENTID = BAGWEIGHT.WEIGHMENTID



             LEFT OUTER JOIN



            SUPPLYMODES ON 



            CROPWEIGHMENTS.SUPPLYMODEID = SUPPLYMODES.SUPPLYMODEID



             LEFT OUTER JOIN



            BAGTYPES ON 



            BAGWEIGHT.BAGTYPEID = BAGTYPES.BAGTYPEID 



             LEFT OUTER JOIN CROPSPURCHASEBODY ON 



            CROPWEIGHMENTS.WEIGHMENTID = CROPSPURCHASEBODY.WEIGHMENTID



        Where CROPSWEIGHMENTS.IsPosted = 1 AND CROPSWEIGHMENTS.IsFinalized = 1



        GROUP BY 



            CROPSWEIGHMENTS.WEIGHMENTID, 



            CROPSWEIGHMENTS.WEIGHMENTDATE, 



            CROPSWEIGHMENTS.VendorID, 



            CROPSWEIGHMENTS.SUPPLYMODE, 



            CROPSWEIGHMENTS.SUPPLYMODEQTY, 



            CROPSPURCHASEBODY.SerialNo,



                    CROPSWEIGHMENTS.GROSSWEIGHT,



                    CROPSWEIGHMENTS.DEDUCTION,



                    CROPSWEIGHMENTS.SHORTAGE



        HAVING (CROPSWEIGHMENTS.GROSSWEIGHT - CROPSWEIGHMENTS.DEDUCTION - CROPSWEIGHMENTS.SHORTAGE - IsNULL((Select Sum(PB1.Weight) FROM CROPSPURCHASEBODY PB1 WHERE WEIGHMENTID = CROPSWEIGHMENTS.WEIGHMENTID),0) > 0 )



    END



End

GO
