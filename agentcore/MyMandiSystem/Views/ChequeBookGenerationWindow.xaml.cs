using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class ChequeBookGenerationWindow : Window
{
    public ChequeBookGenerationWindow()
    {
        InitializeComponent();
        KeyDown += ChequeBookGenerationWindow_KeyDown;
    }

    private void ChequeBookGenerationWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not ChequeBookGenerationViewModel vm) return;

        if (e.Key == Key.F2)
        {
            if (vm.GenerateChequeBookCommand.CanExecute(null))
                vm.GenerateChequeBookCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == Key.F3)
        {
            if (vm.ClearFieldsCommand.CanExecute(null))
                vm.ClearFieldsCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

