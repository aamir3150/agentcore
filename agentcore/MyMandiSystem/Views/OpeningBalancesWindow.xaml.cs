using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class OpeningBalancesWindow : Window
{
    public OpeningBalancesWindow()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            if (DataContext is OpeningBalancesViewModel vm)
            {
                vm.RequestClose = () => Close();
            }
        };

        KeyDown += (s, e) =>
        {
            if (DataContext is OpeningBalancesViewModel vm)
            {
                if (e.Key == Key.F2)
                {
                    if (vm.SaveFinancialCommand.CanExecute(null))
                        vm.SaveFinancialCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.F3)
                {
                    if (vm.ClearCommand.CanExecute(null))
                        vm.ClearCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    Close();
                    e.Handled = true;
                }
            }
        };
    }
}
