import re

# 1. Clean GeneralPurchaseInvoiceViewModel.cs
path1 = r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralPurchaseInvoiceViewModel.cs'
with open(path1, 'r', encoding='utf-8') as f:
    c1 = f.read()

c1 = c1.replace('MessageBox.Show("براہ کرم وینڈر / سپلائر کا انتخاب کریں۔", "لازمی فیلڈ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please select a vendor / supplier first.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);')
c1 = c1.replace('MessageBox.Show("انوائس میں کم از کم ایک آئٹم کا ہونا لازمی ہے۔", "کوئی ریکارڈ نہیں", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please add at least one line item to the invoice.", "No Records", MessageBoxButton.OK, MessageBoxImage.Warning);')
c1 = c1.replace('string modeUrdu = IsReturnMode ? "واپسی خریداری (Return)" : "خریداری (Purchase)";',
                'string mode = IsReturnMode ? "Purchase Return" : "Purchase";')
c1 = c1.replace('MessageBox.Show($"جنرل {modeUrdu} انوائس (#{invoice.InvoiceNo}) کامیابی سے محفوظ ہو گئی۔\\nاسٹاک (CurrentStock) اور کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔", \n                "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"General {mode} Invoice (#{invoice.InvoiceNo}) saved successfully!\\nInventory and General Ledger updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c1 = c1.replace('MessageBox.Show($"جنرل {modeUrdu} انوائس (#{invoice.InvoiceNo}) کامیابی سے محفوظ ہو گئی۔\\nاسٹاک (CurrentStock) اور کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔",\n                "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"General {mode} Invoice (#{invoice.InvoiceNo}) saved successfully!\\nInventory and General Ledger updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c1 = c1.replace('MessageBox.Show($"انوائس محفوظ کرنے میں مسئلہ آیا:\\n{ex.Message}", "خرابی", MessageBoxButton.OK, MessageBoxImage.Error);',
                'MessageBox.Show($"Failed to save invoice:\\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);')
c1 = c1.replace('MessageBox.Show("براہ کرم پہلے سے محفوظ شدہ انوائس اوپن کریں۔", "توجہ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please open an existing saved invoice first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c1 = c1.replace('if (MessageBox.Show($"کیا آپ واقعی انوائس #{PurchaseId} کو حذف کرنا چاہتے ہیں؟\\nاس سے اسٹاک اور کھاتہ جات خودکار ریورس ہو جائیں گے۔",\n            "تصدیق", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)',
                'if (MessageBox.Show($"Are you sure you want to delete invoice #{PurchaseId}?\\nThis will automatically reverse inventory and ledger postings.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)')
c1 = c1.replace('MessageBox.Show("انوائس کامیابی سے ڈیلیٹ ہو گئی۔", "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show("Invoice deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c1 = c1.replace('MessageBox.Show($"ڈیلیٹ کرنے میں مسئلہ آیا:\\n{ex.Message}", "خرابی", MessageBoxButton.OK, MessageBoxImage.Error);',
                'MessageBox.Show($"Failed to delete invoice:\\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);')
c1 = c1.replace('MessageBox.Show("پرنٹ کرنے کے لیے انوائس میں ڈیٹا کا ہونا لازمی ہے۔", "کوئی ریکارڈ نہیں", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("No line items found to print.", "Print Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c1 = c1.replace('MessageBox.Show($"پرنٹنگ انوائس #{PurchaseId}...", "پرنٹ", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"Printing invoice #{PurchaseId}...", "Print Preview", MessageBoxButton.OK, MessageBoxImage.Information);')

with open(path1, 'w', encoding='utf-8') as f:
    f.write(c1)

# 2. Clean GeneralSaleInvoiceViewModel.cs
path2 = r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralSaleInvoiceViewModel.cs'
with open(path2, 'r', encoding='utf-8') as f:
    c2 = f.read()

c2 = c2.replace('[ObservableProperty] private string _windowTitle = "GENERAL SALE INVOICE (جنرل سیل انوائس)";',
                '[ObservableProperty] private string _windowTitle = "GENERAL SALE INVOICE";')
c2 = c2.replace('? "SALE RETURN INVOICE (واپسی فروخت - کریڈٹ نوٹ)" \n            : "GENERAL SALE INVOICE (جنرل سیل انوائس)";',
                '? "SALE RETURN INVOICE (CREDIT NOTE)" \n            : "GENERAL SALE INVOICE";')
c2 = c2.replace('? "SALE RETURN INVOICE (واپسی فروخت - کریڈٹ نوٹ)"\n            : "GENERAL SALE INVOICE (جنرل سیل انوائس)";',
                '? "SALE RETURN INVOICE (CREDIT NOTE)"\n            : "GENERAL SALE INVOICE";')
c2 = c2.replace('MessageBox.Show("براہ کرم پہلے پروڈکٹ منتخب کریں۔", "توجہ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please select a product first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c2 = c2.replace('MessageBox.Show("تعداد (Quantity) صفر سے زیادہ ہونی چاہیے۔", "توجہ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Quantity must be greater than zero.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c2 = c2.replace('if (MessageBox.Show($"فروخت کی جانے والی تعداد ({CurrentQty + CurrentBonus}) دستیاب اسٹاک ({AvailableStock}) سے زیادہ ہے۔\\nکیا آپ پھر بھی جاری رکھنا چاہتے ہیں؟",\n            "اسٹاک وارننگ", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)',
                'if (MessageBox.Show($"Sale quantity ({CurrentQty + CurrentBonus}) exceeds available stock ({AvailableStock}).\\nDo you still want to proceed?", "Stock Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)')
c2 = c2.replace('MessageBox.Show("براہ کرم گاہک / کسٹمر کا انتخاب کریں۔", "لازمی فیلڈ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please select a customer first.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);')
c2 = c2.replace('MessageBox.Show("انوائس میں کم از کم ایک آئٹم کا ہونا لازمی ہے۔", "کوئی ریکارڈ نہیں", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please add at least one line item to the invoice.", "No Records", MessageBoxButton.OK, MessageBoxImage.Warning);')
c2 = c2.replace('string modeUrdu = IsReturnMode ? "واپسی فروخت (Sale Return)" : "فروخت (Sale)";',
                'string mode = IsReturnMode ? "Sale Return" : "Sale";')
c2 = c2.replace('MessageBox.Show($"جنرل {modeUrdu} انوائس (#{invoice.InvoiceNo}) کامیابی سے محفوظ ہو گئی۔\\nاسٹاک اور کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔", \n                "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"General {mode} Invoice (#{invoice.InvoiceNo}) saved successfully!\\nInventory and General Ledger updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c2 = c2.replace('MessageBox.Show($"جنرل {modeUrdu} انوائس (#{invoice.InvoiceNo}) کامیابی سے محفوظ ہو گئی۔\\nاسٹاک اور کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔",\n                "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"General {mode} Invoice (#{invoice.InvoiceNo}) saved successfully!\\nInventory and General Ledger updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c2 = c2.replace('MessageBox.Show($"انوائس محفوظ کرنے میں مسئلہ آیا:\\n{ex.Message}", "خرابی", MessageBoxButton.OK, MessageBoxImage.Error);',
                'MessageBox.Show($"Failed to save invoice:\\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);')
c2 = c2.replace('MessageBox.Show("براہ کرم پہلے سے محفوظ شدہ انوائس اوپن کریں۔", "توجہ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please open an existing saved invoice first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c2 = c2.replace('if (MessageBox.Show($"کیا آپ واقعی انوائس #{SaleId} کو حذف کرنا چاہتے ہیں؟\\nاس سے اسٹاک اور کھاتہ جات خودکار ریورس ہو جائیں گے۔",\n            "تصدیق", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)',
                'if (MessageBox.Show($"Are you sure you want to delete invoice #{SaleId}?\\nThis will automatically reverse inventory and ledger postings.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)')
c2 = c2.replace('MessageBox.Show("انوائس کامیابی سے ڈیلیٹ ہو گئی۔", "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show("Invoice deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c2 = c2.replace('MessageBox.Show($"ڈیلیٹ کرنے میں مسئلہ آیا:\\n{ex.Message}", "خرابی", MessageBoxButton.OK, MessageBoxImage.Error);',
                'MessageBox.Show($"Failed to delete invoice:\\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);')
c2 = c2.replace('MessageBox.Show("پرنٹ کرنے کے لیے انوائس میں ڈیٹا کا ہونا لازمی ہے۔", "کوئی ریکارڈ نہیں", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("No line items found to print.", "Print Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c2 = c2.replace('MessageBox.Show($"پرنٹنگ انوائس #{SaleId}...", "پرنٹ", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"Printing invoice #{SaleId}...", "Print Preview", MessageBoxButton.OK, MessageBoxImage.Information);')

with open(path2, 'w', encoding='utf-8') as f:
    f.write(c2)

# 3. Clean GeneralMultiInvoiceViewModel.cs
path3 = r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralMultiInvoiceViewModel.cs'
with open(path3, 'r', encoding='utf-8') as f:
    c3 = f.read()

c3 = c3.replace('MessageBox.Show("براہ کرم پہلے پروڈکٹ منتخب کریں۔", "توجہ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please select a product first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c3 = c3.replace('MessageBox.Show("تعداد (Quantity) صفر سے زیادہ ہونی چاہیے۔", "توجہ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Quantity must be greater than zero.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c3 = c3.replace('MessageBox.Show("براہ کرم وینڈر اور کسٹمر دونوں کا انتخاب کریں۔", "لازمی فیلڈ", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please select both Vendor and Customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);')
c3 = c3.replace('MessageBox.Show("انوائس میں کم از کم ایک آئٹم کا ہونا لازمی ہے۔", "کوئی ریکارڈ نہیں", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("Please add at least one line item to the invoice.", "No Records", MessageBoxButton.OK, MessageBoxImage.Warning);')
c3 = c3.replace('MessageBox.Show($"جنرل ملٹی انوائس کامیابی سے محفوظ ہو گئی۔\\nخریداری انوائس (#{purInvoice.InvoiceNo}) اور فروخت انوائس (#{saleInvoice.InvoiceNo}) باہم منسلک ہو چکی ہیں اور اسٹاک و کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔", \n                "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"General Multi Invoice saved successfully!\\nPurchase Invoice #{purInvoice.InvoiceNo} and Sale Invoice #{saleInvoice.InvoiceNo} linked and posted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c3 = c3.replace('MessageBox.Show($"جنرل ملٹی انوائس کامیابی سے محفوظ ہو گئی۔\\nخریداری انوائس (#{purInvoice.InvoiceNo}) اور فروخت انوائس (#{saleInvoice.InvoiceNo}) باہم منسلک ہو چکی ہیں اور اسٹاک و کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔",\n                "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"General Multi Invoice saved successfully!\\nPurchase Invoice #{purInvoice.InvoiceNo} and Sale Invoice #{saleInvoice.InvoiceNo} linked and posted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);')
c3 = c3.replace('MessageBox.Show($"ملٹی انوائس محفوظ کرنے میں مسئلہ آیا:\\n{ex.Message}", "خرابی", MessageBoxButton.OK, MessageBoxImage.Error);',
                'MessageBox.Show($"Failed to save multi invoice:\\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);')
c3 = c3.replace('MessageBox.Show("پرنٹ کرنے کے لیے انوائس میں ڈیٹا کا ہونا لازمی ہے۔", "کوئی ریکارڈ نہیں", MessageBoxButton.OK, MessageBoxImage.Warning);',
                'MessageBox.Show("No line items found to print.", "Print Warning", MessageBoxButton.OK, MessageBoxImage.Warning);')
c3 = c3.replace('MessageBox.Show($"پرنٹنگ ملٹی انوائس (Purchase #{PurchaseId} / Sale #{SaleId})...", "پرنٹ", MessageBoxButton.OK, MessageBoxImage.Information);',
                'MessageBox.Show($"Printing General Multi Invoice (Purchase #{PurchaseId} / Sale #{SaleId})...", "Print Preview", MessageBoxButton.OK, MessageBoxImage.Information);')

with open(path3, 'w', encoding='utf-8') as f:
    f.write(c3)

print("Replacement complete.")

