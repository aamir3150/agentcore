using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Enums;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class VoucherService : IVoucherService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    public VoucherService(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
    }

    public async Task<Voucher?> GetVoucherByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Check CashPayment
        var v = await LoadLegacyVoucherByNoAsync(VoucherType.CashPayment, id.ToString(), context);
        if (v != null) return v;
        // Check CashReceiving
        v = await LoadLegacyVoucherByNoAsync(VoucherType.CashReceiving, id.ToString(), context);
        if (v != null) return v;
        // Check Journal
        v = await LoadLegacyVoucherByNoAsync(VoucherType.JournalVoucher, id.ToString(), context);
        if (v != null) return v;

        try
        {
            return await context.Vouchers
                .Include(v => v.Details).ThenInclude(d => d.Account)
                .Include(v => v.CropSeason)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
        catch { return null; }
    }

    public async Task<Voucher?> GetVoucherByNoAsync(VoucherType voucherType, string voucherNo, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var cleanNo = voucherNo.Trim();
        var v = await LoadLegacyVoucherByNoAsync(voucherType, cleanNo, context);
        if (v != null) return v;

        try
        {
            return await context.Vouchers
                .Include(v => v.Details).ThenInclude(d => d.Account)
                .Include(v => v.CropSeason)
                .FirstOrDefaultAsync(v => v.VoucherType == voucherType 
                                       && (v.VoucherNo == cleanNo || v.VoucherNo.EndsWith(cleanNo)));
        }
        catch { return null; }
    }

    private static async Task<long> ExecuteScalarLongAsync(AppDbContext context, string sql, params (string Name, object Value)[] parameters)
    {
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            foreach (var (name, paramVal) in parameters)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = name;
                p.Value = paramVal;
                cmd.Parameters.Add(p);
            }
            var res = await cmd.ExecuteScalarAsync();
            if (res != null && res != DBNull.Value && long.TryParse(res.ToString(), out var parsedLong))
            {
                return parsedLong;
            }
        }
        catch
        {
        }
        return 0;
    }

    public async Task<Voucher?> GetPreviousVoucherAsync(VoucherType voucherType, int currentVoucherId, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        string tbl = voucherType switch
        {
            VoucherType.CashReceiving => "CreditVouchers",
            VoucherType.CashPayment => "DebitVouchers",
            _ => "JournalVouchers"
        };

        var sql = currentVoucherId > 0
            ? $@"IF OBJECT_ID('{tbl}', 'U') IS NOT NULL
                    SELECT TOP 1 CAST(VoucherNo AS BIGINT) FROM {tbl} WHERE VoucherNo < @curr ORDER BY VoucherNo DESC;
                 ELSE
                    SELECT CAST(0 AS BIGINT);"
            : $@"IF OBJECT_ID('{tbl}', 'U') IS NOT NULL
                    SELECT TOP 1 CAST(VoucherNo AS BIGINT) FROM {tbl} ORDER BY VoucherNo DESC;
                 ELSE
                    SELECT CAST(0 AS BIGINT);";

        var prevNo = await ExecuteScalarLongAsync(context, sql, ("@curr", currentVoucherId));
        if (prevNo > 0)
        {
            return await LoadLegacyVoucherByNoAsync(voucherType, prevNo.ToString(), context);
        }
        return null;
    }

    public async Task<Voucher?> GetNextVoucherAsync(VoucherType voucherType, int currentVoucherId, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        string tbl = voucherType switch
        {
            VoucherType.CashReceiving => "CreditVouchers",
            VoucherType.CashPayment => "DebitVouchers",
            _ => "JournalVouchers"
        };

        var sql = $@"IF OBJECT_ID('{tbl}', 'U') IS NOT NULL
                        SELECT TOP 1 CAST(VoucherNo AS BIGINT) FROM {tbl} WHERE VoucherNo > @curr ORDER BY VoucherNo ASC;
                     ELSE
                        SELECT CAST(0 AS BIGINT);";

        var nextNo = await ExecuteScalarLongAsync(context, sql, ("@curr", currentVoucherId));
        if (nextNo > 0)
        {
            return await LoadLegacyVoucherByNoAsync(voucherType, nextNo.ToString(), context);
        }
        return null;
    }

    public async Task<string> GetNextVoucherNoAsync(VoucherType voucherType, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        string tbl = voucherType switch
        {
            VoucherType.CashReceiving => "CreditVouchers",
            VoucherType.CashPayment => "DebitVouchers",
            _ => "JournalVouchers"
        };

        // Exact AgenSoft parity: Select isnull(max(voucherno),0)+1 from DebitVouchers / CreditVouchers / JournalVouchers
        var sql = $@"IF OBJECT_ID('{tbl}', 'U') IS NOT NULL
                        SELECT CAST(ISNULL(MAX(VoucherNo), 0) + 1 AS BIGINT) FROM {tbl};
                     ELSE
                        SELECT CAST(1 AS BIGINT);";

        var nextNo = await ExecuteScalarLongAsync(context, sql);
        return nextNo > 0 ? nextNo.ToString() : "1";
    }

    public async Task SaveVoucherAsync(Voucher voucher)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // Ensure FinancialYearId is valid
        if (voucher.FinancialYearId <= 0)
        {
            voucher.FinancialYearId = 1;
        }

        // Validate Cash Vouchers do not contain internal cash accounts
        if (voucher.VoucherType == VoucherType.CashReceiving || voucher.VoucherType == VoucherType.CashPayment)
        {
            var isCashAccount = false;
            foreach (var d in voucher.Details)
            {
                string ac = !string.IsNullOrWhiteSpace(d.AccountNo) ? d.AccountNo.Trim() : d.AccountId.ToString().Trim();
                if (ac == "1001" || ac.StartsWith("1001"))
                {
                    isCashAccount = true;
                    break;
                }
            }

            if (isCashAccount)
            {
                throw new InvalidOperationException("نقد کھاتہ (Cash in Hand) واؤچر کی تفصیلات (Details) میں شامل نہیں کیا جا سکتا۔");
            }
        }

        string tbl = voucher.VoucherType switch
        {
            VoucherType.CashReceiving => "CreditVouchers",
            VoucherType.CashPayment => "DebitVouchers",
            _ => "JournalVouchers"
        };

        long vNo = ParseVoucherNumber(voucher.VoucherNo, voucher.Id);
        bool isNew = voucher.Id <= 0;

        // If this is a new voucher, check whether vNo already exists or is invalid
        if (isNew)
        {
            bool exists = false;
            if (vNo > 0)
            {
                var checkSql = $@"IF OBJECT_ID('{tbl}', 'U') IS NOT NULL
                                    SELECT COUNT(1) FROM {tbl} WHERE VoucherNo = @vNo;
                                  ELSE
                                    SELECT 0;";
                var count = await ExecuteScalarLongAsync(context, checkSql, ("@vNo", vNo));
                exists = (count > 0);
            }

            if (vNo <= 0 || exists)
            {
                string nextNoStr = await GetNextVoucherNoAsync(voucher.VoucherType, voucher.FinancialYearId);
                vNo = ParseVoucherNumber(nextNoStr, 1);
            }
        }
        else if (vNo <= 0)
        {
            string nextNoStr = await GetNextVoucherNoAsync(voucher.VoucherType, voucher.FinancialYearId);
            vNo = ParseVoucherNumber(nextNoStr, 1);
        }

        voucher.Id = (int)vNo;
        voucher.VoucherNo = vNo.ToString();

        // Save directly to native AgenSoft tables (DebitVouchers / CreditVouchers / JournalVouchers / CashNoteDetail)
        await SyncLegacyVoucherAsync(voucher, context);
    }

    public async Task PostVoucherAsync(int voucherId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                UPDATE CreditVouchers SET IsPosted = 1 WHERE VoucherNo = {0};
            IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL
                UPDATE DebitVouchers SET IsPosted = 1 WHERE VoucherNo = {0};
            IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL
                UPDATE JournalVouchers SET IsPosted = 1 WHERE VoucherNo = {0};", voucherId);
    }

    public async Task ReverseVoucherAsync(int voucherId, string reason)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                UPDATE CreditVouchers SET IsPosted = 0 WHERE VoucherNo = {0};
            IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL
                UPDATE DebitVouchers SET IsPosted = 0 WHERE VoucherNo = {0};
            IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL
                UPDATE JournalVouchers SET IsPosted = 0 WHERE VoucherNo = {0};", voucherId);
    }

    public async Task DeleteVoucherAsync(int voucherId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID('CreditVouchersBody', 'U') IS NOT NULL DELETE FROM CreditVouchersBody WHERE VoucherNo = {0};
            IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL DELETE FROM CreditVouchers WHERE VoucherNo = {0};
            IF OBJECT_ID('DebitVouchersBody', 'U') IS NOT NULL DELETE FROM DebitVouchersBody WHERE VoucherNo = {0};
            IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL DELETE FROM DebitVouchers WHERE VoucherNo = {0};
            IF OBJECT_ID('JournalVouchersBody', 'U') IS NOT NULL DELETE FROM JournalVouchersBody WHERE VoucherNo = {0};
            IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL DELETE FROM JournalVouchers WHERE VoucherNo = {0};
            IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL DELETE FROM CashNoteDetail WHERE VoucherNo = {0};", voucherId);

        try
        {
            var voucher = await context.Vouchers.Include(v => v.Details).FirstOrDefaultAsync(v => v.Id == voucherId);
            if (voucher != null)
            {
                var glEntries = await context.GeneralLedger
                    .Where(gl => gl.SourceType == SourceType.Voucher && gl.SourceId == voucherId)
                    .ToListAsync();
                context.GeneralLedger.RemoveRange(glEntries);
                context.VoucherDetails.RemoveRange(voucher.Details);
                context.Vouchers.Remove(voucher);
                await context.SaveChangesAsync();
            }
        }
        catch { }
    }

    // ═════════════════════════════════════════════════════════════════════════════
    // AGENSOFT EXACT COMPATIBILITY ENGINE (100% REPLICATION OF VB6/SQL LOGIC)
    // ═════════════════════════════════════════════════════════════════════════════

    private static long ParseVoucherNumber(string? voucherNo, int fallbackId)
    {
        if (string.IsNullOrWhiteSpace(voucherNo)) return fallbackId > 0 ? fallbackId : 1;
        var digits = new string(voucherNo.Where(char.IsDigit).ToArray());
        return long.TryParse(digits, out var val) && val > 0 ? val : (fallbackId > 0 ? fallbackId : 1);
    }

    private static async Task EnsureChartOfAccountsAsync(AppDbContext context)
    {
        try
        {
            var sql = @"
                IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL AND OBJECT_ID('Accounts', 'U') IS NOT NULL
                BEGIN
                    INSERT INTO ChartOfAccounts (
                        AccountNo, UserID, AccountType, OpeningDebit, OpeningCredit, 
                        AdjustedDebit, AdjustedCredit, IsDetailed, IsLocked, IsPosted, 
                        IsEditable, BalFlag, PLFlag, ExpFlag, AccountName, AccountNameUrdu
                    )
                    SELECT a.AccountNo, 1, 'D', 0, 0, 0, 0, 1, 0, 1, 1, 1, 'B', 0, MAX(a.Name), MAX(a.Name)
                    FROM Accounts a
                    WHERE a.AccountNo IS NOT NULL 
                      AND NOT EXISTS (SELECT 1 FROM ChartOfAccounts c WHERE c.AccountNo = a.AccountNo)
                    GROUP BY a.AccountNo;
                END";
            await context.Database.ExecuteSqlRawAsync(sql);
        }
        catch
        {
            // Resilient failover
        }
    }

    private async Task SyncLegacyVoucherAsync(Voucher voucher, AppDbContext context)
    {
        try
        {
            long voucherNum = ParseVoucherNumber(voucher.VoucherNo, voucher.Id);
            var voucherDate = voucher.VoucherDate.Date;
            
            short userNo = voucher.CreatedBy > 0 ? (short)voucher.CreatedBy : (short)1;
            if (voucher.CreatedBy <= 0)
            {
                try
                {
                    var uVal = await ExecuteScalarLongAsync(context, 
                        "IF OBJECT_ID('Users', 'U') IS NOT NULL SELECT TOP 1 CAST(UserID AS SMALLINT) FROM Users; ELSE SELECT CAST(1 AS SMALLINT);");
                    if (uVal > 0) userNo = (short)uVal;
                }
                catch { userNo = 1; }
            }

            int branchId = 1;
            try
            {
                var bVal = await ExecuteScalarLongAsync(context, 
                    "IF OBJECT_ID('Branch', 'U') IS NOT NULL SELECT TOP 1 CAST(BranchID AS INT) FROM Branch; ELSE SELECT CAST(1 AS INT);");
                if (bVal > 0) branchId = (int)bVal;
            }
            catch { branchId = 1; }

            int isPosted = voucher.IsPosted ? 1 : 0;
            var narrationUrdu = voucher.NarrationUrdu ?? string.Empty;
            var cropSeason = voucher.CropSeason?.Name ?? string.Empty;
            if (cropSeason.Length > 5) cropSeason = cropSeason.Substring(0, 5);

            var accountMap = new Dictionary<int, string>();
            foreach (var detail in voucher.Details)
            {
                if (!string.IsNullOrWhiteSpace(detail.AccountNo))
                {
                    accountMap[detail.AccountId] = detail.AccountNo.Trim();
                }
            }

            var missingIds = voucher.Details
                .Where(d => !accountMap.ContainsKey(d.AccountId))
                .Select(d => d.AccountId)
                .Distinct()
                .ToList();

            if (missingIds.Any())
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                foreach (var id in missingIds)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT AccountNo FROM ChartOfAccounts WHERE AccountNo = @acc";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@acc";
                    p.Value = id.ToString();
                    cmd.Parameters.Add(p);
                    var res = await cmd.ExecuteScalarAsync();
                    accountMap[id] = res?.ToString()?.Trim() ?? id.ToString();
                }
            }

            if (voucher.VoucherType == VoucherType.CashReceiving)
            {
                // 1. CreditVouchers (Master)
                var sqlMaster = @"
                    IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM CreditVouchersBody WHERE VoucherNo = {0} AND VoucherDate = {1};
                        DELETE FROM CreditVouchers WHERE VoucherNo = {0} AND VoucherDate = {1};
                        INSERT INTO CreditVouchers (VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, VoucherTime, CropSeason, BranchID, IDBranch)
                        VALUES ({0}, {1}, {2}, {3}, {4}, GETDATE(), {5}, {6}, {6});
                    END";
                await context.Database.ExecuteSqlRawAsync(sqlMaster, voucherNum, voucherDate, userNo, isPosted, narrationUrdu, cropSeason, branchId);

                // 2. CreditVouchersBody (Details)
                foreach (var detail in voucher.Details)
                {
                    var acNo = accountMap.TryGetValue(detail.AccountId, out var no) ? no : detail.AccountId.ToString();
                    var narration = detail.Narration ?? string.Empty;
                    var credit = detail.Credit;

                    var sqlBody = @"
                        IF OBJECT_ID('CreditVouchersBody', 'U') IS NOT NULL
                        BEGIN
                            INSERT INTO CreditVouchersBody (AccountNo, VoucherNo, VoucherDate, Narration, Credit)
                            VALUES ({0}, {1}, {2}, {3}, {4});
                        END";
                    await context.Database.ExecuteSqlRawAsync(sqlBody, acNo, voucherNum, voucherDate, narration, credit);
                }

                // 3. CashNoteDetail (Denominations: 'CV')
                var sqlNotes = @"
                    IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM CashNoteDetail WHERE VoucherType = 'CV' AND VoucherNo = {0};
                        INSERT INTO CashNoteDetail (VoucherType, VoucherNo, VoucherDate, R5000, R1000, R500, R100, R50, R20, R10, R5, R2, R1)
                        VALUES ('CV', {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11});
                    END";
                await context.Database.ExecuteSqlRawAsync(sqlNotes, voucherNum, voucherDate, 
                    voucher.Count5000, voucher.Count1000, voucher.Count500, voucher.Count100, 
                    voucher.Count50, voucher.Count20, voucher.Count10, voucher.Count5, 
                    voucher.Count2, voucher.Count1);
            }
            else if (voucher.VoucherType == VoucherType.CashPayment)
            {
                // 1. DebitVouchers (Master)
                var sqlMaster = @"
                    IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM DebitVouchersBody WHERE VoucherNo = {0} AND VoucherDate = {1};
                        DELETE FROM DebitVouchers WHERE VoucherNo = {0} AND VoucherDate = {1};
                        INSERT INTO DebitVouchers (VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, VoucherTime, CropSeason, BranchID, IDBranch)
                        VALUES ({0}, {1}, {2}, {3}, {4}, GETDATE(), {5}, {6}, {6});
                    END";
                await context.Database.ExecuteSqlRawAsync(sqlMaster, voucherNum, voucherDate, userNo, isPosted, narrationUrdu, cropSeason, branchId);

                // 2. DebitVouchersBody (Details - Debit side)
                foreach (var detail in voucher.Details.Where(d => d.Debit > 0))
                {
                    var acNo = accountMap.TryGetValue(detail.AccountId, out var no) ? no : detail.AccountId.ToString();
                    var narration = detail.Narration ?? string.Empty;
                    var debit = detail.Debit;

                    var sqlBody = @"
                        IF OBJECT_ID('DebitVouchersBody', 'U') IS NOT NULL
                        BEGIN
                            INSERT INTO DebitVouchersBody (AccountNo, VoucherNo, VoucherDate, Narration, Debit)
                            VALUES ({0}, {1}, {2}, {3}, {4});
                        END";
                    await context.Database.ExecuteSqlRawAsync(sqlBody, acNo, voucherNum, voucherDate, narration, debit);
                }

                // 3. WHT DUAL VOUCHER LOGIC (Exact AgenSOFT FrmCashWHT Behavior)
                // If there is any Withholding Tax credit deduction line, mirror it as a linked Credit Voucher!
                var whtLines = voucher.Details.Where(d => d.Credit > 0).ToList();
                foreach (var whtDetail in whtLines)
                {
                    string whtAcNo = !string.IsNullOrWhiteSpace(whtDetail.AccountNo) 
                        ? whtDetail.AccountNo.Trim() 
                        : (accountMap.TryGetValue(whtDetail.AccountId, out var wNo) ? wNo : "2303");
                    if (string.IsNullOrWhiteSpace(whtAcNo)) whtAcNo = "2303";

                    var partyDetail = voucher.Details.FirstOrDefault(d => d.Debit > 0);
                    var partyAc = partyDetail != null 
                        ? (!string.IsNullOrWhiteSpace(partyDetail.AccountNo) ? partyDetail.AccountNo.Trim() : (accountMap.TryGetValue(partyDetail.AccountId, out var pNo) ? pNo : ""))
                        : "";

                    string partyName = partyAc;
                    try
                    {
                        var pName = await context.Database.SqlQueryRaw<string>("SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = {0}", partyAc).FirstOrDefaultAsync();
                        if (!string.IsNullOrEmpty(pName)) partyName = pName;
                    }
                    catch { }

                    // AgenSoft gets next Credit Voucher number: Select isnull(max(voucherno),0)+1 from CreditVouchers
                    long crVoucherNo = await ExecuteScalarLongAsync(context, 
                        "IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL SELECT CAST(ISNULL(MAX(VoucherNo), 0) + 1 AS BIGINT) FROM CreditVouchers; ELSE SELECT CAST(1 AS BIGINT);");
                    if (crVoucherNo <= 0) crVoucherNo = 1;

                    var sqlWhtCredit = @"
                        IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                        BEGIN
                            DELETE FROM CreditVouchersBody WHERE VoucherNo = {0} AND VoucherDate = {1} AND AccountNo = {6};
                            DELETE FROM CreditVouchers WHERE VoucherNo = {0} AND VoucherDate = {1};
                            INSERT INTO CreditVouchers (VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, VoucherTime, CropSeason, BranchID, IDBranch)
                            VALUES ({0}, {1}, {2}, {3}, {4}, GETDATE(), {5}, {7}, {7});
                            INSERT INTO CreditVouchersBody (AccountNo, VoucherNo, VoucherDate, Narration, Credit)
                            VALUES ({6}, {0}, {1}, {8}, {9});
                            UPDATE CreditVouchers SET CropSeason = {5} WHERE VoucherNo = {0};
                            UPDATE CreditVouchers SET IsPosted = 1 WHERE VoucherNo = {0};
                        END";
                    var whtNarration = $"WHT received from {partyName} (CPV #{voucherNum})";
                    await context.Database.ExecuteSqlRawAsync(sqlWhtCredit, crVoucherNo, voucherDate, userNo, isPosted, narrationUrdu, cropSeason, whtAcNo, branchId, whtNarration, whtDetail.Credit);
                }

                // 4. CashNoteDetail (Denominations: 'DV')
                var sqlNotes = @"
                    IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM CashNoteDetail WHERE VoucherType = 'DV' AND VoucherNo = {0};
                        INSERT INTO CashNoteDetail (VoucherType, VoucherNo, VoucherDate, R5000, R1000, R500, R100, R50, R20, R10, R5, R2, R1)
                        VALUES ('DV', {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11});
                    END";
                await context.Database.ExecuteSqlRawAsync(sqlNotes, voucherNum, voucherDate, 
                    voucher.Count5000, voucher.Count1000, voucher.Count500, voucher.Count100, 
                    voucher.Count50, voucher.Count20, voucher.Count10, voucher.Count5, 
                    voucher.Count2, voucher.Count1);
            }
            else if (voucher.VoucherType == VoucherType.JournalVoucher)
            {
                // 1. JournalVouchers (Master)
                var sqlMaster = @"
                    IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM JournalVouchersBody WHERE VoucherNo = {0} AND VoucherDate = {1};
                        DELETE FROM JournalVouchers WHERE VoucherNo = {0} AND VoucherDate = {1};
                        INSERT INTO JournalVouchers (VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, VoucherTime, CropSeason, BranchID, IDBranch)
                        VALUES ({0}, {1}, {2}, {3}, {4}, GETDATE(), {5}, {6}, {6});
                    END";
                await context.Database.ExecuteSqlRawAsync(sqlMaster, voucherNum, voucherDate, userNo, isPosted, narrationUrdu, cropSeason, branchId);

                // 2. JournalVouchersBody (Multi-row with Debit & Credit per line)
                foreach (var detail in voucher.Details)
                {
                    string acNo = !string.IsNullOrWhiteSpace(detail.AccountNo) 
                        ? detail.AccountNo.Trim() 
                        : (accountMap.TryGetValue(detail.AccountId, out var no) ? no : detail.AccountId.ToString());
                    var narration = detail.Narration ?? string.Empty;
                    var debit = detail.Debit;
                    var credit = detail.Credit;

                    var sqlBody = @"
                        IF OBJECT_ID('JournalVouchersBody', 'U') IS NOT NULL
                        BEGIN
                            INSERT INTO JournalVouchersBody (AccountNo, VoucherNo, VoucherDate, Narration, Debit, Credit)
                            VALUES ({0}, {1}, {2}, {3}, {4}, {5});
                        END";
                    await context.Database.ExecuteSqlRawAsync(sqlBody, acNo, voucherNum, voucherDate, narration, debit, credit);
                }
            }
        }
        catch (Exception exSync)
        {
            try {
                System.IO.File.AppendAllText(@"D:\Projects\agentcore\agentcore\startup.log", $"[{DateTime.Now:HH:mm:ss.fff}] [SyncLegacyVoucherAsync ERROR] {exSync}\r\n");
            } catch {}
            throw;
        }
    }

    private static async Task UpdateLegacyPostedAsync(Voucher voucher, AppDbContext context, bool isPosted = true)
    {
        try
        {
            long voucherNum = ParseVoucherNumber(voucher.VoucherNo, voucher.Id);
            var voucherDate = voucher.VoucherDate.Date;
            int flag = isPosted ? 1 : 0;

            if (voucher.VoucherType == VoucherType.CashReceiving)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                        UPDATE CreditVouchers SET IsPosted = {0} WHERE VoucherNo = {1} AND VoucherDate = {2};", flag, voucherNum, voucherDate);
            }
            else if (voucher.VoucherType == VoucherType.CashPayment)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL
                        UPDATE DebitVouchers SET IsPosted = {0} WHERE VoucherNo = {1} AND VoucherDate = {2};
                    IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                        UPDATE CreditVouchers SET IsPosted = {0} WHERE VoucherNo = {1} AND VoucherDate = {2};", flag, voucherNum, voucherDate);
            }
            else if (voucher.VoucherType == VoucherType.JournalVoucher)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL
                        UPDATE JournalVouchers SET IsPosted = {0} WHERE VoucherNo = {1} AND VoucherDate = {2};", flag, voucherNum, voucherDate);
            }
        }
        catch
        {
            // Safe ignore
        }
    }

    private static async Task DeleteLegacyVoucherAsync(Voucher voucher, AppDbContext context)
    {
        try
        {
            long voucherNum = ParseVoucherNumber(voucher.VoucherNo, voucher.Id);

            if (voucher.VoucherType == VoucherType.CashReceiving)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('CreditVouchersBody', 'U') IS NOT NULL DELETE FROM CreditVouchersBody WHERE VoucherNo = {0};
                    IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL DELETE FROM CreditVouchers WHERE VoucherNo = {0};
                    IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL DELETE FROM CashNoteDetail WHERE VoucherType = 'CV' AND VoucherNo = {0};", voucherNum);
            }
            else if (voucher.VoucherType == VoucherType.CashPayment)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('DebitVouchersBody', 'U') IS NOT NULL DELETE FROM DebitVouchersBody WHERE VoucherNo = {0};
                    IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL DELETE FROM DebitVouchers WHERE VoucherNo = {0};
                    IF OBJECT_ID('CreditVouchersBody', 'U') IS NOT NULL DELETE FROM CreditVouchersBody WHERE VoucherNo = {0} AND Narration LIKE '%WHT%';
                    IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL DELETE FROM CreditVouchers WHERE VoucherNo = {0} AND NarrationUrdu LIKE '%WHT%';
                    IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL DELETE FROM CashNoteDetail WHERE VoucherType = 'DV' AND VoucherNo = {0};", voucherNum);
            }
            else if (voucher.VoucherType == VoucherType.JournalVoucher)
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('JournalVouchersBody', 'U') IS NOT NULL DELETE FROM JournalVouchersBody WHERE VoucherNo = {0};
                    IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL DELETE FROM JournalVouchers WHERE VoucherNo = {0};", voucherNum);
            }
        }
        catch
        {
            // Safe ignore
        }
    }

    private static async Task<Voucher?> LoadLegacyVoucherByNoAsync(VoucherType voucherType, string voucherNo, AppDbContext context)
    {
        try
        {
            long voucherNum = ParseVoucherNumber(voucherNo, 0);
            if (voucherNum <= 0) return null;

            var connection = context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using var cmd = connection.CreateCommand();

            if (voucherType == VoucherType.CashPayment)
            {
                cmd.CommandText = @"
                    IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL
                        SELECT TOP 1 VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, CropSeason 
                        FROM DebitVouchers WHERE VoucherNo = @vNo";
                var p = cmd.CreateParameter();
                p.ParameterName = "@vNo";
                p.Value = voucherNum;
                cmd.Parameters.Add(p);

                DateTime vDate = DateTime.Today;
                bool isPosted = false;
                string? narrationUrdu = null;
                string? seasonName = null;
                bool found = false;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        found = true;
                        vDate = reader.GetDateTime(1);
                        isPosted = reader.GetBoolean(3);
                        narrationUrdu = reader.IsDBNull(4) ? null : reader.GetString(4);
                        seasonName = reader.IsDBNull(5) ? null : reader.GetString(5);
                    }
                }

                if (!found) return null;

                var v = new Voucher
                {
                    Id = (int)voucherNum,
                    VoucherNo = voucherNum.ToString(),
                    VoucherType = VoucherType.CashPayment,
                    VoucherDate = vDate,
                    IsPosted = isPosted,
                    Status = isPosted ? VoucherStatus.Posted : VoucherStatus.Draft,
                    NarrationUrdu = narrationUrdu
                };

                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    IF OBJECT_ID('DebitVouchersBody', 'U') IS NOT NULL
                        SELECT b.AccountNo, b.Narration, b.Debit, coa.AccountName 
                        FROM DebitVouchersBody b 
                        LEFT JOIN ChartOfAccounts coa ON b.AccountNo = coa.AccountNo
                        WHERE b.VoucherNo = @vNo ORDER BY b.SerialNo ASC";
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@vNo";
                p2.Value = voucherNum;
                cmd.Parameters.Add(p2);

                var lineNo = 1;
                using (var bodyReader = await cmd.ExecuteReaderAsync())
                {
                    while (await bodyReader.ReadAsync())
                    {
                        var acNo = bodyReader.GetString(0);
                        var narr = bodyReader.IsDBNull(1) ? null : bodyReader.GetString(1);
                        var debit = bodyReader.GetDecimal(2);
                        var coaName = bodyReader.IsDBNull(3) ? null : bodyReader.GetString(3);

                        var ac = new Account { AccountNo = acNo, Name = coaName ?? acNo, Id = (int)ParseVoucherNumber(acNo, 1) };

                        v.Details.Add(new VoucherDetail
                        {
                            LineNo = lineNo++,
                            AccountId = ac.Id,
                            AccountNo = acNo,
                            Account = ac,
                            Narration = narr,
                            Debit = debit,
                            Credit = 0m
                        });
                        v.TotalDebit += debit;
                    }
                }

                // Denominations
                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL
                        SELECT TOP 1 R5000, R1000, R500, R100, R50, R20, R10, R5, R2, R1 
                        FROM CashNoteDetail WHERE VoucherType = 'DV' AND VoucherNo = @vNo";
                var p3 = cmd.CreateParameter();
                p3.ParameterName = "@vNo";
                p3.Value = voucherNum;
                cmd.Parameters.Add(p3);

                using (var noteReader = await cmd.ExecuteReaderAsync())
                {
                    if (await noteReader.ReadAsync())
                    {
                        v.Count5000 = (int)noteReader.GetDecimal(0);
                        v.Count1000 = (int)noteReader.GetDecimal(1);
                        v.Count500 = (int)noteReader.GetDecimal(2);
                        v.Count100 = (int)noteReader.GetDecimal(3);
                        v.Count50 = (int)noteReader.GetDecimal(4);
                        v.Count20 = (int)noteReader.GetDecimal(5);
                        v.Count10 = (int)noteReader.GetDecimal(6);
                        v.Count5 = (int)noteReader.GetDecimal(7);
                        v.Count2 = (int)noteReader.GetDecimal(8);
                        v.Count1 = (int)noteReader.GetDecimal(9);
                    }
                }

                return v;
            }
            else if (voucherType == VoucherType.CashReceiving)
            {
                cmd.CommandText = @"
                    IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                        SELECT TOP 1 VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, CropSeason 
                        FROM CreditVouchers WHERE VoucherNo = @vNo";
                var p = cmd.CreateParameter();
                p.ParameterName = "@vNo";
                p.Value = voucherNum;
                cmd.Parameters.Add(p);

                DateTime vDate = DateTime.Today;
                bool isPosted = false;
                string? narrationUrdu = null;
                string? seasonName = null;
                bool found = false;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        found = true;
                        vDate = reader.GetDateTime(1);
                        isPosted = reader.GetBoolean(3);
                        narrationUrdu = reader.IsDBNull(4) ? null : reader.GetString(4);
                        seasonName = reader.IsDBNull(5) ? null : reader.GetString(5);
                    }
                }

                if (!found) return null;

                var v = new Voucher
                {
                    Id = (int)voucherNum,
                    VoucherNo = voucherNum.ToString(),
                    VoucherType = VoucherType.CashReceiving,
                    VoucherDate = vDate,
                    IsPosted = isPosted,
                    Status = isPosted ? VoucherStatus.Posted : VoucherStatus.Draft,
                    NarrationUrdu = narrationUrdu
                };

                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    IF OBJECT_ID('CreditVouchersBody', 'U') IS NOT NULL
                        SELECT b.AccountNo, b.Narration, b.Credit, coa.AccountName 
                        FROM CreditVouchersBody b 
                        LEFT JOIN ChartOfAccounts coa ON b.AccountNo = coa.AccountNo
                        WHERE b.VoucherNo = @vNo ORDER BY b.SerialNo ASC";
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@vNo";
                p2.Value = voucherNum;
                cmd.Parameters.Add(p2);

                var lineNo = 1;
                using (var bodyReader = await cmd.ExecuteReaderAsync())
                {
                    while (await bodyReader.ReadAsync())
                    {
                        var acNo = bodyReader.GetString(0);
                        var narr = bodyReader.IsDBNull(1) ? null : bodyReader.GetString(1);
                        var credit = bodyReader.GetDecimal(2);
                        var coaName = bodyReader.IsDBNull(3) ? null : bodyReader.GetString(3);

                        var ac = new Account { AccountNo = acNo, Name = coaName ?? acNo, Id = (int)ParseVoucherNumber(acNo, 1) };

                        v.Details.Add(new VoucherDetail
                        {
                            LineNo = lineNo++,
                            AccountId = ac.Id,
                            AccountNo = acNo,
                            Account = ac,
                            Narration = narr,
                            Debit = 0m,
                            Credit = credit
                        });
                        v.TotalCredit += credit;
                    }
                }

                // Denominations
                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL
                        SELECT TOP 1 R5000, R1000, R500, R100, R50, R20, R10, R5, R2, R1 
                        FROM CashNoteDetail WHERE VoucherType = 'CV' AND VoucherNo = @vNo";
                var p3 = cmd.CreateParameter();
                p3.ParameterName = "@vNo";
                p3.Value = voucherNum;
                cmd.Parameters.Add(p3);

                using (var noteReader = await cmd.ExecuteReaderAsync())
                {
                    if (await noteReader.ReadAsync())
                    {
                        v.Count5000 = (int)noteReader.GetDecimal(0);
                        v.Count1000 = (int)noteReader.GetDecimal(1);
                        v.Count500 = (int)noteReader.GetDecimal(2);
                        v.Count100 = (int)noteReader.GetDecimal(3);
                        v.Count50 = (int)noteReader.GetDecimal(4);
                        v.Count20 = (int)noteReader.GetDecimal(5);
                        v.Count10 = (int)noteReader.GetDecimal(6);
                        v.Count5 = (int)noteReader.GetDecimal(7);
                        v.Count2 = (int)noteReader.GetDecimal(8);
                        v.Count1 = (int)noteReader.GetDecimal(9);
                    }
                }

                return v;
            }
            else if (voucherType == VoucherType.JournalVoucher)
            {
                cmd.CommandText = @"
                    IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL
                        SELECT TOP 1 VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, CropSeason 
                        FROM JournalVouchers WHERE VoucherNo = @vNo";
                var p = cmd.CreateParameter();
                p.ParameterName = "@vNo";
                p.Value = voucherNum;
                cmd.Parameters.Add(p);

                DateTime vDate = DateTime.Today;
                bool isPosted = false;
                string? narrationUrdu = null;
                string? seasonName = null;
                bool found = false;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        found = true;
                        vDate = reader.GetDateTime(1);
                        isPosted = reader.GetBoolean(3);
                        narrationUrdu = reader.IsDBNull(4) ? null : reader.GetString(4);
                        seasonName = reader.IsDBNull(5) ? null : reader.GetString(5);
                    }
                }

                if (!found) return null;

                var v = new Voucher
                {
                    Id = (int)voucherNum,
                    VoucherNo = voucherNum.ToString(),
                    VoucherType = VoucherType.JournalVoucher,
                    VoucherDate = vDate,
                    IsPosted = isPosted,
                    Status = isPosted ? VoucherStatus.Posted : VoucherStatus.Draft,
                    NarrationUrdu = narrationUrdu
                };

                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    IF OBJECT_ID('JournalVouchersBody', 'U') IS NOT NULL
                        SELECT b.AccountNo, b.Narration, b.Debit, b.Credit, coa.AccountName 
                        FROM JournalVouchersBody b 
                        LEFT JOIN ChartOfAccounts coa ON b.AccountNo = coa.AccountNo
                        WHERE b.VoucherNo = @vNo ORDER BY b.SerialNo ASC";
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@vNo";
                p2.Value = voucherNum;
                cmd.Parameters.Add(p2);

                var lineNo = 1;
                using (var bodyReader = await cmd.ExecuteReaderAsync())
                {
                    while (await bodyReader.ReadAsync())
                    {
                        var acNo = bodyReader.GetString(0);
                        var narr = bodyReader.IsDBNull(1) ? null : bodyReader.GetString(1);
                        var debit = bodyReader.GetDecimal(2);
                        var credit = bodyReader.GetDecimal(3);
                        var coaName = bodyReader.IsDBNull(4) ? null : bodyReader.GetString(4);

                        var ac = new Account { AccountNo = acNo, Name = coaName ?? acNo, Id = (int)ParseVoucherNumber(acNo, 1) };

                        v.Details.Add(new VoucherDetail
                        {
                            LineNo = lineNo++,
                            AccountId = ac.Id,
                            AccountNo = acNo,
                            Account = ac,
                            Narration = narr,
                            Debit = debit,
                            Credit = credit
                        });
                        v.TotalDebit += debit;
                        v.TotalCredit += credit;
                    }
                }

                return v;
            }
        }
        catch
        {
            // Resilient fallback
        }
        return null;
    }
}
