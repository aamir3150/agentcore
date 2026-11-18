using System.Windows;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class InsuranceWindow : Window
{
    public InsuranceWindow()
    {
        InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (DataContext is InsuranceViewModel vm)
            {
                vm.RequestClose ??= () => this.Close();
            }
        };
    }
}
