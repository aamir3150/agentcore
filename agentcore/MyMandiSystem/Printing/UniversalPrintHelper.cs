using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyMandiSystem.Printing;

/// <summary>
/// Universal Print Engine Helper for agentcore ERP.
/// Ensures all reports, ledgers, vouchers, and UI documents print:
/// 1. Exactly centered and fitted to the printer page width (A4, Letter, etc.).
/// 2. Without off-center shifts, huge left gaps, or clipped right edges.
/// 3. Fully supporting both Physical Printers and Microsoft Print-to-PDF.
/// </summary>
public static class UniversalPrintHelper
{
    /// <summary>
    /// Prints any WPF FrameworkElement calibrated and fitted 100% to the printer page width.
    /// Eliminates parent layout offsets (such as centering gaps) and scales to match printable bounds.
    /// </summary>
    public static bool PrintVisual(PrintDialog printDlg, FrameworkElement element, string documentTitle, double pageMargin = 16.0)
    {
        if (element == null) return false;

        // 1. Capture original on-screen layout state
        var origTransform = element.LayoutTransform;
        var origHorizontalAlignment = element.HorizontalAlignment;
        var origVerticalAlignment = element.VerticalAlignment;
        var origMargin = element.Margin;

        // Locate parent ScrollViewer (common cause of horizontal centering offsets)
        var parentScrollViewer = FindVisualParent<ScrollViewer>(element) ?? (element.Parent as ScrollViewer);
        Thickness origScrollPadding = default;
        double origHOffset = 0;
        double origVOffset = 0;

        if (parentScrollViewer != null)
        {
            origScrollPadding = parentScrollViewer.Padding;
            origHOffset = parentScrollViewer.HorizontalOffset;
            origVOffset = parentScrollViewer.VerticalOffset;

            parentScrollViewer.Padding = new Thickness(0);
            parentScrollViewer.ScrollToHorizontalOffset(0);
            parentScrollViewer.ScrollToVerticalOffset(0);
        }

        try
        {
            // 2. Query exact printable area of the target printer / PDF driver
            double printableWidth = printDlg.PrintableAreaWidth;
            double printableHeight = printDlg.PrintableAreaHeight;

            // Safe fallback for virtual drivers or uninitialized tickets
            if (printableWidth <= 100) printableWidth = 793.7; // Standard A4 at 96 DPI
            if (printableHeight <= 100) printableHeight = 1122.5;

            // Target printable width inside margins
            double usableWidth = printableWidth - (pageMargin * 2);
            if (usableWidth <= 0) usableWidth = printableWidth;

            // 3. Obtain current unscaled element width
            double elementWidth = element.ActualWidth > 0 ? element.ActualWidth : element.Width;
            if (double.IsNaN(elementWidth) || elementWidth <= 0)
            {
                element.Measure(new Size(printableWidth, double.PositiveInfinity));
                elementWidth = element.DesiredSize.Width > 0 ? element.DesiredSize.Width : 820.0;
            }

            // 4. Calculate exact proportional scale factor to fit page width 100%
            double scale = usableWidth / elementWidth;

            // 5. Reset alignment to Left with uniform page margin
            // This ELIMINATES the parent centering offset (the large empty gap) completely!
            element.HorizontalAlignment = HorizontalAlignment.Left;
            element.VerticalAlignment = VerticalAlignment.Top;
            element.Margin = new Thickness(pageMargin, pageMargin, 0, 0);
            element.LayoutTransform = new ScaleTransform(scale, scale);

            // Force layout update so the print engine captures the calibrated geometry
            element.UpdateLayout();

            // 6. Send calibrated visual to print engine
            printDlg.PrintVisual(element, documentTitle);
            return true;
        }
        finally
        {
            // 7. Cleanly restore original on-screen visual state
            element.LayoutTransform = origTransform;
            element.HorizontalAlignment = origHorizontalAlignment;
            element.VerticalAlignment = origVerticalAlignment;
            element.Margin = origMargin;

            if (parentScrollViewer != null)
            {
                parentScrollViewer.Padding = origScrollPadding;
                parentScrollViewer.ScrollToHorizontalOffset(origHOffset);
                parentScrollViewer.ScrollToVerticalOffset(origVOffset);
            }

            element.UpdateLayout();
        }
    }

    private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);
        if (parentObject == null) return null;

        if (parentObject is T parent)
            return parent;

        return FindVisualParent<T>(parentObject);
    }
}
