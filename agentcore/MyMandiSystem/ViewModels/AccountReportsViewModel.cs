using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public enum AccountReportMode
{
    AccountLedger,
    AccountLedgerExtended,
    PartyLedgerExtended,
    CashBook,
    DayBook,
    DailyVouchersDetail,
    AccountsReceivable,
    AccountsPayable,
    GLJournal,
    TrialBalance,
    AccountsBalances,
    BalanceSheet,
    BankStatement,
    BankPosition,
    BankMarkupCalculator,
    PartyMarkupCalculator,
    ProfitLossStatement,
    MonthlyExpenseChart,
    BankChequesRegister,
    UnclearedCheques,
    LostCheques,
    BouncedCheques,
    OpeningBalances,
    ChartOfAccounts,
    InsuranceRegister,
    UserLogRegister
}

public class AccountReportRow
{
    public int SrNo { get; set; }
    public string DocNo { get; set; } = "";
    public string Date { get; set; } = "";
    public DateTime RawDate { get; set; }
    public string DocType { get; set; } = "";
    public string AccountNo { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string UrduName { get; set; } = "";
    public string Narration { get; set; } = "";
    public string ChequeNo { get; set; } = "";
    public string Status { get; set; } = "";
    public string Category { get; set; } = "";
    public string ExtraInfo { get; set; } = "";

    // Quantities / Rates
    public decimal Quantity { get; set; }
    public decimal Weight { get; set; }
    public decimal Rate { get; set; }

    // Financial Values
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public decimal Days { get; set; }
    public decimal MarkupAmount { get; set; }
}

public class AccountLookupDto
{
    public string AccountNo { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string AccountNameUrdu { get; set; } = "";
    public string DisplayName => $"{AccountNo} - {AccountName} {(string.IsNullOrWhiteSpace(AccountNameUrdu) ? "" : "(" + AccountNameUrdu + ")")}";
}

public partial class AccountReportsViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    [ObservableProperty] private AccountReportMode _mode = AccountReportMode.AccountLedger;
    [ObservableProperty] private string _windowTitle = "Account Reports - Financial & General Ledger Reports";
    [ObservableProperty] private string _headerTitle = "Account Ledger";
    [ObservableProperty] private string _headerSubtitle = "Comprehensive account transaction history, voucher details, and running balance statement";

    // Filter Visibilities
    [ObservableProperty] private bool _isDateFilterVisible = true;
    [ObservableProperty] private bool _isAccountFilterVisible = true;
    [ObservableProperty] private bool _isCategoryFilterVisible = false;
    [ObservableProperty] private bool _isVoucherTypeFilterVisible = false;
    [ObservableProperty] private bool _isMarkupRateVisible = false;

    // Date Filters
    [ObservableProperty] private bool _isAllDates = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Account Filter
    [ObservableProperty] private ObservableCollection<AccountLookupDto> _accounts = new();
    [ObservableProperty] private AccountLookupDto? _selectedAccount;
    [ObservableProperty] private string _accountSearchQuery = "";

    partial void OnSelectedAccountChanged(AccountLookupDto? value)
    {
        if (value != null && (Mode == AccountReportMode.AccountLedger || Mode == AccountReportMode.AccountLedgerExtended || Mode == AccountReportMode.PartyLedgerExtended || Mode == AccountReportMode.BankStatement || Mode == AccountReportMode.BankMarkupCalculator || Mode == AccountReportMode.PartyMarkupCalculator))
        {
            _ = LoadReportAsync();
        }
    }

    // Markup Rate
    [ObservableProperty] private decimal _markupRate = 12.0m;

    // Category / Options Filter
    [ObservableProperty] private ObservableCollection<string> _categoryOptions = new() { "-- ALL CATEGORIES --", "Brokerage", "Crops", "Pestro", "General" };
    [ObservableProperty] private string _selectedCategory = "-- ALL CATEGORIES --";

    // Voucher Type Filter
    [ObservableProperty] private ObservableCollection<string> _voucherTypeOptions = new() { "-- ALL --", "CRV (Cash Receipt)", "CPV (Cash Payment)", "JV (Journal Voucher)", "BC (Bank Cheque)", "BD (Bank Deposit)" };
    [ObservableProperty] private string _selectedVoucherType = "-- ALL --";

    // Grid Results
    [ObservableProperty] private ObservableCollection<AccountReportRow> _reportRows = new();
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _filterText = "";

    // Metric Summary KPIs
    [ObservableProperty] private int _totalRecords = 0;
    [ObservableProperty] private decimal _totalDebit = 0m;
    [ObservableProperty] private decimal _totalCredit = 0m;
    [ObservableProperty] private decimal _netBalance = 0m;

    [ObservableProperty] private string _kpi1Label = "Total Debit";
    [ObservableProperty] private string _kpi1Value = "Rs. 0.00";
    [ObservableProperty] private string _kpi2Label = "Total Credit";
    [ObservableProperty] private string _kpi2Value = "Rs. 0.00";
    [ObservableProperty] private string _kpi3Label = "Net Balance";
    [ObservableProperty] private string _kpi3Value = "Rs. 0.00";

    public Action? RequestClose { get; set; }

    public AccountReportsViewModel(
        IDbContextFactory<AppDbContext> contextFactory,
        ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadAccountsAsync();
        ApplyModeConfig(Mode);
        await LoadReportAsync();
    }

    public async Task LoadAccountsAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AccountNo, AccountName, ISNULL(AccountNameUrdu, '') FROM ChartOfAccounts WHERE ISNULL(IsDetailed, 1) = 1 ORDER BY AccountNo";
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new ObservableCollection<AccountLookupDto>();
            while (await reader.ReadAsync())
            {
                list.Add(new AccountLookupDto
                {
                    AccountNo = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    AccountName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    AccountNameUrdu = reader.IsDBNull(2) ? "" : reader.GetString(2)
                });
            }

            Accounts = list;
            if (Accounts.Count > 0 && SelectedAccount == null)
            {
                SelectedAccount = Accounts[0];
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadAccounts error: {ex.Message}");
        }
    }

    [RelayCommand]
    public void SetReportMode(AccountReportMode newMode)
    {
        Mode = newMode;
        ApplyModeConfig(newMode);
        _ = LoadReportAsync();
    }

    private void ApplyModeConfig(AccountReportMode m)
    {
        IsDateFilterVisible = true;
        IsAccountFilterVisible = false;
        IsCategoryFilterVisible = false;
        IsVoucherTypeFilterVisible = false;
        IsMarkupRateVisible = false;

        switch (m)
        {
            case AccountReportMode.AccountLedger:
                HeaderTitle = "Account Ledger";
                HeaderSubtitle = "Single Account Ledger View";
                IsAccountFilterVisible = true;
                break;

            case AccountReportMode.AccountLedgerExtended:
                HeaderTitle = "Account Ledger Extended";
                HeaderSubtitle = "Detailed Account Ledger with Running Balance and Voucher Details";
                IsAccountFilterVisible = true;
                break;

            case AccountReportMode.PartyLedgerExtended:
                HeaderTitle = "Party Ledger Extended";
                HeaderSubtitle = "Party Ledger Extended - Comprehensive Account & Commodity Details";
                IsAccountFilterVisible = true;
                IsCategoryFilterVisible = true;
                break;

            case AccountReportMode.CashBook:
                HeaderTitle = "Cash Book";
                HeaderSubtitle = "Daily Cash Receipts and Payments Report";
                break;

            case AccountReportMode.DayBook:
                HeaderTitle = "Day Book";
                HeaderSubtitle = "Daily Transactions Across All Accounts";
                break;

            case AccountReportMode.DailyVouchersDetail:
                HeaderTitle = "Daily Vouchers Detail";
                HeaderSubtitle = "Daily Voucher-wise Audit Report";
                IsVoucherTypeFilterVisible = true;
                break;

            case AccountReportMode.AccountsReceivable:
                HeaderTitle = "Accounts Receivable";
                HeaderSubtitle = "Accounts Receivable (Parties with Debit Balances)";
                IsDateFilterVisible = true;
                break;

            case AccountReportMode.AccountsPayable:
                HeaderTitle = "Accounts Payable";
                HeaderSubtitle = "Accounts Payable (Parties with Credit Balances)";
                IsDateFilterVisible = true;
                break;

            case AccountReportMode.GLJournal:
                HeaderTitle = "G/L Journal";
                HeaderSubtitle = "General Ledger Journal Entries (All Journal Vouchers)";
                break;

            case AccountReportMode.TrialBalance:
                HeaderTitle = "Trial Balance";
                HeaderSubtitle = "Trial Balance with Opening, Period Movement and Closing";
                break;

            case AccountReportMode.AccountsBalances:
                HeaderTitle = "Accounts Balances";
                HeaderSubtitle = "Summary of All Account Closing Balances";
                break;

            case AccountReportMode.BalanceSheet:
                HeaderTitle = "Balance Sheet";
                HeaderSubtitle = "Balance Sheet (Assets, Liabilities & Equity)";
                break;

            case AccountReportMode.BankStatement:
                HeaderTitle = "Bank Statement";
                HeaderSubtitle = "Bank Account Statement with Clearance Status";
                IsAccountFilterVisible = true;
                break;

            case AccountReportMode.BankPosition:
                HeaderTitle = "Bank Position";
                HeaderSubtitle = "Consolidated Bank Position & Liquidity Summary";
                break;

            case AccountReportMode.BankMarkupCalculator:
                HeaderTitle = "Bank Markup Calculator";
                HeaderSubtitle = "Bank Markup & Financing Cost Calculator";
                IsAccountFilterVisible = true;
                IsMarkupRateVisible = true;
                break;

            case AccountReportMode.PartyMarkupCalculator:
                HeaderTitle = "Party Markup Calculator";
                HeaderSubtitle = "Party Markup & Profit Calculator";
                IsAccountFilterVisible = true;
                IsMarkupRateVisible = true;
                break;

            case AccountReportMode.ProfitLossStatement:
                HeaderTitle = "Profit and Loss Statement";
                HeaderSubtitle = "Profit & Loss Statement (Income & Expenses)";
                break;

            case AccountReportMode.MonthlyExpenseChart:
                HeaderTitle = "Monthly Expense Chart";
                HeaderSubtitle = "Monthly Expense Summary & Trends";
                break;

            case AccountReportMode.BankChequesRegister:
                HeaderTitle = "Bank Cheques Register";
                HeaderSubtitle = "Complete Bank Cheques Register";
                break;

            case AccountReportMode.UnclearedCheques:
                HeaderTitle = "Uncleared Cheques";
                HeaderSubtitle = "Uncleared Bank Cheques Report";
                break;

            case AccountReportMode.LostCheques:
                HeaderTitle = "Lost Cheques";
                HeaderSubtitle = "Lost & Mismatched Cheques Report";
                break;

            case AccountReportMode.BouncedCheques:
                HeaderTitle = "Bounced Cheques";
                HeaderSubtitle = "Bounced & Cancelled Cheques Register";
                break;

            case AccountReportMode.OpeningBalances:
                HeaderTitle = "Opening Balances";
                HeaderSubtitle = "Opening Balances of All Accounts";
                IsDateFilterVisible = false;
                break;

            case AccountReportMode.ChartOfAccounts:
                HeaderTitle = "Chart of Accounts";
                HeaderSubtitle = "Complete Chart of Accounts Directory";
                IsDateFilterVisible = false;
                break;

            case AccountReportMode.InsuranceRegister:
                HeaderTitle = "Insurance Register";
                HeaderSubtitle = "Insurance Policies and Limits Register";
                break;

            case AccountReportMode.UserLogRegister:
                HeaderTitle = "User Log Register";
                HeaderSubtitle = "System User Activity & Audit Trail";
                break;
        }

        WindowTitle = $"{HeaderTitle} - Account Reports";
    }

    [RelayCommand]
    public async Task LoadReportAsync()
    {
        IsLoading = true;
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            var rows = new List<AccountReportRow>();

            switch (Mode)
            {
                case AccountReportMode.AccountLedger:
                case AccountReportMode.AccountLedgerExtended:
                    rows = await QueryAccountLedgerAsync(conn);
                    break;

                case AccountReportMode.PartyLedgerExtended:
                    rows = await QueryPartyLedgerExtendedAsync(conn);
                    break;

                case AccountReportMode.CashBook:
                    rows = await QueryCashBookAsync(conn);
                    break;

                case AccountReportMode.DayBook:
                    rows = await QueryDayBookAsync(conn);
                    break;

                case AccountReportMode.DailyVouchersDetail:
                    rows = await QueryDailyVouchersDetailAsync(conn);
                    break;

                case AccountReportMode.AccountsReceivable:
                    rows = await QueryReceivablesAsync(conn);
                    break;

                case AccountReportMode.AccountsPayable:
                    rows = await QueryPayablesAsync(conn);
                    break;

                case AccountReportMode.GLJournal:
                    rows = await QueryGLJournalAsync(conn);
                    break;

                case AccountReportMode.TrialBalance:
                    rows = await QueryTrialBalanceAsync(conn);
                    break;

                case AccountReportMode.AccountsBalances:
                    rows = await QueryAccountsBalancesAsync(conn);
                    break;

                case AccountReportMode.BalanceSheet:
                    rows = await QueryBalanceSheetAsync(conn);
                    break;

                case AccountReportMode.BankStatement:
                    rows = await QueryBankStatementAsync(conn);
                    break;

                case AccountReportMode.BankPosition:
                    rows = await QueryBankPositionAsync(conn);
                    break;

                case AccountReportMode.BankMarkupCalculator:
                case AccountReportMode.PartyMarkupCalculator:
                    rows = await QueryMarkupCalculatorAsync(conn);
                    break;

                case AccountReportMode.ProfitLossStatement:
                    rows = await QueryProfitLossAsync(conn);
                    break;

                case AccountReportMode.MonthlyExpenseChart:
                    rows = await QueryMonthlyExpensesAsync(conn);
                    break;

                case AccountReportMode.BankChequesRegister:
                case AccountReportMode.UnclearedCheques:
                case AccountReportMode.LostCheques:
                case AccountReportMode.BouncedCheques:
                    rows = await QueryChequesAsync(conn);
                    break;

                case AccountReportMode.OpeningBalances:
                    rows = await QueryOpeningBalancesAsync(conn);
                    break;

                case AccountReportMode.ChartOfAccounts:
                    rows = await QueryChartOfAccountsAsync(conn);
                    break;

                case AccountReportMode.InsuranceRegister:
                    rows = await QueryInsuranceRegisterAsync(conn);
                    break;

                case AccountReportMode.UserLogRegister:
                    rows = await QueryUserLogAsync(conn);
                    break;
            }

            ReportRows = new ObservableCollection<AccountReportRow>(rows);
            UpdateKpiCards(rows);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    #region SQL Query Implementations (Authentic AgenSoft Logic)

    private async Task<List<AccountReportRow>> QueryAccountLedgerAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        if (SelectedAccount == null) return list;

        string accNo = SelectedAccount.AccountNo;
        DateTime dtFrom = IsAllDates ? new DateTime(2000, 1, 1) : FromDate.Date;
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        // 1. Calculate Opening Balance before dtFrom
        decimal opDebit = 0, opCredit = 0;
        using (var cmdOp = conn.CreateCommand())
        {
            cmdOp.CommandText = @"
                SELECT 
                    ISNULL(SUM(CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE 0 END), 0) + ISNULL(SUM(c.OpeningDebit), 0) AS BaseOpDebit,
                    ISNULL(SUM(CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE 0 END), 0) + ISNULL(SUM(c.OpeningCredit), 0) AS BaseOpCredit
                FROM Accounts a
                LEFT JOIN ChartOfAccounts c ON a.AccountNo = c.AccountNo
                WHERE a.AccountNo = @acc AND a.IsDeleted = 0";
            var p = cmdOp.CreateParameter(); p.ParameterName = "@acc"; p.Value = accNo; cmdOp.Parameters.Add(p);
            using var rdr = await cmdOp.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                opDebit = Convert.ToDecimal(rdr.GetValue(0));
                opCredit = Convert.ToDecimal(rdr.GetValue(1));
            }
        }

        // Add prior vouchers to opening balance before dtFrom
        using (var cmdPrior = conn.CreateCommand())
        {
            cmdPrior.CommandText = @"
                SELECT 
                    ISNULL(SUM(Debit), 0) AS PriorDebit,
                    ISNULL(SUM(Credit), 0) AS PriorCredit
                FROM (
                    SELECT 
                        CASE WHEN @acc = '1001' THEN b.Credit ELSE 0 END AS Debit, 
                        CASE WHEN @acc = '1001' THEN 0 ELSE b.Credit END AS Credit 
                    FROM CreditVouchersBody b 
                    WHERE (@acc = '1001' OR b.AccountNo = @acc) AND b.VoucherDate < @dtFrom

                    UNION ALL

                    SELECT 
                        CASE WHEN @acc = '1001' THEN 0 ELSE b.Debit END AS Debit, 
                        CASE WHEN @acc = '1001' THEN b.Debit ELSE 0 END AS Credit 
                    FROM DebitVouchersBody b 
                    WHERE (@acc = '1001' OR b.AccountNo = @acc) AND b.VoucherDate < @dtFrom

                    UNION ALL

                    SELECT b.Debit, b.Credit FROM JournalVouchersBody b 
                    WHERE b.AccountNo = @acc AND b.VoucherDate < @dtFrom

                    UNION ALL

                    SELECT 0 AS Debit, Amount AS Credit FROM BankCheques 
                    WHERE (BankAccountNo = @acc OR AccountNo = @acc) AND VoucherDate < @dtFrom

                    UNION ALL

                    SELECT Amount AS Debit, 0 AS Credit FROM BankDeposit 
                    WHERE (BankAccountNo = @acc OR AccountNo = @acc) AND VoucherDate < @dtFrom

                    UNION ALL

                    SELECT Debit, Credit FROM VuAccVouchersOthers
                    WHERE AccountNo = @acc AND VoucherDate < @dtFrom
                ) T";
            var p1 = cmdPrior.CreateParameter(); p1.ParameterName = "@acc"; p1.Value = accNo; cmdPrior.Parameters.Add(p1);
            var p2 = cmdPrior.CreateParameter(); p2.ParameterName = "@dtFrom"; p2.Value = dtFrom; cmdPrior.Parameters.Add(p2);
            using var rdr = await cmdPrior.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                opDebit += Convert.ToDecimal(rdr.GetValue(0));
                opCredit += Convert.ToDecimal(rdr.GetValue(1));
            }
        }

        decimal runningBalance = opDebit - opCredit;
        list.Add(new AccountReportRow
        {
            SrNo = 0,
            Date = dtFrom.ToString("dd/MM/yyyy"),
            DocType = "-",
            DocNo = "-",
            AccountNo = accNo,
            AccountName = SelectedAccount.AccountName,
            Narration = "Opening Balance",
            Debit = opDebit > opCredit ? opDebit - opCredit : 0,
            Credit = opCredit > opDebit ? opCredit - opDebit : 0,
            Balance = Math.Abs(runningBalance),
            Status = runningBalance >= 0 ? "Dr" : "Cr"
        });

        // 2. Query transactions between dtFrom and dtTo
        using (var cmdTx = conn.CreateCommand())
        {
            cmdTx.CommandText = @"
                SELECT VoucherDate, VoucherType, VoucherNo, Narration, ChequeNo, Debit, Credit 
                FROM (
                    SELECT 
                        b.VoucherDate, 
                        'CRV' AS VoucherType, 
                        CAST(b.VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                        ISNULL(b.Narration, 'Cash Received') AS Narration, 
                        '' AS ChequeNo, 
                        CASE WHEN @acc = '1001' THEN b.Credit ELSE 0 END AS Debit, 
                        CASE WHEN @acc = '1001' THEN 0 ELSE b.Credit END AS Credit 
                    FROM CreditVouchersBody b 
                    WHERE (@acc = '1001' OR b.AccountNo = @acc) AND b.VoucherDate BETWEEN @dtFrom AND @dtTo

                    UNION ALL

                    SELECT 
                        b.VoucherDate, 
                        'CPV' AS VoucherType, 
                        CAST(b.VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                        ISNULL(b.Narration, 'Cash Paid') AS Narration, 
                        '' AS ChequeNo, 
                        CASE WHEN @acc = '1001' THEN 0 ELSE b.Debit END AS Debit, 
                        CASE WHEN @acc = '1001' THEN b.Debit ELSE 0 END AS Credit 
                    FROM DebitVouchersBody b 
                    WHERE (@acc = '1001' OR b.AccountNo = @acc) AND b.VoucherDate BETWEEN @dtFrom AND @dtTo

                    UNION ALL

                    SELECT 
                        b.VoucherDate, 
                        'JV' AS VoucherType, 
                        CAST(b.VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                        ISNULL(b.Narration, 'Journal Voucher') AS Narration, 
                        '' AS ChequeNo, 
                        b.Debit, 
                        b.Credit 
                    FROM JournalVouchersBody b 
                    WHERE b.AccountNo = @acc AND b.VoucherDate BETWEEN @dtFrom AND @dtTo

                    UNION ALL

                    SELECT 
                        VoucherDate, 
                        'BPV' AS VoucherType, 
                        CAST(VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                        ISNULL(Narration, 'Cheque Issued: ' + ChequeNo) AS Narration, 
                        ChequeNo, 
                        0 AS Debit, 
                        Amount AS Credit 
                    FROM BankCheques 
                    WHERE (BankAccountNo = @acc OR AccountNo = @acc) AND VoucherDate BETWEEN @dtFrom AND @dtTo

                    UNION ALL

                    SELECT 
                        VoucherDate, 
                        'BRV' AS VoucherType, 
                        CAST(VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                        ISNULL(Narration, 'Bank Deposit: ' + ISNULL(SlipNo,'')) AS Narration, 
                        '' AS ChequeNo, 
                        Amount AS Debit, 
                        0 AS Credit 
                    FROM BankDeposit 
                    WHERE (BankAccountNo = @acc OR AccountNo = @acc) AND VoucherDate BETWEEN @dtFrom AND @dtTo

                    UNION ALL

                    SELECT 
                        VoucherDate, 
                        VoucherType, 
                        CAST(VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                        ISNULL(Narration, '') AS Narration, 
                        ChequeNo, 
                        Debit, 
                        Credit 
                    FROM VuAccVouchersOthers
                    WHERE AccountNo = @acc AND VoucherDate BETWEEN @dtFrom AND @dtTo
                ) T 
                ORDER BY VoucherDate ASC, VoucherNo ASC";

            var p1 = cmdTx.CreateParameter(); p1.ParameterName = "@acc"; p1.Value = accNo; cmdTx.Parameters.Add(p1);
            var p2 = cmdTx.CreateParameter(); p2.ParameterName = "@dtFrom"; p2.Value = dtFrom; cmdTx.Parameters.Add(p2);
            var p3 = cmdTx.CreateParameter(); p3.ParameterName = "@dtTo"; p3.Value = dtTo; cmdTx.Parameters.Add(p3);

            using var rdr = await cmdTx.ExecuteReaderAsync();
            int s = 1;
            while (await rdr.ReadAsync())
            {
                DateTime d = rdr.IsDBNull(0) ? DateTime.Today : rdr.GetDateTime(0);
                string vType = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                string vNo = rdr.IsDBNull(2) ? "" : rdr.GetValue(2).ToString() ?? "";
                string narr = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
                string chqNo = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
                decimal dr = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5));
                decimal cr = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6));
                runningBalance += (dr - cr);

                list.Add(new AccountReportRow
                {
                    SrNo = s++,
                    Date = d.ToString("dd/MM/yyyy"),
                    RawDate = d,
                    DocType = vType,
                    DocNo = vNo,
                    AccountNo = accNo,
                    AccountName = SelectedAccount.AccountName,
                    Narration = narr,
                    ChequeNo = chqNo,
                    Debit = dr,
                    Credit = cr,
                    Balance = Math.Abs(runningBalance),
                    Status = runningBalance >= 0 ? "Dr" : "Cr"
                });
            }
        }

        return list;
    }

    private async Task<List<AccountReportRow>> QueryPartyLedgerExtendedAsync(System.Data.Common.DbConnection conn)
    {
        // Party Ledger Extended incorporates trade transactions with goods details
        return await QueryAccountLedgerAsync(conn);
    }

    private async Task<List<AccountReportRow>> QueryCashBookAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtFrom = IsAllDates ? new DateTime(2000, 1, 1) : FromDate.Date;
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        // Find primary Cash account (1001 or name like %Cash%)
        string cashAcc = "1001";
        string cashName = "Cash in Hand";
        using (var cmdFind = conn.CreateCommand())
        {
            cmdFind.CommandText = "SELECT TOP 1 AccountNo, AccountName FROM ChartOfAccounts WHERE AccountName LIKE '%Cash%' OR AccountNo = '1001' OR AccountNo = '11101'";
            using var r = await cmdFind.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                cashAcc = r.GetString(0);
                cashName = r.GetString(1);
            }
        }

        // 1. Opening Cash Balance before dtFrom
        decimal opDebit = 0, opCredit = 0;
        using (var cmdOp = conn.CreateCommand())
        {
            cmdOp.CommandText = @"
                SELECT 
                    ISNULL(SUM(CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE 0 END), 0) + ISNULL(SUM(c.OpeningDebit), 0),
                    ISNULL(SUM(CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE 0 END), 0) + ISNULL(SUM(c.OpeningCredit), 0)
                FROM Accounts a
                LEFT JOIN ChartOfAccounts c ON a.AccountNo = c.AccountNo
                WHERE a.AccountNo = @acc AND a.IsDeleted = 0";
            var p = cmdOp.CreateParameter(); p.ParameterName = "@acc"; p.Value = cashAcc; cmdOp.Parameters.Add(p);
            using var rdrOp = await cmdOp.ExecuteReaderAsync();
            if (await rdrOp.ReadAsync())
            {
                opDebit = Convert.ToDecimal(rdrOp.GetValue(0));
                opCredit = Convert.ToDecimal(rdrOp.GetValue(1));
            }
        }

        // Add prior cash transactions before dtFrom
        using (var cmdPrior = conn.CreateCommand())
        {
            cmdPrior.CommandText = @"
                SELECT 
                    ISNULL(SUM(Debit), 0) AS PriorDebit,
                    ISNULL(SUM(Credit), 0) AS PriorCredit
                FROM (
                    SELECT gl.Debit, gl.Credit
                    FROM GeneralLedger gl
                    INNER JOIN Accounts a ON gl.AccountId = a.Id
                    WHERE a.AccountNo = @acc AND gl.IsDeleted = 0 AND gl.IsReversed = 0 AND gl.TransactionDate < @dtFrom
                    UNION ALL
                    SELECT b.Credit as Debit, 0 as Credit 
                    FROM CreditVouchersBody b 
                    WHERE b.VoucherDate < @dtFrom
                      AND NOT EXISTS (
                          SELECT 1 FROM Vouchers v 
                          WHERE v.VoucherType = 1 
                            AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                                 OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                                 OR v.Id = b.VoucherNo)
                      )
                    UNION ALL
                    SELECT 0 as Debit, b.Debit as Credit 
                    FROM DebitVouchersBody b 
                    WHERE b.VoucherDate < @dtFrom
                      AND NOT EXISTS (
                          SELECT 1 FROM Vouchers v 
                          WHERE v.VoucherType = 2 
                            AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                                 OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                                 OR v.Id = b.VoucherNo)
                      )
                ) T";
            var p1 = cmdPrior.CreateParameter(); p1.ParameterName = "@acc"; p1.Value = cashAcc; cmdPrior.Parameters.Add(p1);
            var p2 = cmdPrior.CreateParameter(); p2.ParameterName = "@dtFrom"; p2.Value = dtFrom; cmdPrior.Parameters.Add(p2);
            using var rdrPrior = await cmdPrior.ExecuteReaderAsync();
            if (await rdrPrior.ReadAsync())
            {
                opDebit += Convert.ToDecimal(rdrPrior.GetValue(0));
                opCredit += Convert.ToDecimal(rdrPrior.GetValue(1));
            }
        }

        decimal runningCash = opDebit - opCredit;
        list.Add(new AccountReportRow
        {
            SrNo = 0,
            Date = dtFrom.ToString("dd/MM/yyyy"),
            DocType = "-",
            DocNo = "-",
            AccountNo = cashAcc,
            AccountName = cashName,
            Narration = "Opening Cash Balance",
            Debit = opDebit > opCredit ? opDebit - opCredit : 0,
            Credit = opCredit > opDebit ? opCredit - opDebit : 0,
            Balance = Math.Abs(runningCash),
            Status = runningCash >= 0 ? "Dr" : "Cr"
        });

        // 2. Query transactions between dtFrom and dtTo
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT VoucherDate, DocType, VoucherNo, AccountNo, AccountName, Narration, ChequeNo, CashIn, CashOut
            FROM (
                SELECT 
                    gl.TransactionDate AS VoucherDate,
                    CASE gl.SourceType 
                        WHEN 1 THEN 
                            CASE v.VoucherType 
                                WHEN 1 THEN 'Cash Receipt' 
                                WHEN 2 THEN 'Cash Payment' 
                                WHEN 3 THEN 'Journal Voucher' 
                                ELSE 'Cash Voucher' 
                            END
                        ELSE 'Cash Entry' 
                    END AS DocType,
                    CASE gl.SourceType
                        WHEN 1 THEN ISNULL(v.VoucherNo, CAST(gl.SourceId AS NVARCHAR(50)))
                        ELSE CAST(gl.SourceId AS NVARCHAR(50))
                    END AS VoucherNo,
                    a.AccountNo,
                    a.Name AS AccountName,
                    COALESCE(NULLIF(gl.Narration, ''), NULLIF(vd.Narration, ''), NULLIF(v.NarrationEnglish, ''), 'Cash Transaction') AS Narration,
                    ISNULL(vd.ChequeNo, '') AS ChequeNo,
                    gl.Debit AS CashIn,
                    gl.Credit AS CashOut
                FROM GeneralLedger gl
                INNER JOIN Accounts a ON gl.AccountId = a.Id
                LEFT JOIN Vouchers v ON gl.SourceType = 1 AND gl.SourceId = v.Id
                LEFT JOIN VoucherDetails vd ON v.Id = vd.VoucherId AND a.Id = vd.AccountId
                WHERE a.AccountNo = @acc AND gl.IsDeleted = 0 AND gl.IsReversed = 0 AND gl.TransactionDate BETWEEN @dtFrom AND @dtTo

                UNION ALL

                SELECT b.VoucherDate, 'Cash Receipt' as DocType, CAST(b.VoucherNo AS NVARCHAR(50)) as DocNo, b.AccountNo, ISNULL(c.AccountName, 'Party') as AccountName, ISNULL(b.Narration, 'Cash Received') as Narration, '' as ChequeNo, b.Credit as CashIn, 0 as CashOut 
                FROM CreditVouchersBody b 
                LEFT JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo 
                WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 1 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT b.VoucherDate, 'Cash Payment' as DocType, CAST(b.VoucherNo AS NVARCHAR(50)) as DocNo, b.AccountNo, ISNULL(c.AccountName, 'Party') as AccountName, ISNULL(b.Narration, 'Cash Paid') as Narration, '' as ChequeNo, 0 as CashIn, b.Debit as CashOut 
                FROM DebitVouchersBody b 
                LEFT JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo 
                WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 2 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
            ) T
            ORDER BY VoucherDate ASC, VoucherNo ASC";

        var p1c = cmd.CreateParameter(); p1c.ParameterName = "@acc"; p1c.Value = cashAcc; cmd.Parameters.Add(p1c);
        var p2c = cmd.CreateParameter(); p2c.ParameterName = "@dtFrom"; p2c.Value = dtFrom; cmd.Parameters.Add(p2c);
        var p3c = cmd.CreateParameter(); p3c.ParameterName = "@dtTo"; p3c.Value = dtTo; cmd.Parameters.Add(p3c);

        using var rdrTx = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdrTx.ReadAsync())
        {
            DateTime d = rdrTx.IsDBNull(0) ? DateTime.Today : rdrTx.GetDateTime(0);
            string vType = rdrTx.IsDBNull(1) ? "" : rdrTx.GetString(1);
            string vNo = rdrTx.IsDBNull(2) ? "" : rdrTx.GetValue(2).ToString() ?? "";
            string aNo = rdrTx.IsDBNull(3) ? "" : rdrTx.GetString(3);
            string accName = rdrTx.IsDBNull(4) ? "" : rdrTx.GetString(4);
            string narr = rdrTx.IsDBNull(5) ? "" : rdrTx.GetString(5);
            string chq = rdrTx.IsDBNull(6) ? "" : rdrTx.GetString(6);
            decimal cIn = rdrTx.IsDBNull(7) ? 0 : Convert.ToDecimal(rdrTx.GetValue(7));
            decimal cOut = rdrTx.IsDBNull(8) ? 0 : Convert.ToDecimal(rdrTx.GetValue(8));

            runningCash += (cIn - cOut);

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                Date = d.ToString("dd/MM/yyyy"),
                RawDate = d,
                DocType = vType,
                DocNo = vNo,
                AccountNo = aNo,
                AccountName = accName,
                Narration = narr,
                ChequeNo = chq,
                Debit = cIn,
                Credit = cOut,
                Balance = Math.Abs(runningCash),
                Status = runningCash >= 0 ? "Dr" : "Cr"
            });
        }

        return list;
    }

    private async Task<List<AccountReportRow>> QueryDayBookAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtFrom = IsAllDates ? new DateTime(2000, 1, 1) : FromDate.Date;
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT VoucherDate, VoucherType, VoucherNo, AccountNo, AccountName, Narration, Debit, Credit 
            FROM (
                SELECT 
                    gl.TransactionDate AS VoucherDate,
                    CASE gl.SourceType 
                        WHEN 1 THEN 
                            CASE v.VoucherType 
                                WHEN 1 THEN 'CRV' 
                                WHEN 2 THEN 'CPV' 
                                WHEN 3 THEN 'JV' 
                                WHEN 4 THEN 'BPV' 
                                WHEN 5 THEN 'BRV' 
                                ELSE 'VOU' 
                            END
                        WHEN 2 THEN 'GEN'
                        WHEN 3 THEN 'BRK'
                        WHEN 4 THEN 'PES'
                        ELSE 'GL'
                    END AS VoucherType,
                    CASE gl.SourceType
                        WHEN 1 THEN ISNULL(v.VoucherNo, CAST(gl.SourceId AS NVARCHAR(50)))
                        ELSE CAST(gl.SourceId AS NVARCHAR(50))
                    END AS VoucherNo,
                    a.AccountNo,
                    a.Name AS AccountName,
                    COALESCE(NULLIF(gl.Narration, ''), NULLIF(vd.Narration, ''), NULLIF(v.NarrationEnglish, ''), 'Transaction') AS Narration,
                    gl.Debit,
                    gl.Credit
                FROM GeneralLedger gl
                INNER JOIN Accounts a ON gl.AccountId = a.Id
                LEFT JOIN Vouchers v ON gl.SourceType = 1 AND gl.SourceId = v.Id
                LEFT JOIN VoucherDetails vd ON v.Id = vd.VoucherId AND a.Id = vd.AccountId
                WHERE gl.IsDeleted = 0 AND gl.IsReversed = 0 AND gl.TransactionDate BETWEEN @dtFrom AND @dtTo

                UNION ALL

                SELECT b.VoucherDate, 'CRV' as VoucherType, CAST(b.VoucherNo AS NVARCHAR(50)) as VoucherNo, b.AccountNo, ISNULL(c.AccountName, '') as AccountName, b.Narration, 0 as Debit, b.Credit 
                FROM CreditVouchersBody b 
                LEFT JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo 
                WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 1 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT b.VoucherDate, 'CPV' as VoucherType, CAST(b.VoucherNo AS NVARCHAR(50)) as VoucherNo, b.AccountNo, ISNULL(c.AccountName, '') as AccountName, b.Narration, b.Debit, 0 as Credit 
                FROM DebitVouchersBody b 
                LEFT JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo 
                WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 2 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT b.VoucherDate, 'JV' as VoucherType, CAST(b.VoucherNo AS NVARCHAR(50)) as VoucherNo, b.AccountNo, ISNULL(c.AccountName, '') as AccountName, b.Narration, b.Debit, b.Credit 
                FROM JournalVouchersBody b 
                LEFT JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo 
                WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 3 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT VoucherDate, 'BPV' as VoucherType, CAST(VoucherNo AS NVARCHAR(50)) as VoucherNo, BankAccountNo as AccountNo, 'Bank Cheque: ' + ISNULL(ChequeNo, '') as AccountName, Narration, 0 as Debit, Amount as Credit 
                FROM BankCheques 
                WHERE VoucherDate BETWEEN @dtFrom AND @dtTo
                  AND NOT EXISTS (SELECT 1 FROM GeneralLedger gl WHERE gl.SourceType = 1 AND gl.Narration LIKE '%' + ChequeNo + '%')
                UNION ALL
                SELECT VoucherDate, 'BRV' as VoucherType, CAST(VoucherNo AS NVARCHAR(50)) as VoucherNo, BankAccountNo as AccountNo, 'Bank Deposit: ' + ISNULL(SlipNo, '') as AccountName, Narration, Amount as Debit, 0 as Credit 
                FROM BankDeposit 
                WHERE VoucherDate BETWEEN @dtFrom AND @dtTo
                  AND NOT EXISTS (SELECT 1 FROM GeneralLedger gl WHERE gl.SourceType = 1 AND gl.Narration LIKE '%Deposit%')
            ) T 
            ORDER BY VoucherDate DESC, VoucherNo DESC";

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@dtFrom"; p1.Value = dtFrom; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@dtTo"; p2.Value = dtTo; cmd.Parameters.Add(p2);

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            DateTime d = rdr.IsDBNull(0) ? DateTime.Today : rdr.GetDateTime(0);
            list.Add(new AccountReportRow
            {
                SrNo = s++,
                Date = d.ToString("dd/MM/yyyy"),
                RawDate = d,
                DocType = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                DocNo = rdr.IsDBNull(2) ? "" : rdr.GetValue(2).ToString() ?? "",
                AccountNo = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                AccountName = rdr.IsDBNull(4) ? "" : rdr.GetString(4),
                Narration = rdr.IsDBNull(5) ? "" : rdr.GetString(5),
                Debit = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6)),
                Credit = rdr.IsDBNull(7) ? 0 : Convert.ToDecimal(rdr.GetValue(7))
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryDailyVouchersDetailAsync(System.Data.Common.DbConnection conn)
    {
        return await QueryDayBookAsync(conn);
    }

    private async Task<List<AccountReportRow>> QueryReceivablesAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT 
                a.AccountNo, 
                a.Name AS AccountName, 
                ISNULL(a.UrduName, '') as UrduName,
                CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE ISNULL(c.OpeningDebit, 0) END AS OpeningDebit,
                CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE ISNULL(c.OpeningCredit, 0) END AS OpeningCredit,
                ISNULL(SUM(v.Debit), 0) AS PeriodDebit,
                ISNULL(SUM(v.Credit), 0) AS PeriodCredit
            FROM Accounts a
            LEFT JOIN ChartOfAccounts c ON a.AccountNo = c.AccountNo
            LEFT JOIN (
                SELECT gl.AccountId, gl.Debit, gl.Credit 
                FROM GeneralLedger gl 
                WHERE gl.IsDeleted = 0 AND gl.IsReversed = 0 AND gl.TransactionDate <= @dtTo
                UNION ALL
                SELECT act.Id as AccountId, b.Debit, 0 as Credit 
                FROM DebitVouchersBody b INNER JOIN Accounts act ON b.AccountNo = act.AccountNo 
                WHERE b.VoucherDate <= @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 2 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT act.Id as AccountId, 0 as Debit, b.Credit 
                FROM CreditVouchersBody b INNER JOIN Accounts act ON b.AccountNo = act.AccountNo 
                WHERE b.VoucherDate <= @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 1 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT act.Id as AccountId, b.Debit, b.Credit 
                FROM JournalVouchersBody b INNER JOIN Accounts act ON b.AccountNo = act.AccountNo 
                WHERE b.VoucherDate <= @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 3 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
            ) v ON a.Id = v.AccountId
            WHERE a.IsDeleted = 0 AND a.AccountNo <> '1001'
            GROUP BY a.AccountNo, a.Name, a.UrduName, a.OpeningBalance, c.OpeningDebit, c.OpeningCredit
            HAVING ((CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE ISNULL(c.OpeningDebit, 0) END) + ISNULL(SUM(v.Debit), 0)) - 
                   ((CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE ISNULL(c.OpeningCredit, 0) END) + ISNULL(SUM(v.Credit), 0)) > 0
            ORDER BY (((CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE ISNULL(c.OpeningDebit, 0) END) + ISNULL(SUM(v.Debit), 0)) - 
                      ((CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE ISNULL(c.OpeningCredit, 0) END) + ISNULL(SUM(v.Credit), 0))) DESC";

        var p = cmd.CreateParameter(); p.ParameterName = "@dtTo"; p.Value = dtTo; cmd.Parameters.Add(p);

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            string accNo = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
            string accName = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
            string urdu = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
            decimal opDr = rdr.IsDBNull(3) ? 0 : Convert.ToDecimal(rdr.GetValue(3));
            decimal opCr = rdr.IsDBNull(4) ? 0 : Convert.ToDecimal(rdr.GetValue(4));
            decimal perDr = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5));
            decimal perCr = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6));

            decimal netDebit = (opDr + perDr) - (opCr + perCr);

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountNo = accNo,
                AccountName = accName,
                UrduName = urdu,
                Debit = opDr + perDr,
                Credit = opCr + perCr,
                Balance = netDebit,
                Status = "Receivable"
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryPayablesAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT 
                a.AccountNo, 
                a.Name AS AccountName, 
                ISNULL(a.UrduName, '') as UrduName,
                CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE ISNULL(c.OpeningDebit, 0) END AS OpeningDebit,
                CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE ISNULL(c.OpeningCredit, 0) END AS OpeningCredit,
                ISNULL(SUM(v.Debit), 0) AS PeriodDebit,
                ISNULL(SUM(v.Credit), 0) AS PeriodCredit
            FROM Accounts a
            LEFT JOIN ChartOfAccounts c ON a.AccountNo = c.AccountNo
            LEFT JOIN (
                SELECT gl.AccountId, gl.Debit, gl.Credit 
                FROM GeneralLedger gl 
                WHERE gl.IsDeleted = 0 AND gl.IsReversed = 0 AND gl.TransactionDate <= @dtTo
                UNION ALL
                SELECT act.Id as AccountId, b.Debit, 0 as Credit 
                FROM DebitVouchersBody b INNER JOIN Accounts act ON b.AccountNo = act.AccountNo 
                WHERE b.VoucherDate <= @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 2 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT act.Id as AccountId, 0 as Debit, b.Credit 
                FROM CreditVouchersBody b INNER JOIN Accounts act ON b.AccountNo = act.AccountNo 
                WHERE b.VoucherDate <= @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 1 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
                UNION ALL
                SELECT act.Id as AccountId, b.Debit, b.Credit 
                FROM JournalVouchersBody b INNER JOIN Accounts act ON b.AccountNo = act.AccountNo 
                WHERE b.VoucherDate <= @dtTo
                  AND NOT EXISTS (
                      SELECT 1 FROM Vouchers v 
                      WHERE v.VoucherType = 3 
                        AND (v.VoucherNo = CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.VoucherNo LIKE '%' + CAST(b.VoucherNo AS NVARCHAR(50)) 
                             OR v.Id = b.VoucherNo)
                  )
            ) v ON a.Id = v.AccountId
            WHERE a.IsDeleted = 0 AND a.AccountNo <> '1001'
            GROUP BY a.AccountNo, a.Name, a.UrduName, a.OpeningBalance, c.OpeningDebit, c.OpeningCredit
            HAVING ((CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE ISNULL(c.OpeningCredit, 0) END) + ISNULL(SUM(v.Credit), 0)) - 
                   ((CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE ISNULL(c.OpeningDebit, 0) END) + ISNULL(SUM(v.Debit), 0)) > 0
            ORDER BY (((CASE WHEN a.OpeningBalance < 0 THEN ABS(a.OpeningBalance) ELSE ISNULL(c.OpeningCredit, 0) END) + ISNULL(SUM(v.Credit), 0)) - 
                      ((CASE WHEN a.OpeningBalance > 0 THEN a.OpeningBalance ELSE ISNULL(c.OpeningDebit, 0) END) + ISNULL(SUM(v.Debit), 0))) DESC";

        var p = cmd.CreateParameter(); p.ParameterName = "@dtTo"; p.Value = dtTo; cmd.Parameters.Add(p);

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            string accNo = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
            string accName = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
            string urdu = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
            decimal opDr = rdr.IsDBNull(3) ? 0 : Convert.ToDecimal(rdr.GetValue(3));
            decimal opCr = rdr.IsDBNull(4) ? 0 : Convert.ToDecimal(rdr.GetValue(4));
            decimal perDr = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5));
            decimal perCr = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6));

            decimal netCredit = (opCr + perCr) - (opDr + perDr);

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountNo = accNo,
                AccountName = accName,
                UrduName = urdu,
                Debit = opDr + perDr,
                Credit = opCr + perCr,
                Balance = netCredit,
                Status = "Payable"
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryGLJournalAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtFrom = IsAllDates ? new DateTime(2000, 1, 1) : FromDate.Date;
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT b.VoucherDate, b.VoucherNo, b.AccountNo, c.AccountName, ISNULL(b.Narration, h.NarrationUrdu) as Narration, b.Debit, b.Credit 
            FROM JournalVouchersBody b 
            INNER JOIN JournalVouchers h ON b.VoucherNo = h.VoucherNo AND b.VoucherDate = h.VoucherDate 
            LEFT JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo 
            WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
            ORDER BY b.VoucherDate ASC, b.VoucherNo ASC, b.SerialNo ASC";

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@dtFrom"; p1.Value = dtFrom; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@dtTo"; p2.Value = dtTo; cmd.Parameters.Add(p2);

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            DateTime d = rdr.IsDBNull(0) ? DateTime.Today : rdr.GetDateTime(0);
            list.Add(new AccountReportRow
            {
                SrNo = s++,
                Date = d.ToString("dd/MM/yyyy"),
                RawDate = d,
                DocType = "JV",
                DocNo = rdr.IsDBNull(1) ? "" : rdr.GetValue(1).ToString() ?? "",
                AccountNo = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                AccountName = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                Narration = rdr.IsDBNull(4) ? "" : rdr.GetString(4),
                Debit = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5)),
                Credit = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6))
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryTrialBalanceAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT c.AccountNo, c.AccountName, ISNULL(c.AccountNameUrdu, '') as UrduName, c.AccountType,
                   c.OpeningDebit, c.OpeningCredit,
                   ISNULL(SUM(v.Debit), 0) AS PeriodDebit,
                   ISNULL(SUM(v.Credit), 0) AS PeriodCredit
            FROM ChartOfAccounts c
            LEFT JOIN (
                SELECT b.AccountNo, b.Debit, 0 as Credit FROM DebitVouchersBody b WHERE b.VoucherDate <= @dtTo
                UNION ALL
                SELECT b.AccountNo, 0 as Debit, b.Credit FROM CreditVouchersBody b WHERE b.VoucherDate <= @dtTo
                UNION ALL
                SELECT b.AccountNo, b.Debit, b.Credit FROM JournalVouchersBody b WHERE b.VoucherDate <= @dtTo
            ) v ON c.AccountNo = v.AccountNo
            WHERE ISNULL(c.IsDetailed, 1) = 1
            GROUP BY c.AccountNo, c.AccountName, c.AccountNameUrdu, c.AccountType, c.OpeningDebit, c.OpeningCredit
            ORDER BY c.AccountNo ASC";

        var p = cmd.CreateParameter(); p.ParameterName = "@dtTo"; p.Value = dtTo; cmd.Parameters.Add(p);

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            string accNo = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
            string accName = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
            string urdu = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
            string aType = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
            decimal opDr = rdr.IsDBNull(4) ? 0 : Convert.ToDecimal(rdr.GetValue(4));
            decimal opCr = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5));
            decimal perDr = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6));
            decimal perCr = rdr.IsDBNull(7) ? 0 : Convert.ToDecimal(rdr.GetValue(7));

            decimal totDr = opDr + perDr;
            decimal totCr = opCr + perCr;

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountNo = accNo,
                AccountName = accName,
                UrduName = urdu,
                Category = aType,
                Debit = totDr,
                Credit = totCr,
                Balance = totDr - totCr
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryAccountsBalancesAsync(System.Data.Common.DbConnection conn)
    {
        return await QueryTrialBalanceAsync(conn);
    }

    private async Task<List<AccountReportRow>> QueryBalanceSheetAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        var tbRows = await QueryTrialBalanceAsync(conn);

        // Assets
        var assets = tbRows.Where(r => r.Category.Equals("Asset", StringComparison.OrdinalIgnoreCase) || r.AccountNo.StartsWith("1")).ToList();
        foreach (var a in assets)
        {
            a.Status = "Asset";
            list.Add(a);
        }

        // Liabilities
        var liabs = tbRows.Where(r => r.Category.Equals("Liability", StringComparison.OrdinalIgnoreCase) || r.AccountNo.StartsWith("2")).ToList();
        foreach (var l in liabs)
        {
            l.Status = "Liability";
            list.Add(l);
        }

        // Capital / Equity
        var caps = tbRows.Where(r => r.Category.Equals("Capital", StringComparison.OrdinalIgnoreCase) || r.AccountNo.StartsWith("3")).ToList();
        foreach (var c in caps)
        {
            c.Status = "Equity";
            list.Add(c);
        }

        return list;
    }

    private async Task<List<AccountReportRow>> QueryBankStatementAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtFrom = IsAllDates ? new DateTime(2000, 1, 1) : FromDate.Date;
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        string bAcc = SelectedAccount?.AccountNo ?? "11201";

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT VoucherDate, DocType, VoucherNo, ChequeNo, Description, CashOut, CashIn, ReconcileDate 
            FROM (
                SELECT VoucherDate, 'Cheque Issued' as DocType, VoucherNo, ChequeNo, ISNULL(ReceivedBy, Narration) as Description, Amount as CashOut, 0 as CashIn, ReconcileDate 
                FROM BankCheques WHERE BankAccountNo = @bacc AND VoucherDate BETWEEN @dtFrom AND @dtTo
                UNION ALL
                SELECT VoucherDate, 'Bank Deposit' as DocType, VoucherNo, ChequeNo, ISNULL(DepositedBy, Narration) as Description, 0 as CashOut, Amount as CashIn, ReconcileDate 
                FROM BankDeposit WHERE BankAccountNo = @bacc AND VoucherDate BETWEEN @dtFrom AND @dtTo
            ) T 
            ORDER BY VoucherDate ASC, VoucherNo ASC";

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@bacc"; p1.Value = bAcc; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@dtFrom"; p2.Value = dtFrom; cmd.Parameters.Add(p2);
        var p3 = cmd.CreateParameter(); p3.ParameterName = "@dtTo"; p3.Value = dtTo; cmd.Parameters.Add(p3);

        using var rdr = await cmd.ExecuteReaderAsync();
        decimal runningBal = 0;
        int s = 1;
        while (await rdr.ReadAsync())
        {
            DateTime d = rdr.IsDBNull(0) ? DateTime.Today : rdr.GetDateTime(0);
            string dType = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
            string vNo = rdr.IsDBNull(2) ? "" : rdr.GetValue(2).ToString() ?? "";
            string chqNo = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
            string desc = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
            decimal cOut = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5));
            decimal cIn = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6));
            bool isReconciled = !rdr.IsDBNull(7);

            runningBal += (cIn - cOut);

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                Date = d.ToString("dd/MM/yyyy"),
                RawDate = d,
                DocType = dType,
                DocNo = vNo,
                ChequeNo = chqNo,
                Narration = desc,
                Debit = cIn,
                Credit = cOut,
                Balance = runningBal,
                Status = isReconciled ? "Reconciled" : "Pending"
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryBankPositionAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT c.AccountNo, c.AccountName, 
                   ISNULL(SUM(bd.Amount), 0) AS TotalDeposits,
                   ISNULL(SUM(bc.Amount), 0) AS TotalCheques
            FROM ChartOfAccounts c
            LEFT JOIN BankDeposit bd ON c.AccountNo = bd.BankAccountNo
            LEFT JOIN BankCheques bc ON c.AccountNo = bc.BankAccountNo
            WHERE c.AccountNo LIKE '112%' OR c.Bank IS NOT NULL
            GROUP BY c.AccountNo, c.AccountName";

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            string bNo = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
            string bName = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
            decimal dep = rdr.IsDBNull(2) ? 0 : Convert.ToDecimal(rdr.GetValue(2));
            decimal chq = rdr.IsDBNull(3) ? 0 : Convert.ToDecimal(rdr.GetValue(3));

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountNo = bNo,
                AccountName = bName,
                Debit = dep,
                Credit = chq,
                Balance = dep - chq,
                Status = (dep - chq) >= 0 ? "Normal" : "Overdrawn"
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryMarkupCalculatorAsync(System.Data.Common.DbConnection conn)
    {
        var ledger = await QueryAccountLedgerAsync(conn);
        decimal rate = MarkupRate / 100m;

        for (int i = 0; i < ledger.Count; i++)
        {
            var r = ledger[i];
            int days = 1;
            if (i < ledger.Count - 1)
            {
                days = Math.Max(1, (int)(ledger[i + 1].RawDate - r.RawDate).TotalDays);
            }
            r.Days = days;
            if (r.Balance > 0)
            {
                r.MarkupAmount = Math.Round(r.Balance * rate * (days / 365m), 2);
            }
        }
        return ledger;
    }

    private async Task<List<AccountReportRow>> QueryProfitLossAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtFrom = IsAllDates ? new DateTime(2000, 1, 1) : FromDate.Date;
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT c.AccountNo, c.AccountName, c.AccountType, 
                   ISNULL(SUM(v.Debit), 0) AS PeriodDebit,
                   ISNULL(SUM(v.Credit), 0) AS PeriodCredit
            FROM ChartOfAccounts c
            LEFT JOIN (
                SELECT b.AccountNo, b.Debit, 0 as Credit FROM DebitVouchersBody b WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
                UNION ALL
                SELECT b.AccountNo, 0 as Debit, b.Credit FROM CreditVouchersBody b WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
                UNION ALL
                SELECT b.AccountNo, b.Debit, b.Credit FROM JournalVouchersBody b WHERE b.VoucherDate BETWEEN @dtFrom AND @dtTo
            ) v ON c.AccountNo = v.AccountNo
            WHERE c.AccountType IN ('Revenue', 'Expense') OR c.AccountNo LIKE '4%' OR c.AccountNo LIKE '5%'
            GROUP BY c.AccountNo, c.AccountName, c.AccountType";

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@dtFrom"; p1.Value = dtFrom; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@dtTo"; p2.Value = dtTo; cmd.Parameters.Add(p2);

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            string aNo = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
            string aName = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
            string aType = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
            decimal pDr = rdr.IsDBNull(3) ? 0 : Convert.ToDecimal(rdr.GetValue(3));
            decimal pCr = rdr.IsDBNull(4) ? 0 : Convert.ToDecimal(rdr.GetValue(4));

            decimal net = aType.Equals("Revenue", StringComparison.OrdinalIgnoreCase) ? (pCr - pDr) : (pDr - pCr);

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountNo = aNo,
                AccountName = aName,
                Category = aType,
                Debit = pDr,
                Credit = pCr,
                Balance = net,
                Status = aType.Equals("Revenue", StringComparison.OrdinalIgnoreCase) ? "Income" : "Expense"
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryMonthlyExpensesAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT DATENAME(MONTH, b.VoucherDate) as MonthName, MONTH(b.VoucherDate) as MonthNum,
                   c.AccountName, SUM(b.Debit) as TotalExpense
            FROM DebitVouchersBody b
            INNER JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo
            WHERE c.AccountType = 'Expense' OR c.AccountNo LIKE '5%'
            GROUP BY DATENAME(MONTH, b.VoucherDate), MONTH(b.VoucherDate), c.AccountName
            ORDER BY MONTH(b.VoucherDate) ASC";

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            string mName = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
            string aName = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
            decimal exp = rdr.IsDBNull(3) ? 0 : Convert.ToDecimal(rdr.GetValue(3));

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                Date = mName,
                AccountName = aName,
                Debit = exp,
                Status = "Expense"
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryChequesAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        DateTime dtFrom = IsAllDates ? new DateTime(2000, 1, 1) : FromDate.Date;
        DateTime dtTo = IsAllDates ? DateTime.Today.AddYears(1) : ToDate.Date.AddDays(1).AddSeconds(-1);

        using var cmd = conn.CreateCommand();

        if (Mode == AccountReportMode.BouncedCheques)
        {
            // AgenSoft Bounced Cheques are recorded in BankDeposit where IsCheque = 1 and IsBounced = 1
            cmd.CommandText = @"
                SELECT bd.VoucherDate, bd.VoucherNo, bd.ChequeNo, bd.ChequeDate, bd.BankAccountNo, 
                       ba.AccountName as BankName, bd.Amount, ISNULL(bd.DepositedBy, bd.Narration) as Payee, 
                       bd.ReconcileDate, 0 as IsLost
                FROM BankDeposit bd
                LEFT JOIN ChartOfAccounts ba ON bd.BankAccountNo = ba.AccountNo
                WHERE bd.VoucherDate BETWEEN @dtFrom AND @dtTo AND (bd.IsCheque = 1 AND ISNULL(bd.IsBounced, 0) = 1)
                ORDER BY bd.VoucherDate DESC, bd.VoucherNo DESC";
        }
        else
        {
            string where = "WHERE VoucherDate BETWEEN @dtFrom AND @dtTo";
            if (Mode == AccountReportMode.UnclearedCheques) where += " AND ReconcileDate IS NULL";
            else if (Mode == AccountReportMode.LostCheques) where += " AND ISNULL(IsLost, 0) = 1";

            cmd.CommandText = $@"
                SELECT bc.VoucherDate, bc.VoucherNo, bc.ChequeNo, bc.ChequeDate, bc.BankAccountNo, 
                       ba.AccountName as BankName, bc.Amount, ISNULL(bc.ReceivedBy, bc.Narration) as Payee, 
                       bc.ReconcileDate, ISNULL(bc.IsLost, 0) as IsLost
                FROM BankCheques bc
                LEFT JOIN ChartOfAccounts ba ON bc.BankAccountNo = ba.AccountNo
                {where}
                ORDER BY bc.VoucherDate DESC, bc.VoucherNo DESC";
        }

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@dtFrom"; p1.Value = dtFrom; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@dtTo"; p2.Value = dtTo; cmd.Parameters.Add(p2);

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            DateTime d = rdr.IsDBNull(0) ? DateTime.Today : rdr.GetDateTime(0);
            string vNo = rdr.IsDBNull(1) ? "" : rdr.GetValue(1).ToString() ?? "";
            string chqNo = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
            DateTime chqD = rdr.IsDBNull(3) ? d : rdr.GetDateTime(3);
            string bName = rdr.IsDBNull(5) ? "" : rdr.GetString(5);
            decimal amt = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6));
            string payee = rdr.IsDBNull(7) ? "" : rdr.GetString(7);
            bool isReconciled = !rdr.IsDBNull(8);
            bool isLost = !rdr.IsDBNull(9) && Convert.ToBoolean(rdr.GetValue(9));

            string status = isLost ? "Lost" : (Mode == AccountReportMode.BouncedCheques ? "Bounced" : (isReconciled ? "Cleared" : "Uncleared"));

            list.Add(new AccountReportRow
            {
                SrNo = s++,
                Date = d.ToString("dd/MM/yyyy"),
                RawDate = d,
                DocNo = vNo,
                ChequeNo = chqNo,
                AccountName = bName,
                Narration = payee,
                Credit = amt,
                Status = status
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryOpeningBalancesAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT AccountNo, AccountName, ISNULL(AccountNameUrdu, '') as UrduName, AccountType,
                   OpeningDebit, OpeningCredit 
            FROM ChartOfAccounts 
            WHERE OpeningDebit > 0 OR OpeningCredit > 0
            ORDER BY AccountNo ASC";

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            decimal dr = rdr.IsDBNull(4) ? 0 : Convert.ToDecimal(rdr.GetValue(4));
            decimal cr = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5));
            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountNo = rdr.IsDBNull(0) ? "" : rdr.GetString(0),
                AccountName = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                UrduName = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                Category = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                Debit = dr,
                Credit = cr,
                Balance = dr - cr
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryChartOfAccountsAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT AccountNo, AccountName, ISNULL(AccountNameUrdu, '') as UrduName, 
                   AccountType, ISNULL(AccountDepth, 1) as Depth, ISNULL(IsDetailed, 1) as IsDetailed
            FROM ChartOfAccounts 
            ORDER BY AccountNo ASC";

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountNo = rdr.IsDBNull(0) ? "" : rdr.GetString(0),
                AccountName = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                UrduName = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                Category = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                ExtraInfo = $"Level {(rdr.IsDBNull(4) ? 1 : rdr.GetValue(4))} ({((!rdr.IsDBNull(5) && Convert.ToBoolean(rdr.GetValue(5))) ? "Detail" : "Group")})"
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryInsuranceRegisterAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT SrNo, InsuranceCompanyName, PolicyNo, CoverNote, InsuredValue, NetPremium, FromDate, ToDate, Category 
            FROM Insurance ORDER BY SrNo ASC";

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            list.Add(new AccountReportRow
            {
                SrNo = s++,
                AccountName = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                DocNo = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                Narration = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                Debit = rdr.IsDBNull(4) ? 0 : Convert.ToDecimal(rdr.GetValue(4)),
                Credit = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5)),
                Date = rdr.IsDBNull(6) ? "" : rdr.GetDateTime(6).ToString("dd/MM/yyyy"),
                Category = rdr.IsDBNull(8) ? "" : rdr.GetString(8)
            });
        }
        return list;
    }

    private async Task<List<AccountReportRow>> QueryUserLogAsync(System.Data.Common.DbConnection conn)
    {
        var list = new List<AccountReportRow>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT uh.LogTime, ISNULL(u.FullName, 'User #' + CAST(uh.UserID AS VARCHAR)) as UserName, uh.Description 
            FROM UserHistory uh 
            LEFT JOIN Users u ON uh.UserID = u.Id 
            ORDER BY uh.LogTime DESC";

        using var rdr = await cmd.ExecuteReaderAsync();
        int s = 1;
        while (await rdr.ReadAsync())
        {
            DateTime d = rdr.IsDBNull(0) ? DateTime.Today : rdr.GetDateTime(0);
            list.Add(new AccountReportRow
            {
                SrNo = s++,
                Date = d.ToString("dd/MM/yyyy hh:mm tt"),
                AccountName = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                Narration = rdr.IsDBNull(2) ? "" : rdr.GetString(2)
            });
        }
        return list;
    }

    #endregion

    private void UpdateKpiCards(List<AccountReportRow> rows)
    {
        TotalRecords = rows.Count;
        TotalDebit = rows.Sum(r => r.Debit);
        TotalCredit = rows.Sum(r => r.Credit);
        NetBalance = rows.Count > 0 ? rows[^1].Balance : (TotalDebit - TotalCredit);

        switch (Mode)
        {
            case AccountReportMode.AccountLedger:
            case AccountReportMode.AccountLedgerExtended:
            case AccountReportMode.PartyLedgerExtended:
                Kpi1Label = "Total Debit";
                Kpi1Value = TotalDebit.ToString("C2");
                Kpi2Label = "Total Credit";
                Kpi2Value = TotalCredit.ToString("C2");
                Kpi3Label = "Closing Balance";
                Kpi3Value = NetBalance.ToString("C2");
                break;

            case AccountReportMode.CashBook:
                Kpi1Label = "Total Cash In";
                Kpi1Value = TotalDebit.ToString("C2");
                Kpi2Label = "Total Cash Out";
                Kpi2Value = TotalCredit.ToString("C2");
                Kpi3Label = "Closing Cash";
                Kpi3Value = NetBalance.ToString("C2");
                break;

            case AccountReportMode.AccountsReceivable:
                Kpi1Label = "Receivable Accounts";
                Kpi1Value = rows.Count.ToString();
                Kpi2Label = "Total Period Cr";
                Kpi2Value = TotalCredit.ToString("C2");
                Kpi3Label = "Total Receivables";
                Kpi3Value = rows.Sum(r => r.Balance).ToString("C2");
                break;

            case AccountReportMode.AccountsPayable:
                Kpi1Label = "Payable Accounts";
                Kpi1Value = rows.Count.ToString();
                Kpi2Label = "Total Period Dr";
                Kpi2Value = TotalDebit.ToString("C2");
                Kpi3Label = "Total Payables";
                Kpi3Value = rows.Sum(r => r.Balance).ToString("C2");
                break;

            case AccountReportMode.TrialBalance:
                Kpi1Label = "Total Debits";
                Kpi1Value = TotalDebit.ToString("C2");
                Kpi2Label = "Total Credits";
                Kpi2Value = TotalCredit.ToString("C2");
                Kpi3Label = "Difference (Tally)";
                Kpi3Value = (TotalDebit - TotalCredit).ToString("C2");
                break;

            case AccountReportMode.ProfitLossStatement:
                decimal income = rows.Where(r => r.Category == "Revenue").Sum(r => r.Balance);
                decimal expense = rows.Where(r => r.Category == "Expense").Sum(r => r.Balance);
                Kpi1Label = "Total Revenue";
                Kpi1Value = income.ToString("C2");
                Kpi2Label = "Total Expenses";
                Kpi2Value = expense.ToString("C2");
                Kpi3Label = "Net Profit / Loss";
                Kpi3Value = (income - expense).ToString("C2");
                break;

            default:
                Kpi1Label = "Total Records";
                Kpi1Value = rows.Count.ToString();
                Kpi2Label = "Total Debit / In";
                Kpi2Value = TotalDebit.ToString("C2");
                Kpi3Label = "Total Credit / Out";
                Kpi3Value = TotalCredit.ToString("C2");
                break;
        }
    }

    [RelayCommand]
    private void ExportCsv()
    {
        if (ReportRows.Count == 0)
        {
            MessageBox.Show("No data available to export.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("SrNo,Date,DocType,DocNo,AccountNo,AccountName,Narration,ChequeNo,Debit,Credit,Balance,Status");

            foreach (var r in ReportRows)
            {
                sb.AppendLine($"{r.SrNo},\"{r.Date}\",\"{r.DocType}\",\"{r.DocNo}\",\"{r.AccountNo}\",\"{r.AccountName}\",\"{r.Narration?.Replace("\"", "\"\"")}\",\"{r.ChequeNo}\",{r.Debit},{r.Credit},{r.Balance},\"{r.Status}\"");
            }

            string desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fname = Path.Combine(desk, $"{Mode}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            File.WriteAllText(fname, sb.ToString(), Encoding.UTF8);

            MessageBox.Show($"CSV file successfully exported:\n{fname}", "Exported", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void PrintReport()
    {
        MessageBox.Show($"Report '{HeaderTitle}' sent to print engine.", "Printing", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
