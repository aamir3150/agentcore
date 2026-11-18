using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AccountService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Account?> GetAccountByNoAsync(string accountNo)
    {
        if (string.IsNullOrWhiteSpace(accountNo)) return null;
        using var context = await _contextFactory.CreateDbContextAsync();
        var trimmed = accountNo.Trim();
        try
        {
            var acc = await context.Accounts.FirstOrDefaultAsync(a => !a.IsDeleted && a.AccountNo == trimmed);
            if (acc != null) return acc;
        }
        catch
        {
        }

        // Native ChartOfAccounts fallback
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AccountNo, AccountName, ISNULL(AccountNameUrdu, ''), ISNULL(OpeningDebit,0)-ISNULL(OpeningCredit,0) FROM ChartOfAccounts WHERE AccountNo = @acc";
            var p = cmd.CreateParameter();
            p.ParameterName = "@acc";
            p.Value = trimmed;
            cmd.Parameters.Add(p);
            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                string rAccNo = r[0]?.ToString()?.Trim() ?? trimmed;
                int pId = int.TryParse(rAccNo, out int num) && num > 0 ? num : 1;
                return new Account
                {
                    Id = pId,
                    AccountNo = rAccNo,
                    Name = r[1]?.ToString()?.Trim() ?? string.Empty,
                    UrduName = r[2]?.ToString()?.Trim(),
                    OpeningBalance = r[3] != DBNull.Value ? Convert.ToDecimal(r[3]) : 0m,
                    CurrentBalance = r[3] != DBNull.Value ? Convert.ToDecimal(r[3]) : 0m,
                    IsActive = true
                };
            }
        }
        catch
        {
        }
        return null;
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            return await context.Accounts
                .Include(a => a.AccountGroup)
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.AccountNo)
                .ToListAsync();
        }
        catch
        {
            // Native ChartOfAccounts fallback
            var list = new List<Account>();
            try
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT AccountNo, AccountName, ISNULL(OpeningDebit,0)-ISNULL(OpeningCredit,0), ISNULL(AccountNameUrdu, ''), ISNULL(AccountType, 'Asset') FROM ChartOfAccounts WHERE IsDetailed=1 OR IsDetailed IS NULL ORDER BY AccountNo";
                using var r = await cmd.ExecuteReaderAsync();
                int synthId = 1;
                while (await r.ReadAsync())
                {
                    string accNo = r[0]?.ToString()?.Trim() ?? string.Empty;
                    string name = r[1]?.ToString()?.Trim() ?? string.Empty;
                    decimal bal = r[2] != DBNull.Value ? Convert.ToDecimal(r[2]) : 0m;
                    string urdu = r[3]?.ToString()?.Trim() ?? string.Empty;
                    string aType = r[4]?.ToString()?.Trim() ?? "Asset";

                    AccountType typeEnum = AccountType.Asset;
                    if (aType == "L" || aType.StartsWith("Liab", StringComparison.OrdinalIgnoreCase)) typeEnum = AccountType.Liability;
                    else if (aType == "Q" || aType.StartsWith("Equit", StringComparison.OrdinalIgnoreCase)) typeEnum = AccountType.Equity;
                    else if (aType == "R" || aType.StartsWith("Rev", StringComparison.OrdinalIgnoreCase)) typeEnum = AccountType.Revenue;
                    else if (aType == "E" || aType.StartsWith("Exp", StringComparison.OrdinalIgnoreCase)) typeEnum = AccountType.Expense;

                    int parsedId = int.TryParse(accNo, out int num) && num > 0 ? num : synthId++;

                    list.Add(new Account
                    {
                        Id = parsedId,
                        AccountNo = accNo,
                        Name = name,
                        UrduName = urdu,
                        AccountType = typeEnum,
                        AccountGroupId = 1,
                        OpeningBalance = bal,
                        CurrentBalance = bal,
                        IsActive = true
                    });
                }
            }
            catch
            {
            }
            return list;
        }
    }

    public async Task<IEnumerable<AccountGroup>> GetAccountGroupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var groups = await context.AccountGroups.ToListAsync();
            if (groups.Count > 0) return groups;
        }
        catch
        {
        }

        // Native AgenSoft fallback: Get Groups from ChartOfAccounts
        var list = new List<AccountGroup>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
                BEGIN
                    SELECT AccountNo, AccountName, ISNULL(AccountType, 'Asset') 
                    FROM ChartOfAccounts 
                    WHERE IsDetailed = 0 OR AccountDepth = 0
                    ORDER BY AccountNo;
                END";

            int syntheticId = 1;
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                string? typeStr = r[2]?.ToString()?.Trim();
                AccountType gType = AccountType.Asset;
                if (typeStr == "2" || string.Equals(typeStr, "Liability", StringComparison.OrdinalIgnoreCase) || typeStr == "L") gType = AccountType.Liability;
                else if (typeStr == "3" || string.Equals(typeStr, "Equity", StringComparison.OrdinalIgnoreCase) || typeStr == "Q") gType = AccountType.Equity;
                else if (typeStr == "4" || string.Equals(typeStr, "Revenue", StringComparison.OrdinalIgnoreCase) || typeStr == "R") gType = AccountType.Revenue;
                else if (typeStr == "5" || string.Equals(typeStr, "Expense", StringComparison.OrdinalIgnoreCase) || typeStr == "E") gType = AccountType.Expense;

                list.Add(new AccountGroup
                {
                    Id = syntheticId++,
                    Name = r[1]?.ToString()?.Trim() ?? string.Empty,
                    GroupType = gType
                });
            }
        }
        catch
        {
        }

        // If no parent groups exist, provide standard AgenSoft base groups
        if (list.Count == 0)
        {
            list.Add(new AccountGroup { Id = 1, Name = "General Accounts", GroupType = AccountType.Asset });
            list.Add(new AccountGroup { Id = 2, Name = "Customers / Parties", GroupType = AccountType.Asset });
            list.Add(new AccountGroup { Id = 3, Name = "Vendors / Suppliers", GroupType = AccountType.Liability });
            list.Add(new AccountGroup { Id = 4, Name = "Bank & Cash Accounts", GroupType = AccountType.Asset });
            list.Add(new AccountGroup { Id = 5, Name = "Expense Accounts", GroupType = AccountType.Expense });
        }

        return list;
    }

    public async Task AddAccountAsync(Account account)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        account.CurrentBalance = account.OpeningBalance;

        // Always save to native AgenSoft ChartOfAccounts
        await SyncToChartOfAccountsAsync(context, account);

        // Try modern Accounts table if it exists (hybrid fallback)
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT OBJECT_ID('Accounts', 'U')";
            var objId = await cmd.ExecuteScalarAsync();
            if (objId != null && objId != DBNull.Value)
            {
                context.Accounts.Add(account);
                await context.SaveChangesAsync();
            }
        }
        catch
        {
        }
    }

    public async Task UpdateAccountAsync(Account account)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await SyncToChartOfAccountsAsync(context, account);

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT OBJECT_ID('Accounts', 'U')";
            var objId = await cmd.ExecuteScalarAsync();
            if (objId != null && objId != DBNull.Value)
            {
                var ledgerSum = await context.GeneralLedger
                    .Where(gl => gl.AccountId == account.Id)
                    .SumAsync(gl => (decimal?)(gl.Debit - gl.Credit)) ?? 0m;
                account.CurrentBalance = account.OpeningBalance + ledgerSum;
                context.Accounts.Update(account);
                await context.SaveChangesAsync();
            }
        }
        catch
        {
        }
    }

    private static async Task SyncToChartOfAccountsAsync(AppDbContext context, Account account)
    {
        if (string.IsNullOrWhiteSpace(account.AccountNo)) return;

        decimal opDebit = account.OpeningBalance > 0 ? account.OpeningBalance : 0m;
        decimal opCredit = account.OpeningBalance < 0 ? Math.Abs(account.OpeningBalance) : 0m;

        var sql = @"
            IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM ChartOfAccounts WHERE AccountNo = {0})
                BEGIN
                    INSERT INTO ChartOfAccounts (
                        AccountNo, UserID, AccountName, AccountType, AccountDepth, 
                        Narration, ParentAccountNo, OpeningDebit, OpeningCredit, 
                        AdjustedDebit, AdjustedCredit, IsDetailed, IsLocked, IsPosted, 
                        IsEditable, BalFlag, PLFlag, ExpFlag, ExpenseLimit, AccountNameUrdu
                    )
                    VALUES (
                        {0}, 1, {3}, {5}, 1, 
                        '', NULL, {1}, {2}, 
                        {1}, {2}, 1, 0, 1, 
                        1, 1, 'B', 0, NULL, {4}
                    );
                END
                ELSE
                BEGIN
                    UPDATE ChartOfAccounts 
                    SET AccountName = {3}, 
                        AccountNameUrdu = {4},
                        OpeningDebit = {1},
                        OpeningCredit = {2},
                        AdjustedDebit = {1},
                        AdjustedCredit = {2}
                    WHERE AccountNo = {0};
                END
            END";

        string accType = account.AccountType switch
        {
            AccountType.Asset => "A",
            AccountType.Liability => "L",
            AccountType.Equity => "Q",
            AccountType.Revenue => "R",
            AccountType.Expense => "E",
            _ => "D"
        };
        await context.Database.ExecuteSqlRawAsync(sql, 
            account.AccountNo.Trim(), 
            opDebit, 
            opCredit, 
            account.Name.Trim(), 
            account.UrduName ?? account.Name,
            accType);
    }

    public async Task<decimal> GetAccountBalanceAsync(int accountId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        Account? account = null;
        try
        {
            account = await context.Accounts.FindAsync(accountId);
        }
        catch
        {
        }

        string accNo = account?.AccountNo ?? string.Empty;
        decimal opBalance = account?.OpeningBalance ?? 0m;
        string accName = account?.Name ?? string.Empty;

        if (string.IsNullOrWhiteSpace(accNo))
        {
            try
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 1 AccountNo, AccountName, ISNULL(OpeningDebit,0)-ISNULL(OpeningCredit,0) FROM ChartOfAccounts WHERE AccountNo = @acc";
                var p = cmd.CreateParameter();
                p.ParameterName = "@acc";
                p.Value = accountId.ToString();
                cmd.Parameters.Add(p);
                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    accNo = r[0]?.ToString()?.Trim() ?? string.Empty;
                    accName = r[1]?.ToString()?.Trim() ?? string.Empty;
                    opBalance = r[2] != DBNull.Value ? Convert.ToDecimal(r[2]) : 0m;
                }
            }
            catch
            {
            }
        }

        if (string.IsNullOrWhiteSpace(accNo)) return opBalance;

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            if (accNo == "1001" || accName.Contains("Cash in Hand", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = @"
                    SELECT ISNULL(SUM(Debit), 0) - ISNULL(SUM(Credit), 0)
                    FROM (
                        SELECT Credit AS Debit, 0 AS Credit FROM CreditVouchersBody
                        UNION ALL
                        SELECT 0 AS Debit, Debit AS Credit FROM DebitVouchersBody
                        UNION ALL
                        SELECT Debit, Credit FROM JournalVouchersBody WHERE AccountNo = '1001'
                        UNION ALL
                        SELECT Debit, Credit FROM VuAccVouchersOthers WHERE AccountNo = '1001'
                    ) T";
            }
            else
            {
                cmd.CommandText = @"
                    SELECT ISNULL(SUM(Debit), 0) - ISNULL(SUM(Credit), 0)
                    FROM (
                        SELECT 0 AS Debit, Credit FROM CreditVouchersBody WHERE AccountNo = @acc
                        UNION ALL
                        SELECT Debit, 0 AS Credit FROM DebitVouchersBody WHERE AccountNo = @acc
                        UNION ALL
                        SELECT Debit, Credit FROM JournalVouchersBody WHERE AccountNo = @acc
                        UNION ALL
                        SELECT 0 AS Debit, Amount AS Credit FROM BankCheques WHERE AccountNo = @acc
                        UNION ALL
                        SELECT Amount AS Debit, 0 AS Credit FROM BankDeposit WHERE AccountNo = @acc
                        UNION ALL
                        SELECT Debit, Credit FROM VuAccVouchersOthers WHERE AccountNo = @acc
                    ) T";
                var p = cmd.CreateParameter();
                p.ParameterName = "@acc";
                p.Value = accNo;
                cmd.Parameters.Add(p);
            }

            var scalar = await cmd.ExecuteScalarAsync();
            decimal txSum = scalar != null && scalar != DBNull.Value ? Convert.ToDecimal(scalar) : 0m;
            return opBalance + txSum;
        }
        catch
        {
            return opBalance;
        }
    }

    public async Task DeleteAccountAsync(int accountId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var account = await context.Accounts.FindAsync(accountId);
            if (account != null && !account.IsSystemAccount)
            {
                account.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }
        catch
        {
        }
    }

    public async Task AddAccountGroupAsync(AccountGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            context.AccountGroups.Add(group);
            await context.SaveChangesAsync();
        }
        catch
        {
        }
    }

    public async Task UpdateAccountGroupAsync(AccountGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            context.AccountGroups.Update(group);
            await context.SaveChangesAsync();
        }
        catch
        {
        }
    }

    public async Task DeleteAccountGroupAsync(int groupId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var group = await context.AccountGroups.FindAsync(groupId);
            if (group != null)
            {
                group.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }
        catch
        {
        }
    }

    public async Task SetOpeningBalanceAsync(int accountId, decimal amount)
    {
        decimal debit = amount >= 0 ? amount : 0;
        decimal credit = amount < 0 ? Math.Abs(amount) : 0;
        await SetOpeningBalanceAsync(accountId, debit, credit);
    }

    public async Task SetOpeningBalanceAsync(int accountId, decimal debit, decimal credit)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var account = await context.Accounts.FindAsync(accountId);
        if (account != null)
        {
            account.OpeningBalance = debit - credit;
            var ledgerSum = await context.GeneralLedger
                .Where(gl => gl.AccountId == accountId)
                .SumAsync(gl => (decimal?)(gl.Debit - gl.Credit)) ?? 0m;
            account.CurrentBalance = account.OpeningBalance + ledgerSum;
            await context.SaveChangesAsync();

            try
            {
                var sql = @"
                    IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
                    BEGIN
                        UPDATE ChartOfAccounts 
                        SET OpeningDebit = {0}, OpeningCredit = {1}, AdjustedDebit = {0}, AdjustedCredit = {1}
                        WHERE AccountNo = {2};
                    END";
                await context.Database.ExecuteSqlRawAsync(sql, debit, credit, account.AccountNo);
            }
            catch
            {
                // Resilient failover
            }
        }
    }

    public async Task RecalculateAllAccountBalancesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var accounts = await context.Accounts.ToListAsync();
        var ledgerSums = await context.GeneralLedger
            .GroupBy(gl => gl.AccountId)
            .Select(g => new { AccountId = g.Key, Net = g.Sum(x => x.Debit - x.Credit) })
            .ToDictionaryAsync(x => x.AccountId, x => x.Net);

        foreach (var acc in accounts)
        {
            decimal net = ledgerSums.TryGetValue(acc.Id, out var sum) ? sum : 0m;
            acc.CurrentBalance = acc.OpeningBalance + net;
        }

        await context.SaveChangesAsync();
    }

    // ═══ AGENSOFT EXACT REPORT SETTINGS PARITY (P&L, Balance Sheet, Expense Report) ═══

    public async Task<IEnumerable<AccountReportSettingDto>> GetAccountReportSettingsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var accounts = await context.Accounts
            .Include(a => a.AccountGroup)
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.AccountNo)
            .ToListAsync();

        var list = new List<AccountReportSettingDto>();
        var legacyFlags = new Dictionary<string, (string? pl, bool? bal, bool? exp, decimal? limit)>();

        try
        {
            var rawList = await context.Database.SqlQueryRaw<LegacyAccountRow>(
                @"SELECT AccountNo, ISNULL(PLFlag, '') AS PLFlag, ISNULL(BalFlag, 1) AS BalFlag, ISNULL(ExpFlag, 0) AS ExpFlag, ExpenseLimit 
                  FROM ChartOfAccounts WHERE IsDetailed = 1").ToListAsync();
            foreach (var r in rawList)
            {
                legacyFlags[r.AccountNo] = (r.PLFlag, r.BalFlag, r.ExpFlag, r.ExpenseLimit);
            }
        }
        catch { /* Fallback to modern entity properties */ }

        foreach (var a in accounts)
        {
            var pl = a.PLFlag;
            var bal = a.BalFlag;
            var exp = a.ExpFlag;
            var limit = a.ExpenseLimit;

            if (legacyFlags.TryGetValue(a.AccountNo, out var leg))
            {
                if (!string.IsNullOrEmpty(leg.pl)) pl = leg.pl;
                if (leg.bal.HasValue) bal = leg.bal.Value;
                if (leg.exp.HasValue) exp = leg.exp.Value;
                if (leg.limit.HasValue) limit = leg.limit.Value;
            }

            list.Add(new AccountReportSettingDto
            {
                Id = a.Id,
                AccountNo = a.AccountNo,
                AccountName = a.Name,
                AccountNameUrdu = a.UrduName,
                AccountType = a.AccountType.ToString(),
                PLFlag = pl,
                BalFlag = bal,
                ExpFlag = exp,
                ExpenseLimit = limit
            });
        }

        return list;
    }

    public async Task UpdateProfitLossSettingsAsync(IEnumerable<AccountReportSettingDto> settings)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        foreach (var s in settings)
        {
            var ac = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == s.AccountNo);
            if (ac != null)
            {
                ac.PLFlag = s.PLFlag;
            }

            try
            {
                await context.Database.ExecuteSqlRawAsync(
                    "IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL UPDATE ChartOfAccounts SET PLFlag = {0} WHERE AccountNo = {1}",
                    s.PLFlag ?? "", s.AccountNo);
            }
            catch { }
        }
        await context.SaveChangesAsync();
    }

    public async Task UpdateBalanceSheetSettingsAsync(IEnumerable<AccountReportSettingDto> settings)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        foreach (var s in settings)
        {
            var ac = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == s.AccountNo);
            if (ac != null)
            {
                ac.BalFlag = s.BalFlag;
            }

            try
            {
                await context.Database.ExecuteSqlRawAsync(
                    "IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL UPDATE ChartOfAccounts SET BalFlag = {0} WHERE AccountNo = {1}",
                    s.BalFlag ? 1 : 0, s.AccountNo);
            }
            catch { }
        }
        await context.SaveChangesAsync();
    }

    public async Task UpdateExpenseReportSettingsAsync(IEnumerable<AccountReportSettingDto> settings)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        foreach (var s in settings)
        {
            var ac = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == s.AccountNo);
            if (ac != null)
            {
                ac.ExpFlag = s.ExpFlag;
                ac.ExpenseLimit = s.ExpenseLimit;
            }

            try
            {
                await context.Database.ExecuteSqlRawAsync(
                    "IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL UPDATE ChartOfAccounts SET ExpFlag = {0}, ExpenseLimit = {1} WHERE AccountNo = {2}",
                    s.ExpFlag ? 1 : 0, (object?)s.ExpenseLimit ?? DBNull.Value, s.AccountNo);
            }
            catch { }
        }
        await context.SaveChangesAsync();
    }

    public async Task<List<string>> GetOpeningBalanceAccountTypesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<string> { "All" };
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
                BEGIN
                    SELECT DISTINCT AccountType 
                    FROM ChartOfAccounts 
                    WHERE AccountType IS NOT NULL AND LTRIM(RTRIM(AccountType)) <> ''
                    ORDER BY AccountType;
                END";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var type = reader[0]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(type) && !list.Contains(type))
                {
                    list.Add(type);
                }
            }
        }
        catch
        {
        }
        return list;
    }

    public async Task<List<CropSeasonItemDto>> GetOpeningCropSeasonsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<CropSeasonItemDto>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF OBJECT_ID('Products', 'U') IS NOT NULL
                BEGIN
                    SELECT ProductID, ProductName 
                    FROM Products 
                    WHERE (IsCrop = 1 OR IsBroker = 1)
                    ORDER BY ProductName;
                END";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CropSeasonItemDto
                {
                    ProductId = reader[0]?.ToString()?.Trim() ?? string.Empty,
                    ProductName = reader[1]?.ToString()?.Trim() ?? string.Empty
                });
            }
        }
        catch
        {
        }
        return list;
    }

    public async Task<List<AccountOpeningBalanceDto>> GetAccountsOpeningBalancesAsync(string accountType)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<AccountOpeningBalanceDto>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            bool filterByType = !string.IsNullOrWhiteSpace(accountType) && !string.Equals(accountType, "All", StringComparison.OrdinalIgnoreCase);

            cmd.CommandText = @"
                IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
                BEGIN
                    SELECT ChartOfAccounts.AccountNo, 
                           ChartOfAccounts.AccountName, 
                           ISNULL(ChartOfAccounts.AccountType, '') AS AccountType,
                           ISNULL(ChartOfAccounts.OpeningDebit, 0) AS OpeningDebit, 
                           ISNULL(ChartOfAccounts.OpeningCredit, 0) AS OpeningCredit, 
                           ISNULL(ChartOfAccounts.CropSeason, '') AS CropSeason, 
                           ISNULL(CROPS.ProductName, '') AS CropName
                    FROM ChartOfAccounts 
                    LEFT OUTER JOIN (
                        SELECT ProductID, ProductName 
                        FROM Products 
                        WHERE (IsCrop = 1 OR IsBroker = 1)
                    ) CROPS ON CROPS.ProductID = ChartOfAccounts.CropSeason 
                    WHERE (ChartOfAccounts.IsDetailed = 1 OR ChartOfAccounts.IsDetailed IS NULL)
                      AND (@FilterByType = 0 OR ChartOfAccounts.AccountType = @AccountType)
                    ORDER BY ChartOfAccounts.AccountNo;
                END";

            var pFilter = cmd.CreateParameter();
            pFilter.ParameterName = "@FilterByType";
            pFilter.Value = filterByType ? 1 : 0;
            cmd.Parameters.Add(pFilter);

            var pType = cmd.CreateParameter();
            pType.ParameterName = "@AccountType";
            pType.Value = (object?)accountType ?? DBNull.Value;
            cmd.Parameters.Add(pType);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new AccountOpeningBalanceDto
                {
                    AccountNo = reader[0]?.ToString()?.Trim() ?? string.Empty,
                    AccountName = reader[1]?.ToString()?.Trim() ?? string.Empty,
                    AccountType = reader[2]?.ToString()?.Trim() ?? string.Empty,
                    OpeningDebit = reader[3] != DBNull.Value ? Convert.ToDecimal(reader[3]) : 0m,
                    OpeningCredit = reader[4] != DBNull.Value ? Convert.ToDecimal(reader[4]) : 0m,
                    CropSeason = reader[5]?.ToString()?.Trim(),
                    CropName = reader[6]?.ToString()?.Trim()
                });
            }
        }
        catch
        {
        }

        return list;
    }

    public async Task SaveAccountsOpeningBalancesAsync(IEnumerable<AccountOpeningBalanceDto> items)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync();

        using var tx = await conn.BeginTransactionAsync();
        try
        {
            foreach (var item in items)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
                    BEGIN
                        UPDATE ChartOfAccounts 
                        SET OpeningDebit = @OpeningDebit, 
                            OpeningCredit = @OpeningCredit, 
                            AdjustedDebit = @OpeningDebit,
                            AdjustedCredit = @OpeningCredit,
                            CropSeason = @CropSeason 
                        WHERE AccountNo = @AccountNo;
                    END";

                var pDebit = cmd.CreateParameter();
                pDebit.ParameterName = "@OpeningDebit";
                pDebit.Value = item.OpeningDebit;
                cmd.Parameters.Add(pDebit);

                var pCredit = cmd.CreateParameter();
                pCredit.ParameterName = "@OpeningCredit";
                pCredit.Value = item.OpeningCredit;
                cmd.Parameters.Add(pCredit);

                var pCrop = cmd.CreateParameter();
                pCrop.ParameterName = "@CropSeason";
                pCrop.Value = string.IsNullOrWhiteSpace(item.CropSeason) ? (object)DBNull.Value : item.CropSeason.Trim();
                cmd.Parameters.Add(pCrop);

                var pAcc = cmd.CreateParameter();
                pAcc.ParameterName = "@AccountNo";
                pAcc.Value = item.AccountNo.Trim();
                cmd.Parameters.Add(pAcc);

                await cmd.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}

public class LegacyAccountRow
{
    public string AccountNo { get; set; } = string.Empty;
    public string PLFlag { get; set; } = string.Empty;
    public bool BalFlag { get; set; }
    public bool ExpFlag { get; set; }
    public decimal? ExpenseLimit { get; set; }
}
