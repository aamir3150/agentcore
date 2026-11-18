using System.Windows;

namespace MyMandiSystem.Views;

public partial class GeneralMultiInvoiceWindow : Window
{
    public GeneralMultiInvoiceWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
