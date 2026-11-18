using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Printing;
using MyMandiSystem.ViewModels;

namespace MyMandiSystem.Views;

public partial class ReportWindow : Window
{
    private readonly IEnumerable _reportData;

    public ReportWindow(string reportPath, string dataSourceName, object data, Dictionary<string, string>? parameters = null, bool autoPrint = false)
    {
        InitializeComponent();

        _reportData = data as IEnumerable ?? new List<object>();

        // Set Human-readable Title
        var reportName = Path.GetFileNameWithoutExtension(reportPath ?? "Report");
        var displayTitle = reportName switch
        {
            "LedgerReport" => "Account Ledger Report",
            "DayBookReport" => "Daily Transaction Book / Day Book",
            "TrialBalanceReport" => "Trial Balance Report",
            "CashBookReport" => "Cash Book Report",
            "ProfitLossReport" => "Profit & Loss Statement",
            "BalanceSheetReport" => "Balance Sheet",
            "AccountPayablesReport" => "Account Payables Report",
            "AccountPayablesFinalsReport" => "Account Payables (With Today Balance)",
            "AccountPayablesBalanceReport" => "Account Payables (Final Balance)",
            "AccountReceivablesReport" => "Account Receivables Report",
            "AccountReceivablesFinalsReport" => "Account Receivables (With Today Balance)",
            "AccountReceivablesBalanceReport" => "Account Receivables (Final Balance)",
            _ => $"{reportName} Preview"
        };

        TxtReportTitle.Text = displayTitle;
        this.Title = $"agentcore - {displayTitle}";
        TxtHeaderReportBadge.Text = reportName.ToUpperInvariant();
        TxtHeaderTimestamp.Text = $"Date: {DateTime.Now:dd/MM/yyyy hh:mm tt}";

        // Load Business Letterhead
        LoadBusinessHeader();

        // Process Parameters
        if (parameters != null && parameters.Count > 0)
        {
            if (parameters.TryGetValue("Account", out var accVal) || 
                parameters.TryGetValue("AccountTitle", out accVal) ||
                parameters.TryGetValue("AccountName", out accVal))
            {
                TxtAccountHeader.Text = accVal.StartsWith("Party Type:", StringComparison.OrdinalIgnoreCase)
                    ? accVal
                    : $"Account: {accVal}";
            }

            if (parameters.TryGetValue("Period", out var periodVal) || 
                parameters.TryGetValue("DateRange", out periodVal))
            {
                TxtPeriodHeader.Text = $"Period: {periodVal}";
            }
        }

        // Bind Data
        ReportDataGrid.ItemsSource = _reportData;

        // Calculate Totals & Row Count
        CalculateTotals();

        if (autoPrint)
        {
            Loaded += (_, _) => Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Loaded,
                new Action(() => PrintButton_Click(this, new RoutedEventArgs())));
        }
    }

    public void LoadBusinessHeader()
    {
        var comp = PrintContext.Company;
        TxtHeaderBusinessName.Text = string.IsNullOrWhiteSpace(comp.Name) ? "City Computers Marot" : comp.Name;
        TxtHeaderAddress.Text = string.IsNullOrWhiteSpace(comp.Address) ? "Grain Market, Marot" : comp.Address;
        TxtHeaderPhone.Text = string.IsNullOrWhiteSpace(comp.Phone) ? "Phone: 0344-7436314" : $"Phone: {comp.Phone}";
        TxtHeaderProprietor.Text = string.IsNullOrWhiteSpace(comp.Proprietor) ? "" : $"| Proprietor: {comp.Proprietor}";
        TxtHeaderRegNo.Text = string.IsNullOrWhiteSpace(comp.RegistrationNo) ? "" : $"Reg #: {comp.RegistrationNo}";
    }

    private void ReportDataGrid_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var colName = e.PropertyName.ToLowerInvariant();

        // 0. Suppress duplicate Reference column if VoucherNo is present
        if (colName == "reference")
        {
            var firstItem = _reportData?.Cast<object>().FirstOrDefault();
            if (firstItem != null && firstItem.GetType().GetProperty("VoucherNo") != null)
            {
                e.Cancel = true;
                return;
            }
        }

        // 1. Date
        if (colName == "date" || colName == "transactiondate" || e.PropertyType == typeof(DateTime) || e.PropertyType == typeof(DateTime?))
        {
            e.Column.Header = "Date";
            e.Column.Width = new DataGridLength(95);
            if (e.Column is DataGridTextColumn dateCol)
            {
                if (e.PropertyType == typeof(DateTime) || e.PropertyType == typeof(DateTime?))
                {
                    dateCol.Binding = new Binding(e.PropertyName) { StringFormat = "dd/MM/yyyy" };
                }
                dateCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 4, 0))
                    }
                };
            }
            return;
        }

        // 2. Voucher #
        if (colName == "voucherno")
        {
            e.Column.Header = "Voucher #";
            e.Column.Width = new DataGridLength(100);
            if (e.Column is DataGridTextColumn vCol)
            {
                vCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.FontWeightProperty, FontWeights.SemiBold),
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(30, 41, 59))),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 4, 0))
                    }
                };
            }
            return;
        }

        // 2b. Account Payables / Receivables Columns
        if (colName == "accountno")
        {
            ConfigureStyledTextColumn(e, "A/C #", 90, TextAlignment.Center, FontWeights.SemiBold, Color.FromRgb(30, 41, 59), new Thickness(4, 0, 4, 0));
            return;
        }
        if (colName == "contactno")
        {
            ConfigureStyledTextColumn(e, "Contact #", 115, TextAlignment.Center, FontWeights.Normal, Color.FromRgb(71, 85, 105), new Thickness(4, 0, 4, 0));
            return;
        }
        if (colName == "openingbal")
        {
            ConfigureStyledNumericColumn(e, "Opening", 108, FontWeights.SemiBold, Color.FromRgb(51, 65, 85));
            return;
        }
        if (colName == "closingbal")
        {
            ConfigureStyledNumericColumn(e, "Closing Bal", 115, FontWeights.Bold, Color.FromRgb(15, 23, 42));
            return;
        }
        if (colName == "todaybal")
        {
            ConfigureStyledNumericColumn(e, "Today Bal", 115, FontWeights.Bold, Color.FromRgb(15, 118, 110));
            return;
        }
        if (colName == "openingtype" || colName == "closingtype" || colName == "todaytype" || colName == "baltype")
        {
            ConfigureStyledTextColumn(e, "Dr/Cr", 54, TextAlignment.Center, FontWeights.Bold, Color.FromRgb(51, 65, 85), new Thickness(2, 0, 2, 0));
            return;
        }

        // 3. Party Title / Narration / Description (Expands with Star width)
        if (colName.Contains("narration") || colName.Contains("urdu") || colName.Contains("description") || colName.Contains("title"))
        {
            e.Column.Header = (colName == "partytitle" || colName == "accounttitle") ? "Account Title" : "Narration / Description";
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            e.Column.MinWidth = 190;
            if (e.Column is DataGridTextColumn textCol)
            {
                var fontResource = Application.Current.TryFindResource("FontJameelNooriNastaleeq") as FontFamily;
                textCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.FontFamilyProperty, fontResource ?? new FontFamily("Jameel Noori Nastaleeq, Tahoma")),
                        new Setter(TextBlock.FontSizeProperty, 13.5),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.PaddingProperty, new Thickness(8, 0, 8, 0))
                    }
                };
            }
            return;
        }

        // 4. Cheque #
        if (colName == "chequeno")
        {
            e.Column.Header = "Cheque #";
            e.Column.Width = new DataGridLength(85);
            if (e.Column is DataGridTextColumn chqCol)
            {
                chqCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(100, 116, 139))),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 4, 0))
                    }
                };
            }
            return;
        }

        // 5. Debit
        if (colName.Contains("debit"))
        {
            e.Column.Header = "Debit";
            e.Column.Width = new DataGridLength(110);
            if (e.Column is DataGridTextColumn drCol)
            {
                drCol.Binding = new Binding(e.PropertyName) { StringFormat = "{0:N0}" };
                drCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.FontWeightProperty, FontWeights.Bold),
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(225, 29, 72))),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 8, 0))
                    }
                };
            }
            return;
        }

        // 6. Credit
        if (colName.Contains("credit"))
        {
            e.Column.Header = "Credit";
            e.Column.Width = new DataGridLength(110);
            if (e.Column is DataGridTextColumn crCol)
            {
                crCol.Binding = new Binding(e.PropertyName) { StringFormat = "{0:N0}" };
                crCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.FontWeightProperty, FontWeights.Bold),
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(22, 163, 74))),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 8, 0))
                    }
                };
            }
            return;
        }

        // 7. Balance (Running Balance)
        if (colName == "balance")
        {
            e.Column.Header = "Balance";
            e.Column.Width = new DataGridLength(115);
            if (e.Column is DataGridTextColumn balCol)
            {
                balCol.Binding = new Binding(e.PropertyName) { StringFormat = "{0:N0}" };
                balCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.FontWeightProperty, FontWeights.Bold),
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(15, 23, 42))),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 8, 0))
                    }
                };
            }
            return;
        }

        // 8. Dr / Cr Status
        if (colName == "status")
        {
            e.Column.Header = "Dr/Cr";
            e.Column.Width = new DataGridLength(55);
            if (e.Column is DataGridTextColumn statCol)
            {
                statCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.FontWeightProperty, FontWeights.Bold),
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(51, 65, 85))),
                        new Setter(TextBlock.PaddingProperty, new Thickness(2, 0, 2, 0))
                    }
                };
            }
            return;
        }

        // 9. Format any other Numeric Columns
        if (e.PropertyType == typeof(decimal) || e.PropertyType == typeof(decimal?) ||
            e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
        {
            if (e.Column is DataGridTextColumn numCol)
            {
                numCol.Binding = new Binding(e.PropertyName) { StringFormat = "{0:N0}" };
                numCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.FontWeightProperty, FontWeights.Bold),
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(15, 23, 42))),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 8, 0))
                    }
                };
            }
            e.Column.Width = new DataGridLength(110);
            return;
        }

        // 10. Fallback Headers
        e.Column.Header = e.PropertyName switch
        {
            "Reference" => "Ref #",
            "AccountNo" => "A/C #",
            "AccountName" => "Account Title",
            _ => e.PropertyName
        };
    }

    private static void ConfigureStyledTextColumn(
        DataGridAutoGeneratingColumnEventArgs e,
        string header,
        double width,
        TextAlignment align,
        FontWeight weight,
        Color fg,
        Thickness padding)
    {
        e.Column.Header = header;
        e.Column.Width = new DataGridLength(width);
        if (e.Column is DataGridTextColumn textCol)
        {
            textCol.ElementStyle = new Style(typeof(TextBlock))
            {
                Setters =
                {
                    new Setter(TextBlock.TextAlignmentProperty, align),
                    new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                    new Setter(TextBlock.FontWeightProperty, weight),
                    new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(fg)),
                    new Setter(TextBlock.PaddingProperty, padding)
                }
            };
        }
    }

    private static void ConfigureStyledNumericColumn(
        DataGridAutoGeneratingColumnEventArgs e,
        string header,
        double width,
        FontWeight weight,
        Color fg)
    {
        if (e.Column is DataGridTextColumn numCol)
        {
            numCol.Binding = new Binding(e.PropertyName) { StringFormat = "{0:N0}" };
        }
        ConfigureStyledTextColumn(e, header, width, TextAlignment.Right, weight, fg, new Thickness(4, 0, 8, 0));
    }

    private int _totalCount = 0;
    private decimal _totalDebit = 0m;
    private decimal _totalCredit = 0m;

    private void CalculateTotals()
    {
        int count = 0;
        decimal totalDebit = 0m;
        decimal totalCredit = 0m;
        decimal netClosingSigned = 0m;
        bool hasDebit = false;
        bool hasCredit = false;
        bool hasClosing = false;

        foreach (var item in _reportData)
        {
            count++;
            if (item == null) continue;

            var itemType = item.GetType();

            var debitProp = itemType.GetProperty("Debit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (debitProp != null)
            {
                var val = debitProp.GetValue(item);
                if (val is decimal d) { totalDebit += d; hasDebit = true; }
            }

            var creditProp = itemType.GetProperty("Credit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (creditProp != null)
            {
                var val = creditProp.GetValue(item);
                if (val is decimal c) { totalCredit += c; hasCredit = true; }
            }

            var closingProp = itemType.GetProperty("ClosingBal", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (closingProp != null && closingProp.GetValue(item) is decimal cb)
            {
                hasClosing = true;
                var typeProp = itemType.GetProperty("ClosingType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                            ?? itemType.GetProperty("ClosingBalType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                string balType = typeProp?.GetValue(item)?.ToString()?.Trim() ?? string.Empty;
                netClosingSigned += balType.Equals("Cr", StringComparison.OrdinalIgnoreCase) ? -cb : cb;
            }
        }

        _totalCount = count;
        _totalDebit = totalDebit;
        _totalCredit = totalCredit;

        TxtRowCount.Text = count.ToString("N0");

        if (hasDebit)
        {
            BadgeDebit.Visibility = Visibility.Visible;
            TxtTotalDebit.Text = totalDebit.ToString("N0");
        }

        if (hasCredit)
        {
            BadgeCredit.Visibility = Visibility.Visible;
            TxtTotalCredit.Text = totalCredit.ToString("N0");
        }

        if (hasDebit || hasCredit || hasClosing || count > 0)
        {
            BadgeBalance.Visibility = Visibility.Visible;
            decimal finalBalance = hasClosing ? Math.Abs(netClosingSigned) : Math.Abs(totalDebit - totalCredit);
            string finalStatus = hasClosing
                ? (netClosingSigned >= 0 ? "Dr" : "Cr")
                : ((totalDebit >= totalCredit) ? "Dr" : "Cr");

            var lastItem = _reportData?.Cast<object>().LastOrDefault();
            if (lastItem != null)
            {
                var bProp = lastItem.GetType().GetProperty("Balance", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var sProp = lastItem.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (bProp != null && bProp.GetValue(lastItem) is decimal bVal)
                {
                    finalBalance = bVal;
                    if (sProp != null && sProp.GetValue(lastItem)?.ToString() is string sVal && !string.IsNullOrWhiteSpace(sVal))
                    {
                        finalStatus = sVal;
                    }
                }
            }

            TxtNetBalance.Text = $"{finalBalance:N0} ({finalStatus})";
        }
    }

    private System.Windows.Documents.FixedDocument BuildReportDocument(double targetWidth, double targetHeight)
    {
        return ReportDocumentBuilder.Build(
            reportTitle: TxtReportTitle.Text,
            reportBadge: TxtHeaderReportBadge.Text,
            accountHeader: TxtAccountHeader.Text,
            periodHeader: TxtPeriodHeader.Text,
            reportData: _reportData,
            targetPageWidth: targetWidth,
            targetPageHeight: targetHeight,
            totalDebit: _totalDebit,
            totalCredit: _totalCredit,
            totalRecords: _totalCount,
            sourceGrid: ReportDataGrid,
            netBalanceOverride: BadgeBalance.Visibility == Visibility.Visible ? TxtNetBalance.Text : null
        );
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Build calibrated A4 Portrait multi-page document
            var doc = BuildReportDocument(793.7, 1122.5);
            var viewer = new DocumentViewer { Document = doc };
            var previewWin = new Window
            {
                Title = $"{this.Title} - Multi-Page Print Preview",
                Width = 960,
                Height = 850,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = viewer,
                Owner = this
            };
            previewWin.ShowDialog();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Preview failed: {ex.Message}", "Preview Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var printDlg = new System.Windows.Controls.PrintDialog();

            // Set Portrait orientation by default for standard A4 report
            try
            {
                if (printDlg.PrintTicket != null)
                {
                    printDlg.PrintTicket.PageOrientation = System.Printing.PageOrientation.Portrait;
                }
            }
            catch { }

            if (printDlg.ShowDialog() == true)
            {
                double targetWidth = printDlg.PrintableAreaWidth > 100 ? printDlg.PrintableAreaWidth : 793.7;
                double targetHeight = printDlg.PrintableAreaHeight > 100 ? printDlg.PrintableAreaHeight : 1122.5;

                // If user selected Portrait, ensure width is the smaller dimension
                bool isLandscape = printDlg.PrintTicket?.PageOrientation == System.Printing.PageOrientation.Landscape;
                if (!isLandscape && targetWidth > targetHeight)
                {
                    (targetWidth, targetHeight) = (targetHeight, targetWidth);
                }

                // Build calibrated multi-page vector FixedDocument (100% width fit, repeated headers, page numbers)
                var doc = BuildReportDocument(targetWidth, targetHeight);
                printDlg.PrintDocument(doc.DocumentPaginator, this.Title);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Print failed: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var saveDlg = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FileName = $"{TxtReportTitle.Text.Split(' ')[0]}_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            };

            if (saveDlg.ShowDialog() == true)
            {
                var sb = new StringBuilder();

                // Headers
                var columns = ReportDataGrid.Columns;
                var headerNames = columns.Select(c => $"\"{c.Header}\"");
                sb.AppendLine(string.Join(",", headerNames));

                // Rows
                foreach (var item in _reportData)
                {
                    if (item == null) continue;
                    var itemType = item.GetType();
                    var rowValues = new List<string>();

                    foreach (var col in columns)
                    {
                        if (col is DataGridBoundColumn boundCol && boundCol.Binding is Binding b)
                        {
                            var prop = itemType.GetProperty(b.Path.Path);
                            var val = prop?.GetValue(item)?.ToString() ?? "";
                            val = val.Replace("\"", "\"\"");
                            rowValues.Add($"\"{val}\"");
                        }
                        else
                        {
                            rowValues.Add("\"\"");
                        }
                    }
                    sb.AppendLine(string.Join(",", rowValues));
                }

                File.WriteAllText(saveDlg.FileName, sb.ToString(), Encoding.UTF8);
                System.Windows.MessageBox.Show("Report exported successfully to CSV!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
