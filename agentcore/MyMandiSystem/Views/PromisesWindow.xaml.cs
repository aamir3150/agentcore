using System.Windows;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class PromisesWindow : Window
{
    public PromisesWindow()
    {
        InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (DataContext is PromisesViewModel vm)
            {
                vm.RequestClose ??= () => this.Close();
            }
        };
    }
}
