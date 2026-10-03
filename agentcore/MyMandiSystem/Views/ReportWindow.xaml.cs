using System.Collections.Generic;
using System.Windows;
using Microsoft.Reporting.WinForms;

namespace MyMandiSystem.Views;

public partial class ReportWindow : Window
{
    public ReportWindow(string reportPath, string dataSourceName, object data, Dictionary<string, string>? parameters = null)
    {
        InitializeComponent();
        
        Viewer.LocalReport.ReportPath = reportPath;
        Viewer.LocalReport.DataSources.Clear();
        Viewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName, data));
        
        if (parameters != null)
        {
            var pList = new List<ReportParameter>();
            foreach (var p in parameters)
            {
                pList.Add(new ReportParameter(p.Key, p.Value));
            }
            Viewer.LocalReport.SetParameters(pList);
        }
        
        Viewer.RefreshReport();
    }
}
