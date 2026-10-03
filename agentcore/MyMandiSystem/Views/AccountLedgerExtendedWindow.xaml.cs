using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class AccountLedgerExtendedWindow : Window
{
    public AccountLedgerExtendedWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is AccountLedgerExtendedViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
