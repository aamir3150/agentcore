using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class MonthlyExpenseChartWindow : Window
{
    public MonthlyExpenseChartWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is MonthlyExpenseChartViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
