using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MyMandiSystem.Views;

public partial class PurchaseContractWindow : Window
{
    public PurchaseContractWindow()
    {
        InitializeComponent();
    }

    public PurchaseContractWindow(PurchaseContractViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void VendorId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is PurchaseContractViewModel vm && sender is TextBox tb)
        {
            await vm.OnPartyIdChangedAsync(tb.Text);
        }
    }

    private async void BrokerId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is PurchaseContractViewModel vm && sender is TextBox tb)
        {
            await vm.OnBrokerIdChangedAsync(tb.Text);
        }
    }

    private async void ProductId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is PurchaseContractViewModel vm && sender is TextBox tb)
        {
            await vm.OnProductIdChangedAsync(tb.Text);
        }
    }

    private void LineCalc_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is PurchaseContractViewModel vm)
        {
            vm.CalculateCurrentLine();
        }
    }

    private void RecalcTotal_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is PurchaseContractViewModel vm)
        {
            vm.RecalculateTotals();
        }
    }
}
