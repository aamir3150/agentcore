namespace MyMandiSystem.Core.Entities;

public class AccountOpeningBalanceDto
{
    public int AccountId { get; set; }
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal OpeningDebit { get; set; }
    public decimal OpeningCredit { get; set; }
    public string? CropSeason { get; set; }
    public string? CropName { get; set; }
}

public class CropSeasonItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}

