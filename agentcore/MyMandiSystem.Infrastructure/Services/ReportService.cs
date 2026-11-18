using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public partial class ReportService : IReportService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ReportService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<dynamic>> GetLedgerReportAsync(int accountId, DateTime? from = null, DateTime? to = null, bool withoutOpening = false, bool isUrdu = false)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        bool isFromBounded = from.HasValue && from.Value > new DateTime(1900, 1, 1);
        bool isToBounded = to.HasValue && to.Value < new DateTime(2900, 1, 1);

        DateTime? startDate = isFromBounded ? from!.Value.Date : null;
        DateTime? endDate = isToBounded ? to!.Value : null;

        try
        {
            var account = await context.Accounts.FindAsync(accountId);
            decimal baseOpening = account?.OpeningBalance ?? 0m;

            decimal openingBalance = baseOpening;

            if (isFromBounded)
            {
                var priorDebit = await context.GeneralLedger
                    .Where(g => g.AccountId == accountId && g.TransactionDate < startDate!.Value)
                    .SumAsync(g => (decimal?)g.Debit) ?? 0m;

                var priorCredit = await context.GeneralLedger
                    .Where(g => g.AccountId == accountId && g.TransactionDate < startDate!.Value)
                    .SumAsync(g => (decimal?)g.Credit) ?? 0m;

                openingBalance += (priorDebit - priorCredit);
            }

            var query = context.GeneralLedger
                .Include(g => g.Account)
                .Where(g => g.AccountId == accountId);

            if (isFromBounded)
            {
                query = query.Where(g => g.TransactionDate >= startDate!.Value);
            }

            if (isToBounded)
            {
                query = query.Where(g => g.TransactionDate <= endDate!.Value);
            }

            var rawTransactions = await query
                .OrderBy(g => g.TransactionDate)
                .ThenBy(g => g.Id)
                .Select(g => new
                {
                    g.TransactionDate,
                    Narration = g.Narration ?? "",
                    g.Debit,
                    g.Credit,
                    g.SourceType,
                    g.SourceId
                })
                .ToListAsync();

            // 1. Resolve Voucher Numbers
            var voucherIds = rawTransactions
                .Where(t => t.SourceType == SourceType.Voucher && t.SourceId > 0)
                .Select(t => t.SourceId)
                .Distinct()
                .ToList();

            var vouchers = voucherIds.Count > 0
                ? await context.Vouchers.Where(v => voucherIds.Contains(v.Id)).ToDictionaryAsync(v => v.Id, v => v.VoucherNo)
                : new Dictionary<int, string>();

            // 2. Resolve Invoice Numbers (for General / Brokerage / Pestro Invoices)
            var invoiceIds = rawTransactions
                .Where(t => t.SourceType != SourceType.Voucher && t.SourceId > 0)
                .Select(t => t.SourceId)
                .Distinct()
                .ToList();

            var invoices = invoiceIds.Count > 0
                ? await context.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToDictionaryAsync(i => i.Id, i => i.InvoiceNo)
                : new Dictionary<int, string>();

            static string FormatBalance(decimal bal)
            {
                if (bal == 0m) return "0";
                return bal > 0m ? $"{bal:N0} Dr" : $"{Math.Abs(bal):N0} Cr";
            }

            var reportData = new List<LedgerReportItem>();
            decimal runningBalance = 0m;

            // 3. Opening Balance Row
            if (!withoutOpening)
            {
                DateTime opDate;
                if (isFromBounded)
                {
                    opDate = startDate!.Value > DateTime.MinValue.AddDays(1) ? startDate!.Value.AddDays(-1) : startDate!.Value;
                }
                else
                {
                    opDate = rawTransactions.Count > 0 ? rawTransactions[0].TransactionDate.Date : DateTime.Today;
                }

                string narration = isUrdu ? "سابقہ بقایا / اوپننگ بیلنس" : "Opening Balance";
                string vNo = isUrdu ? "سابقہ" : "OPEN";

                decimal opDebit = openingBalance > 0 ? openingBalance : 0m;
                decimal opCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0m;
                runningBalance = openingBalance;

                reportData.Add(new LedgerReportItem
                {
                    VoucherNo = vNo,
                    TransactionDate = opDate,
                    Narration = narration,
                    Debit = opDebit,
                    Credit = opCredit,
                    Balance = FormatBalance(runningBalance)
                });
            }

            // 4. Transaction Rows with Running Balance
            foreach (var t in rawTransactions)
            {
                string vNo;
                if (t.SourceType == SourceType.Voucher && vouchers.TryGetValue(t.SourceId, out var vn) && !string.IsNullOrWhiteSpace(vn))
                {
                    vNo = vn;
                }
                else if (invoices.TryGetValue(t.SourceId, out var invNo) && !string.IsNullOrWhiteSpace(invNo))
                {
                    vNo = invNo;
                }
                else
                {
                    vNo = $"{t.SourceType}-{t.SourceId}";
                }

                runningBalance += (t.Debit - t.Credit);

                reportData.Add(new LedgerReportItem
                {
                    VoucherNo = vNo,
                    TransactionDate = t.TransactionDate,
                    Narration = t.Narration,
                    Debit = t.Debit,
                    Credit = t.Credit,
                    Balance = FormatBalance(runningBalance)
                });
            }

            return reportData;
        }
        catch
        {
            // Native AgenSoft Fallback using ChartOfAccounts & Native Voucher Tables
            var reportData = new List<LedgerReportItem>();
            try
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

                // 1. Resolve AccountNo from ChartOfAccounts
                string accNo = accountId.ToString();
                using (var cmdAcc = conn.CreateCommand())
                {
                    cmdAcc.CommandText = "SELECT TOP 1 AccountNo FROM ChartOfAccounts WHERE AccountNo = @accId OR AccountNo LIKE '%' + @accId";
                    var pAcc = cmdAcc.CreateParameter();
                    pAcc.ParameterName = "@accId";
                    pAcc.Value = accountId.ToString();
                    cmdAcc.Parameters.Add(pAcc);
                    var res = await cmdAcc.ExecuteScalarAsync();
                    if (res != null && res != DBNull.Value)
                    {
                        accNo = res.ToString()!.Trim();
                    }
                }

                DateTime dtFrom = startDate ?? new DateTime(2000, 1, 1);
                DateTime dtTo = endDate ?? DateTime.Today.AddYears(1);

                // 2. Base Opening from ChartOfAccounts
                decimal opDebit = 0m, opCredit = 0m;
                using (var cmdOp = conn.CreateCommand())
                {
                    cmdOp.CommandText = "SELECT ISNULL(SUM(OpeningDebit), 0), ISNULL(SUM(OpeningCredit), 0) FROM ChartOfAccounts WHERE AccountNo = @acc";
                    var p = cmdOp.CreateParameter(); p.ParameterName = "@acc"; p.Value = accNo; cmdOp.Parameters.Add(p);
                    using var rdrOp = await cmdOp.ExecuteReaderAsync();
                    if (await rdrOp.ReadAsync())
                    {
                        opDebit = Convert.ToDecimal(rdrOp.GetValue(0));
                        opCredit = Convert.ToDecimal(rdrOp.GetValue(1));
                    }
                }
                decimal openingBalance = (opDebit - opCredit);

                // 3. Transactions prior to dtFrom
                if (isFromBounded)
                {
                    using var cmdPrior = conn.CreateCommand();
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
                    var pAcc = cmdPrior.CreateParameter(); pAcc.ParameterName = "@acc"; pAcc.Value = accNo; cmdPrior.Parameters.Add(pAcc);
                    var pFrom = cmdPrior.CreateParameter(); pFrom.ParameterName = "@dtFrom"; pFrom.Value = dtFrom; cmdPrior.Parameters.Add(pFrom);
                    using var rdrPrior = await cmdPrior.ExecuteReaderAsync();
                    if (await rdrPrior.ReadAsync())
                    {
                        decimal priorDr = Convert.ToDecimal(rdrPrior.GetValue(0));
                        decimal priorCr = Convert.ToDecimal(rdrPrior.GetValue(1));
                        openingBalance += (priorDr - priorCr);
                    }
                }

                static string FormatBalance(decimal bal)
                {
                    if (bal == 0m) return "0";
                    return bal > 0m ? $"{bal:N0} Dr" : $"{Math.Abs(bal):N0} Cr";
                }

                decimal runningBalance = 0m;
                if (!withoutOpening)
                {
                    DateTime opDate = dtFrom > DateTime.MinValue.AddDays(1) ? dtFrom.AddDays(-1) : dtFrom;
                    string narration = isUrdu ? "سابقہ بقایا / اوپننگ بیلنس" : "Opening Balance";
                    string vNo = isUrdu ? "سابقہ" : "OPEN";
                    decimal baseDr = openingBalance > 0 ? openingBalance : 0m;
                    decimal baseCr = openingBalance < 0 ? Math.Abs(openingBalance) : 0m;
                    runningBalance = openingBalance;

                    reportData.Add(new LedgerReportItem
                    {
                        VoucherNo = vNo,
                        TransactionDate = opDate,
                        Narration = narration,
                        Debit = baseDr,
                        Credit = baseCr,
                        Balance = FormatBalance(runningBalance)
                    });
                }

                // 4. Period Transactions
                using (var cmdTx = conn.CreateCommand())
                {
                    cmdTx.CommandText = @"
                        SELECT 
                            VoucherDate, 
                            VoucherType, 
                            VoucherNo, 
                            Narration, 
                            Debit, 
                            Credit 
                        FROM (
                            SELECT 
                                b.VoucherDate, 
                                'CRV' AS VoucherType, 
                                CAST(b.VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                                ISNULL(b.Narration, '') AS Narration, 
                                CASE WHEN @acc = '1001' THEN b.Credit ELSE 0 END AS Debit, 
                                CASE WHEN @acc = '1001' THEN 0 ELSE b.Credit END AS Credit 
                            FROM CreditVouchersBody b
                            WHERE (@acc = '1001' OR b.AccountNo = @acc) AND b.VoucherDate BETWEEN @dtFrom AND @dtTo

                            UNION ALL

                            SELECT 
                                b.VoucherDate, 
                                'CPV' AS VoucherType, 
                                CAST(b.VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                                ISNULL(b.Narration, '') AS Narration, 
                                CASE WHEN @acc = '1001' THEN 0 ELSE b.Debit END AS Debit, 
                                CASE WHEN @acc = '1001' THEN b.Debit ELSE 0 END AS Credit 
                            FROM DebitVouchersBody b
                            WHERE (@acc = '1001' OR b.AccountNo = @acc) AND b.VoucherDate BETWEEN @dtFrom AND @dtTo

                            UNION ALL

                            SELECT 
                                b.VoucherDate, 
                                'JV' AS VoucherType, 
                                CAST(b.VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                                ISNULL(b.Narration, '') AS Narration, 
                                b.Debit, 
                                b.Credit 
                            FROM JournalVouchersBody b
                            WHERE b.AccountNo = @acc AND b.VoucherDate BETWEEN @dtFrom AND @dtTo

                            UNION ALL

                            SELECT 
                                VoucherDate, 
                                'BPV' AS VoucherType, 
                                CAST(VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                                ISNULL(Narration, '') AS Narration, 
                                0 AS Debit, 
                                Amount AS Credit 
                            FROM BankCheques
                            WHERE (BankAccountNo = @acc OR AccountNo = @acc) AND VoucherDate BETWEEN @dtFrom AND @dtTo

                            UNION ALL

                            SELECT 
                                VoucherDate, 
                                'BRV' AS VoucherType, 
                                CAST(VoucherNo AS NVARCHAR(50)) AS VoucherNo, 
                                ISNULL(Narration, '') AS Narration, 
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
                    while (await rdrTx.ReadAsync())
                    {
                        DateTime d = rdrTx.IsDBNull(0) ? DateTime.Today : rdrTx.GetDateTime(0);
                        string vType = rdrTx.IsDBNull(1) ? "" : rdrTx.GetString(1);
                        string vNo = rdrTx.IsDBNull(2) ? "" : rdrTx.GetValue(2).ToString() ?? "";
                        string narr = rdrTx.IsDBNull(3) ? "" : rdrTx.GetString(3);
                        decimal dr = rdrTx.IsDBNull(4) ? 0 : Convert.ToDecimal(rdrTx.GetValue(4));
                        decimal cr = rdrTx.IsDBNull(5) ? 0 : Convert.ToDecimal(rdrTx.GetValue(5));
                        runningBalance += (dr - cr);

                        string docRef = string.IsNullOrWhiteSpace(vNo) || vNo == "-" ? vType : $"{vType} #{vNo}";

                        reportData.Add(new LedgerReportItem
                        {
                            VoucherNo = docRef,
                            TransactionDate = d,
                            Narration = narr,
                            Debit = dr,
                            Credit = cr,
                            Balance = FormatBalance(runningBalance)
                        });
                    }
                }
            }
            catch { }
            return reportData;
        }
    }

    public async Task<IEnumerable<dynamic>> GetDayBookReportAsync(DateTime date)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        bool isAllDates = date >= new DateTime(2900, 1, 1);
        var query = context.GeneralLedger.Include(g => g.Account).AsQueryable();

        if (!isAllDates)
        {
            var targetDate = date.Date;
            var nextDate = targetDate.AddDays(1);
            query = query.Where(g => g.TransactionDate >= targetDate && g.TransactionDate < nextDate);
        }

        return await query
            .OrderBy(g => g.Id)
            .Select(g => new
            {
                g.TransactionDate,
                AccountName = g.Account.Name,
                g.Narration,
                g.Debit,
                g.Credit,
                Reference = g.SourceType.ToString() + "-" + g.SourceId
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetStockReportAsync(int? productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.StockTransactions.Include(s => s.Product).AsQueryable();
        
        if (productId.HasValue)
            query = query.Where(s => s.ProductId == productId.Value);

        return await query
            .GroupBy(s => new { s.ProductId, s.Product.Name, s.Product.ProductCode })
            .Select(g => new
            {
                g.Key.ProductCode,
                g.Key.Name,
                TotalIn = g.Sum(x => x.QtyIn),
                TotalOut = g.Sum(x => x.QtyOut),
                CurrentStock = g.Sum(x => x.QtyIn) - g.Sum(x => x.QtyOut)
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<dynamic>> GetPurchaseRegisterAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Invoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && (i.InvoiceType == InvoiceType.GenPurchase || i.InvoiceType == InvoiceType.GenPurchaseReturn))
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new
            {
                i.InvoiceNo,
                i.InvoiceDate,
                PartyName = i.Party.Name,
                i.SubTotal,
                i.TaxAmount,
                i.NetAmount,
                i.Status
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetSaleRegisterAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Invoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && (i.InvoiceType == InvoiceType.GenSale || i.InvoiceType == InvoiceType.GenSaleReturn))
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new
            {
                i.InvoiceNo,
                i.InvoiceDate,
                PartyName = i.Party.Name,
                i.SubTotal,
                i.TaxAmount,
                i.NetAmount,
                i.Status
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetBrokerageCommissionAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BrokerageInvoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.IsPosted)
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new
            {
                i.InvoiceNo,
                i.InvoiceDate,
                PartyName = i.Party.Name,
                i.GrossAmount,
                i.CommissionAmount,
                i.Soodh_Amount,
                i.LaborCharges,
                i.NetAmount
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetGatePassRegisterAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GatePasses
            .Include(g => g.Party)
            .Include(g => g.Product)
            .Where(g => g.GatePassDate >= from && g.GatePassDate <= to)
            .OrderBy(g => g.GatePassDate)
            .Select(g => new
            {
                g.GatePassNo,
                g.GatePassDate,
                PartyName = g.Party.Name,
                ProductName = g.Product.Name,
                g.GrossWeight,
                g.NetWeight,
                g.VehicleNo
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetPestroExpiryReportAsync(int daysToExpiry)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var thresholdDate = DateTime.Today.AddDays(daysToExpiry);
        return await context.PestroBatches
            .Include(b => b.Product)
            .Where(b => b.ExpiryDate != null && b.ExpiryDate <= thresholdDate)
            .OrderBy(b => b.ExpiryDate)
            .Select(b => new
            {
                b.BatchNo,
                ProductName = b.Product.Name,
                b.ManufactureDate,
                b.ExpiryDate,
                DaysRemaining = b.ExpiryDate.HasValue ? (b.ExpiryDate.Value - DateTime.Today).TotalDays : 0
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetProfitabilityReportAsync(DateTime from, DateTime to, string type)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var safeFrom = from < new DateTime(1900, 1, 1) ? new DateTime(1900, 1, 1) : from;
        var safeTo = to > new DateTime(2900, 1, 1) ? new DateTime(2900, 1, 1) : to;

        // This is a simplified calculation of Gross Profit
        var salesTotal = await context.Invoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.InvoiceType == InvoiceType.GenSale)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0;
            
        var purchasesTotal = await context.Invoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.InvoiceType == InvoiceType.GenPurchase)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0;
            
        var brkCommission = await context.BrokerageInvoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.IsPosted)
            .SumAsync(i => (decimal?)(i.CommissionAmount + i.Soodh_Amount)) ?? 0;

        // Return as a single row for report summary or grouped by type
        return new List<dynamic> { new {
            Sales = salesTotal,
            Purchases = purchasesTotal,
            Commissions = brkCommission,
            GrossProfit = (salesTotal + brkCommission) - purchasesTotal
        }};
    }

    // =========================================================================
    // 1 & 2. ACCOUNT PAYABLES & RECEIVABLES (Implemented in ReportService.AccountPayables.cs)
    // =========================================================================
    public async Task<IEnumerable<dynamic>> GetAccountPayablesReportAsync(
        DateTime? from, DateTime? to, string partyType, string partyGroup, string city, string town,
        string sector, decimal minAmount, bool showOnlyFinalBalance, bool excludeZeroBalance, bool sortByAccountName)
        => await GetLegacyPartyBalancesAsync(false, from, to, partyType, partyGroup, city, town, sector, minAmount, showOnlyFinalBalance, sortByAccountName);

    public async Task<IEnumerable<dynamic>> GetAccountReceivablesReportAsync(
        DateTime? from, DateTime? to, string partyType, string partyGroup, string city, string town,
        string sector, decimal minAmount, bool showOnlyFinalBalance, bool excludeZeroBalance, bool sortByAccountName)
        => await GetLegacyPartyBalancesAsync(true, from, to, partyType, partyGroup, city, town, sector, minAmount, showOnlyFinalBalance, sortByAccountName);

    // =========================================================================
    // 3. BANK STATEMENT (CrpBankStatement / AgenSoft Specification)
    // =========================================================================
    public async Task<IEnumerable<dynamic>> GetBankStatementReportAsync(int bankAccountId, DateTime? from, DateTime? to, bool includeOtherVouchers)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var bankAccount = await context.Accounts.FindAsync(bankAccountId);
        decimal baseOpening = bankAccount?.OpeningBalance ?? 0m;

        bool isFromBounded = from.HasValue && from.Value > new DateTime(1900, 1, 1);
        bool isToBounded = to.HasValue && to.Value < new DateTime(2900, 1, 1);

        DateTime? startDate = isFromBounded ? from!.Value.Date : null;
        DateTime? endDate = isToBounded ? to!.Value : null;

        decimal openingBalance = baseOpening;

        if (isFromBounded)
        {
            var priorDebit = await context.GeneralLedger
                .Where(g => g.AccountId == bankAccountId && g.TransactionDate < startDate!.Value)
                .SumAsync(g => (decimal?)g.Debit) ?? 0m;

            var priorCredit = await context.GeneralLedger
                .Where(g => g.AccountId == bankAccountId && g.TransactionDate < startDate!.Value)
                .SumAsync(g => (decimal?)g.Credit) ?? 0m;

            openingBalance += (priorDebit - priorCredit);
        }

        var query = context.GeneralLedger
            .Include(g => g.Account)
            .Where(g => g.AccountId == bankAccountId);

        if (isFromBounded) query = query.Where(g => g.TransactionDate >= startDate!.Value);
        if (isToBounded) query = query.Where(g => g.TransactionDate <= endDate!.Value);

        var rawTransactions = await query
            .OrderBy(g => g.TransactionDate)
            .ThenBy(g => g.Id)
            .ToListAsync();

        var reportItems = new List<dynamic>();
        decimal runningBalance = openingBalance;

        // Opening Balance Row
        reportItems.Add(new
        {
            VoucherNo = "OPEN",
            TransactionDate = isFromBounded ? startDate!.Value : DateTime.Today,
            ChequeNo = "-",
            ChequeDate = (DateTime?)null,
            Narration = "Opening Balance / سابقہ اوپننگ بیلنس",
            Debit = openingBalance > 0 ? openingBalance : 0m,
            Credit = openingBalance < 0 ? Math.Abs(openingBalance) : 0m,
            Balance = runningBalance >= 0 ? $"{runningBalance:N0} Dr" : $"{Math.Abs(runningBalance):N0} Cr"
        });

        foreach (var t in rawTransactions)
        {
            runningBalance += (t.Debit - t.Credit);

            string vNo = $"{t.SourceType}-{t.SourceId}";
            string chqNo = "-";
            DateTime? chqDate = null;

            // Try to extract Cheque details if narration contains CHQ #
            if (!string.IsNullOrWhiteSpace(t.Narration) && t.Narration.Contains("CHQ #"))
            {
                int chqIdx = t.Narration.IndexOf("CHQ #", StringComparison.OrdinalIgnoreCase);
                if (chqIdx >= 0)
                {
                    var chqPart = t.Narration.Substring(chqIdx + 5).Trim();
                    int commaIdx = chqPart.IndexOfAny(new[] { ',', ' ', '-' });
                    chqNo = commaIdx > 0 ? chqPart.Substring(0, commaIdx) : chqPart;
                }
            }

            reportItems.Add(new
            {
                VoucherNo = vNo,
                TransactionDate = t.TransactionDate,
                ChequeNo = chqNo,
                ChequeDate = chqDate,
                Narration = t.Narration ?? "",
                Debit = t.Debit,
                Credit = t.Credit,
                Balance = runningBalance >= 0 ? $"{runningBalance:N0} Dr" : $"{Math.Abs(runningBalance):N0} Cr"
            });
        }

        return reportItems;
    }

    // =========================================================================
    // 4. CASH BOOK (SPAgenTechCashBook / AgenSoft Specification)
    // =========================================================================
    public async Task<IEnumerable<dynamic>> GetCashBookReportAsync(DateTime? from, DateTime? to, bool isDefaultCashBookView)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // Dynamically find Cash Account
        var cashAccount = await context.Accounts
            .Include(a => a.AccountGroup)
            .FirstOrDefaultAsync(a => a.AccountNo == "111" || 
                                      a.Name.Contains("Cash In Hand") || 
                                      a.Name.Contains("Cash Book") || 
                                      (a.AccountGroup != null && a.AccountGroup.Name.Contains("Cash")))
            ?? await context.Accounts.FirstOrDefaultAsync();

        if (cashAccount == null) return new List<dynamic>();

        int cashAccountId = cashAccount.Id;
        decimal baseOpening = cashAccount.OpeningBalance;

        bool isFromBounded = from.HasValue && from.Value > new DateTime(1900, 1, 1);
        bool isToBounded = to.HasValue && to.Value < new DateTime(2900, 1, 1);

        DateTime? startDate = isFromBounded ? from!.Value.Date : null;
        DateTime? endDate = isToBounded ? to!.Value : null;

        decimal openingBalance = baseOpening;

        if (isFromBounded)
        {
            var priorDebit = await context.GeneralLedger
                .Where(g => g.AccountId == cashAccountId && g.TransactionDate < startDate!.Value)
                .SumAsync(g => (decimal?)g.Debit) ?? 0m;

            var priorCredit = await context.GeneralLedger
                .Where(g => g.AccountId == cashAccountId && g.TransactionDate < startDate!.Value)
                .SumAsync(g => (decimal?)g.Credit) ?? 0m;

            openingBalance += (priorDebit - priorCredit);
        }

        var query = context.GeneralLedger
            .Where(g => g.AccountId == cashAccountId);

        if (isFromBounded) query = query.Where(g => g.TransactionDate >= startDate!.Value);
        if (isToBounded) query = query.Where(g => g.TransactionDate <= endDate!.Value);

        var transactions = await query
            .OrderBy(g => g.TransactionDate)
            .ThenBy(g => g.Id)
            .ToListAsync();

        var reportItems = new List<dynamic>();
        decimal runningBalance = openingBalance;

        // Opening Balance Row
        reportItems.Add(new
        {
            TransactionDate = isFromBounded ? startDate!.Value : DateTime.Today,
            VoucherNo = "OPEN",
            VoucherType = "-",
            AccountName = "Opening Balance",
            Narration = "سابقہ اوپننگ کیش ان ہینڈ",
            Debit = openingBalance > 0 ? openingBalance : 0m,
            Credit = openingBalance < 0 ? Math.Abs(openingBalance) : 0m,
            Balance = runningBalance
        });

        foreach (var t in transactions)
        {
            runningBalance += (t.Debit - t.Credit);

            string vType = t.Debit > 0 ? "CRV / وصولی" : "CPV / ادائیگی";
            string vNo = $"{t.SourceType}-{t.SourceId}";

            reportItems.Add(new
            {
                TransactionDate = t.TransactionDate,
                VoucherNo = vNo,
                VoucherType = vType,
                AccountName = cashAccount.Name,
                Narration = t.Narration ?? "",
                Debit = t.Debit,
                Credit = t.Credit,
                Balance = runningBalance
            });
        }

        return reportItems;
    }

    // =========================================================================
    // 5. DAILY VOCHURE DETAILS (CrpDailyVoucher / AgenSoft Specification)
    // =========================================================================
    public async Task<IEnumerable<dynamic>> GetDailyVouchersDetailReportAsync(
        DateTime? from, DateTime? to, string voucherType, int? fromVoucherNo, int? toVoucherNo, 
        bool excludeAccountsVouchers, bool includeInvoices)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        bool isFromBounded = from.HasValue && from.Value > new DateTime(1900, 1, 1);
        bool isToBounded = to.HasValue && to.Value < new DateTime(2900, 1, 1);

        DateTime? startDate = isFromBounded ? from!.Value.Date : null;
        DateTime? endDate = isToBounded ? to!.Value : null;

        var vQuery = context.VoucherDetails
            .Include(vd => vd.Voucher)
            .Include(vd => vd.Account)
            .AsQueryable();

        if (isFromBounded) vQuery = vQuery.Where(vd => vd.Voucher.VoucherDate >= startDate!.Value);
        if (isToBounded) vQuery = vQuery.Where(vd => vd.Voucher.VoucherDate <= endDate!.Value);

        var voucherDetails = await vQuery
            .OrderBy(vd => vd.Voucher.VoucherDate)
            .ThenBy(vd => vd.Voucher.VoucherNo)
            .ToListAsync();

        var reportItems = new List<dynamic>();

        foreach (var vd in voucherDetails)
        {
            string typeName = vd.Voucher.VoucherType switch
            {
                MyMandiSystem.Core.Enums.VoucherType.CashReceiving => "Cash Receiving Voucher (CRV)",
                MyMandiSystem.Core.Enums.VoucherType.CashPayment => "Cash Payment Voucher (CPV)",
                MyMandiSystem.Core.Enums.VoucherType.JournalVoucher => "Journal Voucher (JV)",
                MyMandiSystem.Core.Enums.VoucherType.BankReceipt => "Bank Deposit (BD)",
                MyMandiSystem.Core.Enums.VoucherType.BankPayment => "Bank Issue (BC)",
                _ => vd.Voucher.VoucherType.ToString()
            };

            // Check VoucherType filter
            if (!string.IsNullOrWhiteSpace(voucherType) && voucherType != "--ALL TYPES--")
            {
                if (!typeName.Contains(voucherType, StringComparison.OrdinalIgnoreCase) && 
                    !voucherType.Contains(vd.Voucher.VoucherType.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            // Exclude Accounts Vouchers (CV, DV, JV)
            if (excludeAccountsVouchers && (vd.Voucher.VoucherType == MyMandiSystem.Core.Enums.VoucherType.CashReceiving || 
                                           vd.Voucher.VoucherType == MyMandiSystem.Core.Enums.VoucherType.CashPayment || 
                                           vd.Voucher.VoucherType == MyMandiSystem.Core.Enums.VoucherType.JournalVoucher))
            {
                continue;
            }

            // Check VoucherNo bounds
            if (fromVoucherNo.HasValue || toVoucherNo.HasValue)
            {
                if (int.TryParse(vd.Voucher.VoucherNo, out int vNum))
                {
                    if (fromVoucherNo.HasValue && vNum < fromVoucherNo.Value) continue;
                    if (toVoucherNo.HasValue && vNum > toVoucherNo.Value) continue;
                }
            }

            reportItems.Add(new
            {
                VoucherDate = vd.Voucher.VoucherDate,
                VoucherNo = vd.Voucher.VoucherNo,
                Type = typeName,
                AccountNo = vd.Account?.AccountNo ?? "",
                AccountName = vd.Account?.Name ?? "",
                Narration = !string.IsNullOrWhiteSpace(vd.Narration) ? vd.Narration : (vd.Voucher.NarrationEnglish ?? ""),
                Debit = vd.Debit,
                Credit = vd.Credit
            });
        }

        // Include Invoices if selected
        if (includeInvoices)
        {
            var invQuery = context.Invoices
                .Include(i => i.Party)
                .AsQueryable();

            if (isFromBounded) invQuery = invQuery.Where(i => i.InvoiceDate >= startDate!.Value);
            if (isToBounded) invQuery = invQuery.Where(i => i.InvoiceDate <= endDate!.Value);

            var invoices = await invQuery.ToListAsync();
            foreach (var inv in invoices)
            {
                string invType = inv.InvoiceType switch
                {
                    InvoiceType.GenPurchase => "General Purchase (PI)",
                    InvoiceType.GenPurchaseReturn => "General Purchase Return (PR)",
                    InvoiceType.GenSale => "General Sale (SI)",
                    InvoiceType.GenSaleReturn => "General Sale Return (SR)",
                    _ => inv.InvoiceType.ToString()
                };

                reportItems.Add(new
                {
                    VoucherDate = inv.InvoiceDate,
                    VoucherNo = inv.InvoiceNo,
                    Type = invType,
                    AccountNo = inv.Party?.PartyNo ?? "",
                    AccountName = inv.Party?.Name ?? "",
                    Narration = $"Invoice #{inv.InvoiceNo}",
                    Debit = inv.InvoiceType == InvoiceType.GenSale ? inv.NetAmount : 0m,
                    Credit = inv.InvoiceType == InvoiceType.GenPurchase ? inv.NetAmount : 0m
                });
            }
        }

        return reportItems.OrderBy(x => (DateTime)x.VoucherDate).ToList();
    }

    // =========================================================================
    // 6. TRAIL BALANCE (CrpTrialBalanceDetail / SPAccAccountsBalancesFinal Specification)
    // =========================================================================
    public async Task<IEnumerable<dynamic>> GetTrialBalanceReportAsync(
        DateTime? from, DateTime? to, bool showTransactionAccountsOnly, bool showGroupsOnly, 
        bool excludeZeroBalance, decimal minAmount, int partiesScope, bool sortByAccountName)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var accounts = await context.Accounts
            .Include(a => a.AccountGroup)
            .ToListAsync();

        bool isFromBounded = from.HasValue && from.Value > new DateTime(1900, 1, 1);
        bool isToBounded = to.HasValue && to.Value < new DateTime(2900, 1, 1);

        DateTime? startDate = isFromBounded ? from!.Value.Date : null;
        DateTime? endDate = isToBounded ? to!.Value : null;

        // Prior debits & credits
        var priorDebits = new Dictionary<int, decimal>();
        var priorCredits = new Dictionary<int, decimal>();
        if (isFromBounded)
        {
            priorDebits = await context.GeneralLedger
                .Where(g => g.TransactionDate < startDate!.Value)
                .GroupBy(g => g.AccountId)
                .Select(g => new { AccountId = g.Key, Total = g.Sum(x => x.Debit) })
                .ToDictionaryAsync(x => x.AccountId, x => x.Total);

            priorCredits = await context.GeneralLedger
                .Where(g => g.TransactionDate < startDate!.Value)
                .GroupBy(g => g.AccountId)
                .Select(g => new { AccountId = g.Key, Total = g.Sum(x => x.Credit) })
                .ToDictionaryAsync(x => x.AccountId, x => x.Total);
        }

        // Period debits & credits
        var periodQuery = context.GeneralLedger.AsQueryable();
        if (isFromBounded) periodQuery = periodQuery.Where(g => g.TransactionDate >= startDate!.Value);
        if (isToBounded) periodQuery = periodQuery.Where(g => g.TransactionDate <= endDate!.Value);

        var periodDebits = await periodQuery
            .GroupBy(g => g.AccountId)
            .Select(g => new { AccountId = g.Key, Total = g.Sum(x => x.Debit) })
            .ToDictionaryAsync(x => x.AccountId, x => x.Total);

        var periodCredits = await periodQuery
            .GroupBy(g => g.AccountId)
            .Select(g => new { AccountId = g.Key, Total = g.Sum(x => x.Credit) })
            .ToDictionaryAsync(x => x.AccountId, x => x.Total);

        var reportItems = new List<dynamic>();

        foreach (var a in accounts)
        {
            // Scope filter: Transaction vs Groups
            if (showTransactionAccountsOnly && !a.IsActive) continue;
            if (showGroupsOnly && a.IsActive) continue;

            // Parties Scope
            bool isPartyAccount = a.AccountGroup?.Name?.Contains("Customer", StringComparison.OrdinalIgnoreCase) == true ||
                                  a.AccountGroup?.Name?.Contains("Vendor", StringComparison.OrdinalIgnoreCase) == true ||
                                  a.AccountGroup?.Name?.Contains("Party", StringComparison.OrdinalIgnoreCase) == true;

            if (partiesScope == 1 && isPartyAccount) continue; // Exclude Parties
            if (partiesScope == 2 && !isPartyAccount) continue; // Only Parties

            decimal baseOp = a.OpeningBalance;
            decimal priorDr = priorDebits.TryGetValue(a.Id, out var pdr) ? pdr : 0m;
            decimal priorCr = priorCredits.TryGetValue(a.Id, out var pcr) ? pcr : 0m;

            decimal netOpening = isFromBounded ? (baseOp + (priorDr - priorCr)) : baseOp;
            decimal opDr = netOpening > 0 ? netOpening : 0m;
            decimal opCr = netOpening < 0 ? Math.Abs(netOpening) : 0m;

            decimal perDr = periodDebits.TryGetValue(a.Id, out var pddr) ? pddr : 0m;
            decimal perCr = periodCredits.TryGetValue(a.Id, out var pdcr) ? pdcr : 0m;

            decimal closingDr = opDr + perDr;
            decimal closingCr = opCr + perCr;

            decimal netClosing = (opDr - opCr) + (perDr - perCr);

            // Exclude zero balance
            if (excludeZeroBalance && closingDr == 0m && closingCr == 0m && perDr == 0m && perCr == 0m)
            {
                continue;
            }

            // Min Amount filter
            if (minAmount > 0m && Math.Abs(netClosing) < minAmount)
            {
                continue;
            }

            reportItems.Add(new
            {
                AccountNo = a.AccountNo,
                AccountName = a.Name,
                OpeningDebit = opDr,
                OpeningCredit = opCr,
                Debit = perDr,
                Credit = perCr,
                ClosingDebit = netClosing > 0 ? netClosing : 0m,
                ClosingCredit = netClosing < 0 ? Math.Abs(netClosing) : 0m
            });
        }

        if (sortByAccountName)
        {
            return reportItems.OrderBy(x => (string)x.AccountName).ToList();
        }
        else
        {
            return reportItems.OrderBy(x => (string)x.AccountNo).ToList();
        }
    }

    // =========================================================================
    // 7. PROFIT & LOSS STATEMENTS (CrpPLStatement / SPAccAccountsBalancesFinal4PLS Specification)
    // =========================================================================
    public async Task<IEnumerable<dynamic>> GetProfitLossStatementReportAsync(
        DateTime? from, DateTime? to, bool withoutOpening, bool includeStock, bool includeStockCrops, bool includeStockPestro)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var safeFrom = from.HasValue && from.Value > new DateTime(1900, 1, 1) ? from.Value.Date : new DateTime(1900, 1, 1);
        var safeTo = to.HasValue && to.Value < new DateTime(2900, 1, 1) ? to.Value : DateTime.Today;

        // 1. REVENUES & INCOMES
        var genSales = await context.Invoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.InvoiceType == InvoiceType.GenSale)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0m;

        var brkCommission = await context.BrokerageInvoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.IsPosted)
            .SumAsync(i => (decimal?)(i.CommissionAmount + i.Soodh_Amount)) ?? 0m;

        var pestroSales = await context.PestroInvoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0m;

        decimal totalTradingRevenue = genSales + brkCommission + pestroSales;

        // 2. DIRECT PURCHASES & COST OF SALES
        var genPurchases = await context.Invoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.InvoiceType == InvoiceType.GenPurchase)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0m;

        // Closing Stock Value if requested
        decimal closingStockVal = 0m;
        if (includeStock)
        {
            closingStockVal = await context.Products
                .SumAsync(p => (decimal?)(p.CurrentStock * p.PurchasePrice)) ?? 0m;
        }

        decimal grossProfit = totalTradingRevenue + closingStockVal - genPurchases;

        // 3. OPERATING EXPENSES
        var expenseAccounts = await context.Accounts
            .Include(a => a.AccountGroup)
            .Where(a => a.AccountType == AccountType.Expense || 
                        (a.AccountGroup != null && a.AccountGroup.GroupType == AccountType.Expense) ||
                        a.AccountNo.StartsWith("5"))
            .ToListAsync();

        var expAccountIds = expenseAccounts.Select(a => a.Id).ToList();

        var expenseLedgers = await context.GeneralLedger
            .Where(g => expAccountIds.Contains(g.AccountId) && g.TransactionDate >= safeFrom && g.TransactionDate <= safeTo)
            .GroupBy(g => g.AccountId)
            .Select(g => new { AccountId = g.Key, NetExpense = g.Sum(x => x.Debit - x.Credit) })
            .ToListAsync();

        var expMap = expenseLedgers.ToDictionary(x => x.AccountId, x => x.NetExpense);
        decimal totalExpenses = 0m;

        var reportItems = new List<dynamic>();

        // [A] Direct Incomes
        reportItems.Add(new { Category = "1. DIRECT INCOME (براہ راست آمدنی)", AccountName = "General Sales (عام فروخت)", Debit = 0m, Credit = genSales, Amount = genSales });
        reportItems.Add(new { Category = "1. DIRECT INCOME (براہ راست آمدنی)", AccountName = "Brokerage Commission & Soodh (کمیشن و سودھ آڑھت)", Debit = 0m, Credit = brkCommission, Amount = brkCommission });
        if (pestroSales > 0)
        {
            reportItems.Add(new { Category = "1. DIRECT INCOME (براہ راست آمدنی)", AccountName = "Pestro Sales (پیسٹرو ادویات سیل)", Debit = 0m, Credit = pestroSales, Amount = pestroSales });
        }
        if (includeStock && closingStockVal > 0)
        {
            reportItems.Add(new { Category = "1. DIRECT INCOME (براہ راست آمدنی)", AccountName = "Closing Stock Value (موجودہ کل مالیت سٹاک)", Debit = 0m, Credit = closingStockVal, Amount = closingStockVal });
        }

        // [B] Cost of Goods
        reportItems.Add(new { Category = "2. COST OF SALES (لاگت خریداری و مال)", AccountName = "Purchases (خریداری مال)", Debit = genPurchases, Credit = 0m, Amount = genPurchases });
        reportItems.Add(new { Category = "3. GROSS PROFIT (خام منافع)", AccountName = "TOTAL GROSS PROFIT (کل خام منافع)", Debit = 0m, Credit = grossProfit, Amount = grossProfit });

        // [C] Operating Expenses
        foreach (var expAcc in expenseAccounts)
        {
            decimal expAmt = expMap.TryGetValue(expAcc.Id, out var amt) ? amt : 0m;
            if (expAmt != 0m)
            {
                totalExpenses += expAmt;
                reportItems.Add(new { Category = "4. OPERATING EXPENSES (کاروباری اخراجات)", AccountName = expAcc.Name, Debit = expAmt, Credit = 0m, Amount = expAmt });
            }
        }

        // [D] Net Profit
        decimal netProfit = grossProfit - totalExpenses;
        reportItems.Add(new { Category = "5. NET PROFIT / LOSS (خالص منافع / نقصان)", AccountName = netProfit >= 0 ? "NET PROFIT (خالص منافع)" : "NET LOSS (خالص نقصان)", Debit = netProfit < 0 ? Math.Abs(netProfit) : 0m, Credit = netProfit >= 0 ? netProfit : 0m, Amount = netProfit });

        return reportItems;
    }

    public async Task<IEnumerable<dynamic>> GetMasterListAsync(string entityName)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (entityName == "Parties") return await context.Parties.Select(p => new { Name = p.Name, Phone = p.Phone, Type = p.PartyType.ToString() }).ToListAsync();
        if (entityName == "Products") return await context.Products.Select(p => new { Name = p.Name, Price = p.SalePrice, Stock = p.CurrentStock }).ToListAsync();
        return new List<dynamic>();
    }
}
