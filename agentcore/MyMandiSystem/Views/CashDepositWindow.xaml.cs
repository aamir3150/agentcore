using MyMandiSystem.Core.Entities;
using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class CashDepositWindow : Window
{
    public CashDepositWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is CashDepositViewModel vm)
        {
            vm.RequestClose = Close;
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not CashDepositViewModel vm) return;

        // Escape handling
        if (e.Key == Key.Escape)
        {
            if (vm.IsAccountLookupOpen)
            {
                vm.IsAccountLookupOpen = false;
                e.Handled = true;
                return;
            }
            if (vm.IsSearchListOpen)
            {
                vm.IsSearchListOpen = false;
                e.Handled = true;
                return;
            }
            Close();
            e.Handled = true;
            return;
        }

        // F1: Bank Lookup
        if (e.Key == Key.F1)
        {
            vm.OpenBankLookup();
            e.Handled = true;
            return;
        }

        // F2: Save
        if (e.Key == Key.F2)
        {
            if (vm.SaveCommand.CanExecute(null))
            {
                vm.SaveCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // F3: Reset / New
        if (e.Key == Key.F3)
        {
            if (vm.NewVoucherCommand.CanExecute(null))
            {
                vm.NewVoucherCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // F5: Search Bank Cash Deposits
        if (e.Key == Key.F5)
        {
            if (vm.OpenSearchModalCommand.CanExecute(null))
            {
                vm.OpenSearchModalCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // Ctrl + P: Print
        if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            if (vm.PrintCommand.CanExecute(null))
            {
                vm.PrintCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }
    }

    private void TxtBankAcNo_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is CashDepositViewModel vm)
        {
            vm.OnBankAccountNoEntered();
            e.Handled = true;
        }
    }

    private void LookupDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is CashDepositViewModel vm && vm.SelectedLookupAccount != null)
        {
            vm.SelectLookupAccount(vm.SelectedLookupAccount);
        }
    }

    private async void SearchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is CashDepositViewModel vm && vm.SelectedSearchItem != null)
        {
            await vm.SelectSearchItemAsync(vm.SelectedSearchItem);
        }
    }
}

