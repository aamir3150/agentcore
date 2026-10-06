using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using MyMandiSystem.Core.Printing;
using Size = System.Windows.Size;
using Rect = System.Windows.Rect;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace MyMandiSystem.Printing;

/// <summary>
/// Common Print Engine.
/// Handles preview, print, paper sizing, and copies for ANY model.
/// The engine auto-selects the correct DataTemplate via WPF's DataType matching.
/// 
/// Adding a new form NEVER requires editing this class.
/// </summary>
public sealed class PrintService : IPrintService
{
    /// <summary>
    /// Pack URI to the merged dictionary that contains all print templates.
    /// Each template's DataTemplate has DataType="{x:Type ...}" so WPF picks
    /// the right one automatically.
    /// </summary>
    const string TemplatesUri = "pack://application:,,,/Printing/PrintTemplates.xaml";

    public void Preview(object model, PrintOptions? options = null)
    {
        var o = Resolve(model, options);
        var doc = Build(model, o);
        var viewer = new DocumentViewer { Document = doc };
        var window = new Window
        {
            Title = o.Title,
            Width = 900,
            Height = 700,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = viewer
        };
        window.ShowDialog();
    }

    public void Print(object model, PrintOptions? options = null)
    {
        var o = Resolve(model, options);
        var dlg = new PrintDialog();
        if (dlg.ShowDialog() != true) return;

        var doc = Build(model, o);
        for (int i = 0; i < Math.Max(1, o.Copies); i++)
            dlg.PrintDocument(doc.DocumentPaginator, $"{o.Title} ({i + 1})");
    }

    /// <summary>
    /// Resolve print options: caller override > model default > global default.
    /// </summary>
    static PrintOptions Resolve(object model, PrintOptions? o) =>
        o ?? (model as IPrintable)?.DefaultOptions ?? new PrintOptions();

    static ResourceDictionary GetTemplatesDictionary()
    {
        try
        {
            return new ResourceDictionary { Source = new Uri("pack://application:,,,/agentcore;component/Printing/PrintTemplates.xaml") };
        }
        catch
        {
            return new ResourceDictionary { Source = new Uri(TemplatesUri) };
        }
    }

    /// <summary>
    /// Build a FixedDocument from the model. The ContentControl loads
    /// the template dictionary and WPF auto-matches DataType to the model.
    /// </summary>
    static FixedDocument Build(object model, PrintOptions o)
    {
        var size = GetPageSize(o);

        var host = new ContentControl
        {
            Content = model,
            Width = size.Width
        };

        // Load all print templates
        host.Resources.MergedDictionaries.Add(GetTemplatesDictionary());

        if (o.Paper == PaperKind.Thermal80)
        {
            // Thermal roll: height grows to fit content
            host.Measure(new Size(size.Width, double.PositiveInfinity));
            size = new Size(size.Width, host.DesiredSize.Height);
        }
        else
        {
            host.Height = size.Height;
            host.Measure(new Size(size.Width, double.PositiveInfinity));
            if (host.DesiredSize.Height > size.Height)
                Debug.WriteLine($"⚠️ PRINT OVERFLOW: {model.GetType().Name} " +
                                $"(desired={host.DesiredSize.Height:F0}, page={size.Height:F0})");
        }

        host.Measure(size);
        host.Arrange(new Rect(size));
        host.UpdateLayout();

        var page = new FixedPage { Width = size.Width, Height = size.Height };
        page.Children.Add(host);

        var content = new PageContent();
        ((IAddChild)content).AddChild(page);

        var doc = new FixedDocument();
        doc.Pages.Add(content);
        return doc;
    }

    /// <summary>
    /// Paper dimensions in WPF units (1/96 inch).
    /// A4  = 210 × 297 mm ≈ 794 × 1123
    /// A5  = 148 × 210 mm ≈ 559 × 794
    /// 80mm thermal = 80 mm ≈ 302 wide, height = content
    /// </summary>
    static Size GetPageSize(PrintOptions o)
    {
        var (w, h) = o.Paper switch
        {
            PaperKind.A5        => (559.0, 794.0),
            PaperKind.Thermal80 => (302.0, 800.0),
            _                   => (794.0, 1123.0),  // A4
        };
        return o.Landscape ? new Size(h, w) : new Size(w, h);
    }
}
