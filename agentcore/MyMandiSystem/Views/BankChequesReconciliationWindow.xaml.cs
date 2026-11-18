using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class BankChequesReconciliationWindow : Window
{
    public BankChequesReconciliationWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        KeyDown += BankChequesReconciliationWindow_KeyDown;
    }

    public BankChequesReconciliationWindow(BankChequesReconciliationViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is BankChequesReconciliationViewModel vm)
        {
            vm.RequestClose += () => Close();
        }
    }

    private void BankChequesReconciliationWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not BankChequesReconciliationViewModel vm) return;

        switch (e.Key)
        {
            case Key.F2:
                if (vm.SaveReconciliationCommand.CanExecute(null))
                {
                    vm.SaveReconciliationCommand.Execute(null);
                    e.Handled = true;
                }
                break;
            case Key.F3:
                if (vm.ClearCommand.CanExecute(null))
                {
                    vm.ClearCommand.Execute(null);
                    e.Handled = true;
                }
                break;
            case Key.F5:
                if (vm.LoadChequesCommand.CanExecute(null))
                {
                    vm.LoadChequesCommand.Execute(null);
                    e.Handled = true;
                }
                break;
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
        }
    }
}

