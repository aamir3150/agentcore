using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class TrialBalanceWindow : Window
{
    public TrialBalanceWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is TrialBalanceViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
