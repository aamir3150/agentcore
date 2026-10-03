using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BrokeragePurchaseInvoiceWindow : Window
{
    public BrokeragePurchaseInvoiceWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BrokeragePurchaseInvoiceViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
