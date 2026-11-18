using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class BrokerageMultiInvoiceLookupWindow : Window
{
    public BrokerageMultiInvoiceLookupWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BrokerageMultiInvoiceLookupViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };

        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Enter)
            {
                if (DataContext is BrokerageMultiInvoiceLookupViewModel vm)
                {
                    vm.ConfirmCommand.Execute(null);
                }
            }
        };
    }

    private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is BrokerageMultiInvoiceLookupViewModel vm)
        {
            vm.ConfirmCommand.Execute(null);
        }
    }
}

