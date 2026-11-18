using System.Windows;
using System.Windows.Input;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class BrokerageInvoiceLookupWindow : Window
{
    public BrokerageInvoiceLookupWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BrokerageInvoiceLookupViewModel vm)
            {
                vm.RequestClose = () => Close();
            }
        };
    }

    private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is BrokerageInvoiceLookupViewModel vm && vm.SelectedInvoice != null)
        {
            vm.SelectCommand.Execute(null);
        }
    }
}
