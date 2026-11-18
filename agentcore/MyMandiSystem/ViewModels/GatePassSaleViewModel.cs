using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using MyMandiSystem.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels
{
    public class GatePassItemDto : ObservableObject
    {
        public int SerialNo { get; set; } = 1;
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string VehicleNo { get; set; } = "";
        public decimal BagsQty { get; set; } = 0;
        public decimal PQty { get; set; } = 0;
        public decimal WeightPerBag { get; set; } = 0;
        public decimal RatePerMound { get; set; } = 0;
        public decimal TotalKgs { get; set; } = 0;
        public decimal MoundStd { get; set; } = 40;
        public string InMounds => MoundStd > 0 ? $"{(int)(TotalKgs / MoundStd)}-{(TotalKgs % MoundStd):F1}" : "0-0";
    }

    public partial class GatePassSaleViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPartyService _partyService;
        private readonly IProductService _productService;

        // Header Fields
        [ObservableProperty] private int _gatePassId = 0;
        [ObservableProperty] private int _sgpId = 0;
        [ObservableProperty] private DateTime _gatePassDate = DateTime.Today;
        [ObservableProperty] private string _partyId = "";
        [ObservableProperty] private string _partyName = "";
        [ObservableProperty] private string _address = "";
        [ObservableProperty] private string _phone = "";
        [ObservableProperty] private string _customerId = "";
        [ObservableProperty] private string _customerName = "";
        [ObservableProperty] private string _cusAddress = "";
        [ObservableProperty] private string _cusPhone = "";
        [ObservableProperty] private string _billNo = "";
        [ObservableProperty] private string _gatePassNo = "";
        [ObservableProperty] private bool _isSale = true;
        [ObservableProperty] private string _narration = "";

        // Line Item Entry Inputs
        [ObservableProperty] private string _currentProductId = "";
        [ObservableProperty] private string _currentProductName = "";
        [ObservableProperty] private string _currentVehicleNo = "";
        [ObservableProperty] private decimal _currentBagsQty = 0;
        [ObservableProperty] private decimal _currentPQty = 0;
        [ObservableProperty] private decimal _currentWeightPerBag = 50;
        [ObservableProperty] private decimal _currentRatePerMound = 0;
        [ObservableProperty] private decimal _currentTotalKgs = 0;
        [ObservableProperty] private decimal _currentMoundStd = 40;

        // Grid Collection
        [ObservableProperty] private ObservableCollection<GatePassItemDto> _items = new();
        [ObservableProperty] private GatePassItemDto? _selectedItem;

        // Totals
        [ObservableProperty] private decimal _totalBags = 0;
        [ObservableProperty] private decimal _totalKgs = 0;
        [ObservableProperty] private string _totalMoundsDisplay = "0-0";

        // Dropdown Sources
        [ObservableProperty] private ObservableCollection<Party> _parties = new();
        [ObservableProperty] private ObservableCollection<Party> _customers = new();
        [ObservableProperty] private ObservableCollection<Product> _products = new();

        public GatePassSaleViewModel(
            IDbContextFactory<AppDbContext> contextFactory,
            IPartyService partyService,
            IProductService productService)
        {
            _contextFactory = contextFactory;
            _partyService = partyService;
            _productService = productService;

            _ = InitializeDataAsync();
        }

        public async Task InitializeDataAsync()
        {
            try
            {
                var partyList = await _partyService.GetAllPartiesAsync();
                Parties = new ObservableCollection<Party>(partyList);
                Customers = new ObservableCollection<Party>(partyList.Where(p => p.PartyType == PartyType.Customer || p.PartyType == PartyType.Both));

                var prodList = await _productService.GetAllProductsAsync();
                Products = new ObservableCollection<Product>(prodList);

                await GenerateNextIdAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Data load error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GenerateNextIdAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var maxId = await context.Database.SqlQueryRaw<int?>("SELECT MAX(GatePassID) AS Value FROM BrokerageGatePass").FirstOrDefaultAsync();
            GatePassId = (maxId ?? 0) + 1;

            var maxSgpId = await context.Database.SqlQueryRaw<int?>("SELECT MAX(SGPID) AS Value FROM BrokerageGatePass WHERE IsSale = 1").FirstOrDefaultAsync();
            SgpId = (maxSgpId ?? 0) + 1;
            GatePassNo = SgpId.ToString();
        }

        partial void OnPartyIdChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var p = Parties.FirstOrDefault(x => x.PartyNo == value || x.Id.ToString() == value || x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (p != null)
            {
                PartyName = p.Name;
                Address = p.Address ?? "";
                Phone = p.Phone ?? "";
            }
        }

        partial void OnCustomerIdChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var c = Customers.FirstOrDefault(x => x.PartyNo == value || x.Id.ToString() == value || x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (c != null)
            {
                CustomerName = c.Name;
                CusAddress = c.Address ?? "";
                CusPhone = c.Phone ?? "";
            }
        }

        partial void OnCurrentProductIdChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var prod = Products.FirstOrDefault(x => x.ProductCode == value || x.Id.ToString() == value || x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (prod != null)
            {
                CurrentProductName = prod.Name;
                CurrentRatePerMound = prod.SalePrice;
            }
        }

        partial void OnCurrentBagsQtyChanged(decimal value) => CalculateCurrentItemKgs();
        partial void OnCurrentWeightPerBagChanged(decimal value) => CalculateCurrentItemKgs();

        private void CalculateCurrentItemKgs()
        {
            CurrentTotalKgs = CurrentBagsQty * CurrentWeightPerBag;
        }

        [RelayCommand]
        private void AddItem()
        {
            if (string.IsNullOrWhiteSpace(CurrentProductName))
            {
                MessageBox.Show("Please select a Product first!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentTotalKgs <= 0 && CurrentBagsQty <= 0)
            {
                MessageBox.Show("Please enter valid Bags Quantity or Total Kgs!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = new GatePassItemDto
            {
                SerialNo = Items.Count + 1,
                ProductId = CurrentProductId,
                ProductName = CurrentProductName,
                VehicleNo = CurrentVehicleNo,
                BagsQty = CurrentBagsQty,
                PQty = CurrentPQty,
                WeightPerBag = CurrentWeightPerBag,
                RatePerMound = CurrentRatePerMound,
                TotalKgs = CurrentTotalKgs,
                MoundStd = CurrentMoundStd
            };

            Items.Add(item);
            RecalculateTotals();

            // Reset line inputs
            CurrentProductId = "";
            CurrentProductName = "";
            CurrentBagsQty = 0;
            CurrentPQty = 0;
            CurrentTotalKgs = 0;
        }

        [RelayCommand]
        private void RemoveItem()
        {
            if (SelectedItem != null)
            {
                Items.Remove(SelectedItem);
                RecalculateTotals();
            }
        }

        private void RecalculateTotals()
        {
            TotalBags = Items.Sum(i => i.BagsQty);
            TotalKgs = Items.Sum(i => i.TotalKgs);
            int mnds = (int)(TotalKgs / 40);
            decimal rem = TotalKgs % 40;
            TotalMoundsDisplay = $"{mnds}-{rem:F1}";
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(PartyName))
            {
                MessageBox.Show("Party Name is required!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Items.Count == 0)
            {
                MessageBox.Show("Please add at least one line item to the Gate Pass!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                // 1. Delete old record if exists (Clean Re-save pattern of AgenSoft)
                await context.Database.ExecuteSqlRawAsync("DELETE FROM BrokerageGatePassBody WHERE GatePassID = {0}", GatePassId);
                await context.Database.ExecuteSqlRawAsync("DELETE FROM BrokerageGatePass WHERE GatePassID = {0}", GatePassId);

                // 2. Insert Header
                string insertHdr = @"
                    INSERT INTO BrokerageGatePass 
                    (GatePassID, GatePassDate, PartyID, CustomerID, IsSale, Narration, GatePassNo, BillNo, SGPID, PGPID) 
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})";

                await context.Database.ExecuteSqlRawAsync(insertHdr,
                    GatePassId, GatePassDate, PartyId, CustomerId, IsSale ? 1 : 0, Narration, GatePassNo, BillNo, SgpId, 0);

                // 3. Insert Body Items
                int sNo = 1;
                foreach (var item in Items)
                {
                    string insertBody = @"
                        INSERT INTO BrokerageGatePassBody 
                        (GatePassID, ProductID, VehicleNo, BagsQty, PQty, WeightPerBag, RatePerMound, TotalKgs, IsSale, IsDeleted, SerialNo, MoundStd, SGPID, PGPID) 
                        VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})";

                    await context.Database.ExecuteSqlRawAsync(insertBody,
                        GatePassId, item.ProductId, item.VehicleNo, item.BagsQty, item.PQty, item.WeightPerBag,
                        item.RatePerMound, item.TotalKgs, IsSale ? 1 : 0, 0, sNo++, item.MoundStd, SgpId, 0);
                }

                MessageBox.Show($"Gate Pass #{GatePassId} (SGP: {SgpId}) successfully saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await ClearAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving Gate Pass: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ClearAsync()
        {
            PartyId = "";
            PartyName = "";
            Address = "";
            Phone = "";
            CustomerId = "";
            CustomerName = "";
            CusAddress = "";
            CusPhone = "";
            BillNo = "";
            Narration = "";
            Items.Clear();
            RecalculateTotals();
            await GenerateNextIdAsync();
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (GatePassId <= 0) return;

            var res = MessageBox.Show($"Are you sure you want to delete Gate Pass #{GatePassId}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    using var context = await _contextFactory.CreateDbContextAsync();
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM BrokerageGatePassBody WHERE GatePassID = {0}", GatePassId);
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM BrokerageGatePass WHERE GatePassID = {0}", GatePassId);
                    MessageBox.Show($"Gate Pass #{GatePassId} deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    await ClearAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting Gate Pass: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
