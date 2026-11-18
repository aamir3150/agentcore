using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class PartyService : IPartyService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public PartyService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Party?> GetPartyByNoAsync(string partyNo)
    {
        if (string.IsNullOrWhiteSpace(partyNo)) return null;
        using var context = await _contextFactory.CreateDbContextAsync();
        var trimmed = partyNo.Trim();

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1
                    p.PartyID, p.PartyName, p.NameUrdu, p.IsCustomer, p.IsVendor, 
                    p.PartyGroupID, pg.GroupName, p.SectorID, s.SectorName, s.TownID, t.TownName,
                    p.Phone1, p.Phone2, p.Mobile, p.Address, p.City, p.EMail, p.ContactPerson, 
                    p.BankAccountNo, p.STNNo, p.NTNNo,
                    ISNULL(c.OpeningDebit, 0) - ISNULL(c.OpeningCredit, 0) AS OpeningBalance
                FROM Parties p
                LEFT JOIN PartyGroups pg ON p.PartyGroupID = pg.PartyGroupID
                LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                LEFT JOIN Towns t ON s.TownID = t.TownID
                LEFT JOIN ChartOfAccounts c ON p.PartyID = c.AccountNo
                WHERE p.PartyID = @pNo";
            var param = cmd.CreateParameter();
            param.ParameterName = "@pNo";
            param.Value = trimmed;
            cmd.Parameters.Add(param);

            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                return MapPartyFromReader(r);
            }
        }
        catch { }

        try
        {
            return await context.Parties.FirstOrDefaultAsync(p => p.PartyNo == trimmed);
        }
        catch { return null; }
    }

    public async Task<string> GetNextPartyNoAsync(PartyType partyType)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        string prefix = partyType == PartyType.Vendor ? "62" : "61";

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                SELECT 
                    ISNULL(
                        (SELECT MAX(V) FROM (
                            SELECT ISNULL(MAX(CAST(SUBSTRING(AccountNo, 3, 10) AS INT)), 0) AS V 
                            FROM ChartOfAccounts 
                            WHERE AccountNo LIKE '{prefix}%' AND ISNUMERIC(SUBSTRING(AccountNo, 3, 10)) = 1
                            UNION ALL
                            SELECT ISNULL(MAX(CAST(SUBSTRING(PartyID, 3, 10) AS INT)), 0) AS V 
                            FROM Parties 
                            WHERE PartyID LIKE '{prefix}%' AND ISNUMERIC(SUBSTRING(PartyID, 3, 10)) = 1
                        ) T), 0) + 1;";
            var res = await cmd.ExecuteScalarAsync();
            int nextSeq = res != null && res != DBNull.Value ? Convert.ToInt32(res) : 1;
            return $"{prefix}{nextSeq:D4}";
        }
        catch
        {
            return $"{prefix}0001";
        }
    }

    public async Task<IEnumerable<Party>> GetAllPartiesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<Party>();

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    p.PartyID, p.PartyName, p.NameUrdu, p.IsCustomer, p.IsVendor, 
                    p.PartyGroupID, pg.GroupName, p.SectorID, s.SectorName, s.TownID, t.TownName,
                    p.Phone1, p.Phone2, p.Mobile, p.Address, p.City, p.EMail, p.ContactPerson, 
                    p.BankAccountNo, p.STNNo, p.NTNNo,
                    ISNULL(c.OpeningDebit, 0) - ISNULL(c.OpeningCredit, 0) AS OpeningBalance
                FROM Parties p
                LEFT JOIN PartyGroups pg ON p.PartyGroupID = pg.PartyGroupID
                LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                LEFT JOIN Towns t ON s.TownID = t.TownID
                LEFT JOIN ChartOfAccounts c ON p.PartyID = c.AccountNo
                ORDER BY p.PartyName";
            using var r = await cmd.ExecuteReaderAsync();
            int synthId = 1;
            while (await r.ReadAsync())
            {
                list.Add(MapPartyFromReader(r, synthId++));
            }

            if (list.Count > 0) return list;
        }
        catch { }

        try
        {
            return await context.Parties
                .Include(p => p.Account)
                .Include(p => p.PartyGroup)
                .Include(p => p.Town)
                .Include(p => p.Sector)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }
        catch
        {
            return list;
        }
    }

    public async Task<IEnumerable<PartyGroup>> GetPartyGroupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<PartyGroup>();

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PartyGroupID, GroupName FROM PartyGroups ORDER BY GroupName";
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new PartyGroup
                {
                    Id = r[0] != DBNull.Value ? Convert.ToInt32(r[0]) : 0,
                    Name = r[1]?.ToString()?.Trim() ?? string.Empty
                });
            }

            if (list.Count > 0) return list;
        }
        catch { }

        if (list.Count == 0)
        {
            list.Add(new PartyGroup { Id = 1, Name = "General Customers" });
            list.Add(new PartyGroup { Id = 2, Name = "Zamindaraan (Farmers)" });
            list.Add(new PartyGroup { Id = 3, Name = "Karobari Parties (Traders)" });
        }

        return list;
    }

    public async Task AddPartyAsync(Party party)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        if (string.IsNullOrWhiteSpace(party.PartyNo) || party.PartyNo.StartsWith("P-", StringComparison.OrdinalIgnoreCase))
        {
            party.PartyNo = await GetNextPartyNoAsync(party.PartyType);
        }

        string pId = party.PartyNo.Trim();
        string pName = party.Name.Trim();
        string? nameUrdu = party.UrduName ?? party.Name;
        bool isCust = party.PartyType == PartyType.Customer || party.PartyType == PartyType.Both;
        bool isVend = party.PartyType == PartyType.Vendor || party.PartyType == PartyType.Both;
        int? grpId = party.PartyGroupId > 0 ? party.PartyGroupId : (int?)null;
        short? secId = party.SectorId.HasValue && party.SectorId.Value > 0 ? (short)party.SectorId.Value : (short?)null;
        string? phone = party.Phone;
        string? mobile = party.Phone;
        string? addr = party.Address;
        string? cnic = party.CNIC;
        decimal opDebit = 0m;
        decimal opCredit = 0m;
        if (party.Account != null)
        {
            opDebit = party.Account.OpeningBalance > 0 ? party.Account.OpeningBalance : 0m;
            opCredit = party.Account.OpeningBalance < 0 ? Math.Abs(party.Account.OpeningBalance) : 0m;
        }

        string parentAcc = isVend ? "62" : "61";

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT 1 FROM Parties WHERE PartyID = @pId)
                BEGIN
                    INSERT INTO Parties (
                        PartyID, PartyName, NameUrdu, SectorID, Address, City, 
                        Phone1, Mobile, BankAccountNo, IsCustomer, IsVendor, PartyGroupID, STNNo, NTNNo
                    ) VALUES (
                        @pId, @pName, @nameUrdu, @secId, @addr, '', 
                        @phone, @mobile, '', @isCust, @isVend, @grpId, @cnic, ''
                    );
                END
                ELSE
                BEGIN
                    UPDATE Parties SET 
                        PartyName = @pName, NameUrdu = @nameUrdu, SectorID = @secId, 
                        Address = @addr, Phone1 = @phone, Mobile = @mobile, 
                        IsCustomer = @isCust, IsVendor = @isVend, PartyGroupID = @grpId, 
                        STNNo = @cnic
                    WHERE PartyID = @pId;
                END

                IF OBJECT_ID('ChartOfAccounts', 'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM ChartOfAccounts WHERE AccountNo = @pId)
                    BEGIN
                        INSERT INTO ChartOfAccounts (
                            AccountNo, UserID, AccountName, AccountType, AccountDepth, 
                            Narration, ParentAccountNo, OpeningDebit, OpeningCredit, 
                            AdjustedDebit, AdjustedCredit, IsDetailed, IsLocked, IsPosted, 
                            IsEditable, BalFlag, PLFlag, ExpFlag, AccountNameUrdu
                        ) VALUES (
                            @pId, 1, @pName, 'D', 1, 
                            'Party Account', @parentAcc, @opDebit, @opCredit, 
                            @opDebit, @opCredit, 1, 0, 1, 
                            1, 1, 'B', 0, @nameUrdu
                        );
                    END
                    ELSE
                    BEGIN
                        UPDATE ChartOfAccounts SET 
                            AccountName = @pName, 
                            AccountNameUrdu = @nameUrdu,
                            OpeningDebit = @opDebit,
                            OpeningCredit = @opCredit,
                            AdjustedDebit = @opDebit,
                            AdjustedCredit = @opCredit
                        WHERE AccountNo = @pId;
                    END
                END";

            var p1 = cmd.CreateParameter(); p1.ParameterName = "@pId"; p1.Value = pId; cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@pName"; p2.Value = pName; cmd.Parameters.Add(p2);
            var p3 = cmd.CreateParameter(); p3.ParameterName = "@nameUrdu"; p3.Value = (object?)nameUrdu ?? DBNull.Value; cmd.Parameters.Add(p3);
            var p4 = cmd.CreateParameter(); p4.ParameterName = "@secId"; p4.Value = (object?)secId ?? DBNull.Value; cmd.Parameters.Add(p4);
            var p5 = cmd.CreateParameter(); p5.ParameterName = "@addr"; p5.Value = (object?)addr ?? DBNull.Value; cmd.Parameters.Add(p5);
            var p6 = cmd.CreateParameter(); p6.ParameterName = "@phone"; p6.Value = (object?)phone ?? DBNull.Value; cmd.Parameters.Add(p6);
            var p7 = cmd.CreateParameter(); p7.ParameterName = "@mobile"; p7.Value = (object?)mobile ?? DBNull.Value; cmd.Parameters.Add(p7);
            var p8 = cmd.CreateParameter(); p8.ParameterName = "@isCust"; p8.Value = isCust; cmd.Parameters.Add(p8);
            var p9 = cmd.CreateParameter(); p9.ParameterName = "@isVend"; p9.Value = isVend; cmd.Parameters.Add(p9);
            var p10 = cmd.CreateParameter(); p10.ParameterName = "@grpId"; p10.Value = (object?)grpId ?? DBNull.Value; cmd.Parameters.Add(p10);
            var p11 = cmd.CreateParameter(); p11.ParameterName = "@cnic"; p11.Value = (object?)cnic ?? DBNull.Value; cmd.Parameters.Add(p11);
            var p12 = cmd.CreateParameter(); p12.ParameterName = "@parentAcc"; p12.Value = parentAcc; cmd.Parameters.Add(p12);
            var p13 = cmd.CreateParameter(); p13.ParameterName = "@opDebit"; p13.Value = opDebit; cmd.Parameters.Add(p13);
            var p14 = cmd.CreateParameter(); p14.ParameterName = "@opCredit"; p14.Value = opCredit; cmd.Parameters.Add(p14);

            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task UpdatePartyAsync(Party party)
    {
        await AddPartyAsync(party);
    }

    public async Task DeletePartyAsync(int partyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM Parties WHERE PartyID = CAST(@id AS VARCHAR(10));
                DELETE FROM ChartOfAccounts 
                WHERE AccountNo = CAST(@id AS VARCHAR(10))
                  AND NOT EXISTS (SELECT 1 FROM CreditVouchersBody WHERE AccountNo = ChartOfAccounts.AccountNo)
                  AND NOT EXISTS (SELECT 1 FROM DebitVouchersBody WHERE AccountNo = ChartOfAccounts.AccountNo)
                  AND NOT EXISTS (SELECT 1 FROM JournalVouchersBody WHERE AccountNo = ChartOfAccounts.AccountNo);";
            var p = cmd.CreateParameter();
            p.ParameterName = "@id";
            p.Value = partyId;
            cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task AddPartyGroupAsync(PartyGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO PartyGroups (PartyGroupID, GroupName)
                VALUES ((SELECT ISNULL(MAX(PartyGroupID), 0) + 1 FROM PartyGroups), @name);";
            var p = cmd.CreateParameter();
            p.ParameterName = "@name";
            p.Value = group.Name;
            cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task UpdatePartyGroupAsync(PartyGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE PartyGroups SET GroupName = @name WHERE PartyGroupID = @id";
            var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = group.Name; cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@id"; p2.Value = group.Id; cmd.Parameters.Add(p2);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task DeletePartyGroupAsync(int groupId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM PartyGroups WHERE PartyGroupID = @id";
            var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = groupId; cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task<IEnumerable<Salesman>> GetAllSalesmenAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<Salesman>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SalesManId, ISNULL(Address,''), ISNULL(City,''), ISNULL(Mobile,''), ISNULL(Designation,'') FROM Salesmen";
            using var r = await cmd.ExecuteReaderAsync();
            int synthId = 1;
            while (await r.ReadAsync())
            {
                string sId = r[0]?.ToString()?.Trim() ?? string.Empty;
                list.Add(new Salesman
                {
                    Id = int.TryParse(sId, out int num) ? num : synthId++,
                    Name = sId,
                    Phone = r[3]?.ToString()?.Trim()
                });
            }
        }
        catch { }

        return list;
    }

    public async Task AddSalesmanAsync(Salesman salesman)
    {
    }

    public async Task UpdateSalesmanAsync(Salesman salesman)
    {
    }

    public async Task DeleteSalesmanAsync(int salesmanId)
    {
    }

    private static Party MapPartyFromReader(IDataRecord r, int fallbackId = 1)
    {
        string pId = r[0]?.ToString()?.Trim() ?? string.Empty;
        string pName = r[1]?.ToString()?.Trim() ?? string.Empty;
        string? urdu = r[2]?.ToString()?.Trim();
        bool isCust = r[3] != DBNull.Value && Convert.ToBoolean(r[3]);
        bool isVend = r[4] != DBNull.Value && Convert.ToBoolean(r[4]);
        int grpId = r[5] != DBNull.Value ? Convert.ToInt32(r[5]) : 0;
        string grpName = r[6]?.ToString()?.Trim() ?? string.Empty;
        int secId = r[7] != DBNull.Value ? Convert.ToInt32(r[7]) : 0;
        string secName = r[8]?.ToString()?.Trim() ?? string.Empty;
        int tId = r[9] != DBNull.Value ? Convert.ToInt32(r[9]) : 0;
        string tName = r[10]?.ToString()?.Trim() ?? string.Empty;
        string? phone = !string.IsNullOrWhiteSpace(r[13]?.ToString()) ? r[13]?.ToString()?.Trim() : r[11]?.ToString()?.Trim();
        string? addr = r[14]?.ToString()?.Trim();
        string? city = r[15]?.ToString()?.Trim();
        string? cnic = r[19]?.ToString()?.Trim();
        decimal opBal = r[21] != DBNull.Value ? Convert.ToDecimal(r[21]) : 0m;

        int pIdNum = int.TryParse(pId, out int num) && num > 0 ? num : fallbackId;

        PartyType pType = isCust ? (isVend ? PartyType.Both : PartyType.Customer) : PartyType.Vendor;

        return new Party
        {
            Id = pIdNum,
            PartyNo = pId,
            Name = pName,
            UrduName = urdu,
            PartyType = pType,
            PartyGroupId = grpId,
            PartyGroup = new PartyGroup { Id = grpId, Name = grpName },
            SectorId = secId > 0 ? secId : null,
            Sector = secId > 0 ? new Sector { Id = secId, Name = secName, TownId = tId } : null,
            TownId = tId > 0 ? tId : null,
            Town = tId > 0 ? new Town { Id = tId, Name = tName } : null,
            Phone = phone,
            Address = addr,
            CNIC = cnic,
            IsActive = true,
            Account = new Account
            {
                Id = pIdNum,
                AccountNo = pId,
                Name = pName,
                UrduName = urdu,
                OpeningBalance = opBal,
                CurrentBalance = opBal,
                IsActive = true
            }
        };
    }
}
