using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class ProductStockRecalcItem : ObservableObject
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string UrduName { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal CurrentStock { get; set; }
    public decimal RecalculatedStock { get; set; }

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public partial class StockRecalculationViewModel : ObservableObject
{
    private readonly IStockService _stockService;
    private readonly IProductService _productService;

    [ObservableProperty] private ObservableCollection<ProductStockRecalcItem> _allProducts = new();
    [ObservableProperty] private ObservableCollection<ProductStockRecalcItem> _filteredProducts = new();

    // Filter Options matching AgenSoft
    [ObservableProperty] private bool _filterAll = true;
    [ObservableProperty] private bool _filterCrops = false;
    [ObservableProperty] private bool _filterGeneral = false;
    [ObservableProperty] private bool _filterBrokerage = false;

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private double _progressValue;
    [ObservableProperty] private string _progressText = "Ready to recalculate stock / اسٹاک کا حساب کرنے کے لیے تیار";
    [ObservableProperty] private bool _isRecalculating;

    public StockRecalculationViewModel(IStockService stockService, IProductService productService)
    {
        _stockService = stockService;
        _productService = productService;
    }

    public async Task InitializeAsync()
    {
        await LoadProductsAsync();
    }

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        try
        {
            AllProducts.Clear();
            FilteredProducts.Clear();

            var products = await _productService.GetAllProductsAsync();
            foreach (var p in products)
            {
                var item = new ProductStockRecalcItem
                {
                    Id = p.Id,
                    Code = p.ProductCode ?? p.Id.ToString(),
                    Name = p.Name ?? "",
                    UrduName = p.UrduName ?? p.Name ?? "",
                    Category = p.ProductGroup?.Name ?? "General",
                    CurrentStock = p.CurrentStock,
                    IsSelected = true
                };
                AllProducts.Add(item);
                FilteredProducts.Add(item);
            }
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Products for Stock Recalculation");
        }
    }

    [RelayCommand]
    public void ApplyFilters()
    {
        FilteredProducts.Clear();
        var query = AllProducts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var st = SearchText.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(st) || p.UrduName.Contains(st) || p.Code.ToLower().Contains(st));
        }

        if (FilterCrops)
        {
            query = query.Where(p => p.Category.Contains("Crop", StringComparison.OrdinalIgnoreCase) || p.Category.Contains("اجناس", StringComparison.OrdinalIgnoreCase));
        }
        else if (FilterGeneral)
        {
            query = query.Where(p => p.Category.Contains("General", StringComparison.OrdinalIgnoreCase) || p.Category.Contains("عام", StringComparison.OrdinalIgnoreCase));
        }
        else if (FilterBrokerage)
        {
            query = query.Where(p => p.Category.Contains("Brokerage", StringComparison.OrdinalIgnoreCase) || p.Category.Contains("بروکریج", StringComparison.OrdinalIgnoreCase));
        }

        foreach (var p in query)
        {
            FilteredProducts.Add(p);
        }
    }

    [RelayCommand]
    public void SelectAll()
    {
        foreach (var p in FilteredProducts) p.IsSelected = true;
    }

    [RelayCommand]
    public void DeselectAll()
    {
        foreach (var p in FilteredProducts) p.IsSelected = false;
    }

    [RelayCommand]
    public async Task StartRecalculationAsync()
    {
        var selected = FilteredProducts.Where(p => p.IsSelected).ToList();
        if (!selected.Any())
        {
            MessageBox.Show("Baraye meherbani kam az kam aik product muntakhab karein.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsRecalculating = true;
            ProgressValue = 0;
            int total = selected.Count;
            int completed = 0;

            foreach (var item in selected)
            {
                ProgressText = $"Recalculating: {item.Name} ({completed + 1} of {total})...";
                await _stockService.RecalculateStockAsync(item.Id);
                
                // Get updated balance
                item.CurrentStock = await _stockService.GetCurrentStockAsync(item.Id);

                completed++;
                ProgressValue = (completed / (double)total) * 100.0;
            }

            ProgressText = $"کامیابی! {completed} مصنوعات کا اسٹاک بیلنس مکمل طور پر اپ ڈیٹ ہو چکا ہے۔";
            MessageBox.Show($"منتخب کردہ {completed} آئٹمز کا اسٹاک ریکارڈ لیجر اور واؤچرز کی بنیاد پر کامیابی سے دوبارہ تیار ہو گیا!", 
                "کامیابی / Stock Recalculated", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Stock Recalculation");
            ProgressText = $"نقص: {ex.Message}";
        }
        finally
        {
            IsRecalculating = false;
        }
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
