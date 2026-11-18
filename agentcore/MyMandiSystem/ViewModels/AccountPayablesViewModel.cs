using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Printing;
using MyMandiSystem.Views;

namespace MyMandiSystem.ViewModels;

/// <summary>
/// ViewModel for AgenSoft's RptAccountPayables form (supports both Payables and Receivables modes).
/// </summary>
public partial class AccountPayablesViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IPartyService _partyService;
    private readonly ICatalogService _catalogService;
    private readonly ISystemService _systemService;
    private bool _isInitializing;

    public Action? RequestClose { get; set; }
    public Action? FocusFromDate { get; set; }

    [ObservableProperty] private string _windowTitle = "Account Payables";
    [ObservableProperty] private string _headerTitle = "Account Payables";
    [ObservableProperty] private bool _isBusy;

    // Date Mode Radios (AgenSoft defaults: OptAllDates = True, DtpSingleDate/FrmDateRange hidden)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSingleDatePicker))]
    [NotifyPropertyChangedFor(nameof(ShowDateRangeFrame))]
    [NotifyPropertyChangedFor(nameof(ShowFinalBalancesCheckbox))]
    [NotifyPropertyChangedFor(nameof(DateModeDisplayText))]
    private bool _isAllDates = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSingleDatePicker))]
    [NotifyPropertyChangedFor(nameof(ShowDateRangeFrame))]
    [NotifyPropertyChangedFor(nameof(ShowFinalBalancesCheckbox))]
    [NotifyPropertyChangedFor(nameof(DateModeDisplayText))]
    private bool _isTillDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSingleDatePicker))]
    [NotifyPropertyChangedFor(nameof(ShowDateRangeFrame))]
    [NotifyPropertyChangedFor(nameof(ShowFinalBalancesCheckbox))]
    [NotifyPropertyChangedFor(nameof(DateModeDisplayText))]
    private bool _isDateRange;

    [ObservableProperty] private DateTime _singleDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Conditional Crop Season group (visible only when EnableCropSeason = 1)
    [ObservableProperty] private bool _showCropSeason;
    public ObservableCollection<LookupOption> CropSeasons { get; } = new();
    [ObservableProperty] private LookupOption? _selectedCropSeason;

    // 2-Column Filters
    public ObservableCollection<LookupOption> PartyTypes { get; } = new();
    [ObservableProperty] private LookupOption? _selectedPartyType;

    public ObservableCollection<LookupOption> PartyGroups { get; } = new();
    [ObservableProperty] private LookupOption? _selectedPartyGroup;

    [ObservableProperty] private string _greaterThanAmountText = "0";

    public ObservableCollection<string> Cities { get; } = new();
    [ObservableProperty] private string? _selectedCity;

    public ObservableCollection<LookupOption> Towns { get; } = new();
    [ObservableProperty] private LookupOption? _selectedTown;

    public ObservableCollection<LookupOption> Sectors { get; } = new();
    [ObservableProperty] private LookupOption? _selectedSector;

    // Sorting Radios (AgenSoft Frame3: OptByName = True default, OptByAccID = False)
    [ObservableProperty] private bool _sortByName = true;
    [ObservableProperty] private bool _sortByAccountId;

    // Report Options & Checkboxes
    [ObservableProperty] private bool _showOnlyFinalBalance; // ChkOnlyFinals (always visible)
    [ObservableProperty] private bool _showFinalBalances;    // ChkFinalBalances (visible only in Till Date / Date Range)
    [ObservableProperty] private bool _showUrduCheckbox;     // ChkUrdu (visible only when ShowNameInUrdu = '1')
    [ObservableProperty] private bool _isUrduPrint;
    [ObservableProperty] private bool _sendSms;              // ChksmsEnabled (always visible)

    public bool IsReceivable =>
        HeaderTitle.Contains("Receivable", StringComparison.OrdinalIgnoreCase) ||
        WindowTitle.Contains("Receivable", StringComparison.OrdinalIgnoreCase);

    public string HeaderSubtitle => IsReceivable
        ? "Customer & Party Receivables Statement, Balance Summary & Location Filters"
        : "Party Payables Statement, Balance Summary & Location Filters";

    public string ModeBadgeText => IsReceivable ? "RECEIVABLES (DR)" : "PAYABLES (CR)";

    public bool ShowSingleDatePicker => IsTillDate;
    public bool ShowDateRangeFrame => IsDateRange;
    public bool ShowFinalBalancesCheckbox => IsTillDate || IsDateRange;

    public string DateModeDisplayText =>
        IsAllDates ? "All Dates" : (IsTillDate ? $"Till Date ({SingleDate:dd/MM/yyyy})" : $"Date Range ({FromDate:dd/MM/yyyy} - {ToDate:dd/MM/yyyy})");

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

    partial void OnWindowTitleChanged(string value) => NotifyModeProperties();
    partial void OnHeaderTitleChanged(string value) => NotifyModeProperties();

    partial void OnSingleDateChanged(DateTime value) => OnPropertyChanged(nameof(DateModeDisplayText));
    partial void OnFromDateChanged(DateTime value) => OnPropertyChanged(nameof(DateModeDisplayText));
    partial void OnToDateChanged(DateTime value) => OnPropertyChanged(nameof(DateModeDisplayText));

    private void NotifyModeProperties()
    {
        OnPropertyChanged(nameof(IsReceivable));
        OnPropertyChanged(nameof(HeaderSubtitle));
        OnPropertyChanged(nameof(ModeBadgeText));
    }

    /// <summary>
    /// Mirrors AgenSoft ChkFinalBalances_Click (0x112a83b0): checking Show Final Balances unchecks Show Only Final Balance.
    /// </summary>
    partial void OnShowFinalBalancesChanged(bool value)
    {
        if (value)
        {
            ShowOnlyFinalBalance = false;
        }
    }

    /// <summary>
    /// Mirrors AgenSoft CmbTown_Click (0x112a84d0): clears and reloads Sectors for the selected Town.
    /// </summary>
    partial void OnSelectedTownChanged(LookupOption? value)
    {
        if (_isInitializing) return;
        _ = LoadSectorsForTownAsync(value?.Id);
    }

    private async Task InitializeDataAsync()
    {
        _isInitializing = true;
        IsBusy = true;

        try
        {
            var settingsTask = _reportService.GetPayablesFormSettingsAsync();
            var partyTypesTask = _reportService.GetPayablePartyTypesAsync();
            var partyGroupsTask = _reportService.GetPartyGroupsLookupAsync();
            var citiesTask = _reportService.GetPartyCitiesLookupAsync();
            var townsTask = _reportService.GetTownsLookupAsync();

            await Task.WhenAll(settingsTask, partyTypesTask, partyGroupsTask, citiesTask, townsTask);

            var settings = settingsTask.Result;
            FromDate = settings.FinancialYearStartDate;
            ToDate = DateTime.Today;
            SingleDate = DateTime.Today;
            ShowUrduCheckbox = settings.ShowUrduPrint;
            ShowCropSeason = settings.EnableCropSeason;

            ReplaceOptions(PartyTypes, partyTypesTask.Result);
            SelectedPartyType = PartyTypes.FirstOrDefault();

            ReplaceOptions(PartyGroups, partyGroupsTask.Result);
            SelectedPartyGroup = PartyGroups.FirstOrDefault();

            Cities.Clear();
            foreach (var city in citiesTask.Result)
            {
                Cities.Add(city);
            }
            SelectedCity = Cities.FirstOrDefault();

            ReplaceOptions(Towns, townsTask.Result);
            SelectedTown = Towns.FirstOrDefault();

            await LoadSectorsForTownAsync(SelectedTown?.Id);

            if (ShowCropSeason)
            {
                var seasons = await _reportService.GetPayablesCropSeasonsAsync();
                ReplaceOptions(CropSeasons, seasons);
                SelectedCropSeason = CropSeasons.FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load filter options: {ex.Message}", HeaderTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isInitializing = false;
            IsBusy = false;
        }
    }

    private async Task LoadSectorsForTownAsync(int? townId)
    {
        try
        {
            var sectors = await _reportService.GetSectorsByTownLookupAsync(townId);
            ReplaceOptions(Sectors, sectors);
            SelectedSector = Sectors.FirstOrDefault();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load sectors: {ex.Message}", HeaderTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private static void ReplaceOptions(ObservableCollection<LookupOption> target, IEnumerable<LookupOption> items)
    {
        target.Clear();
        foreach (var item in items)
        {
            target.Add(item);
        }
    }

    [RelayCommand]
    private Task PreviewAsync() => ExecuteReportAsync(directPrint: false);

    [RelayCommand]
    private Task PrintAsync() => ExecuteReportAsync(directPrint: true);

    [RelayCommand]
    private void Close() => RequestClose?.Invoke();

    private async Task ExecuteReportAsync(bool directPrint)
    {
        if (IsBusy) return;

        // Exact AgenSoft date range guard (CmdPreview_Click / CmdPrint_Click)
        if (IsDateRange && FromDate.Date > ToDate.Date)
        {
            MessageBox.Show("From date must be less than To date.", HeaderTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            FocusFromDate?.Invoke();
            return;
        }

        IsBusy = true;
        try
        {
            decimal minAmount = 0m;
            if (!string.IsNullOrWhiteSpace(GreaterThanAmountText))
            {
                decimal.TryParse(GreaterThanAmountText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out minAmount);
            }

            var request = new AccountPayablesRequest
            {
                IsReceivable = IsReceivable,
                DateMode = IsAllDates
                    ? PayablesDateMode.AllDates
                    : (IsTillDate ? PayablesDateMode.TillDate : PayablesDateMode.DateRange),
                SingleDate = SingleDate.Date,
                FromDate = FromDate.Date,
                ToDate = ToDate.Date,
                CropSeasonCode = (ShowCropSeason && SelectedCropSeason != null && !string.IsNullOrWhiteSpace(SelectedCropSeason.Code))
                    ? SelectedCropSeason.Code
                    : string.Empty,
                PartyTypeAccountNo = SelectedPartyType?.Code ?? string.Empty,
                PartyTypeName = SelectedPartyType?.Name ?? "--- ALL ---",
                PartyGroupId = SelectedPartyGroup?.Id,
                PartyGroupName = SelectedPartyGroup?.Name ?? "--- ALL ---",
                CityName = SelectedCity ?? string.Empty,
                CityDisplayName = SelectedCity ?? "--- ALL ---",
                TownId = SelectedTown?.Id,
                TownName = SelectedTown?.Name ?? "--- ALL ---",
                SectorId = SelectedSector?.Id,
                SectorName = SelectedSector?.Name ?? "--- ALL SECTORS ---",
                MinAmount = minAmount,
                SortByAccountName = SortByName,
                ShowOnlyFinalBalance = ShowOnlyFinalBalance,
                ShowFinalBalances = ShowFinalBalancesCheckbox && ShowFinalBalances,
                IsUrduPrint = ShowUrduCheckbox && IsUrduPrint,
                SendSms = SendSms
            };

            var records = await _reportService.GetAccountPayablesRecordsAsync(request);
            if (records.Count == 0)
            {
                MessageBox.Show("No record found", HeaderTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (request.SendSms)
            {
                try
                {
                    int sentCount = await _reportService.SendPayablesSmsAsync(
                        records,
                        PrintContext.Company.Name ?? string.Empty,
                        PrintContext.Company.Phone ?? string.Empty,
                        string.Empty);

                    if (!directPrint && sentCount > 0)
                    {
                        MessageBox.Show($"{sentCount} SMS message(s) queued to ComRecords.", HeaderTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception smsEx)
                {
                    MessageBox.Show($"SMS queuing failed: {smsEx.Message}", HeaderTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            var (reportKey, projectedRows) = ProjectReportRows(records, request);

            string periodText = request.DateMode switch
            {
                PayablesDateMode.AllDates => "All Dates",
                PayablesDateMode.TillDate => $"Till Date: {request.SingleDate:dd/MM/yyyy}",
                _ => $"From: {request.FromDate:dd/MM/yyyy}   To: {request.ToDate:dd/MM/yyyy}"
            };

            string filterSummary =
                $"Party Type: {request.PartyTypeName}    Party Group: {request.PartyGroupName}    City: {request.CityDisplayName}";
            if (request.TownId.HasValue)
            {
                filterSummary += $"    Town: {request.TownName}";
            }
            if (request.SectorId.HasValue)
            {
                filterSummary += $"    Sector: {request.SectorName}";
            }

            var parameters = new Dictionary<string, string>
            {
                { "Period", periodText },
                { "Account", filterSummary },
                { "IsUrdu", request.IsUrduPrint ? "1" : "0" }
            };

            var reportWin = new ReportWindow(reportKey, "AccountPayablesDataSet", projectedRows, parameters, autoPrint: directPrint);
            reportWin.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating {HeaderTitle}: {ex.Message}", HeaderTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static (string ReportKey, object Rows) ProjectReportRows(
        List<AccountPayableBalanceRecord> records,
        AccountPayablesRequest request)
    {
        static string PartyTitleOf(AccountPayableBalanceRecord r, bool useUrdu) =>
            useUrdu && !string.IsNullOrWhiteSpace(r.AccountNameUrdu) ? r.AccountNameUrdu : r.AccountName;

        if (request.ShowOnlyFinalBalance)
        {
            string key = request.IsReceivable ? "AccountReceivablesBalanceReport" : "AccountPayablesBalanceReport";
            var rows = records.Select(r => new AccountPayableBalanceOnlyRow
            {
                AccountNo = r.AccountNo,
                PartyTitle = PartyTitleOf(r, request.IsUrduPrint),
                ContactNo = r.ContactNo,
                ClosingBal = r.ClosingBal,
                ClosingType = r.ClosingBalType
            }).ToList();
            return (key, rows);
        }

        if (request.ShowFinalBalances)
        {
            string key = request.IsReceivable ? "AccountReceivablesFinalsReport" : "AccountPayablesFinalsReport";
            var rows = records.Select(r => new AccountPayableFinalsRow
            {
                AccountNo = r.AccountNo,
                PartyTitle = PartyTitleOf(r, request.IsUrduPrint),
                ContactNo = r.ContactNo,
                OpeningBal = r.OpeningBal,
                OpeningType = r.OpeningBalType,
                Debit = r.Debit,
                Credit = r.Credit,
                ClosingBal = r.ClosingBal,
                ClosingType = r.ClosingBalType,
                TodayBal = r.TodayBal,
                TodayType = r.TodayBalType
            }).ToList();
            return (key, rows);
        }

        string defaultKey = request.IsReceivable ? "AccountReceivablesReport" : "AccountPayablesReport";
        var standardRows = records.Select(r => new AccountPayableStandardRow
        {
            AccountNo = r.AccountNo,
            PartyTitle = PartyTitleOf(r, request.IsUrduPrint),
            ContactNo = r.ContactNo,
            OpeningBal = r.OpeningBal,
            OpeningType = r.OpeningBalType,
            Debit = r.Debit,
            Credit = r.Credit,
            ClosingBal = r.ClosingBal,
            ClosingType = r.ClosingBalType
        }).ToList();
        return (defaultKey, standardRows);
    }

    private sealed class AccountPayableStandardRow
    {
        public string AccountNo { get; init; } = string.Empty;
        public string PartyTitle { get; init; } = string.Empty;
        public string ContactNo { get; init; } = string.Empty;
        public decimal OpeningBal { get; init; }
        public string OpeningType { get; init; } = string.Empty;
        public decimal Debit { get; init; }
        public decimal Credit { get; init; }
        public decimal ClosingBal { get; init; }
        public string ClosingType { get; init; } = string.Empty;
    }

    private sealed class AccountPayableFinalsRow
    {
        public string AccountNo { get; init; } = string.Empty;
        public string PartyTitle { get; init; } = string.Empty;
        public string ContactNo { get; init; } = string.Empty;
        public decimal OpeningBal { get; init; }
        public string OpeningType { get; init; } = string.Empty;
        public decimal Debit { get; init; }
        public decimal Credit { get; init; }
        public decimal ClosingBal { get; init; }
        public string ClosingType { get; init; } = string.Empty;
        public decimal TodayBal { get; init; }
        public string TodayType { get; init; } = string.Empty;
    }

    private sealed class AccountPayableBalanceOnlyRow
    {
        public string AccountNo { get; init; } = string.Empty;
        public string PartyTitle { get; init; } = string.Empty;
        public string ContactNo { get; init; } = string.Empty;
        public decimal ClosingBal { get; init; }
        public string ClosingType { get; init; } = string.Empty;
    }
}
