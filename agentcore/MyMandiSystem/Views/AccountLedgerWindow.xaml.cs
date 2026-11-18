using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class AccountLedgerWindow : Window
{
    public AccountLedgerWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is AccountLedgerViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };

        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                if (DataContext is AccountLedgerViewModel vm && vm.IsAccountLookupOpen)
                {
                    vm.IsAccountLookupOpen = false;
                }
                else
                {
                    Close();
                }
            }
            else if (e.Key == Key.F1)
            {
                if (DataContext is AccountLedgerViewModel vm)
                {
                    vm.OpenAccountLookupCommand.Execute(null);
                }
            }
            else if (e.Key == Key.F3)
            {
                if (DataContext is AccountLedgerViewModel vm)
                {
                    vm.ResetCommand.Execute(null);
                }
            }
            else if (e.Key == Key.F4)
            {
                if (DataContext is AccountLedgerViewModel vm)
                {
                    _ = vm.PreviewCommand.ExecuteAsync(null);
                }
            }
            else if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (DataContext is AccountLedgerViewModel vm)
                {
                    _ = vm.PrintCommand.ExecuteAsync(null);
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (DataContext is AccountLedgerViewModel vm && !vm.IsAccountLookupOpen)
                {
                    _ = vm.PreviewCommand.ExecuteAsync(null);
                }
            }
        };
    }

    private void AccountsLookupGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AccountLedgerViewModel vm && vm.SelectedAccount != null)
        {
            vm.SelectAccountFromLookupCommand.Execute(vm.SelectedAccount);
        }
    }
}

