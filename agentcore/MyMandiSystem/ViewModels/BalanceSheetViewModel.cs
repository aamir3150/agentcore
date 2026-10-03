using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class BalanceSheetViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly ISystemService _systemService;

    // Date Filters
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isTillDate = false;
    [ObservableProperty] private bool _isDateRange = false;

    [ObservableProperty] private DateTime _tillDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Options Checkboxes
    [ObservableProperty] private bool _currentStock = false;
    [ObservableProperty] private bool _currentStockBrokerage = false;
    [ObservableProperty] private bool _currentStockCrops = false;
    [ObservableProperty] private bool _currentStockPestro = false;

    // Amount Filter
    [ObservableProperty] private string _amountGreaterThan = "";

    public Action? RequestClose { get; set; }

    public BalanceSheetViewModel(IReportService reportService, ISystemService systemService)
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
        DateTime to = IsAllDates ? DateTime.MaxValue : (IsTillDate ? TillDate.Date.AddDays(1).AddSeconds(-1) : ToDate.Date.AddDays(1).AddSeconds(-1));
        var data = await _reportService.GetProfitabilityReportAsync(DateTime.MinValue, to, "BalanceSheet");

        var reportWindow = new ReportWindow(
            reportPath: "Reports/ProfitabilityReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        reportWindow.Title = $"Balance Sheet - {(IsAllDates ? "All Dates" : (IsTillDate ? $"Till {TillDate.ToShortDateString()}" : $"{FromDate.ToShortDateString()} To {ToDate.ToShortDateString()}"))}";
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
