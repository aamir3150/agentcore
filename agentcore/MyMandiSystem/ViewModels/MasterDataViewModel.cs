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

public partial class MasterDataViewModel : ObservableObject
{
    private readonly ICatalogService _catalogService;

    [ObservableProperty] private ObservableCollection<Company> _companies = new();
    [ObservableProperty] private ObservableCollection<Unit> _units = new();
    [ObservableProperty] private ObservableCollection<Town> _towns = new();
    [ObservableProperty] private ObservableCollection<Sector> _sectors = new();

    // View State Flags for Forms
    [ObservableProperty] private bool _isCompanyFormVisible;
    [ObservableProperty] private bool _isUnitFormVisible;
    [ObservableProperty] private bool _isTownFormVisible;
    [ObservableProperty] private bool _isSectorFormVisible;

    // Selections
    [ObservableProperty] private Company? _selectedCompany;
    [ObservableProperty] private Unit? _selectedUnit;
    [ObservableProperty] private Town? _selectedTown;
    [ObservableProperty] private Sector? _selectedSector;

    // Form Models
    [ObservableProperty] private Company _currentCompany = new();
    [ObservableProperty] private Unit _currentUnit = new();
    [ObservableProperty] private Town _currentTown = new();
    [ObservableProperty] private Sector _currentSector = new();

    public MasterDataViewModel(ICatalogService catalogService)
    {
        _catalogService = catalogService;
        _ = LoadAllDataAsync();
    }

    [RelayCommand]
    private async Task LoadAllDataAsync()
    {
        var companies = await _catalogService.GetCompaniesAsync();
        Companies = new ObservableCollection<Company>(companies);

        var units = await _catalogService.GetUnitsAsync();
        Units = new ObservableCollection<Unit>(units);

        var towns = await _catalogService.GetTownsAsync();
        Towns = new ObservableCollection<Town>(towns);

        var sectors = await _catalogService.GetSectorsAsync();
        Sectors = new ObservableCollection<Sector>(sectors);
    }

    // --- COMPANIES ---
    [RelayCommand] private void AddCompany() { CurrentCompany = new Company { IsActive = true }; IsCompanyFormVisible = true; }
    [RelayCommand] private void EditCompany(Company company) { if (company != null) { CurrentCompany = new Company { Id = company.Id, Name = company.Name, ShortName = company.ShortName, IsActive = company.IsActive }; IsCompanyFormVisible = true; } }
    [RelayCommand] private void CancelCompany() => IsCompanyFormVisible = false;
    
    [RelayCommand]
    private async Task SaveCompany()
    {
        if (string.IsNullOrWhiteSpace(CurrentCompany.Name)) { System.Windows.MessageBox.Show("Company Name is required."); return; }
        if (CurrentCompany.Id == 0) await _catalogService.AddCompanyAsync(CurrentCompany);
        else 
        {
            var existing = Companies.FirstOrDefault(c => c.Id == CurrentCompany.Id);
            if (existing != null) { existing.Name = CurrentCompany.Name; existing.ShortName = CurrentCompany.ShortName; existing.IsActive = CurrentCompany.IsActive; await _catalogService.UpdateCompanyAsync(existing); }
        }
        IsCompanyFormVisible = false;
        await LoadAllDataAsync();
    }

    [RelayCommand]
    private async Task DeleteCompany(Company company)
    {
        if (company == null) return;
        if (System.Windows.MessageBox.Show($"Delete company '{company.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            await _catalogService.DeleteCompanyAsync(company.Id);
            await LoadAllDataAsync();
        }
    }

    // --- UNITS ---
    [RelayCommand] private void AddUnit() { CurrentUnit = new Unit(); IsUnitFormVisible = true; }
    [RelayCommand] private void EditUnit(Unit unit) { if (unit != null) { CurrentUnit = new Unit { Id = unit.Id, Name = unit.Name, ShortName = unit.ShortName }; IsUnitFormVisible = true; } }
    [RelayCommand] private void CancelUnit() => IsUnitFormVisible = false;

    [RelayCommand]
    private async Task SaveUnit()
    {
        if (string.IsNullOrWhiteSpace(CurrentUnit.Name)) { System.Windows.MessageBox.Show("Unit Name is required."); return; }
        if (CurrentUnit.Id == 0) await _catalogService.AddUnitAsync(CurrentUnit);
        else 
        {
            var existing = Units.FirstOrDefault(u => u.Id == CurrentUnit.Id);
            if (existing != null) { existing.Name = CurrentUnit.Name; existing.ShortName = CurrentUnit.ShortName; await _catalogService.UpdateUnitAsync(existing); }
        }
        IsUnitFormVisible = false;
        await LoadAllDataAsync();
    }

    [RelayCommand]
    private async Task DeleteUnit(Unit unit)
    {
        if (unit == null) return;
        if (System.Windows.MessageBox.Show($"Delete unit '{unit.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            await _catalogService.DeleteUnitAsync(unit.Id);
            await LoadAllDataAsync();
        }
    }

    // --- TOWNS ---
    [RelayCommand] private void AddTown() { CurrentTown = new Town(); IsTownFormVisible = true; }
    [RelayCommand] private void EditTown(Town town) { if (town != null) { CurrentTown = new Town { Id = town.Id, Name = town.Name }; IsTownFormVisible = true; } }
    [RelayCommand] private void CancelTown() => IsTownFormVisible = false;

    [RelayCommand]
    private async Task SaveTown()
    {
        if (string.IsNullOrWhiteSpace(CurrentTown.Name)) { System.Windows.MessageBox.Show("Town Name is required."); return; }
        if (CurrentTown.Id == 0) await _catalogService.AddTownAsync(CurrentTown);
        else 
        {
            var existing = Towns.FirstOrDefault(t => t.Id == CurrentTown.Id);
            if (existing != null) { existing.Name = CurrentTown.Name; await _catalogService.UpdateTownAsync(existing); }
        }
        IsTownFormVisible = false;
        await LoadAllDataAsync();
    }

    [RelayCommand]
    private async Task DeleteTown(Town town)
    {
        if (town == null) return;
        if (System.Windows.MessageBox.Show($"Delete town '{town.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            // Verify if sectors exist (simplified)
            if (Sectors.Any(s => s.TownId == town.Id)) { System.Windows.MessageBox.Show("Cannot delete Town with active Sectors."); return; }
            await _catalogService.DeleteTownAsync(town.Id);
            await LoadAllDataAsync();
        }
    }

    // --- SECTORS ---
    [RelayCommand] private void AddSector() { CurrentSector = new Sector { TownId = Towns.FirstOrDefault()?.Id ?? 0 }; IsSectorFormVisible = true; }
    [RelayCommand] private void EditSector(Sector sector) { if (sector != null) { CurrentSector = new Sector { Id = sector.Id, Name = sector.Name, TownId = sector.TownId }; IsSectorFormVisible = true; } }
    [RelayCommand] private void CancelSector() => IsSectorFormVisible = false;

    [RelayCommand]
    private async Task SaveSector()
    {
        if (string.IsNullOrWhiteSpace(CurrentSector.Name) || CurrentSector.TownId == 0) { System.Windows.MessageBox.Show("Sector Name and Town are required."); return; }
        if (CurrentSector.Id == 0) await _catalogService.AddSectorAsync(CurrentSector);
        else 
        {
            var existing = Sectors.FirstOrDefault(s => s.Id == CurrentSector.Id);
            if (existing != null) { existing.Name = CurrentSector.Name; existing.TownId = CurrentSector.TownId; await _catalogService.UpdateSectorAsync(existing); }
        }
        IsSectorFormVisible = false;
        await LoadAllDataAsync();
    }

    [RelayCommand]
    private async Task DeleteSector(Sector sector)
    {
        if (sector == null) return;
        if (System.Windows.MessageBox.Show($"Delete sector '{sector.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            await _catalogService.DeleteSectorAsync(sector.Id);
            await LoadAllDataAsync();
        }
    }
}
