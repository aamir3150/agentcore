using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class ProfitLossStatementWindow : Window
{
    public ProfitLossStatementWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is ProfitLossStatementViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
