using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MyMandiSystem.Views;

public partial class SaleContractWindow : Window
{
    public SaleContractWindow()
    {
        InitializeComponent();
    }

    public SaleContractWindow(SaleContractViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void CustomerId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SaleContractViewModel vm && sender is TextBox tb)
        {
            await vm.OnPartyIdChangedAsync(tb.Text);
        }
    }

    private async void BrokerId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SaleContractViewModel vm && sender is TextBox tb)
        {
            await vm.OnBrokerIdChangedAsync(tb.Text);
        }
    }

    private async void ProductId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SaleContractViewModel vm && sender is TextBox tb)
        {
            await vm.OnProductIdChangedAsync(tb.Text);
        }
    }

    private void LineCalc_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SaleContractViewModel vm)
        {
            vm.CalculateCurrentLine();
        }
    }

    private void RecalcTotal_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SaleContractViewModel vm)
        {
            vm.RecalculateTotals();
        }
    }
}
