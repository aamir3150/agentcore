using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class AccountPayablesWindow : Window
{
    public AccountPayablesWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is AccountPayablesViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
