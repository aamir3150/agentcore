using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class CashBookWindow : Window
{
    public CashBookWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is CashBookViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
