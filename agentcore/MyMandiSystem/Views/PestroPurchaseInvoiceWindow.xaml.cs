using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class PestroPurchaseInvoiceWindow : Window
{
    public PestroPurchaseInvoiceWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is PestroPurchaseInvoiceViewModel vm)
            {
                vm.RequestClose = () => Close();
            }
        };
    }
}
