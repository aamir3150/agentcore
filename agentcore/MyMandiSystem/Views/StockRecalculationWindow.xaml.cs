using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class StockRecalculationWindow : Window
{
    private readonly StockRecalculationViewModel _viewModel;

    public StockRecalculationWindow(StockRecalculationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        Loaded += async (s, e) =>
        {
            await _viewModel.InitializeAsync();
        };
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Filter_Changed(object sender, RoutedEventArgs e)
    {
        _viewModel?.ApplyFilters();
    }

    private void SearchText_Changed(object sender, TextChangedEventArgs e)
    {
        _viewModel?.ApplyFilters();
    }
}
