using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyMandiSystem.Core.Entities;

public class AccountReportSettingDto : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string? AccountNameUrdu { get; set; }
    public string AccountType { get; set; } = string.Empty;
    
    private string? _pLFlag;
    public string? PLFlag
    {
        get => _pLFlag;
        set { _pLFlag = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsPLIncluded)); OnPropertyChanged(nameof(IsPLBoth)); OnPropertyChanged(nameof(PLSection)); }
    }
    
    private bool _balFlag = true;
    public bool BalFlag
    {
        get => _balFlag;
        set { _balFlag = value; OnPropertyChanged(); }
    }
    
    private bool _expFlag;
    public bool ExpFlag
    {
        get => _expFlag;
        set { _expFlag = value; OnPropertyChanged(); }
    }
    
    private decimal? _expenseLimit;
    public decimal? ExpenseLimit
    {
        get => _expenseLimit;
        set { _expenseLimit = value; OnPropertyChanged(); }
    }

    public bool IsPLIncluded
    {
        get => !string.IsNullOrEmpty(PLFlag) && (PLFlag.Trim().ToUpper() == "P" || PLFlag.Trim().ToUpper() == "B");
        set
        {
            if (value)
            {
                if (string.IsNullOrEmpty(PLFlag)) PLFlag = "P";
            }
            else
            {
                PLFlag = "";
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPLBoth));
            OnPropertyChanged(nameof(PLSection));
        }
    }

    public bool IsPLBoth
    {
        get => !string.IsNullOrEmpty(PLFlag) && PLFlag.Trim().ToUpper() == "B";
        set
        {
            if (value) PLFlag = "B";
            else if (PLFlag?.Trim().ToUpper() == "B") PLFlag = "P";
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPLIncluded));
            OnPropertyChanged(nameof(PLSection));
        }
    }

    public string PLSection
    {
        get
        {
            if (string.IsNullOrEmpty(PLFlag)) return "Excluded";
            var typeLow = (AccountType ?? "").ToLower();
            if (typeLow.Contains("rev") || typeLow.Contains("inc")) return "Revenue / Sales";
            if (typeLow.Contains("pur") || typeLow.Contains("cost")) return "Cost of Goods Sold";
            if (typeLow.Contains("exp")) return "Operating Expenses";
            if (PLFlag.Trim().ToUpper() == "B") return "Both (P&L & BS)";
            return "General P&L Item";
        }
    }

    public string BLClassification
    {
        get
        {
            var typeLow = (AccountType ?? "").ToLower();
            if (typeLow.Contains("asset")) return "Assets";
            if (typeLow.Contains("liab")) return "Liabilities";
            if (typeLow.Contains("cap") || typeLow.Contains("equi")) return "Capital & Equity";
            return "Other / Nominal";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
