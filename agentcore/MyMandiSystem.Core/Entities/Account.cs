using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyMandiSystem.Core.Entities;

public class Account : AuditBase
{
    public int Id { get; set; }
    public string AccountNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? UrduName { get; set; }
    public AccountType AccountType { get; set; }
    public int AccountGroupId { get; set; }
    public virtual AccountGroup AccountGroup { get; set; } = null!;
    public int? ParentAccountId { get; set; }
    public virtual Account? ParentAccount { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; } = true;

    // Report Settings Flags (AgenSoft 100% Exact Parity)
    public string? PLFlag { get; set; } // 'B' = Both, 'P' = Profit & Loss, null/empty = Excluded
    public bool BalFlag { get; set; } = true; // Include in Balance Sheet
    public bool ExpFlag { get; set; } // Include in Expense Report
    public decimal? ExpenseLimit { get; set; } // Monthly/Annual Budget Limit
    
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Account> SubAccounts { get; set; } = new List<Account>();
    public virtual ICollection<GeneralLedger> LedgerEntries { get; set; } = new List<GeneralLedger>();
}
