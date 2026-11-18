using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class ChequeDepositView : UserControl
{
    public ChequeDepositView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChequeDepositViewModel oldVm)
            oldVm.RequestFocusControl -= OnRequestFocusControl;
        if (e.NewValue is ChequeDepositViewModel newVm)
            newVm.RequestFocusControl += OnRequestFocusControl;
    }

    private void OnRequestFocusControl(string controlName)
    {
        Dispatcher.BeginInvoke(() =>
        {
            switch (controlName)
            {
                case "txtVoucherno": TxtVoucherno?.Focus(); break;
                case "txtBankAccountno": TxtBankAccountno?.Focus(); break;
                case "txtReceivedBy": TxtReceivedBy?.Focus(); break;
                case "txtAmount":
                    TxtAmount?.Focus();
                    TxtAmount?.SelectAll();
                    break;
            }
        });
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        TxtBankAccountno?.Focus();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChequeDepositViewModel vm)
            vm.StopClock();
    }

    private async void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not ChequeDepositViewModel vm) return;

        bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

        if (e.Key == Key.F1)
        {
            e.Handled = true;
            if (Keyboard.FocusedElement == TxtBankAccountno)
                await vm.OpenBankLookupAsync();
            else
                await vm.OpenPartyLookupAsync();
            _ = Dispatcher.BeginInvoke(() => TxtLookupSearch?.Focus());
        }
        else if (e.Key == Key.F2 || (ctrl && e.Key == Key.S))
        {
            e.Handled = true;
            await vm.SaveAsync();
        }
        else if (e.Key == Key.F3 || (ctrl && e.Key == Key.L))
        {
            e.Handled = true;
            await vm.NewVoucherAsync();
        }
        else if (ctrl && e.Key == Key.O)
        {
            e.Handled = true;
            await vm.OpenSearchModalAsync();
        }
        else if (ctrl && e.Key == Key.P)
        {
            e.Handled = true;
            await vm.PrintAsync();
        }
        else if (ctrl && e.Key == Key.R)
        {
            e.Handled = true;
            await vm.DeleteVoucherAsync();
        }
        else if (e.Key == Key.F4 || e.Key == Key.Escape)
        {
            e.Handled = true;
            if (vm.IsAccountLookupOpen)
                vm.CloseLookup();
            else if (vm.IsSearchListOpen)
                vm.CloseSearchModal();
            else
                vm.Close();
        }
    }

    private void Input_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            if (e.OriginalSource is UIElement el)
                el.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }

    private async void TxtBankAccountno_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is ChequeDepositViewModel vm)
        {
            e.Handled = true;
            await vm.ValidateBankAccountAsync(openLookupIfNotFound: true);
            if (vm.IsAccountLookupOpen)
                _ = Dispatcher.BeginInvoke(() => TxtLookupSearch?.Focus());
            else
                TxtReceivedBy?.Focus();
        }
    }

    private async void TxtBankAccountno_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ChequeDepositViewModel vm) return;
        if (vm.IsAccountLookupOpen || Keyboard.FocusedElement == BtnBankLookup) return;
        await vm.ValidateBankAccountAsync(openLookupIfNotFound: false);
    }

    private async void TxtReceivedBy_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is ChequeDepositViewModel vm)
        {
            e.Handled = true;
            await vm.ValidatePartyAccountAsync(openLookupIfNotFound: true);
            if (vm.IsAccountLookupOpen)
                _ = Dispatcher.BeginInvoke(() => TxtLookupSearch?.Focus());
            else
            {
                TxtAmount?.Focus();
                TxtAmount?.SelectAll();
            }
        }
    }

    private async void TxtReceivedBy_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ChequeDepositViewModel vm) return;
        if (vm.IsAccountLookupOpen || Keyboard.FocusedElement == BtnPartyLookup) return;
        await vm.ValidatePartyAccountAsync(openLookupIfNotFound: false);
    }

    private async void BtnBankLookup_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChequeDepositViewModel vm)
        {
            await vm.OpenBankLookupAsync();
            _ = Dispatcher.BeginInvoke(() => TxtLookupSearch?.Focus());
        }
    }

    private async void BtnPartyLookup_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChequeDepositViewModel vm)
        {
            await vm.OpenPartyLookupAsync();
            _ = Dispatcher.BeginInvoke(() => TxtLookupSearch?.Focus());
        }
    }

    private void TxtSearchVoucherNo_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is ChequeDepositViewModel vm)
        {
            e.Handled = true;
            _ = vm.SearchVoucherAsync();
        }
    }

    private async void TxtLookupSearch_KeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not ChequeDepositViewModel vm) return;
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            if (vm.SelectedLookupAccount != null)
                await vm.SelectLookupAccountAsync(vm.SelectedLookupAccount);
            else if (vm.FilteredAccounts.Count > 0)
                await vm.SelectLookupAccountAsync(vm.FilteredAccounts[0]);
        }
        else if (e.Key == Key.Down)
        {
            LookupDataGrid?.Focus();
        }
    }

    private async void LookupDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem is ChartAccountLookupDto account && DataContext is ChequeDepositViewModel vm)
        {
            await vm.SelectLookupAccountAsync(account);
        }
    }

    private async void LookupDataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is DataGrid grid && grid.SelectedItem is ChartAccountLookupDto account && DataContext is ChequeDepositViewModel vm)
        {
            e.Handled = true;
            await vm.SelectLookupAccountAsync(account);
        }
    }

    private async void BtnLookupSelect_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChequeDepositViewModel vm && vm.SelectedLookupAccount != null)
            await vm.SelectLookupAccountAsync(vm.SelectedLookupAccount);
    }

    private async void TodayDepositsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem is ChequeDepositDto dto && DataContext is ChequeDepositViewModel vm)
        {
            await vm.LoadVoucherAsync(dto.VoucherNo, dto.VoucherDate);
        }
    }

    private async void SearchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem is ChequeDepositDto dto && DataContext is ChequeDepositViewModel vm)
        {
            await vm.SelectSearchItemAsync(dto);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChequeDepositViewModel vm)
            vm.Close();
        else
            Window.GetWindow(this)?.Close();
    }
}
