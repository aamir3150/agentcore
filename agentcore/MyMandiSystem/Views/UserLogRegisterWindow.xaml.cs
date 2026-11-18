using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class UserLogRegisterWindow : Window
{
    public UserLogRegisterWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is UserLogRegisterViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
