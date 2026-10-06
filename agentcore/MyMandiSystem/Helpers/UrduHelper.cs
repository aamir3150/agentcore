using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextBox = System.Windows.Controls.TextBox;
using FlowDirection = System.Windows.FlowDirection;
using FontFamily = System.Windows.Media.FontFamily;

namespace MyMandiSystem.Helpers;

/// <summary>
/// Professional Urdu support for agentcore ERP.
/// Provides native Jameel Noori Nastaleeq font rendering, RTL flow direction,
/// and automated phonetic keyboard transliteration (Pak Urdu / CRULP / InPage standard).
/// </summary>
public static class UrduHelper
{
    private static readonly Dictionary<char, char> PhoneticMap = new()
    {
        // Alif
        { 'a', 'ا' }, { 'A', 'آ' },
        // Bay / Pay
        { 'b', 'ب' }, { 'B', 'ء' },
        { 'p', 'پ' }, { 'P', 'ُ' },
        // Tay / Ttay / Say
        { 't', 'ت' }, { 'T', 'ٹ' },
        { 'C', 'ث' },
        // Jeem / Chay / Khay / Hay
        { 'j', 'ج' }, { 'J', 'ض' },
        { 'c', 'چ' },
        { 'H', 'ھ' }, { 'h', 'ہ' },
        { 'K', 'خ' }, { 'k', 'ک' },
        // Daal / Ddaal / Zaal
        { 'd', 'د' }, { 'D', 'ڈ' },
        { 'Z', 'ذ' },
        // Ray / Rray / Zay / Zhay
        { 'r', 'ر' }, { 'R', 'ڑ' },
        { 'z', 'ز' }, { 'X', 'ژ' },
        // Seen / Sheen / Suad / Zwad
        { 's', 'س' }, { 'S', 'ص' },
        { 'x', 'ش' },
        // Toay / Zoay
        { 'v', 'ط' }, { 'V', 'ظ' },
        // Ain / Ghain
        { 'e', 'ع' }, { 'G', 'غ' },
        // Fay / Qaaf / Kaaf / Gaaf
        { 'f', 'ف' }, { 'F', 'ف' },
        { 'q', 'ق' }, { 'Q', 'ق' },
        { 'g', 'گ' },
        // Laam / Meem / Noon / Noon Ghunna
        { 'l', 'ل' }, { 'L', 'ل' },
        { 'm', 'م' }, { 'M', 'ّ' }, // Tashdeed
        { 'n', 'ن' }, { 'N', 'ں' },
        // Wao
        { 'w', 'و' }, { 'W', 'ؤ' },
        { 'o', 'و' }, { 'O', 'ۃ' },
        // Choti Yay / Bari Yay
        { 'i', 'ی' }, { 'I', 'ٰ' }, // Khari Zabar
        { 'y', 'ے' }, { 'Y', 'ئ' },
        { 'u', 'ء' }, { 'U', 'ئ' },
        // Diacritics & Punctuation
        { '?', '؟' },
        { ';', '؛' },
        { ',', '،' }
    };

    public static readonly DependencyProperty IsUrduOnlyProperty =
        DependencyProperty.RegisterAttached(
            "IsUrduOnly",
            typeof(bool),
            typeof(UrduHelper),
            new PropertyMetadata(false, OnIsUrduOnlyChanged));

    public static bool GetIsUrduOnly(DependencyObject obj) => (bool)obj.GetValue(IsUrduOnlyProperty);
    public static void SetIsUrduOnly(DependencyObject obj, bool value) => obj.SetValue(IsUrduOnlyProperty, value);

    public static readonly FontFamily JameelNooriFamily = new(
        "pack://application:,,,/agentcore;component/Resources/Fonts/#Jameel Noori Nastaleeq, pack://application:,,,/Resources/Fonts/#Jameel Noori Nastaleeq, Jameel Noori Nastaleeq, Nafees Nastaleeq, Urdu Typesetting, Tahoma, Arial");

    private static void OnIsUrduOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
            {
                textBox.FlowDirection = FlowDirection.RightToLeft;
                textBox.TextAlignment = TextAlignment.Right;
                if (textBox.FontSize < 16)
                {
                    textBox.FontSize = 17;
                }

                // Set Font Family to Jameel Noori Nastaleeq
                textBox.FontFamily = JameelNooriFamily;
                textBox.Padding = new Thickness(8, 4, 8, 6);
                if (textBox.MinHeight < 38)
                {
                    textBox.MinHeight = 38;
                }

                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
            }
            else
            {
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            }
        }
    }

    private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is not TextBox textBox || string.IsNullOrEmpty(e.Text))
            return;

        // If the character is in the phonetic map, translate it
        char inputChar = e.Text[0];
        if (PhoneticMap.TryGetValue(inputChar, out char urduChar))
        {
            e.Handled = true;
            InsertChar(textBox, urduChar);
        }
    }

    private static void InsertChar(TextBox textBox, char ch)
    {
        int caret = textBox.CaretIndex;
        int selectionLength = textBox.SelectionLength;

        string currentText = textBox.Text ?? string.Empty;
        if (selectionLength > 0 && caret >= 0 && caret + selectionLength <= currentText.Length)
        {
            currentText = currentText.Remove(caret, selectionLength);
        }

        if (caret < 0) caret = 0;
        if (caret > currentText.Length) caret = currentText.Length;

        textBox.Text = currentText.Insert(caret, ch.ToString());
        textBox.CaretIndex = caret + 1;

        var binding = textBox.GetBindingExpression(TextBox.TextProperty);
        binding?.UpdateSource();
    }
}
