using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Printing;
using MyMandiSystem.Core.PrintModels;

namespace MyMandiSystem.Infrastructure.Printing;

/// <summary>
/// Implements IPrintModelFactory to convert database entities into disconnected
/// snapshot print models. Keeps the print engine completely free of EF Core references.
/// </summary>
public sealed class PrintModelFactory : IPrintModelFactory
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public PrintModelFactory(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public CreditVoucherSlip BuildCreditSlip(int voucherId)
    {
        using var db = _contextFactory.CreateDbContext();
        var voucher = db.Vouchers
            .AsNoTracking()
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .FirstOrDefault(v => v.Id == voucherId)
            ?? throw new InvalidOperationException($"Voucher with ID {voucherId} not found.");

        // Find associated party through account
        var accountIds = voucher.Details.Select(d => d.AccountId).Distinct().ToList();
        var party = db.Parties
            .AsNoTracking()
            .FirstOrDefault(p => accountIds.Contains(p.AccountId));

        return BuildCreditSlip(voucher, party);
    }

    public CreditVoucherSlip BuildCreditSlipFromInvoice(int invoiceId)
    {
        using var db = _contextFactory.CreateDbContext();
        var invoice = db.Invoices
            .AsNoTracking()
            .Include(i => i.Party)
            .Include(i => i.Details)
                .ThenInclude(d => d.Product)
                    .ThenInclude(p => p.Unit)
            .FirstOrDefault(i => i.Id == invoiceId)
            ?? throw new InvalidOperationException($"Invoice with ID {invoiceId} not found.");

        var lines = invoice.Details.Select(d => new SlipLine(
            Product: d.Product?.Name ?? "Item",
            Qty: d.Quantity,
            Unit: d.Product?.Unit?.Name ?? "Bag",
            Rate: d.Rate,
            Amount: d.Amount
        )).ToList();

        return new CreditVoucherSlip
        {
            VoucherNo = invoice.InvoiceNo,
            Date = invoice.InvoiceDate,
            PartyName = invoice.Party?.Name ?? "Cash Customer",
            PartyUrduName = invoice.Party?.UrduName ?? string.Empty,
            PartyPhone = invoice.Party?.Phone ?? string.Empty,
            PartyAddress = invoice.Party?.Address ?? string.Empty,
            Items = lines,
            TotalAmount = invoice.NetAmount > 0 ? invoice.NetAmount : invoice.SubTotal,
            Remarks = $"Invoice #{invoice.InvoiceNo}",
            RemarksUrdu = string.Empty
        };
    }

    public CreditVoucherSlip BuildCreditSlip(Voucher voucher, Party? party = null, IEnumerable<SlipLine>? items = null)
    {
        var linesList = items?.ToList() ?? voucher.Details.Select(d => new SlipLine(
            Product: d.Narration ?? d.Account?.Name ?? "Item",
            Qty: 1,
            Unit: "Qty",
            Rate: d.Debit > 0 ? d.Debit : d.Credit,
            Amount: d.Debit > 0 ? d.Debit : d.Credit
        )).ToList();

        decimal total = voucher.TotalDebit > 0 ? voucher.TotalDebit : linesList.Sum(l => l.Amount);

        return new CreditVoucherSlip
        {
            VoucherNo = voucher.VoucherNo,
            Date = voucher.VoucherDate,
            PartyName = party?.Name ?? voucher.Details.FirstOrDefault()?.Account?.Name ?? "Party",
            PartyUrduName = party?.UrduName ?? voucher.Details.FirstOrDefault()?.Account?.UrduName ?? string.Empty,
            PartyPhone = party?.Phone ?? string.Empty,
            PartyAddress = party?.Address ?? string.Empty,
            Items = linesList,
            TotalAmount = total,
            Remarks = voucher.NarrationEnglish ?? string.Empty,
            RemarksUrdu = voucher.NarrationUrdu ?? string.Empty
        };
    }

    public CashLoanSlip BuildCashLoanSlip(int voucherId)
    {
        using var db = _contextFactory.CreateDbContext();
        var voucher = db.Vouchers
            .AsNoTracking()
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .FirstOrDefault(v => v.Id == voucherId)
            ?? throw new InvalidOperationException($"Voucher with ID {voucherId} not found.");

        var accountIds = voucher.Details.Select(d => d.AccountId).Distinct().ToList();
        var party = db.Parties
            .AsNoTracking()
            .FirstOrDefault(p => accountIds.Contains(p.AccountId));

        return BuildCashLoanSlip(voucher, party);
    }

    public CashLoanSlip BuildCashLoanSlip(Voucher voucher, Party? party = null)
    {
        decimal amount = voucher.TotalDebit > 0 ? voucher.TotalDebit : voucher.TotalCredit;

        return new CashLoanSlip
        {
            VoucherNo = voucher.VoucherNo,
            Date = voucher.VoucherDate,
            PartyName = party?.Name ?? voucher.Details.FirstOrDefault()?.Account?.Name ?? "Party",
            PartyUrduName = party?.UrduName ?? voucher.Details.FirstOrDefault()?.Account?.UrduName ?? string.Empty,
            PartyPhone = party?.Phone ?? string.Empty,
            PartyAddress = party?.Address ?? string.Empty,
            Amount = amount,
            Narration = voucher.NarrationEnglish ?? "Cash Udhaar (Advance Loan)",
            NarrationUrdu = voucher.NarrationUrdu ?? "نقد ادھار"
        };
    }

    public VoucherSlip BuildVoucherSlip(int voucherId)
    {
        using var db = _contextFactory.CreateDbContext();
        Voucher? voucher = null;
        try
        {
            voucher = db.Vouchers
                .AsNoTracking()
                .Include(v => v.Details)
                    .ThenInclude(d => d.Account)
                .FirstOrDefault(v => v.Id == voucherId);
        }
        catch { }

        if (voucher == null || !voucher.Details.Any())
        {
            // Fallback to native AgenSoft database tables
            try
            {
                var conn = db.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                // 1. Check DebitVouchers (Cash Payment)
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        IF OBJECT_ID('DebitVouchers', 'U') IS NOT NULL
                            SELECT TOP 1 VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, CropSeason 
                            FROM DebitVouchers WHERE VoucherNo = @id;";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@id";
                    p.Value = voucherId;
                    cmd.Parameters.Add(p);

                    using var r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        voucher = new Voucher
                        {
                            Id = voucherId,
                            VoucherNo = voucherId.ToString(),
                            VoucherType = Core.Enums.VoucherType.CashPayment,
                            VoucherDate = r.GetDateTime(1),
                            NarrationUrdu = r.IsDBNull(4) ? null : r.GetString(4),
                            CropSeason = r.IsDBNull(5) ? null : new CropSeason { Name = r.GetString(5) }
                        };
                    }
                }

                if (voucher != null && voucher.VoucherType == Core.Enums.VoucherType.CashPayment)
                {
                    using var cmdBody = conn.CreateCommand();
                    cmdBody.CommandText = @"
                        SELECT b.AccountNo, b.Narration, b.Debit, ISNULL(coa.AccountName, b.AccountNo), ISNULL(coa.AccountNameUrdu, '')
                        FROM DebitVouchersBody b
                        LEFT JOIN ChartOfAccounts coa ON b.AccountNo = coa.AccountNo
                        WHERE b.VoucherNo = @id
                        ORDER BY b.SerialNo;";
                    var p = cmdBody.CreateParameter();
                    p.ParameterName = "@id";
                    p.Value = voucherId;
                    cmdBody.Parameters.Add(p);

                    using var rBody = cmdBody.ExecuteReader();
                    decimal total = 0;
                    while (rBody.Read())
                    {
                        var accNo = rBody.GetString(0);
                        var narr = rBody.IsDBNull(1) ? string.Empty : rBody.GetString(1);
                        var debit = rBody.IsDBNull(2) ? 0m : rBody.GetDecimal(2);
                        var accName = rBody.IsDBNull(3) ? accNo : rBody.GetString(3);
                        var accUrdu = rBody.IsDBNull(4) ? string.Empty : rBody.GetString(4);

                        total += debit;
                        voucher.Details.Add(new VoucherDetail
                        {
                            AccountNo = accNo,
                            Narration = narr,
                            Debit = debit,
                            Credit = 0,
                            Account = new Account
                            {
                                AccountNo = accNo,
                                Name = accName,
                                UrduName = accUrdu
                            }
                        });
                    }
                    voucher.TotalDebit = total;
                    voucher.TotalCredit = total;
                }

                // 2. Check CreditVouchers (Cash Receiving)
                if (voucher == null)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                        IF OBJECT_ID('CreditVouchers', 'U') IS NOT NULL
                            SELECT TOP 1 VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, CropSeason 
                            FROM CreditVouchers WHERE VoucherNo = @id;";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@id";
                    p.Value = voucherId;
                    cmd.Parameters.Add(p);

                    using var r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        voucher = new Voucher
                        {
                            Id = voucherId,
                            VoucherNo = voucherId.ToString(),
                            VoucherType = Core.Enums.VoucherType.CashReceiving,
                            VoucherDate = r.GetDateTime(1),
                            NarrationUrdu = r.IsDBNull(4) ? null : r.GetString(4),
                            CropSeason = r.IsDBNull(5) ? null : new CropSeason { Name = r.GetString(5) }
                        };
                    }

                    if (voucher != null)
                    {
                        using var cmdBody = conn.CreateCommand();
                        cmdBody.CommandText = @"
                            SELECT b.AccountNo, b.Narration, b.Credit, ISNULL(coa.AccountName, b.AccountNo), ISNULL(coa.AccountNameUrdu, '')
                            FROM CreditVouchersBody b
                            LEFT JOIN ChartOfAccounts coa ON b.AccountNo = coa.AccountNo
                            WHERE b.VoucherNo = @id
                            ORDER BY b.SerialNo;";
                        var pBody = cmdBody.CreateParameter();
                        pBody.ParameterName = "@id";
                        pBody.Value = voucherId;
                        cmdBody.Parameters.Add(pBody);

                        using var rBody = cmdBody.ExecuteReader();
                        decimal total = 0;
                        while (rBody.Read())
                        {
                            var accNo = rBody.GetString(0);
                            var narr = rBody.IsDBNull(1) ? string.Empty : rBody.GetString(1);
                            var credit = rBody.IsDBNull(2) ? 0m : rBody.GetDecimal(2);
                            var accName = rBody.IsDBNull(3) ? accNo : rBody.GetString(3);
                            var accUrdu = rBody.IsDBNull(4) ? string.Empty : rBody.GetString(4);

                            total += credit;
                            voucher.Details.Add(new VoucherDetail
                            {
                                AccountNo = accNo,
                                Narration = narr,
                                Debit = 0,
                                Credit = credit,
                                Account = new Account
                                {
                                    AccountNo = accNo,
                                    Name = accName,
                                    UrduName = accUrdu
                                }
                            });
                        }
                        voucher.TotalDebit = total;
                        voucher.TotalCredit = total;
                    }
                }

                // 3. Check JournalVouchers (JV)
                if (voucher == null)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                        IF OBJECT_ID('JournalVouchers', 'U') IS NOT NULL
                            SELECT TOP 1 VoucherNo, VoucherDate, UserNo, IsPosted, NarrationUrdu, CropSeason 
                            FROM JournalVouchers WHERE VoucherNo = @id;";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@id";
                    p.Value = voucherId;
                    cmd.Parameters.Add(p);

                    using var r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        voucher = new Voucher
                        {
                            Id = voucherId,
                            VoucherNo = voucherId.ToString(),
                            VoucherType = Core.Enums.VoucherType.JournalVoucher,
                            VoucherDate = r.GetDateTime(1),
                            NarrationUrdu = r.IsDBNull(4) ? null : r.GetString(4),
                            CropSeason = r.IsDBNull(5) ? null : new CropSeason { Name = r.GetString(5) }
                        };
                    }

                    if (voucher != null)
                    {
                        using var cmdBody = conn.CreateCommand();
                        cmdBody.CommandText = @"
                            SELECT b.AccountNo, b.Narration, b.Debit, b.Credit, ISNULL(coa.AccountName, b.AccountNo), ISNULL(coa.AccountNameUrdu, '')
                            FROM JournalVouchersBody b
                            LEFT JOIN ChartOfAccounts coa ON b.AccountNo = coa.AccountNo
                            WHERE b.VoucherNo = @id
                            ORDER BY b.SerialNo;";
                        var pBody = cmdBody.CreateParameter();
                        pBody.ParameterName = "@id";
                        pBody.Value = voucherId;
                        cmdBody.Parameters.Add(pBody);

                        using var rBody = cmdBody.ExecuteReader();
                        decimal totalDebit = 0;
                        decimal totalCredit = 0;
                        while (rBody.Read())
                        {
                            var accNo = rBody.GetString(0);
                            var narr = rBody.IsDBNull(1) ? string.Empty : rBody.GetString(1);
                            var debit = rBody.IsDBNull(2) ? 0m : rBody.GetDecimal(2);
                            var credit = rBody.IsDBNull(3) ? 0m : rBody.GetDecimal(3);
                            var accName = rBody.IsDBNull(4) ? accNo : rBody.GetString(4);
                            var accUrdu = rBody.IsDBNull(5) ? string.Empty : rBody.GetString(5);

                            totalDebit += debit;
                            totalCredit += credit;
                            voucher.Details.Add(new VoucherDetail
                            {
                                AccountNo = accNo,
                                Narration = narr,
                                Debit = debit,
                                Credit = credit,
                                Account = new Account
                                {
                                    AccountNo = accNo,
                                    Name = accName,
                                    UrduName = accUrdu
                                }
                            });
                        }
                        voucher.TotalDebit = totalDebit;
                        voucher.TotalCredit = totalCredit;
                    }
                }

                // Check Denominations from CashNoteDetail
                if (voucher != null)
                {
                    try
                    {
                        using var cmdNotes = conn.CreateCommand();
                        cmdNotes.CommandText = @"
                            IF OBJECT_ID('CashNoteDetail', 'U') IS NOT NULL
                                SELECT TOP 1 R5000, R1000, R500, R100, R50, R20, R10, R5, R2, R1
                                FROM CashNoteDetail WHERE VoucherNo = @id;";
                        var pN = cmdNotes.CreateParameter();
                        pN.ParameterName = "@id";
                        pN.Value = voucherId;
                        cmdNotes.Parameters.Add(pN);

                        using var rN = cmdNotes.ExecuteReader();
                        if (rN.Read())
                        {
                            voucher.Count5000 = rN.IsDBNull(0) ? 0 : Convert.ToInt32(rN.GetDecimal(0));
                            voucher.Count1000 = rN.IsDBNull(1) ? 0 : Convert.ToInt32(rN.GetDecimal(1));
                            voucher.Count500 = rN.IsDBNull(2) ? 0 : Convert.ToInt32(rN.GetDecimal(2));
                            voucher.Count100 = rN.IsDBNull(3) ? 0 : Convert.ToInt32(rN.GetDecimal(3));
                            voucher.Count50 = rN.IsDBNull(4) ? 0 : Convert.ToInt32(rN.GetDecimal(4));
                            voucher.Count20 = rN.IsDBNull(5) ? 0 : Convert.ToInt32(rN.GetDecimal(5));
                            voucher.Count10 = rN.IsDBNull(6) ? 0 : Convert.ToInt32(rN.GetDecimal(6));
                            voucher.Count5 = rN.IsDBNull(7) ? 0 : Convert.ToInt32(rN.GetDecimal(7));
                            voucher.Count2 = rN.IsDBNull(8) ? 0 : Convert.ToInt32(rN.GetDecimal(8));
                            voucher.Count1 = rN.IsDBNull(9) ? 0 : Convert.ToInt32(rN.GetDecimal(9));
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        if (voucher == null)
            throw new InvalidOperationException($"Voucher with ID {voucherId} not found.");

        return BuildVoucherSlip(voucher);
    }

    public VoucherSlip BuildVoucherSlip(Voucher voucher)
    {
        string typeName = voucher.VoucherType switch
        {
            Core.Enums.VoucherType.CashPayment => "Cash Payment",
            Core.Enums.VoucherType.CashReceiving => "Cash Receiving",
            Core.Enums.VoucherType.BankPayment => "Bank Payment",
            Core.Enums.VoucherType.BankReceipt => "Bank Receipt",
            Core.Enums.VoucherType.JournalVoucher => "Journal",
            _ => voucher.VoucherType.ToString()
        };
        string typeUrdu = voucher.VoucherType switch
        {
            Core.Enums.VoucherType.CashPayment => "نقد ادائیگی واؤچر",
            Core.Enums.VoucherType.CashReceiving => "نقد وصولی واؤچر",
            Core.Enums.VoucherType.BankPayment => "بینک ادائیگی واؤچر",
            Core.Enums.VoucherType.BankReceipt => "بینک وصولی واؤچر",
            Core.Enums.VoucherType.JournalVoucher => "جنرل واؤچر (Double Entry)",
            _ => "واؤچر"
        };

        var firstDetail = voucher.Details.FirstOrDefault();
        decimal totalAmount = voucher.TotalDebit > 0 ? voucher.TotalDebit : voucher.TotalCredit;

        var detailLines = voucher.Details.Select(d => new VoucherSlipLine(
            AccountNo: !string.IsNullOrWhiteSpace(d.Account?.AccountNo) ? d.Account.AccountNo : (d.AccountNo ?? string.Empty),
            AccountName: !string.IsNullOrWhiteSpace(d.Account?.Name) ? d.Account.Name : (d.AccountNo ?? string.Empty),
            Narration: d.Narration ?? string.Empty,
            Debit: d.Debit,
            Credit: d.Credit
        )).ToList();

        if (voucher.VoucherType == Core.Enums.VoucherType.CashReceiving)
        {
            detailLines.Insert(0, new VoucherSlipLine(
                AccountNo: "1001",
                AccountName: "Cash in Hand",
                Narration: "Direct Cash Debit",
                Debit: totalAmount,
                Credit: 0
            ));
        }
        else if (voucher.VoucherType == Core.Enums.VoucherType.CashPayment)
        {
            detailLines.Add(new VoucherSlipLine(
                AccountNo: "1001",
                AccountName: "Cash in Hand",
                Narration: "Direct Cash Credit",
                Debit: 0,
                Credit: totalAmount
            ));
        }

        string headerAccountName = voucher.VoucherType == Core.Enums.VoucherType.JournalVoucher
            ? "Inter-Account Transfer / Adjustment"
            : (!string.IsNullOrWhiteSpace(firstDetail?.Account?.Name)
                ? firstDetail.Account.Name
                : (!string.IsNullOrWhiteSpace(firstDetail?.AccountNo) ? firstDetail.AccountNo : string.Empty));
        string headerUrduName = voucher.VoucherType == Core.Enums.VoucherType.JournalVoucher
            ? "کھاتہ جات منتقلی و ایڈجسٹمنٹ"
            : (firstDetail?.Account?.UrduName ?? string.Empty);
        string headerAccountNo = voucher.VoucherType == Core.Enums.VoucherType.JournalVoucher
            ? $"Total Entries: {voucher.Details.Count}"
            : (!string.IsNullOrWhiteSpace(firstDetail?.Account?.AccountNo)
                ? firstDetail.Account.AccountNo
                : (firstDetail?.AccountNo ?? string.Empty));

        return new VoucherSlip
        {
            VoucherNo = voucher.VoucherNo,
            VoucherType = typeName,
            VoucherTypeUrdu = typeUrdu,
            Date = voucher.VoucherDate,
            AccountNo = headerAccountNo,
            AccountName = headerAccountName,
            AccountUrduName = headerUrduName,
            Narration = voucher.NarrationEnglish ?? firstDetail?.Narration ?? string.Empty,
            NarrationUrdu = voucher.NarrationUrdu ?? string.Empty,
            TotalDebit = voucher.TotalDebit,
            TotalCredit = voucher.TotalCredit,
            Amount = totalAmount,
            CropSeason = voucher.CropSeason?.Name ?? string.Empty,
            Count5000 = voucher.Count5000,
            Count1000 = voucher.Count1000,
            Count500 = voucher.Count500,
            Count100 = voucher.Count100,
            Count50 = voucher.Count50,
            Count20 = voucher.Count20,
            Count10 = voucher.Count10,
            Count5 = voucher.Count5,
            Count2 = voucher.Count2,
            Count1 = voucher.Count1,
            Details = detailLines
        };
    }
}
