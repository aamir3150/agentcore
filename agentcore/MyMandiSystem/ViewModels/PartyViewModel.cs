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

public partial class PartyViewModel : ObservableObject
{
    private readonly IPartyService _partyService;
    private readonly ICatalogService _catalogService;

    // Data Collections
    [ObservableProperty] private ObservableCollection<Party> _allParties = new();
    [ObservableProperty] private ObservableCollection<Party> _filteredParties = new();
    
    [ObservableProperty] private ObservableCollection<PartyGroup> _partyGroups = new();
    [ObservableProperty] private ObservableCollection<Town> _towns = new();
    [ObservableProperty] private ObservableCollection<Sector> _sectors = new();
    [ObservableProperty] private ObservableCollection<Salesman> _salesmen = new();

    // UI State
    [ObservableProperty] private int _selectedTabIndex;
    [ObservableProperty] private bool _isFormVisible;
    [ObservableProperty] private Party _currentParty = new();
    [ObservableProperty] private Party? _selectedParty;
    [ObservableProperty] private Salesman _currentSalesman = new();
    [ObservableProperty] private Salesman? _selectedSalesman;
    [ObservableProperty] private bool _isSalesmanFormVisible;

    public Array PartyTypes => Enum.GetValues(typeof(PartyType));

    public PartyViewModel(IPartyService partyService, ICatalogService catalogService)
    {
        _partyService = partyService;
        _catalogService = catalogService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var parties = await _partyService.GetAllPartiesAsync();
        AllParties = new ObservableCollection<Party>(parties);

        var groups = await _partyService.GetPartyGroupsAsync();
        PartyGroups = new ObservableCollection<PartyGroup>(groups);

        var towns = await _catalogService.GetTownsAsync();
        Towns = new ObservableCollection<Town>(towns);

        var sectors = await _catalogService.GetSectorsAsync();
        Sectors = new ObservableCollection<Sector>(sectors);

        var salesmen = await _partyService.GetAllSalesmenAsync();
        Salesmen = new ObservableCollection<Salesman>(salesmen);

        FilterParties();
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        FilterParties();
        IsFormVisible = false;
        IsSalesmanFormVisible = false;
    }

    private void FilterParties()
    {
        if (SelectedTabIndex == 0) // Customers
            FilteredParties = new ObservableCollection<Party>(AllParties.Where(p => p.PartyType == PartyType.Customer || p.PartyType == PartyType.Both));
        else if (SelectedTabIndex == 1) // Vendors
            FilteredParties = new ObservableCollection<Party>(AllParties.Where(p => p.PartyType == PartyType.Vendor || p.PartyType == PartyType.Both));
    }

    // --- PARTY CRUD (Customers/Vendors) ---

    [RelayCommand]
    private void AddParty()
    {
        CurrentParty = new Party 
        { 
            PartyType = SelectedTabIndex == 0 ? PartyType.Customer : PartyType.Vendor,
            IsActive = true 
        };
        IsFormVisible = true;
    }

    [RelayCommand]
    private void EditParty(Party party)
    {
        if (party == null) return;
        CurrentParty = new Party
        {
            Id = party.Id,
            PartyNo = party.PartyNo,
            Name = party.Name,
            UrduName = party.UrduName,
            PartyType = party.PartyType,
            PartyGroupId = party.PartyGroupId,
            TownId = party.TownId,
            SectorId = party.SectorId,
            Phone = party.Phone,
            CNIC = party.CNIC,
            Address = party.Address,
            CreditLimit = party.CreditLimit,
            WHT_Percentage = party.WHT_Percentage,
            DefaultMarkupRate = party.DefaultMarkupRate,
            IsActive = party.IsActive,
            AccountId = party.AccountId,
            Account = party.Account
        };
        IsFormVisible = true;
    }

    [RelayCommand]
    private async Task SaveParty()
    {
        if (string.IsNullOrWhiteSpace(CurrentParty.Name))
        {
            System.Windows.MessageBox.Show("Party Name is required.");
            return;
        }

        try
        {
            if (CurrentParty.Id == 0)
            {
                // Generate a temporary auto PartyNo if blank
                if (string.IsNullOrWhiteSpace(CurrentParty.PartyNo))
                    CurrentParty.PartyNo = "P-" + DateTime.Now.Ticks.ToString().Substring(10);

                await _partyService.AddPartyAsync(CurrentParty);
            }
            else
            {
                var existing = AllParties.FirstOrDefault(p => p.Id == CurrentParty.Id);
                if (existing != null)
                {
                    existing.Name = CurrentParty.Name;
                    existing.UrduName = CurrentParty.UrduName;
                    existing.PartyType = CurrentParty.PartyType;
                    existing.PartyGroupId = CurrentParty.PartyGroupId;
                    existing.TownId = CurrentParty.TownId;
                    existing.SectorId = CurrentParty.SectorId;
                    existing.Phone = CurrentParty.Phone;
                    existing.CNIC = CurrentParty.CNIC;
                    existing.Address = CurrentParty.Address;
                    existing.CreditLimit = CurrentParty.CreditLimit;
                    existing.WHT_Percentage = CurrentParty.WHT_Percentage;
                    existing.DefaultMarkupRate = CurrentParty.DefaultMarkupRate;
                    existing.IsActive = CurrentParty.IsActive;

                    await _partyService.UpdatePartyAsync(existing);
                }
            }

            IsFormVisible = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error saving party: {ex.Message}");
        }
    }

    [RelayCommand]
    private void CancelParty() => IsFormVisible = false;

    [RelayCommand]
    private async Task DeleteParty(Party party)
    {
        if (party == null) return;
        if (System.Windows.MessageBox.Show($"Delete party '{party.Name}'? This will also disable their ledger account.", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            await _partyService.DeletePartyAsync(party.Id);
            await LoadDataAsync();
        }
    }

    // --- SALESMAN CRUD ---

    [RelayCommand]
    private void AddSalesman()
    {
        CurrentSalesman = new Salesman { IsActive = true };
        IsSalesmanFormVisible = true;
    }

    [RelayCommand]
    private void EditSalesman(Salesman salesman)
    {
        if (salesman == null) return;
        CurrentSalesman = new Salesman
        {
            Id = salesman.Id,
            Name = salesman.Name,
            Phone = salesman.Phone,
            CommissionRate = salesman.CommissionRate,
            IsActive = salesman.IsActive,
            AccountId = salesman.AccountId,
            Account = salesman.Account
        };
        IsSalesmanFormVisible = true;
    }

    [RelayCommand]
    private async Task SaveSalesman()
    {
        if (string.IsNullOrWhiteSpace(CurrentSalesman.Name))
        {
            System.Windows.MessageBox.Show("Salesman Name is required.");
            return;
        }

        if (CurrentSalesman.Id == 0) await _partyService.AddSalesmanAsync(CurrentSalesman);
        else
        {
            var existing = Salesmen.FirstOrDefault(s => s.Id == CurrentSalesman.Id);
            if (existing != null)
            {
                existing.Name = CurrentSalesman.Name;
                existing.Phone = CurrentSalesman.Phone;
                existing.CommissionRate = CurrentSalesman.CommissionRate;
                existing.IsActive = CurrentSalesman.IsActive;
                await _partyService.UpdateSalesmanAsync(existing);
            }
        }
        IsSalesmanFormVisible = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private void CancelSalesman() => IsSalesmanFormVisible = false;

    [RelayCommand]
    private async Task DeleteSalesman(Salesman salesman)
    {
        if (salesman == null) return;
        if (System.Windows.MessageBox.Show($"Delete salesman '{salesman.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            await _partyService.DeleteSalesmanAsync(salesman.Id);
            await LoadDataAsync();
        }
    }
}
