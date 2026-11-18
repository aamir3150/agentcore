using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MyMandiSystem.Views;

public partial class BrokerageShortageWindow : Window
{
    public BrokerageShortageWindow()
    {
        InitializeComponent();
    }

    public BrokerageShortageWindow(BrokerageShortageViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void PurchaseId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is BrokerageShortageViewModel vm && sender is TextBox tb)
        {
            if (int.TryParse(tb.Text, out var id))
            {
                await vm.OnPurchaseIdChangedAsync(id);
            }
        }
    }

    private async void SaleId_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is BrokerageShortageViewModel vm && sender is TextBox tb)
        {
            if (int.TryParse(tb.Text, out var id))
            {
                await vm.OnSaleIdChangedAsync(id);
            }
        }
    }

    private void Shortage_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is BrokerageShortageViewModel vm)
        {
            vm.CalculateShortage();
        }
    }
}
