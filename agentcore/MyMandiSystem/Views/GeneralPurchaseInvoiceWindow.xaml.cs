using System.Windows;

namespace MyMandiSystem.Views;

public partial class GeneralPurchaseInvoiceWindow : Window
{
    public GeneralPurchaseInvoiceWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
