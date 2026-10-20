using System;
using System.Collections.Generic;
using System.Text;

namespace MyMandiSystem.Helpers;

public static class ErrorTranslator
{
    public static ErrorExplanation Translate(Exception ex, string? contextTitle = null)
    {
        var rawText = BuildFullTechnicalMessage(ex);
        var explanation = new ErrorExplanation
        {
            TechnicalError = rawText
        };

        var lowerRaw = rawText.ToLowerInvariant();

        // 1. FOREIGN KEY: FK_Products_Companies
        if (lowerRaw.Contains("fk_products_companies"))
        {
            explanation.Headline = "Company Select Karna Zaroori Hai";
            explanation.UrduHeadline = "پروڈکٹ کی کمپنی منتخب کرنا لازمی ہے";
            explanation.Category = "Missing Company Selection";
            explanation.IconKind = "DomainAlert";
            explanation.AccentColor = "#F59E0B"; // Amber
            explanation.WhatHappened = "Aap jo product/item save kar rahe hain, us ke sath Company select nahi hui ya Company database mein mojood nahi hai.";
            explanation.SolutionSteps = new List<string>
            {
                "Product form mein 'Company' ka dropdown kholen aur ek company select karein.",
                "Agar company ka dropdown khali hai, to pehle 'Catalog' menu se company add karein ya software restart karein.",
                "Company select karne ke baad dobara 'Save Product' par click karein."
            };
            return explanation;
        }

        // 2. FOREIGN KEY: FK_Products_Units
        if (lowerRaw.Contains("fk_products_units"))
        {
            explanation.Headline = "Unit (بوری / کلو / من) Select Karna Zaroori Hai";
            explanation.UrduHeadline = "پروڈکٹ کا یونٹ منتخب کرنا لازمی ہے";
            explanation.Category = "Missing Unit Selection";
            explanation.IconKind = "Weight";
            explanation.AccentColor = "#F59E0B";
            explanation.WhatHappened = "Aap ne product ka Unit (maslan Bori, Kilo ya Mann) select nahi kiya.";
            explanation.SolutionSteps = new List<string>
            {
                "Product form mein 'Unit' ka dropdown kholen.",
                "Munasib unit (Bag / KG / Mann) select karein.",
                "Phir 'Save Product' button dabayein."
            };
            return explanation;
        }

        // 3. FOREIGN KEY: FK_Vouchers_FinancialYears
        if (lowerRaw.Contains("fk_vouchers_financialyears"))
        {
            explanation.Headline = "Maliyati Saal (Financial Year) Missing Hai";
            explanation.UrduHeadline = "مالیاتی سال کی معلومات درست نہیں ہیں";
            explanation.Category = "Financial Year Missing";
            explanation.IconKind = "CalendarAlert";
            explanation.AccentColor = "#F59E0B";
            explanation.WhatHappened = "Voucher save karte waqt mojooda maliyati saal (Financial Year) database se connect nahi ho saka.";
            explanation.SolutionSteps = new List<string>
            {
                "Top header par check karein ke Financial Year (maslan 2025-26) show ho raha hai.",
                "Software ko band kar ke dobara chala lein.",
                "Dobara Save button dabayein."
            };
            return explanation;
        }

        // 4. FOREIGN KEY: FK_Vouchers_CropSeasons
        if (lowerRaw.Contains("fk_vouchers_cropseasons"))
        {
            explanation.Headline = "Fasli Season (Crop Season) Select Karein";
            explanation.UrduHeadline = "فصلی سیزن منتخب کرنا ضروری ہے";
            explanation.Category = "Crop Season Missing";
            explanation.IconKind = "CalendarAlert";
            explanation.AccentColor = "#F59E0B";
            explanation.WhatHappened = "Voucher form mein 'Crop Season' select nahi hua ya database mein mojood nahi.";
            explanation.SolutionSteps = new List<string>
            {
                "Form ke top par 'Crop Season' (jaise Kharif 2025) ka dropdown kholen aur season muntakhab karein.",
                "Dobara Save button dabayein."
            };
            return explanation;
        }

        // 5. ANY OTHER FOREIGN KEY
        if (lowerRaw.Contains("foreign key constraint") || lowerRaw.Contains("the insert statement conflicted with the foreign key") || lowerRaw.Contains("the delete statement conflicted with the reference"))
        {
            explanation.Headline = "Jurri Hui Maloomat Missing Hai (Linked Record Missing)";
            explanation.UrduHeadline = "منسلک ریکارڈ موجود نہیں یا استعمال میں ہے";
            explanation.Category = "Linked Data Constraint";
            explanation.IconKind = "LinkVariantAlert";
            explanation.AccentColor = "#F59E0B";
            
            if (lowerRaw.Contains("delete statement conflicted"))
            {
                explanation.WhatHappened = "Aap jis record ko delete karne ki koshish kar rahe hain, wo pehle se kisi doosre voucher, ledger ya invoice mein istemal ho chuka hai. Is liye isay direct delete nahi kiya ja sakta.";
                explanation.SolutionSteps = new List<string>
                {
                    "Pehle is record se jurray huay purane vouchers ya invoices ko delete ya update karein.",
                    "Ya phir is record ko delete karne ki bajaye 'Inactive / Non-Active' kar dein."
                };
            }
            else
            {
                explanation.WhatHappened = "Aap jo record save kar rahe hain, us ke sath jurra koi lazmi dropdown (jaise Account, Party, Town, ya Group) select nahi hua.";
                explanation.SolutionSteps = new List<string>
                {
                    "Form mein tamam dropdowns check karein ke koi lazmi khana khali to nahi hai.",
                    "Yaqeen banayein ke Account aur Party theek se muntakhab hain.",
                    "Dobara Save button dabayein."
                };
            }
            return explanation;
        }

        // 4. DUPLICATE KEY / UNIQUE CONSTRAINT
        if (lowerRaw.Contains("cannot insert duplicate key") || lowerRaw.Contains("unique index") || lowerRaw.Contains("primary key constraint") || lowerRaw.Contains("duplicate"))
        {
            explanation.Headline = "Yeh Code Ya Record Pehle Se Mojood Hai (Duplicate Entry)";
            explanation.UrduHeadline = "یہ کوڈ یا نمبر پہلے سے سسٹم میں موجود ہے";
            explanation.Category = "Duplicate Record";
            explanation.IconKind = "ContentCopy";
            explanation.AccentColor = "#E11D48"; // Rose
            explanation.WhatHappened = "Aap jo Code, Invoice No, ya Naam likh rahe hain, wo pehle se kisi doosre record ke liye save ho chuka hai.";
            explanation.SolutionSteps = new List<string>
            {
                "Mukhtalif Code ya naya number darj karein.",
                "Agar Party ya Product ka code hai, to auto-number check karein.",
                "Tabdeeli ke baad dobara Save karein."
            };
            return explanation;
        }

        // 5. DATE TIME RANGE / UN-REPRESENTABLE
        if (lowerRaw.Contains("un-representable datetime") || lowerRaw.Contains("datetime overflow") || lowerRaw.Contains("sqldatetime overflow") || lowerRaw.Contains("out of range"))
        {
            explanation.Headline = "Tareekh (Date) Durust Nahi Hai";
            explanation.UrduHeadline = "تاریخ کی حد کیلنڈر سے باہر ہے";
            explanation.Category = "Invalid Date Range";
            explanation.IconKind = "CalendarAlert";
            explanation.AccentColor = "#E11D48";
            explanation.WhatHappened = "Tareekh ki value ghalat ya calendar ki hadood se bahar hai (maslan bohot purani ya bohot door ki tareekh).";
            explanation.SolutionSteps = new List<string>
            {
                "Tareekh ke box (DatePicker) mein mojooda saal ya maheenay ki durust tareekh select karein.",
                "Agar report mein 'All Dates' ka option hai to dobara click kar ke koshish karein.",
                "Reporting dobara Preview karein."
            };
            return explanation;
        }

        // 6. SQL SERVER CONNECTION / NETWORK FAILURE
        if (lowerRaw.Contains("cannot open database") || lowerRaw.Contains("a network-related or instance-specific error") || lowerRaw.Contains("server was not found") || lowerRaw.Contains("login failed") || lowerRaw.Contains("connection refused"))
        {
            explanation.Headline = "Database Server Se Rabta Nahi Ho Saka";
            explanation.UrduHeadline = "ڈیٹا بیس سرور سے رابطہ منقطع ہے";
            explanation.Category = "Database Connection";
            explanation.IconKind = "DatabaseAlert";
            explanation.AccentColor = "#DC2626"; // Red
            explanation.WhatHappened = "Software ka SQL Server se rabta nahi ho pa raha. Server band ho sakta hai ya network cable utri ho sakti hai.";
            explanation.SolutionSteps = new List<string>
            {
                "Windows Services khol kar 'SQL Server (SQLEXPRESS)' ko check karein ke 'Running' ho.",
                "Agar aap network/client computer par hain to LAN wire ya Wi-Fi check karein.",
                "Software ko band kar ke dobara start karein."
            };
            return explanation;
        }

        // 7. FILE LOCKED / ACCESS DENIED
        if (lowerRaw.Contains("the process cannot access the file") || lowerRaw.Contains("because it is being used by another process") || lowerRaw.Contains("access to the path is denied"))
        {
            explanation.Headline = "File Doosre Program Mein Khuli Hui Hai (File Locked)";
            explanation.UrduHeadline = "فائل کسی دوسرے پروگرام میں کھلی ہے";
            explanation.Category = "File In Use";
            explanation.IconKind = "FileLockOutline";
            explanation.AccentColor = "#F59E0B";
            explanation.WhatHappened = "Jis file par software kaam karne laga tha, wo pehle se kisi doosre program (maslan Excel, PDF Viewer, ya Task Manager) mein khuli hui hai.";
            explanation.SolutionSteps = new List<string>
            {
                "Pehle se khule huay Excel, PDF ya report documents ko band karein.",
                "Agar software ka doosra instance chal raha hai to usay band karein.",
                "Dobara koshish karein."
            };
            return explanation;
        }

        // 8. REPORT VIEWER / APPDOMAIN COMPATIBILITY
        if (lowerRaw.Contains("reportviewer") || lowerRaw.Contains("activationarguments") || lowerRaw.Contains("microsoft.reporting"))
        {
            explanation.Headline = "Print / Report Module Mein Masla";
            explanation.UrduHeadline = "پرنٹ یا رپورٹ پری ویو میں مسئلہ پیش آیا";
            explanation.Category = "Reporting Engine";
            explanation.IconKind = "PrinterAlert";
            explanation.AccentColor = "#F59E0B";
            explanation.WhatHappened = "Report viewer ya print module initialize hone mein temporary masla pesh aaya hai.";
            explanation.SolutionSteps = new List<string>
            {
                "Software ko band kar ke dobara chalayein.",
                "Agar Ledger ya Voucher print hai to 'Print' ya 'Preview' dobara dabayein.",
                "Agar masla barqarar rahe to neechay diye gaye Support Number par rabta karein."
            };
            return explanation;
        }

        // 9. DEFAULT / GENERAL FALLBACK
        explanation.Headline = !string.IsNullOrWhiteSpace(contextTitle) ? $"{contextTitle} Mein Masla Aaya" : "Amal Mukammal Nahi Ho Saka";
        explanation.UrduHeadline = "کارروائی مکمل نہیں ہو سکی";
        explanation.Category = "General Error";
        explanation.IconKind = "AlertCircleOutline";
        explanation.AccentColor = "#E11D48";
        explanation.WhatHappened = CleanSimpleMessage(ex.Message);
        explanation.SolutionSteps = new List<string>
        {
            "Form mein darj shuda data (Naam, Raqam, Tareekh) dobara check karein.",
            "Form ko refresh ya dobara open kar ke try karein.",
            "Agar masla barqarar rahe to 'Copy Error Details' button daba kar support ko bheinjein."
        };

        return explanation;
    }

    private static string CleanSimpleMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return "Ghair mutawaqqe rukawat pesh aayi hai.";
        
        // Remove raw technical noise if present
        if (message.Contains("See the inner exception for details"))
            message = message.Replace("See the inner exception for details.", "").Trim();

        return message;
    }

    private static string BuildFullTechnicalMessage(Exception ex)
    {
        var sb = new StringBuilder();
        var current = ex;
        int depth = 1;
        while (current != null)
        {
            sb.AppendLine($"[Level {depth}] {current.GetType().FullName}: {current.Message}");
            if (!string.IsNullOrWhiteSpace(current.StackTrace))
            {
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(current.StackTrace);
            }
            sb.AppendLine(new string('-', 50));
            current = current.InnerException;
            depth++;
        }
        return sb.ToString().TrimEnd();
    }
}
