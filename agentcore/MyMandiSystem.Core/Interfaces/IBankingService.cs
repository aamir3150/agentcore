using MyMandiSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IBankingService
{
    Task<IEnumerable<BankAccount>> GetAllBankAccountsAsync();
    Task<BankAccount?> GetBankAccountByIdAsync(int id);
    Task AddBankAccountAsync(BankAccount bankAccount);
    Task UpdateBankAccountAsync(BankAccount bankAccount);
    Task<BankAccount> CreateBankAccountWithGlAccountAsync(string bankName, string branchName, string accountNumber, decimal openingBalance, string? glAccountNo = null);

    Task<IEnumerable<ChequeBook>> GetChequeBooksAsync(int bankAccountId);
    Task<ChequeBook> GenerateChequeBookAsync(int bankAccountId, int seriesStart, int seriesEnd);

    Task<IEnumerable<Cheque>> GetAvailableChequesAsync(int bankAccountId);
    Task<IEnumerable<Cheque>> GetUnclearedChequesAsync(int? bankAccountId = null);
    Task<IEnumerable<Cheque>> GetChequesByStatusAsync(ChequeStatus status, int? bankAccountId = null);
    Task<Cheque?> GetChequeByNoAsync(string chequeNo, int? bankAccountId = null);

    Task IssueChequeAsync(int chequeId, int partyAccountId, decimal amount, DateTime issueDate, string? narration, string? receivedBy, int financialYearId, int? cropSeasonId);
    Task ReconcileChequeAsync(int chequeId, DateTime clearanceDate);
    Task BounceChequeAsync(int chequeId, string reason);
    Task CancelChequeAsync(int chequeId, string reason);

    Task<BankDeposit> DepositCashToBankAsync(int bankAccountId, decimal amount, DateTime date, string? slipNo, string? narration, int financialYearId, int? cropSeasonId);
    Task<BankDeposit> DepositChequeToBankAsync(int bankAccountId, int partyAccountId, string chequeNo, decimal amount, DateTime chequeDate, DateTime depositDate, string? slipNo, string? narration, int financialYearId, int? cropSeasonId);
    Task ReconcileDepositAsync(int depositId, DateTime reconcileDate);
    Task<IEnumerable<BankDeposit>> GetUnclearedDepositsAsync(int? bankAccountId = null);

    // Cheque Book Generation (AgenSoft frmBankChequeGenerate & BankChequeBookHistory Exact Parity)
    Task<int> GenerateChequeBookLeavesAsync(string bankAccountNo, string fromChequeNo, string toChequeNo);
    Task<IEnumerable<ChequeHistoryDto>> GetChequeBookHistoryForBankAsync(string bankAccountNo);

    // Bank Cheques Editing & History (AgenSoft Exact Parity)
    Task<IEnumerable<ChequeHistoryDto>> GetChequeBookHistoryAsync(string? bankAccountNo = null, DateTime? entryDate = null);
    Task CancelChequeInHistoryAsync(string accountNo, string chequeNo, string remarks);
    Task UpdateChequeCancellationAsync(string accountNo, string chequeNo, bool isCancelled, DateTime? cancelDate, string? remarks);
    Task DeleteChequeFromHistoryAsync(string accountNo, string chequeNo);

    // Bank Cheques Reconciliation (AgenSoft frmChequesReconcilation Parity)
    Task<IEnumerable<BankChequeReconciliationItemDto>> GetChequesForReconciliationAsync(string bankAccountNo, bool? isReconciled = null);
    Task<(decimal ReconciledBalance, decimal UnreconciledBalance)> GetBankChequesReconciliationBalancesAsync(string bankAccountNo);
    Task SaveChequeReconciliationAsync(long voucherNo, string bankAccountNo, DateTime? reconcileDate);
    Task BatchSaveChequeReconciliationAsync(IEnumerable<(long voucherNo, string bankAccountNo, DateTime? reconcileDate)> updates);

    // Deposit Reconciliation (AgenSoft frmDepositReconcilation Parity)
    Task<IEnumerable<BankDepositReconciliationItemDto>> GetDepositsForReconciliationAsync(string bankAccountNo, bool? isReconciled = null);
    Task<(decimal ReconciledBalance, decimal UnreconciledBalance)> GetBankDepositsReconciliationBalancesAsync(string bankAccountNo);
    Task SaveDepositReconciliationAsync(long voucherNo, string bankAccountNo, DateTime? reconcileDate);
    Task BatchSaveDepositReconciliationAsync(IEnumerable<(long voucherNo, string bankAccountNo, DateTime? reconcileDate)> updates);

    // Bank Cheque Deposit (AgenSoft frmChequeDeposit & BankDeposit Exact Parity)
    Task<long> GetNextChequeDepositVoucherNoAsync(DateTime? voucherDate = null, int? branchId = null);
    Task<(long VoucherNo, long IdBranch)> GetNextChequeDepositNumbersAsync(DateTime voucherDate, int branchId);
    Task<ChequeDepositDto?> LoadChequeDepositByVoucherNoAsync(long voucherNo, DateTime? voucherDate = null, bool? requireIsCheque = null);
    Task<IEnumerable<ChequeDepositDto>> SearchChequeDepositsAsync(
        string? query = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool? isCheque = null,
        string? depositedBy = null,
        string? cropSeason = null,
        int? branchId = null);
    Task SaveChequeDepositAsync(ChequeDepositDto dto);
    Task DeleteChequeDepositAsync(long voucherNo, DateTime? voucherDate = null, short userId = 1);
    Task<ChequeDepositFormConfigDto> GetChequeDepositFormConfigAsync(int userId);
    Task<bool> CheckUserTaskPermissionAsync(int userId, string taskKey);
    Task<ChartAccountLookupDto?> ValidateChartAccountAsync(string accountNo);
    Task<List<ChartAccountLookupDto>> SearchChartAccountsAsync(string? filter = null, string? accountPrefix = null, bool bankOnly = false);
    Task<(decimal Balance, string FormattedText)> GetAccountBalanceTextAsync(string accountNo, DateTime asOfDate);
    Task<bool> ChequeDepositExistsAsync(long voucherNo, DateTime voucherDate);
    Task<ChequeDepositPrintDataDto?> GetChequeDepositPrintDataAsync(long voucherNo, DateTime voucherDate, int branchId, bool includeMultiForDate = false);

    // Bank Cheques Issuing (AgenSoft frmBankCheque & BankCheques Exact Parity)
    Task<long> GetNextBankChequeVoucherNoAsync();
    Task<BankChequeDto?> LoadBankChequeByVoucherNoAsync(long voucherNo);
    Task<IEnumerable<BankChequeDto>> SearchBankChequesAsync(string? query = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task SaveBankChequeAsync(BankChequeDto dto);
    Task DeleteBankChequeAsync(long voucherNo);
    Task<IEnumerable<string>> GetAvailableChequeNumbersForBankAsync(string bankAccountNo, long currentVoucherNo = 0);
}

public class ChequeDepositDto
{
    public long VoucherNo { get; set; }
    public DateTime VoucherDate { get; set; } = DateTime.Today;
    public bool IsCheque { get; set; } = true;
    public string ChequeNo { get; set; } = string.Empty;
    public DateTime ChequeDate { get; set; } = DateTime.Today;
    public string SlipNo { get; set; } = string.Empty;
    public DateTime DepositedDate { get; set; } = DateTime.Today;
    public bool IsReconciled { get; set; }
    public DateTime? ReconcileDate { get; set; }
    public int BankAccountId { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public int? PartyAccountId { get; set; }
    public string PartyAccountNo { get; set; } = string.Empty;
    public string PartyAccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string DepositedBy { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
    public string Narration { get; set; } = string.Empty;
    public string NarrationUrdu { get; set; } = string.Empty;
    public bool IsDepositConfirmed { get; set; } = true;
    public bool SendSms { get; set; }
    public int? CropSeasonId { get; set; }
    public string CropSeason { get; set; } = string.Empty;
    public int BranchId { get; set; } = 1;
    public string BranchName { get; set; } = "Main Branch";
    public long IdBranch { get; set; }
    public DateTime? OriginalVoucherDate { get; set; }
    public bool IsExisting { get; set; }
    public DateTime? VoucherTime { get; set; }
    public short UserNo { get; set; } = 1;
    public string CreatedByUserName { get; set; } = string.Empty;
    public bool IsPosted { get; set; }
    public bool IsBounced { get; set; }
    public byte[]? FPData { get; set; }
    public string? ThumbImagePath { get; set; }
}

public class BranchLookupDto
{
    public int BranchId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
}

public class ChartAccountLookupDto
{
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNameUrdu { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty;
    public string CropSeason { get; set; } = string.Empty;
}

public class ChequeDepositFormConfigDto
{
    public bool UseBankDepositMulti { get; set; }
    public bool EnableCropSeason { get; set; } = true;
    public bool EnableBranchWise { get; set; }
    public bool ShowUrduNarration { get; set; } = true;
    public bool EnableThumbOnForms { get; set; } = true;
    public bool EnableSms { get; set; } = true;
    public bool ShowPrintDialogVoucher { get; set; }
    public bool DirectPrintVoucher { get; set; }
    public bool ChangeDateBankVoucher { get; set; } = true;
    public bool ChangeVidBankVoucher { get; set; } = true;
    public bool EnableAutoVoucherId { get; set; }
    public string AutoVoucherIdMode { get; set; } = string.Empty;
    public string AccBankAccountPrefix { get; set; } = "112";
    public bool AllowModifyDelete { get; set; } = true;
    public List<BranchLookupDto> Branches { get; set; } = new();
}

public class ChequeDepositPrintDataDto
{
    public ChequeDepositDto Deposit { get; set; } = new();
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyPhone { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public byte[]? LogoBytes { get; set; }
    public string FooterInEnglish { get; set; } = string.Empty;
    public string CreatedByUserName { get; set; } = string.Empty;
    public bool ShowPrintDialogVoucher { get; set; }
    public bool DirectPrintVoucher { get; set; }
    public bool IsMultiMode { get; set; }
    public List<ChequeDepositDto> MultiItems { get; set; } = new();
}

public class BankChequeDto
{
    public long VoucherNo { get; set; }
    public DateTime VoucherDate { get; set; } = DateTime.Today;
    public string ChequeNo { get; set; } = string.Empty;
    public DateTime ChequeDate { get; set; } = DateTime.Today;
    public bool IsReconciled { get; set; }
    public DateTime? ReconcileDate { get; set; }
    public int BankAccountId { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public int? PartyAccountId { get; set; }
    public string PartyAccountNo { get; set; } = string.Empty;
    public string PartyAccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
    public string Narration { get; set; } = string.Empty;
    public string NarrationUrdu { get; set; } = string.Empty;
    public bool IsCrossCheque { get; set; } = true;
    public decimal WhtTax { get; set; }
    public decimal WhtRatio { get; set; }
    public string PmFlag { get; set; } = "-"; // "-" deducts, "+" adds
    public bool WhtToBank { get; set; }
    public bool WhtToPayee { get; set; } = true;
    public bool SendSms { get; set; }
    public int? CropSeasonId { get; set; }
    public string CropSeason { get; set; } = string.Empty;
    public int BranchId { get; set; } = 1;
    public string BranchName { get; set; } = "Main Branch";
    public List<BankChequeBodyDto> SplitDetails { get; set; } = new();
}

public class BankChequeBodyDto
{
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
}
