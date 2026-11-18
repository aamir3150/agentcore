import os
import re

def extract_typelib_strings(dll_path):
    print(f"\n==================== {os.path.basename(dll_path)} ====================")
    with open(dll_path, "rb") as f:
        data = f.read()

    # Find typelib block or all ProgIDs / form methods
    # Look for CoClass / Interface names
    # In VB6, form show methods often look like: Show, ShowForm, PurchaseInvoiceForm, etc.
    words = re.findall(b"[A-Za-z0-9_]{4,}", data)
    classes = set()
    methods = set()
    for w in words:
        s = w.decode('latin1')
        if any(term in s for term in ["Purchase", "Sale", "Multi", "Invoice", "Form"]):
            classes.add(s)
            
    print("Relevant names found:")
    for c in sorted(classes)[:80]:
        print(f"  {c}")

extract_typelib_strings(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll")
extract_typelib_strings(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll")

