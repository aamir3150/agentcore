using System.Collections.ObjectModel;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace MyMandiSystem.Printing.Controls;

public partial class SignatureBlock : UserControl
{
    public static readonly DependencyProperty LabelsProperty =
        DependencyProperty.Register(nameof(Labels), typeof(string), typeof(SignatureBlock),
            new PropertyMetadata(string.Empty, OnLabelsChanged));

    public string Labels
    {
        get => (string)GetValue(LabelsProperty);
        set => SetValue(LabelsProperty, value);
    }

    public ObservableCollection<string> LabelList { get; } = new();

    public SignatureBlock()
    {
        InitializeComponent();
    }

    private static void OnLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SignatureBlock control && e.NewValue is string text)
        {
            control.LabelList.Clear();
            var parts = text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                control.LabelList.Add(part.Trim());
            }
        }
    }
}
