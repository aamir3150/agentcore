using System.Windows;
using System.Windows.Input;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class BusinessProfileWindow : Window
{
    public BusinessProfileWindow()
    {
        InitializeComponent();
    }

    public BusinessProfileWindow(BusinessProfileViewModel vm) : this()
    {
        DataContext = vm;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            try
            {
                this.DragMove();
            }
            catch
            {
            }
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is BusinessProfileViewModel vm)
        {
            await vm.SaveAsync(this);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
