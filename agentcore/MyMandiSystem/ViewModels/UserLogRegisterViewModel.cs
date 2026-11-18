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

public partial class UserLogRegisterViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _windowTitle = "User Log Register - یوزر لاگ رجسٹر";
    [ObservableProperty] private string _headerTitle = "User Log Register";
    [ObservableProperty] private string _headerSubtitle = "صارفین کی سرگرمی اور آڈٹ ٹریل لاگ رجسٹر";

    // Date Filters
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isDateRange = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Filters
    [ObservableProperty] private ObservableCollection<string> _usersList = new();
    [ObservableProperty] private string _selectedUser = "-- ALL USERS --";

    [ObservableProperty] private ObservableCollection<string> _actionTypes = new();
    [ObservableProperty] private string _selectedActionType = "-- ALL ACTIONS --";

    public Action? RequestClose { get; set; }

    public UserLogRegisterViewModel(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var usernames = new List<string>();
            try
            {
                usernames = await context.Database.SqlQueryRaw<string>(
                    "IF OBJECT_ID('Users', 'U') IS NOT NULL SELECT CAST(UserName AS VARCHAR(50)) AS Value FROM Users ELSE SELECT CAST('Admin' AS VARCHAR(50)) AS Value"
                ).ToListAsync();
            }
            catch
            {
                usernames = new List<string> { "Admin" };
            }
            UsersList = new ObservableCollection<string> { "-- ALL USERS --" };
            foreach (var u in usernames) UsersList.Add(u);
            SelectedUser = "-- ALL USERS --";

            ActionTypes = new ObservableCollection<string>
            {
                "-- ALL ACTIONS --",
                "Login",
                "Create",
                "Update",
                "Delete",
                "Post",
                "Cancel"
            };
            SelectedActionType = "-- ALL ACTIONS --";

            var year = await _systemService.GetCurrentYearAsync();
            if (year != null)
            {
                FromDate = year.StartDate;
                ToDate = DateTime.Today < year.EndDate ? DateTime.Today : year.EndDate;
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task PreviewAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.UserLogs
                .Include(l => l.User)
                .AsQueryable();

            if (!IsAllDates)
            {
                query = query.Where(l => l.Timestamp >= FromDate.Date && l.Timestamp <= ToDate.Date.AddDays(1).AddSeconds(-1));
            }

            if (SelectedUser != "-- ALL USERS --")
            {
                query = query.Where(l => l.User.Username == SelectedUser);
            }

            if (SelectedActionType != "-- ALL ACTIONS --")
            {
                query = query.Where(l => l.Action.Contains(SelectedActionType));
            }

            var logs = await query.OrderByDescending(l => l.Timestamp).Take(500).ToListAsync();

            var reportRows = logs.Select(l => new
            {
                l.Id,
                DateTime = l.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"),
                UserName = l.User?.Username ?? "System",
                l.Action,
                l.EntityType,
                EntityId = l.EntityId?.ToString() ?? "-",
                IPAddress = l.IPAddress ?? "Localhost",
                Details = l.NewValues ?? l.OldValues ?? ""
            }).ToList();

            var reportWindow = new ReportWindow(
                reportPath: "Reports/UserLogReport.rdlc",
                dataSourceName: "DS_UserLog",
                data: reportRows
            );
            reportWindow.Title = $"{HeaderTitle} - {SelectedUser} ({SelectedActionType})";
            reportWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Log report error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task PrintAsync() => await PreviewAsync();

    [RelayCommand]
    private void Close() => RequestClose?.Invoke();
}
