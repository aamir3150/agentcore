using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyMandiSystem;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        ContentRendered += (s, e) =>
        {
            try
            {
                System.IO.File.AppendAllText(@"D:\Projects\agentcore\agentcore\startup.log", $"[{DateTime.Now:HH:mm:ss.fff}] [MainWindow.ContentRendered] Window rendered on screen successfully.\r\n");
            }
            catch { }
        };
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var logPath = @"D:\Projects\agentcore\agentcore\startup.log";
            System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss.fff}] [MainWindow.Loaded] ActualWidth={ActualWidth}, ActualHeight={ActualHeight}, Visibility={Visibility}, IsVisible={IsVisible}, ContentType={Content?.GetType().FullName}\r\n");

            if (ActualWidth > 100 && ActualHeight > 100)
            {
                var rtb = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(this);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using var stream = System.IO.File.Create(@"D:\Projects\agentcore\agentcore\mainwindow_render.png");
                encoder.Save(stream);
                System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss.fff}] [MainWindow.Loaded] RenderTargetBitmap saved successfully.\r\n");
            }
        }
        catch (Exception ex)
        {
            try {
                System.IO.File.AppendAllText(@"D:\Projects\agentcore\agentcore\startup.log", $"[{DateTime.Now:HH:mm:ss.fff}] [MainWindow.Loaded] ERROR: {ex}\r\n");
            } catch {}
        }
    }
}