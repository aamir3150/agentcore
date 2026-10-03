using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using MyMandiSystem.Core.Messages;
using System.Windows;
using MyMandiSystem.Core.Interfaces;

namespace MyMandiSystem.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _currentTime;

        [ObservableProperty]
        private string _currentView = "AccountsForms";

        [ObservableProperty]
        private bool _isTrialVisible = true;

        [ObservableProperty] private string _currentYearDisplay = "Financial Year: 2025-26";
        [ObservableProperty] private bool _isArchiveMode;
        [ObservableProperty] private string _archiveWarning = "";

        public string CurrentUser { get; } = "Admin";
        public string CurrentSeason { get; } = "2025-26";
        public string Metric2 { get; } = "1.245";


        // Daily Activity Metrics
        public string CashInHand { get; } = "1931472";
        public string CashPaid { get; } = "308500";
        public string CashReceived { get; } = "0";
        public string ChequeIssued { get; } = "1000000";
        public string ChequeDeposited { get; } = "0";
        public string CashDeposited { get; } = "0";
        public string UnclearedCheques { get; } = "0";

        private DispatcherTimer _timer;
        private readonly ISystemService _systemService;
        private readonly ISystemConfigService _configService;

        public MainViewModel(ISystemService systemService, ISystemConfigService configService)
        {
            _systemService = systemService;
            _configService = configService;

            CurrentYearDisplay = $"Financial Year: {_configService.GetCurrentFinancialYear()}";
            IsArchiveMode = _configService.IsReadOnly;
            
            if (IsArchiveMode)
            {
                ArchiveWarning = "⚠ ARCHIVED YEAR - READ ONLY MODE";
            }

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => { CurrentTime = DateTime.Now.ToString("HH:mm:ss"); };
            _timer.Start();
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");

            // Register for navigation messages
            CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<NavigationMessage>(this, (r, m) =>
            {
                Navigate(m.Destination);
            });
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            CurrentView = destination;
            IsTrialVisible = false; // Dismiss trial on navigation
        }

        [RelayCommand]
        private void DismissTrial()
        {
            IsTrialVisible = false;
        }

        [RelayCommand]
        private void Users() { /* Placeholder */ }

        [RelayCommand]
        private void Configuration() { /* Placeholder */ }

        [RelayCommand]
        private void BackupDatabase() { /* Placeholder */ }

        [RelayCommand]
        private void RefreshSystem() { /* Placeholder */ }

        [RelayCommand]
        private void ExpenseTypePurchase() { /* Placeholder */ }

        [RelayCommand]
        private void ExpenseTypeSale() { /* Placeholder */ }

        [RelayCommand]
        private void PortSetting() { /* Placeholder */ }

        [RelayCommand]
        private void ReCalculateStock() { /* Placeholder */ }

        [RelayCommand]
        private void SmsSending() { /* Placeholder */ }

        // Transaction Menu Commands
        [RelayCommand] private void ChequeBookGeneration() { /* Placeholder */ }
        [RelayCommand] private void BankChequesEditing() { /* Placeholder */ }
        [RelayCommand] private void BankChequesIssuing() { /* Placeholder */ }
        [RelayCommand] private void BankChequesReconciliation() { /* Placeholder */ }
        
        [RelayCommand] private void CashDepositBank() { /* Placeholder */ }
        [RelayCommand] private void ChequeDepositBank() { /* Placeholder */ }
        private void SafeSetOwner(System.Windows.Window window)
        {
            var main = System.Windows.Application.Current?.MainWindow 
                       ?? System.Windows.Application.Current?.Windows.OfType<MainWindow>().FirstOrDefault();
            if (main != null && main.IsLoaded && main.IsVisible && main != window)
            {
                window.Owner = main;
            }
        }

        [RelayCommand] private void DepositReconciliation() { /* Placeholder */ }
        
        [RelayCommand] 
        private void CashReceivingVoucherEntry() 
        { 
            var window = new Views.CashReceivingVoucherWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<CashReceivingVoucherViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
        }
        [RelayCommand] 
        private void CashPaymentVoucherEntry() 
        { 
            var window = new Views.CashPaymentVoucherWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<CashPaymentVoucherViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
        }
        [RelayCommand] 
        private void CashPaymentVoucherWhtEntry() 
        { 
            // Reuse the same window/vm but we can pass a flag for WHT if needed, 
            // or just open it normally for now as requested.
            CashPaymentVoucherEntry();
        }
        [RelayCommand] 
        private void JournalVoucherEntry() 
        { 
            var window = new Views.JournalVoucherWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<JournalVoucherViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
        }
        
        [RelayCommand] private void ProfitLossSettings() { /* Placeholder */ }
        [RelayCommand] private void BalanceSheetSettings() { /* Placeholder */ }
        [RelayCommand] private void ExpenseReportSettings() { /* Placeholder */ }

        // Catalog Menu Commands
        [RelayCommand] private void CompaniesDef() => OpenMasterDataWindow(0);
        [RelayCommand] private void UnitsDef() => OpenMasterDataWindow(1);
        [RelayCommand] private void TownsDef() => OpenMasterDataWindow(2);
        [RelayCommand] private void SectorsDef() => OpenMasterDataWindow(3);

        private void OpenMasterDataWindow(int tabIndex)
        {
            var window = new Views.MasterDataWindow(tabIndex);
            window.DataContext = App.ServiceProvider?.GetRequiredService<MasterDataViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        [RelayCommand] private void ProductsDef() => OpenProductWindow(0);
        [RelayCommand] private void GroupsDef() => OpenProductWindow(1);
        
        private void OpenProductWindow(int tabIndex)
        {
            var window = new Views.ProductWindow(tabIndex);
            window.DataContext = App.ServiceProvider?.GetRequiredService<ProductViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
        [RelayCommand] private void PartyGroupsDef() { /* Placeholder */ }
        [RelayCommand] private void CustomersDef() => OpenPartyWindow(0);
        [RelayCommand] private void VendorsDef() => OpenPartyWindow(1);
        [RelayCommand] private void SalesmenDef() => OpenPartyWindow(2);

        [RelayCommand] private void OpeningBalancesDef()
        {
            var window = new Views.OpeningBalancesWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<OpeningBalancesViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenPartyWindow(int tabIndex)
        {
            var window = new Views.PartyWindow(tabIndex);
            window.DataContext = App.ServiceProvider?.GetRequiredService<PartyViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
        [RelayCommand] 
        private void AccountsManagement() 
        { 
            var window = new Views.ChartOfAccountsWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<ChartOfAccountsViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
        }
        
        [RelayCommand] private void OpeningBalancesAccounts() { /* Placeholder */ }
        [RelayCommand] private void OpeningStock() { /* Placeholder */ }
        [RelayCommand] private void InsuranceManagement() { /* Placeholder */ }
        [RelayCommand] private void PromisesManagement() { /* Placeholder */ }

        // Brokerage Menu Commands
        [RelayCommand] private void GatePassSale() { /* Placeholder */ }
        [RelayCommand] private void PurchaseContract() { /* Placeholder */ }
        [RelayCommand] private void BrokerageContract() => OpenBrokerageWindow(0);
        [RelayCommand] private void BrokerageGatePass() => OpenBrokerageWindow(1);
        [RelayCommand] private void BrokerageInvoice() => OpenBrokerageWindow(2);
        [RelayCommand] private void BrokeragePurchaseInvoice() => OpenBrokeragePurchaseInvoiceWindow();
        [RelayCommand] private void BrokerageSaleInvoice() => OpenBrokerageSaleInvoiceWindow();

        private void OpenBrokeragePurchaseInvoiceWindow()
        {
            var window = new Views.BrokeragePurchaseInvoiceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<BrokeragePurchaseInvoiceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenBrokerageSaleInvoiceWindow()
        {
            var window = new Views.BrokerageSaleInvoiceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<BrokerageSaleInvoiceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        [RelayCommand] private void BrokerageMultiInvoice() => OpenBrokerageMultiInvoiceWindow();

        private void OpenBrokerageMultiInvoiceWindow()
        {
            var window = new Views.BrokerageMultiInvoiceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<BrokerageMultiInvoiceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        [RelayCommand] private void PestroPurchase() => OpenPestroWindow(1); // 1 is Invoices tab
        [RelayCommand] private void PestroSale() => OpenPestroWindow(1);
        [RelayCommand] private void PestroBatches() => OpenPestroWindow(0); // 0 is Batches tab


        private void OpenPestroWindow(int tabIndex)
        {
            var window = new Views.PestroWindow();
            var vm = App.ServiceProvider?.GetRequiredService<PestroViewModel>();
            if (vm != null)
            {
                vm.SelectedTabIndex = tabIndex;
                window.DataContext = vm;
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        private void OpenBrokerageWindow(int tabIndex)
        {
            var window = new Views.BrokerageWindow();
            var vm = App.ServiceProvider?.GetRequiredService<BrokerageViewModel>();
            if (vm != null)
            {
                vm.SelectedTabIndex = tabIndex;
                window.DataContext = vm;
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        private void OpenReportingWindow()
        {
            var window = new System.Windows.Window { Title = "Reporting Center", Width = 1100, Height = 800, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen };
            window.Content = new Views.ReportingHomeView { DataContext = App.ServiceProvider?.GetRequiredService<ReportingViewModel>() };
            window.Show();
        }

        [RelayCommand] private void BrokerageMultiInvoiceMulti() => OpenBrokerageMultiInvoiceWindow();
        [RelayCommand] private void BrokerageMultiInvoiceNew() => OpenBrokerageMultiInvoiceWindow();
        [RelayCommand] private void BrokerageShortage() { /* Placeholder */ }

        // Pestro Menu Commands
        [RelayCommand] private void PestroOpeningStock() { /* Placeholder */ }
        [RelayCommand] private void PestroPurchaseInvoice() { /* Placeholder */ }
        [RelayCommand] private void PestroPurchaseReturnWithInv() { /* Placeholder */ }
        [RelayCommand] private void PestroPurchaseReturnNoInv() { /* Placeholder */ }
        [RelayCommand] private void PestroChangeExpiryDate() { /* Placeholder */ }
        
        [RelayCommand] private void PestroSaleInvoiceAutoBatch() { /* Placeholder */ }
        [RelayCommand] private void PestroSaleInvoiceManualBatch() { /* Placeholder */ }
        [RelayCommand] private void PestroSaleReturnWithInv() { /* Placeholder */ }
        [RelayCommand] private void PestroSaleReturnNoInv() { /* Placeholder */ }

        // General Purchase Menu Commands
        [RelayCommand] private void GeneralPurchaseInvoice() { /* Placeholder */ }
        [RelayCommand] private void GeneralPurchaseReturnInvoice() { /* Placeholder */ }
        [RelayCommand] private void GeneralPurchaseMultiInvoice() { /* Placeholder */ }

        // General Sale Menu Commands
        [RelayCommand] private void GeneralSaleInvoice() { /* Placeholder */ }
        [RelayCommand] private void GeneralSaleReturnInvoice() { /* Placeholder */ }

        // Account Reports Menu Commands
        [RelayCommand] private void RptAccountLedger() => OpenAccountLedgerWindow();
        [RelayCommand] private void RptAccountLedgerExtended() => OpenAccountLedgerWindow();

        private void OpenAccountLedgerWindow()
        {
            var window = new Views.AccountLedgerExtendedWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<AccountLedgerExtendedViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        [RelayCommand] private void RptPartyLedgerExtended() => OpenReportingWindow();
        
        [RelayCommand] private void RptDayBook() => OpenReportingWindow();
        [RelayCommand] private void RptDailyVouchersDetail() => OpenDailyVouchersDetailWindow();

        private void OpenDailyVouchersDetailWindow()
        {
            var window = new Views.DailyVouchersDetailWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<DailyVouchersDetailViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
        [RelayCommand] private void RptAccountsReceivable() => OpenAccountReceivablesWindow();
        [RelayCommand] private void RptAccountsPayable() => OpenAccountPayablesWindow();

        private void OpenAccountPayablesWindow()
        {
            var window = new Views.AccountPayablesWindow();
            var vm = App.ServiceProvider?.GetRequiredService<AccountPayablesViewModel>();
            if (vm != null)
            {
                vm.WindowTitle = "Account Payables";
                vm.HeaderTitle = "Account Payables";
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenAccountReceivablesWindow()
        {
            var window = new Views.AccountPayablesWindow();
            var vm = App.ServiceProvider?.GetRequiredService<AccountPayablesViewModel>();
            if (vm != null)
            {
                vm.WindowTitle = "Account Receivables";
                vm.HeaderTitle = "Account Receivables";
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
        [RelayCommand] private void RptCashBook() => OpenCashBookWindow();

        private void OpenCashBookWindow()
        {
            var window = new Views.CashBookWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<CashBookViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        [RelayCommand] private void RptGLJournal() => OpenReportingWindow();
        [RelayCommand] private void RptTrialBalance() => OpenTrialBalanceWindow();

        private void OpenTrialBalanceWindow()
        {
            var window = new Views.TrialBalanceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<TrialBalanceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        [RelayCommand] private void RptAccountsBalances() { /* Placeholder */ }
        
        [RelayCommand] private void RptBalanceSheet() => OpenBalanceSheetWindow();

        private void OpenBalanceSheetWindow()
        {
            var window = new Views.BalanceSheetWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<BalanceSheetViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        [RelayCommand] private void RptBankStatement() => OpenBankStatementWindow();

        private void OpenBankStatementWindow()
        {
            var window = new Views.BankStatementWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<BankStatementViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        [RelayCommand] private void RptBankPosition() { /* Placeholder */ }
        [RelayCommand] private void RptBankMarkupCalc() { /* Placeholder */ }
        [RelayCommand] private void RptPartyMarkupCalc() { /* Placeholder */ }
        
        [RelayCommand] private void RptProfitLossStatement() => OpenProfitLossStatementWindow();

        private void OpenProfitLossStatementWindow()
        {
            var window = new Views.ProfitLossStatementWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<ProfitLossStatementViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        [RelayCommand] private void RptMonthlyExpenseChart() { /* Placeholder */ }
        
        [RelayCommand] private void RptBankChequesRegister() { /* Placeholder */ }
        [RelayCommand] private void RptUnclearedCheques() { /* Placeholder */ }
        [RelayCommand] private void RptLostCheques() { /* Placeholder */ }
        [RelayCommand] private void RptBouncedCheques() { /* Placeholder */ }
        
        [RelayCommand] private void RptOpeningBalances() { /* Placeholder */ }
        [RelayCommand] private void RptChartOfAccounts() { /* Placeholder */ }
        [RelayCommand] private void RptInsuranceRegister() { /* Placeholder */ }
        
        [RelayCommand] private void RptUserLogRegister() { /* Placeholder */ }

        // System Reports Menu Commands
        [RelayCommand] private void RptGroupBrokerage() { /* Placeholder */ }
        
        // Brokerage Reports Sub-Menu Commands
        [RelayCommand] private void RptBrkCurrentStock() { /* Placeholder */ }
        [RelayCommand] private void RptBrkCommission() { /* Placeholder */ }
        [RelayCommand] private void RptBrkContractsRegister() { /* Placeholder */ }
        [RelayCommand] private void RptBrkInvoicesRegister() { /* Placeholder */ }
        [RelayCommand] private void RptBrkPurchaseRegister() { /* Placeholder */ }
        [RelayCommand] private void RptBrkSaleRegister() { /* Placeholder */ }
        [RelayCommand] private void RptBrkStockReport() { /* Placeholder */ }
        
        [RelayCommand] private void RptBrkPartyLedgerExtended() { /* Placeholder */ }
        [RelayCommand] private void RptBrkMonthlyCommission() { /* Placeholder */ }
        [RelayCommand] private void RptBrkPurchaseSummary() { /* Placeholder */ }
        [RelayCommand] private void RptBrkPurchaseSummaryPartyWise() { /* Placeholder */ }
        [RelayCommand] private void RptBrkPartyWiseWHT() { /* Placeholder */ }
        [RelayCommand] private void RptBrkVehicleWiseSummary() { /* Placeholder */ }
        [RelayCommand] private void RptBrkGatePassRegister() { /* Placeholder */ }
        [RelayCommand] private void RptBrkProductLedger() { /* Placeholder */ }

        [RelayCommand] private void RptGroupDailyWorking() { /* Placeholder */ }

        // Daily Working Reports Sub-Menu Commands
        [RelayCommand] private void RptDlyBusinessActivity() { /* Placeholder */ }

        [RelayCommand] private void RptGroupLists() { /* Placeholder */ }
        [RelayCommand] private void RptListProduct() { /* Placeholder */ }
        [RelayCommand] private void RptListBarcodePrinting() { /* Placeholder */ }

        [RelayCommand] private void GeneralPurchase() => OpenGeneralInvoiceWindow(Core.Entities.InvoiceType.GenPurchase);
        [RelayCommand] private void GeneralSale() => OpenGeneralInvoiceWindow(Core.Entities.InvoiceType.GenSale);

        private async void OpenGeneralInvoiceWindow(Core.Entities.InvoiceType type)
        {
            var vm = App.ServiceProvider?.GetRequiredService<GeneralInvoiceViewModel>();
            if (vm != null)
            {
                await vm.InitializeAsync(type);
                var window = new Views.GeneralInvoiceWindow();
                window.DataContext = vm;
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand] private void PurchaseReturn() => OpenGeneralInvoiceWindow(Core.Entities.InvoiceType.GenPurchaseReturn);
        [RelayCommand] private void SaleReturn() => OpenGeneralInvoiceWindow(Core.Entities.InvoiceType.GenSaleReturn);

        [RelayCommand] private void RptListParties() { /* Placeholder */ }
        [RelayCommand] private void RptListSectors() { /* Placeholder */ }
        [RelayCommand] private void RptListTowns() { /* Placeholder */ }

        [RelayCommand] private void RptGroupPestro() { /* Placeholder */ }

        // Pestro Reports Sub-Menu Commands
        [RelayCommand] private void RptPestroPurchaseRegister() { /* Placeholder */ }
        [RelayCommand] private void RptPestroPurchaseSummary() { /* Placeholder */ }
        [RelayCommand] private void RptPestroPartyPurchaseLedger() { /* Placeholder */ }
        [RelayCommand] private void RptPestroProductPurchaseLedger() { /* Placeholder */ }
        [RelayCommand] private void RptPestroProductWisePurchases() { /* Placeholder */ }

        [RelayCommand] private void RptPestroPurchaseReturnLedger() { /* Placeholder */ }
        [RelayCommand] private void RptPestroPurchaseReturnSummary() { /* Placeholder */ }

        [RelayCommand] private void RptPestroSaleRegister() { /* Placeholder */ }
        [RelayCommand] private void RptPestroSaleSummary() { /* Placeholder */ }
        [RelayCommand] private void RptPestroProductSaleLedger() { /* Placeholder */ }
        [RelayCommand] private void RptPestroProductWiseSales() { /* Placeholder */ }
        [RelayCommand] private void RptPestroSalesStockStatement() { /* Placeholder */ }
        [RelayCommand] private void RptPestroCustomerTownSale() { /* Placeholder */ }
        [RelayCommand] private void RptPestroCompanySectorSale() { /* Placeholder */ }

        [RelayCommand] private void RptPestroSaleReturnLedger() { /* Placeholder */ }
        [RelayCommand] private void RptPestroSaleReturnSummary() { /* Placeholder */ }
        [RelayCommand] private void RptPestroProductSaleReturnLedger() { /* Placeholder */ }
        [RelayCommand] private void RptPestroPartySaleReturnLedger() { /* Placeholder */ }

        [RelayCommand] private void RptPestroSalesmanSummary() { /* Placeholder */ }
        [RelayCommand] private void RptPestroSalesmanCommission() { /* Placeholder */ }
        [RelayCommand] private void RptPestroSalesmanMonthlyPerformance() { /* Placeholder */ }

        [RelayCommand] private void RptGroupGenPurchase() { /* Placeholder */ }

        // General Purchase Reports Sub-Menu Commands
        [RelayCommand] private void RptGenPurRegister() { /* Placeholder */ }
        [RelayCommand] private void RptGenPurSummary() { /* Placeholder */ }
        [RelayCommand] private void RptGenPurProductLedger() { /* Placeholder */ }
        [RelayCommand] private void RptGenPurProductWise() { /* Placeholder */ }
        [RelayCommand] private void RptGroupGenPurchaseReturn() { /* Placeholder */ }

        // General Purchase Return Reports Sub-Menu Commands
        [RelayCommand] private void RptGenPurReturnRegister() { /* Placeholder */ }
        [RelayCommand] private void RptGenPurReturnSummary() { /* Placeholder */ }
        [RelayCommand] private void RptGenPurReturnProductLedger() { /* Placeholder */ }
        [RelayCommand] private void RptGroupGenSales() { /* Placeholder */ }

        // General Sales Reports Sub-Menu Commands
        [RelayCommand] private void RptGenSalesRegister() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesSummary() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesPartyLedgerExtended() { /* Placeholder */ }

        [RelayCommand] private void RptGenSalesStockStatement() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesProductLedger() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesProductWise() { /* Placeholder */ }

        [RelayCommand] private void RptGenSalesCompanySectorWise() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesCompanyWise() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesCustomerTownSectorWise() { /* Placeholder */ }

        [RelayCommand] private void RptGroupGenSalesReturn() { /* Placeholder */ }

        // General Sales Return Reports Sub-Menu Commands
        [RelayCommand] private void RptGenSalesReturnRegister() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesReturnSummary() { /* Placeholder */ }
        [RelayCommand] private void RptGenSalesReturnProductLedger() { /* Placeholder */ }

        [RelayCommand] private void RptGroupProfits() { /* Placeholder */ }

        // Profits Reports Sub-Menu Commands
        [RelayCommand] private void RptProfitsCompanyWise() { /* Placeholder */ }
        [RelayCommand] private void RptProfitsCustomerWise() { /* Placeholder */ }
        [RelayCommand] private void RptProfitsDaily() { /* Placeholder */ }

        [RelayCommand] private void RptProfitsInvoiceDetailed() { /* Placeholder */ }
        [RelayCommand] private void RptProfitsInvoiceWise() { /* Placeholder */ }
        [RelayCommand] private void RptProfitsProductWise() { /* Placeholder */ }

        [RelayCommand] private void RptGroupStock() { /* Placeholder */ }

        // Stock Reports Sub-Menu Commands
        [RelayCommand] private void RptStockOpening() { /* Placeholder */ }
        [RelayCommand] private void RptStockCurrent() { /* Placeholder */ }
        [RelayCommand] private void RptStockProductActivity() { /* Placeholder */ }

        [RelayCommand] private void RptStockPestroOpening() { /* Placeholder */ }
        [RelayCommand] private void RptStockPestroCurrent() { /* Placeholder */ }
        [RelayCommand] private void RptStockPestroExpired() { /* Placeholder */ }
        [RelayCommand] private void RptStockPestroCountSheet() { /* Placeholder */ }

        [RelayCommand]
        private void LogOff()
        {
            CurrentView = "AccountsForms";
            IsTrialVisible = true;
        }

        [RelayCommand]
        private void Exit()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
