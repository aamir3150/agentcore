using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class InsuranceRegisterWindow : Window
{
    public InsuranceRegisterWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is InsuranceRegisterViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
