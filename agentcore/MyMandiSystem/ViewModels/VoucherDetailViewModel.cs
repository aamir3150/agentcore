using CommunityToolkit.Mvvm.ComponentModel;

namespace MyMandiSystem.ViewModels;

public class VoucherDetailViewModel : ObservableObject
{
    public int AccountId { get; set; }
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string? Narration { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
