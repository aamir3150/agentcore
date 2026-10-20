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

    public ReportWindow(string reportPath, string dataSourceName, object data, Dictionary<string, string>? parameters = null)
    {
        InitializeComponent();

        _reportData = data as IEnumerable ?? new List<object>();

        // Set Human-readable Title
        var reportName = Path.GetFileNameWithoutExtension(reportPath ?? "Report");
        var displayTitle = reportName switch
        {
            "LedgerReport" => "Account Ledger Report (کھاتہ لیجر رپورٹ)",
            "DayBookReport" => "Daily Transaction Book / Day Book (روزنامچہ)",
            "TrialBalanceReport" => "Trial Balance Report (میزان کھاتہ جات)",
            "CashBookReport" => "Cash Book Report (روکڑ بہی رپورٹ)",
            "ProfitLossReport" => "Profit & Loss Statement (نفع و نقصان رپورٹ)",
            "BalanceSheetReport" => "Balance Sheet (بیلنس شیٹ)",
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
                TxtAccountHeader.Text = $"Account: {accVal}";
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
    }

    public void LoadBusinessHeader()
    {
        var comp = PrintContext.Company;
        TxtHeaderBusinessUrdu.Text = string.IsNullOrWhiteSpace(comp.NameUrdu) ? comp.Name : comp.NameUrdu;
        TxtHeaderBusinessName.Text = comp.Name;
        TxtHeaderAddress.Text = comp.Address;
        TxtHeaderPhone.Text = string.IsNullOrWhiteSpace(comp.Phone) ? "" : $"📞 {comp.Phone}";
        TxtHeaderProprietor.Text = string.IsNullOrWhiteSpace(comp.Proprietor) ? "" : $"| پروپرائیٹر: {comp.Proprietor}";
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

        // 1. Voucher #
        if (colName == "voucherno")
        {
            e.Column.Header = "Voucher # (واؤچر)";
            e.Column.Width = new DataGridLength(110);
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

        // 2. Format Dates
        if (e.PropertyType == typeof(DateTime) || e.PropertyType == typeof(DateTime?))
        {
            e.Column.Header = "Date (تاریخ)";
            e.Column.Width = new DataGridLength(110);
            if (e.Column is DataGridTextColumn dateCol)
            {
                dateCol.Binding = new Binding(e.PropertyName) { StringFormat = "dd/MM/yyyy" };
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

        // 3. Urdu Font for Narration & Details (Expands with Star width)
        if (colName.Contains("narration") || colName.Contains("urdu") || colName.Contains("description") || colName.Contains("title"))
        {
            e.Column.Header = "Narration / Details (تفصیل)";
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            e.Column.MinWidth = 220;
            if (e.Column is DataGridTextColumn textCol)
            {
                var fontResource = Application.Current.TryFindResource("FontJameelNooriNastaleeq") as FontFamily;
                textCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.FontFamilyProperty, fontResource ?? new FontFamily("Jameel Noori Nastaleeq, Tahoma")),
                        new Setter(TextBlock.FontSizeProperty, 14.0),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.PaddingProperty, new Thickness(8, 0, 8, 0))
                    }
                };
            }
            return;
        }

        // 4. Format Currency & Numbers (Debit / Credit)
        if (e.PropertyType == typeof(decimal) || e.PropertyType == typeof(decimal?) ||
            e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
        {
            if (e.Column is DataGridTextColumn numCol)
            {
                numCol.Binding = new Binding(e.PropertyName) { StringFormat = "{0:N0}" };
                
                var colorBrush = colName.Contains("debit") ? new SolidColorBrush(Color.FromRgb(225, 29, 72)) :
                                 colName.Contains("credit") ? new SolidColorBrush(Color.FromRgb(22, 163, 74)) :
                                 new SolidColorBrush(Color.FromRgb(15, 23, 42));

                numCol.ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                        new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                        new Setter(TextBlock.FontWeightProperty, FontWeights.Bold),
                        new Setter(TextBlock.ForegroundProperty, colorBrush),
                        new Setter(TextBlock.PaddingProperty, new Thickness(4, 0, 8, 0))
                    }
                };
            }

            e.Column.Header = colName.Contains("debit") ? "Debit / بنام" :
                             colName.Contains("credit") ? "Credit / جمع" : e.PropertyName;
            e.Column.Width = new DataGridLength(115);
            return;
        }

        // 5. Balance Column (Cr / Dr formatted string)
        if (colName == "balance")
        {
            e.Column.Header = "Balance (بقایا)";
            e.Column.Width = new DataGridLength(120);
            if (e.Column is DataGridTextColumn balCol)
            {
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

        // 6. Header Titles Fallback
        e.Column.Header = e.PropertyName switch
        {
            "Reference" => "Ref # (حوالہ)",
            "AccountNo" => "A/C #",
            "AccountName" => "Account Title (کھاتہ)",
            _ => e.PropertyName
        };
    }

    private int _totalCount = 0;
    private decimal _totalDebit = 0m;
    private decimal _totalCredit = 0m;

    private void CalculateTotals()
    {
        int count = 0;
        decimal totalDebit = 0m;
        decimal totalCredit = 0m;
        bool hasDebit = false;
        bool hasCredit = false;

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

        if (hasDebit || hasCredit)
        {
            BadgeBalance.Visibility = Visibility.Visible;
            var diff = totalDebit - totalCredit;
            var label = diff >= 0 ? $"{diff:N0} (Dr)" : $"{Math.Abs(diff):N0} (Cr)";
            TxtNetBalance.Text = label;
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
            sourceGrid: ReportDataGrid
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
