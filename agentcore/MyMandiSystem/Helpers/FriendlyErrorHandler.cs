using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using MyMandiSystem.Views;

namespace MyMandiSystem.Helpers;

public static class FriendlyErrorHandler
{
    public static void ShowError(Exception ex, string? contextTitle = null, Window? owner = null)
    {
        if (ex == null) return;

        // Log technical stack trace to disk for offline diagnostics
        LogToFile(ex, contextTitle);

        var explanation = ErrorTranslator.Translate(ex, contextTitle);

        ShowDialogInternal(explanation, owner);
    }

    public static void ShowCustom(
        string headline, 
        string urduHeadline, 
        string whatHappened, 
        List<string> solutionSteps, 
        string category = "System Alert",
        string? technicalDetails = null,
        Window? owner = null)
    {
        var explanation = new ErrorExplanation
        {
            Headline = headline,
            UrduHeadline = urduHeadline,
            Category = category,
            WhatHappened = whatHappened,
            SolutionSteps = solutionSteps ?? new List<string>(),
            TechnicalError = technicalDetails ?? string.Empty
        };

        ShowDialogInternal(explanation, owner);
    }

    private static void ShowDialogInternal(ErrorExplanation explanation, Window? owner)
    {
        var app = System.Windows.Application.Current;
        if (app == null) return;

        Action action = () =>
        {
            try
            {
                var dialog = new FriendlyErrorWindow(explanation);
                
                var targetOwner = owner ?? app.MainWindow;
                if (targetOwner != null && targetOwner.IsLoaded && targetOwner != dialog)
                {
                    dialog.Owner = targetOwner;
                }

                dialog.ShowDialog();
            }
            catch (Exception fallbackEx)
            {
                // Fallback to standard MessageBox if custom window fails to render
                System.Windows.MessageBox.Show(
                    $"{explanation.Headline}\n\n{explanation.WhatHappened}\n\nRahnumai:\n{string.Join("\n", explanation.SolutionSteps)}\n\nFallback: {fallbackEx.Message}", 
                    explanation.Category, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }
        };

        if (app.Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            app.Dispatcher.Invoke(action);
        }
    }

    private static void LogToFile(Exception ex, string? contextTitle)
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "application_errors.log");
            var content = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [Context: {contextTitle ?? "N/A"}]\n{ex}\n{new string('=', 60)}\n";
            File.AppendAllText(logPath, content);
        }
        catch
        {
            // Do not fail if logging fails
        }
    }
}
