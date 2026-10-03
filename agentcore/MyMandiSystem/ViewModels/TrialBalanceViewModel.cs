using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class TrialBalanceViewModel : ObservableObject
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

    // Account Scope (Transaction Accounts, Account Groups, Both)
    [ObservableProperty] private bool _showOnlyTransactionAccounts = true;
    [ObservableProperty] private bool _showOnlyAccountsGroup = false;
    [ObservableProperty] private bool _showBothAccounts = false;

    // Amount Filter
    [ObservableProperty] private string _amountGreaterThan = "";

    // Checkboxes
    [ObservableProperty] private bool _excludeZeroBalance = true;
    [ObservableProperty] private bool _showTransactionAccountOnly = false;
    [ObservableProperty] private bool _isExtended = false;
    [ObservableProperty] private bool _urduPrint = false;

    // Parties Scope (Exclude, Include, Only)
    [ObservableProperty] private bool _excludeParties = false;
    [ObservableProperty] private bool _includeParties = true;
    [ObservableProperty] private bool _onlyParties = false;

    // Sorting (Name vs ID)
    [ObservableProperty] private bool _sortByAccountName = false;
    [ObservableProperty] private bool _sortByAccountId = true;

    public Action? RequestClose { get; set; }

    public TrialBalanceViewModel(IReportService reportService, ISystemService systemService)
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
        DateTime to = IsAllDates ? DateTime.MaxValue : (IsSingleDate ? SingleDate.Date.AddDays(1).AddSeconds(-1) : ToDate.Date.AddDays(1).AddSeconds(-1));
        var data = await _reportService.GetDayBookReportAsync(to); // Or Trial Balance generation

        var reportWindow = new ReportWindow(
            reportPath: "Reports/DayBookReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        reportWindow.Title = $"Trial Balance - {(IsAllDates ? "All Dates" : (IsSingleDate ? SingleDate.ToShortDateString() : $"{FromDate.ToShortDateString()} To {ToDate.ToShortDateString()}"))}";
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
