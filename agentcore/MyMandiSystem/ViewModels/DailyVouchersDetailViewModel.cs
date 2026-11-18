using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class DailyVouchersDetailViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _windowTitle = "Daily Vouchers Detail";
    [ObservableProperty] private string _headerTitle = "Daily Vouchers Detail";
    [ObservableProperty] private string _headerSubtitle = "Detailed Register of Daily Cash, Bank & Journal Vouchers";

    // Date Filters
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isSingleDate = false;
    [ObservableProperty] private bool _isDateRange = false;

    [ObservableProperty] private DateTime _singleDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    public void SetGLJournalMode()
    {
        WindowTitle = "G/L Journal";
        HeaderTitle = "G/L Journal Register";
        HeaderSubtitle = "General Ledger Journal Vouchers & Double Entry Register";
        SelectedVoucherType = "Journal Voucher (JV)";
    }

    // Checkboxes
    [ObservableProperty] private bool _includeBankVouchersAlso = true;
    [ObservableProperty] private bool _excludeAccountsVouchers = false;
    [ObservableProperty] private bool _includeInvoices = true;

    // Voucher Number Filter Range (AgenSoft Specification)
    [ObservableProperty] private string _fromVoucherNo = "";
    [ObservableProperty] private string _toVoucherNo = "";

    // Voucher Type
    [ObservableProperty] private ObservableCollection<string> _voucherTypes = new();
    [ObservableProperty] private string _selectedVoucherType = "--ALL TYPES--";

    public Action? RequestClose { get; set; }

    public DailyVouchersDetailViewModel(IReportService reportService, ISystemService systemService)
    {
        _reportService = reportService;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        VoucherTypes = new ObservableCollection<string>
        {
            "--ALL TYPES--",
            "Credit Voucher (CV)",
            "Debit Voucher (DV)",
            "Journal Voucher (JV)",
            "Bank Issue (BC)",
            "Bank Deposit (BD)",
            "Brokerage Purchase (BP)",
            "Brokerage Sale (BS)",
            "General Purchase (PI)",
            "General Purchase Return (PR)",
            "General Sale (SI)",
            "General Sale Return (SR)",
            "Pestro Purchase (PPI)",
            "Pestro Purchase Return (PPR)",
            "Pestro Sale (PSI)",
            "Pestro Sale Return (PSR)"
        };
        SelectedVoucherType = "--ALL TYPES--";

        var year = await _systemService.GetCurrentYearAsync();
        if (year != null)
        {
            FromDate = year.StartDate;
            ToDate = DateTime.Today < year.EndDate ? DateTime.Today : year.EndDate;
        }
    }

    [RelayCommand]
    private async Task PreviewAsync()
    {
        DateTime? from = IsAllDates ? null : (IsSingleDate ? SingleDate.Date : FromDate.Date);
        DateTime? to = IsAllDates ? null : (IsSingleDate ? SingleDate.Date.AddDays(1).AddSeconds(-1) : ToDate.Date.AddDays(1).AddSeconds(-1));

        int? fromV = int.TryParse(FromVoucherNo, out int f) ? f : null;
        int? toV = int.TryParse(ToVoucherNo, out int t) ? t : null;

        var data = await _reportService.GetDailyVouchersDetailReportAsync(
            from, to, SelectedVoucherType, fromV, toV, ExcludeAccountsVouchers, IncludeInvoices);

        var reportWindow = new ReportWindow(
            reportPath: "Reports/DailyVouchersDetailReport.rdlc",
            dataSourceName: "DS_Vouchers",
            data: data
        );
        reportWindow.Title = $"DAILY VOCHURE DETAILS - {(IsAllDates ? "All Dates" : (IsSingleDate ? SingleDate.ToString("dd/MM/yyyy") : $"{FromDate:dd/MM/yyyy} To {ToDate:dd/MM/yyyy}"))}";
        reportWindow.ShowDialog();
    }

    [RelayCommand]
    private async Task PrintAsync()
    {
        await PreviewAsync();
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
