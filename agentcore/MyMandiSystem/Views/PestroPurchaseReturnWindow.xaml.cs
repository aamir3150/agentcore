using System.Windows;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class PestroPurchaseReturnWindow : Window
{
    public PestroPurchaseReturnWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is PestroPurchaseReturnViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
