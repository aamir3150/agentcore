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

    public string PrintedBy
    {
        get => !string.IsNullOrWhiteSpace(PrintContext.CurrentUserName) ? PrintContext.CurrentUserName : "System";
        set { }
    }
    public DateTime PrintedAt
    {
        get => PrintContext.PrintedAt;
        set { }
    }
    public string PrintedAtTime
    {
        get => PrintedAt.ToString("hh:mm tt");
        set { }
    }

    public PrintFooter()
    {
        InitializeComponent();
    }
}
