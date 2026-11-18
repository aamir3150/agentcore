using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class GeneralInvoiceLookupWindow : Window
{
    public GeneralInvoiceLookupWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is GeneralInvoiceLookupViewModel vm)
            {
                vm.RequestClose = () => Close();
            }
        };
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is GeneralInvoiceLookupViewModel vm && vm.SelectedInvoice != null)
        {
            vm.ConfirmCommand.Execute(null);
        }
    }
}
