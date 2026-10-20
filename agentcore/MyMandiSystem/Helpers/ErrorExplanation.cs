using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMandiSystem.Helpers;

public class SolutionStepItem
{
    public int Number { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class ErrorExplanation
{
    public string Headline { get; set; } = "Amal Mukammal Nahi Ho Saka";
    public string UrduHeadline { get; set; } = "کارروائی مکمل نہیں ہو سکی";
    public string Category { get; set; } = "System Notification";
    public string WhatHappened { get; set; } = string.Empty;
    public List<string> SolutionSteps { get; set; } = new();
    public string TechnicalError { get; set; } = string.Empty;
    public string IconKind { get; set; } = "AlertCircleOutline";
    public string AccentColor { get; set; } = "#E11D48"; // Rose / Amber / Emerald
    public string SupportPhone { get; set; } = "0344-7436314 (City Computers Marot)";

    public IEnumerable<SolutionStepItem> FormattedSteps =>
        SolutionSteps.Select((step, idx) => new SolutionStepItem { Number = idx + 1, Text = step });

    public string FullDiagnosticText
    {
        get
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("        agentcore ERP System Alert       ");
            sb.AppendLine("========================================");
            sb.AppendLine($"Title: {Headline}");
            sb.AppendLine($"Urdu:  {UrduHeadline}");
            sb.AppendLine($"Category: {Category}");
            sb.AppendLine($"Time:  {DateTime.Now:dd-MMM-yyyy hh:mm:ss tt}");
            sb.AppendLine($"Support: {SupportPhone}");
            sb.AppendLine();
            sb.AppendLine("--- Masla (What Happened) ---");
            sb.AppendLine(WhatHappened);
            sb.AppendLine();
            sb.AppendLine("--- Hal Ki Rahnumai (Guide / Steps to Solve) ---");
            for (int i = 0; i < SolutionSteps.Count; i++)
            {
                sb.AppendLine($" {i + 1}. {SolutionSteps[i]}");
            }
            sb.AppendLine();
            sb.AppendLine("--- Technical Details (Developer Diagnostics) ---");
            sb.AppendLine(TechnicalError);
            sb.AppendLine("========================================");
            return sb.ToString();
        }
    }
}
