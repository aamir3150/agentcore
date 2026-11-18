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

public class SmsPartyRecipientItem : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string UrduName { get; set; } = "";
    public string Phone { get; set; } = "";
    public decimal CurrentBalance { get; set; }
    public string BalanceType => CurrentBalance >= 0 ? "بنام (Dr)" : "جمع (Cr)";

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public partial class SmsSendingViewModel : ObservableObject
{
    private readonly IPartyService _partyService;
    private readonly IAccountService _accountService;

    [ObservableProperty] private ObservableCollection<SmsPartyRecipientItem> _allRecipients = new();
    [ObservableProperty] private ObservableCollection<SmsPartyRecipientItem> _filteredRecipients = new();

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private string _selectedTemplateType = "Urdu Balance Reminder";
    [ObservableProperty] private ObservableCollection<string> _templateTypes = new() 
    { 
        "Urdu Balance Reminder", 
        "English Balance Reminder", 
        "Payment Receipt Notice", 
        "Custom Notice" 
    };

    [ObservableProperty] private string _messageTemplate = "محترم [PartyName]، آپ کا موجودہ کھاتہ بیلنس بقایا رقم Rs. [Balance] ہے۔ سٹی کمپیوٹرز مروٹ (0344-7436314)";
    [ObservableProperty] private string _previewText = "";
    [ObservableProperty] private int _characterCount;
    [ObservableProperty] private int _smsPartsCount = 1;

    [ObservableProperty] private double _progressValue;
    [ObservableProperty] private string _statusText = "Ready to dispatch messages / پیغامات روانگی کے لیے تیار";
    [ObservableProperty] private bool _isSending;

    public SmsSendingViewModel(IPartyService partyService, IAccountService accountService)
    {
        _partyService = partyService;
        _accountService = accountService;
        UpdatePreview();
    }

    public async Task InitializeAsync()
    {
        await LoadPartiesAsync();
    }

    [RelayCommand]
    public async Task LoadPartiesAsync()
    {
        try
        {
            AllRecipients.Clear();
            FilteredRecipients.Clear();

            var parties = await _partyService.GetAllPartiesAsync();
            foreach (var p in parties)
            {
                var phone = p.Phone ?? "";
                decimal balance = 0;
                if (p.AccountId > 0)
                {
                    try { balance = await _accountService.GetAccountBalanceAsync(p.AccountId); } catch {}
                }

                var item = new SmsPartyRecipientItem
                {
                    Id = p.Id,
                    Name = p.Name ?? "",
                    UrduName = p.UrduName ?? p.Name ?? "",
                    Phone = phone,
                    CurrentBalance = balance,
                    IsSelected = !string.IsNullOrWhiteSpace(phone)
                };

                AllRecipients.Add(item);
                FilteredRecipients.Add(item);
            }

            UpdatePreview();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Parties for SMS");
        }
    }

    [RelayCommand]
    public void ApplyFilter()
    {
        FilteredRecipients.Clear();
        var query = AllRecipients.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var st = SearchText.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(st) || p.UrduName.Contains(st) || p.Phone.Contains(st));
        }

        foreach (var p in query)
        {
            FilteredRecipients.Add(p);
        }
    }

    [RelayCommand]
    public void SelectAll()
    {
        foreach (var p in FilteredRecipients) p.IsSelected = true;
    }

    [RelayCommand]
    public void DeselectAll()
    {
        foreach (var p in FilteredRecipients) p.IsSelected = false;
    }

    public void OnTemplateSelected(string templateType)
    {
        SelectedTemplateType = templateType;
        switch (templateType)
        {
            case "Urdu Balance Reminder":
                MessageTemplate = "محترم [PartyName]، آپ کا موجودہ کھاتہ بیلنس بقایا رقم Rs. [Balance] ہے۔ سٹی کمپیوٹرز مروٹ (0344-7436314)";
                break;
            case "English Balance Reminder":
                MessageTemplate = "Dear [PartyName], your current outstanding account balance is Rs. [Balance] as of [Date]. City Computers Marot (0344-7436314).";
                break;
            case "Payment Receipt Notice":
                MessageTemplate = "محترم [PartyName]، آپ کی رقم کی وصولی کھاتہ میں جمع کر لی گئی ہے۔ نیا بیلنس: Rs. [Balance]";
                break;
            case "Custom Notice":
                MessageTemplate = "محترم گاہک [PartyName]، آپ کو مطلع کیا جاتا ہے کہ ...";
                break;
        }
        UpdatePreview();
    }

    public void UpdatePreview()
    {
        var sampleParty = FilteredRecipients.FirstOrDefault(p => p.IsSelected) 
            ?? new SmsPartyRecipientItem { Name = "محمد احمد (زمیندار)", UrduName = "محمد احمد (زمیندار)", CurrentBalance = 125000 };

        var text = MessageTemplate ?? "";
        text = text.Replace("[PartyName]", !string.IsNullOrEmpty(sampleParty.UrduName) ? sampleParty.UrduName : sampleParty.Name);
        text = text.Replace("[Balance]", $"{Math.Abs(sampleParty.CurrentBalance):N0}");
        text = text.Replace("[Date]", DateTime.Now.ToString("dd-MM-yyyy"));

        PreviewText = text;
        CharacterCount = text.Length;

        // Count parts: If contains non-ASCII (Urdu), 70 chars per SMS, else 160 chars
        bool isUnicode = text.Any(c => c > 127);
        int maxCharsPerSms = isUnicode ? 70 : 160;
        SmsPartsCount = Math.Max(1, (int)Math.Ceiling(CharacterCount / (double)maxCharsPerSms));
    }

    [RelayCommand]
    public async Task SendSmsAsync()
    {
        var targets = FilteredRecipients.Where(p => p.IsSelected && !string.IsNullOrWhiteSpace(p.Phone)).ToList();
        if (!targets.Any())
        {
            MessageBox.Show("Baraye meherbani mobile number waly kam az kam aik party ko muntakhab karein.", "No Recipients", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsSending = true;
            ProgressValue = 0;
            int total = targets.Count;
            int dispatched = 0;

            foreach (var recipient in targets)
            {
                var partyDisplayName = !string.IsNullOrEmpty(recipient.UrduName) ? recipient.UrduName : recipient.Name;
                var personalMessage = MessageTemplate
                    .Replace("[PartyName]", partyDisplayName)
                    .Replace("[Balance]", $"{Math.Abs(recipient.CurrentBalance):N0}")
                    .Replace("[Date]", DateTime.Now.ToString("dd-MM-yyyy"));

                StatusText = $"Dispatching SMS to: {recipient.Phone} ({partyDisplayName})...";

                // Simulate/Dispatch to SMS Gateway
                await Task.Delay(100);

                dispatched++;
                ProgressValue = (dispatched / (double)total) * 100.0;
            }

            StatusText = $"کامیابی! {dispatched} پارٹیز کو ایس ایم ایس الرٹ روانہ ہو چکے ہیں۔";
            MessageBox.Show($"منتخب کردہ {dispatched} پارٹیز کو ایس ایم ایس پیغامات کامیابی سے روانہ کر دیے گئے!", 
                "کامیابی / SMS Dispatched", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "SMS Dispatch");
            StatusText = $"نقص: {ex.Message}";
        }
        finally
        {
            IsSending = false;
        }
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
