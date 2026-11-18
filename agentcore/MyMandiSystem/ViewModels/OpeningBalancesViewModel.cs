using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class OpeningBalancesViewModel : ObservableObject
{
    private readonly IAccountService _accountService;
    private readonly IProductService _productService;
    private readonly IStockService _stockService;

    [ObservableProperty] private int _selectedTabIndex; // 0: Financial (Accounts), 1: Inventory (Stock)

    // Filter controls (AgenSoft exact parity: CmbFilter & txtAccNameFilter)
    [ObservableProperty] private ObservableCollection<string> _accountTypes = new();
    [ObservableProperty] private string _selectedAccountType = "All";
    [ObservableProperty] private string _accountNameFilter = string.Empty;

    // Available Crop Seasons dropdown (Products where IsCrop=1 or IsBroker=1)
    [ObservableProperty] private ObservableCollection<CropSeasonItemDto> _cropSeasons = new();

    // Master list and filtered list for data grid
    private List<AccountOpeningModel> _allAccountBalances = new();
    [ObservableProperty] private ObservableCollection<AccountOpeningModel> _filteredAccountBalances = new();

    // Stock Opening Balances
    [ObservableProperty] private ObservableCollection<ProductOpeningModel> _productStocks = new();

    // Summary totals
    [ObservableProperty] private decimal _totalDebit;
    [ObservableProperty] private decimal _totalCredit;
    [ObservableProperty] private decimal _difference;
    [ObservableProperty] private int _totalAccountsCount;

    // Window action to close from VM
    public Action? RequestClose { get; set; }

    public OpeningBalancesViewModel(
        IAccountService accountService, 
        IProductService productService,
        IStockService stockService)
    {
        _accountService = accountService;
        _productService = productService;
        _stockService = stockService;
        
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        await LoadAccountTypesAsync();
        await LoadCropsAsync();
        await LoadDataAsync();
    }

    private async Task LoadAccountTypesAsync()
    {
        try
        {
            var types = await _accountService.GetOpeningBalanceAccountTypesAsync();
            AccountTypes = new ObservableCollection<string>(types);
            if (!AccountTypes.Contains(SelectedAccountType))
            {
                SelectedAccountType = AccountTypes.FirstOrDefault() ?? "All";
            }
        }
        catch
        {
            AccountTypes = new ObservableCollection<string> { "All" };
            SelectedAccountType = "All";
        }
    }

    private async Task LoadCropsAsync()
    {
        try
        {
            var crops = await _accountService.GetOpeningCropSeasonsAsync();
            var list = new List<CropSeasonItemDto>
            {
                new() { ProductId = string.Empty, ProductName = "-- None / کوئی نہیں --" }
            };
            list.AddRange(crops);
            CropSeasons = new ObservableCollection<CropSeasonItemDto>(list);
        }
        catch
        {
            CropSeasons = new ObservableCollection<CropSeasonItemDto>();
        }
    }

    partial void OnSelectedAccountTypeChanged(string value)
    {
        _ = LoadDataAsync();
    }

    partial void OnAccountNameFilterChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            // Load accounts for selected account type (AgenSoft query)
            var dtos = await _accountService.GetAccountsOpeningBalancesAsync(SelectedAccountType);
            var models = new List<AccountOpeningModel>();

            foreach (var d in dtos)
            {
                var m = new AccountOpeningModel
                {
                    AccountId = d.AccountId,
                    AccountNo = d.AccountNo,
                    Name = d.AccountName,
                    AccountType = d.AccountType,
                    OpeningDebit = d.OpeningDebit,
                    OpeningCredit = d.OpeningCredit,
                    CropSeason = d.CropSeason ?? string.Empty,
                    CropName = d.CropName ?? string.Empty
                };
                m.PropertyChanged += (s, e) => RecalculateTotals();
                models.Add(m);
            }

            _allAccountBalances = models;
            ApplyFilter();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Accounts Opening Balances Load");
        }
    }

    private void ApplyFilter()
    {
        IEnumerable<AccountOpeningModel> query = _allAccountBalances;

        if (!string.IsNullOrWhiteSpace(AccountNameFilter))
        {
            var filter = AccountNameFilter.Trim().ToLower();
            query = query.Where(a => 
                (!string.IsNullOrEmpty(a.Name) && a.Name.ToLower().Contains(filter)) ||
                (!string.IsNullOrEmpty(a.AccountNo) && a.AccountNo.ToLower().Contains(filter)));
        }

        FilteredAccountBalances = new ObservableCollection<AccountOpeningModel>(query);
        TotalAccountsCount = FilteredAccountBalances.Count;
        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        TotalDebit = _allAccountBalances.Sum(a => a.OpeningDebit);
        TotalCredit = _allAccountBalances.Sum(a => a.OpeningCredit);
        Difference = TotalDebit - TotalCredit;
    }

    [RelayCommand]
    public async Task SaveFinancial()
    {
        try
        {
            // AgenSoft Validation: "Please provide the balance in either Debit or Credit"
            var invalidRow = _allAccountBalances.FirstOrDefault(a => a.OpeningDebit > 0 && a.OpeningCredit > 0);
            if (invalidRow != null)
            {
                MessageBox.Show(
                    $"Account '{invalidRow.AccountNo} - {invalidRow.Name}':\nPlease provide the balance in either Debit or Credit.\nبراہ کرم ڈیبٹ یا کریڈٹ میں سے کسی ایک میں رقم درج کریں۔",
                    "Alert / انتباہ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var dtos = _allAccountBalances.Select(a => new AccountOpeningBalanceDto
            {
                AccountId = a.AccountId,
                AccountNo = a.AccountNo,
                AccountName = a.Name,
                AccountType = a.AccountType,
                OpeningDebit = a.OpeningDebit,
                OpeningCredit = a.OpeningCredit,
                CropSeason = a.CropSeason,
                CropName = a.CropName
            }).ToList();

            await _accountService.SaveAccountsOpeningBalancesAsync(dtos);

            MessageBox.Show(
                "Opening Balances successfully saved.\nابتدائی بیلنس کامیابی سے محفوظ ہو گئے ہیں۔", 
                "Success / کامیابی", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);

            await LoadDataAsync();
            CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new MyMandiSystem.Core.Messages.RefreshDashboardMessage());
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Opening Balances Save");
        }
    }

    [RelayCommand]
    public void Clear()
    {
        AccountNameFilter = string.Empty;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    public void Close()
    {
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private async Task SaveInventory()
    {
        try
        {
            foreach (var item in ProductStocks)
            {
                await _stockService.SetOpeningStockAsync(item.ProductId, item.Qty, item.Rate);
            }
            MessageBox.Show("Inventory Opening Balances saved successfully.\nابتدائی اسٹاک کامیابی سے محفوظ ہو گیا ہے۔", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Inventory Opening Balances Save");
        }
    }

    [RelayCommand]
    private Task SaveStock() => SaveInventory();
}

public class AccountOpeningModel : ObservableObject
{
    public int AccountId { get; set; }
    public string AccountNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    
    private decimal _openingDebit;
    public decimal OpeningDebit
    {
        get => _openingDebit;
        set
        {
            if (SetProperty(ref _openingDebit, value))
            {
                OnPropertyChanged(nameof(NetBalance));
            }
        }
    }

    private decimal _openingCredit;
    public decimal OpeningCredit
    {
        get => _openingCredit;
        set
        {
            if (SetProperty(ref _openingCredit, value))
            {
                OnPropertyChanged(nameof(NetBalance));
            }
        }
    }

    private string? _cropSeason;
    public string? CropSeason
    {
        get => _cropSeason;
        set => SetProperty(ref _cropSeason, value);
    }

    private string? _cropName;
    public string? CropName
    {
        get => _cropName;
        set => SetProperty(ref _cropName, value);
    }

    public decimal NetBalance => OpeningDebit - OpeningCredit;
}

public class ProductOpeningModel : ObservableObject
{
    public int ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    private decimal _qty;
    public decimal Qty
    {
        get => _qty;
        set
        {
            if (SetProperty(ref _qty, value))
            {
                OnPropertyChanged(nameof(TotalValue));
            }
        }
    }

    private decimal _rate;
    public decimal Rate
    {
        get => _rate;
        set
        {
            if (SetProperty(ref _rate, value))
            {
                OnPropertyChanged(nameof(TotalValue));
            }
        }
    }

    public decimal TotalValue => Qty * Rate;
}
