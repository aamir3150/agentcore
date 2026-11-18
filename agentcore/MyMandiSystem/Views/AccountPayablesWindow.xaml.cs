using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class AccountPayablesWindow : Window
{
    public AccountPayablesWindow()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            if (DataContext is AccountPayablesViewModel vm)
            {
                vm.RequestClose = Close;
                vm.FocusFromDate = () => DtpFrom.Focus();
            }
        };
    }

    private void TxtGreaterThen_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            tb.SelectAll();
        }
    }

    /// <summary>
    /// Mirrors AgenSoft RptAccountPayables Form_KeyDown: Enter advances focus (Tab) on inputs; Ctrl+Q closes window.
    /// </summary>
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Q && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            Close();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            if (Keyboard.FocusedElement is DependencyObject focused)
            {
                if (FindAncestor<Button>(focused) != null)
                {
                    return;
                }

                var combo = FindAncestor<ComboBox>(focused);
                if (combo != null && combo.IsDropDownOpen)
                {
                    return;
                }

                if (focused is UIElement element)
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }
    }

    private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T match)
            {
                return match;
            }
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }
}
