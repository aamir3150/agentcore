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

public partial class BrokerageViewModel : ObservableObject
{
    private readonly IBrokerageService _brokerageService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private int _selectedTabIndex;
    
    // Contracts
    [ObservableProperty] private ObservableCollection<Contract> _contracts = new();
    [ObservableProperty] private Contract _selectedContract = new();
    
    // Gate Passes
    [ObservableProperty] private ObservableCollection<GatePass> _gatePasses = new();
    [ObservableProperty] private GatePass _selectedGatePass = new();
    
    // Invoices
    [ObservableProperty] private ObservableCollection<BrokerageInvoice> _invoices = new();
    [ObservableProperty] private BrokerageInvoice _currentInvoice = new();

    // Lookups
    [ObservableProperty] private ObservableCollection<Party> _parties = new();
    [ObservableProperty] private ObservableCollection<Product> _products = new();

    public BrokerageViewModel(
        IBrokerageService brokerageService,
        IPartyService partyService,
        IProductService productService,
        ISystemService systemService)
    {
        _brokerageService = brokerageService;
        _partyService = partyService;
        _productService = productService;
        _systemService = systemService;
        
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        Parties = new ObservableCollection<Party>(await _partyService.GetAllPartiesAsync());
        Products = new ObservableCollection<Product>(await _productService.GetAllProductsAsync());
        
        await LoadContractsAsync();
        await LoadGatePassesAsync();
        await LoadInvoicesAsync();
    }

    #region Contract Management
    [RelayCommand]
    private async Task LoadContractsAsync()
    {
        Contracts = new ObservableCollection<Contract>(await _brokerageService.GetAllContractsAsync());
    }

    [RelayCommand]
    private async Task NewContractAsync()
    {
        var year = await _systemService.GetCurrentYearAsync();
        SelectedContract = new Contract 
        { 
            ContractDate = DateTime.Today,
            FinancialYearId = year?.Id ?? 0,
            Status = ContractStatus.Draft
        };
    }

    [RelayCommand]
    private async Task SaveContractAsync()
    {
        if (SelectedContract.PartyId == 0 || SelectedContract.ProductId == 0)
        {
            System.Windows.MessageBox.Show("Please select Party and Product.");
            return;
        }
        await _brokerageService.SaveContractAsync(SelectedContract);
        await LoadContractsAsync();
        System.Windows.MessageBox.Show("Contract saved successfully.");
    }
    #endregion

    #region GatePass Management
    [RelayCommand]
    private async Task LoadGatePassesAsync()
    {
        GatePasses = new ObservableCollection<GatePass>(await _brokerageService.GetAllGatePassesAsync());
    }

    [RelayCommand]
    private void NewGatePassAsync()
    {
        SelectedGatePass = new GatePass 
        { 
            GatePassDate = DateTime.Today,
            Status = GatePassStatus.Open
        };
    }

    [RelayCommand]
    private async Task SaveGatePassAsync()
    {
        if (SelectedGatePass.PartyId == 0 || SelectedGatePass.ProductId == 0)
        {
            System.Windows.MessageBox.Show("Please select Party and Product.");
            return;
        }
        
        SelectedGatePass.NetWeight = SelectedGatePass.GrossWeight - SelectedGatePass.TareWeight;
        await _brokerageService.SaveGatePassAsync(SelectedGatePass);
        await LoadGatePassesAsync();
        System.Windows.MessageBox.Show("Gate Pass saved successfully.");
    }
    #endregion

    #region Invoice Management
    [ObservableProperty] private ObservableCollection<GatePass> _availableGatePasses = new();

    [RelayCommand]
    private async Task LoadInvoicesAsync()
    {
        Invoices = new ObservableCollection<BrokerageInvoice>(await _brokerageService.GetAllBrokerageInvoicesAsync());
    }

    [RelayCommand]
    private async Task NewInvoiceAsync()
    {
        var year = await _systemService.GetCurrentYearAsync();
        CurrentInvoice = new BrokerageInvoice 
        { 
            InvoiceDate = DateTime.Today,
            FinancialYearId = year?.Id ?? 0,
            Status = InvoiceStatus.Draft
        };
        // Load Open Gate Passes
        var openGP = await _brokerageService.GetGatePassesByStatusAsync(GatePassStatus.Open);
        AvailableGatePasses = new ObservableCollection<GatePass>(openGP);
    }

    public void OnInvoiceContractChanged()
    {
        if (CurrentInvoice.ContractId != null)
        {
            var contract = Contracts.FirstOrDefault(c => c.Id == CurrentInvoice.ContractId);
            if (contract != null)
            {
                CurrentInvoice.PartyId = contract.PartyId;
                // Filter available gate passes by party and product
                var filtered = AvailableGatePasses.Where(g => g.PartyId == contract.PartyId && g.ProductId == contract.ProductId).ToList();
                // (In a real app, we'd have a multi-select list for these)
            }
        }
    }

    public void CalculateInvoiceTotals()
    {
        // Simple manual calculation for demo
        CurrentInvoice.NetAmount = CurrentInvoice.GrossAmount 
                                 - CurrentInvoice.CommissionAmount 
                                 - CurrentInvoice.WHT_Amount 
                                 + CurrentInvoice.Soodh_Amount 
                                 - CurrentInvoice.LaborCharges 
                                 - CurrentInvoice.MarketCommitteeFee;
        
        OnPropertyChanged(nameof(CurrentInvoice));
    }

    [RelayCommand]
    private async Task SaveInvoiceAsync()
    {
        if (CurrentInvoice.PartyId == 0)
        {
            System.Windows.MessageBox.Show("Please select a Party/Contract.");
            return;
        }
        await _brokerageService.SaveBrokerageInvoiceAsync(CurrentInvoice);
        await LoadInvoicesAsync();
        System.Windows.MessageBox.Show("Brokerage Invoice saved.");
    }

    [RelayCommand]
    private async Task PostInvoiceAsync()
    {
        if (CurrentInvoice.Id != 0)
        {
            await _brokerageService.PostBrokerageInvoiceToGLAsync(CurrentInvoice.Id);
            System.Windows.MessageBox.Show("Invoice posted to Ledger.");
            await LoadInvoicesAsync();
        }
    }
    #endregion
}
