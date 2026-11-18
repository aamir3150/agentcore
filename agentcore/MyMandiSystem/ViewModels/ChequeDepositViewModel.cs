using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Core.PrintModels;
using MyMandiSystem.Printing.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MyMandiSystem.ViewModels;

public partial class ChequeDepositViewModel : ObservableObject
{
    private readonly IBankingService _bankingService;
    private readonly ISystemService _systemService;
    private readonly ICurrentUserService _currentUserService;
    private readonly MyMandiSystem.Core.Printing.IPrintService _printService;

    private readonly DispatcherTimer _clockTimer;
    private bool _isLoadingVoucher;
    private DateTime? _originalVoucherDate;
    private ChequeDepositFormConfigDto _config = new();
    private byte[]? _thumbBytes;

    public event Action<string>? RequestFocusControl;
    public Action? RequestClose { get; set; }

    // Header & Branch / Crop Season
    [ObservableProperty] private ObservableCollection<BranchLookupDto> _branches = new();
    [ObservableProperty] private BranchLookupDto? _selectedBranch;
    [ObservableProperty] private bool _isBranchVisible;
    [ObservableProperty] private bool _isIdBranchVisible;
    [ObservableProperty] private string _idBranch = "1";

    [ObservableProperty] private ObservableCollection<CropSeason> _seasons = new();
    [ObservableProperty] private CropSeason? _selectedSeason;
    [ObservableProperty] private bool _isCropSeasonVisible = true;

    [ObservableProperty] private string _voucherNo = "1";
    [ObservableProperty] private DateTime _voucherDate = DateTime.Today;
    [ObservableProperty] private string _voucherTimeText = DateTime.Now.ToString("hh:mm:ss tt");
    private DateTime _voucherTimeValue = DateTime.Now;
    [ObservableProperty] private bool _isVoucherDateEnabled = true;
    [ObservableProperty] private bool _isVoucherNoReadOnly;
    [ObservableProperty] private bool _isExistingVoucher;
    [ObservableProperty] private string _searchVoucherNo = string.Empty;

    // Cheque Specific Fields (AgenSoft frmChequeDeposit Controls)
    [ObservableProperty] private string _slipNo = string.Empty;
    [ObservableProperty] private DateTime _chequeDate = DateTime.Today;
    [ObservableProperty] private string _chequeNo = string.Empty;
    [ObservableProperty] private bool _isReconciled;
    [ObservableProperty] private bool _isDepositConfirmed = true;
    [ObservableProperty] private bool _isBounced;
    [ObservableProperty] private string _reconcileStatusText = "Pending Clearance (Unposted)";

    // Bank Account Selection (AgenSoft txtBankAccountno / txtBankAccountName / TxtAccountBal)
    [ObservableProperty] private string _bankAccountNo = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;
    [ObservableProperty] private string _bankAccountBalanceText = "0 Dr";

    // Party / Received By Account (AgenSoft txtReceivedBy / txtAccountName / TxtCashInHand)
    [ObservableProperty] private string _partyAccountNo = string.Empty;
    [ObservableProperty] private string _partyAccountName = string.Empty;
    [ObservableProperty] private string _partyAccountBalanceText = "0 Dr";

    // Amount, Words, Narration, Deposited By
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string _amountText = "0";
    [ObservableProperty] private string _amountInWords = string.Empty;
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private string _depositedBy = string.Empty;
    [ObservableProperty] private string _narrationUrdu = string.Empty;
    [ObservableProperty] private bool _isUrduNarrationVisible = true;

    // SMS & Thumb Biometric (AgenSoft ChksmsEnabled / ChkShowThumb / cmdThumbReg / Picture1)
    [ObservableProperty] private bool _sendSms;
    [ObservableProperty] private bool _isSmsVisible = true;
    [ObservableProperty] private bool _isThumbSectionVisible = true;
    [ObservableProperty] private bool _showThumb;
    [ObservableProperty] private ImageSource? _thumbImageSource;
    [ObservableProperty] private string _thumbStatusText = "No Thumb Loaded";

    // Right-Side Multi-Deposit Grid (AgenSoft UseBankDepositMulti PopulateGrid)
    [ObservableProperty] private bool _isMultiGridVisible = true;
    [ObservableProperty] private ObservableCollection<ChequeDepositDto> _todayDeposits = new();
    [ObservableProperty] private decimal _todayTotalAmount;

    // F1 Account Lookup Modal (AgenSoft SchAccounts Parity)
    [ObservableProperty] private bool _isAccountLookupOpen;
    [ObservableProperty] private bool _isLookupForBank;
    [ObservableProperty] private string _lookupTitle = "Search Accounts (F1)";
    [ObservableProperty] private string _lookupSearchQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<ChartAccountLookupDto> _filteredAccounts = new();
    [ObservableProperty] private ChartAccountLookupDto? _selectedLookupAccount;

    // Search Bank Cheque Deposits Modal (AgenSoft SchBankDeposits Parity)
    [ObservableProperty] private bool _isSearchListOpen;
    [ObservableProperty] private string _searchFilterVoucherNo = string.Empty;
    [ObservableProperty] private DateTime _searchFromDate = DateTime.Today.AddDays(-30);
    [ObservableProperty] private DateTime _searchToDate = DateTime.Today;
    [ObservableProperty] private string _searchFilterDepositedBy = string.Empty;
    [ObservableProperty] private string _searchListQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<ChequeDepositDto> _searchListResults = new();
    [ObservableProperty] private ChequeDepositDto? _selectedSearchItem;

    public ChequeDepositViewModel(
        IBankingService bankingService,
        ISystemService systemService,
        ICurrentUserService currentUserService,
        MyMandiSystem.Core.Printing.IPrintService printService)
    {
        _bankingService = bankingService;
        _systemService = systemService;
        _currentUserService = currentUserService;
        _printService = printService;

        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _clockTimer.Tick += (_, _) =>
        {
            if (!IsExistingVoucher)
            {
                _voucherTimeValue = DateTime.Now;
                VoucherTimeText = _voucherTimeValue.ToString("hh:mm:ss tt");
            }
        };

        _ = LoadInitialDataAsync();
    }

    public async Task LoadInitialDataAsync()
    {
        try
        {
            int userId = _currentUserService.GetCurrentUserId();
            _config = await _bankingService.GetChequeDepositFormConfigAsync(userId);

            Branches = new ObservableCollection<BranchLookupDto>(_config.Branches);
            SelectedBranch = Branches.FirstOrDefault();
            IsBranchVisible = _config.EnableBranchWise;
            IsIdBranchVisible = _config.EnableBranchWise;
            IsCropSeasonVisible = _config.EnableCropSeason;
            IsUrduNarrationVisible = _config.ShowUrduNarration;
            IsThumbSectionVisible = _config.EnableThumbOnForms;
            IsSmsVisible = _config.EnableSms;
            IsMultiGridVisible = _config.UseBankDepositMulti;
            IsVoucherDateEnabled = _config.ChangeDateBankVoucher;
            IsVoucherNoReadOnly = !_config.ChangeVidBankVoucher;

            var seasonsList = await _systemService.GetActiveSeasonsAsync();
            Seasons = new ObservableCollection<CropSeason>(seasonsList);
            SelectedSeason = Seasons.FirstOrDefault(s => s.IsActive) ?? Seasons.FirstOrDefault();

            await NewVoucherAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading Bank Cheque Deposit:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void StopClock()
    {
        _clockTimer.Stop();
    }

    partial void OnVoucherDateChanged(DateTime value)
    {
        if (_isLoadingVoucher) return;
        _ = HandleVoucherDateChangedAsync(value);
    }

    private async Task HandleVoucherDateChangedAsync(DateTime date)
    {
        try
        {
            if (!IsExistingVoucher && (_config.EnableAutoVoucherId || _config.AutoVoucherIdMode.Equals("Daily", StringComparison.OrdinalIgnoreCase) || _config.AutoVoucherIdMode.Equals("Monthly", StringComparison.OrdinalIgnoreCase)))
            {
                var nums = await _bankingService.GetNextChequeDepositNumbersAsync(date, SelectedBranch?.BranchId ?? 1);
                VoucherNo = nums.VoucherNo.ToString();
                IdBranch = nums.IdBranch.ToString();
            }

            if (!string.IsNullOrWhiteSpace(BankAccountNo))
                await RefreshBankBalanceAsync();
            if (!string.IsNullOrWhiteSpace(PartyAccountNo))
                await RefreshPartyBalanceAsync();

            await LoadTodayDepositsAsync();
        }
        catch { }
    }

    partial void OnSelectedBranchChanged(BranchLookupDto? value)
    {
        if (_isLoadingVoucher || value == null) return;
        if (!IsExistingVoucher && _config.EnableBranchWise)
        {
            _ = RefreshNewIdsAsync();
        }
        _ = LoadTodayDepositsAsync();
    }

    private async Task RefreshNewIdsAsync()
    {
        var nums = await _bankingService.GetNextChequeDepositNumbersAsync(VoucherDate, SelectedBranch?.BranchId ?? 1);
        VoucherNo = nums.VoucherNo.ToString();
        IdBranch = nums.IdBranch.ToString();
    }

    partial void OnAmountTextChanged(string value)
    {
        if (_isLoadingVoucher) return;
        var clean = (value ?? string.Empty).Replace(",", "").Trim();
        if (decimal.TryParse(clean, out var parsed) && parsed >= 0)
        {
            Amount = parsed;
            AmountInWords = parsed > 0 ? AmountInWordsConverter.FormatAmountInWords(parsed) : string.Empty;
        }
        else if (string.IsNullOrEmpty(clean))
        {
            Amount = 0;
            AmountInWords = string.Empty;
        }
    }

    partial void OnAmountChanged(decimal value)
    {
        AmountInWords = value > 0 ? AmountInWordsConverter.FormatAmountInWords(value) : string.Empty;
    }

    partial void OnIsReconciledChanged(bool value)
    {
        UpdateStatusText();
    }

    partial void OnIsBouncedChanged(bool value)
    {
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (IsBounced) ReconcileStatusText = "Bounced / Returned";
        else if (IsReconciled) ReconcileStatusText = "Reconciled & Posted to Ledger";
        else ReconcileStatusText = "Pending Clearance (Unposted)";
    }

    partial void OnShowThumbChanged(bool value)
    {
        if (!value)
        {
            ThumbImageSource = null;
            ThumbStatusText = _thumbBytes is { Length: > 0 } ? "Thumb Stored (Hidden)" : "No Thumb Loaded";
            return;
        }

        if (_thumbBytes is { Length: > 0 })
        {
            ThumbImageSource = LoadImageFromBytes(_thumbBytes);
            ThumbStatusText = "Thumb Loaded";
        }
        else
        {
            TryLoadLocalThumbFile(silent: true);
        }
    }

    [RelayCommand]
    public async Task NewVoucherAsync()
    {
        _isLoadingVoucher = true;
        try
        {
            IsExistingVoucher = false;
            _originalVoucherDate = null;
            VoucherDate = DateTime.Today;
            _voucherTimeValue = DateTime.Now;
            VoucherTimeText = _voucherTimeValue.ToString("hh:mm:ss tt");

            var nums = await _bankingService.GetNextChequeDepositNumbersAsync(VoucherDate, SelectedBranch?.BranchId ?? 1);
            VoucherNo = nums.VoucherNo.ToString();
            IdBranch = nums.IdBranch.ToString();

            SlipNo = string.Empty;
            ChequeDate = DateTime.Today;
            ChequeNo = string.Empty;
            IsReconciled = false;
            IsDepositConfirmed = true;
            IsBounced = false;
            UpdateStatusText();

            BankAccountNo = string.Empty;
            BankAccountName = string.Empty;
            BankAccountBalanceText = "0 Dr";

            PartyAccountNo = string.Empty;
            PartyAccountName = string.Empty;
            PartyAccountBalanceText = "0 Dr";

            Amount = 0;
            AmountText = "0";
            AmountInWords = string.Empty;
            Narration = string.Empty;
            DepositedBy = string.Empty;
            NarrationUrdu = string.Empty;
            SendSms = false;

            _thumbBytes = null;
            ShowThumb = false;
            ThumbImageSource = null;
            ThumbStatusText = "No Thumb Loaded";

            _clockTimer.Start();
        }
        finally
        {
            _isLoadingVoucher = false;
        }

        await LoadTodayDepositsAsync();
        RequestFocusControl?.Invoke("txtBankAccountno");
    }

    public async Task LoadTodayDepositsAsync()
    {
        try
        {
            var items = await _bankingService.SearchChequeDepositsAsync(
                fromDate: VoucherDate.Date,
                toDate: VoucherDate.Date,
                isCheque: true,
                branchId: IsBranchVisible ? SelectedBranch?.BranchId : null);

            var list = items.ToList();
            TodayDeposits = new ObservableCollection<ChequeDepositDto>(list);
            TodayTotalAmount = list.Sum(x => x.Amount);
        }
        catch
        {
            TodayDeposits = new ObservableCollection<ChequeDepositDto>();
            TodayTotalAmount = 0;
        }
    }

    // ─── ACCOUNT VALIDATION & F1 LOOKUP (SchAccounts Parity) ──────────────────

    public async Task ValidateBankAccountAsync(bool openLookupIfNotFound = true)
    {
        var code = BankAccountNo?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(code))
        {
            BankAccountName = string.Empty;
            BankAccountBalanceText = "0 Dr";
            return;
        }

        var match = await _bankingService.ValidateChartAccountAsync(code);
        if (match == null)
        {
            BankAccountNo = string.Empty;
            BankAccountName = string.Empty;
            BankAccountBalanceText = "0 Dr";
            if (openLookupIfNotFound)
                await OpenBankLookupAsync();
            return;
        }

        BankAccountNo = match.AccountNo;
        BankAccountName = match.AccountName;
        await RefreshBankBalanceAsync();
    }

    public async Task ValidatePartyAccountAsync(bool openLookupIfNotFound = true)
    {
        var code = PartyAccountNo?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(code))
        {
            PartyAccountName = string.Empty;
            PartyAccountBalanceText = "0 Dr";
            return;
        }

        var match = await _bankingService.ValidateChartAccountAsync(code);
        if (match == null)
        {
            PartyAccountNo = string.Empty;
            PartyAccountName = string.Empty;
            PartyAccountBalanceText = "0 Dr";
            if (openLookupIfNotFound)
                await OpenPartyLookupAsync();
            return;
        }

        PartyAccountNo = match.AccountNo;
        PartyAccountName = match.AccountName;
        await RefreshPartyBalanceAsync();
    }

    private async Task RefreshBankBalanceAsync()
    {
        if (string.IsNullOrWhiteSpace(BankAccountNo))
        {
            BankAccountBalanceText = "0 Dr";
            return;
        }
        var (_, formatted) = await _bankingService.GetAccountBalanceTextAsync(BankAccountNo, VoucherDate);
        BankAccountBalanceText = formatted;
    }

    private async Task RefreshPartyBalanceAsync()
    {
        if (string.IsNullOrWhiteSpace(PartyAccountNo))
        {
            PartyAccountBalanceText = "0 Dr";
            return;
        }
        var (_, formatted) = await _bankingService.GetAccountBalanceTextAsync(PartyAccountNo, VoucherDate);
        PartyAccountBalanceText = formatted;
    }

    [RelayCommand]
    public async Task OpenBankLookupAsync()
    {
        IsLookupForBank = true;
        LookupTitle = "Select Bank Account (F1)";
        LookupSearchQuery = string.Empty;
        var rows = await _bankingService.SearchChartAccountsAsync(null, _config.AccBankAccountPrefix, bankOnly: true);
        FilteredAccounts = new ObservableCollection<ChartAccountLookupDto>(rows);
        SelectedLookupAccount = FilteredAccounts.FirstOrDefault();
        IsAccountLookupOpen = true;
    }

    [RelayCommand]
    public async Task OpenPartyLookupAsync()
    {
        IsLookupForBank = false;
        LookupTitle = "Select Received By Account (F1)";
        LookupSearchQuery = string.Empty;
        var rows = await _bankingService.SearchChartAccountsAsync(null, null, bankOnly: false);
        FilteredAccounts = new ObservableCollection<ChartAccountLookupDto>(rows);
        SelectedLookupAccount = FilteredAccounts.FirstOrDefault();
        IsAccountLookupOpen = true;
    }

    partial void OnLookupSearchQueryChanged(string value)
    {
        _ = FilterLookupAccountsAsync(value);
    }

    private async Task FilterLookupAccountsAsync(string filter)
    {
        var rows = await _bankingService.SearchChartAccountsAsync(
            filter,
            IsLookupForBank ? _config.AccBankAccountPrefix : null,
            IsLookupForBank);
        FilteredAccounts = new ObservableCollection<ChartAccountLookupDto>(rows);
        SelectedLookupAccount = FilteredAccounts.FirstOrDefault();
    }

    [RelayCommand]
    public async Task SelectLookupAccountAsync(ChartAccountLookupDto? account)
    {
        if (account == null) return;
        IsAccountLookupOpen = false;

        if (IsLookupForBank)
        {
            BankAccountNo = account.AccountNo;
            BankAccountName = account.AccountName;
            await RefreshBankBalanceAsync();
            RequestFocusControl?.Invoke("txtReceivedBy");
        }
        else
        {
            PartyAccountNo = account.AccountNo;
            PartyAccountName = account.AccountName;
            await RefreshPartyBalanceAsync();
            RequestFocusControl?.Invoke("txtAmount");
        }
    }

    [RelayCommand]
    public void CloseLookup()
    {
        IsAccountLookupOpen = false;
    }

    // ─── DEFINITIONS MENU (MnuDef: Customers, Vendors, Others) ─────────────────

    [RelayCommand]
    public void OpenCustomersDefinition()
    {
        try
        {
            var vm = App.ServiceProvider?.GetRequiredService<PartyViewModel>();
            if (vm != null) vm.SelectedTabIndex = 0;
            var window = new Views.PartyWindow(0) { DataContext = vm };
            window.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open Customers:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    public void OpenVendorsDefinition()
    {
        try
        {
            var vm = App.ServiceProvider?.GetRequiredService<PartyViewModel>();
            if (vm != null) vm.SelectedTabIndex = 1;
            var window = new Views.PartyWindow(1) { DataContext = vm };
            window.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open Vendors:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    public void OpenOtherAccountsDefinition()
    {
        try
        {
            var window = new Views.ChartOfAccountsWindow
            {
                DataContext = App.ServiceProvider?.GetRequiredService<ChartOfAccountsViewModel>()
            };
            window.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open Chart of Accounts:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // ─── THUMB / BIOMETRIC (ShowThumb & cmdThumbReg Parity) ───────────────────

    [RelayCommand]
    public void ThumbRegister()
    {
        if (TryLoadLocalThumbFile(silent: false))
            return;

        var dlg = new OpenFileDialog
        {
            Title = "Select Depositor Thumb Impression Image",
            Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
        };
        if (dlg.ShowDialog() == true)
        {
            try
            {
                _thumbBytes = File.ReadAllBytes(dlg.FileName);
                ShowThumb = true;
                ThumbImageSource = LoadImageFromBytes(_thumbBytes);
                ThumbStatusText = "Thumb Registered";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to read thumb image:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private bool TryLoadLocalThumbFile(bool silent)
    {
        string[] candidates = { @"C:\Thumb.jpg", @"C:\thumb.bmp", @"C:\Thumb.bmp" };
        foreach (var path in candidates)
        {
            if (File.Exists(path))
            {
                try
                {
                    _thumbBytes = File.ReadAllBytes(path);
                    ShowThumb = true;
                    ThumbImageSource = LoadImageFromBytes(_thumbBytes);
                    ThumbStatusText = $"Loaded {Path.GetFileName(path)}";
                    return true;
                }
                catch { }
            }
        }
        if (!silent)
        {
            ThumbStatusText = "Select an image or connect scanner";
        }
        return false;
    }

    private static ImageSource? LoadImageFromBytes(byte[]? bytes)
    {
        if (bytes == null || bytes.Length == 0) return null;
        try
        {
            using var ms = new MemoryStream(bytes);
            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();
            img.Freeze();
            return img;
        }
        catch
        {
            return null;
        }
    }

    // ─── SAVE / VALIDATE (AgenSoft Exact Alerts & Permissions) ────────────────

    [RelayCommand]
    public async Task SaveAsync()
    {
        // Re-validate account codes if typed without LostFocus
        if (!string.IsNullOrWhiteSpace(BankAccountNo) && string.IsNullOrWhiteSpace(BankAccountName))
            await ValidateBankAccountAsync(openLookupIfNotFound: false);
        if (!string.IsNullOrWhiteSpace(PartyAccountNo) && string.IsNullOrWhiteSpace(PartyAccountName))
            await ValidatePartyAccountAsync(openLookupIfNotFound: false);

        if (string.IsNullOrWhiteSpace(VoucherNo) || !long.TryParse(VoucherNo.Trim(), out var vNo) || vNo <= 0)
        {
            MessageBox.Show("Please specify a Voucher no", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            RequestFocusControl?.Invoke("txtVoucherno");
            return;
        }

        if (string.IsNullOrWhiteSpace(BankAccountName) || string.IsNullOrWhiteSpace(BankAccountNo))
        {
            MessageBox.Show("Please specify a valid Bank A/C no.", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            RequestFocusControl?.Invoke("txtBankAccountno");
            return;
        }

        if (string.IsNullOrWhiteSpace(PartyAccountName) || string.IsNullOrWhiteSpace(PartyAccountNo))
        {
            MessageBox.Show("Please specify a valid Received By A/C no.", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            RequestFocusControl?.Invoke("txtReceivedBy");
            return;
        }

        if (Amount <= 0)
        {
            MessageBox.Show("Please Enter Amount.", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            RequestFocusControl?.Invoke("txtAmount");
            return;
        }

        if (Amount >= 1_000_000_000m)
        {
            MessageBox.Show("Amount must be less than 1 billion ", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            RequestFocusControl?.Invoke("txtAmount");
            return;
        }

        int currentUserId = _currentUserService.GetCurrentUserId();

        if (IsExistingVoucher)
        {
            var canModify = await _bankingService.CheckUserTaskPermissionAsync(currentUserId, "ChequeDepositsModification");
            if (!canModify)
            {
                MessageBox.Show("You are not allowed to change / edit or delete this voucher", "Security Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }
        else
        {
            if (await _bankingService.ChequeDepositExistsAsync(vNo, VoucherDate))
            {
                await RefreshNewIdsAsync();
                MessageBox.Show("This ID already exists. A new ID has been generated. Please save again", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        try
        {
            long.TryParse(IdBranch, out var idBranchVal);
            if (idBranchVal <= 0) idBranchVal = vNo;

            var dto = new ChequeDepositDto
            {
                VoucherNo = vNo,
                VoucherDate = VoucherDate.Date,
                OriginalVoucherDate = _originalVoucherDate ?? VoucherDate.Date,
                IsExisting = IsExistingVoucher,
                IsCheque = true,
                ChequeNo = ChequeNo?.Trim() ?? string.Empty,
                ChequeDate = ChequeDate.Date,
                SlipNo = SlipNo?.Trim() ?? string.Empty,
                DepositedDate = ChequeDate.Date,
                IsDepositConfirmed = IsDepositConfirmed,
                IsReconciled = IsReconciled,
                ReconcileDate = IsReconciled ? ChequeDate.Date : null,
                BankAccountNo = BankAccountNo.Trim(),
                BankAccountName = BankAccountName.Trim(),
                PartyAccountNo = PartyAccountNo.Trim(),
                PartyAccountName = PartyAccountName.Trim(),
                Amount = Amount,
                DepositedBy = DepositedBy?.Trim() ?? string.Empty,
                Narration = Narration?.Trim() ?? string.Empty,
                NarrationUrdu = NarrationUrdu?.Trim() ?? string.Empty,
                SendSms = SendSms,
                CropSeasonId = SelectedSeason?.Id,
                CropSeason = SelectedSeason?.Name ?? string.Empty,
                BranchId = SelectedBranch?.BranchId ?? 1,
                BranchName = SelectedBranch?.CompanyName ?? "Main Branch",
                IdBranch = idBranchVal,
                UserNo = (short)Math.Max(1, currentUserId),
                VoucherTime = IsExistingVoucher ? _voucherTimeValue : DateTime.Now,
                IsBounced = IsBounced,
                FPData = ShowThumb ? _thumbBytes : null
            };

            await _bankingService.SaveChequeDepositAsync(dto);

            // Refresh multi-deposit grid immediately before print prompt so user sees updated list
            await LoadTodayDepositsAsync();

            var result = MessageBox.Show(
                "Are you sure do you want to print?",
                "Alert",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await PrintVoucherInternalAsync(vNo, VoucherDate.Date);
            }

            await NewVoucherAsync();
        }
        catch (InvalidOperationException dupEx) when (dupEx.Message == "DUPLICATE_VOUCHER_ID")
        {
            await RefreshNewIdsAsync();
            MessageBox.Show("This ID already exists. A new ID has been generated. Please save again", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving voucher:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ─── LOAD / SEARCH / DELETE ───────────────────────────────────────────────

    [RelayCommand]
    public async Task SearchVoucherAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchVoucherNo)) return;
        if (long.TryParse(SearchVoucherNo.Trim(), out var vNo))
        {
            await LoadVoucherAsync(vNo, null);
        }
        else
        {
            MessageBox.Show("Please specify a Voucher no", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }

    [RelayCommand]
    public async Task PreviousVoucherAsync()
    {
        if (long.TryParse(VoucherNo, out var current) && current > 1)
            await LoadVoucherAsync(current - 1, null);
    }

    [RelayCommand]
    public async Task NextVoucherAsync()
    {
        if (long.TryParse(VoucherNo, out var current))
            await LoadVoucherAsync(current + 1, null);
    }

    public async Task LoadVoucherAsync(long voucherNo, DateTime? voucherDate = null)
    {
        try
        {
            var dto = await _bankingService.LoadChequeDepositByVoucherNoAsync(voucherNo, voucherDate, requireIsCheque: true);
            if (dto == null)
            {
                MessageBox.Show($"Bank Cheque Deposit Voucher #{voucherNo} not found!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _isLoadingVoucher = true;
            _clockTimer.Stop();
            try
            {
                IsExistingVoucher = true;
                _originalVoucherDate = dto.VoucherDate.Date;

                VoucherNo = dto.VoucherNo.ToString();
                IdBranch = (dto.IdBranch > 0 ? dto.IdBranch : dto.VoucherNo).ToString();
                VoucherDate = dto.VoucherDate;
                _voucherTimeValue = dto.VoucherTime ?? dto.VoucherDate;
                VoucherTimeText = _voucherTimeValue.ToString("hh:mm:ss tt");

                if (dto.BranchId > 0)
                    SelectedBranch = Branches.FirstOrDefault(b => b.BranchId == dto.BranchId) ?? SelectedBranch;

                if (!string.IsNullOrEmpty(dto.CropSeason))
                {
                    SelectedSeason = Seasons.FirstOrDefault(s =>
                        s.Name.StartsWith(dto.CropSeason, StringComparison.OrdinalIgnoreCase)) ?? SelectedSeason;
                }

                SlipNo = dto.SlipNo;
                ChequeNo = dto.ChequeNo;
                ChequeDate = dto.ChequeDate;
                IsDepositConfirmed = dto.IsDepositConfirmed;
                IsReconciled = dto.IsReconciled;
                IsBounced = dto.IsBounced;
                UpdateStatusText();

                BankAccountNo = dto.BankAccountNo;
                BankAccountName = dto.BankAccountName;
                PartyAccountNo = dto.PartyAccountNo;
                PartyAccountName = dto.PartyAccountName;

                Amount = dto.Amount;
                AmountText = dto.Amount.ToString("0.##");
                AmountInWords = dto.Amount > 0 ? AmountInWordsConverter.FormatAmountInWords(dto.Amount) : string.Empty;
                Narration = dto.Narration;
                NarrationUrdu = dto.NarrationUrdu;
                DepositedBy = dto.DepositedBy;

                _thumbBytes = dto.FPData;
                if (_thumbBytes is { Length: > 0 })
                {
                    ShowThumb = true;
                    ThumbImageSource = LoadImageFromBytes(_thumbBytes);
                    ThumbStatusText = "Thumb Loaded";
                }
                else
                {
                    ShowThumb = false;
                    ThumbImageSource = null;
                    ThumbStatusText = "No Thumb Loaded";
                }
            }
            finally
            {
                _isLoadingVoucher = false;
            }

            await RefreshBankBalanceAsync();
            await RefreshPartyBalanceAsync();
            await LoadTodayDepositsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    public async Task DeleteVoucherAsync()
    {
        if (!IsExistingVoucher || !long.TryParse(VoucherNo, out var vNo))
        {
            MessageBox.Show("No saved voucher selected to remove.", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int currentUserId = _currentUserService.GetCurrentUserId();
        var canModify = await _bankingService.CheckUserTaskPermissionAsync(currentUserId, "ChequeDepositsModification");
        if (!canModify)
        {
            MessageBox.Show("You are not allowed to change / edit or delete this voucher", "Security Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;
        }

        var result = MessageBox.Show(
            "Do you really want to remove this record?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _bankingService.DeleteChequeDepositAsync(vNo, _originalVoucherDate ?? VoucherDate.Date, (short)Math.Max(1, currentUserId));
                await NewVoucherAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing record:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // ─── SEARCH MODAL (SchBankDeposits Parity) ────────────────────────────────

    [RelayCommand]
    public async Task OpenSearchModalAsync()
    {
        IsSearchListOpen = true;
        SearchFilterVoucherNo = string.Empty;
        SearchFilterDepositedBy = string.Empty;
        SearchListQuery = string.Empty;
        SearchFromDate = DateTime.Today.AddDays(-30);
        SearchToDate = DateTime.Today;
        await RefreshSearchModalAsync();
    }

    [RelayCommand]
    public async Task RefreshSearchModalAsync()
    {
        string? combinedQuery = !string.IsNullOrWhiteSpace(SearchFilterVoucherNo)
            ? SearchFilterVoucherNo.Trim()
            : (!string.IsNullOrWhiteSpace(SearchListQuery) ? SearchListQuery.Trim() : null);

        var list = await _bankingService.SearchChequeDepositsAsync(
            query: combinedQuery,
            fromDate: SearchFromDate.Date,
            toDate: SearchToDate.Date,
            isCheque: true,
            depositedBy: SearchFilterDepositedBy);

        SearchListResults = new ObservableCollection<ChequeDepositDto>(list);
        SelectedSearchItem = SearchListResults.FirstOrDefault();
    }

    [RelayCommand]
    public async Task SelectSearchItemAsync(ChequeDepositDto? item)
    {
        var target = item ?? SelectedSearchItem;
        if (target == null) return;
        IsSearchListOpen = false;
        await LoadVoucherAsync(target.VoucherNo, target.VoucherDate);
    }

    [RelayCommand]
    public void CloseSearchModal()
    {
        IsSearchListOpen = false;
    }

    // ─── PRINT (CrpChequeDepost / CrpChequeDepositMulti English Parity) ───────

    [RelayCommand]
    public async Task PrintAsync()
    {
        if (!long.TryParse(VoucherNo, out var vNo) || vNo <= 0)
        {
            MessageBox.Show("Please specify a Voucher no", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;
        }
        await PrintVoucherInternalAsync(vNo, (_originalVoucherDate ?? VoucherDate).Date);
    }

    private async Task PrintVoucherInternalAsync(long vNo, DateTime vDate)
    {
        try
        {
            var data = await _bankingService.GetChequeDepositPrintDataAsync(
                vNo,
                vDate,
                SelectedBranch?.BranchId ?? 1,
                includeMultiForDate: false);

            ChequeDepositDto d = data?.Deposit ?? new ChequeDepositDto
            {
                VoucherNo = vNo,
                IdBranch = long.TryParse(IdBranch, out var idb) && idb > 0 ? idb : vNo,
                VoucherDate = VoucherDate,
                VoucherTime = _voucherTimeValue,
                ChequeNo = ChequeNo,
                ChequeDate = ChequeDate,
                SlipNo = SlipNo,
                DepositedDate = IsDepositConfirmed ? ChequeDate : VoucherDate,
                IsDepositConfirmed = IsDepositConfirmed,
                ReconcileDate = IsReconciled ? ChequeDate : null,
                IsReconciled = IsReconciled,
                BankAccountNo = BankAccountNo,
                BankAccountName = BankAccountName,
                PartyAccountNo = PartyAccountNo,
                PartyAccountName = PartyAccountName,
                Amount = Amount,
                DepositedBy = DepositedBy,
                Narration = Narration,
                CropSeason = SelectedSeason?.Name ?? string.Empty,
                CreatedByUserName = _currentUserService.GetCurrentUserName(),
                FPData = ShowThumb ? _thumbBytes : null
            };

            var slip = new ChequeDepositSlip
            {
                Title = "Cheque Deposit",
                CompanyName = data?.CompanyName ?? string.Empty,
                CompanyAddress = data?.CompanyAddress ?? string.Empty,
                CompanyPhone = data?.CompanyPhone ?? string.Empty,
                CompanyEmail = data?.CompanyEmail ?? string.Empty,
                FooterText = data?.FooterInEnglish ?? string.Empty,
                VoucherNo = d.VoucherNo,
                IdBranch = d.IdBranch,
                VoucherDate = d.VoucherDate,
                VoucherTime = d.VoucherTime,
                ChequeNo = d.ChequeNo,
                ChequeDate = d.ChequeDate,
                SlipNo = d.SlipNo,
                DepositedDate = d.IsDepositConfirmed ? d.DepositedDate : null,
                ReconcileDate = d.ReconcileDate,
                IsPosted = d.IsPosted,
                BankAccountNo = d.BankAccountNo,
                BankName = d.BankAccountName,
                AccountNo = d.PartyAccountNo,
                PayToName = d.PartyAccountName,
                Amount = d.Amount,
                AmountInWords = AmountInWordsConverter.FormatAmountInWords(d.Amount),
                DepositedBy = d.DepositedBy,
                Narration = d.Narration,
                CropSeason = d.CropSeason,
                UserName = !string.IsNullOrWhiteSpace(d.CreatedByUserName)
                    ? d.CreatedByUserName
                    : _currentUserService.GetCurrentUserName(),
                FPData = d.FPData,
                IsMultiMode = false
            };

            if (data != null && (data.DirectPrintVoucher || data.ShowPrintDialogVoucher))
                _printService.Print(slip);
            else
                _printService.Preview(slip);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error preparing print:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public async Task PrintDayRegisterAsync()
    {
        try
        {
            if (!long.TryParse(VoucherNo, out var vNo)) vNo = 1;
            var data = await _bankingService.GetChequeDepositPrintDataAsync(
                vNo,
                VoucherDate.Date,
                SelectedBranch?.BranchId ?? 1,
                includeMultiForDate: true);

            var items = (data?.MultiItems ?? TodayDeposits.ToList())
                .Select(x => new ChequeDepositSlipItem(
                    x.IdBranch > 0 ? x.IdBranch : x.VoucherNo,
                    x.ChequeNo,
                    x.ChequeDate,
                    x.SlipNo,
                    x.IsDepositConfirmed ? x.DepositedDate : null,
                    x.BankAccountNo,
                    x.BankAccountName,
                    x.PartyAccountNo,
                    x.PartyAccountName,
                    x.DepositedBy,
                    x.Amount))
                .ToList();

            var slip = new ChequeDepositSlip
            {
                Title = "Cheque Deposit",
                CompanyName = data?.CompanyName ?? string.Empty,
                CompanyAddress = data?.CompanyAddress ?? string.Empty,
                CompanyPhone = data?.CompanyPhone ?? string.Empty,
                CompanyEmail = data?.CompanyEmail ?? string.Empty,
                VoucherDate = VoucherDate.Date,
                UserName = _currentUserService.GetCurrentUserName(),
                IsMultiMode = true,
                MultiItems = items
            };

            if (data != null && (data.DirectPrintVoucher || data.ShowPrintDialogVoucher))
                _printService.Print(slip);
            else
                _printService.Preview(slip);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error printing cheque deposit register:\n{ex.Message}", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public void Close()
    {
        StopClock();
        RequestClose?.Invoke();
    }
}
