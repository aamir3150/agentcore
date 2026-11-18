using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class ExpenseTypesWindow : Window
{
    private readonly ExpenseTypesViewModel _viewModel;

    public ExpenseTypesWindow(ExpenseTypesViewModel viewModel, int initialTabIndex = 0)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        Loaded += async (s, e) =>
        {
            await _viewModel.InitializeAsync(initialTabIndex);
            UpdateTabVisibility(initialTabIndex);
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

    private void TabPurchase_Checked(object sender, RoutedEventArgs e)
    {
        UpdateTabVisibility(0);
    }

    private void TabSale_Checked(object sender, RoutedEventArgs e)
    {
        UpdateTabVisibility(1);
    }

    private void UpdateTabVisibility(int tabIndex)
    {
        if (PurchaseFieldsPanel == null || SaleFieldsPanel == null || SchemesListBox == null) return;

        if (tabIndex == 0)
        {
            PurchaseFieldsPanel.Visibility = Visibility.Visible;
            SaleFieldsPanel.Visibility = Visibility.Collapsed;
            SchemesListBox.ItemsSource = _viewModel.PurchaseSchemes;
        }
        else
        {
            PurchaseFieldsPanel.Visibility = Visibility.Collapsed;
            SaleFieldsPanel.Visibility = Visibility.Visible;
            SchemesListBox.ItemsSource = _viewModel.SaleSchemes;
        }
        _viewModel.SelectedTabIndex = tabIndex;
    }
}
