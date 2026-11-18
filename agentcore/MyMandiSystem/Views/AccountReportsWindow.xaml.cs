using MyMandiSystem.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MyMandiSystem.Views;

public partial class AccountReportsWindow : Window
{
    public AccountReportsWindow()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            if (DataContext is AccountReportsViewModel vm)
            {
                vm.RequestClose = () => this.Close();
                SetupColumnsForMode(vm.Mode);

                vm.PropertyChanged += (ps, pe) =>
                {
                    if (pe.PropertyName == nameof(AccountReportsViewModel.Mode))
                    {
                        Dispatcher.Invoke(() => SetupColumnsForMode(vm.Mode));
                    }
                };
            }
        };

        DataContextChanged += (s, e) =>
        {
            if (e.NewValue is AccountReportsViewModel vm)
            {
                vm.RequestClose = () => this.Close();
                SetupColumnsForMode(vm.Mode);

                vm.PropertyChanged += (ps, pe) =>
                {
                    if (pe.PropertyName == nameof(AccountReportsViewModel.Mode))
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
                if (DataContext is AccountReportsViewModel vm)
                {
                    _ = vm.LoadReportCommand.ExecuteAsync(null);
                }
            }
        };
    }

    private void SetupColumnsForMode(AccountReportMode mode)
    {
        ReportDataGrid.Columns.Clear();

        switch (mode)
        {
            case AccountReportMode.AccountLedger:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Voucher #", "DocNo", 95));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 75, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(StarCol("Narration", "Narration", 180));
                ReportDataGrid.Columns.Add(Col("Cheque #", "ChequeNo", 90));
                ReportDataGrid.Columns.Add(Col("Debit", "Debit", 110, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Credit", "Credit", 110, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Balance", "Balance", 125, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Dr/Cr", "Status", 65, TextAlignment.Center, isBold: true));
                break;

            case AccountReportMode.AccountLedgerExtended:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Voucher #", "DocNo", 90));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 70, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(Col("Account Name", "AccountName", 140));
                ReportDataGrid.Columns.Add(StarCol("Narration", "Narration", 160));
                ReportDataGrid.Columns.Add(Col("Cheque #", "ChequeNo", 85));
                ReportDataGrid.Columns.Add(Col("Debit", "Debit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Credit", "Credit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Balance", "Balance", 120, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Dr/Cr", "Status", 65, TextAlignment.Center, isBold: true));
                break;

            case AccountReportMode.PartyLedgerExtended:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Doc #", "DocNo", 90));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 70, TextAlignment.Center));
                ReportDataGrid.Columns.Add(StarCol("Item / Description", "Narration", 140));
                ReportDataGrid.Columns.Add(Col("Bags / Qty", "Quantity", 80, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Weight (Kg)", "Weight", 90, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Rate", "Rate", 80, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Debit", "Debit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Credit", "Credit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Balance", "Balance", 120, TextAlignment.Right, "{0:N2}", isBold: true));
                break;

            case AccountReportMode.CashBook:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Voucher #", "DocNo", 90));
                ReportDataGrid.Columns.Add(Col("A/C #", "AccountNo", 75));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 170, isBold: true));
                ReportDataGrid.Columns.Add(Col("Narration", "Narration", 140));
                ReportDataGrid.Columns.Add(Col("Cheque #", "ChequeNo", 85));
                ReportDataGrid.Columns.Add(Col("Receipt (Debit)", "Debit", 115, TextAlignment.Right, "{0:N2}", foreground: Brushes.ForestGreen));
                ReportDataGrid.Columns.Add(Col("Payment (Credit)", "Credit", 115, TextAlignment.Right, "{0:N2}", foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Cash Balance", "Balance", 125, TextAlignment.Right, "{0:N2}", isBold: true));
                break;

            case AccountReportMode.DayBook:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Voucher #", "DocNo", 90));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 70, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(Col("A/C #", "AccountNo", 75));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 160, isBold: true));
                ReportDataGrid.Columns.Add(Col("Narration", "Narration", 130));
                ReportDataGrid.Columns.Add(Col("Debit", "Debit", 110, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Credit", "Credit", 110, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Balance", "Balance", 120, TextAlignment.Right, "{0:N2}", isBold: true));
                break;

            case AccountReportMode.DailyVouchersDetail:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Voucher #", "DocNo", 95));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Type", "DocType", 75, TextAlignment.Center, isBold: true));
                ReportDataGrid.Columns.Add(Col("A/C #", "AccountNo", 80));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Narration", "Narration", 160));
                ReportDataGrid.Columns.Add(Col("Debit", "Debit", 115, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Credit", "Credit", 115, TextAlignment.Right, "{0:N2}"));
                break;

            case AccountReportMode.AccountsReceivable:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Account #", "AccountNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Customer / Party Name", "AccountName", 200, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "UrduName", 130));
                ReportDataGrid.Columns.Add(Col("Opening Bal", "Quantity", 115, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Total Debit", "Debit", 115, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Total Credit", "Credit", 115, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Receivable", "Balance", 135, TextAlignment.Right, "{0:N2}", isBold: true, foreground: Brushes.ForestGreen));
                break;

            case AccountReportMode.AccountsPayable:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Account #", "AccountNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Vendor / Party Name", "AccountName", 200, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "UrduName", 130));
                ReportDataGrid.Columns.Add(Col("Opening Bal", "Quantity", 115, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Total Debit", "Debit", 115, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Total Credit", "Credit", 115, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Payable", "Balance", 135, TextAlignment.Right, "{0:N2}", isBold: true, foreground: Brushes.Crimson));
                break;

            case AccountReportMode.GLJournal:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("JV #", "DocNo", 95));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 90, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("A/C #", "AccountNo", 85));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Narration", "Narration", 160));
                ReportDataGrid.Columns.Add(Col("Debit", "Debit", 120, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Credit", "Credit", 120, TextAlignment.Right, "{0:N2}"));
                break;

            case AccountReportMode.TrialBalance:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("A/C #", "AccountNo", 85));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "UrduName", 120));
                ReportDataGrid.Columns.Add(Col("Opening Dr", "Quantity", 100, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Opening Cr", "Weight", 100, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Period Dr", "Debit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Period Cr", "Credit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Closing Dr", "Rate", 115, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Closing Cr", "Balance", 115, TextAlignment.Right, "{0:N2}", isBold: true));
                break;

            case AccountReportMode.AccountsBalances:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Account #", "AccountNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 210, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "UrduName", 130));
                ReportDataGrid.Columns.Add(Col("Account Type", "DocType", 110));
                ReportDataGrid.Columns.Add(Col("Closing Balance", "Balance", 135, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Dr / Cr", "Status", 75, TextAlignment.Center, isBold: true));
                break;

            case AccountReportMode.BalanceSheet:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Classification", "DocType", 120, isBold: true));
                ReportDataGrid.Columns.Add(Col("Account #", "AccountNo", 95));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 220, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "UrduName", 130));
                ReportDataGrid.Columns.Add(Col("Net Balance (Rs)", "Balance", 145, TextAlignment.Right, "{0:N2}", isBold: true));
                break;

            case AccountReportMode.BankStatement:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Doc #", "DocNo", 85));
                ReportDataGrid.Columns.Add(Col("Cheque #", "ChequeNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Account / Beneficiary", "AccountName", 160, isBold: true));
                ReportDataGrid.Columns.Add(Col("Narration", "Narration", 130));
                ReportDataGrid.Columns.Add(Col("Deposit", "Debit", 110, TextAlignment.Right, "{0:N2}", foreground: Brushes.ForestGreen));
                ReportDataGrid.Columns.Add(Col("Withdrawal", "Credit", 110, TextAlignment.Right, "{0:N2}", foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Bank Balance", "Balance", 125, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 85, TextAlignment.Center));
                break;

            case AccountReportMode.BankPosition:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Bank A/C #", "AccountNo", 100));
                ReportDataGrid.Columns.Add(StarCol("Bank Name", "AccountName", 200, isBold: true));
                ReportDataGrid.Columns.Add(Col("Ledger Balance", "Debit", 130, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Uncleared Cheques", "Credit", 130, TextAlignment.Right, "{0:N2}", foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("Available Position", "Balance", 140, TextAlignment.Right, "{0:N2}", isBold: true, foreground: Brushes.Navy));
                break;

            case AccountReportMode.BankMarkupCalculator:
            case AccountReportMode.PartyMarkupCalculator:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Doc #", "DocNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Narration", "Narration", 160));
                ReportDataGrid.Columns.Add(Col("Debit", "Debit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Credit", "Credit", 105, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Balance", "Balance", 120, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Days", "Days", 80, TextAlignment.Right, "{0:N0}"));
                ReportDataGrid.Columns.Add(Col("Markup Amount", "MarkupAmount", 130, TextAlignment.Right, "{0:N2}", isBold: true, foreground: Brushes.Crimson));
                break;

            case AccountReportMode.ProfitLossStatement:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Category", "Category", 120, isBold: true));
                ReportDataGrid.Columns.Add(Col("Account #", "AccountNo", 95));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 220, isBold: true));
                ReportDataGrid.Columns.Add(Col("Total Amount", "Balance", 145, TextAlignment.Right, "{0:N2}", isBold: true));
                break;

            case AccountReportMode.MonthlyExpenseChart:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Month / Year", "Date", 100, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Category", "Category", 120));
                ReportDataGrid.Columns.Add(StarCol("Expense Account", "AccountName", 220, isBold: true));
                ReportDataGrid.Columns.Add(Col("Expense Amount", "Debit", 140, TextAlignment.Right, "{0:N2}", isBold: true, foreground: Brushes.Crimson));
                break;

            case AccountReportMode.BankChequesRegister:
            case AccountReportMode.UnclearedCheques:
            case AccountReportMode.LostCheques:
            case AccountReportMode.BouncedCheques:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Voucher #", "DocNo", 90));
                ReportDataGrid.Columns.Add(Col("Voucher Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Cheque #", "ChequeNo", 95, isBold: true));
                ReportDataGrid.Columns.Add(Col("Cheque Date", "Category", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Bank A/C", "AccountNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Party / Person Name", "AccountName", 160, isBold: true));
                ReportDataGrid.Columns.Add(Col("Amount", "Debit", 115, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Received / Deposited By", "Narration", 130));
                ReportDataGrid.Columns.Add(Col("Status", "Status", 95, TextAlignment.Center, isBold: true));
                break;

            case AccountReportMode.OpeningBalances:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Account #", "AccountNo", 90));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 200, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "UrduName", 130));
                ReportDataGrid.Columns.Add(Col("Account Type", "DocType", 110));
                ReportDataGrid.Columns.Add(Col("Opening Debit", "Debit", 120, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Opening Credit", "Credit", 120, TextAlignment.Right, "{0:N2}"));
                ReportDataGrid.Columns.Add(Col("Net Balance", "Balance", 130, TextAlignment.Right, "{0:N2}", isBold: true));
                break;

            case AccountReportMode.ChartOfAccounts:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Account #", "AccountNo", 95, isBold: true));
                ReportDataGrid.Columns.Add(StarCol("Account Title", "AccountName", 220, isBold: true));
                ReportDataGrid.Columns.Add(Col("Urdu Name", "UrduName", 140));
                ReportDataGrid.Columns.Add(Col("Depth Level", "DocType", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Account Type", "Category", 110));
                ReportDataGrid.Columns.Add(Col("Detailed?", "Status", 80, TextAlignment.Center));
                break;

            case AccountReportMode.InsuranceRegister:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Policy #", "DocNo", 95, isBold: true));
                ReportDataGrid.Columns.Add(StarCol("Insurance Company", "AccountName", 180, isBold: true));
                ReportDataGrid.Columns.Add(Col("Cover Note", "ChequeNo", 90));
                ReportDataGrid.Columns.Add(Col("Category", "Category", 100));
                ReportDataGrid.Columns.Add(Col("Insured Value", "Debit", 120, TextAlignment.Right, "{0:N2}", isBold: true));
                ReportDataGrid.Columns.Add(Col("Net Premium", "Credit", 110, TextAlignment.Right, "{0:N2}", foreground: Brushes.Crimson));
                ReportDataGrid.Columns.Add(Col("From Date", "Date", 85, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("To Date", "Status", 85, TextAlignment.Center));
                break;

            case AccountReportMode.UserLogRegister:
                ReportDataGrid.Columns.Add(Col("#", "SrNo", 45, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Timestamp", "Date", 135, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("User ID", "DocNo", 75, TextAlignment.Center));
                ReportDataGrid.Columns.Add(Col("Username", "AccountName", 130, isBold: true));
                ReportDataGrid.Columns.Add(StarCol("Activity Description", "Narration", 260));
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
