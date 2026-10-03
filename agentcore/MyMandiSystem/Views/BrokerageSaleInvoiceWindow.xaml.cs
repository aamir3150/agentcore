using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BrokerageSaleInvoiceWindow : Window
{
    public BrokerageSaleInvoiceWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BrokerageSaleInvoiceViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
