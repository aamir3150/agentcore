using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace MyMandiSystem.Printing;

/// <summary>
/// Column metadata descriptor for universal reporting.
/// </summary>
public class ReportColumnDesc
{
    public string Header { get; set; } = "";
    public string PropertyName { get; set; } = "";
    public double Weight { get; set; } = 1.0;
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
    public bool IsUrdu { get; set; } = false;
    public Brush? Foreground { get; set; } = null;
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
    public double FontSize { get; set; } = 10.0;
}

/// <summary>
/// Professional Multi-Page Document Engine for agentcore ERP.
/// Builds calibrated, vector FixedDocuments for A4 Portrait/Landscape:
/// 1. Width-wise 100% fit (equal, professional margins; zero empty side gap).
/// 2. Clean multi-page pagination when records exceed one page (repeated table headers).
/// 3. Standard page numbering at bottom: "Page X of Y (صفحہ X از Y)".
/// 4. Authentic Mandi letterhead on Page 1 and compact running header on subsequent pages.
/// </summary>
public static class ReportDocumentBuilder
{
    private static readonly FontFamily FontSegoe = new("Segoe UI");
    private static readonly Brush ColorTextDark = new SolidColorBrush(Color.FromRgb(15, 23, 42));
    private static readonly Brush ColorTextMuted = new SolidColorBrush(Color.FromRgb(71, 85, 105));
    private static readonly Brush ColorNavy = new SolidColorBrush(Color.FromRgb(15, 23, 42));
    private static readonly Brush ColorBorder = new SolidColorBrush(Color.FromRgb(226, 232, 240));
    private static readonly Brush ColorHeaderBg = new SolidColorBrush(Color.FromRgb(241, 245, 249));
    private static readonly Brush ColorRowAlt = new SolidColorBrush(Color.FromRgb(248, 250, 252));
    private static readonly Brush ColorDebit = new SolidColorBrush(Color.FromRgb(225, 29, 72));
    private static readonly Brush ColorCredit = new SolidColorBrush(Color.FromRgb(22, 163, 74));

    public static FixedDocument Build(
        string reportTitle,
        string reportBadge,
        string accountHeader,
        string periodHeader,
        IEnumerable reportData,
        double targetPageWidth,
        double targetPageHeight,
        decimal totalDebit,
        decimal totalCredit,
        int totalRecords,
        DataGrid? sourceGrid = null,
        string? netBalanceOverride = null)
    {
        var doc = new FixedDocument();

        // 1. Determine Page Dimensions (A4 standard: 793.7 x 1122.5 pt @ 96 DPI)
        double pageWidth = targetPageWidth > 200 ? targetPageWidth : 793.7;
        double pageHeight = targetPageHeight > 200 ? targetPageHeight : 1122.5;

        doc.DocumentPaginator.PageSize = new Size(pageWidth, pageHeight);

        // 2. Uniform Page Margins (30pt left & right guarantees 100% width fit with balanced margins)
        double marginH = 28.0;
        double marginV = 26.0;
        double contentWidth = pageWidth - (marginH * 2);
        double contentHeight = pageHeight - (marginV * 2);

        // 3. Resolve Urdu Font
        FontFamily fontUrdu = Application.Current?.TryFindResource("FontJameelNooriNastaleeq") as FontFamily
                             ?? new FontFamily("Jameel Noori Nastaleeq, Tahoma");

        // 4. Flatten Data Records
        var itemsList = new List<object>();
        if (reportData != null)
        {
            foreach (var item in reportData)
            {
                if (item != null) itemsList.Add(item);
            }
        }

        // 5. Resolve Column Definitions
        var columns = ResolveColumns(sourceGrid);

        // 6. Calculate Page Capacity
        bool isLandscape = pageWidth > pageHeight;
        // On portrait: Page 1 with letterhead comfortably fits ~22 rows (with summary) or ~26 (without).
        // Subsequent pages comfortably fit ~30 rows (with summary) or ~34 (without).
        int page1Capacity = isLandscape ? 16 : 24;
        int subsequentCapacity = isLandscape ? 22 : 30;

        var pagesList = new List<List<object>>();
        if (itemsList.Count <= page1Capacity)
        {
            pagesList.Add(itemsList);
        }
        else
        {
            pagesList.Add(itemsList.Take(page1Capacity).ToList());
            int offset = page1Capacity;

            while (offset < itemsList.Count)
            {
                int count = Math.Min(subsequentCapacity, itemsList.Count - offset);
                pagesList.Add(itemsList.Skip(offset).Take(count).ToList());
                offset += count;
            }
        }

        int totalPages = Math.Max(1, pagesList.Count);

        // 7. Build Each FixedPage
        for (int pIndex = 0; pIndex < totalPages; pIndex++)
        {
            var pageItems = pIndex < pagesList.Count ? pagesList[pIndex] : new List<object>();
            bool isFirstPage = (pIndex == 0);
            bool isLastPage = (pIndex == totalPages - 1);

            var fixedPage = new FixedPage
            {
                Width = pageWidth,
                Height = pageHeight,
                Background = Brushes.White
            };

            var rootGrid = new Grid
            {
                Width = contentWidth,
                Height = contentHeight,
                Margin = new Thickness(marginH, marginV, marginH, marginV)
            };

            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // 0: Header
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // 1: Table Header
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // 2: Table Rows
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // 3: Spacer / Summary
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // 4: Page Footer

            // --- Row 0: Header ---
            if (isFirstPage)
            {
                rootGrid.Children.Add(BuildFirstPageHeader(reportTitle, reportBadge, accountHeader, periodHeader, fontUrdu));
            }
            else
            {
                rootGrid.Children.Add(BuildSubsequentPageHeader(reportTitle, accountHeader, pIndex + 1, fontUrdu));
            }

            // --- Row 1: Table Column Headers (100% Width Fit) ---
            var tableHeader = BuildTableHeader(columns);
            Grid.SetRow(tableHeader, 1);
            rootGrid.Children.Add(tableHeader);

            // --- Row 2: Table Data Rows ---
            var rowsContainer = BuildTableRows(pageItems, columns, fontUrdu);
            Grid.SetRow(rowsContainer, 2);
            rootGrid.Children.Add(rowsContainer);

            // --- Row 3: Totals Summary Card (Only on Last Page) ---
            if (isLastPage)
            {
                var summaryCard = BuildSummaryCard(totalDebit, totalCredit, totalRecords, netBalanceOverride);
                Grid.SetRow(summaryCard, 3);
                rootGrid.Children.Add(summaryCard);
            }

            // --- Row 4: Page Footer with Page Numbers (Page X of Y) ---
            var footer = BuildPageFooter(pIndex + 1, totalPages, contentWidth);
            Grid.SetRow(footer, 4);
            rootGrid.Children.Add(footer);

            // Add rootGrid to FixedPage
            FixedPage.SetLeft(rootGrid, marginH);
            FixedPage.SetTop(rootGrid, marginV);
            rootGrid.Width = contentWidth;
            rootGrid.Height = contentHeight;
            rootGrid.Margin = new Thickness(0);
            fixedPage.Children.Add(rootGrid);

            // Measure & Arrange Page
            var pageSize = new Size(pageWidth, pageHeight);
            fixedPage.Measure(pageSize);
            fixedPage.Arrange(new Rect(new Point(0, 0), pageSize));
            fixedPage.UpdateLayout();

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);
            doc.Pages.Add(pageContent);
        }

        return doc;
    }

    // ══════════════════════════════════════════════════════════════════════
    //                       1. HEADER BUILDERS
    // ══════════════════════════════════════════════════════════════════════

    private static UIElement BuildFirstPageHeader(string title, string badge, string account, string period, FontFamily fontUrdu)
    {
        var comp = PrintContext.Company;
        var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };

        // Letterhead Grid
        var letterhead = new Border
        {
            BorderBrush = ColorNavy,
            BorderThickness = new Thickness(0, 0, 0, 2),
            Padding = new Thickness(0, 0, 0, 8),
            Margin = new Thickness(0, 0, 0, 8)
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(55) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(170) });

        // Left Emblem
        var emblem = new Border
        {
            Width = 46,
            Height = 46,
            CornerRadius = new CornerRadius(8),
            Background = ColorHeaderBg,
            BorderBrush = ColorBorder,
            BorderThickness = new Thickness(1),
            VerticalAlignment = VerticalAlignment.Center,
            Child = new TextBlock
            {
                Text = "🏛",
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Grid.SetColumn(emblem, 0);
        grid.Children.Add(emblem);

        // Center: Firm Details
        var centerStack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
        var urduText = string.IsNullOrWhiteSpace(comp.NameUrdu) ? comp.Name : comp.NameUrdu;
        centerStack.Children.Add(new TextBlock
        {
            Text = urduText,
            FontFamily = fontUrdu,
            FontSize = 22,
            FontWeight = FontWeights.Bold,
            Foreground = ColorNavy,
            HorizontalAlignment = HorizontalAlignment.Center,
            FlowDirection = FlowDirection.RightToLeft
        });

        centerStack.Children.Add(new TextBlock
        {
            Text = comp.Name,
            FontFamily = FontSegoe,
            FontSize = 13.5,
            FontWeight = FontWeights.Bold,
            Foreground = ColorTextDark,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 1, 0, 2)
        });

        string propPart = string.IsNullOrWhiteSpace(comp.Proprietor) ? "" : $" | پروپرائیٹر: {comp.Proprietor}";
        centerStack.Children.Add(new TextBlock
        {
            Text = $"{comp.Address}{propPart}  📞 {comp.Phone}",
            FontFamily = FontSegoe,
            FontSize = 10,
            Foreground = ColorTextMuted,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        Grid.SetColumn(centerStack, 1);
        grid.Children.Add(centerStack);

        // Right: Badge & Timestamp
        var rightStack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        var badgeBorder = new Border
        {
            Background = ColorNavy,
            CornerRadius = new CornerRadius(3),
            Padding = new Thickness(8, 2.5, 8, 2.5),
            HorizontalAlignment = HorizontalAlignment.Right,
            Child = new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(badge) ? "REPORT" : badge,
                FontFamily = FontSegoe,
                FontSize = 9.5,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            }
        };
        rightStack.Children.Add(badgeBorder);
        rightStack.Children.Add(new TextBlock
        {
            Text = $"Date: {DateTime.Now:dd/MM/yyyy hh:mm tt}",
            FontFamily = FontSegoe,
            FontSize = 9.5,
            Foreground = ColorTextMuted,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 3, 0, 0)
        });

        Grid.SetColumn(rightStack, 2);
        grid.Children.Add(rightStack);

        letterhead.Child = grid;
        panel.Children.Add(letterhead);

        // Target Account & Filter Subheader
        var subheader = new Border
        {
            Background = ColorRowAlt,
            BorderBrush = ColorBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10, 5, 10, 5)
        };
        var subGrid = new Grid();
        subGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        subGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var accStack = new StackPanel();
        accStack.Children.Add(new TextBlock
        {
            Text = string.IsNullOrWhiteSpace(account) ? "Account: General Ledger" : account,
            FontFamily = FontSegoe,
            FontSize = 11.5,
            FontWeight = FontWeights.Bold,
            Foreground = ColorNavy
        });
        accStack.Children.Add(new TextBlock
        {
            Text = string.IsNullOrWhiteSpace(period) ? "Period: All Dates" : period,
            FontFamily = FontSegoe,
            FontSize = 9.5,
            Foreground = ColorTextMuted,
            Margin = new Thickness(0, 1, 0, 0)
        });
        Grid.SetColumn(accStack, 0);
        subGrid.Children.Add(accStack);

        var curText = new TextBlock
        {
            Text = "Currency: PKR (روپے)",
            FontFamily = FontSegoe,
            FontSize = 9.5,
            FontWeight = FontWeights.SemiBold,
            Foreground = ColorTextMuted,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(curText, 1);
        subGrid.Children.Add(curText);

        subheader.Child = subGrid;
        panel.Children.Add(subheader);

        return panel;
    }

    private static UIElement BuildSubsequentPageHeader(string title, string account, int pageNum, FontFamily fontUrdu)
    {
        var comp = PrintContext.Company;
        var border = new Border
        {
            BorderBrush = ColorNavy,
            BorderThickness = new Thickness(0, 0, 0, 1.5),
            Padding = new Thickness(0, 0, 0, 6),
            Margin = new Thickness(0, 0, 0, 8)
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var left = new TextBlock
        {
            Text = $"{comp.Name} — {account}",
            FontFamily = FontSegoe,
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = ColorNavy,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(left, 0);
        grid.Children.Add(left);

        var right = new TextBlock
        {
            Text = $"(Continued / جاری ہے — Page {pageNum})",
            FontFamily = FontSegoe,
            FontSize = 10,
            Foreground = ColorTextMuted,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(right, 1);
        grid.Children.Add(right);

        border.Child = grid;
        return border;
    }

    // ══════════════════════════════════════════════════════════════════════
    //                       2. TABLE COLUMN RESOLUTION
    // ══════════════════════════════════════════════════════════════════════

    private static List<ReportColumnDesc> ResolveColumns(DataGrid? grid)
    {
        var list = new List<ReportColumnDesc>();

        if (grid != null && grid.Columns.Count > 0)
        {
            foreach (var col in grid.Columns)
            {
                if (col.Visibility != Visibility.Visible) continue;
                string header = col.Header?.ToString() ?? "";
                string propName = "";
                if (col is DataGridBoundColumn boundCol && boundCol.Binding is Binding b)
                {
                    propName = b.Path?.Path ?? "";
                }

                var lower = (header + " " + propName).ToLowerInvariant();
                var desc = new ReportColumnDesc
                {
                    Header = header,
                    PropertyName = propName
                };

                if (propName.Equals("AccountNo", StringComparison.OrdinalIgnoreCase) || lower.Contains("a/c #"))
                {
                    desc.Weight = 9;
                    desc.Alignment = TextAlignment.Center;
                    desc.FontWeight = FontWeights.SemiBold;
                }
                else if (propName.Equals("ContactNo", StringComparison.OrdinalIgnoreCase) || lower.Contains("contact"))
                {
                    desc.Weight = 11;
                    desc.Alignment = TextAlignment.Center;
                    desc.FontSize = 9;
                }
                else if (propName.Equals("PartyTitle", StringComparison.OrdinalIgnoreCase) || propName.Equals("AccountTitle", StringComparison.OrdinalIgnoreCase))
                {
                    desc.Weight = 24;
                    desc.Alignment = TextAlignment.Left;
                    desc.IsUrdu = true;
                    desc.FontSize = 10;
                }
                else if (propName.Equals("OpeningBal", StringComparison.OrdinalIgnoreCase))
                {
                    desc.Weight = 10.5;
                    desc.Alignment = TextAlignment.Right;
                    desc.FontWeight = FontWeights.SemiBold;
                }
                else if (propName.Equals("ClosingBal", StringComparison.OrdinalIgnoreCase) || propName.Equals("TodayBal", StringComparison.OrdinalIgnoreCase))
                {
                    desc.Weight = 11;
                    desc.Alignment = TextAlignment.Right;
                    desc.Foreground = ColorNavy;
                    desc.FontWeight = FontWeights.Bold;
                }
                else if (propName.Equals("OpeningType", StringComparison.OrdinalIgnoreCase) ||
                         propName.Equals("ClosingType", StringComparison.OrdinalIgnoreCase) ||
                         propName.Equals("TodayType", StringComparison.OrdinalIgnoreCase) ||
                         propName.Equals("BalType", StringComparison.OrdinalIgnoreCase))
                {
                    desc.Weight = 5;
                    desc.Alignment = TextAlignment.Center;
                    desc.FontWeight = FontWeights.Bold;
                    desc.FontSize = 8.5;
                }
                else if (lower.Contains("voucherno") || lower == "voucher #" || lower.Contains("واؤچر"))
                {
                    desc.Weight = 12;
                    desc.Alignment = TextAlignment.Center;
                    desc.FontWeight = FontWeights.SemiBold;
                }
                else if (lower.Contains("date") || lower.Contains("تاریخ"))
                {
                    desc.Weight = 11;
                    desc.Alignment = TextAlignment.Center;
                }
                else if (lower.Contains("chequeno") || lower.Contains("cheque") || lower.Contains("چیک"))
                {
                    desc.Weight = 9;
                    desc.Alignment = TextAlignment.Center;
                }
                else if (lower.Contains("narration") || lower.Contains("تفصیل") || lower.Contains("detail") || lower.Contains("title"))
                {
                    desc.Weight = 34;
                    desc.Alignment = TextAlignment.Left;
                    desc.IsUrdu = true;
                    desc.FontSize = 10.5;
                }
                else if (lower.Contains("debit") || lower.Contains("بنام"))
                {
                    desc.Weight = 11;
                    desc.Alignment = TextAlignment.Right;
                    desc.Foreground = ColorDebit;
                    desc.FontWeight = FontWeights.Bold;
                }
                else if (lower.Contains("credit") || lower.Contains("جمع"))
                {
                    desc.Weight = 11;
                    desc.Alignment = TextAlignment.Right;
                    desc.Foreground = ColorCredit;
                    desc.FontWeight = FontWeights.Bold;
                }
                else if (lower.Contains("balance") || lower.Contains("بقایا"))
                {
                    desc.Weight = 12;
                    desc.Alignment = TextAlignment.Right;
                    desc.Foreground = ColorNavy;
                    desc.FontWeight = FontWeights.Bold;
                }
                else if (lower.Contains("status") || lower.Contains("dr/cr"))
                {
                    desc.Weight = 6;
                    desc.Alignment = TextAlignment.Center;
                    desc.FontWeight = FontWeights.Bold;
                }
                else
                {
                    desc.Weight = 14;
                    desc.Alignment = TextAlignment.Left;
                }

                list.Add(desc);
            }
        }

        // Fallback default for standard Ledger
        if (list.Count == 0)
        {
            list.Add(new ReportColumnDesc { Header = "Date", PropertyName = "Date", Weight = 11, Alignment = TextAlignment.Center });
            list.Add(new ReportColumnDesc { Header = "Voucher #", PropertyName = "VoucherNo", Weight = 12, Alignment = TextAlignment.Center, FontWeight = FontWeights.SemiBold });
            list.Add(new ReportColumnDesc { Header = "Narration / Description", PropertyName = "Narration", Weight = 34, Alignment = TextAlignment.Left, IsUrdu = true, FontSize = 10.5 });
            list.Add(new ReportColumnDesc { Header = "Cheque #", PropertyName = "ChequeNo", Weight = 9, Alignment = TextAlignment.Center });
            list.Add(new ReportColumnDesc { Header = "Debit", PropertyName = "Debit", Weight = 11, Alignment = TextAlignment.Right, Foreground = ColorDebit, FontWeight = FontWeights.Bold });
            list.Add(new ReportColumnDesc { Header = "Credit", PropertyName = "Credit", Weight = 11, Alignment = TextAlignment.Right, Foreground = ColorCredit, FontWeight = FontWeights.Bold });
            list.Add(new ReportColumnDesc { Header = "Balance", PropertyName = "Balance", Weight = 12, Alignment = TextAlignment.Right, Foreground = ColorNavy, FontWeight = FontWeights.Bold });
            list.Add(new ReportColumnDesc { Header = "Dr/Cr", PropertyName = "Status", Weight = 6, Alignment = TextAlignment.Center, FontWeight = FontWeights.Bold });
        }

        return list;
    }

    // ══════════════════════════════════════════════════════════════════════
    //                       3. TABLE HEADERS (100% WIDTH FIT)
    // ══════════════════════════════════════════════════════════════════════

    private static UIElement BuildTableHeader(List<ReportColumnDesc> columns)
    {
        var border = new Border
        {
            Background = ColorHeaderBg,
            BorderBrush = ColorBorder,
            BorderThickness = new Thickness(1),
            Height = 28
        };

        var grid = new Grid();
        for (int i = 0; i < columns.Count; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columns[i].Weight, GridUnitType.Star) });
            var pad = columns[i].Alignment switch
            {
                TextAlignment.Right => new Thickness(4, 0, 8, 0),
                TextAlignment.Left => new Thickness(8, 0, 4, 0),
                _ => new Thickness(4, 0, 4, 0)
            };
            AddHeaderCell(grid, i, columns[i].Header, columns[i].Alignment, pad);
        }

        border.Child = grid;
        return border;
    }

    private static void AddHeaderCell(Grid grid, int col, string text, TextAlignment align, Thickness pad)
    {
        var tb = new TextBlock
        {
            Text = text,
            FontFamily = FontSegoe,
            FontSize = 9.5,
            FontWeight = FontWeights.Bold,
            Foreground = ColorNavy,
            TextAlignment = align,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = pad
        };
        Grid.SetColumn(tb, col);
        grid.Children.Add(tb);
    }

    // ══════════════════════════════════════════════════════════════════════
    //                       4. TABLE ROWS
    // ══════════════════════════════════════════════════════════════════════

    private static UIElement BuildTableRows(List<object> items, List<ReportColumnDesc> columns, FontFamily fontUrdu)
    {
        var panel = new StackPanel();

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var border = new Border
            {
                Background = (i % 2 == 1) ? ColorRowAlt : Brushes.White,
                BorderBrush = ColorBorder,
                BorderThickness = new Thickness(1, 0, 1, 1),
                Height = 26.0
            };

            var grid = new Grid();
            for (int c = 0; c < columns.Count; c++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columns[c].Weight, GridUnitType.Star) });
            }

            for (int c = 0; c < columns.Count; c++)
            {
                var col = columns[c];
                string cellText = ExtractCellValue(item, col.PropertyName);

                var pad = col.Alignment switch
                {
                    TextAlignment.Right => new Thickness(4, 0, 8, 0),
                    TextAlignment.Left => new Thickness(8, 0, 4, 0),
                    _ => new Thickness(4, 0, 4, 0)
                };

                FontFamily fontToUse = col.IsUrdu ? fontUrdu : FontSegoe;
                Brush fgToUse = col.Foreground ?? ColorTextDark;

                AddRowCell(grid, c, cellText, col.Alignment, fontToUse, col.FontSize, col.FontWeight, fgToUse, pad);
            }

            border.Child = grid;
            panel.Children.Add(border);
        }

        return panel;
    }

    private static void AddRowCell(Grid grid, int col, string text, TextAlignment align, FontFamily font, double size, FontWeight weight, Brush fg, Thickness pad)
    {
        var tb = new TextBlock
        {
            Text = text,
            FontFamily = font,
            FontSize = size,
            FontWeight = weight,
            Foreground = fg,
            TextAlignment = align,
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Padding = pad
        };
        Grid.SetColumn(tb, col);
        grid.Children.Add(tb);
    }

    private static string ExtractCellValue(object item, string propName)
    {
        object? raw = GetPropRaw(item, propName);

        // Fallback checks for common synonym property names
        if (raw == null)
        {
            if (propName.Contains("Voucher", StringComparison.OrdinalIgnoreCase))
                raw = GetPropRaw(item, "Reference") ?? GetPropRaw(item, "RefNo");
            else if (propName.Contains("Date", StringComparison.OrdinalIgnoreCase))
                raw = GetPropRaw(item, "TransactionDate") ?? GetPropRaw(item, "Date");
            else if (propName.Contains("Narration", StringComparison.OrdinalIgnoreCase))
                raw = GetPropRaw(item, "Description") ?? GetPropRaw(item, "Details");
        }

        if (raw is DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy");
        }
        if (raw is decimal dec)
        {
            return dec.ToString("N0");
        }
        if (raw is double dbl)
        {
            return dbl.ToString("N0");
        }

        string str = raw?.ToString() ?? "";

        // Detect if string is a DateTime
        if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDt) ||
            DateTime.TryParse(str, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDt))
        {
            if (parsedDt.Year > 1950 && parsedDt.Year < 2100)
                return parsedDt.ToString("dd/MM/yyyy");
        }

        bool isIdentifierText = propName.Contains("No", StringComparison.OrdinalIgnoreCase) ||
                                propName.Contains("Phone", StringComparison.OrdinalIgnoreCase) ||
                                propName.Contains("Mobile", StringComparison.OrdinalIgnoreCase) ||
                                propName.Contains("Contact", StringComparison.OrdinalIgnoreCase) ||
                                propName.Contains("Account", StringComparison.OrdinalIgnoreCase);

        // Detect if string is a decimal number (like "50000.00"), but never reformat phone/account/contact identifiers
        if (!isIdentifierText && decimal.TryParse(str, out var parsedDec) && (str.Contains('.') || str.Length >= 4))
        {
            return parsedDec.ToString("N0");
        }

        return str;
    }

    // ══════════════════════════════════════════════════════════════════════
    //                       5. TOTALS SUMMARY CARD
    // ══════════════════════════════════════════════════════════════════════

    private static UIElement BuildSummaryCard(decimal debit, decimal credit, int count, string? netBalanceOverride = null)
    {
        var border = new Border
        {
            Background = ColorHeaderBg,
            BorderBrush = ColorBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(12, 6, 12, 6),
            Margin = new Thickness(0, 8, 0, 0),
            VerticalAlignment = VerticalAlignment.Top
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Left: Record Count
        var left = new TextBlock
        {
            Text = $"Total Records: {count:N0}",
            FontFamily = FontSegoe,
            FontSize = 10,
            FontWeight = FontWeights.SemiBold,
            Foreground = ColorTextMuted,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(left, 0);
        grid.Children.Add(left);

        // Right: Badges
        var stack = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

        decimal diff = debit - credit;
        string balLabel = !string.IsNullOrWhiteSpace(netBalanceOverride)
            ? netBalanceOverride
            : (diff >= 0 ? $"{diff:N0} (Dr)" : $"{Math.Abs(diff):N0} (Cr)");

        stack.Children.Add(CreateBadge($"Total Debit: Rs. {debit:N0}", ColorDebit));
        stack.Children.Add(CreateBadge($"Total Credit: Rs. {credit:N0}", ColorCredit));
        stack.Children.Add(CreateBadge($"Net Balance: Rs. {balLabel}", ColorNavy));

        Grid.SetColumn(stack, 1);
        grid.Children.Add(stack);

        border.Child = grid;
        return border;
    }

    private static Border CreateBadge(string text, Brush fg)
    {
        return new Border
        {
            Background = Brushes.White,
            BorderBrush = ColorBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(3),
            Padding = new Thickness(8, 2.5, 8, 2.5),
            Margin = new Thickness(6, 0, 0, 0),
            Child = new TextBlock
            {
                Text = text,
                FontFamily = FontSegoe,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = fg
            }
        };
    }

    // ══════════════════════════════════════════════════════════════════════
    //                       6. PAGE FOOTER (PAGE X OF Y)
    // ══════════════════════════════════════════════════════════════════════

    private static UIElement BuildPageFooter(int pageNumber, int totalPages, double totalWidth)
    {
        var border = new Border
        {
            BorderBrush = ColorBorder,
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(0, 6, 0, 0),
            Margin = new Thickness(0, 6, 0, 0)
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Left info
        var left = new TextBlock
        {
            Text = $"Printed: {DateTime.Now:dd-MMM-yyyy hh:mm tt} | agentcore ERP System",
            FontFamily = FontSegoe,
            FontSize = 9,
            Foreground = ColorTextMuted,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(left, 0);
        grid.Children.Add(left);

        // Right Page Number: "Page X of Y (صفحہ X از Y)"
        var right = new TextBlock
        {
            Text = $"Page {pageNumber} of {totalPages} (صفحہ {pageNumber} از {totalPages})",
            FontFamily = FontSegoe,
            FontSize = 9.5,
            FontWeight = FontWeights.Bold,
            Foreground = ColorNavy,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(right, 1);
        grid.Children.Add(right);

        border.Child = grid;
        return border;
    }

    // ══════════════════════════════════════════════════════════════════════
    //                       7. REFLECTION HELPERS
    // ══════════════════════════════════════════════════════════════════════

    private static object? GetPropRaw(object? obj, string name)
    {
        if (obj == null || string.IsNullOrWhiteSpace(name)) return null;
        var prop = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return prop?.GetValue(obj);
    }
}
