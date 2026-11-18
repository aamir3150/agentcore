using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class DatabaseBackupWindow : Window
{
    public DatabaseBackupWindow(DatabaseBackupViewModel viewModel)
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
}
