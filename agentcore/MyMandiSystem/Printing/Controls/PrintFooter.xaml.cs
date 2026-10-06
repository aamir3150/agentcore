using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace MyMandiSystem.Printing.Controls;

public partial class PrintFooter : UserControl
{
    public static readonly DependencyProperty CopyLabelProperty =
        DependencyProperty.Register(nameof(CopyLabel), typeof(string), typeof(PrintFooter), new PropertyMetadata(string.Empty));

    public string CopyLabel
    {
        get => (string)GetValue(CopyLabelProperty);
        set => SetValue(CopyLabelProperty, value);
    }

    public string PrintedBy => !string.IsNullOrWhiteSpace(PrintContext.CurrentUserName) ? PrintContext.CurrentUserName : "System";
    public DateTime PrintedAt => PrintContext.PrintedAt;
    public string PrintedAtTime => PrintedAt.ToString("hh:mm tt");

    public PrintFooter()
    {
        InitializeComponent();
    }
}
