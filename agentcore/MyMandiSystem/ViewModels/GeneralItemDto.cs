using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MyMandiSystem.ViewModels;

public class GeneralGridItemDto : ObservableObject
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

    private string _unitName = "PIECE";
    public string UnitName
    {
        get => _unitName;
        set => SetProperty(ref _unitName, value);
    }

    private decimal _multiplier = 1;
    public decimal Multiplier
    {
        get => _multiplier;
        set
        {
            if (SetProperty(ref _multiplier, value))
                Recalculate();
        }
    }

    private decimal _availableStock = 0;
    public decimal AvailableStock
    {
        get => _availableStock;
        set => SetProperty(ref _availableStock, value);
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

    private decimal _grossAmount = 0;
    public decimal GrossAmount
    {
        get => _grossAmount;
        set => SetProperty(ref _grossAmount, value);
    }

    private decimal _discRatio = 0;
    public decimal DiscRatio
    {
        get => _discRatio;
        set
        {
            if (SetProperty(ref _discRatio, value))
                Recalculate();
        }
    }

    private decimal _discValue = 0;
    public decimal DiscValue
    {
        get => _discValue;
        set => SetProperty(ref _discValue, value);
    }

    private decimal _ttlValue = 0;
    public decimal TTLValue
    {
        get => _ttlValue;
        set => SetProperty(ref _ttlValue, value);
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

    private decimal _totalValue = 0;
    public decimal TotalValue
    {
        get => _totalValue;
        set => SetProperty(ref _totalValue, value);
    }

    private decimal _cost = 0;
    public decimal Cost
    {
        get => _cost;
        set => SetProperty(ref _cost, value);
    }

    private string? _narration;
    public string? Narration
    {
        get => _narration;
        set => SetProperty(ref _narration, value);
    }

    public void Recalculate()
    {
        decimal mult = Multiplier > 0 ? Multiplier : 1m;
        GrossAmount = Math.Round((Quantity * mult) * Price, 2);
        DiscValue = Math.Round(GrossAmount * (DiscRatio / 100m), 2);
        TTLValue = GrossAmount - DiscValue;
        STValue = Math.Round(TTLValue * (STRatio / 100m), 2);
        TotalValue = TTLValue + STValue;
        decimal totalEffectiveQty = (Quantity + Bonus) * mult;
        Cost = totalEffectiveQty > 0 ? Math.Round(TotalValue / totalEffectiveQty, 4) : 0;
    }
}
