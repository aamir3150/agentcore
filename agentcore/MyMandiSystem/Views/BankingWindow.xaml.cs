using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class BankingWindow : Window
{
    public BankingWindow()
    {
        InitializeComponent();
    }

    public BankingWindow(BankingViewModel viewModel, int selectedTab = 0) : this()
    {
        viewModel.SelectedTabIndex = selectedTab;
        viewModel.RequestClose = Close;
        DataContext = viewModel;
    }
}
