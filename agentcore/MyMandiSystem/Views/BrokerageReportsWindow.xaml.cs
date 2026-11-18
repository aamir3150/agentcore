using MyMandiSystem.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MyMandiSystem.Views;

public partial class BrokerageReportsWindow : Window
{
    public BrokerageReportsWindow()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            if (DataContext is BrokerageReportsViewModel vm)
            {
                vm.RequestClose = () => this.Close();
                SetupColumnsForMode(vm.Mode);

                vm.PropertyChanged += (ps, pe) =>
                {
                    if (pe.PropertyName == nameof(BrokerageReportsViewModel.Mode))
                    {
                        Dispatcher.Invoke(() => SetupColumnsForMode(vm.Mode));
                    }
                };
            }
        };

        DataContextChanged += (s, e) =>
        {
            if (e.NewValue is BrokerageReportsViewModel vm)
            {
                vm.RequestClose = () => this.Close();
                SetupColumnsForMode(vm.Mode);

                vm.PropertyChanged += (ps, pe) =>
                {
                    if (pe.PropertyName == nameof(BrokerageReportsViewModel.Mode))
                    {
                        Dispatcher.Invoke(() => SetupColumnsForMode(vm.Mode));
                    }
                };
            }
        };

        KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.F5)
            {
                if (DataContext is BrokerageReportsViewModel vm)
                {
                    _ = vm.LoadDataCommand.ExecuteAsync(null);
                }
            }
        };
    }

    private void SetupColumnsForMode(BrokerageReportMode mode)
    {
        ReportDataGrid.Columns.Clear();

        switch (mode)
        {
            case BrokerageReportMode.CurrentStock:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Code", "DocNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Product Name", "ProductName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "Narration", 140));
                ReportDataGrid.Columns.Add(Col("Current Stock (Kg)", "NetWeight", 125, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Mounds (من)", "Mounds", 100, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Rate (Rs)", "Rate", 90, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Stock Value (Rs)", "GrossAmount", 135, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 95, TextAlignment.Center, isBold: true));
                break;

            case BrokerageReportMode.CommissionReport:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Invoice #", "DocNo", 100));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 100, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Gross Amount", "GrossAmount", 120, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Commission (کمیشن)", "CommissionAmount", 125, TextAlignment.Right, "{0:N0}", isBold: true, foreground: Brushes.ForestGreen));
                ReportDataGrid.Columns.Add(Col("Soodh (سود)", "SoodhAmount", 100, TextAlignment.Right, "{0:N0}", foreground: Brushes.Purple));
                ReportDataGrid.Columns.Add(Col("Expenses", "Expenses", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Amount", "NetAmount", 120, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center, isBold: true));
                break;

            case BrokerageReportMode.ContractsRegister:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Contract #", "DocNo", 100));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 85, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 160, isBold: true));
                ReportDataGrid.Columns.Add(Col("Product", "ProductName", 130));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 95, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Driver", "DriverName", 110));
                ReportDataGrid.Columns.Add(Col("Bags", "Bags", 80, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Weight (Kg)", "NetWeight", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Mounds (من)", "Mounds", 90, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Rate", "Rate", 80, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Total Amount", "GrossAmount", 120, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Commission", "CommissionAmount", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center));
                break;

            case BrokerageReportMode.InvoicesRegister:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Invoice #", "DocNo", 100));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 85, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 95, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Gross Amount", "GrossAmount", 120, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Commission", "CommissionAmount", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("W.H.T", "WHT_Amount", 85, TextAlignment.Right, "{0:N0}", foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Soodh", "SoodhAmount", 85, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Amount", "NetAmount", 120, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center));
                break;

            case BrokerageReportMode.PurchaseRegister:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Invoice #", "DocNo", 100));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 170, isBold: true));
                ReportDataGrid.Columns.Add(Col("Product", "ProductName", 130));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 95, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Bags", "Bags", 70, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Wt (Kg)", "NetWeight", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Mounds (من)", "Mounds", 90, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Rate", "Rate", 80, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Gross Amount", "GrossAmount", 115, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Commission", "CommissionAmount", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Expenses", "Expenses", 90, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Amount", "NetAmount", 120, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center));
                break;

            case BrokerageReportMode.SaleRegister:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Invoice #", "DocNo", 100));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 170, isBold: true));
                ReportDataGrid.Columns.Add(Col("Product", "ProductName", 130));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 95, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Bags", "Bags", 70, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Wt (Kg)", "NetWeight", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Mounds (من)", "Mounds", 90, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Rate", "Rate", 80, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Gross Amount", "GrossAmount", 115, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Commission", "CommissionAmount", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Expenses", "Expenses", 90, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Amount", "NetAmount", 120, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center));
                break;

            case BrokerageReportMode.StockReport:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Code", "DocNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Product Name", "ProductName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Opening (Kg)", "DrWeight", 110, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Purchases (Kg)", "GrossWeight", 110, TextAlignment.Right, "{0:N0}", foreground: Brushes.ForestGreen));
                ReportDataGrid.Columns.Add(Col("Sales (Kg)", "TareWeight", 110, TextAlignment.Right, "{0:N0}", foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Closing (Kg)", "NetWeight", 120, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Mounds (من)", "Mounds", 100, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Rate", "Rate", 90, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Stock Value", "NetAmount", 130, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 90, TextAlignment.Center, isBold: true));
                break;

            case BrokerageReportMode.PartyLedgerExtended:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Doc #", "DocNo", 95, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 95, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Bags", "Bags", 70, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Mounds (من)", "Mounds", 85, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(StarCol("Narration / تفصیل", "Narration", 200));
                ReportDataGrid.Columns.Add(Col("Debit (بنام)", "DrValue", 115, TextAlignment.Right, "{0:N0}", isBold: true, foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Credit (جمع)", "CrValue", 115, TextAlignment.Right, "{0:N0}", isBold: true, foreground: Brushes.ForestGreen));
                ReportDataGrid.Columns.Add(Col("Balance (بقایا)", "BalValue", 125, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Dr/Cr", "Status", 65, TextAlignment.Center, isBold: true));
                break;

            case BrokerageReportMode.MonthlyCommissionSoodh:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Month / Year", "MonthName", 160, isBold: true));
                ReportDataGrid.Columns.Add(Col("Invoices Count", "InvoicesCount", 110, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Gross Business (Rs)", "GrossAmount", 140, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Commission (Rs)", "CommissionAmount", 140, TextAlignment.Right, "{0:N0}", isBold: true, foreground: Brushes.ForestGreen));
                ReportDataGrid.Columns.Add(Col("Soodh (Rs)", "SoodhAmount", 130, TextAlignment.Right, "{0:N0}", foreground: Brushes.Purple));
                ReportDataGrid.Columns.Add(Col("Total Earnings (کمیشن + سود)", "NetAmount", 165, TextAlignment.Right, "{0:N0}", isBold: true, foreground: Brushes.DarkGreen));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center));
                break;

            case BrokerageReportMode.PurchaseSummary:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Product Name (جنس)", "ProductName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Invoices Count", "InvoicesCount", 110, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Total Bags", "Bags", 100, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Total Net Weight (Kg)", "NetWeight", 130, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Total Mounds (من)", "Mounds", 120, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Average Rate", "Rate", 110, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Total Amount (Rs)", "NetAmount", 140, TextAlignment.Right, "{0:N0}", isBold: true));
                break;

            case BrokerageReportMode.PurchaseSummaryPartyWise:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Party Name (زمیندار / وینڈر)", "PartyName", 200, isBold: true));
                ReportDataGrid.Columns.Add(Col("Invoices Count", "InvoicesCount", 110, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Total Bags", "Bags", 100, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Total Net Weight (Kg)", "NetWeight", 130, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Total Mounds (من)", "Mounds", 120, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Gross Amount", "GrossAmount", 130, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Amount (Rs)", "NetAmount", 140, TextAlignment.Right, "{0:N0}", isBold: true));
                break;

            case BrokerageReportMode.PartyWiseWHT:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 200, isBold: true));
                ReportDataGrid.Columns.Add(Col("CNIC / NTN", "Narration", 140, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Invoices Count", "InvoicesCount", 110, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Gross Amount (Rs)", "GrossAmount", 130, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("W.H.T Rate (%)", "WHT_Rate", 100, TextAlignment.Right, "{0:N2}%"));
                ReportDataGrid.Columns.Add(Col("W.H.T Amount (ود ہولڈنگ)", "WHT_Amount", 135, TextAlignment.Right, "{0:N0}", isBold: true, foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Net After Tax (Rs)", "NetAmount", 140, TextAlignment.Right, "{0:N0}", isBold: true));
                break;

            case BrokerageReportMode.VehicleWiseSummary:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 110, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Product", "ProductName", 120));
                ReportDataGrid.Columns.Add(StarCol("Purchase Party (فروخت کنندہ)", "PurchaseParty", 150));
                ReportDataGrid.Columns.Add(StarCol("Sale Party (خریدار)", "SaleParty", 150));
                ReportDataGrid.Columns.Add(Col("Pur Wt (Kg)", "PurchaseKgs", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Sale Wt (Kg)", "SaleKgs", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Shortage (Kg)", "Shortage", 105, TextAlignment.Right, "{0:N0}", isBold: true, foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Pur Value", "PurchaseValue", 110, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Sale Value", "SaleValue", 110, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Profit / Loss", "ProfitLoss", 125, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center, isBold: true));
                break;

            case BrokerageReportMode.GatePassRegister:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Gate Pass #", "DocNo", 100));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Type (آمد / روانگی)", "DocType", 100, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 160, isBold: true));
                ReportDataGrid.Columns.Add(Col("Product / Item", "ProductName", 130));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 95, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Driver Name", "DriverName", 120));
                ReportDataGrid.Columns.Add(Col("Bags", "Bags", 70, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Gross Wt (Kg)", "GrossWeight", 95, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Tare Wt (Kg)", "TareWeight", 90, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Net Wt (Kg)", "NetWeight", 100, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Mounds (من)", "Mounds", 95, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center));
                break;

            case BrokerageReportMode.ProductLedger:
                ReportDataGrid.Columns.Add(Col("#", "Id", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Doc / Inv #", "DocNo", 95));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 80, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Party Name", "PartyName", 150));
                ReportDataGrid.Columns.Add(Col("Vehicle #", "VehicleNo", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Bags", "Bags", 65, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("In / Dr Wt (آمد)", "DrWeight", 105, TextAlignment.Right, "{0:N0}", foreground: Brushes.ForestGreen));
                ReportDataGrid.Columns.Add(Col("Out / Cr Wt (خرچ)", "CrWeight", 105, TextAlignment.Right, "{0:N0}", foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Bal Wt (بقایا)", "BalWeight", 110, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Rate", "Rate", 80, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Avg Cost", "Cost", 90, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Dr Value", "DrValue", 105, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Cr Value", "CrValue", 105, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Bal Value (مالیت)", "BalValue", 115, TextAlignment.Right, "{0:N0}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 75, TextAlignment.Center));
                break;
        }
    }

    private DataGridTextColumn Col(string header, string bindingPath, double width, TextAlignment align = TextAlignment.Left, string? stringFormat = null, bool isBold = false, Brush? foreground = null)
    {
        var col = new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(bindingPath) { StringFormat = stringFormat },
            Width = new DataGridLength(width, DataGridLengthUnitType.Pixel)
        };
        var style = new Style(typeof(TextBlock));
        style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, align));
        style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
        style.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(6, 2, 6, 2)));
        if (isBold) style.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
        if (foreground != null) style.Setters.Add(new Setter(TextBlock.ForegroundProperty, foreground));
        col.ElementStyle = style;
        return col;
    }

    private DataGridTextColumn StarCol(string header, string bindingPath, double minWidth, TextAlignment align = TextAlignment.Left, bool isBold = false)
    {
        var col = new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(bindingPath),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            MinWidth = minWidth
        };
        var style = new Style(typeof(TextBlock));
        style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, align));
        style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
        style.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(6, 2, 6, 2)));
        if (isBold) style.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
        col.ElementStyle = style;
        return col;
    }
}
