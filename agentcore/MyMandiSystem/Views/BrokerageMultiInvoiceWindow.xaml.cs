using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BrokerageMultiInvoiceWindow : Window
{
    public BrokerageMultiInvoiceWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BrokerageMultiInvoiceViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
