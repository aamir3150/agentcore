using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class GatePassSaleWindow : Window
{
    public GatePassSaleWindow()
    {
        InitializeComponent();
    }

    public GatePassSaleWindow(GatePassSaleViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
