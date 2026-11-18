using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class CashBookViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _windowTitle = "Cash Book";
    [ObservableProperty] private string _headerTitle = "Cash Book";
    [ObservableProperty] private string _headerSubtitle = "Daily Cash Receipts & Payments Register";

    public void SetDayBookMode(bool isDayBook)
    {
        if (isDayBook)
        {
            WindowTitle = "Day Book";
            HeaderTitle = "Day Book";
            HeaderSubtitle = "Day Book Financial Journal & Daily Transactions";
            IsSingleDate = true;
            IsAllDates = false;
        }
        else
        {
            WindowTitle = "Cash Book";
            HeaderTitle = "Cash Book";
            HeaderSubtitle = "Daily Cash Receipts & Payments Register";
        }
    }

    // Date Filters
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isSingleDate = false;
    [ObservableProperty] private bool _isDateRange = false;

    [ObservableProperty] private DateTime _singleDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Options Checkboxes
    [ObservableProperty] private bool _showDefaultCashBookView = false;
    [ObservableProperty] private bool _printCreditOnly = false;
    [ObservableProperty] private bool _printDebitOnly = false;
    [ObservableProperty] private bool _brokerageInvoiceSeparateIDs = false;

    public Action? RequestClose { get; set; }

    public CashBookViewModel(IReportService reportService, ISystemService systemService)
    {
        _reportService = reportService;
        _systemService = systemService;

        _ = InitializeDatesAsync();
    }

    private async Task InitializeDatesAsync()
    {
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

        var data = await _reportService.GetCashBookReportAsync(from, to, ShowDefaultCashBookView);

        var reportWindow = new ReportWindow(
            reportPath: "Reports/CashBookReport.rdlc",
            dataSourceName: "DS_CashBook",
            data: data
        );
        reportWindow.Title = $"Cash Book - {(IsAllDates ? "All Dates" : (IsSingleDate ? SingleDate.ToString("dd/MM/yyyy") : $"{FromDate:dd/MM/yyyy} To {ToDate:dd/MM/yyyy}"))}";
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
