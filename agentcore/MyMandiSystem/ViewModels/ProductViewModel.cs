using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class ProductViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly ICatalogService _catalogService;
    private readonly IStockService _stockService;

    // Collections
    [ObservableProperty] private ObservableCollection<Product> _products = new();
    [ObservableProperty] private ObservableCollection<ProductGroup> _productGroups = new();
    
    // Lookups
    [ObservableProperty] private ObservableCollection<Company> _companies = new();
    [ObservableProperty] private ObservableCollection<Unit> _units = new();

    // UI States
    [ObservableProperty] private int _selectedTabIndex;
    
    [ObservableProperty] private Product _currentProduct = new();
    [ObservableProperty] private Product? _selectedProduct;
    [ObservableProperty] private bool _isProductFormVisible;

    [ObservableProperty] private ProductGroup _currentGroup = new();
    [ObservableProperty] private ProductGroup? _selectedGroup;
    [ObservableProperty] private bool _isGroupFormVisible;

    public ProductViewModel(IProductService productService, ICatalogService catalogService, IStockService stockService)
    {
        _productService = productService;
        _catalogService = catalogService;
        _stockService = stockService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var companies = await _catalogService.GetCompaniesAsync();
        Companies = new ObservableCollection<Company>(companies);

        var units = await _catalogService.GetUnitsAsync();
        Units = new ObservableCollection<Unit>(units);

        var groups = await _productService.GetAllProductGroupsAsync();
        ProductGroups = new ObservableCollection<ProductGroup>(groups);

        var products = await _productService.GetAllProductsAsync();
        Products = new ObservableCollection<Product>(products);
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        IsProductFormVisible = false;
        IsGroupFormVisible = false;
    }

    // --- PRODUCTS ---
    [RelayCommand]
    private void AddProduct()
    {
        CurrentProduct = new Product { IsActive = true };
        IsProductFormVisible = true;
    }

    [RelayCommand]
    private void EditProduct(Product product)
    {
        if (product == null) return;
        CurrentProduct = new Product
        {
            Id = product.Id,
            ProductCode = product.ProductCode,
            Name = product.Name,
            UrduName = product.UrduName,
            CompanyId = product.CompanyId,
            UnitId = product.UnitId,
            GroupId = product.GroupId,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            BarcodeNo = product.BarcodeNo,
            IsPestro = product.IsPestro,
            ExpiryTracking = product.ExpiryTracking,
            ReorderLevel = product.ReorderLevel,
            HSCode = product.HSCode,
            IsActive = product.IsActive
        };
        IsProductFormVisible = true;
    }

    [RelayCommand]
    private async Task SaveProduct()
    {
        if (string.IsNullOrWhiteSpace(CurrentProduct.Name))
        {
            System.Windows.MessageBox.Show("Product Name is required.");
            return;
        }

        if (CurrentProduct.Id == 0)
        {
             if (string.IsNullOrWhiteSpace(CurrentProduct.ProductCode))
                 CurrentProduct.ProductCode = "ITM-" + DateTime.Now.Ticks.ToString().Substring(10);
                 
             await _productService.AddProductAsync(CurrentProduct);
        }
        else
        {
            var existing = Products.FirstOrDefault(p => p.Id == CurrentProduct.Id);
            if (existing != null)
            {
                existing.Name = CurrentProduct.Name;
                existing.UrduName = CurrentProduct.UrduName;
                existing.CompanyId = CurrentProduct.CompanyId;
                existing.UnitId = CurrentProduct.UnitId;
                existing.GroupId = CurrentProduct.GroupId;
                existing.PurchasePrice = CurrentProduct.PurchasePrice;
                existing.SalePrice = CurrentProduct.SalePrice;
                existing.BarcodeNo = CurrentProduct.BarcodeNo;
                existing.IsPestro = CurrentProduct.IsPestro;
                existing.ExpiryTracking = CurrentProduct.ExpiryTracking;
                existing.ReorderLevel = CurrentProduct.ReorderLevel;
                existing.HSCode = CurrentProduct.HSCode;
                existing.IsActive = CurrentProduct.IsActive;

                await _productService.UpdateProductAsync(existing);
            }
        }
        IsProductFormVisible = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private void CancelProduct() => IsProductFormVisible = false;

    [RelayCommand]
    private async Task DeleteProduct(Product product)
    {
        if (product == null) return;
        if (System.Windows.MessageBox.Show($"Delete product '{product.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            await _productService.DeleteProductAsync(product.Id);
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    private async Task RecalculateStockAsync()
    {
        try
        {
            await _stockService.RecalculateStockAsync(SelectedProduct?.Id);
            await LoadDataAsync();
            System.Windows.MessageBox.Show("Stock recalculation completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error recalculating stock: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // --- PRODUCT GROUPS ---
    [RelayCommand]
    private void AddGroup()
    {
        CurrentGroup = new ProductGroup();
        IsGroupFormVisible = true;
    }

    [RelayCommand]
    private void EditGroup(ProductGroup group)
    {
        if (group == null) return;
        CurrentGroup = new ProductGroup { Id = group.Id, Name = group.Name };
        IsGroupFormVisible = true;
    }

    [RelayCommand]
    private async Task SaveGroup()
    {
        if (string.IsNullOrWhiteSpace(CurrentGroup.Name))
        {
            System.Windows.MessageBox.Show("Group Name is required.");
            return;
        }

        if (CurrentGroup.Id == 0) await _productService.AddProductGroupAsync(CurrentGroup);
        else
        {
            var existing = ProductGroups.FirstOrDefault(g => g.Id == CurrentGroup.Id);
            if (existing != null)
            {
                existing.Name = CurrentGroup.Name;
                await _productService.UpdateProductGroupAsync(existing);
            }
        }
        IsGroupFormVisible = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private void CancelGroup() => IsGroupFormVisible = false;

    [RelayCommand]
    private async Task DeleteGroup(ProductGroup group)
    {
        if (group == null) return;
        if (System.Windows.MessageBox.Show($"Delete group '{group.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            await _productService.DeleteProductGroupAsync(group.Id);
            await LoadDataAsync();
        }
    }
}
