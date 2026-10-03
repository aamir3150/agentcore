using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BalanceSheetWindow : Window
{
    public BalanceSheetWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BalanceSheetViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
