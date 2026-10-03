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
        DateTime date = IsSingleDate ? SingleDate.Date : (IsDateRange ? ToDate.Date : DateTime.Today);
        var data = await _reportService.GetDayBookReportAsync(date);

        var reportWindow = new ReportWindow(
            reportPath: "Reports/DayBookReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        reportWindow.Title = $"Cash Book Report - {(IsAllDates ? "All Dates" : (IsSingleDate ? SingleDate.ToShortDateString() : $"{FromDate.ToShortDateString()} To {ToDate.ToShortDateString()}"))}";
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
