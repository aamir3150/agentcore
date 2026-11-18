using System.Windows;

namespace MyMandiSystem.Views;

public partial class GeneralSaleInvoiceWindow : Window
{
    public GeneralSaleInvoiceWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
