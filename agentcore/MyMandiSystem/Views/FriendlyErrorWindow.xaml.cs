using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using MyMandiSystem.Helpers;

namespace MyMandiSystem.Views;

public partial class FriendlyErrorWindow : Window
{
    private readonly ErrorExplanation _explanation;

    public FriendlyErrorWindow(ErrorExplanation explanation)
    {
        InitializeComponent();
        _explanation = explanation ?? new ErrorExplanation();
        this.DataContext = _explanation;

        this.Loaded += (s, e) =>
        {
            try
            {
                SystemSounds.Exclamation.Play();
            }
            catch
            {
                // Ignore audio failure
            }
        };
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

    private async void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Windows.Clipboard.SetText(_explanation.FullDiagnosticText);

            CopyIcon.Kind = PackIconKind.Check;
            CopyText.Text = "Copied! / کاپی ہو گیا";
            CopyButton.Background = System.Windows.Media.Brushes.Honeydew;

            await Task.Delay(2500);

            CopyIcon.Kind = PackIconKind.ContentCopy;
            CopyText.Text = "Copy Details (تفصیلات کاپی کریں)";
            CopyButton.ClearValue(BackgroundProperty);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Clipboard copy failed: {ex.Message}", "Copy Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        this.Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}
