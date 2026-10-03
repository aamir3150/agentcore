using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BankStatementWindow : Window
{
    public BankStatementWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is BankStatementViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
