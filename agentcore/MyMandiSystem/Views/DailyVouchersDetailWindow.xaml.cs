using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class DailyVouchersDetailWindow : Window
{
    public DailyVouchersDetailWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is DailyVouchersDetailViewModel vm)
            {
                vm.RequestClose = () => this.Close();
            }
        };
    }
}
