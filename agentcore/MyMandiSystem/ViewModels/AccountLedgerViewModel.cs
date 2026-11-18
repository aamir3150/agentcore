using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using MyMandiSystem.Infrastructure;
using MyMandiSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class AccountLedgerViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IReportService _reportService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;

    // Filters
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = "All Crops";
    [ObservableProperty] private bool _withoutOpening = false;
    [ObservableProperty] private bool _isUrduPrint = false;
    [ObservableProperty] private bool _separateBrokerageInvoiceIds = false;

    // Account
    [ObservableProperty] private string _accountNo = "";
    [ObservableProperty] private string _accountName = "";
    [ObservableProperty] private decimal _currentBalance = 0m;
    [ObservableProperty] private string _currentBalanceStatus = "Dr";
    [ObservableProperty] private Account? _selectedAccount;
    [ObservableProperty] private ObservableCollection<Account> _accountsList = new();

    // Date Options
    [ObservableProperty] private bool _isAllDates = false;
    [ObservableProperty] private bool _isSingleDate = false;
    [ObservableProperty] private bool _isDateRange = true;

    [ObservableProperty] private DateTime _singleDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Grid Results
    [ObservableProperty] private ObservableCollection<AccountReportRow> _ledgerRows = new();
    [ObservableProperty] private bool _isLoading = false;

    // Metric Summary
    [ObservableProperty] private int _totalRecords = 0;
    [ObservableProperty] private decimal _totalDebit = 0m;
    [ObservableProperty] private decimal _totalCredit = 0m;
    [ObservableProperty] private decimal _closingBalance = 0m;
    [ObservableProperty] private string _closingBalanceStatus = "Dr";

    // Lookup Modal Visibility
    [ObservableProperty] private bool _isAccountLookupOpen = false;
    [ObservableProperty] private string _lookupSearchQuery = "";
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();

    public Action? RequestClose { get; set; }

    public AccountLedgerViewModel(
        IDbContextFactory<AppDbContext> contextFactory,
        IReportService reportService,
        IAccountService accountService,
        ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _reportService = reportService;
        _accountService = accountService;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        try
        {
            // 1. Load Accounts FIRST directly from ChartOfAccounts
            var accounts = await _accountService.GetAllAccountsAsync();
            AccountsList = new ObservableCollection<Account>(accounts);
            FilteredAccounts = new ObservableCollection<Account>(accounts);

            // Default account selection: pick first customer/party account, or first account
            if (AccountsList.Count > 0)
            {
                var def = AccountsList.FirstOrDefault(a => a.AccountNo == "610001")
                          ?? AccountsList.FirstOrDefault(a => a.AccountNo != "1001")
                          ?? AccountsList[0];
                AccountNo = def.AccountNo;
                AccountName = def.Name;
                SelectedAccount = def;
                CurrentBalance = Math.Abs(def.CurrentBalance);
                CurrentBalanceStatus = def.CurrentBalance >= 0 ? "Dr" : "Cr";
            }

            // 2. Load Seasons with native fallback
            var dbSeasons = await _systemService.GetAllSeasonsAsync();
            var seasonsList = new List<string> { "All Crops" };
            seasonsList.AddRange(dbSeasons.Select(s => s.Name).Where(n => !string.IsNullOrWhiteSpace(n)));
            CropSeasons = new ObservableCollection<string>(seasonsList);
            SelectedCropSeason = "All Crops";

            // 3. Set financial year dates
            var year = await _systemService.GetCurrentYearAsync();
            if (year != null)
            {
                FromDate = year.StartDate;
                ToDate = DateTime.Today < year.EndDate ? DateTime.Today : year.EndDate;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Initialize error: {ex.Message}");
        }
    }

    partial void OnAccountNoChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AccountName = "";
            SelectedAccount = null;
            CurrentBalance = 0m;
            CurrentBalanceStatus = "Dr";
            return;
        }

        var match = AccountsList.FirstOrDefault(a => a.AccountNo.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            SelectedAccount = match;
            AccountName = match.Name;
            CurrentBalance = Math.Abs(match.CurrentBalance);
            CurrentBalanceStatus = match.CurrentBalance >= 0 ? "Dr" : "Cr";
            _ = LoadLedgerAsync();
        }
        else
        {
            _ = LookupAndSetAccountAsync(value.Trim());
        }
    }

    private async Task LookupAndSetAccountAsync(string no)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AccountNo, AccountName, ISNULL(OpeningDebit,0)-ISNULL(OpeningCredit,0) FROM ChartOfAccounts WHERE AccountNo = @acc";
            var p = cmd.CreateParameter();
            p.ParameterName = "@acc";
            p.Value = no;
            cmd.Parameters.Add(p);
            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                string rAccNo = r[0]?.ToString()?.Trim() ?? no;
                string rName = r[1]?.ToString()?.Trim() ?? string.Empty;
                decimal bal = r[2] != DBNull.Value ? Convert.ToDecimal(r[2]) : 0m;

                if (AccountNo?.Trim() == no)
                {
                    AccountName = rName;
                    CurrentBalance = Math.Abs(bal);
                    CurrentBalanceStatus = bal >= 0 ? "Dr" : "Cr";
                    SelectedAccount = new Account
                    {
                        AccountNo = rAccNo,
                        Name = rName,
                        OpeningBalance = bal,
                        CurrentBalance = bal
                    };
                }
            }
        }
        catch { }
    }

    partial void OnLookupSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            FilteredAccounts = new ObservableCollection<Account>(AccountsList);
        }
        else
        {
            var query = value.ToLower().Trim();
            var matches = AccountsList.Where(a =>
                (a.AccountNo != null && a.AccountNo.ToLower().Contains(query)) ||
                (a.Name != null && a.Name.ToLower().Contains(query)));
            FilteredAccounts = new ObservableCollection<Account>(matches);
        }
    }

    [RelayCommand]
    private void OpenAccountLookup()
    {
        LookupSearchQuery = "";
        FilteredAccounts = new ObservableCollection<Account>(AccountsList);
        IsAccountLookupOpen = true;
    }

    [RelayCommand]
    private void SelectAccountFromLookup(Account? account)
    {
        if (account != null)
        {
            SelectedAccount = account;
            AccountNo = account.AccountNo;
            AccountName = account.Name;
            CurrentBalance = Math.Abs(account.CurrentBalance);
            CurrentBalanceStatus = account.CurrentBalance >= 0 ? "Dr" : "Cr";
            _ = LoadLedgerAsync();
        }
        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    private void CloseAccountLookup()
    {
        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    public async Task LoadLedgerAsync()
    {
        if (string.IsNullOrWhiteSpace(AccountNo)) return;

        IsLoading = true;
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            string accNo = AccountNo.Trim();
            DateTime dtFrom;
            DateTime dtTo;

            if (IsSingleDate)
            {
                dtFrom = SingleDate.Date;
                dtTo = SingleDate.Date.AddDays(1).AddSeconds(-1);
            }
            else if (IsAllDates)
            {
                dtFrom = new DateTime(2000, 1, 1);
                dtTo = DateTime.Today.AddYears(1);
            }
            else
            {
                dtFrom = FromDate.Date;
                dtTo = ToDate.Date.AddDays(1).AddSeconds(-1);
            }

            var list = new List<AccountReportRow>();

            // 1. Calculate Opening Balance before dtFrom (Native ChartOfAccounts)
            decimal opDebit = 0, opCredit = 0;
            using (var cmdOp = conn.CreateCommand())
            {
                cmdOp.CommandText = @"
                    SELECT 
                        ISNULL(SUM(OpeningDebit), 0) AS BaseOpDebit,
                        ISNULL(SUM(OpeningCredit), 0) AS BaseOpCredit
                    FROM ChartOfAccounts
                    WHERE AccountNo = @acc";
                var p = cmdOp.CreateParameter(); p.ParameterName = "@acc"; p.Value = accNo; cmdOp.Parameters.Add(p);
                using var rdrOp = await cmdOp.ExecuteReaderAsync();
                if (await rdrOp.ReadAsync())
                {
                    opDebit = Convert.ToDecimal(rdrOp.GetValue(0));
                    opCredit = Convert.ToDecimal(rdrOp.GetValue(1));
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
                using var rdrPrior = await cmdPrior.ExecuteReaderAsync();
                if (await rdrPrior.ReadAsync())
                {
                    opDebit += Convert.ToDecimal(rdrPrior.GetValue(0));
                    opCredit += Convert.ToDecimal(rdrPrior.GetValue(1));
                }
            }

            decimal runningBalance = opDebit - opCredit;

            // Row 0: Opening Balance (unless WithoutOpening is checked)
            if (!WithoutOpening)
            {
                list.Add(new AccountReportRow
                {
                    SrNo = 0,
                    Date = IsAllDates ? "-" : dtFrom.ToString("dd/MM/yyyy"),
                    DocType = "-",
                    DocNo = "-",
                    AccountNo = accNo,
                    AccountName = AccountName,
                    Narration = IsUrduPrint ? "سابقہ بقایا" : "Opening Balance",
                    ChequeNo = "-",
                    Debit = opDebit > opCredit ? opDebit - opCredit : 0,
                    Credit = opCredit > opDebit ? opCredit - opDebit : 0,
                    Balance = Math.Abs(runningBalance),
                    Status = runningBalance >= 0 ? "Dr" : "Cr"
                });
            }

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

                using var rdrTx = await cmdTx.ExecuteReaderAsync();
                int s = 1;
                while (await rdrTx.ReadAsync())
                {
                    DateTime d = rdrTx.IsDBNull(0) ? DateTime.Today : rdrTx.GetDateTime(0);
                    string vType = rdrTx.IsDBNull(1) ? "" : rdrTx.GetString(1);
                    string vNo = rdrTx.IsDBNull(2) ? "" : rdrTx.GetValue(2).ToString() ?? "";
                    string rawNarr = rdrTx.IsDBNull(3) ? "" : rdrTx.GetString(3);
                    string chqNo = rdrTx.IsDBNull(4) ? "" : rdrTx.GetString(4);
                    decimal dr = rdrTx.IsDBNull(5) ? 0 : Convert.ToDecimal(rdrTx.GetValue(5));
                    decimal cr = rdrTx.IsDBNull(6) ? 0 : Convert.ToDecimal(rdrTx.GetValue(6));

                    // Normalize Voucher Types
                    string displayVType = vType switch
                    {
                        "DV" => "CPV",
                        "CV" => "CRV",
                        "BC" => "BPV",
                        "BD" => "BRV",
                        _ => vType
                    };

                    // Sensible default narration if empty
                    string narr = string.IsNullOrWhiteSpace(rawNarr)
                        ? (displayVType == "CPV" ? "Cash Paid" :
                           displayVType == "CRV" ? "Cash Received" :
                           displayVType == "JV" ? "Journal Voucher" :
                           displayVType == "BPV" ? "Bank Payment" :
                           displayVType == "BRV" ? "Bank Deposit" : "")
                        : rawNarr.Trim();

                    runningBalance += (dr - cr);

                    list.Add(new AccountReportRow
                    {
                        SrNo = s++,
                        Date = d.ToString("dd/MM/yyyy"),
                        RawDate = d,
                        DocType = displayVType,
                        DocNo = vNo,
                        AccountNo = accNo,
                        AccountName = AccountName,
                        Narration = narr,
                        ChequeNo = string.IsNullOrWhiteSpace(chqNo) ? "-" : chqNo,
                        Debit = dr,
                        Credit = cr,
                        Balance = Math.Abs(runningBalance),
                        Status = runningBalance >= 0 ? "Dr" : "Cr"
                    });
                }
            }

            LedgerRows = new ObservableCollection<AccountReportRow>(list);

            // Update Summary
            TotalRecords = list.Count;
            TotalDebit = list.Sum(r => r.Debit);
            TotalCredit = list.Sum(r => r.Credit);
            ClosingBalance = Math.Abs(runningBalance);
            ClosingBalanceStatus = runningBalance >= 0 ? "Dr" : "Cr";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading ledger: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task PreviewAsync()
    {
        if (string.IsNullOrWhiteSpace(AccountNo))
        {
            MessageBox.Show("Please select an Account (A/c No.) first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await LoadLedgerAsync();

        try
        {
            var data = LedgerRows.Select(r => new
            {
                Date = r.Date,
                VoucherNo = string.IsNullOrWhiteSpace(r.DocNo) || r.DocNo == "-" ? (r.DocType ?? "-") : $"{r.DocType} #{r.DocNo}",
                Narration = r.Narration ?? string.Empty,
                ChequeNo = string.IsNullOrWhiteSpace(r.ChequeNo) ? "-" : r.ChequeNo,
                Debit = r.Debit,
                Credit = r.Credit,
                Balance = r.Balance,
                Status = r.Status
            }).ToList();

            string titleName = IsUrduPrint && !string.IsNullOrWhiteSpace(SelectedAccount?.UrduName)
                ? $"{AccountNo} ({SelectedAccount.UrduName})"
                : $"{AccountNo} ({AccountName})";

            string periodText = IsSingleDate ? $"{SingleDate:dd/MM/yyyy}" :
                                IsDateRange ? $"{FromDate:dd/MM/yyyy} to {ToDate:dd/MM/yyyy}" :
                                "All Dates";

            var parameters = new Dictionary<string, string>
            {
                { "Account", titleName },
                { "Period", periodText }
            };

            var reportWindow = new ReportWindow(
                reportPath: "Reports/LedgerReport.rdlc",
                dataSourceName: "DS_Ledger",
                data: data,
                parameters: parameters
            );

            reportWindow.Title = $"Account Ledger - {titleName}";
            reportWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating preview: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task PrintAsync()
    {
        await PreviewAsync();
    }

    [RelayCommand]
    private void ExportCsv()
    {
        if (LedgerRows.Count == 0)
        {
            MessageBox.Show("No data available to export.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("SrNo,Date,DocType,DocNo,AccountNo,AccountName,Narration,ChequeNo,Debit,Credit,Balance,Status");

            foreach (var r in LedgerRows)
            {
                sb.AppendLine($"{r.SrNo},\"{r.Date}\",\"{r.DocType}\",\"{r.DocNo}\",\"{r.AccountNo}\",\"{r.AccountName}\",\"{r.Narration?.Replace("\"", "\"\"")}\",\"{r.ChequeNo}\",{r.Debit},{r.Credit},{r.Balance},\"{r.Status}\"");
            }

            string desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fname = Path.Combine(desk, $"AccountLedger_{AccountNo}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            File.WriteAllText(fname, sb.ToString(), Encoding.UTF8);

            MessageBox.Show($"CSV file successfully exported:\n{fname}", "Exported", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Reset()
    {
        AccountNo = "";
        AccountName = "";
        SelectedAccount = null;
        LedgerRows.Clear();
        TotalRecords = 0;
        TotalDebit = 0m;
        TotalCredit = 0m;
        ClosingBalance = 0m;
        ClosingBalanceStatus = "Dr";
        CurrentBalance = 0m;
        CurrentBalanceStatus = "Dr";
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
