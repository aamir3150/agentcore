using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class ExpenseSchemeItem : ObservableObject
{
    public string ExpanseType { get; set; } = "";
    public bool IsPurchase { get; set; } = true;
    public decimal ArhatAmount { get; set; }
    public decimal MazdooriAmount { get; set; }
    public decimal ComisionAmount { get; set; }
    public decimal KantaExpense { get; set; }
    public string SummaryText => IsPurchase 
        ? $"آڑھت: {ArhatAmount:N2} | کمیشن: {ComisionAmount:N2} | کانٹا: {KantaExpense:N2}"
        : $"کمیشن: {ComisionAmount:N2} | مزدوری: {MazdooriAmount:N2} | کانٹا: {KantaExpense:N2}";
}

public partial class ExpenseTypesViewModel : ObservableObject
{
    private readonly ISystemConfigService _configService;

    [ObservableProperty] private int _selectedTabIndex = 0; // 0 = Purchase, 1 = Sale
    [ObservableProperty] private ObservableCollection<ExpenseSchemeItem> _purchaseSchemes = new();
    [ObservableProperty] private ObservableCollection<ExpenseSchemeItem> _saleSchemes = new();
    [ObservableProperty] private ExpenseSchemeItem? _selectedScheme;

    // Common Scheme Attributes
    [ObservableProperty] private string _schemeName = "";
    [ObservableProperty] private bool _isPurchase = true;

    // Purchase Deductions & Expenses
    [ObservableProperty] private decimal _purArhatAmount;
    [ObservableProperty] private string _purArhatType = "Per 100";
    [ObservableProperty] private bool _purArhatAdd = true;

    [ObservableProperty] private decimal _purMazdooriAmount;
    [ObservableProperty] private string _purMazdooriType = "Per Bag";
    [ObservableProperty] private bool _purMazdooriAdd = true;

    [ObservableProperty] private decimal _purLoadingAmount;
    [ObservableProperty] private string _purLoadingType = "Per Bag";

    [ObservableProperty] private decimal _purUnLoadingAmount;
    [ObservableProperty] private string _purUnLoadingType = "Per Bag";

    [ObservableProperty] private decimal _purDalaliAmount;
    [ObservableProperty] private decimal _purMarketFeesAmount;
    [ObservableProperty] private decimal _purCommissionAmount;
    [ObservableProperty] private decimal _purMunshianaAmount;
    [ObservableProperty] private string _purMunshianaPer = "Per Vehicle";
    [ObservableProperty] private decimal _purChangraiAmount;
    [ObservableProperty] private decimal _purDamiAmount;
    [ObservableProperty] private decimal _purWhtAmount;
    [ObservableProperty] private decimal _purKantaExpense;

    // Mandi Additional Expenses (Purchase)
    [ObservableProperty] private decimal _purStitching;
    [ObservableProperty] private decimal _purRentGodown;
    [ObservableProperty] private decimal _purChokidara;
    [ObservableProperty] private decimal _purBagUsage;
    [ObservableProperty] private decimal _purShortage;
    [ObservableProperty] private decimal _purTractorExp;
    [ObservableProperty] private decimal _purSotli;

    // Sale Deductions & Expenses
    [ObservableProperty] private decimal _saleCommissionAmount;
    [ObservableProperty] private decimal _saleMazdooriAmount;
    [ObservableProperty] private string _saleMazdooriType = "Per Bag";
    [ObservableProperty] private decimal _saleMarketFeesAmount;
    [ObservableProperty] private decimal _saleSootaliAmount;
    [ObservableProperty] private decimal _saleDalaliAmount;
    [ObservableProperty] private decimal _saleMunshianaAmount;
    [ObservableProperty] private decimal _saleWhtAmount;
    [ObservableProperty] private decimal _saleLoadingAmount;
    [ObservableProperty] private decimal _saleUnloadingAmount;
    [ObservableProperty] private decimal _saleDamiAmount;
    [ObservableProperty] private decimal _saleKantaExpense;

    // Search filter
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private bool _isEditing;

    public ExpenseTypesViewModel(ISystemConfigService configService)
    {
        _configService = configService;
    }

    public async Task InitializeAsync(int initialTabIndex = 0)
    {
        SelectedTabIndex = initialTabIndex;
        await LoadSchemesAsync();
    }

    [RelayCommand]
    public async Task LoadSchemesAsync()
    {
        try
        {
            PurchaseSchemes.Clear();
            SaleSchemes.Clear();

            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = @"
                SELECT ExpanseType, IsPurchase, 
                       ISNULL(ArhatAmount, 0) as ArhatAmount, 
                       ISNULL(MazdooriAmount, 0) as MazdooriAmount, 
                       ISNULL(ComisionAmount, 0) as ComisionAmount, 
                       ISNULL(KantaExpenseP, 0) as KantaExpenseP,
                       ISNULL(MazdooriAmountS, 0) as MazdooriAmountS,
                       ISNULL(ComisionAmountS, 0) as ComisionAmountS,
                       ISNULL(KantaExpenseS, 0) as KantaExpenseS
                FROM ExpenseType 
                ORDER BY ExpanseType";

            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var isPur = reader["IsPurchase"] != DBNull.Value && Convert.ToBoolean(reader["IsPurchase"]);
                var item = new ExpenseSchemeItem
                {
                    ExpanseType = reader["ExpanseType"]?.ToString() ?? "",
                    IsPurchase = isPur,
                    ArhatAmount = Convert.ToDecimal(reader["ArhatAmount"]),
                    MazdooriAmount = isPur ? Convert.ToDecimal(reader["MazdooriAmount"]) : Convert.ToDecimal(reader["MazdooriAmountS"]),
                    ComisionAmount = isPur ? Convert.ToDecimal(reader["ComisionAmount"]) : Convert.ToDecimal(reader["ComisionAmountS"]),
                    KantaExpense = isPur ? Convert.ToDecimal(reader["KantaExpenseP"]) : Convert.ToDecimal(reader["KantaExpenseS"])
                };

                if (isPur) PurchaseSchemes.Add(item);
                else SaleSchemes.Add(item);
            }

            var targetList = SelectedTabIndex == 0 ? PurchaseSchemes : SaleSchemes;
            if (targetList.Any())
            {
                SelectedScheme = targetList.First();
                await LoadSchemeDetailsAsync(SelectedScheme.ExpanseType);
            }
            else
            {
                NewScheme();
            }
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Expense Types");
        }
    }

    [RelayCommand]
    public async Task SelectSchemeAsync(ExpenseSchemeItem? item)
    {
        if (item == null) return;
        SelectedScheme = item;
        await LoadSchemeDetailsAsync(item.ExpanseType);
    }

    private async Task LoadSchemeDetailsAsync(string schemeName)
    {
        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = "SELECT * FROM ExpenseType WHERE ExpanseType = @Name";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", schemeName);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                SchemeName = reader["ExpanseType"]?.ToString() ?? "";
                IsPurchase = reader["IsPurchase"] != DBNull.Value && Convert.ToBoolean(reader["IsPurchase"]);
                IsEditing = true;

                // Purchase fields
                PurArhatAmount = reader["ArhatAmount"] != DBNull.Value ? Convert.ToDecimal(reader["ArhatAmount"]) : 0;
                PurArhatType = reader["ArhatType"]?.ToString() ?? "Per 100";
                PurArhatAdd = (reader["Arhat"]?.ToString() ?? "+") == "+";

                PurMazdooriAmount = reader["MazdooriAmount"] != DBNull.Value ? Convert.ToDecimal(reader["MazdooriAmount"]) : 0;
                PurMazdooriType = reader["MazdooriType"]?.ToString() ?? "Per Bag";
                PurMazdooriAdd = (reader["Mazdoori"]?.ToString() ?? "+") == "+";

                PurLoadingAmount = reader["LoadingAmount"] != DBNull.Value ? Convert.ToDecimal(reader["LoadingAmount"]) : 0;
                PurLoadingType = reader["LoadingType"]?.ToString() ?? "Per Bag";

                PurUnLoadingAmount = reader["UnLoadingAmount"] != DBNull.Value ? Convert.ToDecimal(reader["UnLoadingAmount"]) : 0;
                PurUnLoadingType = reader["UnLoadingType"]?.ToString() ?? "Per Bag";

                PurDalaliAmount = reader["DalaliAmount"] != DBNull.Value ? Convert.ToDecimal(reader["DalaliAmount"]) : 0;
                PurMarketFeesAmount = reader["MarketAmount"] != DBNull.Value ? Convert.ToDecimal(reader["MarketAmount"]) : 0;
                PurCommissionAmount = reader["ComisionAmount"] != DBNull.Value ? Convert.ToDecimal(reader["ComisionAmount"]) : 0;
                PurMunshianaAmount = reader["MunshayanaAmount"] != DBNull.Value ? Convert.ToDecimal(reader["MunshayanaAmount"]) : 0;
                PurMunshianaPer = reader["MunshianaPer"]?.ToString() ?? "Per Vehicle";
                PurChangraiAmount = reader["ChangraiAmount"] != DBNull.Value ? Convert.ToDecimal(reader["ChangraiAmount"]) : 0;
                PurDamiAmount = reader["DamiAmount"] != DBNull.Value ? Convert.ToDecimal(reader["DamiAmount"]) : 0;
                PurWhtAmount = reader["WHTaxAmount"] != DBNull.Value ? Convert.ToDecimal(reader["WHTaxAmount"]) : 0;
                PurKantaExpense = reader["KantaExpenseP"] != DBNull.Value ? Convert.ToDecimal(reader["KantaExpenseP"]) : 0;

                PurStitching = reader["Stitching"] != DBNull.Value ? Convert.ToDecimal(reader["Stitching"]) : 0;
                PurRentGodown = reader["RentGodown"] != DBNull.Value ? Convert.ToDecimal(reader["RentGodown"]) : 0;
                PurChokidara = reader["ChokiDara"] != DBNull.Value ? Convert.ToDecimal(reader["ChokiDara"]) : 0;
                PurBagUsage = reader["BagUsage"] != DBNull.Value ? Convert.ToDecimal(reader["BagUsage"]) : 0;
                PurShortage = reader["ShortageG"] != DBNull.Value ? Convert.ToDecimal(reader["ShortageG"]) : 0;
                PurTractorExp = reader["TractorExp"] != DBNull.Value ? Convert.ToDecimal(reader["TractorExp"]) : 0;
                PurSotli = reader["Sotli"] != DBNull.Value ? Convert.ToDecimal(reader["Sotli"]) : 0;

                // Sale fields
                SaleCommissionAmount = reader["ComisionAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["ComisionAmountS"]) : 0;
                SaleMazdooriAmount = reader["MazdooriAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["MazdooriAmountS"]) : 0;
                SaleMazdooriType = reader["MazdooriTypeS"]?.ToString() ?? "Per Bag";
                SaleMarketFeesAmount = reader["MarketAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["MarketAmountS"]) : 0;
                SaleSootaliAmount = reader["SootaliAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["SootaliAmountS"]) : 0;
                SaleDalaliAmount = reader["DalaliAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["DalaliAmountS"]) : 0;
                SaleMunshianaAmount = reader["MunshayanaAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["MunshayanaAmountS"]) : 0;
                SaleWhtAmount = reader["WHTaxAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["WHTaxAmountS"]) : 0;
                SaleLoadingAmount = reader["LoadingAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["LoadingAmountS"]) : 0;
                SaleUnloadingAmount = reader["UnLoadingAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["UnLoadingAmountS"]) : 0;
                SaleDamiAmount = reader["DamiAmountS"] != DBNull.Value ? Convert.ToDecimal(reader["DamiAmountS"]) : 0;
                SaleKantaExpense = reader["KantaExpenseS"] != DBNull.Value ? Convert.ToDecimal(reader["KantaExpenseS"]) : 0;
            }
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Scheme Details");
        }
    }

    [RelayCommand]
    public void NewScheme()
    {
        SelectedScheme = null;
        SchemeName = SelectedTabIndex == 0 ? "Standard Purchase Scheme" : "Standard Sale Scheme";
        IsPurchase = SelectedTabIndex == 0;
        IsEditing = false;

        PurArhatAmount = 0;
        PurMazdooriAmount = 0;
        PurLoadingAmount = 0;
        PurUnLoadingAmount = 0;
        PurDalaliAmount = 0;
        PurMarketFeesAmount = 0;
        PurCommissionAmount = 0;
        PurMunshianaAmount = 0;
        PurChangraiAmount = 0;
        PurDamiAmount = 0;
        PurWhtAmount = 0;
        PurKantaExpense = 0;
        PurStitching = 0;
        PurRentGodown = 0;
        PurChokidara = 0;
        PurBagUsage = 0;
        PurShortage = 0;
        PurTractorExp = 0;
        PurSotli = 0;

        SaleCommissionAmount = 0;
        SaleMazdooriAmount = 0;
        SaleMarketFeesAmount = 0;
        SaleSootaliAmount = 0;
        SaleDalaliAmount = 0;
        SaleMunshianaAmount = 0;
        SaleWhtAmount = 0;
        SaleLoadingAmount = 0;
        SaleUnloadingAmount = 0;
        SaleDamiAmount = 0;
        SaleKantaExpense = 0;
    }

    [RelayCommand]
    public async Task SaveSchemeAsync()
    {
        if (string.IsNullOrWhiteSpace(SchemeName))
        {
            MessageBox.Show("Baraye meherbani Expense Scheme ka naam darj karein.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var isPur = SelectedTabIndex == 0;

            var saveSql = @"
MERGE INTO ExpenseType AS target
USING (VALUES (@ExpanseType)) AS source (ExpanseType)
ON target.ExpanseType = source.ExpanseType
WHEN MATCHED THEN
    UPDATE SET 
        IsPurchase = @IsPurchase,
        ArhatAmount = @ArhatAmount, ArhatType = @ArhatType, Arhat = @Arhat,
        MazdooriAmount = @MazdooriAmount, MazdooriType = @MazdooriType, Mazdoori = @Mazdoori,
        LoadingAmount = @LoadingAmount, LoadingType = @LoadingType, Loading = '+',
        UnLoadingAmount = @UnLoadingAmount, UnLoadingType = @UnLoadingType, UnLoading = '+',
        DalaliAmount = @DalaliAmount, Dalali = '+',
        MarketAmount = @MarketAmount, Market = '+',
        ComisionAmount = @ComisionAmount, Comision = '+',
        MunshayanaAmount = @MunshayanaAmount, MunshianaPer = @MunshianaPer, Munshayana = '+',
        ChangraiAmount = @ChangraiAmount, Changrai = '+',
        DamiAmount = @DamiAmount, DamiType = 'Percent', Dami = '+',
        WHTaxAmount = @WHTaxAmount, WHTax = '+',
        KantaExpenseP = @KantaExpenseP,
        Stitching = @Stitching, RentGodown = @RentGodown, ChokiDara = @ChokiDara,
        BagUsage = @BagUsage, ShortageG = @ShortageG, TractorExp = @TractorExp, Sotli = @Sotli,
        ComisionAmountS = @ComisionAmountS, ComisionS = '+',
        MazdooriAmountS = @MazdooriAmountS, MazdooriTypeS = @MazdooriTypeS, MazdooriS = '+',
        MarketAmountS = @MarketAmountS, MarketS = '+',
        SootaliAmountS = @SootaliAmountS, SootaliS = '+',
        DalaliAmountS = @DalaliAmountS, DalaliS = '+',
        MunshayanaAmountS = @MunshayanaAmountS, MunshayanaS = '+',
        WHTaxAmountS = @WHTaxAmountS, WHTaxS = '+',
        LoadingAmountS = @LoadingAmountS, LoadingTypeS = 'Per Bag', LoadingS = '+',
        UnLoadingAmountS = @UnLoadingAmountS, UnLoadingTypeS = 'Per Bag', UnLoadingS = '+',
        DamiAmountS = @DamiAmountS, DamiTypeS = 'Percent', DamiS = '+',
        KantaExpenseS = @KantaExpenseS
WHEN NOT MATCHED THEN
    INSERT (ExpanseType, IsPurchase, 
            ArhatAmount, ArhatType, Arhat, MazdooriAmount, MazdooriType, Mazdoori,
            LoadingAmount, LoadingType, Loading, UnLoadingAmount, UnLoadingType, UnLoading,
            DalaliAmount, Dalali, MarketAmount, Market, ComisionAmount, Comision,
            MunshayanaAmount, MunshianaPer, Munshayana, ChangraiAmount, Changrai,
            DamiAmount, DamiType, Dami, WHTaxAmount, WHTax, KantaExpenseP,
            Stitching, RentGodown, ChokiDara, BagUsage, ShortageG, TractorExp, Sotli,
            ComisionAmountS, ComisionS, MazdooriAmountS, MazdooriTypeS, MazdooriS,
            MarketAmountS, MarketS, SootaliAmountS, SootaliS, DalaliAmountS, DalaliS,
            MunshayanaAmountS, MunshayanaS, WHTaxAmountS, WHTaxS, LoadingAmountS, LoadingTypeS, LoadingS,
            UnLoadingAmountS, UnLoadingTypeS, UnLoadingS, DamiAmountS, DamiTypeS, DamiS, KantaExpenseS)
    VALUES (@ExpanseType, @IsPurchase, 
            @ArhatAmount, @ArhatType, @Arhat, @MazdooriAmount, @MazdooriType, @Mazdoori,
            @LoadingAmount, @LoadingType, '+', @UnLoadingAmount, @UnLoadingType, '+',
            @DalaliAmount, '+', @MarketAmount, '+', @ComisionAmount, '+',
            @MunshayanaAmount, @MunshianaPer, '+', @ChangraiAmount, '+',
            @DamiAmount, 'Percent', '+', @WHTaxAmount, '+', @KantaExpenseP,
            @Stitching, @RentGodown, @ChokiDara, @BagUsage, @ShortageG, @TractorExp, @Sotli,
            @ComisionAmountS, '+', @MazdooriAmountS, @MazdooriTypeS, '+',
            @MarketAmountS, '+', @SootaliAmountS, '+', @DalaliAmountS, '+',
            @MunshayanaAmountS, '+', @WHTaxAmountS, '+', @LoadingAmountS, 'Per Bag', '+',
            @UnLoadingAmountS, 'Per Bag', '+', @DamiAmountS, 'Percent', '+', @KantaExpenseS);";

            await using var cmd = new SqlCommand(saveSql, conn);
            cmd.Parameters.AddWithValue("@ExpanseType", SchemeName.Trim());
            cmd.Parameters.AddWithValue("@IsPurchase", isPur);

            // Purchase params
            cmd.Parameters.AddWithValue("@ArhatAmount", PurArhatAmount);
            cmd.Parameters.AddWithValue("@ArhatType", PurArhatType);
            cmd.Parameters.AddWithValue("@Arhat", PurArhatAdd ? "+" : "-");
            cmd.Parameters.AddWithValue("@MazdooriAmount", PurMazdooriAmount);
            cmd.Parameters.AddWithValue("@MazdooriType", PurMazdooriType);
            cmd.Parameters.AddWithValue("@Mazdoori", PurMazdooriAdd ? "+" : "-");
            cmd.Parameters.AddWithValue("@LoadingAmount", PurLoadingAmount);
            cmd.Parameters.AddWithValue("@LoadingType", PurLoadingType);
            cmd.Parameters.AddWithValue("@UnLoadingAmount", PurUnLoadingAmount);
            cmd.Parameters.AddWithValue("@UnLoadingType", PurUnLoadingType);
            cmd.Parameters.AddWithValue("@DalaliAmount", PurDalaliAmount);
            cmd.Parameters.AddWithValue("@MarketAmount", PurMarketFeesAmount);
            cmd.Parameters.AddWithValue("@ComisionAmount", PurCommissionAmount);
            cmd.Parameters.AddWithValue("@MunshayanaAmount", PurMunshianaAmount);
            cmd.Parameters.AddWithValue("@MunshianaPer", PurMunshianaPer);
            cmd.Parameters.AddWithValue("@ChangraiAmount", PurChangraiAmount);
            cmd.Parameters.AddWithValue("@DamiAmount", PurDamiAmount);
            cmd.Parameters.AddWithValue("@WHTaxAmount", PurWhtAmount);
            cmd.Parameters.AddWithValue("@KantaExpenseP", PurKantaExpense);
            cmd.Parameters.AddWithValue("@Stitching", PurStitching);
            cmd.Parameters.AddWithValue("@RentGodown", PurRentGodown);
            cmd.Parameters.AddWithValue("@ChokiDara", PurChokidara);
            cmd.Parameters.AddWithValue("@BagUsage", PurBagUsage);
            cmd.Parameters.AddWithValue("@ShortageG", PurShortage);
            cmd.Parameters.AddWithValue("@TractorExp", PurTractorExp);
            cmd.Parameters.AddWithValue("@Sotli", PurSotli);

            // Sale params
            cmd.Parameters.AddWithValue("@ComisionAmountS", SaleCommissionAmount);
            cmd.Parameters.AddWithValue("@MazdooriAmountS", SaleMazdooriAmount);
            cmd.Parameters.AddWithValue("@MazdooriTypeS", SaleMazdooriType);
            cmd.Parameters.AddWithValue("@MarketAmountS", SaleMarketFeesAmount);
            cmd.Parameters.AddWithValue("@SootaliAmountS", SaleSootaliAmount);
            cmd.Parameters.AddWithValue("@DalaliAmountS", SaleDalaliAmount);
            cmd.Parameters.AddWithValue("@MunshayanaAmountS", SaleMunshianaAmount);
            cmd.Parameters.AddWithValue("@WHTaxAmountS", SaleWhtAmount);
            cmd.Parameters.AddWithValue("@LoadingAmountS", SaleLoadingAmount);
            cmd.Parameters.AddWithValue("@UnLoadingAmountS", SaleUnloadingAmount);
            cmd.Parameters.AddWithValue("@DamiAmountS", SaleDamiAmount);
            cmd.Parameters.AddWithValue("@KantaExpenseS", SaleKantaExpense);

            await cmd.ExecuteNonQueryAsync();

            MessageBox.Show($"اخراجات کی اسکیم '{SchemeName}' کامیابی سے محفوظ ہو گئی!", 
                "کامیابی / Success", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadSchemesAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Save Expense Scheme");
        }
    }

    [RelayCommand]
    public async Task DeleteSchemeAsync()
    {
        if (SelectedScheme == null) return;

        var result = MessageBox.Show($"کیا آپ واقعی اسکیم '{SelectedScheme.ExpanseType}' حذف کرنا چاہتے ہیں؟", 
            "تصدیق / Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = "DELETE FROM ExpenseType WHERE ExpanseType = @Name";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", SelectedScheme.ExpanseType);
            await cmd.ExecuteNonQueryAsync();

            MessageBox.Show("اسکیم کامیابی سے ڈیلیٹ ہو گئی!", "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadSchemesAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Delete Expense Scheme");
        }
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
