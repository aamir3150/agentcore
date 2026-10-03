using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class AccountPayablesViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IPartyService _partyService;
    private readonly ICatalogService _catalogService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _windowTitle = "Account Payables";
    [ObservableProperty] private string _headerTitle = "Account Payables";

    // Date Filters
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isTillDate = false;
    [ObservableProperty] private bool _isDateRange = false;

    [ObservableProperty] private DateTime _tillDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Crop Season
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = "All Crops";

    // Party Filters
    [ObservableProperty] private ObservableCollection<string> _partyTypes = new();
    [ObservableProperty] private string _selectedPartyType = "------- All Parties -------";

    [ObservableProperty] private ObservableCollection<string> _partyGroups = new();
    [ObservableProperty] private string _selectedPartyGroup = "--- ALL ---";

    [ObservableProperty] private string _amountGreaterThan = "";

    [ObservableProperty] private ObservableCollection<string> _cities = new();
    [ObservableProperty] private string _selectedCity = "------- All Cities -------";

    [ObservableProperty] private ObservableCollection<string> _towns = new();
    [ObservableProperty] private string _selectedTown = "--- ALL TOWNS ---";

    [ObservableProperty] private ObservableCollection<string> _sectors = new();
    [ObservableProperty] private string _selectedSector = "--- ALL SECTORS ---";

    // Sorting & Options
    [ObservableProperty] private bool _isSortByAccountName = true;
    [ObservableProperty] private bool _isSortByAccountId = false;
    [ObservableProperty] private bool _showOnlyFinalBalance = false;
    [ObservableProperty] private bool _urduPrint = false;
    [ObservableProperty] private bool _sendSMS = false;

    public Action? RequestClose { get; set; }

    public AccountPayablesViewModel(
        IReportService reportService, 
        IPartyService partyService, 
        ICatalogService catalogService, 
        ISystemService systemService)
    {
        _reportService = reportService;
        _partyService = partyService;
        _catalogService = catalogService;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        // 1. Seasons
        CropSeasons = new ObservableCollection<string> { "All Crops", "Kharif 2025", "Rabi 2025-26", "Kharif 2026" };
        SelectedCropSeason = "All Crops";

        // 2. Party Types
        PartyTypes = new ObservableCollection<string> 
        { 
            "------- All Parties -------", 
            "Vendors", 
            "Customers", 
            "Commission Agents", 
            "Brokers" 
        };
        SelectedPartyType = "------- All Parties -------";

        // 3. Party Groups
        var groups = await _partyService.GetPartyGroupsAsync();
        var groupNames = new List<string> { "--- ALL ---" };
        groupNames.AddRange(groups.Select(g => g.Name));
        PartyGroups = new ObservableCollection<string>(groupNames);
        SelectedPartyGroup = "--- ALL ---";

        // 4. Towns
        var towns = await _catalogService.GetTownsAsync();
        var townNames = new List<string> { "--- ALL TOWNS ---" };
        townNames.AddRange(towns.Select(t => t.Name));
        Towns = new ObservableCollection<string>(townNames);
        SelectedTown = "--- ALL TOWNS ---";

        // 5. Cities / Regions
        var cityList = new List<string> { "------- All Cities -------", "Lahore", "Faisalabad", "Multan", "Okara", "Sahiwal", "Sargodha", "Pakpattan" };
        Cities = new ObservableCollection<string>(cityList);
        SelectedCity = "------- All Cities -------";

        // 6. Sectors
        var sectors = await _catalogService.GetSectorsAsync();
        var sectorNames = new List<string> { "--- ALL SECTORS ---" };
        sectorNames.AddRange(sectors.Select(s => s.Name));
        Sectors = new ObservableCollection<string>(sectorNames);
        SelectedSector = "--- ALL SECTORS ---";

        // 7. Dates
        var currentYear = await _systemService.GetCurrentYearAsync();
        if (currentYear != null)
        {
            FromDate = currentYear.StartDate;
            ToDate = DateTime.Today < currentYear.EndDate ? DateTime.Today : currentYear.EndDate;
        }
    }

    [RelayCommand]
    private async Task PreviewAsync()
    {
        DateTime from = IsAllDates ? DateTime.MinValue : (IsTillDate ? DateTime.MinValue : FromDate.Date);
        DateTime to = IsAllDates ? DateTime.MaxValue : (IsTillDate ? TillDate.Date.AddDays(1).AddSeconds(-1) : ToDate.Date.AddDays(1).AddSeconds(-1));

        // Fetch Parties data
        var parties = await _partyService.GetAllPartiesAsync();
        bool isReceivable = HeaderTitle.Contains("Receivable", StringComparison.OrdinalIgnoreCase);

        var filtered = isReceivable 
            ? parties.Where(p => p.PartyType == PartyType.Customer || p.PartyType == PartyType.Both).ToList()
            : parties.Where(p => p.PartyType == PartyType.Vendor || p.PartyType == PartyType.Both).ToList();

        if (SelectedPartyType != "------- All Parties -------")
        {
            if (SelectedPartyType == "Vendors") filtered = parties.Where(p => p.PartyType == PartyType.Vendor).ToList();
            else if (SelectedPartyType == "Customers") filtered = parties.Where(p => p.PartyType == PartyType.Customer).ToList();
            else if (SelectedPartyType == "Commission Agents") filtered = parties.Where(p => p.PartyType == PartyType.Both).ToList();
        }

        if (SelectedTown != "--- ALL TOWNS ---")
        {
            filtered = filtered.Where(p => p.Town?.Name == SelectedTown).ToList();
        }

        if (SelectedSector != "--- ALL SECTORS ---")
        {
            filtered = filtered.Where(p => p.Sector?.Name == SelectedSector).ToList();
        }

        decimal minAmount = 0;
        if (decimal.TryParse(AmountGreaterThan, out decimal parsedAmount))
        {
            minAmount = parsedAmount;
        }

        if (minAmount > 0)
        {
            filtered = filtered.Where(p => (p.Account?.CurrentBalance ?? 0) >= minAmount).ToList();
        }

        if (IsSortByAccountId)
        {
            filtered = filtered.OrderBy(p => p.PartyNo).ToList();
        }
        else
        {
            filtered = filtered.OrderBy(p => p.Name).ToList();
        }

        var reportData = filtered.Select(p => new
        {
            PartyNo = p.PartyNo,
            Name = p.Name,
            UrduName = p.UrduName,
            City = p.Address ?? "",
            Town = p.Town?.Name ?? "",
            Phone = p.Phone ?? "",
            CurrentBalance = p.Account?.CurrentBalance ?? 0
        }).ToList();

        var reportWindow = new ReportWindow(
            reportPath: "Reports/TradeRegister.rdlc",
            dataSourceName: "DataSet1",
            data: reportData
        );
        reportWindow.Title = $"{WindowTitle} Summary - Total: {filtered.Count} Accounts";
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
