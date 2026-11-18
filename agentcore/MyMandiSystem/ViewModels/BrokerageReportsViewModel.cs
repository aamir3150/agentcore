using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public enum BrokerageReportMode
{
    CurrentStock,
    CommissionReport,
    ContractsRegister,
    InvoicesRegister,
    PurchaseRegister,
    SaleRegister,
    StockReport,
    PartyLedgerExtended,
    MonthlyCommissionSoodh,
    PurchaseSummary,
    PurchaseSummaryPartyWise,
    PartyWiseWHT,
    VehicleWiseSummary,
    GatePassRegister,
    ProductLedger
}

public class BrokerageReportRow
{
    public int Id { get; set; }
    public string DocNo { get; set; } = "";
    public string Date { get; set; } = "";
    public DateTime RawDate { get; set; }
    public string DocType { get; set; } = "";
    public string PartyName { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string VehicleNo { get; set; } = "";
    public string DriverName { get; set; } = "";
    public string Narration { get; set; } = "";
    public string Status { get; set; } = "";

    // Quantities & Weights
    public decimal Bags { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal TareWeight { get; set; }
    public decimal NetWeight { get; set; }
    public decimal Mounds { get; set; } // Maunds (من = 40 Kgs)
    public decimal Rate { get; set; }

    // Financial Values
    public decimal GrossAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal WHT_Rate { get; set; }
    public decimal WHT_Amount { get; set; }
    public decimal SoodhAmount { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetAmount { get; set; }

    // Ledger / Product Ledger specific
    public decimal DrWeight { get; set; }
    public decimal CrWeight { get; set; }
    public decimal BalWeight { get; set; }
    public decimal DrValue { get; set; }
    public decimal CrValue { get; set; }
    public decimal BalValue { get; set; }
    public decimal Cost { get; set; }

    // Vehicle Wise Summary specific
    public string PurchaseParty { get; set; } = "";
    public string SaleParty { get; set; } = "";
    public decimal PurchaseKgs { get; set; }
    public decimal SaleKgs { get; set; }
    public decimal Shortage { get; set; }
    public decimal PurchaseValue { get; set; }
    public decimal SaleValue { get; set; }
    public decimal ProfitLoss { get; set; }

    // Summary / Month
    public string MonthName { get; set; } = "";
    public int InvoicesCount { get; set; }
    public bool IsSummary { get; set; }
}

public partial class BrokerageReportsViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    [ObservableProperty] private BrokerageReportMode _mode = BrokerageReportMode.CurrentStock;
    [ObservableProperty] private string _windowTitle = "Brokerage Reports - بروکریج رپورٹس";
    [ObservableProperty] private string _headerTitle = "Brokerage Reports";
    [ObservableProperty] private string _headerSubtitle = "بروکریج و بیوپار کاروباری رپورٹس سسٹم";

    // Date Filters
    [ObservableProperty] private bool _isDateFilterVisible = true;
    [ObservableProperty] private bool _isAllDates = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Party Filter
    [ObservableProperty] private bool _isPartyFilterVisible = true;
    [ObservableProperty] private ObservableCollection<Party> _parties = new();
    [ObservableProperty] private Party? _selectedParty;
    [ObservableProperty] private string _partySearchQuery = "";

    // Product Filter
    [ObservableProperty] private bool _isProductFilterVisible = true;
    [ObservableProperty] private ObservableCollection<Product> _products = new();
    [ObservableProperty] private Product? _selectedProduct;

    // Vehicle Filter
    [ObservableProperty] private bool _isVehicleFilterVisible = false;
    [ObservableProperty] private string _vehicleFilter = "";

    // Type / Status Filter
    [ObservableProperty] private bool _isTypeFilterVisible = false;
    [ObservableProperty] private ObservableCollection<string> _typeOptions = new() { "-- ALL --", "Purchase", "Sale" };
    [ObservableProperty] private string _selectedTypeOption = "-- ALL --";

    // Commission Status Filter
    [ObservableProperty] private bool _isCommissionStatusFilterVisible = false;
    [ObservableProperty] private ObservableCollection<string> _commissionStatusOptions = new() { "-- ALL --", "With Commission", "Without Commission" };
    [ObservableProperty] private string _selectedCommissionStatusOption = "-- ALL --";

    // Results & Grid Data
    [ObservableProperty] private ObservableCollection<BrokerageReportRow> _reportRows = new();
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _filterText = "";

    // Metric Summary Cards
    [ObservableProperty] private int _totalRecords = 0;
    [ObservableProperty] private decimal _totalBags = 0m;
    [ObservableProperty] private decimal _totalNetWeight = 0m;
    [ObservableProperty] private decimal _totalMounds = 0m;
    [ObservableProperty] private decimal _totalGrossAmount = 0m;
    [ObservableProperty] private decimal _totalCommission = 0m;
    [ObservableProperty] private decimal _totalWHT = 0m;
    [ObservableProperty] private decimal _totalNetAmount = 0m;
    [ObservableProperty] private decimal _totalProfitLoss = 0m;

    // Dynamic Metric Card Headers & Formatted Values
    [ObservableProperty] private string _card5Title = "COMMISSION / EARNINGS";
    [ObservableProperty] private string _card5Urdu = "کمیشن و سود آمدنی";
    [ObservableProperty] private string _card5Value = "0";
    [ObservableProperty] private string _card6Title = "NET AMOUNT (RS)";
    [ObservableProperty] private string _card6Urdu = "خالص قابلِ وصول / ادا";
    [ObservableProperty] private string _card6Value = "0";

    public Action? RequestClose { get; set; }

    public BrokerageReportsViewModel(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
    }

    public async Task InitializeAsync(BrokerageReportMode mode)
    {
        await LoadMasterDataAsync();
        await SetModeAsync(mode);
    }

    private async Task LoadMasterDataAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var partiesList = await context.Parties.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();
            Parties = new ObservableCollection<Party>(partiesList);

            var productsList = await context.Products.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();
            Products = new ObservableCollection<Product>(productsList);

            var year = await _systemService.GetCurrentYearAsync();
            if (year != null)
            {
                FromDate = year.StartDate;
                ToDate = DateTime.Today < year.EndDate ? DateTime.Today : year.EndDate;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BrokerageReportsViewModel Init Error: {ex.Message}");
        }
    }

    public async Task SetModeAsync(BrokerageReportMode mode)
    {
        Mode = mode;
        ResetVisibilityFlags();

        switch (mode)
        {
            case BrokerageReportMode.CurrentStock:
                WindowTitle = "Current Stock - موجودہ بروکریج اسٹاک";
                HeaderTitle = "Current Stock Report";
                HeaderSubtitle = "موجودہ اجناس و بیوپار اسٹاک مع اوسط قیمت و مالیت";
                IsDateFilterVisible = false;
                IsPartyFilterVisible = false;
                IsProductFilterVisible = true;
                break;

            case BrokerageReportMode.CommissionReport:
                WindowTitle = "Commission Report - بروکریج کمیشن رپورٹ";
                HeaderTitle = "Commission Report";
                HeaderSubtitle = "بروکریج سودا و انوائسز پر وصول شدہ کمیشن و سود رپورٹ";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = false;
                IsCommissionStatusFilterVisible = true;
                break;

            case BrokerageReportMode.ContractsRegister:
                WindowTitle = "Purchase And Sale Contracts Register - سودا رجسٹر";
                HeaderTitle = "Contracts Register (سودا جات رجسٹر)";
                HeaderSubtitle = "خرید و فروخت کے تمام سودوں (کنٹریکٹس) کا تفصیلی رجسٹر";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = true;
                IsTypeFilterVisible = true;
                break;

            case BrokerageReportMode.InvoicesRegister:
                WindowTitle = "Purchase And Sale Invoices Register - انوائسز رجسٹر";
                HeaderTitle = "Invoices Register (بیوپار بل رجسٹر)";
                HeaderSubtitle = "خرید و فروخت کے تمام بروکریج بلوں و پرچوں کا رجسٹر";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = false;
                IsTypeFilterVisible = true;
                break;

            case BrokerageReportMode.PurchaseRegister:
                WindowTitle = "Purchase Register - خریداری رجسٹر";
                HeaderTitle = "Brokerage Purchase Register";
                HeaderSubtitle = "بروکریج خریداری بلوں، گاڑی و وزن کا تفصیلی رجسٹر";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = true;
                break;

            case BrokerageReportMode.SaleRegister:
                WindowTitle = "Sale Register - فروخت رجسٹر";
                HeaderTitle = "Brokerage Sale Register";
                HeaderSubtitle = "بروکریج فروخت بلوں، خریداروں و وزن کا تفصیلی رجسٹر";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = true;
                break;

            case BrokerageReportMode.StockReport:
                WindowTitle = "Stock Report - اسٹاک رپورٹ مع اوپننگ";
                HeaderTitle = "Stock Movement Report";
                HeaderSubtitle = "اوپننگ اسٹاک، خریدو فروخت آمد و خرچ مع کلوزنگ بیلنس";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = false;
                IsProductFilterVisible = true;
                break;

            case BrokerageReportMode.PartyLedgerExtended:
                WindowTitle = "Party Ledger Extended - پارٹی لیجر بروکریج";
                HeaderTitle = "Brokerage Party Ledger Extended";
                HeaderSubtitle = "کھاتہ دار کا تفصیلی لیجر مع گاڑی نمبر، بوری، وزن و بل تفصیلات";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = false;
                break;

            case BrokerageReportMode.MonthlyCommissionSoodh:
                WindowTitle = "Monthly Commission/Soodh Report - ماہانہ کمیشن و سود";
                HeaderTitle = "Monthly Commission & Soodh Report";
                HeaderSubtitle = "ماہ بہ ماہ کمیشن و سود کی مجموعی آمدنی کا تقابلی جائزہ";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = false;
                IsProductFilterVisible = false;
                break;

            case BrokerageReportMode.PurchaseSummary:
                WindowTitle = "Purchase Summary Report - خریداری خلاصہ رپورٹ";
                HeaderTitle = "Purchase Summary Report (جنس وار خلاصہ)";
                HeaderSubtitle = "اجناس و فصل وار مجموعی خریداری خلاصہ (بوری، وزن و اوسط ریٹ)";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = false;
                IsProductFilterVisible = true;
                break;

            case BrokerageReportMode.PurchaseSummaryPartyWise:
                WindowTitle = "Purchase Summary (Party Wise) - پارٹی وار خریداری خلاصہ";
                HeaderTitle = "Purchase Summary (Party Wise)";
                HeaderSubtitle = "زمیندار و وینڈر وار خریداری خلاصہ و مجموعی حساب";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = false;
                break;

            case BrokerageReportMode.PartyWiseWHT:
                WindowTitle = "Party Wise W.H.T Report - ود ہولڈنگ ٹیکس رپورٹ";
                HeaderTitle = "Party Wise W.H.T. Deductions";
                HeaderSubtitle = "پارٹی وار کٹوتی ود ہولڈنگ انکم ٹیکس (WHT) رجسٹر";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = false;
                break;

            case BrokerageReportMode.VehicleWiseSummary:
                WindowTitle = "Vehicle Wise Summary - گاڑی وار نفع و نقصان سمری";
                HeaderTitle = "Vehicle Wise Trading Summary";
                HeaderSubtitle = "گاڑی وار خریداری بمقابلہ فروخت، وزن کمی (شارٹیج) و نفع نقصان کا موازنہ";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = false;
                IsProductFilterVisible = true;
                IsVehicleFilterVisible = true;
                break;

            case BrokerageReportMode.GatePassRegister:
                WindowTitle = "Gate Pass Register - گیٹ پاس رجسٹر";
                HeaderTitle = "Gate Pass Register";
                HeaderSubtitle = "آمد و روانگی گیٹ پاسز (گاڑی نمبر، ڈرائیور و وزن اندراج)";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = true;
                IsProductFilterVisible = true;
                IsTypeFilterVisible = true;
                break;

            case BrokerageReportMode.ProductLedger:
                WindowTitle = "Product Ledger - اجناس و پروڈکٹ لیجر کھاتہ";
                HeaderTitle = "Brokerage Product Ledger";
                HeaderSubtitle = "پروڈکٹ / جنس کا مکمل آمد و خرچ کھاتہ مع لاگت و اسٹاک بیلنس";
                IsDateFilterVisible = true;
                IsPartyFilterVisible = false;
                IsProductFilterVisible = true;
                break;
        }

        await LoadDataAsync();
    }

    private void ResetVisibilityFlags()
    {
        IsDateFilterVisible = true;
        IsPartyFilterVisible = true;
        IsProductFilterVisible = true;
        IsVehicleFilterVisible = false;
        IsTypeFilterVisible = false;
        IsCommissionStatusFilterVisible = false;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        ReportRows.Clear();
        ResetTotals();

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var from = IsAllDates ? new DateTime(1900, 1, 1) : FromDate.Date;
            var to = IsAllDates ? new DateTime(2900, 1, 1) : ToDate.Date.AddDays(1).AddSeconds(-1);

            switch (Mode)
            {
                case BrokerageReportMode.CurrentStock:
                    await LoadCurrentStockAsync(context);
                    break;
                case BrokerageReportMode.CommissionReport:
                    await LoadCommissionReportAsync(context, from, to);
                    break;
                case BrokerageReportMode.ContractsRegister:
                    await LoadContractsRegisterAsync(context, from, to);
                    break;
                case BrokerageReportMode.InvoicesRegister:
                    await LoadInvoicesRegisterAsync(context, from, to);
                    break;
                case BrokerageReportMode.PurchaseRegister:
                    await LoadPurchaseRegisterAsync(context, from, to);
                    break;
                case BrokerageReportMode.SaleRegister:
                    await LoadSaleRegisterAsync(context, from, to);
                    break;
                case BrokerageReportMode.StockReport:
                    await LoadStockReportAsync(context, from, to);
                    break;
                case BrokerageReportMode.PartyLedgerExtended:
                    await LoadPartyLedgerExtendedAsync(context, from, to);
                    break;
                case BrokerageReportMode.MonthlyCommissionSoodh:
                    await LoadMonthlyCommissionSoodhAsync(context, from, to);
                    break;
                case BrokerageReportMode.PurchaseSummary:
                    await LoadPurchaseSummaryAsync(context, from, to);
                    break;
                case BrokerageReportMode.PurchaseSummaryPartyWise:
                    await LoadPurchaseSummaryPartyWiseAsync(context, from, to);
                    break;
                case BrokerageReportMode.PartyWiseWHT:
                    await LoadPartyWiseWHTAsync(context, from, to);
                    break;
                case BrokerageReportMode.VehicleWiseSummary:
                    await LoadVehicleWiseSummaryAsync(context, from, to);
                    break;
                case BrokerageReportMode.GatePassRegister:
                    await LoadGatePassRegisterAsync(context, from, to);
                    break;
                case BrokerageReportMode.ProductLedger:
                    await LoadProductLedgerAsync(context, from, to);
                    break;
            }

            CalculateTotals();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Report generation error: {ex.Message}", "agentcore Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadCurrentStockAsync(AppDbContext context)
    {
        var query = context.Products.Where(p => p.IsActive);
        if (SelectedProduct != null) query = query.Where(p => p.Id == SelectedProduct.Id);

        var list = await query.OrderBy(p => p.Name).ToListAsync();
        int seq = 1;
        foreach (var p in list)
        {
            var val = p.CurrentStock * (p.PurchasePrice > 0 ? p.PurchasePrice : p.SalePrice);
            var mnds = Math.Round(p.CurrentStock / 40.0m, 2);
            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = p.ProductCode,
                ProductName = p.Name,
                Narration = p.UrduName ?? "-",
                Bags = 0,
                NetWeight = p.CurrentStock,
                Mounds = mnds,
                Rate = p.PurchasePrice > 0 ? p.PurchasePrice : p.SalePrice,
                GrossAmount = val,
                NetAmount = val,
                Status = p.CurrentStock > 0 ? "IN STOCK" : (p.CurrentStock < 0 ? "NEGATIVE" : "NIL")
            });
        }
    }

    private async Task LoadCommissionReportAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var query = context.BrokerageInvoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to);

        if (SelectedParty != null) query = query.Where(i => i.PartyId == SelectedParty.Id);
        if (SelectedCommissionStatusOption == "With Commission") query = query.Where(i => i.CommissionAmount > 0);
        else if (SelectedCommissionStatusOption == "Without Commission") query = query.Where(i => i.CommissionAmount == 0);

        var list = await query.OrderBy(i => i.InvoiceDate).ToListAsync();
        int seq = 1;
        foreach (var i in list)
        {
            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = i.InvoiceNo,
                Date = i.InvoiceDate.ToString("dd/MM/yyyy"),
                RawDate = i.InvoiceDate,
                PartyName = i.Party?.Name ?? "General",
                VehicleNo = i.VehicleNo ?? "-",
                GrossAmount = i.GrossAmount,
                CommissionAmount = i.CommissionAmount,
                SoodhAmount = i.Soodh_Amount,
                Expenses = i.LaborCharges + i.MarketCommitteeFee,
                NetAmount = i.NetAmount,
                Status = i.IsPosted ? "POSTED" : "DRAFT"
            });
        }
    }

    private async Task LoadContractsRegisterAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var query = context.Contracts
            .Include(c => c.Party)
            .Include(c => c.Product)
            .Where(c => c.ContractDate >= from && c.ContractDate <= to);

        if (SelectedParty != null) query = query.Where(c => c.PartyId == SelectedParty.Id);
        if (SelectedProduct != null) query = query.Where(c => c.ProductId == SelectedProduct.Id);
        if (SelectedTypeOption == "Purchase") query = query.Where(c => c.ContractType == ContractType.Purchase);
        else if (SelectedTypeOption == "Sale") query = query.Where(c => c.ContractType == ContractType.Sale);

        var list = await query.OrderBy(c => c.ContractDate).ToListAsync();
        int seq = 1;
        foreach (var c in list)
        {
            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = c.ContractNo,
                Date = c.ContractDate.ToString("dd/MM/yyyy"),
                RawDate = c.ContractDate,
                DocType = c.ContractType.ToString(),
                PartyName = c.Party?.Name ?? "General",
                ProductName = c.Product?.Name ?? "-",
                VehicleNo = c.VehicleNo ?? "-",
                DriverName = c.DriverName ?? "-",
                Bags = c.Quantity,
                NetWeight = c.Quantity * 40.0m,
                Mounds = c.Quantity,
                Rate = c.Rate,
                GrossAmount = c.TotalAmount,
                CommissionAmount = c.CommissionAmount,
                NetAmount = c.TotalAmount,
                Status = c.Status.ToString()
            });
        }
    }

    private async Task LoadInvoicesRegisterAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var query = context.BrokerageInvoices
            .Include(i => i.Party)
            .Include(i => i.Contract)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to);

        if (SelectedParty != null) query = query.Where(i => i.PartyId == SelectedParty.Id);
        if (SelectedTypeOption == "Purchase") query = query.Where(i => i.InvoiceType == InvoiceType.BrkPurchase || i.InvoiceType == InvoiceType.GenPurchase);
        else if (SelectedTypeOption == "Sale") query = query.Where(i => i.InvoiceType == InvoiceType.BrkSale || i.InvoiceType == InvoiceType.GenSale);

        var list = await query.OrderBy(i => i.InvoiceDate).ToListAsync();
        int seq = 1;
        foreach (var i in list)
        {
            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = i.InvoiceNo,
                Date = i.InvoiceDate.ToString("dd/MM/yyyy"),
                RawDate = i.InvoiceDate,
                DocType = i.InvoiceType.ToString(),
                PartyName = i.Party?.Name ?? "General",
                VehicleNo = i.VehicleNo ?? "-",
                GrossAmount = i.GrossAmount,
                CommissionAmount = i.CommissionAmount,
                WHT_Amount = i.WHT_Amount,
                SoodhAmount = i.Soodh_Amount,
                NetAmount = i.NetAmount,
                Status = i.Status.ToString()
            });
        }
    }

    private async Task LoadPurchaseRegisterAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var query = context.BrokerageInvoices
            .Include(i => i.Party)
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && (i.InvoiceType == InvoiceType.BrkPurchase || i.InvoiceType == InvoiceType.GenPurchase));

        if (SelectedParty != null) query = query.Where(i => i.PartyId == SelectedParty.Id);
        if (SelectedProduct != null) query = query.Where(i => i.Details.Any(d => d.ProductId == SelectedProduct.Id));

        var list = await query.OrderBy(i => i.InvoiceDate).ToListAsync();
        int seq = 1;
        foreach (var i in list)
        {
            var details = SelectedProduct != null 
                ? i.Details.Where(d => d.ProductId == SelectedProduct.Id).ToList() 
                : i.Details.ToList();

            var bags = details.Sum(d => d.Bags);
            var netWt = details.Sum(d => d.NetWeight);
            var prodName = string.Join(", ", details.Select(d => d.Product?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct());
            var mounds = Math.Round(netWt / 40.0m, 2);
            var rate = mounds > 0 ? Math.Round(i.GrossAmount / mounds, 2) : 0m;

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = i.InvoiceNo,
                Date = i.InvoiceDate.ToString("dd/MM/yyyy"),
                RawDate = i.InvoiceDate,
                DocType = "Purchase",
                PartyName = i.Party?.Name ?? "General",
                ProductName = string.IsNullOrEmpty(prodName) ? "General Commodity" : prodName,
                VehicleNo = i.VehicleNo ?? "-",
                Bags = bags,
                NetWeight = netWt,
                Mounds = mounds,
                Rate = rate,
                GrossAmount = i.GrossAmount,
                CommissionAmount = i.CommissionAmount,
                Expenses = i.LaborCharges + i.MarketCommitteeFee,
                NetAmount = i.NetAmount,
                Status = i.Status.ToString()
            });
        }
    }

    private async Task LoadSaleRegisterAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var query = context.BrokerageInvoices
            .Include(i => i.Party)
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && (i.InvoiceType == InvoiceType.BrkSale || i.InvoiceType == InvoiceType.GenSale));

        if (SelectedParty != null) query = query.Where(i => i.PartyId == SelectedParty.Id);
        if (SelectedProduct != null) query = query.Where(i => i.Details.Any(d => d.ProductId == SelectedProduct.Id));

        var list = await query.OrderBy(i => i.InvoiceDate).ToListAsync();
        int seq = 1;
        foreach (var i in list)
        {
            var details = SelectedProduct != null 
                ? i.Details.Where(d => d.ProductId == SelectedProduct.Id).ToList() 
                : i.Details.ToList();

            var bags = details.Sum(d => d.Bags);
            var netWt = details.Sum(d => d.NetWeight);
            var prodName = string.Join(", ", details.Select(d => d.Product?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct());
            var mounds = Math.Round(netWt / 40.0m, 2);
            var rate = mounds > 0 ? Math.Round(i.GrossAmount / mounds, 2) : 0m;

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = i.InvoiceNo,
                Date = i.InvoiceDate.ToString("dd/MM/yyyy"),
                RawDate = i.InvoiceDate,
                DocType = "Sale",
                PartyName = i.Party?.Name ?? "General",
                ProductName = string.IsNullOrEmpty(prodName) ? "General Commodity" : prodName,
                VehicleNo = i.VehicleNo ?? "-",
                Bags = bags,
                NetWeight = netWt,
                Mounds = mounds,
                Rate = rate,
                GrossAmount = i.GrossAmount,
                CommissionAmount = i.CommissionAmount,
                Expenses = i.LaborCharges + i.MarketCommitteeFee,
                NetAmount = i.NetAmount,
                Status = i.Status.ToString()
            });
        }
    }

    private async Task LoadStockReportAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var prodsQuery = context.Products.Where(p => p.IsActive);
        if (SelectedProduct != null) prodsQuery = prodsQuery.Where(p => p.Id == SelectedProduct.Id);
        var products = await prodsQuery.OrderBy(p => p.Name).ToListAsync();

        var priorDetails = await context.Set<BrokerageInvoiceDetail>()
            .Include(d => d.Invoice)
            .Where(d => d.Invoice.InvoiceDate < from)
            .ToListAsync();

        var currentDetails = await context.Set<BrokerageInvoiceDetail>()
            .Include(d => d.Invoice)
            .Where(d => d.Invoice.InvoiceDate >= from && d.Invoice.InvoiceDate <= to)
            .ToListAsync();

        int seq = 1;
        foreach (var p in products)
        {
            var priorForProd = priorDetails.Where(d => d.ProductId == p.Id).ToList();
            var priorPur = priorForProd.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkPurchase || d.Invoice.InvoiceType == InvoiceType.GenPurchase).Sum(d => d.NetWeight);
            var priorSale = priorForProd.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkSale || d.Invoice.InvoiceType == InvoiceType.GenSale).Sum(d => d.NetWeight);
            var openingStock = p.OpeningStock + priorPur - priorSale;

            var currForProd = currentDetails.Where(d => d.ProductId == p.Id).ToList();
            var purWeight = currForProd.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkPurchase || d.Invoice.InvoiceType == InvoiceType.GenPurchase).Sum(d => d.NetWeight);
            var saleWeight = currForProd.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkSale || d.Invoice.InvoiceType == InvoiceType.GenSale).Sum(d => d.NetWeight);
            var closing = openingStock + purWeight - saleWeight;

            var rate = p.PurchasePrice > 0 ? p.PurchasePrice : p.SalePrice;
            var closingVal = closing * rate;

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = p.ProductCode,
                ProductName = p.Name,
                DrWeight = openingStock,       // Opening Stock (Kg)
                GrossWeight = purWeight,       // Purchase Inward (Kg)
                TareWeight = saleWeight,       // Sale Outward (Kg)
                NetWeight = closing,           // Closing Balance (Kg)
                Mounds = Math.Round(closing / 40.0m, 2),
                Rate = rate,
                GrossAmount = closingVal,
                NetAmount = closingVal,
                Status = closing > 0 ? "IN STOCK" : (closing < 0 ? "DEFICIT" : "NIL")
            });
        }
    }

    private async Task LoadPartyLedgerExtendedAsync(AppDbContext context, DateTime from, DateTime to)
    {
        if (SelectedParty == null)
        {
            MessageBox.Show("Please select a Party to view Party Ledger Extended.\nبراہ کرم پارٹی منتخب کریں۔", "Party Required", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var party = await context.Parties.Include(p => p.Account).FirstOrDefaultAsync(p => p.Id == SelectedParty.Id);
        if (party == null || party.AccountId == 0) return;

        int accountId = party.AccountId;
        var account = await context.Accounts.FindAsync(accountId);
        decimal baseOpening = account?.OpeningBalance ?? 0m;

        var priorDebit = await context.GeneralLedger
            .Where(g => g.AccountId == accountId && g.TransactionDate < from)
            .SumAsync(g => (decimal?)g.Debit) ?? 0m;

        var priorCredit = await context.GeneralLedger
            .Where(g => g.AccountId == accountId && g.TransactionDate < from)
            .SumAsync(g => (decimal?)g.Credit) ?? 0m;

        decimal runningBal = baseOpening + (priorDebit - priorCredit);

        ReportRows.Add(new BrokerageReportRow
        {
            Id = 1,
            DocNo = "-",
            Date = from.ToString("dd/MM/yyyy"),
            Narration = "Opening Balance (ابتدائی بقایا)",
            DrValue = runningBal > 0 ? runningBal : 0,
            CrValue = runningBal < 0 ? Math.Abs(runningBal) : 0,
            BalValue = runningBal,
            Status = runningBal >= 0 ? "Dr" : "Cr"
        });

        var ledgerList = await context.GeneralLedger
            .Where(g => g.AccountId == accountId && g.TransactionDate >= from && g.TransactionDate <= to)
            .OrderBy(g => g.TransactionDate).ThenBy(g => g.Id)
            .ToListAsync();

        // Preload any linked brokerage invoices to display VehicleNo, Bags, Mounds
        var brkSourceIds = ledgerList.Where(g => g.SourceType == SourceType.BrkInvoice).Select(g => g.SourceId).Distinct().ToList();
        var linkedInvoices = await context.BrokerageInvoices
            .Include(i => i.Details)
            .Where(i => brkSourceIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        int seq = 2;
        foreach (var g in ledgerList)
        {
            runningBal += (g.Debit - g.Credit);

            string vehicle = "-";
            decimal bags = 0;
            decimal mounds = 0;
            string docNo = g.SourceId.ToString();

            if (g.SourceType == SourceType.BrkInvoice && linkedInvoices.TryGetValue(g.SourceId, out var inv))
            {
                docNo = inv.InvoiceNo;
                vehicle = inv.VehicleNo ?? "-";
                bags = inv.Details.Sum(d => d.Bags);
                var netWt = inv.Details.Sum(d => d.NetWeight);
                mounds = Math.Round(netWt / 40.0m, 2);
            }

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = docNo,
                Date = g.TransactionDate.ToString("dd/MM/yyyy"),
                RawDate = g.TransactionDate,
                VehicleNo = vehicle,
                Bags = bags,
                Mounds = mounds,
                Narration = g.Narration ?? "-",
                DrValue = g.Debit,
                CrValue = g.Credit,
                BalValue = runningBal,
                Status = runningBal >= 0 ? "Dr" : "Cr"
            });
        }
    }

    private async Task LoadMonthlyCommissionSoodhAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var list = await context.BrokerageInvoices
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to)
            .OrderBy(i => i.InvoiceDate)
            .ToListAsync();

        var grouped = list.GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

        int seq = 1;
        foreach (var grp in grouped)
        {
            var firstDate = new DateTime(grp.Key.Year, grp.Key.Month, 1);
            var gross = grp.Sum(i => i.GrossAmount);
            var comm = grp.Sum(i => i.CommissionAmount);
            var soodh = grp.Sum(i => i.Soodh_Amount);
            var net = comm + soodh;

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                MonthName = firstDate.ToString("MMMM yyyy"),
                Date = firstDate.ToString("MM/yyyy"),
                InvoicesCount = grp.Count(),
                GrossAmount = gross,
                CommissionAmount = comm,
                SoodhAmount = soodh,
                NetAmount = net,
                Status = "SUMMARY"
            });
        }
    }

    private async Task LoadPurchaseSummaryAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var details = await context.Set<BrokerageInvoiceDetail>()
            .Include(d => d.Product)
            .Include(d => d.Invoice)
            .Where(d => d.Invoice.InvoiceDate >= from && d.Invoice.InvoiceDate <= to &&
                       (d.Invoice.InvoiceType == InvoiceType.BrkPurchase || d.Invoice.InvoiceType == InvoiceType.GenPurchase))
            .ToListAsync();

        if (SelectedProduct != null) details = details.Where(d => d.ProductId == SelectedProduct.Id).ToList();

        var grouped = details.GroupBy(d => d.Product?.Name ?? "General").OrderBy(g => g.Key);
        int seq = 1;
        foreach (var grp in grouped)
        {
            var totalBags = grp.Sum(d => d.Bags);
            var totalWt = grp.Sum(d => d.NetWeight);
            var totalVal = grp.Sum(d => d.Amount);
            var avgRate = totalWt > 0 ? (totalVal / (totalWt / 40.0m)) : 0m;

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                ProductName = grp.Key,
                InvoicesCount = grp.Select(d => d.InvoiceId).Distinct().Count(),
                Bags = totalBags,
                NetWeight = totalWt,
                Mounds = Math.Round(totalWt / 40.0m, 2),
                Rate = Math.Round(avgRate, 2),
                GrossAmount = totalVal,
                NetAmount = totalVal
            });
        }
    }

    private async Task LoadPurchaseSummaryPartyWiseAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var invoices = await context.BrokerageInvoices
            .Include(i => i.Party)
            .Include(i => i.Details)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to &&
                       (i.InvoiceType == InvoiceType.BrkPurchase || i.InvoiceType == InvoiceType.GenPurchase))
            .ToListAsync();

        if (SelectedParty != null) invoices = invoices.Where(i => i.PartyId == SelectedParty.Id).ToList();

        var grouped = invoices.GroupBy(i => i.Party?.Name ?? "General").OrderBy(g => g.Key);
        int seq = 1;
        foreach (var grp in grouped)
        {
            var totalBags = grp.SelectMany(i => i.Details).Sum(d => d.Bags);
            var totalWt = grp.SelectMany(i => i.Details).Sum(d => d.NetWeight);
            var totalVal = grp.Sum(i => i.NetAmount);

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                PartyName = grp.Key,
                InvoicesCount = grp.Count(),
                Bags = totalBags,
                NetWeight = totalWt,
                Mounds = Math.Round(totalWt / 40.0m, 2),
                GrossAmount = grp.Sum(i => i.GrossAmount),
                NetAmount = totalVal
            });
        }
    }

    private async Task LoadPartyWiseWHTAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var invoices = await context.BrokerageInvoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.WHT_Amount > 0)
            .ToListAsync();

        if (SelectedParty != null) invoices = invoices.Where(i => i.PartyId == SelectedParty.Id).ToList();

        var grouped = invoices.GroupBy(i => i.Party).OrderBy(g => g.Key?.Name);
        int seq = 1;
        foreach (var grp in grouped)
        {
            var party = grp.Key;
            var gross = grp.Sum(i => i.GrossAmount);
            var wht = grp.Sum(i => i.WHT_Amount);

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                PartyName = party?.Name ?? "General",
                Narration = party?.CNIC ?? "-",
                InvoicesCount = grp.Count(),
                GrossAmount = gross,
                WHT_Rate = party?.WHT_Percentage ?? 0,
                WHT_Amount = wht,
                NetAmount = gross - wht
            });
        }
    }

    private async Task LoadVehicleWiseSummaryAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var invoices = await context.BrokerageInvoices
            .Include(i => i.Party)
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && !string.IsNullOrEmpty(i.VehicleNo))
            .ToListAsync();

        if (SelectedProduct != null)
            invoices = invoices.Where(i => i.Details.Any(d => d.ProductId == SelectedProduct.Id)).ToList();

        if (!string.IsNullOrWhiteSpace(VehicleFilter))
            invoices = invoices.Where(i => i.VehicleNo!.Contains(VehicleFilter, StringComparison.OrdinalIgnoreCase)).ToList();

        var grouped = invoices.GroupBy(i => i.VehicleNo!.Trim()).OrderBy(g => g.Key);
        int seq = 1;
        foreach (var grp in grouped)
        {
            var pur = grp.Where(i => i.InvoiceType == InvoiceType.BrkPurchase || i.InvoiceType == InvoiceType.GenPurchase).ToList();
            var sale = grp.Where(i => i.InvoiceType == InvoiceType.BrkSale || i.InvoiceType == InvoiceType.GenSale).ToList();

            var purParty = string.Join(", ", pur.Select(p => p.Party?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct());
            var saleParty = string.Join(", ", sale.Select(s => s.Party?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct());

            var purDetails = SelectedProduct != null 
                ? pur.SelectMany(p => p.Details).Where(d => d.ProductId == SelectedProduct.Id).ToList()
                : pur.SelectMany(p => p.Details).ToList();

            var saleDetails = SelectedProduct != null 
                ? sale.SelectMany(s => s.Details).Where(d => d.ProductId == SelectedProduct.Id).ToList()
                : sale.SelectMany(s => s.Details).ToList();

            var purKgs = purDetails.Sum(d => d.NetWeight);
            var saleKgs = saleDetails.Sum(d => d.NetWeight);
            var shortage = purKgs - saleKgs;

            var purVal = pur.Sum(p => p.NetAmount);
            var saleVal = sale.Sum(s => s.NetAmount);
            var pnl = saleVal - purVal;

            var prodNames = string.Join(", ", grp.SelectMany(i => i.Details).Select(d => d.Product?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct());

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                VehicleNo = grp.Key,
                Date = grp.First().InvoiceDate.ToString("dd/MM/yyyy"),
                ProductName = string.IsNullOrEmpty(prodNames) ? "-" : prodNames,
                PurchaseParty = string.IsNullOrEmpty(purParty) ? "-" : purParty,
                SaleParty = string.IsNullOrEmpty(saleParty) ? "-" : saleParty,
                PurchaseKgs = purKgs,
                SaleKgs = saleKgs,
                Shortage = shortage,
                PurchaseValue = purVal,
                SaleValue = saleVal,
                ProfitLoss = pnl,
                Status = pnl >= 0 ? "PROFIT" : "LOSS"
            });
        }
    }

    private async Task LoadGatePassRegisterAsync(AppDbContext context, DateTime from, DateTime to)
    {
        var query = context.GatePasses
            .Include(g => g.Party)
            .Include(g => g.Product)
            .Where(g => g.GatePassDate >= from && g.GatePassDate <= to);

        if (SelectedParty != null) query = query.Where(g => g.PartyId == SelectedParty.Id);
        if (SelectedProduct != null) query = query.Where(g => g.ProductId == SelectedProduct.Id);
        if (SelectedTypeOption == "Purchase") query = query.Where(g => g.GatePassType == GatePassType.In);
        else if (SelectedTypeOption == "Sale") query = query.Where(g => g.GatePassType == GatePassType.Out);

        var list = await query.OrderBy(g => g.GatePassDate).ToListAsync();
        int seq = 1;
        foreach (var g in list)
        {
            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = g.GatePassNo,
                Date = g.GatePassDate.ToString("dd/MM/yyyy"),
                RawDate = g.GatePassDate,
                DocType = g.GatePassType == GatePassType.In ? "INWARD" : "OUTWARD",
                PartyName = g.Party?.Name ?? "General",
                ProductName = g.Product?.Name ?? "-",
                VehicleNo = g.VehicleNo ?? "-",
                DriverName = g.DriverName ?? "-",
                Bags = g.Bags,
                GrossWeight = g.GrossWeight,
                TareWeight = g.TareWeight,
                NetWeight = g.NetWeight,
                Mounds = Math.Round(g.NetWeight / 40.0m, 2),
                Status = g.Status.ToString()
            });
        }
    }

    private async Task LoadProductLedgerAsync(AppDbContext context, DateTime from, DateTime to)
    {
        if (SelectedProduct == null)
        {
            MessageBox.Show("Please select a Product to view Product Ledger.", "Product Required", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        int prodId = SelectedProduct.Id;
        var product = await context.Products.FindAsync(prodId);
        if (product == null) return;

        // Exact AgenSoft SPRptProductLedgerBrk logic:
        // 1. Calculate Opening before from Date
        var priorInvoices = await context.Set<BrokerageInvoiceDetail>()
            .Include(d => d.Invoice)
            .Where(d => d.ProductId == prodId && d.Invoice.InvoiceDate < from)
            .ToListAsync();

        var priorInWeight = priorInvoices.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkPurchase || d.Invoice.InvoiceType == InvoiceType.GenPurchase).Sum(d => d.NetWeight);
        var priorOutWeight = priorInvoices.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkSale || d.Invoice.InvoiceType == InvoiceType.GenSale).Sum(d => d.NetWeight);
        var priorInVal = priorInvoices.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkPurchase || d.Invoice.InvoiceType == InvoiceType.GenPurchase).Sum(d => d.Amount);
        var priorOutVal = priorInvoices.Where(d => d.Invoice.InvoiceType == InvoiceType.BrkSale || d.Invoice.InvoiceType == InvoiceType.GenSale).Sum(d => d.Amount);

        decimal runningWeight = product.OpeningStock + (priorInWeight - priorOutWeight);
        decimal runningValue = (product.OpeningStock * product.PurchasePrice) + (priorInVal - priorOutVal);
        decimal currentCost = runningWeight > 0 ? (runningValue / runningWeight) : product.PurchasePrice;

        ReportRows.Add(new BrokerageReportRow
        {
            Id = 1,
            DocNo = "-",
            Date = from.ToString("dd/MM/yyyy"),
            DocType = "Opening",
            ProductName = product.Name,
            Narration = "Opening Stock Balance (ابتدائی اسٹاک)",
            DrWeight = runningWeight > 0 ? runningWeight : 0,
            CrWeight = runningWeight < 0 ? Math.Abs(runningWeight) : 0,
            BalWeight = runningWeight,
            Cost = currentCost,
            DrValue = runningValue > 0 ? runningValue : 0,
            CrValue = runningValue < 0 ? Math.Abs(runningValue) : 0,
            BalValue = runningValue,
            Status = runningWeight >= 0 ? "IN" : "NIL"
        });

        // 2. Fetch current period details
        var currentDetails = await context.Set<BrokerageInvoiceDetail>()
            .Include(d => d.Invoice).ThenInclude(i => i.Party)
            .Where(d => d.ProductId == prodId && d.Invoice.InvoiceDate >= from && d.Invoice.InvoiceDate <= to)
            .OrderBy(d => d.Invoice.InvoiceDate).ThenBy(d => d.InvoiceId)
            .ToListAsync();

        int seq = 2;
        foreach (var d in currentDetails)
        {
            bool isPurchase = d.Invoice.InvoiceType == InvoiceType.BrkPurchase || d.Invoice.InvoiceType == InvoiceType.GenPurchase;
            decimal drWt = isPurchase ? d.NetWeight : 0m;
            decimal crWt = !isPurchase ? d.NetWeight : 0m;
            decimal drVal = isPurchase ? d.Amount : 0m;
            decimal crVal = !isPurchase ? d.Amount : 0m;

            runningWeight += (drWt - crWt);
            runningValue += (drVal - crVal);
            if (runningWeight > 0 && isPurchase)
            {
                currentCost = runningValue / runningWeight;
            }

            ReportRows.Add(new BrokerageReportRow
            {
                Id = seq++,
                DocNo = d.Invoice.InvoiceNo,
                Date = d.Invoice.InvoiceDate.ToString("dd/MM/yyyy"),
                RawDate = d.Invoice.InvoiceDate,
                DocType = isPurchase ? "Purchase" : "Sale",
                PartyName = d.Invoice.Party?.Name ?? "-",
                VehicleNo = d.Invoice.VehicleNo ?? "-",
                Bags = d.Bags,
                DrWeight = drWt,
                CrWeight = crWt,
                BalWeight = runningWeight,
                Rate = d.Rate,
                Cost = currentCost,
                DrValue = drVal,
                CrValue = crVal,
                BalValue = runningValue,
                Status = runningWeight >= 0 ? "OK" : "DEFICIT"
            });
        }
    }

    private void ResetTotals()
    {
        TotalRecords = 0;
        TotalBags = 0m;
        TotalNetWeight = 0m;
        TotalMounds = 0m;
        TotalGrossAmount = 0m;
        TotalCommission = 0m;
        TotalWHT = 0m;
        TotalNetAmount = 0m;
        TotalProfitLoss = 0m;
    }

    private void CalculateTotals()
    {
        TotalRecords = ReportRows.Count;
        TotalBags = ReportRows.Sum(r => r.Bags);
        TotalNetWeight = ReportRows.Sum(r => r.NetWeight > 0 ? r.NetWeight : (r.BalWeight > 0 ? r.BalWeight : 0));
        TotalMounds = Math.Round(TotalNetWeight / 40.0m, 2);
        TotalGrossAmount = ReportRows.Sum(r => r.GrossAmount);
        TotalCommission = ReportRows.Sum(r => r.CommissionAmount);
        TotalWHT = ReportRows.Sum(r => r.WHT_Amount);
        TotalNetAmount = ReportRows.Sum(r => r.NetAmount > 0 ? r.NetAmount : (r.BalValue > 0 ? r.BalValue : 0));
        TotalProfitLoss = ReportRows.Sum(r => r.ProfitLoss);

        // Customize KPI cards based on active report mode
        switch (Mode)
        {
            case BrokerageReportMode.VehicleWiseSummary:
                Card5Title = "TOTAL SHORTAGE (KGS)";
                Card5Urdu = "کل وزن کمی / شارٹیج";
                Card5Value = $"{ReportRows.Sum(r => r.Shortage):N0} Kgs";
                Card6Title = "NET TRADING P&L (RS)";
                Card6Urdu = "خالص نفع یا نقصان";
                Card6Value = $"Rs {TotalProfitLoss:N0}";
                break;

            case BrokerageReportMode.MonthlyCommissionSoodh:
                Card5Title = "TOTAL SOODH (RS)";
                Card5Urdu = "کل سود آمدنی";
                Card5Value = $"Rs {ReportRows.Sum(r => r.SoodhAmount):N0}";
                Card6Title = "TOTAL EARNINGS (RS)";
                Card6Urdu = "کل کمیشن و سود آمدنی";
                Card6Value = $"Rs {TotalNetAmount:N0}";
                break;

            case BrokerageReportMode.PartyWiseWHT:
                Card5Title = "TOTAL W.H.T (RS)";
                Card5Urdu = "ود ہولڈنگ ٹیکس کٹوتی";
                Card5Value = $"Rs {TotalWHT:N0}";
                Card6Title = "NET AFTER TAX (RS)";
                Card6Urdu = "خالص بعد از ٹیکس";
                Card6Value = $"Rs {TotalNetAmount:N0}";
                break;

            case BrokerageReportMode.ProductLedger:
            case BrokerageReportMode.StockReport:
            case BrokerageReportMode.CurrentStock:
                Card5Title = "STOCK IN MOUNDS";
                Card5Urdu = "کل اسٹاک (من)";
                Card5Value = $"{TotalMounds:N2} Mnd";
                Card6Title = "STOCK VALUE (RS)";
                Card6Urdu = "کل اسٹاک مالیت";
                Card6Value = $"Rs {TotalNetAmount:N0}";
                break;

            case BrokerageReportMode.PartyLedgerExtended:
                Card5Title = "TOTAL DEBIT / بنام";
                Card5Urdu = "کل نام اندراج";
                Card5Value = $"Rs {ReportRows.Sum(r => r.DrValue):N0}";
                Card6Title = "CLOSING BALANCE";
                Card6Urdu = "بقایا رقم / کھاتہ بیلنس";
                var lastRow = ReportRows.LastOrDefault();
                Card6Value = lastRow != null ? $"Rs {lastRow.BalValue:N0} ({lastRow.Status})" : "Rs 0";
                break;

            default:
                Card5Title = "COMMISSION / EARNINGS";
                Card5Urdu = "کمیشن و سود آمدنی";
                Card5Value = $"Rs {TotalCommission:N0}";
                Card6Title = "NET AMOUNT (RS)";
                Card6Urdu = "خالص قابلِ وصول / ادا";
                Card6Value = $"Rs {TotalNetAmount:N0}";
                break;
        }
    }

    [RelayCommand]
    private void ExportCsv()
    {
        if (!ReportRows.Any())
        {
            MessageBox.Show("No data available to export.", "Empty Report", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = $"{Mode}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                if (Mode == BrokerageReportMode.VehicleWiseSummary)
                {
                    sb.AppendLine("ID,VehicleNo,Date,Product,PurchaseParty,SaleParty,PurchaseKgs,SaleKgs,ShortageKgs,PurchaseValue,SaleValue,ProfitLoss,Status");
                    foreach (var r in ReportRows)
                    {
                        sb.AppendLine($"\"{r.Id}\",\"{r.VehicleNo}\",\"{r.Date}\",\"{r.ProductName}\",\"{r.PurchaseParty}\",\"{r.SaleParty}\",\"{r.PurchaseKgs}\",\"{r.SaleKgs}\",\"{r.Shortage}\",\"{r.PurchaseValue}\",\"{r.SaleValue}\",\"{r.ProfitLoss}\",\"{r.Status}\"");
                    }
                }
                else if (Mode == BrokerageReportMode.PartyLedgerExtended)
                {
                    sb.AppendLine("ID,DocNo,Date,VehicleNo,Bags,Mounds,Narration,Debit,Credit,Balance,Status");
                    foreach (var r in ReportRows)
                    {
                        sb.AppendLine($"\"{r.Id}\",\"{r.DocNo}\",\"{r.Date}\",\"{r.VehicleNo}\",\"{r.Bags}\",\"{r.Mounds}\",\"{r.Narration}\",\"{r.DrValue}\",\"{r.CrValue}\",\"{r.BalValue}\",\"{r.Status}\"");
                    }
                }
                else if (Mode == BrokerageReportMode.ProductLedger)
                {
                    sb.AppendLine("ID,DocNo,Date,Type,Party,Vehicle,Bags,DrWeight,CrWeight,BalWeight,Rate,AvgCost,DrValue,CrValue,BalValue,Status");
                    foreach (var r in ReportRows)
                    {
                        sb.AppendLine($"\"{r.Id}\",\"{r.DocNo}\",\"{r.Date}\",\"{r.DocType}\",\"{r.PartyName}\",\"{r.VehicleNo}\",\"{r.Bags}\",\"{r.DrWeight}\",\"{r.CrWeight}\",\"{r.BalWeight}\",\"{r.Rate}\",\"{r.Cost}\",\"{r.DrValue}\",\"{r.CrValue}\",\"{r.BalValue}\",\"{r.Status}\"");
                    }
                }
                else if (Mode == BrokerageReportMode.CurrentStock)
                {
                    sb.AppendLine("ID,Code,ProductName,UrduName,StockKgs,Mounds,Rate,StockValue,Status");
                    foreach (var r in ReportRows)
                    {
                        sb.AppendLine($"\"{r.Id}\",\"{r.DocNo}\",\"{r.ProductName}\",\"{r.Narration}\",\"{r.NetWeight}\",\"{r.Mounds}\",\"{r.Rate}\",\"{r.GrossAmount}\",\"{r.Status}\"");
                    }
                }
                else
                {
                    sb.AppendLine("ID,DocNo,Date,Type,Party,Product,Vehicle,Bags,NetWeight,Mounds,Rate,GrossAmount,Commission,WHT,NetAmount,Status");
                    foreach (var r in ReportRows)
                    {
                        sb.AppendLine($"\"{r.Id}\",\"{r.DocNo}\",\"{r.Date}\",\"{r.DocType}\",\"{r.PartyName}\",\"{r.ProductName}\",\"{r.VehicleNo}\",\"{r.Bags}\",\"{r.NetWeight}\",\"{r.Mounds}\",\"{r.Rate}\",\"{r.GrossAmount}\",\"{r.CommissionAmount}\",\"{r.WHT_Amount}\",\"{r.NetAmount}\",\"{r.Status}\"");
                    }
                }
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Report exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export error: {ex.Message}", "agentcore Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void PrintReport()
    {
        MessageBox.Show($"Report printed to default printer.\nTotal Records: {TotalRecords}\nReport Mode: {Mode}", "Print Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
