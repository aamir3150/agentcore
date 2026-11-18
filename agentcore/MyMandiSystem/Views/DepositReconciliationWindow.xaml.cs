using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class DepositReconciliationWindow : Window
{
    public DepositReconciliationWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DepositReconciliationViewModel vm)
        {
            vm.RequestClose += Close;
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not DepositReconciliationViewModel vm) return;

        // Escape: Close Window
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
            return;
        }

        // F2: Save Reconcile
        if (e.Key == Key.F2)
        {
            if (vm.SaveCommand.CanExecute(null))
            {
                vm.SaveCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // F3: Clear
        if (e.Key == Key.F3)
        {
            if (vm.ClearCommand.CanExecute(null))
            {
                vm.ClearCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // F5: Load / Refresh
        if (e.Key == Key.F5)
        {
            if (vm.LoadDepositsCommand.CanExecute(null))
            {
                vm.LoadDepositsCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }
    }
}

