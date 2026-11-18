using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class BankChequeEditingWindow : Window
{
    public BankChequeEditingWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        KeyDown += BankChequeEditingWindow_KeyDown;
    }

    public BankChequeEditingWindow(BankChequeEditingViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is BankChequeEditingViewModel vm)
        {
            vm.RequestClose += () => Close();
        }
    }

    private void BankChequeEditingWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not BankChequeEditingViewModel vm) return;

        switch (e.Key)
        {
            case Key.F2:
                if (vm.ApplyActionsCommand.CanExecute(null))
                {
                    vm.ApplyActionsCommand.Execute(null);
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
