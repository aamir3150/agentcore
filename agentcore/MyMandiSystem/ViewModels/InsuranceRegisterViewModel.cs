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

public partial class InsuranceRegisterViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _windowTitle = "Insurance Register - انشورنس رجسٹر";
    [ObservableProperty] private string _headerTitle = "Insurance Register";
    [ObservableProperty] private string _headerSubtitle = "فصل و بیوپار انشورنس پالیسی و پریمیئم رجسٹر";

    // Date Filters
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isDateRange = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Status Filters
    [ObservableProperty] private bool _showAll = true;
    [ObservableProperty] private bool _showExpiredOnly = false;
    [ObservableProperty] private bool _showNearExpireOnly = false;
    [ObservableProperty] private int _nearExpireDays = 30;

    public Action? RequestClose { get; set; }

    public InsuranceRegisterViewModel(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
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
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.InsuranceEntries
                .Include(i => i.Party)
                .Include(i => i.Product)
                .AsQueryable();

            if (!IsAllDates)
            {
                query = query.Where(i => i.EntryDate >= FromDate.Date && i.EntryDate <= ToDate.Date.AddDays(1).AddSeconds(-1));
            }

            var list = await query.ToListAsync();

            var today = DateTime.Today;
            var reportRows = list.Select(i =>
            {
                var isExpired = !i.IsActive;
                return new
                {
                    i.Id,
                    Date = i.EntryDate.ToString("dd/MM/yyyy"),
                    PartyName = i.Party?.Name ?? "General",
                    ProductName = i.Product?.Name ?? "-",
                    i.PolicyNo,
                    Rate = i.InsuranceRate,
                    Premium = i.PremiumAmount,
                    Status = i.IsActive ? "ACTIVE" : "EXPIRED"
                };
            }).ToList();

            var reportWindow = new ReportWindow(
                reportPath: "Reports/InsuranceRegisterReport.rdlc",
                dataSourceName: "DS_Insurance",
                data: reportRows
            );
            reportWindow.Title = $"{HeaderTitle} - {(ShowExpiredOnly ? "Expired Policies" : "All Policies")}";
            reportWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Report generation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task PrintAsync() => await PreviewAsync();

    [RelayCommand]
    private void Close() => RequestClose?.Invoke();
}
