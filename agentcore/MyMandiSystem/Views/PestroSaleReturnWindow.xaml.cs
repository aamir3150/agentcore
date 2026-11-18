using System.Windows;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class PestroSaleReturnWindow : Window
{
    public PestroSaleReturnWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is PestroSaleReturnViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
