using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BrokerageSaleInvoiceWindow : Window
{
    public BrokerageSaleInvoiceWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BrokerageSaleInvoiceViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };

        KeyDown += (s, e) =>
        {
            if (DataContext is not BrokerageSaleInvoiceViewModel vm) return;
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            else if (e.Key == System.Windows.Input.Key.F2 || (e.Key == System.Windows.Input.Key.S && (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control))
            {
                _ = vm.SaveCommand.ExecuteAsync(null);
            }
            else if (e.Key == System.Windows.Input.Key.F3)
            {
                vm.ClearCommand.Execute(null);
            }
            else if (e.Key == System.Windows.Input.Key.P && (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                vm.PrintCommand.Execute(null);
            }
            else if (e.Key == System.Windows.Input.Key.O && (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                _ = vm.OpenCommand.ExecuteAsync(null);
            }
        };
    }
}
