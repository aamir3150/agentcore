using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class ReportingViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IAccountService _accountService;

    [ObservableProperty] private DateTime _fromDate = DateTime.Today.AddDays(-30);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;
    [ObservableProperty] private Account? _selectedAccount;
    [ObservableProperty] private int _daysToExpiry = 30;
    [ObservableProperty] private ObservableCollection<Account> _accounts = new();

    public ReportingViewModel(IReportService reportService, IAccountService accountService)
    {
        _reportService = reportService;
        _accountService = accountService;
        _ = LoadAccountsAsync();
    }

    private async Task LoadAccountsAsync()
    {
        Accounts = new ObservableCollection<Account>(await _accountService.GetAllAccountsAsync());
    }

    [RelayCommand]
    private async Task ViewLedgerReportAsync()
    {
        if (SelectedAccount == null)
        {
            System.Windows.MessageBox.Show("Please select an account.");
            return;
        }

        var data = await _reportService.GetLedgerReportAsync(SelectedAccount.Id, FromDate, ToDate);
        var window = new ReportWindow(
            reportPath: "Reports/LedgerReport.rdlc",
            dataSourceName: "DS_Ledger",
            data: data
        );
        window.ShowDialog();
    }

    [RelayCommand]
    private async Task ViewDayBookReportAsync()
    {
        var data = await _reportService.GetDayBookReportAsync(ToDate);
        var window = new ReportWindow(
            reportPath: "Reports/DayBookReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Day-Book: " + ToDate.ToShortDateString();
        window.Show();
    }

    [RelayCommand]
    private async Task ViewStockReportAsync()
    {
        var data = await _reportService.GetStockReportAsync(null);
        var window = new ReportWindow(
            reportPath: "Reports/StockReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Current Stock Summary";
        window.Show();
    }

    [RelayCommand]
    private async Task ViewPurchaseRegisterAsync()
    {
        var data = await _reportService.GetPurchaseRegisterAsync(FromDate, ToDate);
        var window = new ReportWindow(
            reportPath: "Reports/TradeRegister.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Purchase Register: " + FromDate.ToShortDateString() + " To " + ToDate.ToShortDateString();
        window.Show();
    }

    [RelayCommand]
    private async Task ViewSaleRegisterAsync()
    {
        var data = await _reportService.GetSaleRegisterAsync(FromDate, ToDate);
        var window = new ReportWindow(
            reportPath: "Reports/TradeRegister.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Sale Register: " + FromDate.ToShortDateString() + " To " + ToDate.ToShortDateString();
        window.Show();
    }

    [RelayCommand]
    private async Task ViewCommissionReportAsync()
    {
        var data = await _reportService.GetBrokerageCommissionAsync(FromDate, ToDate);
        var window = new ReportWindow(
            reportPath: "Reports/CommissionReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Commission Report: " + FromDate.ToShortDateString() + " To " + ToDate.ToShortDateString();
        window.Show();
    }

    [RelayCommand]
    private async Task ViewGatePassRegisterAsync()
    {
        var data = await _reportService.GetGatePassRegisterAsync(FromDate, ToDate);
        var window = new ReportWindow(
            reportPath: "Reports/GatePassReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Gate Pass Register";
        window.Show();
    }

    [RelayCommand]
    private async Task ViewPestroExpiryReportAsync()
    {
        var data = await _reportService.GetPestroExpiryReportAsync(DaysToExpiry);
        var window = new ReportWindow(
            reportPath: "Reports/PestroExpiryReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Pestro Expiry Forecast (" + DaysToExpiry + " Days)";
        window.Show();
    }

    [RelayCommand]
    private async Task ViewProfitabilityReportAsync()
    {
        var data = await _reportService.GetProfitabilityReportAsync(FromDate, ToDate, "Summary");
        var window = new ReportWindow(
            reportPath: "Reports/ProfitabilityReport.rdlc",
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Gross Profit Summary";
        window.Show();
    }

    [RelayCommand]
    private async Task ViewPartiesListAsync()
    {
        var data = await _reportService.GetMasterListAsync("Parties");
        var window = new ReportWindow(
            reportPath: "Reports/TradeRegister.rdlc", // Reusing generic grid template
            dataSourceName: "DataSet1",
            data: data
        );
        window.Title = "Active Party List";
        window.Show();
    }
}
