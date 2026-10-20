using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class CashPaymentVoucherView : System.Windows.Controls.UserControl
{
    public CashPaymentVoucherView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            AccountNoBox?.Focus();
            AccountNoBox?.SelectAll();
        }));
    }

    private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not CashPaymentVoucherViewModel vm) return;

        // F1: Search Account
        if (e.Key == Key.F1)
        {
            e.Handled = true;
            vm.OpenAccountLookupCommand.Execute(null);
            return;
        }

        // F2 or Ctrl+S: Save Voucher
        if (e.Key == Key.F2 || (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
        {
            e.Handled = true;
            vm.SaveCommand.Execute(null);
            return;
        }

        // F3 or Ctrl+N: Reset / New
        if (e.Key == Key.F3 || (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
        {
            e.Handled = true;
            vm.ClearCommand.Execute(null);
            AccountNoBox?.Focus();
            return;
        }

        // Ctrl+P: Print Voucher
        if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            e.Handled = true;
            vm.PrintCommand.Execute(null);
            return;
        }

        // Escape: Close modal if open, else close window
        if (e.Key == Key.Escape)
        {
            if (vm.IsAccountLookupOpen)
            {
                e.Handled = true;
                vm.CloseAccountLookupCommand.Execute(null);
                AccountNoBox?.Focus();
                return;
            }
            
            var win = Window.GetWindow(this);
            if (win != null)
            {
                e.Handled = true;
                win.Close();
            }
        }
    }

    private void AccountNoBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            if (DataContext is CashPaymentVoucherViewModel vm)
            {
                if (string.IsNullOrWhiteSpace(AccountNoBox.Text))
                {
                    vm.OpenAccountLookupCommand.Execute(null);
                }
                else
                {
                    vm.LookupAccountCommand.Execute(null);
                    NarrationBox?.Focus();
                    NarrationBox?.SelectAll();
                }
            }
        }
    }

    private void AccountTitleBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.F1)
        {
            e.Handled = true;
            if (DataContext is CashPaymentVoucherViewModel vm)
            {
                vm.OpenAccountLookupCommand.Execute(AccountTitleBox.Text);
            }
        }
    }

    private void NarrationBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            AmountBox?.Focus();
            AmountBox?.SelectAll();
        }
    }

    private async void AmountBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            if (DataContext is CashPaymentVoucherViewModel vm)
            {
                await vm.AddDetailCommand.ExecuteAsync(null);
                AccountNoBox?.Focus();
                AccountNoBox?.SelectAll();
            }
        }
    }

    private void AccountsLookupGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is CashPaymentVoucherViewModel vm && vm.SelectedAccount != null)
        {
            vm.SelectAccountFromLookupCommand.Execute(vm.SelectedAccount);
            NarrationBox?.Focus();
            NarrationBox?.SelectAll();
        }
    }

    private void AccountLookupSearchBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is TextBox tb && tb.IsVisible)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                tb.Focus();
                tb.SelectAll();
            }));
        }
    }

    private void AccountLookupSearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is CashPaymentVoucherViewModel vm && vm.SelectedAccount != null)
            {
                e.Handled = true;
                vm.SelectAccountFromLookupCommand.Execute(vm.SelectedAccount);
                NarrationBox?.Focus();
                NarrationBox?.SelectAll();
                return;
            }
        }

        if (e.Key == Key.Down && AccountsLookupGrid != null)
        {
            AccountsLookupGrid.Focus();
            if (AccountsLookupGrid.SelectedIndex < 0 && AccountsLookupGrid.Items.Count > 0)
            {
                AccountsLookupGrid.SelectedIndex = 0;
            }
            e.Handled = true;
        }
    }
}
