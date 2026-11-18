using MyMandiSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyMandiSystem.Views;

public partial class SmsSendingWindow : Window
{
    private readonly SmsSendingViewModel _viewModel;

    public SmsSendingWindow(SmsSendingViewModel viewModel)
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

    private void SearchText_Changed(object sender, TextChangedEventArgs e)
    {
        _viewModel?.ApplyFilter();
    }

    private void Template_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb && cb.SelectedItem is string templateType)
        {
            _viewModel?.OnTemplateSelected(templateType);
        }
    }

    private void MessageTemplate_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel?.UpdatePreview();
    }
}
