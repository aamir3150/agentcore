using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class PromiseService : IPromiseService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public PromiseService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Promise>> GetAllPromisesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        var list = new List<Promise>();
        var sql = @"
            SELECT p.Id, p.PromiseNo, p.PartyId, p.PartyNo, ISNULL(pt.Name, '') AS PartyName, 
                   p.CurrentDate, p.PromiseDate, p.Amount, ISNULL(p.Narration, ISNULL(p.Remarks, '')) AS Narration, 
                   p.IsPayment, ISNULL(p.IsFullFilled, ISNULL(p.IsFulfilled, 0)) AS IsFulfilled
            FROM Promises p
            LEFT JOIN Parties pt ON p.PartyId = pt.Id OR p.PartyNo = pt.PartyNo
            WHERE p.IsDeleted = 0
            ORDER BY p.PromiseDate DESC";

        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var id = reader.GetInt32(0);
            var promiseNo = Convert.ToInt32(reader.GetDecimal(1));
            var partyId = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
            var partyNo = reader.IsDBNull(3) ? "" : reader.GetString(3);
            var partyName = reader.GetString(4);
            var currentDate = reader.GetDateTime(5);
            var promiseDate = reader.GetDateTime(6);
            var amount = reader.GetDecimal(7);
            var narration = reader.IsDBNull(8) ? "" : reader.GetString(8);
            var isPayment = reader.GetBoolean(9);
            var isFulfilled = reader.GetBoolean(10);

            list.Add(new Promise
            {
                Id = id,
                PromiseNo = promiseNo,
                PartyId = partyId,
                Party = new Party { Id = partyId, PartyNo = partyNo, Name = partyName },
                PromiseDate = currentDate,
                DueDate = promiseDate,
                Amount = amount,
                Reason = narration,
                IsPayment = isPayment,
                Status = isFulfilled ? PromiseStatus.Fulfilled : (promiseDate < DateTime.Today ? PromiseStatus.Overdue : PromiseStatus.Pending)
            });
        }

        return list;
    }

    public async Task<Promise?> GetPromiseByIdAsync(int id)
    {
        var all = await GetAllPromisesAsync();
        return all.FirstOrDefault(p => p.Id == id || p.PromiseNo == id);
    }

    public async Task AddPromiseAsync(Promise promise)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        // 1. Get next PromiseNo
        int nextNo = 1;
        using (var cmdMax = conn.CreateCommand())
        {
            cmdMax.CommandText = "SELECT ISNULL(MAX(PromiseNo), 0) + 1 FROM Promises";
            var res = await cmdMax.ExecuteScalarAsync();
            if (res != null && res != DBNull.Value)
                nextNo = Convert.ToInt32(res);
        }

        // 2. Resolve party details
        string partyNo = promise.Party?.PartyNo ?? "";
        if (string.IsNullOrEmpty(partyNo) && promise.PartyId > 0)
        {
            var party = await context.Parties.FindAsync(promise.PartyId);
            if (party != null) partyNo = party.PartyNo;
        }

        bool isFulfilled = promise.Status == PromiseStatus.Fulfilled;

        var sql = @"
            INSERT INTO Promises (
                PromiseNo, IsPayment, CurrentDate, PromiseDate, Narration, Amount,
                PartyNo, UserNo, IsFullFilled, PartyId, Remarks, IsFulfilled,
                CreatedBy, CreatedAt, IsDeleted
            )
            VALUES (
                @PromiseNo, @IsPayment, @CurrentDate, @PromiseDate, @Narration, @Amount,
                @PartyNo, 1, @IsFullFilled, @PartyId, @Remarks, @IsFulfilled,
                1, GETDATE(), 0
            )";

        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqlParameter("@PromiseNo", (decimal)nextNo));
        cmd.Parameters.Add(new SqlParameter("@IsPayment", promise.IsPayment));
        cmd.Parameters.Add(new SqlParameter("@CurrentDate", promise.PromiseDate));
        cmd.Parameters.Add(new SqlParameter("@PromiseDate", promise.DueDate));
        cmd.Parameters.Add(new SqlParameter("@Narration", (object?)promise.Reason ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Amount", promise.Amount));
        cmd.Parameters.Add(new SqlParameter("@PartyNo", partyNo));
        cmd.Parameters.Add(new SqlParameter("@IsFullFilled", isFulfilled));
        cmd.Parameters.Add(new SqlParameter("@PartyId", promise.PartyId > 0 ? (object)promise.PartyId : DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Remarks", (object?)promise.Reason ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@IsFulfilled", isFulfilled));

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdatePromiseAsync(Promise promise)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        string partyNo = promise.Party?.PartyNo ?? "";
        if (string.IsNullOrEmpty(partyNo) && promise.PartyId > 0)
        {
            var party = await context.Parties.FindAsync(promise.PartyId);
            if (party != null) partyNo = party.PartyNo;
        }

        bool isFulfilled = promise.Status == PromiseStatus.Fulfilled;

        var sql = @"
            UPDATE Promises 
            SET IsPayment = @IsPayment,
                PromiseDate = @PromiseDate,
                Narration = @Narration,
                Amount = @Amount,
                PartyNo = @PartyNo,
                IsFullFilled = @IsFullFilled,
                PartyId = @PartyId,
                Remarks = @Remarks,
                IsFulfilled = @IsFulfilled,
                UpdatedAt = GETDATE()
            WHERE Id = @Id OR PromiseNo = @PromiseNo";

        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqlParameter("@Id", promise.Id));
        cmd.Parameters.Add(new SqlParameter("@PromiseNo", (decimal)promise.PromiseNo));
        cmd.Parameters.Add(new SqlParameter("@IsPayment", promise.IsPayment));
        cmd.Parameters.Add(new SqlParameter("@PromiseDate", promise.DueDate));
        cmd.Parameters.Add(new SqlParameter("@Narration", (object?)promise.Reason ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Amount", promise.Amount));
        cmd.Parameters.Add(new SqlParameter("@PartyNo", partyNo));
        cmd.Parameters.Add(new SqlParameter("@IsFullFilled", isFulfilled));
        cmd.Parameters.Add(new SqlParameter("@PartyId", promise.PartyId > 0 ? (object)promise.PartyId : DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Remarks", (object?)promise.Reason ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@IsFulfilled", isFulfilled));

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeletePromiseAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        var sql = "UPDATE Promises SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id OR PromiseNo = @Id";
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqlParameter("@Id", id));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task ToggleFulfilledAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        var sql = @"
            UPDATE Promises 
            SET IsFullFilled = CASE WHEN ISNULL(IsFullFilled, 0) = 1 THEN 0 ELSE 1 END,
                IsFulfilled = CASE WHEN ISNULL(IsFulfilled, 0) = 1 THEN 0 ELSE 1 END,
                UpdatedAt = GETDATE()
            WHERE Id = @Id OR PromiseNo = @Id";

        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqlParameter("@Id", id));
        await cmd.ExecuteNonQueryAsync();
    }
}
