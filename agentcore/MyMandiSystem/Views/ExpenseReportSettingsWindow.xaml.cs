using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class ExpenseReportSettingsWindow : Window
{
    public ExpenseReportSettingsWindow(ExpenseReportSettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.RequestClose = Close;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not ExpenseReportSettingsViewModel vm) return;

        // Escape: Close Window
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
            return;
        }

        // F2: Save Settings
        if (e.Key == Key.F2)
        {
            if (vm.SaveCommand.CanExecute(null))
            {
                vm.SaveCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // F3: Reset / Reload
        if (e.Key == Key.F3)
        {
            if (vm.LoadDataCommand.CanExecute(null))
            {
                vm.LoadDataCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // F5: Auto Configure
        if (e.Key == Key.F5)
        {
            if (vm.AutoConfigureCommand.CanExecute(null))
            {
                vm.AutoConfigureCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }
    }
}
