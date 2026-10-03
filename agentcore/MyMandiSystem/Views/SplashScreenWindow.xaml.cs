using System.Windows;

namespace MyMandiSystem.Views;

public partial class SplashScreenWindow : Window
{
    public SplashScreenWindow()
    {
        InitializeComponent();
    }

    public void UpdateProgress(double percentage, string statusMessage)
    {
        Dispatcher.Invoke(() =>
        {
            StartupProgressBar.Value = percentage;
            TxtPercentage.Text = $"{(int)percentage}%";
            TxtStatus.Text = statusMessage;
        });
    }
}
