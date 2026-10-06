using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

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

    private void AccountsLookupGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is BankStatementViewModel vm && vm.SelectedAccount != null)
        {
            vm.SelectAccountFromLookupCommand.Execute(vm.SelectedAccount);
        }
    }
}
