using System.Windows;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class PestroOpeningStockWindow : Window
{
    public PestroOpeningStockWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is PestroOpeningStockViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
