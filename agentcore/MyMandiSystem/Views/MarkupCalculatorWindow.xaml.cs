using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class MarkupCalculatorWindow : Window
{
    public MarkupCalculatorWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is MarkupCalculatorViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
