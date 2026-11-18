using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class BankingViewModel : ObservableObject
{
    private readonly IBankingService _bankingService;
    private readonly IPartyService _partyService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private int _selectedTabIndex = 0;

    // Collections
    [ObservableProperty] private ObservableCollection<BankAccount> _bankAccounts = new();
    [ObservableProperty] private ObservableCollection<Party> _parties = new();
    [ObservableProperty] private ObservableCollection<CropSeason> _cropSeasons = new();
    [ObservableProperty] private CropSeason? _selectedCropSeason;

    // TAB 0: Cheque Book Generation
    [ObservableProperty] private bool _isAddBankCardVisible = false;
    [ObservableProperty] private string _newBankName = string.Empty;
    [ObservableProperty] private string _newBranchName = string.Empty;
    [ObservableProperty] private string _newAccountNumber = string.Empty;
    [ObservableProperty] private decimal _newBankOpeningBalance = 0;

    [ObservableProperty] private BankAccount? _selectedGenBankAccount;
    [ObservableProperty] private int _seriesStart = 0;
    [ObservableProperty] private int _seriesEnd = 0;
    [ObservableProperty] private int _totalLeaves = 0;
    [ObservableProperty] private ObservableCollection<ChequeBook> _existingChequeBooks = new();

    // TAB 1: Cheque Issuing
    [ObservableProperty] private BankAccount? _selectedIssueBankAccount;
    [ObservableProperty] private ObservableCollection<Cheque> _availableCheques = new();
    [ObservableProperty] private Cheque? _selectedAvailableCheque;
    [ObservableProperty] private string _selectedPartyId = string.Empty;
    [ObservableProperty] private string _partyName = string.Empty;
    [ObservableProperty] private Party? _selectedParty;
    [ObservableProperty] private decimal _issueAmount = 0;
    [ObservableProperty] private DateTime _issueDate = DateTime.Today;
    [ObservableProperty] private string _receivedBy = string.Empty;
    [ObservableProperty] private string _narration = string.Empty;

    // TAB 2: Deposits (Cash & Cheque)
    [ObservableProperty] private BankAccount? _selectedDepositBankAccount;
    [ObservableProperty] private ObservableCollection<string> _depositTypes = new() { "Cash Deposit", "Cheque Deposit" };
    [ObservableProperty] private string _selectedDepositType = "Cash Deposit";
    [ObservableProperty] private bool _isChequeDepositMode = false;
    [ObservableProperty] private string _depositPartyId = string.Empty;
    [ObservableProperty] private string _depositPartyName = string.Empty;
    [ObservableProperty] private string _depositChequeNo = string.Empty;
    [ObservableProperty] private DateTime _depositChequeDate = DateTime.Today;
    [ObservableProperty] private decimal _depositAmount = 0;
    [ObservableProperty] private string _depositSlipNo = string.Empty;
    [ObservableProperty] private DateTime _depositDate = DateTime.Today;
    [ObservableProperty] private string _depositNarration = string.Empty;

    // TAB 3: Reconciliation (Cheques & Deposits)
    [ObservableProperty] private bool _isReconcilingCheques = true;
    [ObservableProperty] private bool _isReconcilingDeposits = false;
    [ObservableProperty] private ObservableCollection<Cheque> _unclearedCheques = new();
    [ObservableProperty] private Cheque? _selectedUnclearedCheque;
    [ObservableProperty] private DateTime _clearanceDate = DateTime.Today;
    [ObservableProperty] private string _bounceReason = string.Empty;
    [ObservableProperty] private ObservableCollection<BankDeposit> _unclearedDeposits = new();
    [ObservableProperty] private BankDeposit? _selectedUnclearedDeposit;
    [ObservableProperty] private DateTime _depositClearanceDate = DateTime.Today;

    // TAB 4: Bank Cheques Editing & History (AgenSoft Exact FrmChequesEditing Parity)
    [ObservableProperty] private BankAccount? _selectedHistoryBank;
    [ObservableProperty] private DateTime? _historyEntryDate;
    [ObservableProperty] private ObservableCollection<ChequeHistoryDto> _historyCheques = new();
    [ObservableProperty] private ChequeHistoryDto? _selectedHistoryCheque;
    [ObservableProperty] private string _historyCancelRemarks = string.Empty;

    // Cheques Register
    [ObservableProperty] private ObservableCollection<Cheque> _allIssuedCheques = new();
    [ObservableProperty] private Cheque? _selectedRegisterCheque;
    [ObservableProperty] private ObservableCollection<string> _statusFilters = new() { "All", "Blank", "Issued", "Cleared", "Bounced", "Cancelled" };
    [ObservableProperty] private string _selectedStatusFilter = "Issued";
    [ObservableProperty] private string _cancellationReason = string.Empty;

    public Action? RequestClose { get; set; }

    public BankingViewModel(IBankingService bankingService, IPartyService partyService, ISystemService systemService)
    {
        _bankingService = bankingService;
        _partyService = partyService;
        _systemService = systemService;

        _ = LoadInitialDataAsync();
    }

    public async Task LoadInitialDataAsync()
    {
        try
        {
            var banks = await _bankingService.GetAllBankAccountsAsync();
            BankAccounts = new ObservableCollection<BankAccount>(banks);

            if (BankAccounts.Any())
            {
                SelectedGenBankAccount = BankAccounts.First();
                SelectedIssueBankAccount = BankAccounts.First();
                SelectedDepositBankAccount = BankAccounts.First();
            }

            var parties = await _partyService.GetAllPartiesAsync();
            Parties = new ObservableCollection<Party>(parties);

            var seasons = await _systemService.GetActiveSeasonsAsync();
            CropSeasons = new ObservableCollection<CropSeason>(seasons);
            SelectedCropSeason = seasons.FirstOrDefault(s => s.IsActive) ?? seasons.FirstOrDefault();

            await RefreshTabAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Banking Initial Data Load");
        }
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        _ = RefreshTabAsync();
    }

    partial void OnSelectedGenBankAccountChanged(BankAccount? value)
    {
        if (value != null)
        {
            _ = LoadChequeBooksForBankAsync(value.Id);
        }
    }

    partial void OnSelectedIssueBankAccountChanged(BankAccount? value)
    {
        if (value != null)
        {
            _ = LoadAvailableChequesForBankAsync(value.Id);
        }
    }

    partial void OnSeriesStartChanged(int value)
    {
        if (SeriesEnd >= value)
        {
            TotalLeaves = (SeriesEnd - value) + 1;
        }
    }

    partial void OnSeriesEndChanged(int value)
    {
        if (value >= SeriesStart)
        {
            TotalLeaves = (value - SeriesStart) + 1;
        }
    }

    partial void OnSelectedPartyIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            PartyName = "";
            SelectedParty = null;
            return;
        }

        var p = Parties.FirstOrDefault(x => x.PartyNo == value || x.Id.ToString() == value);
        if (p != null)
        {
            SelectedParty = p;
            PartyName = p.Name;
        }
    }

    partial void OnDepositPartyIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            DepositPartyName = "";
            return;
        }

        var p = Parties.FirstOrDefault(x => x.PartyNo == value || x.Id.ToString() == value);
        if (p != null)
        {
            DepositPartyName = p.Name;
        }
    }

    partial void OnSelectedDepositTypeChanged(string value)
    {
        IsChequeDepositMode = (value == "Cheque Deposit");
    }

    partial void OnIsReconcilingChequesChanged(bool value)
    {
        if (value && IsReconcilingDeposits)
        {
            IsReconcilingDeposits = false;
        }
        else
        {
            _ = RefreshTabAsync();
        }
    }

    partial void OnIsReconcilingDepositsChanged(bool value)
    {
        if (value && IsReconcilingCheques)
        {
            IsReconcilingCheques = false;
        }
        else
        {
            _ = RefreshTabAsync();
        }
    }

    partial void OnSelectedStatusFilterChanged(string value)
    {
        _ = LoadRegisterAsync();
    }

    public async Task RefreshTabAsync()
    {
        if (SelectedTabIndex == 0 && SelectedGenBankAccount != null)
        {
            await LoadChequeBooksForBankAsync(SelectedGenBankAccount.Id);
        }
        else if (SelectedTabIndex == 1 && SelectedIssueBankAccount != null)
        {
            await LoadAvailableChequesForBankAsync(SelectedIssueBankAccount.Id);
        }
        else if (SelectedTabIndex == 3)
        {
            if (IsReconcilingDeposits)
            {
                await LoadUnclearedDepositsAsync();
            }
            else
            {
                await LoadUnclearedChequesAsync();
            }
        }
        else if (SelectedTabIndex == 4)
        {
            await LoadRegisterAsync();
            await LoadHistoryAsync();
        }
    }

    private async Task LoadChequeBooksForBankAsync(int bankAccountId)
    {
        var books = await _bankingService.GetChequeBooksAsync(bankAccountId);
        ExistingChequeBooks = new ObservableCollection<ChequeBook>(books);
    }

    private async Task LoadAvailableChequesForBankAsync(int bankAccountId)
    {
        var cheques = await _bankingService.GetAvailableChequesAsync(bankAccountId);
        AvailableCheques = new ObservableCollection<Cheque>(cheques);
        SelectedAvailableCheque = AvailableCheques.FirstOrDefault();
    }

    private async Task LoadUnclearedChequesAsync()
    {
        var cheques = await _bankingService.GetUnclearedChequesAsync();
        UnclearedCheques = new ObservableCollection<Cheque>(cheques);
        SelectedUnclearedCheque = UnclearedCheques.FirstOrDefault();
    }

    private async Task LoadRegisterAsync()
    {
        if (SelectedStatusFilter == "All")
        {
            var all = await _bankingService.GetUnclearedChequesAsync();
            AllIssuedCheques = new ObservableCollection<Cheque>(all);
        }
        else if (Enum.TryParse<ChequeStatus>(SelectedStatusFilter, out var status))
        {
            var filtered = await _bankingService.GetChequesByStatusAsync(status);
            AllIssuedCheques = new ObservableCollection<Cheque>(filtered);
        }
    }

    [RelayCommand]
    public async Task LoadHistoryAsync()
    {
        try
        {
            string? acNo = SelectedHistoryBank?.Account?.AccountNo ?? SelectedHistoryBank?.AccountNumber;
            var list = await _bankingService.GetChequeBookHistoryAsync(acNo, HistoryEntryDate);
            HistoryCheques = new ObservableCollection<ChequeHistoryDto>(list);
            SelectedHistoryCheque = HistoryCheques.FirstOrDefault();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Cheque History");
        }
    }

    [RelayCommand]
    private async Task CancelHistoryChequeAsync()
    {
        if (SelectedHistoryCheque == null)
        {
            MessageBox.Show("Please select a cheque leaf to cancel.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedHistoryCheque.IsCancelled)
        {
            MessageBox.Show("This cheque is already cancelled.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var reason = string.IsNullOrWhiteSpace(HistoryCancelRemarks) ? "Cancelled by User" : HistoryCancelRemarks.Trim();
        var confirm = MessageBox.Show($"Are you sure you want to cancel Cheque #{SelectedHistoryCheque.ChequeNo}?\nRemarks: {reason}", "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            await _bankingService.CancelChequeInHistoryAsync(SelectedHistoryCheque.AccountNo, SelectedHistoryCheque.ChequeNo, reason);
            MessageBox.Show($"Cheque #{SelectedHistoryCheque.ChequeNo} has been cancelled in history.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            HistoryCancelRemarks = string.Empty;
            await LoadHistoryAsync();
            await LoadRegisterAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Cancel Cheque History");
        }
    }

    [RelayCommand]
    private async Task DeleteHistoryChequeAsync()
    {
        if (SelectedHistoryCheque == null)
        {
            MessageBox.Show("Please select an unused cheque leaf to delete.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirm = MessageBox.Show($"Are you sure you want to permanently DELETE unused Cheque #{SelectedHistoryCheque.ChequeNo} from history?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            await _bankingService.DeleteChequeFromHistoryAsync(SelectedHistoryCheque.AccountNo, SelectedHistoryCheque.ChequeNo);
            MessageBox.Show($"Cheque #{SelectedHistoryCheque.ChequeNo} deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadHistoryAsync();
            await LoadRegisterAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Delete Cheque History");
        }
    }

    [RelayCommand]
    private void OpenAddBankCard()
    {
        NewBankName = string.Empty;
        NewBranchName = string.Empty;
        NewAccountNumber = string.Empty;
        NewBankOpeningBalance = 0;
        IsAddBankCardVisible = true;
    }

    [RelayCommand]
    private void CancelAddBankCard()
    {
        IsAddBankCardVisible = false;
    }

    [RelayCommand]
    private async Task SaveNewBankAccountAsync()
    {
        if (string.IsNullOrWhiteSpace(NewBankName))
        {
            MessageBox.Show("Please enter the Bank Name (e.g. Habib Bank Limited).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(NewAccountNumber))
        {
            MessageBox.Show("Please enter the Bank Account Number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var createdBank = await _bankingService.CreateBankAccountWithGlAccountAsync(
                NewBankName,
                NewBranchName,
                NewAccountNumber,
                NewBankOpeningBalance);

            MessageBox.Show($"Bank Account '{createdBank.BankName}' (A/C: {createdBank.AccountNumber}) created successfully and linked to General Ledger!", "Bank Account Created", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reload bank accounts
            var banks = await _bankingService.GetAllBankAccountsAsync();
            BankAccounts = new ObservableCollection<BankAccount>(banks);

            // Select the newly created bank account
            var matching = BankAccounts.FirstOrDefault(b => b.Id == createdBank.Id);
            if (matching != null)
            {
                SelectedGenBankAccount = matching;
                SelectedIssueBankAccount = matching;
                SelectedDepositBankAccount = matching;
                SelectedHistoryBank = matching;
            }

            IsAddBankCardVisible = false;
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Create Bank Account");
        }
    }

    [RelayCommand]
    private async Task GenerateChequeBookAsync()
    {
        if (SelectedGenBankAccount == null)
        {
            MessageBox.Show("Please select a Bank Account.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SeriesEnd < SeriesStart)
        {
            MessageBox.Show("Series End must be greater than or equal to Series Start.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var book = await _bankingService.GenerateChequeBookAsync(SelectedGenBankAccount.Id, SeriesStart, SeriesEnd);
            MessageBox.Show($"Cheque Book with {TotalLeaves} leaves ({SeriesStart:D6} to {SeriesEnd:D6}) generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            await LoadChequeBooksForBankAsync(SelectedGenBankAccount.Id);
            if (SelectedIssueBankAccount?.Id == SelectedGenBankAccount.Id)
            {
                await LoadAvailableChequesForBankAsync(SelectedGenBankAccount.Id);
            }

            SeriesStart = 0;
            SeriesEnd = 0;
            TotalLeaves = 0;
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Generate Cheque Book");
        }
    }

    [RelayCommand]
    private async Task IssueChequeAsync()
    {
        if (SelectedIssueBankAccount == null)
        {
            MessageBox.Show("Please select a Bank Account.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedAvailableCheque == null)
        {
            MessageBox.Show("Please select an available blank cheque to issue.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedParty == null)
        {
            MessageBox.Show("Please select or enter a valid Party.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IssueAmount <= 0)
        {
            MessageBox.Show("Please enter a valid Cheque Amount greater than zero.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var year = await _systemService.GetCurrentYearAsync();
        if (year == null)
        {
            MessageBox.Show("Active Financial Year not found.", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            await _bankingService.IssueChequeAsync(
                SelectedAvailableCheque.Id,
                SelectedParty.AccountId,
                IssueAmount,
                IssueDate,
                Narration,
                ReceivedBy,
                year.Id,
                SelectedCropSeason?.Id);

            MessageBox.Show($"Cheque #{SelectedAvailableCheque.ChequeNo} of Rs. {IssueAmount:N2} successfully issued to {PartyName} and posted to General Ledger!", "Cheque Issued", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reset issuing inputs
            IssueAmount = 0;
            ReceivedBy = string.Empty;
            Narration = string.Empty;
            SelectedPartyId = string.Empty;
            PartyName = string.Empty;
            SelectedParty = null;

            await LoadAvailableChequesForBankAsync(SelectedIssueBankAccount.Id);
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Issue Cheque");
        }
    }

    [RelayCommand]
    private async Task DepositAsync()
    {
        if (SelectedDepositBankAccount == null)
        {
            MessageBox.Show("Please select a Bank Account.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (DepositAmount <= 0)
        {
            MessageBox.Show("Please enter a valid Deposit Amount.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var year = await _systemService.GetCurrentYearAsync();
        if (year == null)
        {
            MessageBox.Show("Active Financial Year not found.", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (SelectedDepositType == "Cash Deposit")
            {
                await _bankingService.DepositCashToBankAsync(
                    SelectedDepositBankAccount.Id,
                    DepositAmount,
                    DepositDate,
                    DepositSlipNo,
                    DepositNarration,
                    year.Id,
                    SelectedCropSeason?.Id);

                MessageBox.Show($"Cash Deposit of Rs. {DepositAmount:N2} in {SelectedDepositBankAccount.BankName} posted successfully (Contra entry created)!", "Deposit Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var party = Parties.FirstOrDefault(p => p.PartyNo == DepositPartyId || p.Id.ToString() == DepositPartyId);
                if (party == null)
                {
                    MessageBox.Show("Please enter a valid Party for Cheque Deposit.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _bankingService.DepositChequeToBankAsync(
                    SelectedDepositBankAccount.Id,
                    party.AccountId,
                    DepositChequeNo,
                    DepositAmount,
                    DepositChequeDate,
                    DepositDate,
                    DepositSlipNo,
                    DepositNarration,
                    year.Id,
                    SelectedCropSeason?.Id);

                MessageBox.Show($"Cheque #{DepositChequeNo} from {party.Name} deposited in {SelectedDepositBankAccount.BankName} successfully!", "Deposit Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Reset deposit fields
            DepositAmount = 0;
            DepositSlipNo = string.Empty;
            DepositChequeNo = string.Empty;
            DepositNarration = string.Empty;
            DepositPartyId = string.Empty;
            DepositPartyName = string.Empty;
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Bank Deposit");
        }
    }

    [RelayCommand]
    private async Task ClearChequeAsync()
    {
        if (SelectedUnclearedCheque == null)
        {
            MessageBox.Show("Please select an uncleared cheque from the grid.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _bankingService.ReconcileChequeAsync(SelectedUnclearedCheque.Id, ClearanceDate);
            MessageBox.Show($"Cheque #{SelectedUnclearedCheque.ChequeNo} marked as Cleared on {ClearanceDate:d}!", "Reconciliation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadUnclearedChequesAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Cheque Clearance");
        }
    }

    [RelayCommand]
    private async Task BounceChequeAsync()
    {
        if (SelectedUnclearedCheque == null)
        {
            MessageBox.Show("Please select a cheque to mark as Bounced.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(BounceReason))
        {
            BounceReason = "Insufficient Funds";
        }

        try
        {
            await _bankingService.BounceChequeAsync(SelectedUnclearedCheque.Id, BounceReason);
            MessageBox.Show($"Cheque #{SelectedUnclearedCheque.ChequeNo} marked as Bounced. GL reversal entries have been posted!", "Cheque Bounced", MessageBoxButton.OK, MessageBoxImage.Information);
            BounceReason = string.Empty;
            await LoadUnclearedChequesAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Cheque Bouncing");
        }
    }

    public async Task LoadUnclearedDepositsAsync()
    {
        try
        {
            var deposits = await _bankingService.GetUnclearedDepositsAsync();
            UnclearedDeposits = new ObservableCollection<BankDeposit>(deposits);
            SelectedUnclearedDeposit = UnclearedDeposits.FirstOrDefault();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Uncleared Deposits");
        }
    }

    [RelayCommand]
    private async Task ClearDepositAsync()
    {
        if (SelectedUnclearedDeposit == null)
        {
            MessageBox.Show("Please select an uncleared deposit from the grid.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _bankingService.ReconcileDepositAsync(SelectedUnclearedDeposit.Id, DepositClearanceDate);
            MessageBox.Show($"Bank Deposit ({SelectedUnclearedDeposit.ReferenceNo}) of Rs. {SelectedUnclearedDeposit.Amount:N2} marked as Reconciled/Cleared on {DepositClearanceDate:d}!", "Deposit Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadUnclearedDepositsAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Deposit Clearance");
        }
    }

    [RelayCommand]
    private async Task CancelChequeAsync()
    {
        if (SelectedRegisterCheque == null)
        {
            MessageBox.Show("Please select a cheque from the register to cancel.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedRegisterCheque.Status == ChequeStatus.Cancelled)
        {
            MessageBox.Show("This cheque is already cancelled.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (SelectedRegisterCheque.Status == ChequeStatus.Cleared)
        {
            MessageBox.Show("Cleared cheques cannot be cancelled directly. You must first reverse or bounce the transaction.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var reason = string.IsNullOrWhiteSpace(CancellationReason) ? "Cancelled by User" : CancellationReason.Trim();

        var confirm = MessageBox.Show($"Are you sure you want to cancel Cheque #{SelectedRegisterCheque.ChequeNo}?\nReason: {reason}", "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            await _bankingService.CancelChequeAsync(SelectedRegisterCheque.Id, reason);
            MessageBox.Show($"Cheque #{SelectedRegisterCheque.ChequeNo} has been cancelled successfully.", "Cheque Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            CancellationReason = string.Empty;
            await LoadRegisterAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Cancel Cheque");
        }
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
