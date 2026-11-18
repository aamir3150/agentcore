using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Infrastructure;
using MyMandiSystem.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class ExpenseMonthSummary
{
    public string AccountNo { get; set; } = "";
    public string AccountName { get; set; } = "";
    public decimal TotalExpense { get; set; }
}

public partial class MonthlyExpenseChartViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _windowTitle = "Monthly Expense Chart - ماہانہ اخراجات چارٹ";
    [ObservableProperty] private string _headerTitle = "Monthly Expense Chart";
    [ObservableProperty] private string _headerSubtitle = "ماہ بہ ماہ اخراجات کا تقابلی جائزہ و چارٹ رپورٹ";

    // Date Filters
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    [ObservableProperty] private decimal _grandTotalExpense = 0m;
    [ObservableProperty] private ObservableCollection<ExpenseMonthSummary> _expenses = new();

    public Action? RequestClose { get; set; }

    public MonthlyExpenseChartViewModel(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        var year = await _systemService.GetCurrentYearAsync();
        if (year != null)
        {
            FromDate = year.StartDate;
            ToDate = DateTime.Today < year.EndDate ? DateTime.Today : year.EndDate;
        }

        await LoadExpensesAsync();
    }

    [RelayCommand]
    private async Task LoadExpensesAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            // Get all Expense accounts
            var expenseAccounts = await context.Accounts
                .Where(a => a.AccountType == AccountType.Expense)
                .ToListAsync();

            var expenseAccountIds = expenseAccounts.Select(a => a.Id).ToList();

            var ledgerEntries = await context.GeneralLedger
                .Where(g => expenseAccountIds.Contains(g.AccountId) && g.TransactionDate >= FromDate.Date && g.TransactionDate <= ToDate.Date.AddDays(1).AddSeconds(-1))
                .GroupBy(g => g.AccountId)
                .Select(grp => new
                {
                    AccountId = grp.Key,
                    TotalDebit = grp.Sum(x => x.Debit),
                    TotalCredit = grp.Sum(x => x.Credit)
                })
                .ToListAsync();

            var list = new ObservableCollection<ExpenseMonthSummary>();
            decimal grandTotal = 0m;

            foreach (var acc in expenseAccounts)
            {
                var entry = ledgerEntries.FirstOrDefault(e => e.AccountId == acc.Id);
                decimal netExpense = entry != null ? (entry.TotalDebit - entry.TotalCredit) : 0m;
                if (netExpense != 0)
                {
                    list.Add(new ExpenseMonthSummary
                    {
                        AccountNo = acc.AccountNo,
                        AccountName = acc.Name,
                        TotalExpense = netExpense
                    });
                    grandTotal += netExpense;
                }
            }

            Expenses = new ObservableCollection<ExpenseMonthSummary>(list.OrderByDescending(x => x.TotalExpense));
            GrandTotalExpense = grandTotal;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading expenses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task PreviewAsync()
    {
        await LoadExpensesAsync();
        var reportWindow = new ReportWindow(
            reportPath: "Reports/MonthlyExpenseReport.rdlc",
            dataSourceName: "DS_Expenses",
            data: Expenses
        );
        reportWindow.Title = $"{HeaderTitle} - {FromDate:dd/MM/yyyy} to {ToDate:dd/MM/yyyy} [Total: Rs. {GrandTotalExpense:N2}]";
        reportWindow.ShowDialog();
    }

    [RelayCommand]
    private async Task PrintAsync() => await PreviewAsync();

    [RelayCommand]
    private void Close() => RequestClose?.Invoke();
}
