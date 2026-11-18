using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BankChequeIssueWindow : Window
{
    public BankChequeIssueWindow()
    {
        InitializeComponent();
        DataContextChanged += BankChequeIssueWindow_DataContextChanged;
    }

    private void BankChequeIssueWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is BankChequeIssueViewModel vm)
        {
            vm.RequestClose = () => Close();
        }
    }
}

