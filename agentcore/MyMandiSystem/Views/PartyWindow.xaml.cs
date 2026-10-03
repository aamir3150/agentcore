using System.Windows;
using System.Windows.Controls;

namespace MyMandiSystem.Views;

public partial class PartyWindow : Window
{
    public PartyWindow(int defaultTabIndex = 0)
    {
        InitializeComponent();
        this.Loaded += (s, e) => 
        {
            var tabControl = FindChild<System.Windows.Controls.TabControl>(PartyViewControl);
            if (tabControl != null)
            {
                tabControl.SelectedIndex = defaultTabIndex;
            }
        };
    }

    private T FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child != null && child is T) return (T)child;
            else
            {
                var result = FindChild<T>(child);
                if (result != null) return result;
            }
        }
        return null;
    }
}
