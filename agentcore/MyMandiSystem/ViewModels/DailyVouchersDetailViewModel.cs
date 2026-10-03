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

    // Date Filters
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isSingleDate = false;
    [ObservableProperty] private bool _isDateRange = false;

    [ObservableProperty] private DateTime _singleDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Checkboxes
    [ObservableProperty] private bool _includeBankVouchersAlso = true;
    [ObservableProperty] private bool _excludeAccountsVouchers = false;
    [ObservableProperty] private bool _includeInvoices = true;

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
            "Cash Receiving Voucher (CRV)",
            "Cash Payment Voucher (CPV)",
            "Journal Voucher (JV)",
            "Bank Cheque Deposit (BDV)",
            "Bank Cheque Issue (BPV)",
            "Cash Payment WHT"
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
        DateTime date = IsSingleDate ? SingleDate.Date : (IsDateRange ? ToDate.Date : DateTime.Today);
        var data = await _reportService.GetDayBookReportAsync(date);

        var reportWindow = new ReportWindow(
            reportPath: "Reports/DayBookReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        reportWindow.Title = $"Daily Vouchers Detail - {(IsAllDates ? "All Dates" : (IsSingleDate ? SingleDate.ToShortDateString() : $"{FromDate.ToShortDateString()} To {ToDate.ToShortDateString()}"))}";
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
