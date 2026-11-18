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

    // Tab 1: Profile & Mandi Information
    [ObservableProperty] private string _businessName = "";
    [ObservableProperty] private string _businessNameUrdu = "";
    [ObservableProperty] private string _proprietor = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private string _city = "";
    [ObservableProperty] private string _phone = "";
    [ObservableProperty] private string _phone2 = "";
    [ObservableProperty] private string _mobile = "";
    [ObservableProperty] private string _registrationNo = "";
    [ObservableProperty] private string _ntn = "";
    [ObservableProperty] private string _stn = "";

    // Tab 2: System, Printer & Auto-Backup Preferences
    [ObservableProperty] private bool _thermalPrint;
    [ObservableProperty] private bool _autoPrint;
    [ObservableProperty] private bool _directPrintVoucher;
    [ObservableProperty] private bool _showReportBodyLines = true;
    [ObservableProperty] private bool _hourlyBackupEnabled;
    [ObservableProperty] private string _hourlyBackupTime = "60";
    [ObservableProperty] private string _backupPath = "";

    // Tab 3: Urdu Footers & Mandi Terms
    [ObservableProperty] private string _footerGenUrdu = "کسی بھی قسم کے نقص یا شکایت کی صورت میں تین دن کے اندر رابطہ کریں۔";
    [ObservableProperty] private string _footerCropUrdu = "وزن اور کٹوتی غلہ منڈی کے مروجہ قوانین کے مطابق کی گئی ہے۔";
    [ObservableProperty] private string _footerPestUrdu = "ادویات کے استعمال سے قبل لیبل کی ہدایات پر عمل لازمی ہے۔";
    [ObservableProperty] private string _footerBrokUrdu = "بروکریج سودا دونوں فریقین کی باہمی رضامندی سے طے پایا ہے۔";

    [ObservableProperty] private bool _isSaving;

    public BusinessProfileViewModel(ISystemService systemService)
    {
        _systemService = systemService;
        _ = LoadCurrentProfileAsync();
    }

    public async Task LoadCurrentProfileAsync()
    {
        try
        {
            var comp = PrintContext.Company;
            BusinessName = comp.Name;
            BusinessNameUrdu = comp.NameUrdu;
            Proprietor = comp.Proprietor;
            Address = comp.Address;
            Phone = comp.Phone;
            RegistrationNo = comp.RegistrationNo ?? "";

            City = await _systemService.GetConfigValueAsync("Business_City");
            Phone2 = await _systemService.GetConfigValueAsync("Business_Phone2");
            Mobile = await _systemService.GetConfigValueAsync("Business_Mobile");
            Ntn = await _systemService.GetConfigValueAsync("Business_NTN");
            Stn = await _systemService.GetConfigValueAsync("Business_STN");

            var thermal = await _systemService.GetConfigValueAsync("Print_Thermal");
            ThermalPrint = thermal == "1" || thermal.Equals("true", StringComparison.OrdinalIgnoreCase);

            var autoP = await _systemService.GetConfigValueAsync("Print_AutoPrint");
            AutoPrint = autoP == "1" || autoP.Equals("true", StringComparison.OrdinalIgnoreCase);

            var directP = await _systemService.GetConfigValueAsync("Print_DirectPrint");
            DirectPrintVoucher = directP == "1" || directP.Equals("true", StringComparison.OrdinalIgnoreCase);

            var showL = await _systemService.GetConfigValueAsync("Print_ShowBodyLines");
            if (!string.IsNullOrEmpty(showL)) ShowReportBodyLines = showL == "1" || showL.Equals("true", StringComparison.OrdinalIgnoreCase);

            var hBackup = await _systemService.GetConfigValueAsync("Backup_HourlyEnabled");
            HourlyBackupEnabled = hBackup == "1" || hBackup.Equals("true", StringComparison.OrdinalIgnoreCase);

            var bTime = await _systemService.GetConfigValueAsync("Backup_HourlyTime");
            if (!string.IsNullOrEmpty(bTime)) HourlyBackupTime = bTime;

            BackupPath = await _systemService.GetConfigValueAsync("Backup_Path");

            var fGen = await _systemService.GetConfigValueAsync("Footer_GenUrdu");
            if (!string.IsNullOrEmpty(fGen)) FooterGenUrdu = fGen;

            var fCrop = await _systemService.GetConfigValueAsync("Footer_CropUrdu");
            if (!string.IsNullOrEmpty(fCrop)) FooterCropUrdu = fCrop;

            var fPest = await _systemService.GetConfigValueAsync("Footer_PestUrdu");
            if (!string.IsNullOrEmpty(fPest)) FooterPestUrdu = fPest;

            var fBrok = await _systemService.GetConfigValueAsync("Footer_BrokUrdu");
            if (!string.IsNullOrEmpty(fBrok)) FooterBrokUrdu = fBrok;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading business profile: {ex.Message}");
        }
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
            await _systemService.SetConfigValueAsync("Business_City", City?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_Phone", Phone?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_Phone2", Phone2?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_Mobile", Mobile?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_RegistrationNo", RegistrationNo?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_NTN", Ntn?.Trim() ?? "");
            await _systemService.SetConfigValueAsync("Business_STN", Stn?.Trim() ?? "");

            await _systemService.SetConfigValueAsync("Print_Thermal", ThermalPrint ? "1" : "0");
            await _systemService.SetConfigValueAsync("Print_AutoPrint", AutoPrint ? "1" : "0");
            await _systemService.SetConfigValueAsync("Print_DirectPrint", DirectPrintVoucher ? "1" : "0");
            await _systemService.SetConfigValueAsync("Print_ShowBodyLines", ShowReportBodyLines ? "1" : "0");

            await _systemService.SetConfigValueAsync("Backup_HourlyEnabled", HourlyBackupEnabled ? "1" : "0");
            await _systemService.SetConfigValueAsync("Backup_HourlyTime", HourlyBackupTime ?? "60");
            await _systemService.SetConfigValueAsync("Backup_Path", BackupPath ?? "");

            await _systemService.SetConfigValueAsync("Footer_GenUrdu", FooterGenUrdu ?? "");
            await _systemService.SetConfigValueAsync("Footer_CropUrdu", FooterCropUrdu ?? "");
            await _systemService.SetConfigValueAsync("Footer_PestUrdu", FooterPestUrdu ?? "");
            await _systemService.SetConfigValueAsync("Footer_BrokUrdu", FooterBrokUrdu ?? "");

            // 2. Update Live In-Memory PrintContext
            PrintContext.Company.Name = BusinessName?.Trim() ?? "";
            PrintContext.Company.NameUrdu = BusinessNameUrdu?.Trim() ?? "";
            PrintContext.Company.Proprietor = Proprietor?.Trim() ?? "";
            PrintContext.Company.Address = Address?.Trim() ?? "";
            PrintContext.Company.Phone = Phone?.Trim() ?? "";
            PrintContext.Company.RegistrationNo = RegistrationNo?.Trim() ?? "";

            MessageBox.Show("کاروباری تفصیلات، پرنٹر ترتیبات اور اردو نوٹس کامیابی سے محفوظ ہو گئے!\n(Business Profile & System Preferences successfully saved)", 
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
