using MyMandiSystem.ViewModels;
using System.Windows;

namespace MyMandiSystem.Views;

public partial class PestroSaleManualBatchWindow : Window
{
    public PestroSaleManualBatchWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is PestroSaleManualBatchViewModel vm)
            {
                vm.RequestClose = () => Close();
            }
        };
    }
}
