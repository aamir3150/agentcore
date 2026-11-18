using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class BusinessProfileWindow : Window
{
    public BusinessProfileWindow(BusinessProfileViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
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

    private void Tab1_Checked(object sender, RoutedEventArgs e)
    {
        ShowTab(1);
    }

    private void Tab2_Checked(object sender, RoutedEventArgs e)
    {
        ShowTab(2);
    }

    private void Tab3_Checked(object sender, RoutedEventArgs e)
    {
        ShowTab(3);
    }

    private void ShowTab(int tabIndex)
    {
        if (Tab1Panel == null || Tab2Panel == null || Tab3Panel == null) return;

        Tab1Panel.Visibility = tabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        Tab2Panel.Visibility = tabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        Tab3Panel.Visibility = tabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
    }
}
