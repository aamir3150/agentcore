using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;

namespace MyMandiSystem.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 1A. System Foundation
    public DbSet<FinancialYear> FinancialYears => Set<FinancialYear>();
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserLog> UserLogs => Set<UserLog>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<CropSeason> CropSeasons => Set<CropSeason>();

    // 1B. Chart of Accounts
    public DbSet<AccountGroup> AccountGroups => Set<AccountGroup>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<GeneralLedger> GeneralLedger => Set<GeneralLedger>();

    // 1C. Party Management
    public DbSet<PartyGroup> PartyGroups => Set<PartyGroup>();
    public DbSet<Town> Towns => Set<Town>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<Party> Parties => Set<Party>();
    public DbSet<Salesman> Salesmen => Set<Salesman>();

    // 1D. Product & Inventory
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
    public DbSet<Product> Products => Set<Product>();

    // 1E. Voucher System
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<VoucherDetail> VoucherDetails => Set<VoucherDetail>();

    // 1F. Banking Module
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<ChequeBook> ChequeBooks => Set<ChequeBook>();
    public DbSet<Cheque> Cheques => Set<Cheque>();
    public DbSet<BankDeposit> BankDeposits => Set<BankDeposit>();

    // 1G. Trade Module
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();

    // 1H. Brokerage Module
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<BrokerageInvoice> BrokerageInvoices => Set<BrokerageInvoice>();
    public DbSet<BrokerageInvoiceDetail> BrokerageInvoiceDetails => Set<BrokerageInvoiceDetail>();
    public DbSet<GatePass> GatePasses => Set<GatePass>();
    public DbSet<BrokerageShortage> BrokerageShortages => Set<BrokerageShortage>();

    // 1I. Pestro Module
    public DbSet<PestroBatch> PestroBatches => Set<PestroBatch>();
    public DbSet<PestroInvoice> PestroInvoices => Set<PestroInvoice>();
    public DbSet<PestroInvoiceDetail> PestroInvoiceDetails => Set<PestroInvoiceDetail>();

    // 1J. Stock & Inventory
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<InsuranceEntry> InsuranceEntries => Set<InsuranceEntry>();
    public DbSet<Promise> Promises => Set<Promise>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Decimal Precision
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // Specific Decimal Precision for high-accuracy fields (Weights)
        modelBuilder.Entity<StockTransaction>().Property(s => s.QtyIn).HasColumnType("decimal(18,3)");
        modelBuilder.Entity<StockTransaction>().Property(s => s.QtyOut).HasColumnType("decimal(18,3)");
        modelBuilder.Entity<Product>().Property(s => s.OpeningStock).HasColumnType("decimal(18,3)");
        modelBuilder.Entity<Product>().Property(s => s.CurrentStock).HasColumnType("decimal(18,3)");
        modelBuilder.Entity<BrokerageInvoiceDetail>().Property(s => s.GrossWeight).HasColumnType("decimal(18,3)");
        modelBuilder.Entity<BrokerageInvoiceDetail>().Property(s => s.NetWeight).HasColumnType("decimal(18,3)");
        modelBuilder.Entity<GatePass>().Property(s => s.GrossWeight).HasColumnType("decimal(18,3)");
        modelBuilder.Entity<GatePass>().Property(s => s.NetWeight).HasColumnType("decimal(18,3)");

        // 1B. COA Hierarchy
        modelBuilder.Entity<AccountGroup>()
            .HasOne(g => g.ParentGroup)
            .WithMany(g => g.SubGroups)
            .HasForeignKey(g => g.ParentGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Account>()
            .HasOne(a => a.ParentAccount)
            .WithMany(a => a.SubAccounts)
            .HasForeignKey(a => a.ParentAccountId)
            .OnDelete(DeleteBehavior.NoAction);

        // 1J. Stock to Pestro Batch relation (Restrict delete batch if transactions exist)
        modelBuilder.Entity<StockTransaction>()
            .HasOne(s => s.Batch)
            .WithMany()
            .HasForeignKey(s => s.BatchId)
            .OnDelete(DeleteBehavior.NoAction);

        // General Ledger reverse relationship
        modelBuilder.Entity<GeneralLedger>()
            .HasOne(gl => gl.ReversedBy)
            .WithMany()
            .HasForeignKey(gl => gl.ReversedById)
            .OnDelete(DeleteBehavior.NoAction);

        // Vouchers
        modelBuilder.Entity<Voucher>()
            .HasOne(v => v.FinancialYear)
            .WithMany()
            .HasForeignKey(v => v.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Voucher>()
            .HasOne(v => v.CropSeason)
            .WithMany()
            .HasForeignKey(v => v.CropSeasonId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<VoucherDetail>()
            .HasOne(vd => vd.Voucher)
            .WithMany(v => v.Details)
            .HasForeignKey(vd => vd.VoucherId)
            .OnDelete(DeleteBehavior.Cascade);

        // Invoices
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.FinancialYear)
            .WithMany()
            .HasForeignKey(i => i.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.CropSeason)
            .WithMany()
            .HasForeignKey(i => i.CropSeasonId)
            .OnDelete(DeleteBehavior.NoAction);

        // Brokerage
        modelBuilder.Entity<Contract>()
            .HasOne(c => c.FinancialYear)
            .WithMany()
            .HasForeignKey(c => c.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.CropSeason)
            .WithMany()
            .HasForeignKey(c => c.CropSeasonId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BrokerageInvoice>()
            .HasOne(i => i.FinancialYear)
            .WithMany()
            .HasForeignKey(i => i.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BrokerageInvoice>()
            .HasOne(i => i.CropSeason)
            .WithMany()
            .HasForeignKey(i => i.CropSeasonId)
            .OnDelete(DeleteBehavior.NoAction);

        // Pestro
        modelBuilder.Entity<PestroInvoice>()
            .HasOne(i => i.FinancialYear)
            .WithMany()
            .HasForeignKey(i => i.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PestroInvoice>()
            .HasOne(i => i.CropSeason)
            .WithMany()
            .HasForeignKey(i => i.CropSeasonId)
            .OnDelete(DeleteBehavior.NoAction);

        // Stock & Ledger
        modelBuilder.Entity<StockTransaction>()
            .HasOne(s => s.FinancialYear)
            .WithMany()
            .HasForeignKey(s => s.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<GeneralLedger>()
            .HasOne(gl => gl.FinancialYear)
            .WithMany()
            .HasForeignKey(gl => gl.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<GeneralLedger>()
            .HasOne(gl => gl.CropSeason)
            .WithMany()
            .HasForeignKey(gl => gl.CropSeasonId)
            .OnDelete(DeleteBehavior.NoAction);

        // CropSeason to FinancialYear
        modelBuilder.Entity<CropSeason>()
            .HasOne(cs => cs.FinancialYear)
            .WithMany(fy => fy.CropSeasons)
            .HasForeignKey(cs => cs.FinancialYearId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
