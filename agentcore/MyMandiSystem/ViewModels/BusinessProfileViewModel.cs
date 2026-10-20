using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using MyMandiSystem.Printing;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class BusinessProfileViewModel : ObservableObject
{
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _businessName = "";
    [ObservableProperty] private string _businessNameUrdu = "";
    [ObservableProperty] private string _proprietor = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private string _phone = "";
    [ObservableProperty] private string _registrationNo = "";
    [ObservableProperty] private bool _isSaving;

    public BusinessProfileViewModel(ISystemService systemService)
    {
        _systemService = systemService;
        LoadCurrentProfile();
    }

    public void LoadCurrentProfile()
    {
        var comp = PrintContext.Company;
        BusinessName = comp.Name;
        BusinessNameUrdu = comp.NameUrdu;
        Proprietor = comp.Proprietor;
        Address = comp.Address;
        Phone = comp.Phone;
        RegistrationNo = comp.RegistrationNo ?? "";
    }

    [RelayCommand]
    public async Task SaveAsync(Window? window)
    {
        if (string.IsNullOrWhiteSpace(BusinessName) && string.IsNullOrWhiteSpace(BusinessNameUrdu))
        {
            MessageBox.Show("Baraye meherbani Business / Arhat ka naam darj karein.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsSaving = true;

            // 1. Save to Database SystemConfigs
            await _systemService.SetConfigValueAsync("Business_Name", BusinessName?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_NameUrdu", BusinessNameUrdu?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_Proprietor", Proprietor?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_Address", Address?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_Phone", Phone?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_RegistrationNo", RegistrationNo?.Trim() ?? "");

            // 2. Update Live In-Memory PrintContext
            PrintContext.Company.Name = BusinessName?.Trim() ?? "";
            PrintContext.Company.NameUrdu = BusinessNameUrdu?.Trim() ?? "";
            PrintContext.Company.Proprietor = Proprietor?.Trim() ?? "";
            PrintContext.Company.Address = Address?.Trim() ?? "";
            PrintContext.Company.Phone = Phone?.Trim() ?? "";
            PrintContext.Company.RegistrationNo = RegistrationNo?.Trim() ?? "";

            MessageBox.Show("کاروباری تفصیلات اور پرنٹ ہیڈر کامیابی سے محفوظ ہو گئے!\n(Business Profile successfully saved and applied to all prints)", 
                "Success / کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);

            if (window != null)
            {
                try { window.DialogResult = true; }
                catch { window.Close(); }
            }
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Business Profile Save");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
