import re

def find_form_def(dll_path, form_name):
    print(f"\n================ Form {form_name} in {dll_path} ================")
    with open(dll_path, "rb") as f:
        data = f.read()

    pos = 0
    while True:
        idx = data.find(form_name.encode('latin1'), pos)
        if idx == -1: break
        print(f"Found {form_name} at 0x{idx:x}")
        # search for nearby strings
        start = max(0, idx - 500)
        end = min(len(data), idx + 4000)
        chunk = data[start:end]
        
        # print all strings >= 3 chars
        strs = [m.group(0).decode('latin1') for m in re.finditer(b"[\x20-\x7E]{3,}", chunk)]
        print("  Strings:")
        for s in strs[:60]:
            if any(term in s.lower() for term in ['btn', 'txt', 'lbl', 'grid', 'dtp', 'cmb', 'caption', 'label', 'text', 'select', 'insert', 'from', 'table', 'date', 'price', 'total', 'net', 'bill', 'invoice', 'product', 'vendor', 'customer', 'party']):
                print(f"    {s}")
        pos = idx + len(form_name)

find_form_def(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "Purchaseinvoices")
find_form_def(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll", "SaleInvoice")

