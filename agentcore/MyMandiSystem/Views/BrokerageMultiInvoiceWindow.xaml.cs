using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class BrokerageMultiInvoiceWindow : Window
{
    public BrokerageMultiInvoiceWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BrokerageMultiInvoiceViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };

        KeyDown += (s, e) =>
        {
            if (DataContext is not BrokerageMultiInvoiceViewModel vm) return;
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.F2 || (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
            {
                _ = vm.SaveCommand.ExecuteAsync(null);
            }
            else if (e.Key == Key.F3)
            {
                vm.ClearCommand.Execute(null);
            }
            else if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                vm.PrintCommand.Execute(null);
            }
            else if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                _ = vm.OpenCommand.ExecuteAsync(null);
            }
        };
    }
}
