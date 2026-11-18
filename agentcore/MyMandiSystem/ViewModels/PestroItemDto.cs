using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MyMandiSystem.ViewModels;

public class PestroGridItemDto : ObservableObject
{
    private int _serialNo = 1;
    public int SerialNo
    {
        get => _serialNo;
        set => SetProperty(ref _serialNo, value);
    }

    private int _productId;
    public int ProductId
    {
        get => _productId;
        set => SetProperty(ref _productId, value);
    }

    private string _productCode = string.Empty;
    public string ProductCode
    {
        get => _productCode;
        set => SetProperty(ref _productCode, value);
    }

    private string _productName = string.Empty;
    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value);
    }

    private string _packing = "PIECE";
    public string Packing
    {
        get => _packing;
        set => SetProperty(ref _packing, value);
    }

    private string _batchNo = string.Empty;
    public string BatchNo
    {
        get => _batchNo;
        set => SetProperty(ref _batchNo, value);
    }

    private DateTime? _expiryDate = DateTime.Today.AddYears(2);
    public DateTime? ExpiryDate
    {
        get => _expiryDate;
        set => SetProperty(ref _expiryDate, value);
    }

    private decimal _quantity = 1;
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
                Recalculate();
        }
    }

    private decimal _bonus = 0;
    public decimal Bonus
    {
        get => _bonus;
        set
        {
            if (SetProperty(ref _bonus, value))
                Recalculate();
        }
    }

    private decimal _price = 0;
    public decimal Price
    {
        get => _price;
        set
        {
            if (SetProperty(ref _price, value))
                Recalculate();
        }
    }

    private decimal _discRatio1 = 0;
    public decimal DiscRatio1
    {
        get => _discRatio1;
        set
        {
            if (SetProperty(ref _discRatio1, value))
                Recalculate();
        }
    }

    private decimal _discRatio2 = 0;
    public decimal DiscRatio2
    {
        get => _discRatio2;
        set
        {
            if (SetProperty(ref _discRatio2, value))
                Recalculate();
        }
    }

    private decimal _stRatio = 0;
    public decimal STRatio
    {
        get => _stRatio;
        set
        {
            if (SetProperty(ref _stRatio, value))
                Recalculate();
        }
    }

    private decimal _stValue = 0;
    public decimal STValue
    {
        get => _stValue;
        set => SetProperty(ref _stValue, value);
    }

    private decimal _ttlValue = 0;
    public decimal TTLValue
    {
        get => _ttlValue;
        set => SetProperty(ref _ttlValue, value);
    }

    private decimal _ttlSalestax = 0;
    public decimal TTLSalestax
    {
        get => _ttlSalestax;
        set => SetProperty(ref _ttlSalestax, value);
    }

    private decimal _totalValue = 0;
    public decimal TotalValue
    {
        get => _totalValue;
        set => SetProperty(ref _totalValue, value);
    }

    private decimal _totalUnits = 1;
    public decimal TotalUnits
    {
        get => _totalUnits;
        set => SetProperty(ref _totalUnits, value);
    }

    private decimal _cost = 0;
    public decimal Cost
    {
        get => _cost;
        set => SetProperty(ref _cost, value);
    }

    public void Recalculate()
    {
        TotalUnits = Quantity + Bonus;
        decimal gross = Quantity * Price;
        decimal d1 = gross * (DiscRatio1 / 100m);
        decimal afterD1 = gross - d1;
        decimal d2 = afterD1 * (DiscRatio2 / 100m);
        TTLValue = Math.Round(afterD1 - d2, 2);
        TTLSalestax = Math.Round(TTLValue * (STRatio / 100m), 2);
        STValue = TTLSalestax;
        TotalValue = TTLValue + TTLSalestax;
        Cost = TotalUnits > 0 ? Math.Round(TTLValue / TotalUnits, 4) : Price;
    }
}
