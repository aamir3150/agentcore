using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class PestroSaleAutoBatchWindow : Window
{
    public PestroSaleAutoBatchWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is PestroSaleAutoBatchViewModel vm)
            {
                vm.RequestClose = () => Close();
            }
        };
    }
}
