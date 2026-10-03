using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class ProfitLossStatementViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly ISystemService _systemService;

    // Date Range
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Calculation Type
    [ObservableProperty] private bool _accordingToSetting = true;
    [ObservableProperty] private bool _onClosingBalanceType = false;

    // Checkboxes
    [ObservableProperty] private bool _withoutOpening = true;
    [ObservableProperty] private bool _profitIncludingCurrentStock = true;
    [ObservableProperty] private bool _currentStockBrokerage = false;
    [ObservableProperty] private bool _profitIncludingCurrentStockCrops = false;
    [ObservableProperty] private bool _profitIncludingCurrentStockPestro = false;

    public Action? RequestClose { get; set; }

    public ProfitLossStatementViewModel(IReportService reportService, ISystemService systemService)
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
        var data = await _reportService.GetProfitabilityReportAsync(FromDate.Date, ToDate.Date.AddDays(1).AddSeconds(-1), "Summary");

        var reportWindow = new ReportWindow(
            reportPath: "Reports/ProfitabilityReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        reportWindow.Title = $"Profit & Loss Statement ({FromDate.ToShortDateString()} To {ToDate.ToShortDateString()})";
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
