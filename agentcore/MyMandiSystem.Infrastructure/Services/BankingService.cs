using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class BankingService : IBankingService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    public BankingService(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
    }

    public async Task<IEnumerable<BankAccount>> GetAllBankAccountsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BankAccounts
            .Include(b => b.Account)
            .Include(b => b.ChequeBooks)
            .Where(b => !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<BankAccount?> GetBankAccountByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BankAccounts
            .Include(b => b.Account)
            .Include(b => b.ChequeBooks)
                .ThenInclude(cb => cb.Cheques)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddBankAccountAsync(BankAccount bankAccount)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.BankAccounts.Add(bankAccount);
        await context.SaveChangesAsync();
    }

    public async Task UpdateBankAccountAsync(BankAccount bankAccount)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.BankAccounts.Update(bankAccount);
        await context.SaveChangesAsync();
    }

    public async Task<BankAccount> CreateBankAccountWithGlAccountAsync(string bankName, string branchName, string accountNumber, decimal openingBalance, string? glAccountNo = null)
    {
        if (string.IsNullOrWhiteSpace(bankName))
            throw new ArgumentException("Bank Name is required.", nameof(bankName));

        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account Number is required.", nameof(accountNumber));

        using var context = await _contextFactory.CreateDbContextAsync();

        // 1. Get or create AccountGroup for Bank Accounts
        var bankGroup = await context.AccountGroups
            .FirstOrDefaultAsync(g => !g.IsDeleted && (g.Name.Contains("Bank") || g.Id == 1));

        if (bankGroup == null)
        {
            bankGroup = new AccountGroup
            {
                Name = "Bank Accounts",
                GroupType = AccountType.Asset,
                CreatedAt = DateTime.Now
            };
            context.AccountGroups.Add(bankGroup);
            await context.SaveChangesAsync();
        }

        // 2. Determine GL AccountNo if not provided
        string acNo = glAccountNo?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(acNo))
        {
            var existingCodes = await context.Accounts
                .Where(a => a.AccountNo.Length == 4 && a.AccountNo.StartsWith("10"))
                .Select(a => a.AccountNo)
                .ToListAsync();

            int nextNum = 1002;
            while (existingCodes.Contains(nextNum.ToString()) && nextNum < 9999)
            {
                nextNum++;
            }
            acNo = nextNum.ToString();
        }

        // 3. Create GL Account
        var glAccount = new Account
        {
            AccountNo = acNo,
            Name = $"{bankName} ({branchName})".Trim(),
            AccountType = AccountType.Asset,
            AccountGroupId = bankGroup.Id,
            OpeningBalance = openingBalance,
            CurrentBalance = openingBalance,
            IsSystemAccount = false,
            IsActive = true,
            BalFlag = true,
            CreatedAt = DateTime.Now
        };
        context.Accounts.Add(glAccount);
        await context.SaveChangesAsync();

        // 4. Sync to ChartOfAccounts (AgenSoft Parity)
        try
        {
            var sql = @"
                IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM ChartOfAccounts WHERE AccountNo = {0})
                    BEGIN
                        INSERT INTO ChartOfAccounts (
                            AccountNo, UserID, AccountType, OpeningDebit, OpeningCredit, 
                            AdjustedDebit, AdjustedCredit, IsDetailed, IsLocked, IsPosted, 
                            IsEditable, BalFlag, PLFlag, ExpFlag, AccountName, AccountNameUrdu
                        )
                        VALUES (
                            {0}, 1, 'D', {1}, {2}, 
                            0, 0, 1, 0, 1, 
                            1, 1, 'B', 0, {3}, {3}
                        );
                    END
                END";
            decimal opDebit = openingBalance > 0 ? openingBalance : 0m;
            decimal opCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0m;
            await context.Database.ExecuteSqlRawAsync(sql, acNo, opDebit, opCredit, glAccount.Name);
        }
        catch { }

        // 5. Create BankAccount entity
        var bankAccount = new BankAccount
        {
            AccountId = glAccount.Id,
            BankName = bankName.Trim(),
            BranchName = branchName?.Trim() ?? string.Empty,
            AccountNumber = accountNumber.Trim(),
            CurrentBalance = openingBalance,
            CreatedAt = DateTime.Now
        };
        context.BankAccounts.Add(bankAccount);
        await context.SaveChangesAsync();

        bankAccount.Account = glAccount;
        return bankAccount;
    }

    public async Task<IEnumerable<ChequeBook>> GetChequeBooksAsync(int bankAccountId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ChequeBooks
            .Include(cb => cb.Cheques)
            .Where(cb => cb.BankAccountId == bankAccountId && !cb.IsDeleted)
            .OrderByDescending(cb => cb.Id)
            .ToListAsync();
    }

    public async Task<ChequeBook> GenerateChequeBookAsync(int bankAccountId, int seriesStart, int seriesEnd)
    {
        if (seriesEnd < seriesStart)
            throw new ArgumentException("Series End must be greater than or equal to Series Start.");

        using var context = await _contextFactory.CreateDbContextAsync();

        var chequeBook = new ChequeBook
        {
            BankAccountId = bankAccountId,
            SeriesStart = seriesStart,
            SeriesEnd = seriesEnd,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        var bankAccount = await context.BankAccounts.Include(b => b.Account).FirstOrDefaultAsync(b => b.Id == bankAccountId);
        string bankAcNo = bankAccount?.Account?.AccountNo ?? bankAccount?.AccountNumber ?? string.Empty;

        for (int num = seriesStart; num <= seriesEnd; num++)
        {
            var chqNo = num.ToString("D6");
            chequeBook.Cheques.Add(new Cheque
            {
                ChequeNo = chqNo,
                Status = ChequeStatus.Blank,
                Amount = 0,
                CreatedAt = DateTime.Now
            });

            // ═══ AGENSOFT EXACT DATABASE REPLICATION (BankChequeBookHistory) ═══
            if (!string.IsNullOrEmpty(bankAcNo))
            {
                try
                {
                    await context.Database.ExecuteSqlRawAsync(@"
                        IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                        BEGIN
                            IF NOT EXISTS (SELECT 1 FROM BankChequeBookHistory WHERE AccountNo = {0} AND ChequeNo = {1})
                                INSERT INTO BankChequeBookHistory (AccountNo, ChequeNo, EntryDate, IsCancelled, Remarks)
                                VALUES ({0}, {1}, {2}, 0, '');
                        END", bankAcNo, chqNo, DateTime.Today);
                }
                catch { /* Ignore */ }
            }
        }

        context.ChequeBooks.Add(chequeBook);
        await context.SaveChangesAsync();
        return chequeBook;
    }

    public async Task<IEnumerable<Cheque>> GetAvailableChequesAsync(int bankAccountId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Cheques
            .Include(c => c.ChequeBook)
            .Where(c => c.ChequeBook.BankAccountId == bankAccountId && c.Status == ChequeStatus.Blank && !c.IsDeleted)
            .OrderBy(c => c.ChequeNo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cheque>> GetUnclearedChequesAsync(int? bankAccountId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Cheques
            .Include(c => c.ChequeBook)
                .ThenInclude(cb => cb.BankAccount)
                    .ThenInclude(b => b.Account)
            .Include(c => c.IssuedToParty)
            .Where(c => c.Status == ChequeStatus.Issued && !c.IsDeleted);

        if (bankAccountId.HasValue && bankAccountId.Value > 0)
        {
            query = query.Where(c => c.ChequeBook.BankAccountId == bankAccountId.Value);
        }

        return await query.OrderBy(c => c.IssueDate).ToListAsync();
    }

    public async Task<IEnumerable<Cheque>> GetChequesByStatusAsync(ChequeStatus status, int? bankAccountId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Cheques
            .Include(c => c.ChequeBook)
                .ThenInclude(cb => cb.BankAccount)
                    .ThenInclude(b => b.Account)
            .Include(c => c.IssuedToParty)
            .Where(c => c.Status == status && !c.IsDeleted);

        if (bankAccountId.HasValue && bankAccountId.Value > 0)
        {
            query = query.Where(c => c.ChequeBook.BankAccountId == bankAccountId.Value);
        }

        return await query.OrderByDescending(c => c.IssueDate).ToListAsync();
    }

    public async Task<Cheque?> GetChequeByNoAsync(string chequeNo, int? bankAccountId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Cheques
            .Include(c => c.ChequeBook)
                .ThenInclude(cb => cb.BankAccount)
            .Include(c => c.IssuedToParty)
            .Where(c => c.ChequeNo == chequeNo.Trim());

        if (bankAccountId.HasValue && bankAccountId.Value > 0)
        {
            query = query.Where(c => c.ChequeBook.BankAccountId == bankAccountId.Value);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task IssueChequeAsync(
        int chequeId, 
        int partyAccountId, 
        decimal amount, 
        DateTime issueDate, 
        string? narration, 
        string? receivedBy, 
        int financialYearId, 
        int? cropSeasonId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var cheque = await context.Cheques
            .Include(c => c.ChequeBook)
                .ThenInclude(cb => cb.BankAccount)
            .FirstOrDefaultAsync(c => c.Id == chequeId);

        if (cheque == null)
            throw new InvalidOperationException("Cheque not found.");

        if (cheque.Status != ChequeStatus.Blank)
            throw new InvalidOperationException($"Cheque is already {cheque.Status}. Only blank cheques can be issued.");

        var party = await context.Parties.FirstOrDefaultAsync(p => p.AccountId == partyAccountId);

        cheque.Status = ChequeStatus.Issued;
        cheque.Amount = amount;
        cheque.IssueDate = issueDate;
        cheque.IssuedToPartyId = party?.Id;
        cheque.UpdatedAt = DateTime.Now;

        // Balanced GL Entries (Bank Payment Voucher):
        // 1. Debit: Party Account
        var partyGL = new GeneralLedger
        {
            TransactionDate = issueDate,
            AccountId = partyAccountId,
            Debit = amount,
            Credit = 0,
            Narration = $"Cheque Issued #{cheque.ChequeNo} - {narration} (Rcvd: {receivedBy})",
            SourceType = SourceType.Voucher,
            SourceId = cheque.Id,
            FinancialYearId = financialYearId,
            CropSeasonId = cropSeasonId
        };
        context.GeneralLedger.Add(partyGL);

        // 2. Credit: Bank Account
        var bankGL = new GeneralLedger
        {
            TransactionDate = issueDate,
            AccountId = cheque.ChequeBook.BankAccount.AccountId,
            Debit = 0,
            Credit = amount,
            Narration = $"Cheque #{cheque.ChequeNo} Issued to Party ({party?.Name ?? partyAccountId.ToString()})",
            SourceType = SourceType.Voucher,
            SourceId = cheque.Id,
            FinancialYearId = financialYearId,
            CropSeasonId = cropSeasonId
        };
        context.GeneralLedger.Add(bankGL);

        // Update Bank Balance
        cheque.ChequeBook.BankAccount.CurrentBalance -= amount;

        await context.SaveChangesAsync();

        // ═══ AGENSOFT EXACT DATABASE REPLICATION SYNC (BankCheques & BankChequeBookHistory) ═══
        await SyncLegacyChequeIssueAsync(context, cheque, partyAccountId, amount, issueDate, narration, receivedBy, cropSeasonId);
    }

    public async Task ReconcileChequeAsync(int chequeId, DateTime clearanceDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var cheque = await context.Cheques.FindAsync(chequeId);
        if (cheque != null && cheque.Status == ChequeStatus.Issued)
        {
            cheque.Status = ChequeStatus.Cleared;
            cheque.ClearanceDate = clearanceDate;
            cheque.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();

            // Sync legacy ReconcileDate
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                        UPDATE BankCheques SET ReconcileDate = {0} WHERE ChequeNo = {1};", 
                    clearanceDate.Date, cheque.ChequeNo);
            }
            catch { /* Ignore */ }
        }
    }

    public async Task BounceChequeAsync(int chequeId, string reason)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var cheque = await context.Cheques
            .Include(c => c.ChequeBook)
                .ThenInclude(cb => cb.BankAccount)
            .Include(c => c.IssuedToParty)
            .FirstOrDefaultAsync(c => c.Id == chequeId);

        if (cheque != null && cheque.Status == ChequeStatus.Issued)
        {
            cheque.Status = ChequeStatus.Bounced;
            cheque.BounceReason = reason;
            cheque.UpdatedAt = DateTime.Now;

            // Reverse GL entries if party account exists
            // Reverse GL entries if party account exists
            if (cheque.IssuedToParty != null && cheque.Amount > 0)
            {
                var curYear = await _systemService.GetCurrentYearAsync();
                int finYearId = curYear?.Id ?? 0;

                // Credit Party (reverse original debit)
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = DateTime.Today,
                    AccountId = cheque.IssuedToParty.AccountId,
                    Debit = 0,
                    Credit = cheque.Amount,
                    Narration = $"REVERSAL - Cheque Bounced #{cheque.ChequeNo}: {reason}",
                    SourceType = SourceType.Voucher,
                    SourceId = cheque.Id,
                    FinancialYearId = finYearId
                });

                // Debit Bank (reverse original credit)
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = DateTime.Today,
                    AccountId = cheque.ChequeBook.BankAccount.AccountId,
                    Debit = cheque.Amount,
                    Credit = 0,
                    Narration = $"REVERSAL - Cheque Bounced #{cheque.ChequeNo} reversed to Bank",
                    SourceType = SourceType.Voucher,
                    SourceId = cheque.Id,
                    FinancialYearId = finYearId
                });

                cheque.ChequeBook.BankAccount.CurrentBalance += cheque.Amount;
            }

            await context.SaveChangesAsync();

            // Sync legacy Bounced status
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                        UPDATE BankCheques SET PMFlag = 'B', Narration = Narration + ' [Bounced: ' + {0} + ']' WHERE ChequeNo = {1};", 
                    reason, cheque.ChequeNo);
            }
            catch { /* Ignore */ }
        }
    }

    public async Task CancelChequeAsync(int chequeId, string reason)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var cheque = await context.Cheques.FindAsync(chequeId);
        if (cheque != null)
        {
            cheque.Status = ChequeStatus.Cancelled;
            cheque.BounceReason = reason;
            cheque.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();

            // Sync legacy cancellation
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                        UPDATE BankChequeBookHistory SET IsCancelled = 1, CancelDate = GETDATE(), Remarks = {0} WHERE ChequeNo = {1};", 
                    reason, cheque.ChequeNo);
            }
            catch { /* Ignore */ }
        }
    }

    public async Task<BankDeposit> DepositCashToBankAsync(
        int bankAccountId, 
        decimal amount, 
        DateTime date, 
        string? slipNo, 
        string? narration, 
        int financialYearId, 
        int? cropSeasonId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var bankAccount = await context.BankAccounts.Include(b => b.Account).FirstOrDefaultAsync(b => b.Id == bankAccountId);
        if (bankAccount == null)
            throw new InvalidOperationException("Bank account not found.");

        var cashAccount = await context.Accounts
            .FirstOrDefaultAsync(a => !a.IsDeleted && (a.AccountNo == "1001" || a.Name.Contains("Cash") || a.Name.Contains("روکڑ")));

        if (cashAccount == null)
            throw new InvalidOperationException("Cash in Hand Account (روکڑ کھاتہ) not found.");

        var deposit = new BankDeposit
        {
            BankAccountId = bankAccountId,
            DepositType = DepositType.Cash,
            Amount = amount,
            DepositDate = date,
            ReferenceNo = slipNo,
            IsReconciled = true,
            ReconciledDate = date,
            CreatedAt = DateTime.Now
        };
        context.BankDeposits.Add(deposit);

        // Balanced Contra GL Entry:
        // 1. Debit Bank
        context.GeneralLedger.Add(new GeneralLedger
        {
            TransactionDate = date,
            AccountId = bankAccount.AccountId,
            Debit = amount,
            Credit = 0,
            Narration = $"Cash Deposited in Bank (Slip: {slipNo}) - {narration}",
            SourceType = SourceType.Voucher,
            FinancialYearId = financialYearId,
            CropSeasonId = cropSeasonId
        });

        // 2. Credit Cash
        context.GeneralLedger.Add(new GeneralLedger
        {
            TransactionDate = date,
            AccountId = cashAccount.Id,
            Debit = 0,
            Credit = amount,
            Narration = $"Cash Deposited in {bankAccount.BankName} (Slip: {slipNo})",
            SourceType = SourceType.Voucher,
            FinancialYearId = financialYearId,
            CropSeasonId = cropSeasonId
        });

        bankAccount.CurrentBalance += amount;
        await context.SaveChangesAsync();

        // ═══ AGENSOFT EXACT DATABASE REPLICATION SYNC (BankDeposit: Cash) ═══
        await SyncLegacyBankDepositAsync(context, deposit, bankAccount, 0, "", date, date, slipNo, narration, cropSeasonId, amount);

        return deposit;
    }

    public async Task<BankDeposit> DepositChequeToBankAsync(
        int bankAccountId, 
        int partyAccountId, 
        string chequeNo, 
        decimal amount, 
        DateTime chequeDate, 
        DateTime depositDate, 
        string? slipNo, 
        string? narration, 
        int financialYearId, 
        int? cropSeasonId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var bankAccount = await context.BankAccounts.Include(b => b.Account).FirstOrDefaultAsync(b => b.Id == bankAccountId);
        if (bankAccount == null)
            throw new InvalidOperationException("Bank account not found.");

        var deposit = new BankDeposit
        {
            BankAccountId = bankAccountId,
            DepositType = DepositType.Cheque,
            Amount = amount,
            DepositDate = depositDate,
            ReferenceNo = $"Chq: {chequeNo} / Slip: {slipNo}",
            IsReconciled = false,
            CreatedAt = DateTime.Now
        };
        context.BankDeposits.Add(deposit);

        // Balanced GL Entries (Bank Receiving):
        // 1. Debit Bank
        context.GeneralLedger.Add(new GeneralLedger
        {
            TransactionDate = depositDate,
            AccountId = bankAccount.AccountId,
            Debit = amount,
            Credit = 0,
            Narration = $"Cheque #{chequeNo} Deposited in Bank (Slip: {slipNo}) - {narration}",
            SourceType = SourceType.Voucher,
            FinancialYearId = financialYearId,
            CropSeasonId = cropSeasonId
        });

        // 2. Credit Party
        context.GeneralLedger.Add(new GeneralLedger
        {
            TransactionDate = depositDate,
            AccountId = partyAccountId,
            Debit = 0,
            Credit = amount,
            Narration = $"Cheque #{chequeNo} Deposited to Bank ({bankAccount.BankName})",
            SourceType = SourceType.Voucher,
            FinancialYearId = financialYearId,
            CropSeasonId = cropSeasonId
        });

        bankAccount.CurrentBalance += amount;
        await context.SaveChangesAsync();

        // ═══ AGENSOFT EXACT DATABASE REPLICATION SYNC (BankDeposit: Cheque) ═══
        await SyncLegacyBankDepositAsync(context, deposit, bankAccount, partyAccountId, chequeNo, chequeDate, depositDate, slipNo, narration, cropSeasonId, amount);

        return deposit;
    }

    public async Task ReconcileDepositAsync(int depositId, DateTime reconcileDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var deposit = await context.BankDeposits.FindAsync(depositId);
        if (deposit != null)
        {
            deposit.IsReconciled = true;
            deposit.ReconciledDate = reconcileDate;
            deposit.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();

            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankDeposit', 'U') IS NOT NULL
                        UPDATE BankDeposit SET ReconcileDate = {0}, IsPosted = 1 WHERE VoucherNo = {1};", 
                    reconcileDate.Date, deposit.Id);
            }
            catch { /* Ignore */ }
        }
    }

    private static async Task SyncLegacyChequeIssueAsync(
        AppDbContext context,
        Cheque cheque, 
        int partyAccountId, 
        decimal amount, 
        DateTime issueDate, 
        string? narration, 
        string? receivedBy, 
        int? cropSeasonId)
    {
        try
        {
            long vNum = cheque.Id;
            var digits = new string(cheque.ChequeNo.Where(char.IsDigit).ToArray());
            if (long.TryParse(digits, out var cNum) && cNum > 0) vNum = cNum;

            string bankAcNo = string.Empty;
            if (cheque.ChequeBook?.BankAccount?.Account != null)
            {
                bankAcNo = cheque.ChequeBook.BankAccount.Account.AccountNo;
            }
            else if (cheque.ChequeBook != null)
            {
                var bAc = await context.BankAccounts.Include(b => b.Account).FirstOrDefaultAsync(b => b.Id == cheque.ChequeBook.BankAccountId);
                bankAcNo = bAc?.Account?.AccountNo ?? bAc?.AccountNumber ?? string.Empty;
            }

            var partyAc = string.Empty;
            if (partyAccountId > 0)
            {
                var pAc = await context.Accounts.FindAsync(partyAccountId);
                partyAc = pAc?.AccountNo ?? string.Empty;
            }
            
            var cropSeason = "";
            if (cropSeasonId.HasValue && cropSeasonId.Value > 0)
            {
                try
                {
                    var season = await context.CropSeasons.FindAsync(cropSeasonId.Value);
                    cropSeason = season?.Name ?? "";
                }
                catch { }
            }
            if (cropSeason.Length > 5) cropSeason = cropSeason.Substring(0, 5);

            short userNo = 1;
            try
            {
                var uId = await context.Database.SqlQueryRaw<short?>(
                    "IF OBJECT_ID('Users', 'U') IS NOT NULL SELECT TOP 1 CAST(UserID AS SMALLINT) AS Value FROM Users ELSE SELECT CAST(1 AS SMALLINT) AS Value"
                ).FirstOrDefaultAsync();
                if (uId.HasValue && uId.Value > 0) userNo = uId.Value;
            }
            catch { userNo = 1; }
            int branchId = 1;
            try
            {
                var bVal = await context.Database.SqlQueryRaw<int?>("SELECT TOP 1 CAST(BranchID AS INT) AS Value FROM Branch").FirstOrDefaultAsync();
                if (bVal.HasValue && bVal.Value > 0) branchId = bVal.Value;
            }
            catch { branchId = 1; }

            var sqlCheque = @"
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                BEGIN
                    DELETE FROM BankChequesBody WHERE VoucherNo = {0};
                    DELETE FROM BankCheques WHERE VoucherNo = {0};
                    INSERT INTO BankCheques (
                        VoucherNo, VoucherDate, ChequeNo, ChequeDate, ReconcileDate, 
                        BankAccountNo, AccountNo, UserNo, Amount, ReceivedBy, 
                        Narration, IsLost, IsPosted, WHTAx, PMFlag, 
                        NarrationUrdu, WHTtoBank, WHTtoPayee, VoucherTime, CropSeason, 
                        BranchID, IDBranch
                    )
                    VALUES (
                        {0}, {1}, {2}, {1}, NULL, 
                        {3}, {4}, {5}, {6}, {7}, 
                        {8}, 0, 1, 0, 'I', 
                        N'', 0, 0, GETDATE(), {9}, 
                        {10}, {10}
                    );
                    INSERT INTO BankChequesBody (VoucherNo, VoucherDate, AccountNo, Amount)
                    VALUES ({0}, {1}, {4}, {6});
                END";
            await context.Database.ExecuteSqlRawAsync(sqlCheque, vNum, issueDate.Date, cheque.ChequeNo, bankAcNo, partyAc, userNo, amount, receivedBy ?? "", narration ?? "", cropSeason, branchId);

            // Sync to BankChequeBookHistory
            var sqlHistory = @"
                IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM BankChequeBookHistory WHERE AccountNo = {0} AND ChequeNo = {1})
                        INSERT INTO BankChequeBookHistory (AccountNo, ChequeNo, EntryDate, IsCancelled, Remarks)
                        VALUES ({0}, {1}, {2}, 0, {3});
                END";
            await context.Database.ExecuteSqlRawAsync(sqlHistory, bankAcNo, cheque.ChequeNo, issueDate.Date, narration ?? "Issued");
        }
        catch
        {
            // Resilient failover
        }
    }

    private static async Task SyncLegacyBankDepositAsync(
        AppDbContext context,
        BankDeposit deposit,
        BankAccount bankAccount,
        int partyAccountId,
        string chequeNo,
        DateTime chequeDate,
        DateTime depositDate,
        string? slipNo,
        string? narration,
        int? cropSeasonId,
        decimal amount)
    {
        try
        {
            long vNum = deposit.Id;
            string bankAcNo = bankAccount.Account?.AccountNo ?? string.Empty;
            if (string.IsNullOrEmpty(bankAcNo) && bankAccount.AccountId > 0)
            {
                var ac = await context.Accounts.FindAsync(bankAccount.AccountId);
                bankAcNo = ac?.AccountNo ?? bankAccount.AccountNumber;
            }

            string partyAcNo = string.Empty;
            if (partyAccountId > 0)
            {
                var pAc = await context.Accounts.FindAsync(partyAccountId);
                partyAcNo = pAc?.AccountNo ?? string.Empty;
            }

            var isCheque = deposit.DepositType == DepositType.Cheque ? 1 : 0;

            var cropSeason = "";
            if (cropSeasonId.HasValue && cropSeasonId.Value > 0)
            {
                try
                {
                    var season = await context.CropSeasons.FindAsync(cropSeasonId.Value);
                    cropSeason = season?.Name ?? "";
                }
                catch { }
            }
            if (cropSeason.Length > 5) cropSeason = cropSeason.Substring(0, 5);

            short userNo = 1;
            try
            {
                var uId = await context.Database.SqlQueryRaw<short?>(
                    "IF OBJECT_ID('Users', 'U') IS NOT NULL SELECT TOP 1 CAST(UserID AS SMALLINT) AS Value FROM Users ELSE SELECT CAST(1 AS SMALLINT) AS Value"
                ).FirstOrDefaultAsync();
                if (uId.HasValue && uId.Value > 0) userNo = uId.Value;
            }
            catch { userNo = 1; }
            int branchId = 1;
            try
            {
                var bVal = await context.Database.SqlQueryRaw<int?>("SELECT TOP 1 CAST(BranchID AS INT) AS Value FROM Branch").FirstOrDefaultAsync();
                if (bVal.HasValue && bVal.Value > 0) branchId = bVal.Value;
            }
            catch { branchId = 1; }

            var sqlDeposit = @"
                IF OBJECT_ID('BankDeposit', 'U') IS NOT NULL
                BEGIN
                    DELETE FROM BankDeposit WHERE VoucherNo = {0} AND VoucherDate = {1};
                    INSERT INTO BankDeposit (
                        VoucherNo, VoucherDate, IsCheque, ChequeNo, ChequeDate, 
                        SlipNo, DepositedDate, ReconcileDate, BankAccountNo, AccountNo, 
                        UserNo, Amount, DepositedBy, Narration, IsBounced, 
                        IsPosted, NarrationUrdu, VoucherTime, CropSeason, BranchID, 
                        IDBranch
                    )
                    VALUES (
                        {0}, {1}, {2}, {3}, {4}, 
                        {5}, {1}, CASE WHEN {2} = 1 THEN NULL ELSE {1} END, {6}, {7}, 
                        {8}, {9}, {10}, {11}, 0, 
                        1, N'', GETDATE(), {12}, {13}, 
                        {13}
                    );
                END";
            await context.Database.ExecuteSqlRawAsync(sqlDeposit, 
                vNum, depositDate.Date, isCheque, chequeNo ?? "", chequeDate.Date, 
                slipNo ?? "", bankAcNo, partyAcNo, 
                userNo, amount, "", narration ?? "", cropSeason, branchId);
        }
        catch
        {
            // Resilient failover
        }
    }

    public async Task<IEnumerable<BankDeposit>> GetUnclearedDepositsAsync(int? bankAccountId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.BankDeposits
            .Include(d => d.BankAccount)
                .ThenInclude(b => b.Account)
            .Where(d => !d.IsReconciled && !d.IsDeleted);

        if (bankAccountId.HasValue && bankAccountId.Value > 0)
        {
            query = query.Where(d => d.BankAccountId == bankAccountId.Value);
        }

        return await query.OrderBy(d => d.DepositDate).ToListAsync();
    }

    // ═══ AGENSOFT EXACT CHEQUE BOOK GENERATION PARITY (frmBankChequeGenerate & BankChequeBookHistory) ═══

    public async Task<int> GenerateChequeBookLeavesAsync(string bankAccountNo, string fromChequeNo, string toChequeNo)
    {
        if (string.IsNullOrWhiteSpace(bankAccountNo))
            throw new ArgumentException("Bank Account Number is required.", nameof(bankAccountNo));
        if (string.IsNullOrWhiteSpace(fromChequeNo) || string.IsNullOrWhiteSpace(toChequeNo))
            throw new ArgumentException("Both From and To Cheque Numbers are required.");

        string rawFrom = fromChequeNo.Trim();
        string rawTo = toChequeNo.Trim();

        if (!long.TryParse(rawFrom, out long start) || !long.TryParse(rawTo, out long end))
            throw new ArgumentException("Cheque numbers must be numeric.");

        if (end < start)
            throw new ArgumentException("From Cheque No cannot be greater than To Cheque No.");

        if (end - start + 1 > 500)
            throw new ArgumentException("Cannot generate more than 500 cheque leaves at a time.");

        int padLen = Math.Max(rawFrom.Length, rawTo.Length);

        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Ensure legacy BankChequeBookHistory table exists
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('BankChequeBookHistory', 'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[BankChequeBookHistory] (
                        [AccountNo] [varchar](10) NOT NULL,
                        [ChequeNo] [varchar](50) NOT NULL,
                        [EntryDate] [smalldatetime] NOT NULL,
                        [IsCancelled] [bit] NULL DEFAULT(0),
                        [Remarks] [varchar](150) NULL,
                        [CancelDate] [smalldatetime] NULL,
                        CONSTRAINT PK_BankChequeBookHistory PRIMARY KEY (AccountNo, ChequeNo)
                    );
                END");
        }
        catch { }

        int count = 0;
        DateTime entryDate = DateTime.Today;

        for (long num = start; num <= end; num++)
        {
            string chqNo = num.ToString().PadLeft(padLen, '0');
            try
            {
                int rows = await context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT 1 FROM BankChequeBookHistory WHERE AccountNo = {0} AND ChequeNo = {1})
                    BEGIN
                        INSERT INTO BankChequeBookHistory (AccountNo, ChequeNo, EntryDate, IsCancelled, Remarks)
                        VALUES ({0}, {1}, {2}, 0, '');
                    END", bankAccountNo, chqNo, entryDate);

                if (rows > 0) count++;
            }
            catch { }
        }

        // Also sync to modern EF Core ChequeBook / Cheques if bank account is registered
        try
        {
            var bAccount = await context.BankAccounts
                .Include(b => b.Account)
                .Include(b => b.ChequeBooks)
                .FirstOrDefaultAsync(b => (b.Account != null && b.Account.AccountNo == bankAccountNo) || b.AccountNumber == bankAccountNo);

            if (bAccount != null)
            {
                var book = new ChequeBook
                {
                    BankAccountId = bAccount.Id,
                    SeriesStart = (int)(start % int.MaxValue),
                    SeriesEnd = (int)(end % int.MaxValue),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                for (long num = start; num <= end; num++)
                {
                    string chqNo = num.ToString().PadLeft(padLen, '0');
                    book.Cheques.Add(new Cheque
                    {
                        ChequeNo = chqNo,
                        Status = ChequeStatus.Blank,
                        Amount = 0,
                        CreatedAt = DateTime.Now
                    });
                }
                context.ChequeBooks.Add(book);
                await context.SaveChangesAsync();
            }
        }
        catch { }

        return count;
    }

    public async Task<IEnumerable<ChequeHistoryDto>> GetChequeBookHistoryForBankAsync(string bankAccountNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<ChequeHistoryDto>();

        try
        {
            var sql = @"
                IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        b.AccountNo, 
                        b.ChequeNo, 
                        b.EntryDate, 
                        ISNULL(c.AccountName, '') AS AccountName, 
                        ISNULL(b.Remarks, '') AS Remarks, 
                        ISNULL(b.IsCancelled, 0) AS IsCancelled, 
                        b.CancelDate,
                        CASE 
                            WHEN ISNULL(b.IsCancelled, 0) = 1 THEN 'Cancelled'
                            WHEN bc.ChequeNo IS NOT NULL THEN 'Issued'
                            ELSE 'Available'
                        END AS Status,
                        bc.VoucherNo,
                        bc.Amount,
                        ISNULL(p.AccountName, bc.ReceivedBy) AS Payee
                    FROM BankChequeBookHistory b
                    LEFT OUTER JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo
                    LEFT OUTER JOIN BankCheques bc ON bc.BankAccountNo = b.AccountNo AND bc.ChequeNo = b.ChequeNo
                    LEFT OUTER JOIN ChartOfAccounts p ON bc.AccountNo = p.AccountNo
                    WHERE b.AccountNo = {0}
                    ORDER BY b.ChequeNo ASC;
                END";

            var rows = await context.Database.SqlQueryRaw<ChequeHistoryDto>(sql, bankAccountNo).ToListAsync();
            list.AddRange(rows);
        }
        catch { }

        return list;
    }

    // ═══ AGENSOFT EXACT BANK CHEQUES EDITING PARITY (BankChequeBookHistory) ═══

    public async Task<IEnumerable<ChequeHistoryDto>> GetChequeBookHistoryAsync(string? bankAccountNo = null, DateTime? entryDate = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<ChequeHistoryDto>();

        try
        {
            object pAccount = string.IsNullOrWhiteSpace(bankAccountNo) ? DBNull.Value : bankAccountNo.Trim();
            object pDate = entryDate.HasValue ? (object)entryDate.Value.Date : DBNull.Value;

            var sql = @"
                IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        b.AccountNo, 
                        b.ChequeNo, 
                        b.EntryDate, 
                        ISNULL(c.AccountName, '') AS AccountName, 
                        ISNULL(b.Remarks, '') AS Remarks, 
                        ISNULL(b.IsCancelled, 0) AS IsCancelled, 
                        b.CancelDate,
                        CASE 
                            WHEN ISNULL(b.IsCancelled, 0) = 1 THEN 'Cancelled'
                            WHEN bc.ChequeNo IS NOT NULL THEN 'Issued'
                            ELSE 'Available'
                        END AS Status,
                        bc.VoucherNo,
                        bc.Amount,
                        bc.ReceivedBy AS Payee
                    FROM BankChequeBookHistory b
                    LEFT OUTER JOIN ChartOfAccounts c ON b.AccountNo = c.AccountNo
                    LEFT OUTER JOIN BankCheques bc ON b.AccountNo = bc.BankAccountNo AND b.ChequeNo = bc.ChequeNo
                    WHERE (bc.ChequeNo IS NULL)
                      AND ({0} IS NULL OR b.AccountNo = {0})
                      AND ({1} IS NULL OR CAST(b.EntryDate AS DATE) = CAST({1} AS DATE))
                    ORDER BY b.AccountNo, b.ChequeNo;
                END";

            var rows = await context.Database.SqlQueryRaw<ChequeHistoryDto>(sql, pAccount, pDate).ToListAsync();
            list.AddRange(rows);
        }
        catch { /* Resilient failover */ }

        return list;
    }

    public async Task CancelChequeInHistoryAsync(string accountNo, string chequeNo, string remarks)
    {
        await UpdateChequeCancellationAsync(accountNo, chequeNo, true, DateTime.Today, remarks);
    }

    public async Task UpdateChequeCancellationAsync(string accountNo, string chequeNo, bool isCancelled, DateTime? cancelDate, string? remarks)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            if (isCancelled)
            {
                var cDate = cancelDate ?? DateTime.Today;
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                        UPDATE BankChequeBookHistory 
                        SET Remarks = {0}, IsCancelled = 1, CancelDate = {1} 
                        WHERE AccountNo = {2} AND ChequeNo = {3};",
                    remarks ?? string.Empty, cDate, accountNo, chequeNo);
            }
            else
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                        UPDATE BankChequeBookHistory 
                        SET Remarks = NULL, IsCancelled = 0, CancelDate = NULL 
                        WHERE AccountNo = {0} AND ChequeNo = {1};",
                    accountNo, chequeNo);
            }
        }
        catch { }

        try
        {
            var chq = await context.Cheques
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.BankAccount)
                        .ThenInclude(ba => ba.Account)
                .FirstOrDefaultAsync(c => c.ChequeNo == chequeNo && (c.ChequeBook.BankAccount.Account.AccountNo == accountNo || c.ChequeBook.BankAccount.AccountNumber == accountNo));
            if (chq != null)
            {
                chq.Status = isCancelled ? ChequeStatus.Cancelled : ChequeStatus.Blank;
                chq.BounceReason = isCancelled ? remarks : null;
                chq.UpdatedAt = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
        catch { }
    }

    public async Task DeleteChequeFromHistoryAsync(string accountNo, string chequeNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                    DELETE FROM BankChequeBookHistory 
                    WHERE AccountNo = {0} AND ChequeNo = {1} 
                      AND ChequeNo NOT IN (SELECT ChequeNo FROM BankCheques WHERE BankAccountNo = {0});",
                accountNo, chequeNo);
        }
        catch { }

        try
        {
            var chq = await context.Cheques
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.BankAccount)
                        .ThenInclude(ba => ba.Account)
                .FirstOrDefaultAsync(c => c.ChequeNo == chequeNo && (c.ChequeBook.BankAccount.Account.AccountNo == accountNo || c.ChequeBook.BankAccount.AccountNumber == accountNo));
            if (chq != null && (chq.Status == ChequeStatus.Blank || chq.Status == ChequeStatus.Cancelled))
            {
                context.Cheques.Remove(chq);
                await context.SaveChangesAsync();
            }
        }
        catch { }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AGENSOFT EXACT BANK CHEQUES RECONCILIATION PARITY (frmChequesReconcilation)
    // ══════════════════════════════════════════════════════════════════════════

    public async Task<IEnumerable<BankChequeReconciliationItemDto>> GetChequesForReconciliationAsync(string bankAccountNo, bool? isReconciled = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<BankChequeReconciliationItemDto>();

        try
        {
            if (string.IsNullOrWhiteSpace(bankAccountNo)) return list;

            using var command = context.Database.GetDbConnection().CreateCommand();
            var sql = @"
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        bc.VoucherNo,
                        bc.VoucherDate,
                        bc.ChequeNo,
                        bc.ChequeDate,
                        bc.BankAccountNo,
                        ISNULL(c2.AccountName, '') AS BankAccountName,
                        bc.AccountNo AS PartyAccountNo,
                        ISNULL(c1.AccountName, ISNULL(bc.ReceivedBy, '')) AS AccountName,
                        bc.Amount,
                        ISNULL(bc.Narration, '') AS Narration,
                        bc.ReconcileDate,
                        ISNULL(bc.IsLost, 0) AS IsLost,
                        CASE WHEN bc.ReconcileDate IS NOT NULL THEN 1 ELSE 0 END AS IsReconciled
                    FROM BankCheques bc
                    LEFT OUTER JOIN ChartOfAccounts c2 ON bc.BankAccountNo = c2.AccountNo
                    LEFT OUTER JOIN ChartOfAccounts c1 ON bc.AccountNo = c1.AccountNo
                    WHERE bc.BankAccountNo = @bankAcNo
                      AND (@isReconciled IS NULL 
                           OR (@isReconciled = 1 AND bc.ReconcileDate IS NOT NULL)
                           OR (@isReconciled = 0 AND bc.ReconcileDate IS NULL))
                    ORDER BY bc.ChequeDate DESC, bc.VoucherNo DESC, bc.VoucherDate DESC;
                END";

            command.CommandText = sql;
            var p1 = command.CreateParameter();
            p1.ParameterName = "@bankAcNo";
            p1.Value = bankAccountNo.Trim();
            command.Parameters.Add(p1);

            var p2 = command.CreateParameter();
            p2.ParameterName = "@isReconciled";
            p2.Value = isReconciled.HasValue ? (object)(isReconciled.Value ? 1 : 0) : DBNull.Value;
            command.Parameters.Add(p2);

            await context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new BankChequeReconciliationItemDto
                {
                    VoucherNo = reader.GetInt64(0),
                    VoucherDate = reader.GetDateTime(1),
                    ChequeNo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    ChequeDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                    BankAccountNo = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    BankAccountName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    PartyAccountNo = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    AccountName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Amount = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                    Narration = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    ReconcileDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                    IsLost = !reader.IsDBNull(11) && reader.GetBoolean(11),
                    IsReconciled = !reader.IsDBNull(12) && reader.GetInt32(12) == 1
                });
            }
        }
        catch { /* Resilient failover */ }

        return list;
    }

    public async Task<(decimal ReconciledBalance, decimal UnreconciledBalance)> GetBankChequesReconciliationBalancesAsync(string bankAccountNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        decimal recBal = 0;
        decimal unrecBal = 0;

        try
        {
            if (string.IsNullOrWhiteSpace(bankAccountNo)) return (0, 0);

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        ISNULL(SUM(CASE WHEN ReconcileDate IS NOT NULL THEN Amount ELSE 0 END), 0) AS RecBal,
                        ISNULL(SUM(CASE WHEN ReconcileDate IS NULL THEN Amount ELSE 0 END), 0) AS UnrecBal
                    FROM BankCheques
                    WHERE BankAccountNo = @bankAcNo;
                END
                ELSE
                BEGIN
                    SELECT CAST(0 AS DECIMAL(18,2)) AS RecBal, CAST(0 AS DECIMAL(18,2)) AS UnrecBal;
                END";

            var p = command.CreateParameter();
            p.ParameterName = "@bankAcNo";
            p.Value = bankAccountNo.Trim();
            command.Parameters.Add(p);

            await context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                recBal = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                unrecBal = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
            }
        }
        catch { }

        return (recBal, unrecBal);
    }

    public async Task SaveChequeReconciliationAsync(long voucherNo, string bankAccountNo, DateTime? reconcileDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            if (reconcileDate.HasValue)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                        UPDATE BankCheques 
                        SET ReconcileDate = {0}, IsLost = 0 
                        WHERE VoucherNo = {1} AND BankAccountNo = {2};",
                    reconcileDate.Value, voucherNo, bankAccountNo);
            }
            else
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                        UPDATE BankCheques 
                        SET ReconcileDate = NULL 
                        WHERE VoucherNo = {0} AND BankAccountNo = {1};",
                    voucherNo, bankAccountNo);
            }

            // Sync with EF Cheques
            var chqNo = await context.Database.SqlQueryRaw<string>(@"
                SELECT ChequeNo AS Value FROM BankCheques WHERE VoucherNo = {0} AND BankAccountNo = {1}", voucherNo, bankAccountNo).FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(chqNo))
            {
                var chq = await context.Cheques.FirstOrDefaultAsync(c => c.ChequeNo == chqNo);
                if (chq != null)
                {
                    if (reconcileDate.HasValue)
                    {
                        chq.Status = ChequeStatus.Cleared;
                        chq.ClearanceDate = reconcileDate.Value;
                    }
                    else
                    {
                        chq.Status = ChequeStatus.Issued;
                        chq.ClearanceDate = null;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
        catch { }
    }

    public async Task BatchSaveChequeReconciliationAsync(IEnumerable<(long voucherNo, string bankAccountNo, DateTime? reconcileDate)> updates)
    {
        foreach (var u in updates)
        {
            await SaveChequeReconciliationAsync(u.voucherNo, u.bankAccountNo, u.reconcileDate);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AGENSOFT EXACT DEPOSIT RECONCILIATION PARITY (frmDepositReconcilation)
    // ══════════════════════════════════════════════════════════════════════════

    public async Task<IEnumerable<BankDepositReconciliationItemDto>> GetDepositsForReconciliationAsync(string bankAccountNo, bool? isReconciled = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<BankDepositReconciliationItemDto>();

        try
        {
            if (string.IsNullOrWhiteSpace(bankAccountNo)) return list;

            using var command = context.Database.GetDbConnection().CreateCommand();
            var sql = @"
                IF OBJECT_ID('BankDeposit', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        bd.VoucherNo,
                        bd.VoucherDate,
                        ISNULL(bd.SlipNo, '') AS SlipNo,
                        bd.DepositedDate,
                        bd.BankAccountNo,
                        ISNULL(c1.AccountName, '') AS BankAccountName,
                        ISNULL(bd.AccountNo, '') AS AccountNo,
                        ISNULL(c2.AccountName, '') AS AccountName,
                        bd.Amount,
                        ISNULL(bd.Narration, '') AS Narration,
                        bd.ReconcileDate,
                        ISNULL(bd.IsCheque, 0) AS IsCheque,
                        bd.ChequeNo,
                        CASE WHEN bd.ReconcileDate IS NOT NULL THEN 1 ELSE 0 END AS IsReconciled
                    FROM BankDeposit bd
                    LEFT OUTER JOIN ChartOfAccounts c1 ON bd.BankAccountNo = c1.AccountNo
                    LEFT OUTER JOIN ChartOfAccounts c2 ON bd.AccountNo = c2.AccountNo
                    WHERE bd.BankAccountNo = @bankAcNo
                      AND (@isReconciled IS NULL 
                           OR (@isReconciled = 1 AND bd.ReconcileDate IS NOT NULL)
                           OR (@isReconciled = 0 AND bd.ReconcileDate IS NULL))
                    ORDER BY bd.DepositedDate DESC, bd.VoucherNo DESC, bd.VoucherDate DESC;
                END";

            command.CommandText = sql;
            var p1 = command.CreateParameter();
            p1.ParameterName = "@bankAcNo";
            p1.Value = bankAccountNo.Trim();
            command.Parameters.Add(p1);

            var p2 = command.CreateParameter();
            p2.ParameterName = "@isReconciled";
            p2.Value = isReconciled.HasValue ? (object)(isReconciled.Value ? 1 : 0) : DBNull.Value;
            command.Parameters.Add(p2);

            await context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new BankDepositReconciliationItemDto
                {
                    VoucherNo = reader.GetInt64(0),
                    VoucherDate = reader.GetDateTime(1),
                    SlipNo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    DepositedDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                    BankAccountNo = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    BankAccountName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    AccountNo = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    AccountName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Amount = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                    Narration = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    ReconcileDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                    IsCheque = !reader.IsDBNull(11) && reader.GetBoolean(11),
                    ChequeNo = reader.IsDBNull(12) ? null : reader.GetString(12),
                    IsReconciled = !reader.IsDBNull(13) && reader.GetInt32(13) == 1
                });
            }
        }
        catch { }

        return list;
    }

    public async Task<(decimal ReconciledBalance, decimal UnreconciledBalance)> GetBankDepositsReconciliationBalancesAsync(string bankAccountNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        decimal recBal = 0;
        decimal unrecBal = 0;

        try
        {
            if (string.IsNullOrWhiteSpace(bankAccountNo)) return (0, 0);

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"
                IF OBJECT_ID('BankDeposit', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        ISNULL(SUM(CASE WHEN ReconcileDate IS NOT NULL THEN Amount ELSE 0 END), 0) AS RecBal,
                        ISNULL(SUM(CASE WHEN ReconcileDate IS NULL THEN Amount ELSE 0 END), 0) AS UnrecBal
                    FROM BankDeposit
                    WHERE BankAccountNo = @bankAcNo;
                END
                ELSE
                BEGIN
                    SELECT CAST(0 AS DECIMAL(18,2)) AS RecBal, CAST(0 AS DECIMAL(18,2)) AS UnrecBal;
                END";

            var p = command.CreateParameter();
            p.ParameterName = "@bankAcNo";
            p.Value = bankAccountNo.Trim();
            command.Parameters.Add(p);

            await context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                recBal = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                unrecBal = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
            }
        }
        catch { }

        return (recBal, unrecBal);
    }

    public async Task SaveDepositReconciliationAsync(long voucherNo, string bankAccountNo, DateTime? reconcileDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            if (reconcileDate.HasValue)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankDeposit', 'U') IS NOT NULL
                        UPDATE BankDeposit 
                        SET ReconcileDate = {0} 
                        WHERE VoucherNo = {1} AND BankAccountNo = {2};",
                    reconcileDate.Value, voucherNo, bankAccountNo);
            }
            else
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('BankDeposit', 'U') IS NOT NULL
                        UPDATE BankDeposit 
                        SET ReconcileDate = NULL 
                        WHERE VoucherNo = {0} AND BankAccountNo = {1};",
                    voucherNo, bankAccountNo);
            }

            // Sync with EF BankDeposits
            var deposit = await context.BankDeposits.FirstOrDefaultAsync(d => d.Id == (int)voucherNo);
            if (deposit != null)
            {
                deposit.IsReconciled = reconcileDate.HasValue;
                deposit.ReconciledDate = reconcileDate;
                deposit.UpdatedAt = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
        catch { }
    }

    public async Task BatchSaveDepositReconciliationAsync(IEnumerable<(long voucherNo, string bankAccountNo, DateTime? reconcileDate)> updates)
    {
        foreach (var u in updates)
        {
            await SaveDepositReconciliationAsync(u.voucherNo, u.bankAccountNo, u.reconcileDate);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AGENSOFT EXACT BANK CHEQUE DEPOSIT PARITY (frmChequeDeposit & BankDeposit)
    // ══════════════════════════════════════════════════════════════════════════

    private static string Trunc(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed.Substring(0, maxLength);
    }

    private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value, System.Data.DbType? dbType = null)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        if (dbType.HasValue) p.DbType = dbType.Value;
        p.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(p);
    }

    private static async Task<bool> ObjectExistsAsync(AppDbContext context, string objectName, string typeCode = "U")
    {
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await context.Database.OpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CASE WHEN OBJECT_ID(@name, @type) IS NOT NULL THEN 1 ELSE 0 END";
            AddParam(cmd, "@name", objectName);
            AddParam(cmd, "@type", typeCode);
            var res = await cmd.ExecuteScalarAsync();
            return res != null && res != DBNull.Value && Convert.ToInt32(res) == 1;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<Dictionary<string, string>> ReadRegistryMapAsync(System.Data.Common.DbConnection conn)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            using var chk = conn.CreateCommand();
            chk.CommandText = "SELECT CASE WHEN OBJECT_ID('Project_Registry', 'U') IS NOT NULL THEN 1 ELSE 0 END";
            var exists = chk.ExecuteScalar();
            if (exists == null || exists == DBNull.Value || Convert.ToInt32(exists) != 1)
                return map;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT RegistryKey, ISNULL(Value, N'') AS Val FROM Project_Registry";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var k = reader["RegistryKey"]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(k))
                    map[k] = reader["Val"]?.ToString()?.Trim() ?? string.Empty;
            }
        }
        catch { }
        return map;
    }

    private static bool RegBool(IReadOnlyDictionary<string, string> reg, string key, bool defaultValue)
    {
        if (!reg.TryGetValue(key, out var val) || string.IsNullOrWhiteSpace(val))
            return defaultValue;
        return val == "1" || val.Equals("true", StringComparison.OrdinalIgnoreCase) || val.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> CheckUserTaskPermissionAsync(int userId, string taskKey)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (!await ObjectExistsAsync(context, "Users", "U"))
                return true;

            var conn = context.Database.GetDbConnection();
            using (var countCmd = conn.CreateCommand())
            {
                countCmd.CommandText = "SELECT COUNT(*) FROM Users";
                var totalUsers = Convert.ToInt32(await countCmd.ExecuteScalarAsync() ?? 0);
                if (totalUsers == 0)
                    return true;
            }

            bool? isAdmin = null;
            using (var userCmd = conn.CreateCommand())
            {
                userCmd.CommandText = "SELECT IsAdministrator FROM Users WHERE UserID = @uid";
                AddParam(userCmd, "@uid", (short)Math.Max(0, userId));
                var obj = await userCmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                    isAdmin = Convert.ToBoolean(obj);
            }

            if (isAdmin == null && userId <= 1)
                return true;

            if (isAdmin == true)
                return true;

            if (!await ObjectExistsAsync(context, "UsersTasks", "U"))
                return true;

            using var taskCmd = conn.CreateCommand();
            taskCmd.CommandText = "SELECT COUNT(*) FROM UsersTasks WHERE UserID = @uid AND TaskKey = @key";
            AddParam(taskCmd, "@uid", (short)Math.Max(0, userId));
            AddParam(taskCmd, "@key", taskKey);
            var granted = Convert.ToInt32(await taskCmd.ExecuteScalarAsync() ?? 0);
            return granted > 0;
        }
        catch
        {
            return true;
        }
    }

    public async Task<ChequeDepositFormConfigDto> GetChequeDepositFormConfigAsync(int userId)
    {
        var cfg = new ChequeDepositFormConfigDto();
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await context.Database.OpenConnectionAsync();

        var reg = await ReadRegistryMapAsync(conn);
        cfg.UseBankDepositMulti = RegBool(reg, "UseBankDepositMulti", false);
        cfg.EnableCropSeason = RegBool(reg, "EnableCropSeason", true);
        cfg.EnableBranchWise = RegBool(reg, "EnableBrancheWise", false);
        cfg.ShowUrduNarration = RegBool(reg, "ShowUrduNarration", true);
        cfg.EnableThumbOnForms = RegBool(reg, "EnableThumbOnForms", true);
        cfg.EnableSms = RegBool(reg, "EnableSms", true);
        cfg.ShowPrintDialogVoucher = RegBool(reg, "ShowPrintDialogVoucher", false);
        cfg.DirectPrintVoucher = RegBool(reg, "DirectPrintVoucher", false);
        cfg.ChangeDateBankVoucher = RegBool(reg, "ChangeDateBankVoucher", true);
        cfg.ChangeVidBankVoucher = RegBool(reg, "ChangeVIDBankVoucher", true);

        if (reg.TryGetValue("AutoGeneratedChequeDepositVoucherID", out var autoMode) && !string.IsNullOrWhiteSpace(autoMode))
        {
            cfg.AutoVoucherIdMode = autoMode.Trim();
            cfg.EnableAutoVoucherId = autoMode.Equals("Yearly", StringComparison.OrdinalIgnoreCase)
                || autoMode.Equals("Monthly", StringComparison.OrdinalIgnoreCase)
                || autoMode.Equals("Daily", StringComparison.OrdinalIgnoreCase);
        }

        if (reg.TryGetValue("AccBankAccount", out var bankPrefix) && !string.IsNullOrWhiteSpace(bankPrefix))
            cfg.AccBankAccountPrefix = bankPrefix.Trim();

        cfg.AllowModifyDelete = await CheckUserTaskPermissionAsync(userId, "ChequeDepositsModification");

        // Load Branches
        try
        {
            if (await ObjectExistsAsync(context, "Branch", "U"))
            {
                using var bCmd = conn.CreateCommand();
                bCmd.CommandText = "SELECT BranchID, ISNULL(CompanyName, N'') AS CompanyName FROM Branch ORDER BY BranchID";
                using var reader = await bCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    cfg.Branches.Add(new BranchLookupDto
                    {
                        BranchId = Convert.ToInt32(reader["BranchID"]),
                        CompanyName = reader["CompanyName"]?.ToString() ?? string.Empty
                    });
                }
                if (cfg.Branches.Count > 1 && !reg.ContainsKey("EnableBrancheWise"))
                    cfg.EnableBranchWise = true;
            }
        }
        catch { }

        if (cfg.Branches.Count == 0)
            cfg.Branches.Add(new BranchLookupDto { BranchId = 1, CompanyName = "Main Branch" });

        return cfg;
    }

    public async Task<long> GetNextChequeDepositVoucherNoAsync(DateTime? voucherDate = null, int? branchId = null)
    {
        var nums = await GetNextChequeDepositNumbersAsync(voucherDate ?? DateTime.Today, branchId ?? 1);
        return nums.VoucherNo;
    }

    public async Task<(long VoucherNo, long IdBranch)> GetNextChequeDepositNumbersAsync(DateTime voucherDate, int branchId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        long nextNo = 1;
        long nextIdBranch = 1;

        try
        {
            if (await ObjectExistsAsync(context, "BankDeposit", "U"))
            {
                var conn = context.Database.GetDbConnection();
                var reg = await ReadRegistryMapAsync(conn);
                reg.TryGetValue("AutoGeneratedChequeDepositVoucherID", out var mode);
                bool branchWise = RegBool(reg, "EnableBrancheWise", false);

                string dateFilter = string.Empty;
                DateTime? fyStart = null;
                DateTime? fyEnd = null;
                if (string.Equals(mode, "Daily", StringComparison.OrdinalIgnoreCase))
                {
                    dateFilter = " WHERE CONVERT(date, VoucherDate) = @vDate";
                }
                else if (string.Equals(mode, "Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    dateFilter = " WHERE YEAR(VoucherDate) = YEAR(@vDate) AND MONTH(VoucherDate) = MONTH(@vDate)";
                }
                else if (string.Equals(mode, "Yearly", StringComparison.OrdinalIgnoreCase))
                {
                    var fy = await _systemService.GetCurrentYearAsync();
                    if (fy != null && fy.StartDate != default && fy.EndDate != default)
                    {
                        fyStart = fy.StartDate.Date;
                        fyEnd = fy.EndDate.Date;
                        dateFilter = " WHERE CONVERT(date, VoucherDate) BETWEEN @fyStart AND @fyEnd";
                    }
                    else
                    {
                        dateFilter = " WHERE YEAR(VoucherDate) = YEAR(@vDate)";
                    }
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT CAST(ISNULL(MAX(VoucherNo), 0) + 1 AS BIGINT) FROM BankDeposit{dateFilter}";
                    if (dateFilter.Contains("@vDate")) AddParam(cmd, "@vDate", voucherDate.Date, System.Data.DbType.Date);
                    if (fyStart.HasValue) AddParam(cmd, "@fyStart", fyStart.Value, System.Data.DbType.Date);
                    if (fyEnd.HasValue) AddParam(cmd, "@fyEnd", fyEnd.Value, System.Data.DbType.Date);
                    var val = await cmd.ExecuteScalarAsync();
                    if (val != null && val != DBNull.Value)
                        nextNo = Convert.ToInt64(val);
                }

                if (branchWise)
                {
                    using var bCmd = conn.CreateCommand();
                    var branchClause = string.IsNullOrEmpty(dateFilter)
                        ? " WHERE BranchID = @branchId"
                        : $"{dateFilter} AND BranchID = @branchId";
                    bCmd.CommandText = $"SELECT CAST(ISNULL(MAX(IDBranch), 0) + 1 AS BIGINT) FROM BankDeposit{branchClause}";
                    if (branchClause.Contains("@vDate")) AddParam(bCmd, "@vDate", voucherDate.Date, System.Data.DbType.Date);
                    if (fyStart.HasValue) AddParam(bCmd, "@fyStart", fyStart.Value, System.Data.DbType.Date);
                    if (fyEnd.HasValue) AddParam(bCmd, "@fyEnd", fyEnd.Value, System.Data.DbType.Date);
                    AddParam(bCmd, "@branchId", branchId);
                    var bVal = await bCmd.ExecuteScalarAsync();
                    nextIdBranch = (bVal != null && bVal != DBNull.Value) ? Convert.ToInt64(bVal) : 1;
                }
                else
                {
                    nextIdBranch = nextNo;
                }

                return (Math.Max(1, nextNo), Math.Max(1, nextIdBranch));
            }
        }
        catch { }

        try
        {
            if (await ObjectExistsAsync(context, "BankDeposits", "U"))
            {
                var maxEf = await context.BankDeposits.MaxAsync(d => (int?)d.Id) ?? 0;
                if (maxEf + 1 > nextNo) nextNo = maxEf + 1;
            }
        }
        catch { }

        return (nextNo, nextNo);
    }

    public async Task<bool> ChequeDepositExistsAsync(long voucherNo, DateTime voucherDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (!await ObjectExistsAsync(context, "BankDeposit", "U"))
            return false;

        var conn = context.Database.GetDbConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM BankDeposit WHERE VoucherNo = @vNo AND CONVERT(date, VoucherDate) = @vDate";
        AddParam(cmd, "@vNo", voucherNo);
        AddParam(cmd, "@vDate", voucherDate.Date, System.Data.DbType.Date);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync() ?? 0) > 0;
    }

    public async Task<ChartAccountLookupDto?> ValidateChartAccountAsync(string accountNo)
    {
        if (string.IsNullOrWhiteSpace(accountNo)) return null;
        var clean = accountNo.Trim();

        using var context = await _contextFactory.CreateDbContextAsync();
        if (await ObjectExistsAsync(context, "ChartOfAccounts", "U"))
        {
            var conn = context.Database.GetDbConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 AccountNo, ISNULL(AccountName, '') AS AccountName,
                       ISNULL(AccountNameUrdu, N'') AS AccountNameUrdu,
                       ISNULL(AccountType, '') AS AccountType,
                       ISNULL(Bank, N'') AS Bank,
                       ISNULL(CropSeason, '') AS CropSeason
                FROM ChartOfAccounts
                WHERE IsLocked = 0 AND IsDetailed = 1 AND AccountNo = @acc";
            AddParam(cmd, "@acc", clean);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ChartAccountLookupDto
                {
                    AccountNo = reader["AccountNo"]?.ToString() ?? string.Empty,
                    AccountName = reader["AccountName"]?.ToString() ?? string.Empty,
                    AccountNameUrdu = reader["AccountNameUrdu"]?.ToString() ?? string.Empty,
                    AccountType = reader["AccountType"]?.ToString() ?? string.Empty,
                    Bank = reader["Bank"]?.ToString() ?? string.Empty,
                    CropSeason = reader["CropSeason"]?.ToString() ?? string.Empty
                };
            }
            return null;
        }

        if (await ObjectExistsAsync(context, "Accounts", "U"))
        {
            var ef = await context.Accounts.FirstOrDefaultAsync(a => !a.IsDeleted && a.AccountNo == clean);
            if (ef != null)
            {
                return new ChartAccountLookupDto
                {
                    AccountNo = ef.AccountNo,
                    AccountName = ef.Name,
                    AccountNameUrdu = ef.UrduName ?? string.Empty
                };
            }
        }
        return null;
    }

    public async Task<List<ChartAccountLookupDto>> SearchChartAccountsAsync(string? filter = null, string? accountPrefix = null, bool bankOnly = false)
    {
        var list = new List<ChartAccountLookupDto>();
        using var context = await _contextFactory.CreateDbContextAsync();

        if (await ObjectExistsAsync(context, "ChartOfAccounts", "U"))
        {
            var conn = context.Database.GetDbConnection();
            async Task<List<ChartAccountLookupDto>> QueryAsync(string extraWhere)
            {
                var rows = new List<ChartAccountLookupDto>();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $@"
                    SELECT AccountNo, ISNULL(AccountName, '') AS AccountName,
                           ISNULL(AccountNameUrdu, N'') AS AccountNameUrdu,
                           ISNULL(AccountType, '') AS AccountType,
                           ISNULL(Bank, N'') AS Bank,
                           ISNULL(CropSeason, '') AS CropSeason
                    FROM ChartOfAccounts
                    WHERE IsLocked = 0 AND IsDetailed = 1 {extraWhere}
                    ORDER BY AccountNo";
                if (!string.IsNullOrWhiteSpace(filter))
                    AddParam(cmd, "@q", $"%{filter.Trim()}%");
                if (!string.IsNullOrWhiteSpace(accountPrefix))
                    AddParam(cmd, "@prefix", $"{accountPrefix.Trim()}%");

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    rows.Add(new ChartAccountLookupDto
                    {
                        AccountNo = reader["AccountNo"]?.ToString() ?? string.Empty,
                        AccountName = reader["AccountName"]?.ToString() ?? string.Empty,
                        AccountNameUrdu = reader["AccountNameUrdu"]?.ToString() ?? string.Empty,
                        AccountType = reader["AccountType"]?.ToString() ?? string.Empty,
                        Bank = reader["Bank"]?.ToString() ?? string.Empty,
                        CropSeason = reader["CropSeason"]?.ToString() ?? string.Empty
                    });
                }
                return rows;
            }

            string searchWhere = string.IsNullOrWhiteSpace(filter)
                ? string.Empty
                : " AND (AccountNo LIKE @q OR AccountName LIKE @q OR AccountNameUrdu LIKE @q OR Bank LIKE @q)";

            if (bankOnly && !string.IsNullOrWhiteSpace(accountPrefix))
            {
                list = await QueryAsync($" AND AccountNo LIKE @prefix{searchWhere}");
                if (list.Count == 0 && string.IsNullOrWhiteSpace(filter))
                {
                    list = await QueryAsync(" AND (AccountType = 'B' OR LEN(ISNULL(Bank, N'')) > 0 OR AccountName LIKE '%Bank%' OR AccountName LIKE N'%بینک%')");
                }
                if (list.Count == 0 && string.IsNullOrWhiteSpace(filter))
                {
                    list = await QueryAsync(string.Empty);
                }
                return list;
            }

            if (bankOnly)
            {
                list = await QueryAsync($" AND (AccountType = 'B' OR LEN(ISNULL(Bank, N'')) > 0 OR AccountName LIKE '%Bank%' OR AccountName LIKE N'%بینک%'){searchWhere}");
                if (list.Count == 0 && string.IsNullOrWhiteSpace(filter))
                    list = await QueryAsync(string.Empty);
                return list;
            }

            return await QueryAsync(searchWhere);
        }

        if (await ObjectExistsAsync(context, "Accounts", "U"))
        {
            var q = (filter ?? string.Empty).Trim().ToLower();
            var acs = await context.Accounts.Where(a => !a.IsDeleted).OrderBy(a => a.AccountNo).ToListAsync();
            foreach (var a in acs)
            {
                if (!string.IsNullOrEmpty(q) &&
                    !(a.AccountNo.ToLower().Contains(q) || a.Name.ToLower().Contains(q) || (a.UrduName ?? "").Contains(filter!.Trim())))
                    continue;
                list.Add(new ChartAccountLookupDto
                {
                    AccountNo = a.AccountNo,
                    AccountName = a.Name,
                    AccountNameUrdu = a.UrduName ?? string.Empty
                });
            }
        }
        return list;
    }

    public async Task<(decimal Balance, string FormattedText)> GetAccountBalanceTextAsync(string accountNo, DateTime asOfDate)
    {
        if (string.IsNullOrWhiteSpace(accountNo))
            return (0m, "0 Dr");

        var clean = accountNo.Trim();
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await context.Database.OpenConnectionAsync();

        // 1. Try AgenSoft SPAccAccountsBalancesFinal first
        try
        {
            using var spCmd = conn.CreateCommand();
            spCmd.CommandText = "EXEC SPAccAccountsBalancesFinal @acc, @toDate";
            AddParam(spCmd, "@acc", clean);
            AddParam(spCmd, "@toDate", asOfDate.Date, System.Data.DbType.Date);
            using var reader = await spCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                decimal dr = 0, cr = 0;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var col = reader.GetName(i);
                    if (col.Equals("Debit", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                        dr = Convert.ToDecimal(reader[i]);
                    else if (col.Equals("Credit", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                        cr = Convert.ToDecimal(reader[i]);
                    else if (col.Equals("Balance", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                    {
                        var balVal = Convert.ToDecimal(reader[i]);
                        return (balVal, $"{Math.Abs(balVal):N0} {(balVal >= 0 ? "Dr" : "Cr")}");
                    }
                }
                var net = dr - cr;
                return (net, $"{Math.Abs(net):N0} {(net >= 0 ? "Dr" : "Cr")}");
            }
        }
        catch { }

        // 2. Fallback: ChartOfAccounts opening + VuAccVouchersAccounts (+ VuAccVouchersOthers if functional)
        decimal opening = 0m;
        decimal txNet = 0m;
        try
        {
            if (await ObjectExistsAsync(context, "ChartOfAccounts", "U"))
            {
                using var coaCmd = conn.CreateCommand();
                coaCmd.CommandText = "SELECT ISNULL(OpeningDebit, 0) - ISNULL(OpeningCredit, 0) FROM ChartOfAccounts WHERE AccountNo = @acc";
                AddParam(coaCmd, "@acc", clean);
                var obj = await coaCmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                    opening = Convert.ToDecimal(obj);
            }

            if (await ObjectExistsAsync(context, "VuAccVouchersAccounts", "V"))
            {
                using var vCmd = conn.CreateCommand();
                vCmd.CommandText = "SELECT ISNULL(SUM(ISNULL(Debit, 0) - ISNULL(Credit, 0)), 0) FROM VuAccVouchersAccounts WHERE AccountNo = @acc AND CONVERT(date, VoucherDate) <= @toDate";
                AddParam(vCmd, "@acc", clean);
                AddParam(vCmd, "@toDate", asOfDate.Date, System.Data.DbType.Date);
                var obj = await vCmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                    txNet += Convert.ToDecimal(obj);
            }

            try
            {
                if (await ObjectExistsAsync(context, "VuAccVouchersOthers", "V"))
                {
                    using var oCmd = conn.CreateCommand();
                    oCmd.CommandText = "SELECT ISNULL(SUM(ISNULL(Debit, 0) - ISNULL(Credit, 0)), 0) FROM VuAccVouchersOthers WHERE AccountNo = @acc AND CONVERT(date, VoucherDate) <= @toDate";
                    AddParam(oCmd, "@acc", clean);
                    AddParam(oCmd, "@toDate", asOfDate.Date, System.Data.DbType.Date);
                    var obj = await oCmd.ExecuteScalarAsync();
                    if (obj != null && obj != DBNull.Value)
                        txNet += Convert.ToDecimal(obj);
                }
            }
            catch { }

            var total = opening + txNet;
            return (total, $"{Math.Abs(total):N0} {(total >= 0 ? "Dr" : "Cr")}");
        }
        catch { }

        return (0m, "0 Dr");
    }

    public async Task<ChequeDepositDto?> LoadChequeDepositByVoucherNoAsync(long voucherNo, DateTime? voucherDate = null, bool? requireIsCheque = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        if (await ObjectExistsAsync(context, "BankDeposit", "U"))
        {
            var conn = context.Database.GetDbConnection();
            using var cmd = conn.CreateCommand();
            var dateClause = voucherDate.HasValue
                ? " AND CONVERT(date, bd.VoucherDate) = @vDate"
                : string.Empty;

            cmd.CommandText = $@"
                SELECT TOP 1 bd.VoucherNo, bd.VoucherDate, bd.IsCheque,
                       ISNULL(bd.ChequeNo, '') AS ChequeNo,
                       bd.ChequeDate,
                       ISNULL(bd.SlipNo, '') AS SlipNo,
                       bd.DepositedDate,
                       bd.ReconcileDate,
                       ISNULL(bd.BankAccountNo, '') AS BankAccountNo,
                       ISNULL(c1.AccountName, '') AS BankAccountName,
                       ISNULL(bd.AccountNo, '') AS PartyAccountNo,
                       ISNULL(c2.AccountName, '') AS PartyAccountName,
                       bd.UserNo,
                       ISNULL(u.UserName, '') AS CreatedByUserName,
                       bd.Amount,
                       ISNULL(bd.DepositedBy, '') AS DepositedBy,
                       ISNULL(bd.Narration, '') AS Narration,
                       ISNULL(bd.NarrationUrdu, N'') AS NarrationUrdu,
                       bd.IsBounced,
                       bd.IsPosted,
                       bd.VoucherTime,
                       bd.FPData,
                       ISNULL(bd.CropSeason, '') AS CropSeason,
                       ISNULL(bd.BranchID, 1) AS BranchID,
                       ISNULL(bd.IDBranch, bd.VoucherNo) AS IDBranch
                FROM BankDeposit bd
                LEFT JOIN ChartOfAccounts c1 ON bd.BankAccountNo = c1.AccountNo
                LEFT JOIN ChartOfAccounts c2 ON bd.AccountNo = c2.AccountNo
                LEFT JOIN Users u ON bd.UserNo = u.UserID
                WHERE bd.VoucherNo = @vNo{dateClause}
                ORDER BY bd.VoucherDate DESC";

            AddParam(cmd, "@vNo", voucherNo);
            if (voucherDate.HasValue)
                AddParam(cmd, "@vDate", voucherDate.Value.Date, System.Data.DbType.Date);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                bool isChequeRow = Convert.ToBoolean(reader["IsCheque"]);
                if (requireIsCheque.HasValue && isChequeRow != requireIsCheque.Value)
                {
                    throw new InvalidOperationException(requireIsCheque.Value
                        ? $"Voucher #{voucherNo} is a Cash Deposit voucher, not a Cheque Deposit."
                        : $"Voucher #{voucherNo} is a Cheque Deposit voucher, not a Cash Deposit.");
                }

                var vDate = Convert.ToDateTime(reader["VoucherDate"]);
                var hasDepDate = reader["DepositedDate"] != DBNull.Value;
                var depDate = hasDepDate ? Convert.ToDateTime(reader["DepositedDate"]) : vDate;
                var chqDate = reader["ChequeDate"] != DBNull.Value ? Convert.ToDateTime(reader["ChequeDate"]) : vDate;

                return new ChequeDepositDto
                {
                    VoucherNo = Convert.ToInt64(reader["VoucherNo"]),
                    VoucherDate = vDate,
                    OriginalVoucherDate = vDate,
                    IsExisting = true,
                    IsCheque = isChequeRow,
                    ChequeNo = reader["ChequeNo"]?.ToString() ?? string.Empty,
                    ChequeDate = chqDate,
                    SlipNo = reader["SlipNo"]?.ToString() ?? string.Empty,
                    DepositedDate = depDate,
                    IsDepositConfirmed = hasDepDate,
                    IsReconciled = reader["ReconcileDate"] != DBNull.Value,
                    ReconcileDate = reader["ReconcileDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReconcileDate"]) : null,
                    BankAccountNo = reader["BankAccountNo"]?.ToString() ?? string.Empty,
                    BankAccountName = reader["BankAccountName"]?.ToString() ?? string.Empty,
                    PartyAccountNo = reader["PartyAccountNo"]?.ToString() ?? string.Empty,
                    PartyAccountName = reader["PartyAccountName"]?.ToString() ?? string.Empty,
                    UserNo = reader["UserNo"] != DBNull.Value ? Convert.ToInt16(reader["UserNo"]) : (short)1,
                    CreatedByUserName = reader["CreatedByUserName"]?.ToString() ?? string.Empty,
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    DepositedBy = reader["DepositedBy"]?.ToString() ?? string.Empty,
                    Narration = reader["Narration"]?.ToString() ?? string.Empty,
                    NarrationUrdu = reader["NarrationUrdu"]?.ToString() ?? string.Empty,
                    IsBounced = reader["IsBounced"] != DBNull.Value && Convert.ToBoolean(reader["IsBounced"]),
                    IsPosted = reader["IsPosted"] != DBNull.Value && Convert.ToBoolean(reader["IsPosted"]),
                    VoucherTime = reader["VoucherTime"] != DBNull.Value ? Convert.ToDateTime(reader["VoucherTime"]) : null,
                    FPData = reader["FPData"] != DBNull.Value ? (byte[])reader["FPData"] : null,
                    CropSeason = reader["CropSeason"]?.ToString() ?? string.Empty,
                    BranchId = Convert.ToInt32(reader["BranchID"]),
                    IdBranch = reader["IDBranch"] != DBNull.Value ? Convert.ToInt64(reader["IDBranch"]) : voucherNo
                };
            }
            return null;
        }

        // EF fallback if BankDeposit does not exist
        if (await ObjectExistsAsync(context, "BankDeposits", "U"))
        {
            var efDeposit = await context.BankDeposits
                .Include(d => d.BankAccount).ThenInclude(b => b.Account)
                .FirstOrDefaultAsync(d => d.Id == (int)voucherNo);

            if (efDeposit != null)
            {
                return new ChequeDepositDto
                {
                    VoucherNo = efDeposit.Id,
                    VoucherDate = efDeposit.DepositDate,
                    OriginalVoucherDate = efDeposit.DepositDate,
                    IsExisting = true,
                    IsCheque = efDeposit.DepositType == DepositType.Cheque,
                    BankAccountId = efDeposit.BankAccountId,
                    BankAccountNo = efDeposit.BankAccount?.Account?.AccountNo ?? efDeposit.BankAccount?.AccountNumber ?? string.Empty,
                    BankAccountName = efDeposit.BankAccount?.BankName ?? string.Empty,
                    Amount = efDeposit.Amount,
                    DepositedDate = efDeposit.DepositDate,
                    IsReconciled = efDeposit.IsReconciled,
                    ReconcileDate = efDeposit.ReconciledDate,
                    SlipNo = efDeposit.ReferenceNo ?? string.Empty
                };
            }
        }

        return null;
    }

    public async Task<IEnumerable<ChequeDepositDto>> SearchChequeDepositsAsync(
        string? query = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool? isCheque = null,
        string? depositedBy = null,
        string? cropSeason = null,
        int? branchId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var results = new List<ChequeDepositDto>();

        try
        {
            if (!await ObjectExistsAsync(context, "BankDeposit", "U"))
                return results;

            var conn = context.Database.GetDbConnection();
            using var cmd = conn.CreateCommand();
            var sql = new System.Text.StringBuilder(@"
                SELECT TOP 500 bd.VoucherNo, bd.VoucherDate, bd.IsCheque,
                       ISNULL(bd.ChequeNo, '') AS ChequeNo,
                       bd.ChequeDate,
                       ISNULL(bd.SlipNo, '') AS SlipNo,
                       bd.DepositedDate,
                       bd.ReconcileDate,
                       ISNULL(bd.BankAccountNo, '') AS BankAccountNo,
                       ISNULL(c1.AccountName, '') AS BankAccountName,
                       ISNULL(bd.AccountNo, '') AS PartyAccountNo,
                       ISNULL(c2.AccountName, '') AS PartyAccountName,
                       bd.Amount,
                       ISNULL(bd.DepositedBy, '') AS DepositedBy,
                       ISNULL(bd.Narration, '') AS Narration,
                       ISNULL(bd.NarrationUrdu, N'') AS NarrationUrdu,
                       bd.IsBounced,
                       bd.IsPosted,
                       bd.VoucherTime,
                       ISNULL(bd.CropSeason, '') AS CropSeason,
                       ISNULL(bd.BranchID, 1) AS BranchID,
                       ISNULL(bd.IDBranch, bd.VoucherNo) AS IDBranch
                FROM BankDeposit bd
                LEFT JOIN ChartOfAccounts c1 ON bd.BankAccountNo = c1.AccountNo
                LEFT JOIN ChartOfAccounts c2 ON bd.AccountNo = c2.AccountNo
                WHERE 1=1");

            if (isCheque.HasValue)
            {
                sql.Append(" AND bd.IsCheque = @isCheque");
                AddParam(cmd, "@isCheque", isCheque.Value);
            }
            if (fromDate.HasValue)
            {
                sql.Append(" AND CONVERT(date, bd.VoucherDate) >= @fromDate");
                AddParam(cmd, "@fromDate", fromDate.Value.Date, System.Data.DbType.Date);
            }
            if (toDate.HasValue)
            {
                sql.Append(" AND CONVERT(date, bd.VoucherDate) <= @toDate");
                AddParam(cmd, "@toDate", toDate.Value.Date, System.Data.DbType.Date);
            }
            if (!string.IsNullOrWhiteSpace(depositedBy))
            {
                sql.Append(" AND bd.DepositedBy LIKE @depBy");
                AddParam(cmd, "@depBy", $"%{depositedBy.Trim()}%");
            }
            if (!string.IsNullOrWhiteSpace(cropSeason))
            {
                sql.Append(" AND bd.CropSeason = @season");
                AddParam(cmd, "@season", Trunc(cropSeason, 5));
            }
            if (branchId.HasValue && branchId.Value > 0)
            {
                sql.Append(" AND bd.BranchID = @branchId");
                AddParam(cmd, "@branchId", branchId.Value);
            }
            if (!string.IsNullOrWhiteSpace(query))
            {
                sql.Append(" AND (CAST(bd.VoucherNo AS VARCHAR(30)) LIKE @q OR CAST(bd.IDBranch AS VARCHAR(30)) LIKE @q OR bd.ChequeNo LIKE @q OR bd.SlipNo LIKE @q OR c1.AccountName LIKE @q OR c2.AccountName LIKE @q OR bd.DepositedBy LIKE @q)");
                AddParam(cmd, "@q", $"%{query.Trim()}%");
            }

            sql.Append(" ORDER BY bd.VoucherDate DESC, bd.VoucherNo DESC");
            cmd.CommandText = sql.ToString();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var vDate = Convert.ToDateTime(reader["VoucherDate"]);
                var hasDep = reader["DepositedDate"] != DBNull.Value;
                results.Add(new ChequeDepositDto
                {
                    VoucherNo = Convert.ToInt64(reader["VoucherNo"]),
                    VoucherDate = vDate,
                    OriginalVoucherDate = vDate,
                    IsExisting = true,
                    IsCheque = Convert.ToBoolean(reader["IsCheque"]),
                    ChequeNo = reader["ChequeNo"]?.ToString() ?? string.Empty,
                    ChequeDate = reader["ChequeDate"] != DBNull.Value ? Convert.ToDateTime(reader["ChequeDate"]) : vDate,
                    SlipNo = reader["SlipNo"]?.ToString() ?? string.Empty,
                    DepositedDate = hasDep ? Convert.ToDateTime(reader["DepositedDate"]) : vDate,
                    IsDepositConfirmed = hasDep,
                    IsReconciled = reader["ReconcileDate"] != DBNull.Value,
                    ReconcileDate = reader["ReconcileDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReconcileDate"]) : null,
                    BankAccountNo = reader["BankAccountNo"]?.ToString() ?? string.Empty,
                    BankAccountName = reader["BankAccountName"]?.ToString() ?? string.Empty,
                    PartyAccountNo = reader["PartyAccountNo"]?.ToString() ?? string.Empty,
                    PartyAccountName = reader["PartyAccountName"]?.ToString() ?? string.Empty,
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    DepositedBy = reader["DepositedBy"]?.ToString() ?? string.Empty,
                    Narration = reader["Narration"]?.ToString() ?? string.Empty,
                    NarrationUrdu = reader["NarrationUrdu"]?.ToString() ?? string.Empty,
                    IsBounced = reader["IsBounced"] != DBNull.Value && Convert.ToBoolean(reader["IsBounced"]),
                    IsPosted = reader["IsPosted"] != DBNull.Value && Convert.ToBoolean(reader["IsPosted"]),
                    VoucherTime = reader["VoucherTime"] != DBNull.Value ? Convert.ToDateTime(reader["VoucherTime"]) : null,
                    CropSeason = reader["CropSeason"]?.ToString() ?? string.Empty,
                    BranchId = Convert.ToInt32(reader["BranchID"]),
                    IdBranch = reader["IDBranch"] != DBNull.Value ? Convert.ToInt64(reader["IDBranch"]) : Convert.ToInt64(reader["VoucherNo"])
                });
            }
        }
        catch { }

        return results;
    }

    public async Task SaveChequeDepositAsync(ChequeDepositDto dto)
    {
        if (dto.Amount <= 0)
            throw new InvalidOperationException("Please Enter Amount.");
        if (dto.Amount >= 1_000_000_000m)
            throw new InvalidOperationException("Amount must be less than 1 billion ");

        using var context = await _contextFactory.CreateDbContextAsync();

        // 1. Validate Bank Account against ChartOfAccounts (or EF fallback)
        var bankAcNo = Trunc(dto.BankAccountNo, 10);
        if (string.IsNullOrEmpty(bankAcNo) && dto.BankAccountId > 0 && await ObjectExistsAsync(context, "BankAccounts", "U"))
        {
            var efBank = await context.BankAccounts.Include(b => b.Account).FirstOrDefaultAsync(b => b.Id == dto.BankAccountId);
            if (efBank != null) bankAcNo = Trunc(efBank.Account?.AccountNo ?? efBank.AccountNumber, 10);
        }
        var validBank = await ValidateChartAccountAsync(bankAcNo);
        if (validBank == null)
            throw new InvalidOperationException("Please specify a valid Bank A/C no.");

        // 2. Validate Party / Received By Account against ChartOfAccounts (for Cash Deposit default 1001 allowed)
        var partyAcNo = Trunc(dto.PartyAccountNo, 10);
        if (!dto.IsCheque && string.IsNullOrEmpty(partyAcNo))
            partyAcNo = "1001";
        var validParty = !string.IsNullOrEmpty(partyAcNo) ? await ValidateChartAccountAsync(partyAcNo) : null;
        if (dto.IsCheque && validParty == null)
            throw new InvalidOperationException("Please specify a valid Received By A/C no.");

        // 3. Resolve CropSeason & Branch
        var cropSeasonStr = dto.CropSeason ?? string.Empty;
        if (string.IsNullOrEmpty(cropSeasonStr) && dto.CropSeasonId.HasValue && dto.CropSeasonId.Value > 0 && await ObjectExistsAsync(context, "CropSeasons", "U"))
        {
            var season = await context.CropSeasons.FindAsync(dto.CropSeasonId.Value);
            cropSeasonStr = season?.Name ?? string.Empty;
        }
        cropSeasonStr = Trunc(cropSeasonStr, 5);
        int branchId = dto.BranchId > 0 ? dto.BranchId : 1;
        short userNo = dto.UserNo > 0 ? dto.UserNo : (short)1;

        // 4. AgenSoft Dates & Posting parity:
        // DepositedDate = ChkDepositConfirmed ? ChequeDate : NULL (for ChequeDeposit; CashDeposit uses dto.DepositedDate)
        // ReconcileDate = chkReconcile ? ChequeDate : NULL
        // IsPosted = 1 ONLY when ReconcileDate IS NOT NULL AND IsBounced = 0
        DateTime? depDateVal = dto.IsDepositConfirmed
            ? (dto.IsCheque ? dto.ChequeDate.Date : dto.DepositedDate.Date)
            : null;
        DateTime? recDateVal = dto.IsReconciled
            ? (dto.ReconcileDate?.Date ?? (dto.IsCheque ? dto.ChequeDate.Date : dto.DepositedDate.Date))
            : null;
        DateTime voucherTimeVal = dto.VoucherTime ?? DateTime.Now;
        long idBranch = dto.IdBranch > 0 ? dto.IdBranch : dto.VoucherNo;

        // ══════ AGENSOFT DATABASE SYNC (BankDeposit TABLE) ══════
        if (await ObjectExistsAsync(context, "BankDeposit", "U"))
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await context.Database.OpenConnectionAsync();

            using var tx = await conn.BeginTransactionAsync();
            try
            {
                DateTime origDate = (dto.OriginalVoucherDate ?? dto.VoucherDate).Date;
                DateTime targetDate = dto.VoucherDate.Date;

                // Unpost prior row if updating existing voucher
                if (dto.IsExisting)
                {
                    using var unpostCmd = conn.CreateCommand();
                    unpostCmd.Transaction = tx;
                    unpostCmd.CommandText = "UPDATE BankDeposit SET IsPosted = 0 WHERE VoucherNo = @vNo AND CONVERT(date, VoucherDate) = @origDate";
                    AddParam(unpostCmd, "@vNo", dto.VoucherNo);
                    AddParam(unpostCmd, "@origDate", origDate, System.Data.DbType.Date);
                    await unpostCmd.ExecuteNonQueryAsync();
                }

                // Check if row exists for key (VoucherNo, VoucherDate)
                bool rowExistsAtTarget;
                using (var chkCmd = conn.CreateCommand())
                {
                    chkCmd.Transaction = tx;
                    chkCmd.CommandText = "SELECT COUNT(*) FROM BankDeposit WHERE VoucherNo = @vNo AND CONVERT(date, VoucherDate) = @vDate";
                    AddParam(chkCmd, "@vNo", dto.VoucherNo);
                    AddParam(chkCmd, "@vDate", targetDate, System.Data.DbType.Date);
                    rowExistsAtTarget = Convert.ToInt32(await chkCmd.ExecuteScalarAsync() ?? 0) > 0;
                }

                if (dto.IsExisting && origDate != targetDate)
                {
                    if (rowExistsAtTarget)
                        throw new InvalidOperationException($"Voucher #{dto.VoucherNo} already exists on {targetDate:dd-MM-yyyy}.");

                    using var delOld = conn.CreateCommand();
                    delOld.Transaction = tx;
                    delOld.CommandText = "DELETE FROM BankDeposit WHERE VoucherNo = @vNo AND CONVERT(date, VoucherDate) = @origDate";
                    AddParam(delOld, "@vNo", dto.VoucherNo);
                    AddParam(delOld, "@origDate", origDate, System.Data.DbType.Date);
                    await delOld.ExecuteNonQueryAsync();
                    rowExistsAtTarget = false;
                }

                if (rowExistsAtTarget && !dto.IsExisting)
                {
                    throw new InvalidOperationException("DUPLICATE_VOUCHER_ID");
                }

                using (var saveCmd = conn.CreateCommand())
                {
                    saveCmd.Transaction = tx;
                    if (rowExistsAtTarget)
                    {
                        saveCmd.CommandText = @"
                            UPDATE BankDeposit SET
                                IsCheque = @isCheque,
                                ChequeNo = @chequeNo,
                                ChequeDate = @chequeDate,
                                SlipNo = @slipNo,
                                DepositedDate = @depDate,
                                ReconcileDate = @recDate,
                                BankAccountNo = @bankAc,
                                AccountNo = @partyAc,
                                UserNo = @userNo,
                                Amount = @amount,
                                DepositedBy = @depBy,
                                Narration = @narration,
                                IsBounced = @isBounced,
                                IsPosted = 0,
                                NarrationUrdu = @narrationUrdu,
                                VoucherTime = @vTime,
                                FPData = @fpData,
                                CropSeason = @season,
                                BranchID = @branchId,
                                IDBranch = @idBranch
                            WHERE VoucherNo = @vNo AND CONVERT(date, VoucherDate) = @vDate";
                    }
                    else
                    {
                        saveCmd.CommandText = @"
                            INSERT INTO BankDeposit (
                                VoucherNo, VoucherDate, IsCheque, ChequeNo, ChequeDate,
                                SlipNo, DepositedDate, ReconcileDate, BankAccountNo, AccountNo,
                                UserNo, Amount, DepositedBy, Narration, IsBounced,
                                IsPosted, NarrationUrdu, VoucherTime, FPData, CropSeason, BranchID, IDBranch
                            ) VALUES (
                                @vNo, @vDate, @isCheque, @chequeNo, @chequeDate,
                                @slipNo, @depDate, @recDate, @bankAc, @partyAc,
                                @userNo, @amount, @depBy, @narration, @isBounced,
                                0, @narrationUrdu, @vTime, @fpData, @season, @branchId, @idBranch
                            )";
                    }

                    AddParam(saveCmd, "@vNo", dto.VoucherNo);
                    AddParam(saveCmd, "@vDate", targetDate, System.Data.DbType.Date);
                    AddParam(saveCmd, "@isCheque", dto.IsCheque);
                    AddParam(saveCmd, "@chequeNo", dto.IsCheque ? Trunc(dto.ChequeNo, 15) : string.Empty);
                    AddParam(saveCmd, "@chequeDate", dto.IsCheque ? dto.ChequeDate.Date : DBNull.Value, System.Data.DbType.Date);
                    AddParam(saveCmd, "@slipNo", Trunc(dto.SlipNo, 15));
                    AddParam(saveCmd, "@depDate", depDateVal.HasValue ? depDateVal.Value : DBNull.Value, System.Data.DbType.Date);
                    AddParam(saveCmd, "@recDate", recDateVal.HasValue ? recDateVal.Value : DBNull.Value, System.Data.DbType.Date);
                    AddParam(saveCmd, "@bankAc", bankAcNo);
                    AddParam(saveCmd, "@partyAc", string.IsNullOrEmpty(partyAcNo) ? DBNull.Value : partyAcNo);
                    AddParam(saveCmd, "@userNo", userNo);
                    AddParam(saveCmd, "@amount", dto.Amount);
                    AddParam(saveCmd, "@depBy", Trunc(dto.DepositedBy, 30));
                    AddParam(saveCmd, "@narration", Trunc(dto.Narration, 1000));
                    AddParam(saveCmd, "@isBounced", dto.IsBounced);
                    AddParam(saveCmd, "@narrationUrdu", Trunc(dto.NarrationUrdu, 1000));
                    AddParam(saveCmd, "@vTime", voucherTimeVal, System.Data.DbType.DateTime);
                    AddParam(saveCmd, "@fpData", dto.FPData, System.Data.DbType.Binary);
                    AddParam(saveCmd, "@season", cropSeasonStr);
                    AddParam(saveCmd, "@branchId", branchId);
                    AddParam(saveCmd, "@idBranch", idBranch);

                    await saveCmd.ExecuteNonQueryAsync();
                }

                // Exact AgenSoft ReconcileDate -> IsPosted = 1 rule
                if (recDateVal.HasValue && !dto.IsBounced)
                {
                    using var postCmd = conn.CreateCommand();
                    postCmd.Transaction = tx;
                    postCmd.CommandText = "UPDATE BankDeposit SET IsPosted = 1 WHERE VoucherNo = @vNo AND CONVERT(date, VoucherDate) = @vDate AND ReconcileDate IS NOT NULL AND IsBounced = 0";
                    AddParam(postCmd, "@vNo", dto.VoucherNo);
                    AddParam(postCmd, "@vDate", targetDate, System.Data.DbType.Date);
                    await postCmd.ExecuteNonQueryAsync();
                }

                // Insert UserHistory log (only if Users has valid user or no FK failure)
                try
                {
                    using var histChk = conn.CreateCommand();
                    histChk.Transaction = tx;
                    histChk.CommandText = "SELECT CASE WHEN OBJECT_ID('UserHistory', 'U') IS NOT NULL THEN 1 ELSE 0 END";
                    if (Convert.ToInt32(await histChk.ExecuteScalarAsync() ?? 0) == 1)
                    {
                        using var hCmd = conn.CreateCommand();
                        hCmd.Transaction = tx;
                        hCmd.CommandText = "INSERT INTO UserHistory (UserID, Description, LogTime) VALUES (@uid, @desc, GETDATE())";
                        var actionLabel = dto.IsExisting ? "UPDATED/MODIFIED" : "INSERTED";
                        var formLabel = dto.IsCheque ? "BANK CHEQUE DEPOSIT" : "BANK CASH DEPOSIT";
                        AddParam(hCmd, "@uid", userNo);
                        AddParam(hCmd, "@desc", Trunc($"{actionLabel} {formLabel} # {dto.VoucherNo}", 1000));
                        await hCmd.ExecuteNonQueryAsync();
                    }
                }
                catch { }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            // SMS queue insertion parity (after commit so balance includes this deposit if posted)
            if (dto.SendSms && dto.IsCheque && !string.IsNullOrWhiteSpace(partyAcNo))
            {
                try
                {
                    if (await ObjectExistsAsync(context, "ComSettings", "U") &&
                        await ObjectExistsAsync(context, "ComRecords", "U") &&
                        await ObjectExistsAsync(context, "Parties", "U"))
                    {
                        string fromNo = string.Empty;
                        using (var sCmd = conn.CreateCommand())
                        {
                            sCmd.CommandText = "SELECT TOP 1 ISNULL(ControlNo, '') FROM ComSettings";
                            fromNo = (await sCmd.ExecuteScalarAsync())?.ToString()?.Trim() ?? string.Empty;
                        }

                        string partyName = validParty?.AccountName ?? dto.PartyAccountName;
                        string mobileNo = string.Empty;
                        using (var pCmd = conn.CreateCommand())
                        {
                            pCmd.CommandText = "SELECT TOP 1 ISNULL(PartyName, ''), REPLACE(ISNULL(Mobile, ''), '-', '') AS MobileNo FROM Parties WHERE PartyID = @pid";
                            AddParam(pCmd, "@pid", partyAcNo);
                            using var pr = await pCmd.ExecuteReaderAsync();
                            if (await pr.ReadAsync())
                            {
                                var pNameDb = pr[0]?.ToString()?.Trim();
                                if (!string.IsNullOrEmpty(pNameDb)) partyName = pNameDb;
                                mobileNo = pr[1]?.ToString()?.Trim() ?? string.Empty;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(mobileNo))
                        {
                            var (_, balText) = await GetAccountBalanceTextAsync(partyAcNo, dto.VoucherDate.Date);
                            var body = $"Dear {partyName}, we received Rs.{dto.Amount:0.##} by cheque#{dto.ChequeNo} on {dto.VoucherDate:dd-MM-yy} new balance: {balText} Thanks.";
                            using var insSms = conn.CreateCommand();
                            insSms.CommandText = @"
                                INSERT INTO ComRecords (FromNo, ToNo, Body, SourceType, ComTime, IsSMS, IsProcessed, UnitID, IsWebRequest)
                                VALUES (@fromNo, @toNo, @body, 'ChequeDeposit', GETDATE(), 1, 0, @unitId, 0)";
                            AddParam(insSms, "@fromNo", Trunc(fromNo, 20));
                            AddParam(insSms, "@toNo", Trunc(mobileNo, 20));
                            AddParam(insSms, "@body", Trunc(body, 3500));
                            AddParam(insSms, "@unitId", Trunc(branchId.ToString(), 20));
                            await insSms.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch { }
            }
        }

        // ══════ EF CORE ENTITY & GL SYNC (Only when EF schema exists alongside legacy DB) ══════
        if (await ObjectExistsAsync(context, "BankDeposits", "U") && await ObjectExistsAsync(context, "BankAccounts", "U"))
        {
            var bankAccount = await context.BankAccounts.Include(b => b.Account)
                .FirstOrDefaultAsync(b => b.Id == dto.BankAccountId || b.Account.AccountNo == bankAcNo || b.AccountNumber == bankAcNo);
            if (bankAccount != null)
            {
                var partyAccount = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == partyAcNo);
                var currentYear = await _systemService.GetCurrentYearAsync();
                int finYearId = currentYear?.Id ?? 1;

                var existingDeposit = await context.BankDeposits.FirstOrDefaultAsync(d => d.Id == (int)dto.VoucherNo);
                if (existingDeposit == null)
                {
                    existingDeposit = new BankDeposit
                    {
                        Id = (int)dto.VoucherNo,
                        BankAccountId = bankAccount.Id,
                        DepositType = dto.IsCheque ? DepositType.Cheque : DepositType.Cash,
                        Amount = dto.Amount,
                        DepositDate = depDateVal ?? dto.VoucherDate.Date,
                        ReferenceNo = dto.IsCheque ? $"Chq: {dto.ChequeNo} / Slip: {dto.SlipNo}" : $"Slip: {dto.SlipNo}",
                        IsReconciled = dto.IsReconciled,
                        ReconciledDate = recDateVal,
                        CreatedAt = DateTime.Now
                    };
                    context.BankDeposits.Add(existingDeposit);
                }
                else
                {
                    existingDeposit.BankAccountId = bankAccount.Id;
                    existingDeposit.DepositType = dto.IsCheque ? DepositType.Cheque : DepositType.Cash;
                    existingDeposit.Amount = dto.Amount;
                    existingDeposit.DepositDate = depDateVal ?? dto.VoucherDate.Date;
                    existingDeposit.ReferenceNo = dto.IsCheque ? $"Chq: {dto.ChequeNo} / Slip: {dto.SlipNo}" : $"Slip: {dto.SlipNo}";
                    existingDeposit.IsReconciled = dto.IsReconciled;
                    existingDeposit.ReconciledDate = recDateVal;
                    existingDeposit.UpdatedAt = DateTime.Now;
                }

                var oldGL = await context.GeneralLedger
                    .Where(g => g.SourceType == SourceType.Voucher && g.SourceId == (int)dto.VoucherNo && g.Narration != null && (g.Narration.Contains("Bank Cheque Deposit") || g.Narration.Contains("Bank Cash Deposit")))
                    .ToListAsync();
                if (oldGL.Any()) context.GeneralLedger.RemoveRange(oldGL);

                if (dto.IsReconciled && !dto.IsBounced)
                {
                    string vTitle = dto.IsCheque ? "Bank Cheque Deposit" : "Bank Cash Deposit";
                    context.GeneralLedger.Add(new GeneralLedger
                    {
                        TransactionDate = dto.VoucherDate.Date,
                        AccountId = bankAccount.AccountId,
                        Debit = dto.Amount,
                        Credit = 0,
                        Narration = dto.IsCheque
                            ? $"{vTitle} #{dto.VoucherNo} (Chq: {dto.ChequeNo}, Slip: {dto.SlipNo}) - {dto.Narration}"
                            : $"{vTitle} #{dto.VoucherNo} (Slip: {dto.SlipNo}) - {dto.Narration}",
                        SourceType = SourceType.Voucher,
                        SourceId = (int)dto.VoucherNo,
                        FinancialYearId = finYearId,
                        CropSeasonId = dto.CropSeasonId
                    });

                    if (partyAccount != null)
                    {
                        context.GeneralLedger.Add(new GeneralLedger
                        {
                            TransactionDate = dto.VoucherDate.Date,
                            AccountId = partyAccount.Id,
                            Debit = 0,
                            Credit = dto.Amount,
                            Narration = dto.IsCheque
                                ? $"{vTitle} #{dto.VoucherNo} to Bank {bankAccount.BankName} (Chq: {dto.ChequeNo}) - {dto.Narration}"
                                : $"{vTitle} #{dto.VoucherNo} to Bank {bankAccount.BankName} - {dto.Narration}",
                            SourceType = SourceType.Voucher,
                            SourceId = (int)dto.VoucherNo,
                            FinancialYearId = finYearId,
                            CropSeasonId = dto.CropSeasonId
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }

    public async Task DeleteChequeDepositAsync(long voucherNo, DateTime? voucherDate = null, short userId = 1)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        if (await ObjectExistsAsync(context, "BankDeposit", "U"))
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await context.Database.OpenConnectionAsync();

            using var tx = await conn.BeginTransactionAsync();
            try
            {
                string where = voucherDate.HasValue
                    ? " WHERE VoucherNo = @vNo AND CONVERT(date, VoucherDate) = @vDate"
                    : " WHERE VoucherNo = @vNo";

                using (var upCmd = conn.CreateCommand())
                {
                    upCmd.Transaction = tx;
                    upCmd.CommandText = $"UPDATE BankDeposit SET IsPosted = 0{where}";
                    AddParam(upCmd, "@vNo", voucherNo);
                    if (voucherDate.HasValue) AddParam(upCmd, "@vDate", voucherDate.Value.Date, System.Data.DbType.Date);
                    await upCmd.ExecuteNonQueryAsync();
                }

                using (var delCmd = conn.CreateCommand())
                {
                    delCmd.Transaction = tx;
                    delCmd.CommandText = $"DELETE FROM BankDeposit{where}";
                    AddParam(delCmd, "@vNo", voucherNo);
                    if (voucherDate.HasValue) AddParam(delCmd, "@vDate", voucherDate.Value.Date, System.Data.DbType.Date);
                    await delCmd.ExecuteNonQueryAsync();
                }

                try
                {
                    if (await ObjectExistsAsync(context, "UserHistory", "U"))
                    {
                        using var hCmd = conn.CreateCommand();
                        hCmd.Transaction = tx;
                        hCmd.CommandText = "INSERT INTO UserHistory (UserID, Description, LogTime) VALUES (@uid, @desc, GETDATE())";
                        AddParam(hCmd, "@uid", userId > 0 ? userId : (short)1);
                        AddParam(hCmd, "@desc", $"REMOVED BANK CHEQUE DEPOSIT # {voucherNo}");
                        await hCmd.ExecuteNonQueryAsync();
                    }
                }
                catch { }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        if (await ObjectExistsAsync(context, "BankDeposits", "U"))
        {
            var deposit = await context.BankDeposits.FirstOrDefaultAsync(d => d.Id == (int)voucherNo);
            if (deposit != null)
            {
                var glEntries = await context.GeneralLedger
                    .Where(g => g.SourceType == SourceType.Voucher && g.SourceId == (int)voucherNo)
                    .ToListAsync();
                if (glEntries.Any()) context.GeneralLedger.RemoveRange(glEntries);
                context.BankDeposits.Remove(deposit);
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<ChequeDepositPrintDataDto?> GetChequeDepositPrintDataAsync(long voucherNo, DateTime voucherDate, int branchId, bool includeMultiForDate = false)
    {
        var dto = await LoadChequeDepositByVoucherNoAsync(voucherNo, voucherDate, requireIsCheque: true)
            ?? await LoadChequeDepositByVoucherNoAsync(voucherNo, null, requireIsCheque: true);
        if (dto == null) return null;

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await context.Database.OpenConnectionAsync();

        var reg = await ReadRegistryMapAsync(conn);
        var result = new ChequeDepositPrintDataDto
        {
            Deposit = dto,
            CreatedByUserName = dto.CreatedByUserName,
            ShowPrintDialogVoucher = RegBool(reg, "ShowPrintDialogVoucher", false),
            DirectPrintVoucher = RegBool(reg, "DirectPrintVoucher", false),
            IsMultiMode = includeMultiForDate
        };

        try
        {
            if (await ObjectExistsAsync(context, "Branch", "U"))
            {
                using var bCmd = conn.CreateCommand();
                bCmd.CommandText = @"
                    SELECT TOP 1 CompanyName, ISNULL(CompanyAddress, N'') AS CompanyAddress,
                           ISNULL(CompanyPhone1, N'') AS CompanyPhone1,
                           ISNULL(CompanyPhone2, N'') AS CompanyPhone2,
                           ISNULL(CompanyEmail, N'') AS CompanyEmail,
                           ImageData, ISNULL(FooterInEnglish, N'') AS FooterInEnglish
                    FROM Branch WHERE BranchID = @bid";
                AddParam(bCmd, "@bid", branchId > 0 ? branchId : dto.BranchId);
                using var r = await bCmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    result.CompanyName = r["CompanyName"]?.ToString() ?? string.Empty;
                    result.CompanyAddress = r["CompanyAddress"]?.ToString() ?? string.Empty;
                    var p1 = r["CompanyPhone1"]?.ToString()?.Trim() ?? string.Empty;
                    var p2 = r["CompanyPhone2"]?.ToString()?.Trim() ?? string.Empty;
                    result.CompanyPhone = string.IsNullOrEmpty(p2) ? p1 : $"{p1}  {p2}".Trim();
                    var em = r["CompanyEmail"]?.ToString()?.Trim() ?? string.Empty;
                    result.CompanyEmail = string.IsNullOrEmpty(em) ? string.Empty : $"Email: {em}";
                    if (r["ImageData"] != DBNull.Value) result.LogoBytes = (byte[])r["ImageData"];
                    result.FooterInEnglish = r["FooterInEnglish"]?.ToString() ?? string.Empty;
                }
            }

            if (result.LogoBytes == null && await ObjectExistsAsync(context, "CompanyLogos", "U"))
            {
                using var lCmd = conn.CreateCommand();
                lCmd.CommandText = "SELECT TOP 1 ImageData, ISNULL(FooterInEnglish, '') AS FooterInEnglish FROM CompanyLogos";
                using var r = await lCmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    if (r["ImageData"] != DBNull.Value) result.LogoBytes = (byte[])r["ImageData"];
                    if (string.IsNullOrEmpty(result.FooterInEnglish))
                        result.FooterInEnglish = r["FooterInEnglish"]?.ToString() ?? string.Empty;
                }
            }
        }
        catch { }

        if (includeMultiForDate)
        {
            var dayItems = await SearchChequeDepositsAsync(
                fromDate: dto.VoucherDate.Date,
                toDate: dto.VoucherDate.Date,
                isCheque: true,
                branchId: dto.BranchId);
            result.MultiItems = dayItems.ToList();
        }

        return result;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AGENSOFT EXACT BANK CHEQUES ISSUING PARITY (frmBankCheque & BankCheques)
    // ══════════════════════════════════════════════════════════════════════════

    public async Task<long> GetNextBankChequeVoucherNoAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        long nextNo = 1;

        try
        {
            var maxLegacy = await context.Database.SqlQueryRaw<long?>(@"
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                    SELECT CAST(ISNULL(MAX(VoucherNo), 0) + 1 AS BIGINT) AS Value FROM BankCheques
                ELSE
                    SELECT CAST(1 AS BIGINT) AS Value").FirstOrDefaultAsync();
            if (maxLegacy.HasValue && maxLegacy.Value > 0)
            {
                nextNo = maxLegacy.Value;
            }
        }
        catch { }

        try
        {
            var maxEf = await context.Cheques.MaxAsync(c => (int?)c.Id) ?? 0;
            if (maxEf + 1 > nextNo)
            {
                nextNo = maxEf + 1;
            }
        }
        catch { }

        return nextNo;
    }

    public async Task<IEnumerable<string>> GetAvailableChequeNumbersForBankAsync(string bankAccountNo, long currentVoucherNo = 0)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var result = new List<string>();

        // 1. Try legacy BankChequeBookHistory table first (AgenSoft exact query)
        try
        {
            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"
                IF OBJECT_ID('BankChequeBookHistory', 'U') IS NOT NULL
                BEGIN
                    SELECT ChequeNo 
                    FROM BankChequeBookHistory 
                    WHERE ISNULL(IsCancelled, 0) = 0 
                      AND AccountNo = @acNo
                      AND ChequeNo NOT IN (
                          SELECT ChequeNo FROM BankCheques 
                          WHERE BankAccountNo = @acNo AND VoucherNo <> @curNo
                      )
                    ORDER BY ChequeNo;
                END";

            var p1 = command.CreateParameter();
            p1.ParameterName = "@acNo";
            p1.Value = bankAccountNo;
            command.Parameters.Add(p1);

            var p2 = command.CreateParameter();
            p2.ParameterName = "@curNo";
            p2.Value = currentVoucherNo;
            command.Parameters.Add(p2);

            if (command.Connection?.State != System.Data.ConnectionState.Open)
                await context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var chqNo = reader["ChequeNo"]?.ToString();
                if (!string.IsNullOrWhiteSpace(chqNo))
                    result.Add(chqNo.Trim());
            }
        }
        catch { }

        // 2. Also merge with EF Core available Cheques
        try
        {
            var efCheques = await context.Cheques
                .Include(c => c.ChequeBook)
                    .ThenInclude(cb => cb.BankAccount)
                        .ThenInclude(ba => ba.Account)
                .Where(c => (c.ChequeBook.BankAccount.Account.AccountNo == bankAccountNo || c.ChequeBook.BankAccount.AccountNumber == bankAccountNo)
                            && (c.Status == ChequeStatus.Blank || (currentVoucherNo > 0 && c.VoucherId == (int)currentVoucherNo)))
                .Select(c => c.ChequeNo)
                .ToListAsync();

            foreach (var chq in efCheques)
            {
                if (!string.IsNullOrWhiteSpace(chq) && !result.Contains(chq.Trim()))
                    result.Add(chq.Trim());
            }
        }
        catch { }

        return result.Distinct();
    }

    public async Task<BankChequeDto?> LoadBankChequeByVoucherNoAsync(long voucherNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // 1. Try legacy BankCheques table first
        try
        {
            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                BEGIN
                    SELECT bc.VoucherNo, bc.VoucherDate, bc.ChequeNo, 
                           ISNULL(bc.ChequeDate, bc.VoucherDate) AS ChequeDate, 
                           bc.ReconcileDate, 
                           ISNULL(bc.BankAccountNo, '') AS BankAccountNo, 
                           ISNULL(c1.AccountName, '') AS BankAccountName, 
                           ISNULL(bc.AccountNo, '') AS PartyAccountNo, 
                           ISNULL(c2.AccountName, '') AS PartyAccountName, 
                           bc.Amount, 
                           ISNULL(bc.ReceivedBy, '') AS ReceivedBy, 
                           ISNULL(bc.Narration, '') AS Narration, 
                           ISNULL(bc.NarrationUrdu, N'') AS NarrationUrdu, 
                           ISNULL(bc.WHTax, 0) AS WHTax, 
                           ISNULL(bc.PMFlag, '-') AS PMFlag, 
                           ISNULL(bc.WHTtoBank, 0) AS WHTtoBank, 
                           ISNULL(bc.WHTtoPayee, 1) AS WHTtoPayee, 
                           ISNULL(bc.CropSeason, '') AS CropSeason, 
                           ISNULL(bc.BranchID, 1) AS BranchID
                    FROM BankCheques bc
                    LEFT JOIN ChartOfAccounts c1 ON bc.BankAccountNo = c1.AccountNo
                    LEFT JOIN ChartOfAccounts c2 ON bc.AccountNo = c2.AccountNo
                    WHERE bc.VoucherNo = " + voucherNo + @";
                END";

            if (command.Connection?.State != System.Data.ConnectionState.Open)
                await context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var dto = new BankChequeDto
                {
                    VoucherNo = Convert.ToInt64(reader["VoucherNo"]),
                    VoucherDate = Convert.ToDateTime(reader["VoucherDate"]),
                    ChequeNo = reader["ChequeNo"]?.ToString() ?? string.Empty,
                    ChequeDate = reader["ChequeDate"] != DBNull.Value ? Convert.ToDateTime(reader["ChequeDate"]) : DateTime.Today,
                    IsReconciled = reader["ReconcileDate"] != DBNull.Value,
                    ReconcileDate = reader["ReconcileDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReconcileDate"]) : null,
                    BankAccountNo = reader["BankAccountNo"]?.ToString() ?? string.Empty,
                    BankAccountName = reader["BankAccountName"]?.ToString() ?? string.Empty,
                    PartyAccountNo = reader["PartyAccountNo"]?.ToString() ?? string.Empty,
                    PartyAccountName = reader["PartyAccountName"]?.ToString() ?? string.Empty,
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    ReceivedBy = reader["ReceivedBy"]?.ToString() ?? string.Empty,
                    Narration = reader["Narration"]?.ToString() ?? string.Empty,
                    NarrationUrdu = reader["NarrationUrdu"]?.ToString() ?? string.Empty,
                    WhtTax = Convert.ToDecimal(reader["WHTax"]),
                    PmFlag = reader["PMFlag"]?.ToString() ?? "-",
                    WhtToBank = Convert.ToBoolean(reader["WHTtoBank"]),
                    WhtToPayee = Convert.ToBoolean(reader["WHTtoPayee"]),
                    CropSeason = reader["CropSeason"]?.ToString() ?? string.Empty,
                    BranchId = Convert.ToInt32(reader["BranchID"])
                };

                // Close reader to read split details
                reader.Close();

                // Resolve Bank Account
                var bankAc = await context.BankAccounts.Include(b => b.Account)
                    .FirstOrDefaultAsync(b => b.Account.AccountNo == dto.BankAccountNo || b.AccountNumber == dto.BankAccountNo);
                if (bankAc != null)
                {
                    dto.BankAccountId = bankAc.Id;
                    if (string.IsNullOrEmpty(dto.BankAccountName))
                        dto.BankAccountName = bankAc.BankName;
                }

                // Resolve Party Account
                if (!string.IsNullOrEmpty(dto.PartyAccountNo))
                {
                    var pAc = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == dto.PartyAccountNo);
                    if (pAc != null)
                    {
                        dto.PartyAccountId = pAc.Id;
                        if (string.IsNullOrEmpty(dto.PartyAccountName))
                            dto.PartyAccountName = pAc.Name;
                    }
                }

                // Check for multi-payee split details in BankChequesBody
                using var bodyCmd = context.Database.GetDbConnection().CreateCommand();
                bodyCmd.CommandText = @"
                    IF OBJECT_ID('BankChequesBody', 'U') IS NOT NULL
                    BEGIN
                        SELECT bcb.AccountNo, ISNULL(c.AccountName, '') AS AccountName, bcb.Amount
                        FROM BankChequesBody bcb
                        LEFT JOIN ChartOfAccounts c ON bcb.AccountNo = c.AccountNo
                        WHERE bcb.VoucherNo = " + voucherNo + @";
                    END";

                using var bodyReader = await bodyCmd.ExecuteReaderAsync();
                while (await bodyReader.ReadAsync())
                {
                    dto.SplitDetails.Add(new BankChequeBodyDto
                    {
                        AccountNo = bodyReader["AccountNo"]?.ToString() ?? string.Empty,
                        AccountName = bodyReader["AccountName"]?.ToString() ?? string.Empty,
                        Amount = Convert.ToDecimal(bodyReader["Amount"])
                    });
                }

                return dto;
            }
        }
        catch { }

        // 2. Fallback to EF Core Cheques
        var chq = await context.Cheques
            .Include(c => c.ChequeBook)
                .ThenInclude(cb => cb.BankAccount)
                    .ThenInclude(ba => ba.Account)
            .Include(c => c.IssuedToParty)
                .ThenInclude(p => p.Account)
            .Include(c => c.Voucher)
                .ThenInclude(v => v.CropSeason)
            .FirstOrDefaultAsync(c => c.Id == (int)voucherNo || c.VoucherId == (int)voucherNo);

        if (chq != null)
        {
            return new BankChequeDto
            {
                VoucherNo = chq.VoucherId ?? chq.Id,
                VoucherDate = chq.IssueDate ?? chq.CreatedAt,
                ChequeNo = chq.ChequeNo,
                ChequeDate = chq.IssueDate ?? DateTime.Today,
                IsReconciled = chq.Status == ChequeStatus.Cleared,
                ReconcileDate = chq.ClearanceDate,
                BankAccountId = chq.ChequeBook.BankAccountId,
                BankAccountNo = chq.ChequeBook.BankAccount.Account.AccountNo,
                BankAccountName = chq.ChequeBook.BankAccount.BankName,
                PartyAccountId = chq.IssuedToPartyId,
                PartyAccountNo = chq.IssuedToParty?.Account?.AccountNo ?? string.Empty,
                PartyAccountName = chq.IssuedToParty?.Account?.Name ?? string.Empty,
                Amount = chq.Amount,
                Narration = chq.Voucher?.NarrationEnglish ?? string.Empty,
                NarrationUrdu = chq.Voucher?.NarrationUrdu ?? string.Empty,
                CropSeason = chq.Voucher?.CropSeason?.Name ?? string.Empty
            };
        }

        return null;
    }

    public async Task<IEnumerable<BankChequeDto>> SearchBankChequesAsync(string? query = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var results = new List<BankChequeDto>();

        try
        {
            using var command = context.Database.GetDbConnection().CreateCommand();
            var sql = @"
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                BEGIN
                    SELECT TOP 100 bc.VoucherNo, bc.VoucherDate, bc.ChequeNo, 
                           ISNULL(bc.ChequeDate, bc.VoucherDate) AS ChequeDate, 
                           bc.ReconcileDate, 
                           ISNULL(bc.BankAccountNo, '') AS BankAccountNo, 
                           ISNULL(c1.AccountName, '') AS BankAccountName, 
                           ISNULL(bc.AccountNo, '') AS PartyAccountNo, 
                           ISNULL(c2.AccountName, '') AS PartyAccountName, 
                           bc.Amount, 
                           ISNULL(bc.ReceivedBy, '') AS ReceivedBy, 
                           ISNULL(bc.Narration, '') AS Narration, 
                           ISNULL(bc.NarrationUrdu, N'') AS NarrationUrdu, 
                           ISNULL(bc.WHTax, 0) AS WHTax, 
                           ISNULL(bc.PMFlag, '-') AS PMFlag, 
                           ISNULL(bc.CropSeason, '') AS CropSeason, 
                           ISNULL(bc.BranchID, 1) AS BranchID
                    FROM BankCheques bc
                    LEFT JOIN ChartOfAccounts c1 ON bc.BankAccountNo = c1.AccountNo
                    LEFT JOIN ChartOfAccounts c2 ON bc.AccountNo = c2.AccountNo
                    WHERE 1 = 1 ";

            if (!string.IsNullOrWhiteSpace(query))
            {
                sql += @" AND (
                    CAST(bc.VoucherNo AS VARCHAR(20)) LIKE @q OR
                    bc.ChequeNo LIKE @q OR
                    bc.BankAccountNo LIKE @q OR
                    c1.AccountName LIKE @q OR
                    bc.AccountNo LIKE @q OR
                    c2.AccountName LIKE @q OR
                    bc.ReceivedBy LIKE @q
                )";
                var p = command.CreateParameter();
                p.ParameterName = "@q";
                p.Value = "%" + query.Trim() + "%";
                command.Parameters.Add(p);
            }

            if (fromDate.HasValue)
            {
                sql += " AND bc.VoucherDate >= @fDate";
                var pf = command.CreateParameter();
                pf.ParameterName = "@fDate";
                pf.Value = fromDate.Value.Date;
                command.Parameters.Add(pf);
            }

            if (toDate.HasValue)
            {
                sql += " AND bc.VoucherDate <= @tDate";
                var pt = command.CreateParameter();
                pt.ParameterName = "@tDate";
                pt.Value = toDate.Value.Date;
                command.Parameters.Add(pt);
            }

            sql += " ORDER BY bc.VoucherNo DESC END";
            command.CommandText = sql;

            if (command.Connection?.State != System.Data.ConnectionState.Open)
                await context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new BankChequeDto
                {
                    VoucherNo = Convert.ToInt64(reader["VoucherNo"]),
                    VoucherDate = Convert.ToDateTime(reader["VoucherDate"]),
                    ChequeNo = reader["ChequeNo"]?.ToString() ?? string.Empty,
                    ChequeDate = reader["ChequeDate"] != DBNull.Value ? Convert.ToDateTime(reader["ChequeDate"]) : DateTime.Today,
                    IsReconciled = reader["ReconcileDate"] != DBNull.Value,
                    ReconcileDate = reader["ReconcileDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReconcileDate"]) : null,
                    BankAccountNo = reader["BankAccountNo"]?.ToString() ?? string.Empty,
                    BankAccountName = reader["BankAccountName"]?.ToString() ?? string.Empty,
                    PartyAccountNo = reader["PartyAccountNo"]?.ToString() ?? string.Empty,
                    PartyAccountName = reader["PartyAccountName"]?.ToString() ?? string.Empty,
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    ReceivedBy = reader["ReceivedBy"]?.ToString() ?? string.Empty,
                    Narration = reader["Narration"]?.ToString() ?? string.Empty,
                    NarrationUrdu = reader["NarrationUrdu"]?.ToString() ?? string.Empty,
                    WhtTax = Convert.ToDecimal(reader["WHTax"]),
                    PmFlag = reader["PMFlag"]?.ToString() ?? "-",
                    CropSeason = reader["CropSeason"]?.ToString() ?? string.Empty,
                    BranchId = Convert.ToInt32(reader["BranchID"])
                });
            }
        }
        catch { }

        return results;
    }

    public async Task SaveBankChequeAsync(BankChequeDto dto)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // 1. Resolve Bank Account
        BankAccount? bankAccount = null;
        if (dto.BankAccountId > 0)
        {
            bankAccount = await context.BankAccounts.Include(b => b.Account).FirstOrDefaultAsync(b => b.Id == dto.BankAccountId);
        }
        else if (!string.IsNullOrEmpty(dto.BankAccountNo))
        {
            bankAccount = await context.BankAccounts.Include(b => b.Account)
                .FirstOrDefaultAsync(b => b.Account.AccountNo == dto.BankAccountNo || b.AccountNumber == dto.BankAccountNo);
        }

        if (bankAccount == null)
            throw new InvalidOperationException($"Bank Account '{dto.BankAccountNo}' not found.");

        var bankAcNo = bankAccount.Account?.AccountNo ?? bankAccount.AccountNumber;

        // 2. Resolve Party Account (Primary Pay To)
        Account? partyAccount = null;
        if (dto.PartyAccountId.HasValue && dto.PartyAccountId.Value > 0)
        {
            partyAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.PartyAccountId.Value);
        }
        else if (!string.IsNullOrEmpty(dto.PartyAccountNo))
        {
            partyAccount = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == dto.PartyAccountNo);
        }

        var partyAcNo = partyAccount?.AccountNo ?? dto.PartyAccountNo;

        // 3. Resolve Current User & Branch
        short userNo = 1;
        try
        {
            var uId = await context.Database.SqlQueryRaw<short?>(
                "IF OBJECT_ID('Users', 'U') IS NOT NULL SELECT TOP 1 CAST(UserID AS SMALLINT) AS Value FROM Users ELSE SELECT CAST(1 AS SMALLINT) AS Value"
            ).FirstOrDefaultAsync();
            if (uId.HasValue && uId.Value > 0) userNo = uId.Value;
        }
        catch { userNo = 1; }
        int branchId = dto.BranchId > 0 ? dto.BranchId : 1;
        try
        {
            var bVal = await context.Database.SqlQueryRaw<int?>("SELECT TOP 1 CAST(BranchID AS INT) AS Value FROM Branch").FirstOrDefaultAsync();
            if (bVal.HasValue && bVal.Value > 0) branchId = bVal.Value;
        }
        catch { }

        // 4. Resolve Crop Season
        var cropSeasonStr = dto.CropSeason ?? "";
        if (string.IsNullOrEmpty(cropSeasonStr) && dto.CropSeasonId.HasValue && dto.CropSeasonId.Value > 0)
        {
            try
            {
                var season = await context.CropSeasons.FindAsync(dto.CropSeasonId.Value);
                cropSeasonStr = season?.Name ?? "";
            }
            catch { }
        }
        if (cropSeasonStr.Length > 5) cropSeasonStr = cropSeasonStr.Substring(0, 5);

        // 5. Financial Year
        var currentYear = await _systemService.GetCurrentYearAsync();
        int finYearId = currentYear?.Id ?? 1;

        // ══════ AGENSOFT DATABASE SYNC (BankCheques & BankChequesBody TABLES) ══════
        try
        {
            var reconcileDateVal = dto.IsReconciled 
                ? (dto.ReconcileDate ?? dto.ChequeDate).Date 
                : (DateTime?)null;

            var sqlInsert = @"
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                BEGIN
                    DELETE FROM BankChequesBody WHERE VoucherNo = {0};
                    DELETE FROM BankCheques WHERE VoucherNo = {0};
                    INSERT INTO BankCheques (
                        VoucherNo, VoucherDate, ChequeNo, ChequeDate, ReconcileDate, 
                        BankAccountNo, AccountNo, UserNo, Amount, ReceivedBy, 
                        IsPosted, Narration, IsLost, WHTax, PMFlag, 
                        NarrationUrdu, WHTtoBank, WHTtoPayee, VoucherTime, CropSeason, 
                        BranchID, IDBranch
                    )
                    VALUES (
                        {0}, {1}, {2}, {3}, {4}, 
                        {5}, {6}, {7}, {8}, {9}, 
                        1, {10}, 0, {11}, {12}, 
                        {13}, {14}, {15}, GETDATE(), {16}, 
                        {17}, {17}
                    );
                END";

            await context.Database.ExecuteSqlRawAsync(sqlInsert,
                dto.VoucherNo,
                dto.VoucherDate.Date,
                dto.ChequeNo ?? "",
                dto.ChequeDate.Date,
                (object?)reconcileDateVal ?? DBNull.Value,
                bankAcNo,
                partyAcNo ?? "",
                userNo,
                dto.Amount,
                dto.ReceivedBy ?? "",
                dto.Narration ?? "",
                dto.WhtTax,
                dto.PmFlag ?? "-",
                dto.NarrationUrdu ?? "",
                dto.WhtToBank ? 1 : 0,
                dto.WhtToPayee ? 1 : 0,
                cropSeasonStr,
                branchId);

            // Insert split details if multi-payee
            if (dto.SplitDetails.Any())
            {
                foreach (var detail in dto.SplitDetails)
                {
                    await context.Database.ExecuteSqlRawAsync(@"
                        IF OBJECT_ID('BankChequesBody', 'U') IS NOT NULL
                        INSERT INTO BankChequesBody (VoucherNo, VoucherDate, AccountNo, Amount)
                        VALUES ({0}, {1}, {2}, {3});",
                        dto.VoucherNo,
                        dto.VoucherDate.Date,
                        detail.AccountNo,
                        detail.Amount);
                }
            }
        }
        catch { /* Resilient SQL error handling */ }

        // ══════ EF CORE ENTITY & BALANCED GENERAL LEDGER SYNC ══════
        var cheque = await context.Cheques
            .Include(c => c.ChequeBook)
            .FirstOrDefaultAsync(c => c.ChequeNo == dto.ChequeNo && c.ChequeBook.BankAccountId == bankAccount.Id);

        if (cheque == null)
        {
            var chequeBook = await context.ChequeBooks
                .FirstOrDefaultAsync(cb => cb.BankAccountId == bankAccount.Id && cb.IsActive)
                ?? await context.ChequeBooks.FirstOrDefaultAsync(cb => cb.BankAccountId == bankAccount.Id);

            if (chequeBook == null)
            {
                chequeBook = new ChequeBook
                {
                    BankAccountId = bankAccount.Id,
                    SeriesStart = 1,
                    SeriesEnd = 100,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                context.ChequeBooks.Add(chequeBook);
                await context.SaveChangesAsync();
            }

            cheque = new Cheque
            {
                ChequeBookId = chequeBook.Id,
                ChequeNo = dto.ChequeNo,
                Amount = dto.Amount,
                IssueDate = dto.ChequeDate,
                Status = dto.IsReconciled ? ChequeStatus.Cleared : ChequeStatus.Issued,
                ClearanceDate = dto.IsReconciled ? (dto.ReconcileDate ?? dto.ChequeDate) : null,
                VoucherId = (int)dto.VoucherNo,
                CreatedAt = DateTime.Now
            };
            context.Cheques.Add(cheque);
        }
        else
        {
            cheque.Amount = dto.Amount;
            cheque.IssueDate = dto.ChequeDate;
            cheque.Status = dto.IsReconciled ? ChequeStatus.Cleared : ChequeStatus.Issued;
            cheque.ClearanceDate = dto.IsReconciled ? (dto.ReconcileDate ?? dto.ChequeDate) : null;
            cheque.VoucherId = (int)dto.VoucherNo;
            cheque.UpdatedAt = DateTime.Now;
        }

        // Clean previous GL entries for this cheque issue voucher
        var oldGL = await context.GeneralLedger
            .Where(g => g.SourceType == SourceType.Voucher && g.SourceId == (int)dto.VoucherNo && g.Narration != null && g.Narration.Contains("Bank Cheque Issue"))
            .ToListAsync();
        if (oldGL.Any())
        {
            context.GeneralLedger.RemoveRange(oldGL);
        }

        // Balanced GL Postings for Bank Cheque Issuing:
        // 1. CREDIT Bank Account (Money leaving bank):
        context.GeneralLedger.Add(new GeneralLedger
        {
            TransactionDate = dto.ChequeDate,
            AccountId = bankAccount.AccountId,
            Debit = 0,
            Credit = dto.Amount,
            Narration = $"Bank Cheque Issue #{dto.VoucherNo} (Chq #{dto.ChequeNo} to {dto.PartyAccountName}) - {dto.Narration}",
            SourceType = SourceType.Voucher,
            SourceId = (int)dto.VoucherNo,
            FinancialYearId = finYearId,
            CropSeasonId = dto.CropSeasonId
        });

        // 2. DEBIT Party / Payee Account (Party received money):
        if (dto.SplitDetails.Any())
        {
            foreach (var detail in dto.SplitDetails)
            {
                var dAc = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == detail.AccountNo);
                if (dAc != null)
                {
                    context.GeneralLedger.Add(new GeneralLedger
                    {
                        TransactionDate = dto.ChequeDate,
                        AccountId = dAc.Id,
                        Debit = detail.Amount,
                        Credit = 0,
                        Narration = $"Bank Cheque Issue #{dto.VoucherNo} (Chq #{dto.ChequeNo} from {bankAccount.BankName}) - {detail.Narration}",
                        SourceType = SourceType.Voucher,
                        SourceId = (int)dto.VoucherNo,
                        FinancialYearId = finYearId,
                        CropSeasonId = dto.CropSeasonId
                    });
                }
            }
        }
        else if (partyAccount != null)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = dto.ChequeDate,
                AccountId = partyAccount.Id,
                Debit = dto.Amount,
                Credit = 0,
                Narration = $"Bank Cheque Issue #{dto.VoucherNo} (Chq #{dto.ChequeNo} from {bankAccount.BankName}) - {dto.Narration}",
                SourceType = SourceType.Voucher,
                SourceId = (int)dto.VoucherNo,
                FinancialYearId = finYearId,
                CropSeasonId = dto.CropSeasonId
            });
        }

        // Update Bank Balance
        bankAccount.CurrentBalance -= dto.Amount;

        await context.SaveChangesAsync();
    }

    public async Task DeleteBankChequeAsync(long voucherNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // 1. Delete from legacy tables
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('BankChequesBody', 'U') IS NOT NULL
                    DELETE FROM BankChequesBody WHERE VoucherNo = {0};
                IF OBJECT_ID('BankCheques', 'U') IS NOT NULL
                    DELETE FROM BankCheques WHERE VoucherNo = {0};", voucherNo);
        }
        catch { }

        // 2. Delete from EF Core and revert GL
        var chq = await context.Cheques
            .Include(c => c.ChequeBook)
                .ThenInclude(cb => cb.BankAccount)
            .FirstOrDefaultAsync(c => c.VoucherId == (int)voucherNo || c.Id == (int)voucherNo);

        if (chq != null)
        {
            chq.ChequeBook.BankAccount.CurrentBalance += chq.Amount;
            chq.Status = ChequeStatus.Blank;
            chq.Amount = 0;
            chq.IssuedToPartyId = null;
            chq.IssueDate = null;
            chq.ClearanceDate = null;
            chq.VoucherId = null;
        }

        var glEntries = await context.GeneralLedger
            .Where(g => g.SourceType == SourceType.Voucher && g.SourceId == (int)voucherNo)
            .ToListAsync();
        if (glEntries.Any())
        {
            context.GeneralLedger.RemoveRange(glEntries);
        }

        await context.SaveChangesAsync();
    }
}
