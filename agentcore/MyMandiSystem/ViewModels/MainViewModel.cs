using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using MyMandiSystem.Core.Messages;
using System.Windows;
using MyMandiSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Infrastructure;
using MyMandiSystem.Helpers;
using System.IO;
using System.Diagnostics;

namespace MyMandiSystem.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _currentTime;

        [ObservableProperty]
        private string _currentView = "AccountsForms";

        [ObservableProperty]
        private bool _isTrialVisible = false;

        [ObservableProperty] private string _currentYearDisplay = "Financial Year: 2025-26";
        [ObservableProperty] private bool _isArchiveMode;
        [ObservableProperty] private string _archiveWarning = "";

        [ObservableProperty] private string _currentUser = "Admin";
        [ObservableProperty] private string _currentSeason = "";
        [ObservableProperty] private string _metric2 = "0";

        // Daily Activity Metrics (Real Live Data)
        [ObservableProperty] private string _cashInHand = "0";
        [ObservableProperty] private string _cashPaid = "0";
        [ObservableProperty] private string _cashReceived = "0";
        [ObservableProperty] private string _chequeIssued = "0";
        [ObservableProperty] private string _chequeDeposited = "0";
        [ObservableProperty] private string _cashDeposited = "0";
        [ObservableProperty] private string _unclearedCheques = "0";

        private DispatcherTimer _timer;
        private readonly ISystemService _systemService;
        private readonly ISystemConfigService _configService;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountService _accountService;

        public MainViewModel(
            ISystemService systemService, 
            ISystemConfigService configService,
            IDbContextFactory<AppDbContext> contextFactory,
            ICurrentUserService currentUserService,
            IAccountService accountService)
        {
            try { System.IO.File.AppendAllText(@"D:\Projects\agentcore\agentcore\startup.log", $"[{DateTime.Now:HH:mm:ss.fff}] MainViewModel constructor entered.\r\n"); } catch {}
            _systemService = systemService;
            _configService = configService;
            _contextFactory = contextFactory;
            _currentUserService = currentUserService;
            _accountService = accountService;

            CurrentYearDisplay = $"Financial Year: {_configService.GetCurrentFinancialYear()}";
            IsArchiveMode = _configService.IsReadOnly;
            CurrentUser = _currentUserService.GetCurrentUserName();
            
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

            // Register for dashboard refresh messages
            CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<RefreshDashboardMessage>(this, async (r, m) =>
            {
                await LoadDashboardMetricsAsync();
            });

            // Load Real Metrics from Database
            _ = LoadDashboardMetricsAsync();
        }

        public async Task LoadDashboardMetricsAsync()
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var todayStart = DateTime.Today;
                var todayEnd = todayStart.AddDays(1);

                decimal totalCashInHand = 0m;
                decimal todayCashReceived = 0m;
                decimal todayCashPaid = 0m;
                decimal chequesIssued = 0m;
                decimal chequesDeposited = 0m;

                bool nativeHandled = false;
                try
                {
                    var conn = context.Database.GetDbConnection();
                    if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                    using var cmd = conn.CreateCommand();

                    // Check if native ChartOfAccounts exists
                    cmd.CommandText = "SELECT OBJECT_ID('ChartOfAccounts', 'U')";
                    var obj = await cmd.ExecuteScalarAsync();
                    if (obj != null && obj != DBNull.Value)
                    {
                        // 1. Native Cash In Hand (ChartOfAccounts Opening + Vouchers transactions)
                        cmd.CommandText = @"
                            DECLARE @opening DECIMAL(18,2) = 0;
                            SELECT @opening = ISNULL(SUM(ISNULL(OpeningDebit, 0) - ISNULL(OpeningCredit, 0)), 0)
                            FROM ChartOfAccounts
                            WHERE AccountNo = '1001' OR AccountName LIKE '%cash%' OR AccountNameUrdu LIKE N'%روکڑ%';

                            DECLARE @tx DECIMAL(18,2) = 0;
                            SELECT @tx = ISNULL(SUM(Debit), 0) - ISNULL(SUM(Credit), 0)
                            FROM (
                                SELECT Credit AS Debit, 0 AS Credit FROM CreditVouchersBody
                                UNION ALL
                                SELECT 0 AS Debit, Debit AS Credit FROM DebitVouchersBody
                                UNION ALL
                                SELECT Debit, Credit FROM JournalVouchersBody WHERE AccountNo = '1001'
                                UNION ALL
                                SELECT Debit, Credit FROM VuAccVouchersOthers WHERE AccountNo = '1001'
                            ) T;

                            SELECT (@opening + @tx);";
                        var resCash = await cmd.ExecuteScalarAsync();
                        if (resCash != null && resCash != DBNull.Value)
                            totalCashInHand = Convert.ToDecimal(resCash);

                        // 2. Today's Inflow (CreditVouchersBody)
                        cmd.CommandText = @"
                            SELECT ISNULL(SUM(Credit), 0) 
                            FROM CreditVouchersBody 
                            WHERE VoucherDate >= @todayStart AND VoucherDate < @todayEnd";
                        cmd.Parameters.Clear();
                        var p1 = cmd.CreateParameter(); p1.ParameterName = "@todayStart"; p1.Value = todayStart; cmd.Parameters.Add(p1);
                        var p2 = cmd.CreateParameter(); p2.ParameterName = "@todayEnd"; p2.Value = todayEnd; cmd.Parameters.Add(p2);
                        var resInflow = await cmd.ExecuteScalarAsync();
                        if (resInflow != null && resInflow != DBNull.Value)
                            todayCashReceived = Convert.ToDecimal(resInflow);

                        // 3. Today's Outflow (DebitVouchersBody)
                        cmd.CommandText = @"
                            SELECT ISNULL(SUM(Debit), 0) 
                            FROM DebitVouchersBody 
                            WHERE VoucherDate >= @todayStart AND VoucherDate < @todayEnd";
                        var resOutflow = await cmd.ExecuteScalarAsync();
                        if (resOutflow != null && resOutflow != DBNull.Value)
                            todayCashPaid = Convert.ToDecimal(resOutflow);

                        // 4. Cheques Issued (BankCheques)
                        cmd.CommandText = @"SELECT ISNULL(SUM(Amount), 0) FROM BankCheques WHERE ReconcileDate IS NULL";
                        cmd.Parameters.Clear();
                        var resCheques = await cmd.ExecuteScalarAsync();
                        if (resCheques != null && resCheques != DBNull.Value)
                            chequesIssued = Convert.ToDecimal(resCheques);

                        // 5. Cheques Deposited (BankDeposit)
                        cmd.CommandText = @"SELECT ISNULL(SUM(Amount), 0) FROM BankDeposit WHERE ReconcileDate IS NOT NULL";
                        var resDep = await cmd.ExecuteScalarAsync();
                        if (resDep != null && resDep != DBNull.Value)
                            chequesDeposited = Convert.ToDecimal(resDep);

                        nativeHandled = true;
                    }
                }
                catch (Exception exNative)
                {
                    try {
                        System.IO.File.AppendAllText(@"D:\Projects\agentcore\agentcore\startup.log", $"[{DateTime.Now:HH:mm:ss.fff}] [NativeDashboardMetrics ERROR] {exNative}\r\n");
                    } catch {}
                }

                if (!nativeHandled)
                {
                    // Fallback to modern EF Core entities if available
                    try
                    {
                        var cashAccounts = await context.Accounts
                            .Where(a => !a.IsDeleted && (a.AccountNo == "1001" || a.Name.Contains("Cash") || a.Name.Contains("روکڑ") || a.Name == "General Cash Account"))
                            .ToListAsync();

                        if (cashAccounts.Any())
                        {
                            foreach (var ca in cashAccounts)
                            {
                                totalCashInHand += await _accountService.GetAccountBalanceAsync(ca.Id);
                            }
                        }
                        else
                        {
                            var totalReceipts = await context.Vouchers
                                .Where(v => v.VoucherType == Core.Enums.VoucherType.CashReceiving && v.Status != Core.Entities.VoucherStatus.Reversed)
                                .SumAsync(v => (decimal?)v.TotalDebit) ?? 0m;

                            var totalPayments = await context.Vouchers
                                .Where(v => v.VoucherType == Core.Enums.VoucherType.CashPayment && v.Status != Core.Entities.VoucherStatus.Reversed)
                                .SumAsync(v => (decimal?)v.TotalDebit) ?? 0m;

                            totalCashInHand = totalReceipts - totalPayments;
                        }

                        var cashAccountIds = cashAccounts.Select(a => a.Id).ToList();
                        todayCashReceived = await context.VoucherDetails
                            .Where(vd => vd.Voucher.VoucherType == Core.Enums.VoucherType.CashReceiving 
                                     && vd.Voucher.VoucherDate >= todayStart && vd.Voucher.VoucherDate < todayEnd 
                                     && vd.Voucher.Status != Core.Entities.VoucherStatus.Reversed
                                     && !cashAccountIds.Contains(vd.AccountId))
                            .SumAsync(vd => (decimal?)vd.Credit) ?? 0m;

                        todayCashPaid = await context.VoucherDetails
                            .Where(vd => vd.Voucher.VoucherType == Core.Enums.VoucherType.CashPayment 
                                     && vd.Voucher.VoucherDate >= todayStart && vd.Voucher.VoucherDate < todayEnd 
                                     && vd.Voucher.Status != Core.Entities.VoucherStatus.Reversed
                                     && !cashAccountIds.Contains(vd.AccountId))
                            .SumAsync(vd => (decimal?)vd.Debit) ?? 0m;

                        chequesIssued = await context.Cheques
                            .Where(c => c.Status == Core.Entities.ChequeStatus.Issued)
                            .SumAsync(c => (decimal?)c.Amount) ?? 0m;

                        chequesDeposited = await context.Cheques
                            .Where(c => c.Status == Core.Entities.ChequeStatus.Cleared)
                            .SumAsync(c => (decimal?)c.Amount) ?? 0m;
                    }
                    catch { }
                }

                CashInHand = totalCashInHand.ToString("N0");
                CashReceived = todayCashReceived.ToString("N0");
                CashPaid = todayCashPaid.ToString("N0");
                ChequeIssued = chequesIssued.ToString("N0");
                ChequeDeposited = chequesDeposited.ToString("N0");

                var activeSeasons = await _systemService.GetActiveSeasonsAsync();
                CurrentSeason = activeSeasons.FirstOrDefault()?.Name ?? _configService.GetCurrentFinancialYear();
            }
            catch (Exception ex)
            {
                try {
                    System.IO.File.AppendAllText(@"D:\Projects\agentcore\agentcore\startup.log", $"[{DateTime.Now:HH:mm:ss.fff}] [LoadDashboardMetricsAsync ERROR] {ex}\r\n");
                } catch {}

                CashInHand = "0";
                CashReceived = "0";
                CashPaid = "0";
                ChequeIssued = "0";
                ChequeDeposited = "0";
            }
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
        private async Task ShowDailyActivity()
        {
            IsTrialVisible = true;
            await LoadDashboardMetricsAsync();
        }

        [RelayCommand]
        private void Users()
        {
            var vm = App.ServiceProvider?.GetRequiredService<UsersManagementViewModel>();
            if (vm != null)
            {
                var window = new Views.UsersManagementWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void Configuration()
        {
            var vm = App.ServiceProvider?.GetRequiredService<BusinessProfileViewModel>();
            if (vm != null)
            {
                var window = new Views.BusinessProfileWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void BackupDatabase()
        {
            var vm = App.ServiceProvider?.GetRequiredService<DatabaseBackupViewModel>();
            if (vm != null)
            {
                var window = new Views.DatabaseBackupWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void OpenDatabaseTool()
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var toolExe = Path.Combine(baseDir, "agentcore.DbTool.exe");
                if (!File.Exists(toolExe))
                {
                    var altPath = @"D:\Projects\agentcore\agentcore\agentcore.DbTool\bin\Debug\net8.0-windows\agentcore.DbTool.exe";
                    if (File.Exists(altPath)) toolExe = altPath;
                }

                if (File.Exists(toolExe))
                {
                    Process.Start(new ProcessStartInfo(toolExe) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("agentcore.DbTool.exe فائل نہیں ملی۔ برائے مہربانی ٹول بلڈ کریں۔", "فائل غائب", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ڈیٹا بیس ٹول چلاتے وقت خرابی:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task RefreshSystem() 
        { 
            await LoadDashboardMetricsAsync(); 
            MessageBox.Show("سسٹم ڈیٹا اور ڈیش بورڈ کیشے کامیابی سے ریفریش ہو گئے!", "ریفریش مکمل", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ExpenseTypePurchase()
        {
            var vm = App.ServiceProvider?.GetRequiredService<ExpenseTypesViewModel>();
            if (vm != null)
            {
                var window = new Views.ExpenseTypesWindow(vm, 0);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void ExpenseTypeSale()
        {
            var vm = App.ServiceProvider?.GetRequiredService<ExpenseTypesViewModel>();
            if (vm != null)
            {
                var window = new Views.ExpenseTypesWindow(vm, 1);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void PortSetting()
        {
            var vm = App.ServiceProvider?.GetRequiredService<PortSettingViewModel>();
            if (vm != null)
            {
                var window = new Views.PortSettingWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void ReCalculateStock()
        {
            var vm = App.ServiceProvider?.GetRequiredService<StockRecalculationViewModel>();
            if (vm != null)
            {
                var window = new Views.StockRecalculationWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void SmsSending()
        {
            var vm = App.ServiceProvider?.GetRequiredService<SmsSendingViewModel>();
            if (vm != null)
            {
                var window = new Views.SmsSendingWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private void LogOff()
        {
            var result = MessageBox.Show("کیا آپ واقعی موجودہ سیشن سے لاگ آف (Log off) ہونا چاہتے ہیں؟", 
                "لاگ آف کی تصدیق", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(Environment.ProcessPath ?? Environment.CommandLine);
                System.Windows.Application.Current.Shutdown();
            }
        }

        [RelayCommand]
        private void Exit()
        {
            var result = MessageBox.Show("کیا آپ واقعی agentcore ERP کو بند کرنا چاہتے ہیں؟", 
                "بند کرنے کی تصدیق / Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        // Transaction Menu Commands - Banking & Cheques
        [RelayCommand]
        private void ChequeBookGeneration()
        {
            var window = new Views.ChequeBookGenerationWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<ChequeBookGenerationViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void BankChequesIssuing() 
        { 
            var window = new Views.BankChequeIssueWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<BankChequeIssueViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void CashDepositBank() 
        { 
            var window = new Views.CashDepositWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<CashDepositViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void ChequeDepositBank() 
        { 
            var window = new Views.ChequeDepositWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<ChequeDepositViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void BankChequesReconciliation()
        {
            var vm = App.ServiceProvider?.GetRequiredService<BankChequesReconciliationViewModel>();
            if (vm != null)
            {
                var window = new Views.BankChequesReconciliationWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
                _ = LoadDashboardMetricsAsync();
            }
        }
        [RelayCommand] 
        private void DepositReconciliation() 
        { 
            var window = new Views.DepositReconciliationWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<DepositReconciliationViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void BankChequesEditing()
        {
            var vm = App.ServiceProvider?.GetRequiredService<BankChequeEditingViewModel>();
            if (vm != null)
            {
                var window = new Views.BankChequeEditingWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
                _ = LoadDashboardMetricsAsync();
            }
        }

        private void OpenBankingWindow(int tabIndex = 0, string? depositType = null, bool? isDepositRecon = null)
        {
            var vm = App.ServiceProvider?.GetRequiredService<BankingViewModel>();
            if (vm != null)
            {
                if (!string.IsNullOrEmpty(depositType))
                {
                    vm.SelectedDepositType = depositType;
                }
                if (isDepositRecon.HasValue)
                {
                    if (isDepositRecon.Value)
                    {
                        vm.IsReconcilingDeposits = true;
                        vm.IsReconcilingCheques = false;
                    }
                    else
                    {
                        vm.IsReconcilingCheques = true;
                        vm.IsReconcilingDeposits = false;
                    }
                }
                var window = new Views.BankingWindow(vm, tabIndex);
                SafeSetOwner(window);
                window.ShowDialog();
                _ = LoadDashboardMetricsAsync();
            }
        }

        private void SafeSetOwner(System.Windows.Window window)
        {
            var main = System.Windows.Application.Current?.MainWindow 
                       ?? System.Windows.Application.Current?.Windows.OfType<MainWindow>().FirstOrDefault();
            if (main != null && main.IsLoaded && main.IsVisible && main != window)
            {
                window.Owner = main;
            }
        }
        
        [RelayCommand] 
        private void CashReceivingVoucherEntry() 
        { 
            var window = new Views.CashReceivingVoucherWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<CashReceivingVoucherViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void CashPaymentVoucherEntry() 
        { 
            var window = new Views.CashPaymentVoucherWindow();
            var vm = App.ServiceProvider?.GetRequiredService<CashPaymentVoucherViewModel>();
            if (vm != null)
            {
                vm.IsWhtMode = false;
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void CashPaymentVoucherWhtEntry() 
        { 
            var window = new Views.CashPaymentVoucherWindow();
            var vm = App.ServiceProvider?.GetRequiredService<CashPaymentVoucherViewModel>();
            if (vm != null)
            {
                vm.IsWhtMode = true;
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        [RelayCommand] 
        private void JournalVoucherEntry() 
        { 
            var window = new Views.JournalVoucherWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<JournalVoucherViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        
        [RelayCommand] 
        private void ProfitLossSettings() 
        {
            var vm = App.ServiceProvider?.GetRequiredService<ProfitLossSettingsViewModel>();
            if (vm != null)
            {
                var window = new Views.ProfitLossSettingsWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand] 
        private void BalanceSheetSettings() 
        {
            var vm = App.ServiceProvider?.GetRequiredService<BalanceSheetSettingsViewModel>();
            if (vm != null)
            {
                var window = new Views.BalanceSheetSettingsWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand] 
        private void ExpenseReportSettings() 
        {
            var vm = App.ServiceProvider?.GetRequiredService<ExpenseReportSettingsViewModel>();
            if (vm != null)
            {
                var window = new Views.ExpenseReportSettingsWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        // Catalog Menu Commands
        [RelayCommand] private void CompaniesDef() => OpenMasterDataWindow(0);
        [RelayCommand] private void UnitsDef() => OpenMasterDataWindow(1);
        [RelayCommand] private void TownsDef() => OpenMasterDataWindow(2);
        [RelayCommand] private void SectorsDef() => OpenMasterDataWindow(3);
        [RelayCommand] private void CropSeasonsDef() => OpenMasterDataWindow(4);

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
        
        [RelayCommand] private void PartyGroupsDef() => OpenPartyWindow(3);
        [RelayCommand] private void CustomersDef() => OpenPartyWindow(0);
        [RelayCommand] private void VendorsDef() => OpenPartyWindow(1);
        [RelayCommand] private void SalesmenDef() => OpenPartyWindow(2);

        [RelayCommand] private void OpeningBalancesDef() => OpenOpeningBalancesWindow(0);
        [RelayCommand] private void OpeningBalancesAccounts() => OpenOpeningBalancesWindow(0);
        [RelayCommand] private void OpeningStock() => OpenOpeningBalancesWindow(1);

        private void OpenOpeningBalancesWindow(int tabIndex = 0)
        {
            var vm = App.ServiceProvider?.GetRequiredService<OpeningBalancesViewModel>();
            if (vm != null) vm.SelectedTabIndex = tabIndex;
            var window = new Views.OpeningBalancesWindow { DataContext = vm };
            SafeSetOwner(window);
            window.ShowDialog();
            _ = LoadDashboardMetricsAsync();
        }

        private void OpenPartyWindow(int tabIndex)
        {
            var vm = App.ServiceProvider?.GetRequiredService<PartyViewModel>();
            if (vm != null) vm.SelectedTabIndex = tabIndex;
            var window = new Views.PartyWindow(tabIndex);
            window.DataContext = vm;
            SafeSetOwner(window);
            window.ShowDialog();
            _ = LoadDashboardMetricsAsync();
        }
        
        [RelayCommand] 
        private void AccountsManagement() 
        { 
            var window = new Views.ChartOfAccountsWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<ChartOfAccountsViewModel>();
            SafeSetOwner(window);
            window.ShowDialog(); 
            _ = LoadDashboardMetricsAsync();
        }
        
        [RelayCommand] 
        private void InsuranceManagement()
        {
            var window = new Views.InsuranceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<InsuranceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        [RelayCommand] 
        private void PromisesManagement()
        {
            var window = new Views.PromisesWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<PromisesViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        // Brokerage Menu Commands (All 9 Options mapped exactly to AgenSoft)
        [RelayCommand] private void GatePassSale() => OpenGatePassSaleWindow();
        [RelayCommand] private void PurchaseContract() => OpenPurchaseContractWindow();
        [RelayCommand] private void SaleContract() => OpenSaleContractWindow();
        [RelayCommand] private void BrokeragePurchaseInvoice() => OpenBrokeragePurchaseInvoiceWindow();
        [RelayCommand] private void BrokerageSaleInvoice() => OpenBrokerageSaleInvoiceWindow();
        [RelayCommand] private void BrokerageMultiInvoice() => OpenBrokerageMultiInvoiceWindow("Brokerage Multi Invoice");
        [RelayCommand] private void BrokerageMultiInvoiceMulti() => OpenBrokerageMultiInvoiceWindow("Brokerage Multi Invoice Multi");
        [RelayCommand] private void BrokerageMultiInvoiceNew() => OpenBrokerageMultiInvoiceWindow("Brokerage Multi Invoice (New)");
        [RelayCommand] private void BrokerageShortage() => OpenBrokerageShortageWindow();

        [RelayCommand] private void BrokerageContract() => OpenPurchaseContractWindow();
        [RelayCommand] private void BrokerageGatePass() => OpenGatePassSaleWindow();
        [RelayCommand] private void BrokerageInvoice() => OpenBrokeragePurchaseInvoiceWindow();

        private void OpenGatePassSaleWindow()
        {
            var vm = App.ServiceProvider?.GetRequiredService<GatePassSaleViewModel>();
            if (vm != null)
            {
                var window = new Views.GatePassSaleWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        private void OpenPurchaseContractWindow()
        {
            var vm = App.ServiceProvider?.GetRequiredService<PurchaseContractViewModel>();
            if (vm != null)
            {
                var window = new Views.PurchaseContractWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        private void OpenSaleContractWindow()
        {
            var vm = App.ServiceProvider?.GetRequiredService<SaleContractViewModel>();
            if (vm != null)
            {
                var window = new Views.SaleContractWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

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

        private void OpenBrokerageMultiInvoiceWindow(string title = "Brokerage Multi Invoice")
        {
            var window = new Views.BrokerageMultiInvoiceWindow();
            window.Title = title;
            window.DataContext = App.ServiceProvider?.GetRequiredService<BrokerageMultiInvoiceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenBrokerageShortageWindow()
        {
            var vm = App.ServiceProvider?.GetRequiredService<BrokerageShortageViewModel>();
            if (vm != null)
            {
                var window = new Views.BrokerageShortageWindow(vm);
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        [RelayCommand] private void PestroPurchase() => OpenPestroPurchaseInvoiceWindow();
        [RelayCommand] private void PestroSale() => OpenPestroSaleAutoBatchWindow();
        [RelayCommand] private void PestroBatches() => OpenPestroWindow(0); // 0 is Batches tab

        private void OpenPestroPurchaseInvoiceWindow()
        {
            var window = new Views.PestroPurchaseInvoiceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<PestroPurchaseInvoiceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenPestroSaleAutoBatchWindow()
        {
            var window = new Views.PestroSaleAutoBatchWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<PestroSaleAutoBatchViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenPestroSaleManualBatchWindow()
        {
            var window = new Views.PestroSaleManualBatchWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<PestroSaleManualBatchViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenPestroOpeningStockWindow()
        {
            var window = new Views.PestroOpeningStockWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<PestroOpeningStockViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenPestroPurchaseReturnWindow(bool isWithInvoice)
        {
            var vm = App.ServiceProvider?.GetRequiredService<PestroPurchaseReturnViewModel>();
            if (vm != null)
            {
                _ = vm.InitializeAsync(isWithInvoice);
                var window = new Views.PestroPurchaseReturnWindow();
                window.DataContext = vm;
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

        private void OpenPestroChangeExpiryDateWindow()
        {
            var window = new Views.PestroChangeExpiryDateWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<PestroChangeExpiryDateViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenPestroSaleReturnWindow(bool isWithInvoice)
        {
            var vm = App.ServiceProvider?.GetRequiredService<PestroSaleReturnViewModel>();
            if (vm != null)
            {
                _ = vm.InitializeAsync(isWithInvoice);
                var window = new Views.PestroSaleReturnWindow();
                window.DataContext = vm;
                SafeSetOwner(window);
                window.ShowDialog();
            }
        }

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

        // Pestro Menu Commands - Authentic AgenSoft Routing
        [RelayCommand] private void PestroOpeningStock() => OpenPestroOpeningStockWindow();
        [RelayCommand] private void PestroPurchaseInvoice() => OpenPestroPurchaseInvoiceWindow();
        [RelayCommand] private void PestroPurchaseReturnWithInv() => OpenPestroPurchaseReturnWindow(true);
        [RelayCommand] private void PestroPurchaseReturnNoInv() => OpenPestroPurchaseReturnWindow(false);
        [RelayCommand] private void PestroChangeExpiryDate() => OpenPestroChangeExpiryDateWindow();
        
        [RelayCommand] private void PestroSaleInvoiceAutoBatch() => OpenPestroSaleAutoBatchWindow();
        [RelayCommand] private void PestroSaleInvoiceManualBatch() => OpenPestroSaleManualBatchWindow();
        [RelayCommand] private void PestroSaleReturnWithInv() => OpenPestroSaleReturnWindow(true);
        [RelayCommand] private void PestroSaleReturnNoInv() => OpenPestroSaleReturnWindow(false);

        // General Purchase Menu Commands
        [RelayCommand] private void GeneralPurchaseInvoice() => OpenGeneralPurchaseInvoiceWindow(false);
        [RelayCommand] private void GeneralPurchaseReturnInvoice() => OpenGeneralPurchaseInvoiceWindow(true);
        [RelayCommand] private void GeneralPurchaseMultiInvoice() => OpenGeneralMultiInvoiceWindow();

        // General Sale Menu Commands
        [RelayCommand] private void GeneralSaleInvoice() => OpenGeneralSaleInvoiceWindow(false);
        [RelayCommand] private void GeneralSaleReturnInvoice() => OpenGeneralSaleInvoiceWindow(true);

        private void OpenGeneralPurchaseInvoiceWindow(bool isReturn)
        {
            var window = new Views.GeneralPurchaseInvoiceWindow();
            var vm = App.ServiceProvider?.GetRequiredService<GeneralPurchaseInvoiceViewModel>();
            if (vm != null)
            {
                vm.SetReturnMode(isReturn);
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenGeneralSaleInvoiceWindow(bool isReturn)
        {
            var window = new Views.GeneralSaleInvoiceWindow();
            var vm = App.ServiceProvider?.GetRequiredService<GeneralSaleInvoiceViewModel>();
            if (vm != null)
            {
                vm.SetReturnMode(isReturn);
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenGeneralMultiInvoiceWindow()
        {
            var window = new Views.GeneralMultiInvoiceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<GeneralMultiInvoiceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenAccountLedgerWindow()
        {
            var window = new Views.AccountLedgerWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<AccountLedgerViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        // Account Reports Menu Commands (All 26 Reports from AgenSoft)
        [RelayCommand] private void RptAccountLedger() => OpenAccountLedgerWindow();
        [RelayCommand] private void RptAccountLedgerExtended() => OpenAccountLedgerExtendedWindow();
        [RelayCommand] private void RptPartyLedgerExtended() => OpenAccountReportsWindow(AccountReportMode.PartyLedgerExtended);
        [RelayCommand] private void RptCashBook() => OpenCashBookWindow(isDayBook: false);
        [RelayCommand] private void RptDayBook() => OpenCashBookWindow(isDayBook: true);
        [RelayCommand] private void RptDailyVouchersDetail() => OpenDailyVouchersDetailWindow(isGLJournal: false);
        [RelayCommand] private void RptAccountsReceivable() => OpenAccountReceivablesWindow();
        [RelayCommand] private void RptAccountsPayable() => OpenAccountPayablesWindow();
        [RelayCommand] private void RptGLJournal() => OpenDailyVouchersDetailWindow(isGLJournal: true);
        [RelayCommand] private void RptTrialBalance() => OpenTrialBalanceWindow();
        [RelayCommand] private void RptAccountsBalances() => OpenAccountReportsWindow(AccountReportMode.AccountsBalances);
        [RelayCommand] private void RptBalanceSheet() => OpenBalanceSheetWindow();
        [RelayCommand] private void RptBankStatement() => OpenBankStatementWindow(BankReportMode.Statement);
        [RelayCommand] private void RptBankPosition() => OpenBankStatementWindow(BankReportMode.Position);
        [RelayCommand] private void RptBankMarkupCalc() => OpenMarkupCalculatorWindow(MarkupCalculatorMode.BankMarkup);
        [RelayCommand] private void RptPartyMarkupCalc() => OpenMarkupCalculatorWindow(MarkupCalculatorMode.PartyMarkup);
        [RelayCommand] private void RptProfitLossStatement() => OpenProfitLossStatementWindow();
        [RelayCommand] private void RptMonthlyExpenseChart() => OpenMonthlyExpenseChartWindow();
        [RelayCommand] private void RptBankChequesRegister() => OpenBankStatementWindow(BankReportMode.ChequesRegister);
        [RelayCommand] private void RptUnclearedCheques() => OpenBankStatementWindow(BankReportMode.UnclearedCheques);
        [RelayCommand] private void RptLostCheques() => OpenBankStatementWindow(BankReportMode.LostCheques);
        [RelayCommand] private void RptBouncedCheques() => OpenBankStatementWindow(BankReportMode.BouncedCheques);
        [RelayCommand] private void RptOpeningBalances() => OpenAccountReportsWindow(AccountReportMode.OpeningBalances);
        [RelayCommand] private void RptChartOfAccounts() => OpenAccountReportsWindow(AccountReportMode.ChartOfAccounts);
        [RelayCommand] private void RptInsuranceRegister() => OpenInsuranceRegisterWindow();
        [RelayCommand] private void RptUserLogRegister() => OpenUserLogRegisterWindow();

        private void OpenAccountReportsWindow(AccountReportMode mode)
        {
            var window = new Views.AccountReportsWindow();
            var vm = App.ServiceProvider?.GetRequiredService<AccountReportsViewModel>();
            if (vm != null)
            {
                vm.SetReportMode(mode);
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenAccountLedgerExtendedWindow()
        {
            var window = new Views.AccountLedgerExtendedWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<AccountLedgerExtendedViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
        private void OpenDailyVouchersDetailWindow(bool isGLJournal)
        {
            var window = new Views.DailyVouchersDetailWindow();
            var vm = App.ServiceProvider?.GetRequiredService<DailyVouchersDetailViewModel>();
            if (vm != null)
            {
                if (isGLJournal) vm.SetGLJournalMode();
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
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
        
        private void OpenCashBookWindow(bool isDayBook)
        {
            var window = new Views.CashBookWindow();
            var vm = App.ServiceProvider?.GetRequiredService<CashBookViewModel>();
            if (vm != null)
            {
                vm.SetDayBookMode(isDayBook);
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenTrialBalanceWindow()
        {
            var window = new Views.TrialBalanceWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<TrialBalanceViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
        private void OpenBalanceSheetWindow()
        {
            var window = new Views.BalanceSheetWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<BalanceSheetViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenBankStatementWindow(BankReportMode mode)
        {
            var window = new Views.BankStatementWindow();
            var vm = App.ServiceProvider?.GetRequiredService<BankStatementViewModel>();
            if (vm != null)
            {
                vm.SetMode(mode);
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenMarkupCalculatorWindow(MarkupCalculatorMode mode)
        {
            var window = new Views.MarkupCalculatorWindow();
            var vm = App.ServiceProvider?.GetRequiredService<MarkupCalculatorViewModel>();
            if (vm != null)
            {
                _ = vm.SetModeAsync(mode);
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }
        
        private void OpenProfitLossStatementWindow()
        {
            var window = new Views.ProfitLossStatementWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<ProfitLossStatementViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenMonthlyExpenseChartWindow()
        {
            var window = new Views.MonthlyExpenseChartWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<MonthlyExpenseChartViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenInsuranceRegisterWindow()
        {
            var window = new Views.InsuranceRegisterWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<InsuranceRegisterViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        private void OpenUserLogRegisterWindow()
        {
            var window = new Views.UserLogRegisterWindow();
            window.DataContext = App.ServiceProvider?.GetRequiredService<UserLogRegisterViewModel>();
            SafeSetOwner(window);
            window.ShowDialog();
        }

        // System Reports Menu Commands
        [RelayCommand] private void RptGroupBrokerage() => OpenBrokerageReportsWindow(BrokerageReportMode.CurrentStock);
        
        // Brokerage Reports Sub-Menu Commands
        [RelayCommand] private void RptBrkCurrentStock() => OpenBrokerageReportsWindow(BrokerageReportMode.CurrentStock);
        [RelayCommand] private void RptBrkCommission() => OpenBrokerageReportsWindow(BrokerageReportMode.CommissionReport);
        [RelayCommand] private void RptBrkContractsRegister() => OpenBrokerageReportsWindow(BrokerageReportMode.ContractsRegister);
        [RelayCommand] private void RptBrkInvoicesRegister() => OpenBrokerageReportsWindow(BrokerageReportMode.InvoicesRegister);
        [RelayCommand] private void RptBrkPurchaseRegister() => OpenBrokerageReportsWindow(BrokerageReportMode.PurchaseRegister);
        [RelayCommand] private void RptBrkSaleRegister() => OpenBrokerageReportsWindow(BrokerageReportMode.SaleRegister);
        [RelayCommand] private void RptBrkStockReport() => OpenBrokerageReportsWindow(BrokerageReportMode.StockReport);
        
        [RelayCommand] private void RptBrkPartyLedgerExtended() => OpenBrokerageReportsWindow(BrokerageReportMode.PartyLedgerExtended);
        [RelayCommand] private void RptBrkMonthlyCommission() => OpenBrokerageReportsWindow(BrokerageReportMode.MonthlyCommissionSoodh);
        [RelayCommand] private void RptBrkPurchaseSummary() => OpenBrokerageReportsWindow(BrokerageReportMode.PurchaseSummary);
        [RelayCommand] private void RptBrkPurchaseSummaryPartyWise() => OpenBrokerageReportsWindow(BrokerageReportMode.PurchaseSummaryPartyWise);
        [RelayCommand] private void RptBrkPartyWiseWHT() => OpenBrokerageReportsWindow(BrokerageReportMode.PartyWiseWHT);
        [RelayCommand] private void RptBrkVehicleWiseSummary() => OpenBrokerageReportsWindow(BrokerageReportMode.VehicleWiseSummary);
        [RelayCommand] private void RptBrkGatePassRegister() => OpenBrokerageReportsWindow(BrokerageReportMode.GatePassRegister);
        [RelayCommand] private void RptBrkProductLedger() => OpenBrokerageReportsWindow(BrokerageReportMode.ProductLedger);

        private void OpenBrokerageReportsWindow(BrokerageReportMode mode)
        {
            var window = new Views.BrokerageReportsWindow();
            var vm = App.ServiceProvider?.GetRequiredService<BrokerageReportsViewModel>();
            if (vm != null)
            {
                _ = vm.InitializeAsync(mode);
                window.DataContext = vm;
            }
            SafeSetOwner(window);
            window.ShowDialog();
        }

        [RelayCommand] private void RptGroupDailyWorking() => OpenReportingWindow();

        // Daily Working Reports Sub-Menu Commands
        [RelayCommand] private void RptDlyBusinessActivity() => OpenReportingWindow();

        [RelayCommand] private void RptGroupLists() => OpenReportingWindow();
        [RelayCommand] private void RptListProduct() => OpenReportingWindow();
        [RelayCommand] private void RptListBarcodePrinting() => OpenReportingWindow();

        [RelayCommand] private void GeneralPurchase() => OpenGeneralPurchaseInvoiceWindow(false);
        [RelayCommand] private void GeneralSale() => OpenGeneralSaleInvoiceWindow(false);
        [RelayCommand] private void PurchaseReturn() => OpenGeneralPurchaseInvoiceWindow(true);
        [RelayCommand] private void SaleReturn() => OpenGeneralSaleInvoiceWindow(true);

        [RelayCommand] private void RptListParties() => OpenReportingWindow();
        [RelayCommand] private void RptListSectors() => OpenReportingWindow();
        [RelayCommand] private void RptListTowns() => OpenReportingWindow();

        [RelayCommand] private void RptGroupPestro() => OpenReportingWindow();

        // Pestro Reports Sub-Menu Commands
        [RelayCommand] private void RptPestroPurchaseRegister() => OpenReportingWindow();
        [RelayCommand] private void RptPestroPurchaseSummary() => OpenReportingWindow();
        [RelayCommand] private void RptPestroPartyPurchaseLedger() => OpenReportingWindow();
        [RelayCommand] private void RptPestroProductPurchaseLedger() => OpenReportingWindow();
        [RelayCommand] private void RptPestroProductWisePurchases() => OpenReportingWindow();

        [RelayCommand] private void RptPestroPurchaseReturnLedger() => OpenReportingWindow();
        [RelayCommand] private void RptPestroPurchaseReturnSummary() => OpenReportingWindow();

        [RelayCommand] private void RptPestroSaleRegister() => OpenReportingWindow();
        [RelayCommand] private void RptPestroSaleSummary() => OpenReportingWindow();
        [RelayCommand] private void RptPestroProductSaleLedger() => OpenReportingWindow();
        [RelayCommand] private void RptPestroProductWiseSales() => OpenReportingWindow();
        [RelayCommand] private void RptPestroSalesStockStatement() => OpenReportingWindow();
        [RelayCommand] private void RptPestroCustomerTownSale() => OpenReportingWindow();
        [RelayCommand] private void RptPestroCompanySectorSale() => OpenReportingWindow();

        [RelayCommand] private void RptPestroSaleReturnLedger() => OpenReportingWindow();
        [RelayCommand] private void RptPestroSaleReturnSummary() => OpenReportingWindow();
        [RelayCommand] private void RptPestroProductSaleReturnLedger() => OpenReportingWindow();
        [RelayCommand] private void RptPestroPartySaleReturnLedger() => OpenReportingWindow();

        [RelayCommand] private void RptPestroSalesmanSummary() => OpenReportingWindow();
        [RelayCommand] private void RptPestroSalesmanCommission() => OpenReportingWindow();
        [RelayCommand] private void RptPestroSalesmanMonthlyPerformance() => OpenReportingWindow();

        [RelayCommand] private void RptGroupGenPurchase() => OpenReportingWindow();

        // General Purchase Reports Sub-Menu Commands
        [RelayCommand] private void RptGenPurRegister() => OpenReportingWindow();
        [RelayCommand] private void RptGenPurSummary() => OpenReportingWindow();
        [RelayCommand] private void RptGenPurProductLedger() => OpenReportingWindow();
        [RelayCommand] private void RptGenPurProductWise() => OpenReportingWindow();
        [RelayCommand] private void RptGroupGenPurchaseReturn() => OpenReportingWindow();

        // General Purchase Return Reports Sub-Menu Commands
        [RelayCommand] private void RptGenPurReturnRegister() => OpenReportingWindow();
        [RelayCommand] private void RptGenPurReturnSummary() => OpenReportingWindow();
        [RelayCommand] private void RptGenPurReturnProductLedger() => OpenReportingWindow();
        [RelayCommand] private void RptGroupGenSales() => OpenReportingWindow();

        // General Sales Reports Sub-Menu Commands
        [RelayCommand] private void RptGenSalesRegister() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesSummary() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesPartyLedgerExtended() => OpenReportingWindow();

        [RelayCommand] private void RptGenSalesStockStatement() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesProductLedger() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesProductWise() => OpenReportingWindow();

        [RelayCommand] private void RptGenSalesCompanySectorWise() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesCompanyWise() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesCustomerTownSectorWise() => OpenReportingWindow();

        [RelayCommand] private void RptGroupGenSalesReturn() => OpenReportingWindow();

        // General Sales Return Reports Sub-Menu Commands
        [RelayCommand] private void RptGenSalesReturnRegister() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesReturnSummary() => OpenReportingWindow();
        [RelayCommand] private void RptGenSalesReturnProductLedger() => OpenReportingWindow();

        [RelayCommand] private void RptGroupProfits() => OpenReportingWindow();

        // Profits Reports Sub-Menu Commands
        [RelayCommand] private void RptProfitsCompanyWise() => OpenReportingWindow();
        [RelayCommand] private void RptProfitsCustomerWise() => OpenReportingWindow();
        [RelayCommand] private void RptProfitsDaily() => OpenReportingWindow();

        [RelayCommand] private void RptProfitsInvoiceDetailed() => OpenReportingWindow();
        [RelayCommand] private void RptProfitsInvoiceWise() => OpenReportingWindow();
        [RelayCommand] private void RptProfitsProductWise() => OpenReportingWindow();

        [RelayCommand] private void RptGroupStock() => OpenReportingWindow();

        // Stock Reports Sub-Menu Commands
        [RelayCommand] private void RptStockOpening() => OpenReportingWindow();
        [RelayCommand] private void RptStockCurrent() => OpenReportingWindow();
        [RelayCommand] private void RptStockProductActivity() => OpenReportingWindow();

        [RelayCommand] private void RptStockPestroOpening() => OpenReportingWindow();
        [RelayCommand] private void RptStockPestroCurrent() => OpenReportingWindow();
        [RelayCommand] private void RptStockPestroExpired() => OpenReportingWindow();
        [RelayCommand] private void RptStockPestroCountSheet() => OpenReportingWindow();
    }
}
