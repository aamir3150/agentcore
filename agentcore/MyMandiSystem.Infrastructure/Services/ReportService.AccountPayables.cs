using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public partial class ReportService
{
    private static readonly string[] VoucherSourceViews =
    {
        "VuAccVouchersAccounts",
        "VuAccVouchersOthers",
        "VuAccVouchersOthersExtension",
        "VuPestroVouchers",
        "VuAccVouchersOthersCrop",
        "VuAccVouchersOthersMultiPLS"
    };

    /// <summary>
    /// Reads Form_Load configuration flags matching AgenSoft RptAccountPayables:
    /// ShowNameInUrdu function, EnableCropSeason in Project_Registry, and company FY start date.
    /// </summary>
    public async Task<PayablesFormSettings> GetPayablesFormSettingsAsync()
    {
        bool showUrdu = false;
        bool enableCrop = false;
        DateTime fyStart = DateTime.Today.Month >= 7
            ? new DateTime(DateTime.Today.Year, 7, 1)
            : new DateTime(DateTime.Today.Year - 1, 7, 1);

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);

        if (await DbObjectExistsAsync(conn, "dbo.RegistryValue", "FN"))
        {
            var val = await ExecuteScalarStringAsync(conn, "SELECT dbo.RegistryValue('ShowNameInUrdu')");
            showUrdu = string.Equals(val?.Trim(), "1", StringComparison.OrdinalIgnoreCase);
        }

        if (await DbObjectExistsAsync(conn, "Project_Registry", "U"))
        {
            var cropFlag = await ExecuteScalarStringAsync(
                conn,
                "SELECT TOP 1 CAST(Value AS NVARCHAR(50)) FROM Project_Registry WHERE RegistryKey = 'EnableCropSeason' AND CAST(Value AS NVARCHAR(50)) = '1'");
            enableCrop = cropFlag == "1";
        }

        if (await DbObjectExistsAsync(conn, "FinancialYears", "U"))
        {
            try
            {
                using var fyCmd = conn.CreateCommand();
                fyCmd.CommandText = "SELECT TOP 1 StartDate FROM FinancialYears WHERE IsCurrent = 1 ORDER BY Id DESC";
                var obj = await fyCmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                    fyStart = Convert.ToDateTime(obj).Date;
            }
            catch { }
        }

        return new PayablesFormSettings
        {
            ShowUrduPrint = showUrdu,
            EnableCropSeason = enableCrop,
            FinancialYearStartDate = fyStart
        };
    }

    public async Task<IReadOnlyList<LookupOption>> GetPayablesCropSeasonsAsync()
    {
        var list = new List<LookupOption>
        {
            new() { Id = 0, Code = "0", Name = "All Crops" }
        };

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);
        if (!await DbObjectExistsAsync(conn, "Products", "U"))
            return list;

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ProductID, ProductName FROM Products WHERE (IsCrop = 1 OR IsBroker = 1)";
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            int id = r[0] == DBNull.Value ? 0 : Convert.ToInt32(r[0]);
            string name = r[1]?.ToString()?.Trim() ?? string.Empty;
            if (id <= 0 || string.IsNullOrEmpty(name)) continue;

            string idText = id.ToString();
            string padded = idText.Length < 5 ? "0" + idText : idText;
            list.Add(new LookupOption { Id = id, Code = padded, Name = name });
        }
        return list;
    }

    public async Task<IReadOnlyList<LookupOption>> GetPayablePartyTypesAsync()
    {
        var list = new List<LookupOption>
        {
            new() { Id = null, Code = string.Empty, Name = "------- All Parties -------" }
        };

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);
        if (!await DbObjectExistsAsync(conn, "ChartOfAccounts", "U"))
            return list;

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT AccountNo, AccountName FROM ChartOfAccounts WHERE AccountNo LIKE '6%' AND IsDetailed = 0 AND LEN(ParentAccountNo) > 0";
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            string accNo = r[0]?.ToString()?.Trim() ?? string.Empty;
            string name = r[1]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(accNo) && !string.IsNullOrEmpty(name))
                list.Add(new LookupOption { Code = accNo, Name = name });
        }
        return list;
    }

    public async Task<IReadOnlyList<LookupOption>> GetPartyGroupsLookupAsync()
    {
        var list = new List<LookupOption>
        {
            new() { Id = null, Code = string.Empty, Name = "--- ALL ---" }
        };

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);
        if (!await DbObjectExistsAsync(conn, "PartyGroups", "U"))
            return list;

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT PartyGroupID, GroupName FROM PartyGroups ORDER BY GroupName";
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            int id = r[0] == DBNull.Value ? 0 : Convert.ToInt32(r[0]);
            string name = r[1]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
                list.Add(new LookupOption { Id = id, Code = id.ToString(), Name = name });
        }
        return list;
    }

    public async Task<IReadOnlyList<string>> GetPartyCitiesLookupAsync()
    {
        var list = new List<string> { "------- All Cities -------" };

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);
        if (!await DbObjectExistsAsync(conn, "Parties", "U"))
            return list;

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT City FROM Parties WHERE ISNULL(City, '') <> '' GROUP BY City ORDER BY City";
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            string city = r[0]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(city))
                list.Add(city);
        }
        return list;
    }

    public async Task<IReadOnlyList<LookupOption>> GetTownsLookupAsync()
    {
        var list = new List<LookupOption>
        {
            new() { Id = null, Code = string.Empty, Name = "--- ALL TOWNS ---" }
        };

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);
        if (!await DbObjectExistsAsync(conn, "Towns", "U"))
            return list;

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            int id = r[0] == DBNull.Value ? 0 : Convert.ToInt32(r[0]);
            string name = r[1]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
                list.Add(new LookupOption { Id = id, Code = id.ToString(), Name = name });
        }
        return list;
    }

    public async Task<IReadOnlyList<LookupOption>> GetSectorsByTownLookupAsync(int? townId)
    {
        var list = new List<LookupOption>
        {
            new() { Id = null, Code = string.Empty, Name = "--- ALL SECTORS ---" }
        };

        if (!townId.HasValue || townId.Value <= 0)
            return list;

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);
        if (!await DbObjectExistsAsync(conn, "Sectors", "U"))
            return list;

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT DISTINCT SectorID, SectorName FROM Sectors WHERE TownID = @townId ORDER BY SectorName";
        AddParam(cmd, "@townId", townId.Value, DbType.Int32);
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            int id = r[0] == DBNull.Value ? 0 : Convert.ToInt32(r[0]);
            string name = r[1]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
                list.Add(new LookupOption { Id = id, Code = id.ToString(), Name = name });
        }
        return list;
    }

    /// <summary>
    /// Exact AgenSoft RptAccountPayables.SetReport (0x112a9b00) pipeline:
    /// 1. Executes SPAccAccountsBalancesFinal (with resilient schema-aware T-SQL fallback),
    /// 2. Filters IsDetailed=1, Party accounts, ClosingBal > MinAmount, ClosingBalType ('Cr' or 'Dr'), optional AccountNo prefix,
    /// 3. Sorts by AccountName or AccountNo ascending,
    /// 4. Joins filtered Parties (Town, Sector, PartyGroup, City), formats ContactNo, NameUrdu, and signed TodayBal.
    /// </summary>
    public async Task<List<AccountPayableBalanceRecord>> GetAccountPayablesRecordsAsync(AccountPayablesRequest request)
    {
        var (fromDate, toDate) = request.DateMode switch
        {
            PayablesDateMode.AllDates => (new DateTime(1901, 1, 1), new DateTime(2050, 1, 1)),
            PayablesDateMode.TillDate => (new DateTime(1901, 1, 1), (request.SingleDate ?? DateTime.Today).Date),
            PayablesDateMode.DateRange => ((request.FromDate ?? DateTime.Today).Date, (request.ToDate ?? DateTime.Today).Date),
            _ => (new DateTime(1901, 1, 1), new DateTime(2050, 1, 1))
        };

        string cropSeason = string.IsNullOrWhiteSpace(request.CropSeasonCode) ? "0" : request.CropSeasonCode.Trim();
        string targetBalType = request.IsReceivable ? "Dr" : "Cr";

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);

        var spRows = await ExecuteSpBalancesWithFallbackAsync(conn, fromDate, toDate, cropSeason);
        var filteredParties = await LoadFilteredPartiesAsync(conn, request);

        IEnumerable<SpBalanceRow> filtered = spRows.Where(r =>
            r.IsDetailed == 1 &&
            (r.AccountType.Equals("Party", StringComparison.OrdinalIgnoreCase) || r.AccountType.Equals("D", StringComparison.OrdinalIgnoreCase)) &&
            r.ClosingBal > request.MinAmount &&
            r.ClosingBalType.Equals(targetBalType, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.PartyTypeAccountNo))
        {
            string prefix = request.PartyTypeAccountNo.Trim();
            filtered = filtered.Where(r => r.AccountNo.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        IOrderedEnumerable<SpBalanceRow> sorted = request.SortByAccountName
            ? filtered.OrderBy(r => r.AccountName, StringComparer.OrdinalIgnoreCase)
            : filtered.OrderBy(r => r.AccountNo, StringComparer.OrdinalIgnoreCase);

        var output = new List<AccountPayableBalanceRecord>();
        foreach (var row in sorted)
        {
            if (!filteredParties.TryGetValue(row.AccountNo, out var party))
                continue;

            decimal signedTodayBal = row.TodayBalType.Equals("Cr", StringComparison.OrdinalIgnoreCase)
                ? -Math.Abs(row.TodayBal)
                : Math.Abs(row.TodayBal);

            output.Add(new AccountPayableBalanceRecord
            {
                AccountNo = row.AccountNo,
                AccountName = row.AccountName,
                AccountNameUrdu = party.NameUrdu,
                ContactNo = FormatContactNo(party.Mobile, party.Phone2, party.Phone1),
                AccountType = row.AccountType,
                IsDetailed = row.IsDetailed,
                PLFlag = row.PLFlag,
                BALFlag = row.BALFlag,
                OpeningDebit = row.OpeningDebit,
                OpeningCredit = row.OpeningCredit,
                OpeningBal = row.OpeningBal,
                OpeningBalType = row.OpeningBalType,
                Debit = row.Debit,
                Credit = row.Credit,
                Bal = row.Bal,
                BalType = row.BalType,
                ClosingDebit = row.ClosingDebit,
                ClosingCredit = row.ClosingCredit,
                ClosingBal = row.ClosingBal,
                ClosingBalType = row.ClosingBalType,
                TodayBal = signedTodayBal,
                TodayBalType = row.TodayBalType
            });
        }

        return output;
    }

    /// <summary>
    /// Exact AgenSoft RptAccountPayables.SendSMS (0x112b28f0) implementation:
    /// checks [dbo].[ComRecords], reads ControlNo from ComSettings and Mobile from Parties,
    /// and inserts outbound SMS records for each party and optionally the company owner.
    /// </summary>
    public async Task<int> SendPayablesSmsAsync(
        IEnumerable<AccountPayableBalanceRecord> records,
        string companyName,
        string companyPhone,
        string ownerMobile)
    {
        var list = records?.ToList() ?? new List<AccountPayableBalanceRecord>();
        if (list.Count == 0) return 0;

        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = await OpenConnectionAsync(context);

        if (!await DbObjectExistsAsync(conn, "ComRecords", "U"))
            return 0;

        string fromNo = await ExecuteScalarStringAsync(conn, "SELECT TOP 1 ISNULL(ControlNo, '') FROM ComSettings") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fromNo))
            return 0;

        int queuedCount = 0;
        bool hasParties = await DbObjectExistsAsync(conn, "Parties", "U");
        if (!hasParties) return 0;

        foreach (var rec in list)
        {
            string partyName = rec.AccountName;
            string mobileNo = string.Empty;

            using (var pCmd = conn.CreateCommand())
            {
                pCmd.CommandText = "SELECT TOP 1 ISNULL(PartyName, ''), REPLACE(ISNULL(Mobile, ''), '-', '') AS MobileNo FROM Parties WHERE PartyID = @pid";
                AddParam(pCmd, "@pid", rec.AccountNo);
                using var r = await pCmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    partyName = r[0]?.ToString()?.Trim() ?? partyName;
                    mobileNo = r[1]?.ToString()?.Trim() ?? string.Empty;
                }
            }

            string partyBody = $"M/S {partyName}\r your remaining balance is Rs.{rec.ClosingBal:0.##}/{rec.ClosingBalType}\rThanks.\r{companyName}\r{companyPhone}";
            if (!string.IsNullOrWhiteSpace(mobileNo))
            {
                await InsertComRecordAsync(conn, fromNo, mobileNo, partyBody, rec.AccountNo);
                queuedCount++;
            }

            if (!string.IsNullOrWhiteSpace(ownerMobile))
            {
                string ownerBody = $"M/S {partyName}\rremaining balance is Rs.{rec.ClosingBal:0.##}/{rec.ClosingBalType} \rThanks.\r{companyName}\r{companyPhone}";
                await InsertComRecordAsync(conn, fromNo, ownerMobile.Trim(), ownerBody, rec.AccountNo);
                queuedCount++;
            }
        }

        return queuedCount;
    }

    private async Task<IEnumerable<dynamic>> GetLegacyPartyBalancesAsync(
        bool isReceivable,
        DateTime? from,
        DateTime? to,
        string partyType,
        string partyGroup,
        string city,
        string town,
        string sector,
        decimal minAmount,
        bool showOnlyFinalBalance,
        bool sortByAccountName)
    {
        var partyTypes = await GetPayablePartyTypesAsync();
        var partyGroups = await GetPartyGroupsLookupAsync();
        var towns = await GetTownsLookupAsync();

        var selectedType = partyTypes.FirstOrDefault(x => x.Name.Equals(partyType, StringComparison.OrdinalIgnoreCase));
        var selectedGroup = partyGroups.FirstOrDefault(x => x.Name.Equals(partyGroup, StringComparison.OrdinalIgnoreCase));
        var selectedTown = towns.FirstOrDefault(x => x.Name.Equals(town, StringComparison.OrdinalIgnoreCase));
        var sectors = await GetSectorsByTownLookupAsync(selectedTown?.Id);
        var selectedSector = sectors.FirstOrDefault(x => x.Name.Equals(sector, StringComparison.OrdinalIgnoreCase));

        PayablesDateMode mode = (!from.HasValue && !to.HasValue)
            ? PayablesDateMode.AllDates
            : (!from.HasValue ? PayablesDateMode.TillDate : PayablesDateMode.DateRange);

        var rows = await GetAccountPayablesRecordsAsync(new AccountPayablesRequest
        {
            IsReceivable = isReceivable,
            DateMode = mode,
            SingleDate = mode == PayablesDateMode.TillDate ? to : null,
            FromDate = from,
            ToDate = to,
            PartyTypeAccountNo = selectedType?.Code,
            PartyTypeName = partyType ?? string.Empty,
            PartyGroupId = selectedGroup?.Id,
            PartyGroupName = partyGroup ?? string.Empty,
            CityName = string.IsNullOrWhiteSpace(city) || city == "------- All Cities -------" ? null : city,
            TownId = selectedTown?.Id,
            SectorId = selectedSector?.Id,
            MinAmount = minAmount,
            ShowOnlyFinalBalance = showOnlyFinalBalance,
            SortByAccountName = sortByAccountName
        });

        return rows.Cast<dynamic>().ToList();
    }

    private async Task<List<SpBalanceRow>> ExecuteSpBalancesWithFallbackAsync(
        DbConnection conn,
        DateTime fromDate,
        DateTime toDate,
        string cropSeason)
    {
        // Try executing native stored procedure first when available
        if (await DbObjectExistsAsync(conn, "SPAccAccountsBalancesFinal", "P"))
        {
            try
            {
                bool hasCropParam = await SpHasParameterAsync(conn, "SPAccAccountsBalancesFinal", "@CropSeason");
                using var spCmd = conn.CreateCommand();
                spCmd.CommandText = hasCropParam
                    ? "EXEC SPAccAccountsBalancesFinal @FromDate, @ToDate, @AccNo, @CropSeason"
                    : "EXEC SPAccAccountsBalancesFinal @FromDate, @ToDate, @AccNo";
                AddParam(spCmd, "@FromDate", fromDate.Date, DbType.DateTime);
                AddParam(spCmd, "@ToDate", toDate.Date, DbType.DateTime);
                AddParam(spCmd, "@AccNo", "%");
                if (hasCropParam)
                    AddParam(spCmd, "@CropSeason", cropSeason);

                var rows = new List<SpBalanceRow>();
                bool missingTodayBal = false;
                using (var r = await spCmd.ExecuteReaderAsync())
                {
                    int cAccNo = GetOrdinalSafe(r, "AccountNo");
                    int cAccName = GetOrdinalSafe(r, "AccountName");
                    int cAccType = GetOrdinalSafe(r, "AccountType");
                    int cDet = GetOrdinalSafe(r, "IsDetailed");
                    int cPl = GetOrdinalSafe(r, "PLFlag");
                    int cBalF = GetOrdinalSafe(r, "BALFlag");
                    int cOpDr = GetOrdinalSafe(r, "OpeningDebit");
                    int cOpCr = GetOrdinalSafe(r, "OpeningCredit");
                    int cOpBal = GetOrdinalSafe(r, "OpeningBal");
                    int cOpType = GetOrdinalSafe(r, "OpeningBalType");
                    int cDr = GetOrdinalSafe(r, "Debit");
                    int cCr = GetOrdinalSafe(r, "Credit");
                    int cBal = GetOrdinalSafe(r, "Bal");
                    int cBalType = GetOrdinalSafe(r, "BalType");
                    int cClDr = GetOrdinalSafe(r, "ClosingDebit");
                    int cClCr = GetOrdinalSafe(r, "ClosingCredit");
                    int cClBal = GetOrdinalSafe(r, "ClosingBal");
                    int cClType = GetOrdinalSafe(r, "ClosingBalType");
                    int cTdBal = GetOrdinalSafe(r, "TodayBal");
                    int cTdType = GetOrdinalSafe(r, "TodayBalType");
                    missingTodayBal = cTdBal < 0 || cTdType < 0;

                    while (await r.ReadAsync())
                    {
                        rows.Add(new SpBalanceRow
                        {
                            AccountNo = ReadStr(r, cAccNo),
                            AccountName = ReadStr(r, cAccName),
                            AccountType = ReadStr(r, cAccType),
                            IsDetailed = ReadBoolInt(r, cDet),
                            PLFlag = ReadStr(r, cPl),
                            BALFlag = ReadBoolInt(r, cBalF),
                            OpeningDebit = ReadDec(r, cOpDr),
                            OpeningCredit = ReadDec(r, cOpCr),
                            OpeningBal = ReadDec(r, cOpBal),
                            OpeningBalType = ReadStr(r, cOpType, "Dr"),
                            Debit = ReadDec(r, cDr),
                            Credit = ReadDec(r, cCr),
                            Bal = ReadDec(r, cBal),
                            BalType = ReadStr(r, cBalType, "Dr"),
                            ClosingDebit = ReadDec(r, cClDr),
                            ClosingCredit = ReadDec(r, cClCr),
                            ClosingBal = ReadDec(r, cClBal),
                            ClosingBalType = ReadStr(r, cClType, "Dr"),
                            TodayBal = cTdBal >= 0 ? ReadDec(r, cTdBal) : 0m,
                            TodayBalType = cTdType >= 0 ? ReadStr(r, cTdType, "Dr") : "Dr"
                        });
                    }
                }

                if (missingTodayBal && rows.Count > 0)
                {
                    var todayMap = await ComputeTodayBalancesAsync(conn, cropSeason);
                    foreach (var row in rows)
                    {
                        if (todayMap.TryGetValue(row.AccountNo, out var td))
                        {
                            row.TodayBal = td.TodayBal;
                            row.TodayBalType = td.TodayBalType;
                        }
                    }
                }

                return rows;
            }
            catch
            {
                // Fall through to schema-aware AgenSoft T-SQL computation when SP has view column mismatch
            }
        }

        return await ExecuteInlinedSpBalancesAsync(conn, fromDate, toDate, cropSeason);
    }

    private async Task<List<SpBalanceRow>> ExecuteInlinedSpBalancesAsync(
        DbConnection conn,
        DateTime fromDate,
        DateTime toDate,
        string cropSeason)
    {
        var voucherUnion = await BuildVoucherUnionSqlAsync(conn, cropSeason);
        DateTime todayCeiling = DateTime.Today.AddYears(10);

        string sql = string.IsNullOrEmpty(voucherUnion)
            ? @"
SELECT
    COA.AccountNo,
    COA.AccountName,
    COA.AccountType,
    ISNULL(COA.IsDetailed, 0) AS IsDetailed,
    ISNULL(COA.PLFlag, '') AS PLFlag,
    ISNULL(COA.BalFlag, 0) AS BALFlag,
    ROUND(ISNULL(COA.OpeningDebit, 0), 2) AS OpeningDebit,
    ROUND(ISNULL(COA.OpeningCredit, 0), 2) AS OpeningCredit,
    ABS(ROUND(ISNULL(COA.OpeningCredit, 0) - ISNULL(COA.OpeningDebit, 0), 2)) AS OpeningBal,
    CASE WHEN (ISNULL(COA.OpeningCredit, 0) - ISNULL(COA.OpeningDebit, 0)) > 0 THEN 'Cr' ELSE 'Dr' END AS OpeningBalType,
    CAST(0 AS DECIMAL(14,2)) AS Debit,
    CAST(0 AS DECIMAL(14,2)) AS Credit,
    CAST(0 AS DECIMAL(14,2)) AS Bal,
    'Dr' AS BalType,
    ROUND(ISNULL(COA.OpeningDebit, 0), 2) AS ClosingDebit,
    ROUND(ISNULL(COA.OpeningCredit, 0), 2) AS ClosingCredit,
    ABS(ROUND(ISNULL(COA.OpeningCredit, 0) - ISNULL(COA.OpeningDebit, 0), 2)) AS ClosingBal,
    CASE WHEN (ISNULL(COA.OpeningCredit, 0) - ISNULL(COA.OpeningDebit, 0)) > 0 THEN 'Cr' ELSE 'Dr' END AS ClosingBalType,
    ABS(ROUND(ISNULL(COA.OpeningCredit, 0) - ISNULL(COA.OpeningDebit, 0), 2)) AS TodayBal,
    CASE WHEN (ISNULL(COA.OpeningCredit, 0) - ISNULL(COA.OpeningDebit, 0)) > 0 THEN 'Cr' ELSE 'Dr' END AS TodayBalType
FROM ChartOfAccounts COA
WHERE COA.IsDetailed = 1;"
            : $@"
;WITH AllVouchers AS (
{voucherUnion}
),
DetailedAgg AS (
    SELECT
        COA.AccountNo,
        COA.AccountName,
        COA.AccountType,
        ISNULL(COA.IsDetailed, 0) AS IsDetailed,
        ISNULL(COA.PLFlag, '') AS PLFlag,
        ISNULL(COA.BalFlag, 0) AS BALFlag,
        ISNULL(COA.OpeningDebit, 0) + SUM(CASE WHEN ISNULL(V.VoucherDate, '1900-01-01') < @FromDate THEN ROUND(ISNULL(V.Debit, 0), 2) ELSE 0 END) AS OpeningDebit,
        ISNULL(COA.OpeningCredit, 0) + SUM(CASE WHEN ISNULL(V.VoucherDate, '1900-01-01') < @FromDate THEN ROUND(ISNULL(V.Credit, 0), 2) ELSE 0 END) AS OpeningCredit,
        SUM(CASE WHEN V.VoucherDate BETWEEN @FromDate AND @ToDate THEN ROUND(ISNULL(V.Debit, 0), 2) ELSE 0 END) AS Debit,
        SUM(CASE WHEN V.VoucherDate BETWEEN @FromDate AND @ToDate THEN ROUND(ISNULL(V.Credit, 0), 2) ELSE 0 END) AS Credit,
        ABS((ISNULL(COA.OpeningDebit, 0) + SUM(CASE WHEN ISNULL(V.VoucherDate, '1900-01-01') <= @TodayDate THEN ROUND(ISNULL(V.Debit, 0), 2) ELSE 0 END))
          - (ISNULL(COA.OpeningCredit, 0) + SUM(CASE WHEN ISNULL(V.VoucherDate, '1900-01-01') <= @TodayDate THEN ROUND(ISNULL(V.Credit, 0), 2) ELSE 0 END))) AS TodayBal,
        CASE WHEN ((ISNULL(COA.OpeningDebit, 0) + SUM(CASE WHEN ISNULL(V.VoucherDate, '1900-01-01') <= @TodayDate THEN ROUND(ISNULL(V.Debit, 0), 2) ELSE 0 END))
                 - (ISNULL(COA.OpeningCredit, 0) + SUM(CASE WHEN ISNULL(V.VoucherDate, '1900-01-01') <= @TodayDate THEN ROUND(ISNULL(V.Credit, 0), 2) ELSE 0 END))) < 0
             THEN 'Cr' ELSE 'Dr' END AS TodayBalType
    FROM ChartOfAccounts COA
    LEFT OUTER JOIN AllVouchers V
        ON COA.AccountNo COLLATE SQL_Latin1_General_CP1_CI_AS = V.AccountNo COLLATE SQL_Latin1_General_CP1_CI_AS
    WHERE COA.IsDetailed = 1
    GROUP BY COA.AccountNo, COA.AccountName, COA.AccountType, COA.IsDetailed, COA.PLFlag, COA.BalFlag, COA.OpeningDebit, COA.OpeningCredit
)
SELECT
    AccountNo,
    AccountName,
    AccountType,
    IsDetailed,
    PLFlag,
    BALFlag,
    OpeningDebit,
    OpeningCredit,
    ABS(OpeningCredit - OpeningDebit) AS OpeningBal,
    CASE WHEN (OpeningCredit - OpeningDebit) > 0 THEN 'Cr' ELSE 'Dr' END AS OpeningBalType,
    Debit,
    Credit,
    ABS(Credit - Debit) AS Bal,
    CASE WHEN (Credit - Debit) > 0 THEN 'Cr' ELSE 'Dr' END AS BalType,
    OpeningDebit + Debit AS ClosingDebit,
    OpeningCredit + Credit AS ClosingCredit,
    ABS((OpeningCredit + Credit) - (OpeningDebit + Debit)) AS ClosingBal,
    CASE WHEN ((OpeningCredit + Credit) - (OpeningDebit + Debit)) > 0 THEN 'Cr' ELSE 'Dr' END AS ClosingBalType,
    TodayBal,
    TodayBalType
FROM DetailedAgg;";

        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        AddParam(cmd, "@FromDate", fromDate.Date, DbType.DateTime);
        AddParam(cmd, "@ToDate", toDate.Date, DbType.DateTime);
        AddParam(cmd, "@TodayDate", todayCeiling, DbType.DateTime);

        var rows = new List<SpBalanceRow>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            rows.Add(new SpBalanceRow
            {
                AccountNo = ReadStr(r, 0),
                AccountName = ReadStr(r, 1),
                AccountType = ReadStr(r, 2),
                IsDetailed = ReadBoolInt(r, 3),
                PLFlag = ReadStr(r, 4),
                BALFlag = ReadBoolInt(r, 5),
                OpeningDebit = ReadDec(r, 6),
                OpeningCredit = ReadDec(r, 7),
                OpeningBal = ReadDec(r, 8),
                OpeningBalType = ReadStr(r, 9, "Dr"),
                Debit = ReadDec(r, 10),
                Credit = ReadDec(r, 11),
                Bal = ReadDec(r, 12),
                BalType = ReadStr(r, 13, "Dr"),
                ClosingDebit = ReadDec(r, 14),
                ClosingCredit = ReadDec(r, 15),
                ClosingBal = ReadDec(r, 16),
                ClosingBalType = ReadStr(r, 17, "Dr"),
                TodayBal = ReadDec(r, 18),
                TodayBalType = ReadStr(r, 19, "Dr")
            });
        }
        return rows;
    }

    private async Task<Dictionary<string, (decimal TodayBal, string TodayBalType)>> ComputeTodayBalancesAsync(
        DbConnection conn,
        string cropSeason)
    {
        var map = new Dictionary<string, (decimal, string)>(StringComparer.OrdinalIgnoreCase);
        var voucherUnion = await BuildVoucherUnionSqlAsync(conn, cropSeason);
        DateTime todayCeiling = DateTime.Today.AddYears(10);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = string.IsNullOrEmpty(voucherUnion)
            ? @"
SELECT
    AccountNo,
    ABS(ISNULL(OpeningDebit, 0) - ISNULL(OpeningCredit, 0)) AS TodayBal,
    CASE WHEN (ISNULL(OpeningDebit, 0) - ISNULL(OpeningCredit, 0)) < 0 THEN 'Cr' ELSE 'Dr' END AS TodayBalType
FROM ChartOfAccounts
WHERE IsDetailed = 1;"
            : $@"
;WITH AllVouchers AS (
{voucherUnion}
)
SELECT
    COA.AccountNo,
    ABS((ISNULL(COA.OpeningDebit, 0) + SUM(ISNULL(V.Debit, 0))) - (ISNULL(COA.OpeningCredit, 0) + SUM(ISNULL(V.Credit, 0)))) AS TodayBal,
    CASE WHEN ((ISNULL(COA.OpeningDebit, 0) + SUM(ISNULL(V.Debit, 0))) - (ISNULL(COA.OpeningCredit, 0) + SUM(ISNULL(V.Credit, 0)))) < 0 THEN 'Cr' ELSE 'Dr' END AS TodayBalType
FROM ChartOfAccounts COA
LEFT OUTER JOIN AllVouchers V
    ON COA.AccountNo COLLATE SQL_Latin1_General_CP1_CI_AS = V.AccountNo COLLATE SQL_Latin1_General_CP1_CI_AS
WHERE ISNULL(V.VoucherDate, '1900-01-01') <= @TodayDate
  AND COA.IsDetailed = 1
GROUP BY COA.AccountNo, COA.OpeningCredit, COA.OpeningDebit;";

        AddParam(cmd, "@TodayDate", todayCeiling, DbType.DateTime);
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            string accNo = ReadStr(r, 0);
            if (!string.IsNullOrEmpty(accNo))
                map[accNo] = (ReadDec(r, 1), ReadStr(r, 2, "Dr"));
        }
        return map;
    }

    private async Task<string> BuildVoucherUnionSqlAsync(DbConnection conn, string cropSeason)
    {
        var sb = new StringBuilder();
        bool filterCrop = !string.IsNullOrWhiteSpace(cropSeason) && cropSeason != "0";
        string escapedCrop = cropSeason.Replace("'", "''");

        foreach (var viewName in VoucherSourceViews)
        {
            if (!await DbObjectExistsAsync(conn, viewName, "V"))
                continue;

            bool hasCropCol = await ColumnExistsAsync(conn, viewName, "CropSeason");
            if (filterCrop && !hasCropCol)
                continue;

            if (sb.Length > 0)
                sb.AppendLine("UNION ALL");

            string whereClause = filterCrop && hasCropCol
                ? $"WHERE CropSeason = '{escapedCrop}'"
                : string.Empty;

            sb.AppendLine($"SELECT VoucherDate, AccountNo, Debit, Credit FROM {viewName} {whereClause}");
        }

        return sb.ToString();
    }

    private async Task<Dictionary<string, PartyFilterRow>> LoadFilteredPartiesAsync(
        DbConnection conn,
        AccountPayablesRequest request)
    {
        var dict = new Dictionary<string, PartyFilterRow>(StringComparer.OrdinalIgnoreCase);
        if (!await DbObjectExistsAsync(conn, "Parties", "U"))
            return dict;

        var sql = new StringBuilder(
            "SELECT PartyID, ISNULL(City, '') AS City, ISNULL(Phone1, '') AS Phone1, ISNULL(Phone2, '') AS Phone2, ISNULL(Mobile, '') AS Mobile, ISNULL(NameUrdu, N'') AS NameUrdu FROM Parties WHERE 1=1");

        using var cmd = conn.CreateCommand();
        if (request.TownId is > 0)
        {
            sql.Append(" AND ISNULL(SectorID, 0) IN (SELECT SectorID FROM Sectors WHERE TownID = @townId)");
            AddParam(cmd, "@townId", request.TownId.Value, DbType.Int32);
        }

        if (request.SectorId is > 0)
        {
            sql.Append(" AND ISNULL(SectorID, 0) = @sectorId");
            AddParam(cmd, "@sectorId", request.SectorId.Value, DbType.Int32);
        }

        if (request.PartyGroupId is > 0)
        {
            sql.Append(" AND PartyGroupID = @partyGroupId");
            AddParam(cmd, "@partyGroupId", request.PartyGroupId.Value, DbType.Int32);
        }

        if (!string.IsNullOrWhiteSpace(request.CityName))
        {
            sql.Append(" AND City = @city");
            AddParam(cmd, "@city", request.CityName.Trim());
        }

        cmd.CommandText = sql.ToString();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            string id = ReadStr(r, 0);
            if (string.IsNullOrEmpty(id)) continue;

            dict[id] = new PartyFilterRow
            {
                PartyId = id,
                City = ReadStr(r, 1),
                Phone1 = ReadStr(r, 2),
                Phone2 = ReadStr(r, 3),
                Mobile = ReadStr(r, 4),
                NameUrdu = r[5]?.ToString()?.Trim() ?? string.Empty
            };
        }

        return dict;
    }

    private static string FormatContactNo(string mobile, string phone2, string phone1)
    {
        var parts = new List<string>(3);
        foreach (var raw in new[] { mobile, phone2, phone1 })
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            parts.Add(raw.Trim().Replace(" ", "-"));
        }
        return string.Join(", ", parts);
    }

    private static async Task InsertComRecordAsync(DbConnection conn, string fromNo, string toNo, string body, string unitId)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO ComRecords (FromNo, ToNo, Body, SourceType, ComTime, IsSMS, IsProcessed, UnitID)
VALUES (@fromNo, @toNo, @body, 'TERMINAL', GETDATE(), 1, 0, @unitId)";
        AddParam(cmd, "@fromNo", fromNo);
        AddParam(cmd, "@toNo", toNo);
        AddParam(cmd, "@body", body);
        AddParam(cmd, "@unitId", unitId);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<DbConnection> OpenConnectionAsync(DbContext context)
    {
        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();
        return conn;
    }

    private static async Task<bool> DbObjectExistsAsync(DbConnection conn, string name, string type)
    {
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CASE WHEN OBJECT_ID(@name, @type) IS NOT NULL THEN 1 ELSE 0 END";
            AddParam(cmd, "@name", name);
            AddParam(cmd, "@type", type);
            var res = await cmd.ExecuteScalarAsync();
            return res != null && res != DBNull.Value && Convert.ToInt32(res) == 1;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> SpHasParameterAsync(DbConnection conn, string spName, string paramName)
    {
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM sys.parameters WHERE object_id = OBJECT_ID(@sp) AND name = @p";
            AddParam(cmd, "@sp", spName);
            AddParam(cmd, "@p", paramName);
            var res = await cmd.ExecuteScalarAsync();
            return res != null && res != DBNull.Value && Convert.ToInt32(res) > 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> ColumnExistsAsync(DbConnection conn, string objectName, string columnName)
    {
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM sys.columns WHERE object_id = OBJECT_ID(@obj) AND name = @col";
            AddParam(cmd, "@obj", objectName);
            AddParam(cmd, "@col", columnName);
            var res = await cmd.ExecuteScalarAsync();
            return res != null && res != DBNull.Value && Convert.ToInt32(res) > 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<string?> ExecuteScalarStringAsync(DbConnection conn, string sql)
    {
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            var obj = await cmd.ExecuteScalarAsync();
            return obj == null || obj == DBNull.Value ? null : obj.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static void AddParam(DbCommand cmd, string name, object? value, DbType? dbType = null)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        if (dbType.HasValue) p.DbType = dbType.Value;
        p.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(p);
    }

    private static int GetOrdinalSafe(DbDataReader r, string name)
    {
        for (int i = 0; i < r.FieldCount; i++)
        {
            if (r.GetName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    private static string ReadStr(DbDataReader r, int idx, string fallback = "")
        => idx < 0 || idx >= r.FieldCount || r.IsDBNull(idx) ? fallback : (r[idx]?.ToString()?.Trim() ?? fallback);

    private static decimal ReadDec(DbDataReader r, int idx)
        => idx < 0 || idx >= r.FieldCount || r.IsDBNull(idx) ? 0m : Convert.ToDecimal(r[idx]);

    private static int ReadBoolInt(DbDataReader r, int idx)
    {
        if (idx < 0 || idx >= r.FieldCount || r.IsDBNull(idx)) return 0;
        object val = r[idx];
        if (val is bool b) return b ? 1 : 0;
        if (val is byte or short or int or long or decimal) return Convert.ToDecimal(val) != 0m ? 1 : 0;
        string s = val.ToString()?.Trim() ?? string.Empty;
        if (int.TryParse(s, out int num)) return num != 0 ? 1 : 0;
        return s.Equals("true", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
    }

    private sealed class SpBalanceRow
    {
        public string AccountNo { get; init; } = string.Empty;
        public string AccountName { get; init; } = string.Empty;
        public string AccountType { get; init; } = string.Empty;
        public int IsDetailed { get; init; }
        public string PLFlag { get; init; } = string.Empty;
        public int BALFlag { get; init; }
        public decimal OpeningDebit { get; init; }
        public decimal OpeningCredit { get; init; }
        public decimal OpeningBal { get; init; }
        public string OpeningBalType { get; init; } = "Dr";
        public decimal Debit { get; init; }
        public decimal Credit { get; init; }
        public decimal Bal { get; init; }
        public string BalType { get; init; } = "Dr";
        public decimal ClosingDebit { get; init; }
        public decimal ClosingCredit { get; init; }
        public decimal ClosingBal { get; init; }
        public string ClosingBalType { get; init; } = "Dr";
        public decimal TodayBal { get; set; }
        public string TodayBalType { get; set; } = "Dr";
    }

    private sealed class PartyFilterRow
    {
        public string PartyId { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Phone1 { get; init; } = string.Empty;
        public string Phone2 { get; init; } = string.Empty;
        public string Mobile { get; init; } = string.Empty;
        public string NameUrdu { get; init; } = string.Empty;
    }
}
