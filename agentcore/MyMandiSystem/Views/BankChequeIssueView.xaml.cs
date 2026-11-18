using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class BankChequeIssueView : UserControl
{
    public BankChequeIssueView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        PartyAccountNoBox?.Focus();
    }

    private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not BankChequeIssueViewModel vm) return;

        if (e.Key == Key.F1)
        {
            e.Handled = true;
            vm.OpenPartyLookup();
            Dispatcher.BeginInvoke(() => AccountLookupSearchBox?.Focus());
        }
        else if (e.Key == Key.F2)
        {
            e.Handled = true;
            _ = vm.SaveAsync();
        }
        else if (e.Key == Key.F3)
        {
            e.Handled = true;
            _ = vm.NewVoucherAsync();
        }
        else if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            e.Handled = true;
            vm.Print();
        }
        else if (e.Key == Key.Escape)
        {
            if (vm.IsAccountLookupOpen)
            {
                e.Handled = true;
                vm.CloseLookup();
            }
            else if (vm.IsSearchListOpen)
            {
                e.Handled = true;
                vm.CloseSearchModal();
            }
            else
            {
                e.Handled = true;
                var win = Window.GetWindow(this);
                win?.Close();
            }
        }
    }

    private void PartyAccountNoBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is BankChequeIssueViewModel vm)
        {
            e.Handled = true;
            vm.OnPartyAccountNoEntered();
        }
    }

    private void SplitAccountNoBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is BankChequeIssueViewModel vm)
        {
            e.Handled = true;
            vm.OnSplitAccountNoEntered();
        }
    }

    private void AccountLookupSearchBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            Dispatcher.BeginInvoke(() => AccountLookupSearchBox?.Focus());
        }
    }

    private void AccountLookupSearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is BankChequeIssueViewModel vm)
        {
            if (vm.SelectedLookupAccount != null)
            {
                vm.SelectLookupAccount(vm.SelectedLookupAccount);
            }
        }
    }

    private void AccountsLookupGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem is Account account && DataContext is BankChequeIssueViewModel vm)
        {
            vm.SelectLookupAccount(account);
        }
    }

    private void SearchChequesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem is BankChequeDto dto && DataContext is BankChequeIssueViewModel vm)
        {
            _ = vm.SelectSearchItemAsync(dto);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        var win = Window.GetWindow(this);
        win?.Close();
    }
}
