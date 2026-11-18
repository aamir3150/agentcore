using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public CatalogService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // --- Companies ---
    public async Task<IEnumerable<Company>> GetCompaniesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<Company>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
            using var r = await cmd.ExecuteReaderAsync();
            int synthId = 1;
            while (await r.ReadAsync())
            {
                string cId = r[0]?.ToString()?.Trim() ?? string.Empty;
                list.Add(new Company
                {
                    Id = int.TryParse(cId, out int n) ? n : synthId++,
                    Name = r[1]?.ToString()?.Trim() ?? string.Empty,
                    ShortName = cId,
                    IsActive = true
                });
            }
            if (list.Count > 0) return list;
        }
        catch { }

        try
        {
            return await context.Companies.Where(c => !c.IsDeleted).ToListAsync();
        }
        catch { return list; }
    }

    public async Task AddCompanyAsync(Company company)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DECLARE @cId VARCHAR(10) = CAST((SELECT ISNULL(MAX(CAST(CompanyID AS INT)), 0) + 1 FROM Companies WHERE ISNUMERIC(CompanyID)=1) AS VARCHAR(10));
                IF @cId IS NULL OR @cId = '0' SET @cId = '1';
                INSERT INTO Companies (CompanyID, CompanyName) VALUES (@cId, @name);";
            var p = cmd.CreateParameter(); p.ParameterName = "@name"; p.Value = company.Name.Trim(); cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Companies SET CompanyName = @name WHERE CompanyID = CAST(@id AS VARCHAR(10))";
            var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = company.Name.Trim(); cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@id"; p2.Value = company.Id; cmd.Parameters.Add(p2);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task DeleteCompanyAsync(int companyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Companies WHERE CompanyID = CAST(@id AS VARCHAR(10))";
            var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = companyId; cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    // --- Units ---
    public async Task<IEnumerable<Unit>> GetUnitsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<Unit>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UnitID, UnitName, ISNULL(Multiplier, 1) FROM Units ORDER BY UnitID";
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new Unit
                {
                    Id = r[0] != DBNull.Value ? Convert.ToInt32(r[0]) : 0,
                    Name = r[1]?.ToString()?.Trim() ?? string.Empty,
                    Multiplier = r[2] != DBNull.Value ? Convert.ToDecimal(r[2]) : 1.0m
                });
            }
            if (list.Count > 0) return list;
        }
        catch { }

        try
        {
            return await context.Units.Where(u => !u.IsDeleted).ToListAsync();
        }
        catch { return list; }
    }

    public async Task AddUnitAsync(Unit unit)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Units (UnitID, UnitName, Multiplier)
                VALUES ((SELECT ISNULL(MAX(UnitID), 0) + 1 FROM Units), @name, @mult);";
            var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = unit.Name.Trim(); cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@mult"; p2.Value = unit.Multiplier ?? 1.0m; cmd.Parameters.Add(p2);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task UpdateUnitAsync(Unit unit)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET UnitName = @name, Multiplier = @mult WHERE UnitID = @id";
            var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = unit.Name.Trim(); cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@mult"; p2.Value = unit.Multiplier ?? 1.0m; cmd.Parameters.Add(p2);
            var p3 = cmd.CreateParameter(); p3.ParameterName = "@id"; p3.Value = unit.Id; cmd.Parameters.Add(p3);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task DeleteUnitAsync(int unitId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Units WHERE UnitID = @id";
            var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = unitId; cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    // --- Towns ---
    public async Task<IEnumerable<Town>> GetTownsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<Town>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new Town
                {
                    Id = r[0] != DBNull.Value ? Convert.ToInt32(r[0]) : 0,
                    Name = r[1]?.ToString()?.Trim() ?? string.Empty
                });
            }
            if (list.Count > 0) return list;
        }
        catch { }

        try
        {
            return await context.Towns.Where(t => !t.IsDeleted).ToListAsync();
        }
        catch { return list; }
    }

    public async Task AddTownAsync(Town town)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Towns (TownID, TownName)
                VALUES ((SELECT ISNULL(MAX(TownID), 0) + 1 FROM Towns), @name);";
            var p = cmd.CreateParameter(); p.ParameterName = "@name"; p.Value = town.Name.Trim(); cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task UpdateTownAsync(Town town)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Towns SET TownName = @name WHERE TownID = @id";
            var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = town.Name.Trim(); cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@id"; p2.Value = town.Id; cmd.Parameters.Add(p2);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task DeleteTownAsync(int townId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Towns WHERE TownID = @id";
            var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = townId; cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    // --- Sectors ---
    public async Task<IEnumerable<Sector>> GetSectorsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<Sector>();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT s.SectorID, s.SectorName, s.TownID, ISNULL(t.TownName, '')
                FROM Sectors s
                LEFT JOIN Towns t ON s.TownID = t.TownID
                ORDER BY s.SectorName";
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                int sId = r[0] != DBNull.Value ? Convert.ToInt32(r[0]) : 0;
                string sName = r[1]?.ToString()?.Trim() ?? string.Empty;
                int tId = r[2] != DBNull.Value ? Convert.ToInt32(r[2]) : 0;
                string tName = r[3]?.ToString()?.Trim() ?? string.Empty;
                list.Add(new Sector
                {
                    Id = sId,
                    Name = sName,
                    TownId = tId,
                    Town = tId > 0 ? new Town { Id = tId, Name = tName } : null!
                });
            }
            if (list.Count > 0) return list;
        }
        catch { }

        try
        {
            return await context.Sectors.Include(s => s.Town).Where(s => !s.IsDeleted).ToListAsync();
        }
        catch { return list; }
    }

    public async Task AddSectorAsync(Sector sector)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Sectors (SectorID, TownID, SectorName)
                VALUES ((SELECT ISNULL(MAX(SectorID), 0) + 1 FROM Sectors), @tId, @name);";
            var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = sector.Name.Trim(); cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@tId"; p2.Value = sector.TownId > 0 ? (object)sector.TownId : DBNull.Value; cmd.Parameters.Add(p2);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task UpdateSectorAsync(Sector sector)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Sectors SET SectorName = @name, TownID = @tId WHERE SectorID = @id";
            var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = sector.Name.Trim(); cmd.Parameters.Add(p1);
            var p2 = cmd.CreateParameter(); p2.ParameterName = "@tId"; p2.Value = sector.TownId > 0 ? (object)sector.TownId : DBNull.Value; cmd.Parameters.Add(p2);
            var p3 = cmd.CreateParameter(); p3.ParameterName = "@id"; p3.Value = sector.Id; cmd.Parameters.Add(p3);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }

    public async Task DeleteSectorAsync(int sectorId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Sectors WHERE SectorID = @id";
            var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = sectorId; cmd.Parameters.Add(p);
            await cmd.ExecuteNonQueryAsync();
        }
        catch { }
    }
}
