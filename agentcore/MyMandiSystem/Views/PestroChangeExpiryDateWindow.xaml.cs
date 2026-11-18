using System.Windows;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class PestroChangeExpiryDateWindow : Window
{
    public PestroChangeExpiryDateWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is PestroChangeExpiryDateViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
