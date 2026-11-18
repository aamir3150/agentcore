using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class ChequeDepositWindow : Window
{
    public ChequeDepositWindow()
    {
        InitializeComponent();
        DataContextChanged += ChequeDepositWindow_DataContextChanged;
        Closed += (_, _) =>
        {
            if (DataContext is ChequeDepositViewModel vm)
                vm.StopClock();
        };
    }

    private void ChequeDepositWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ChequeDepositViewModel vm)
        {
            vm.RequestClose = () => Close();
        }
    }
}

