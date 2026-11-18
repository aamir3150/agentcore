with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "rb") as f:
    pur_data = f.read()

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll", "rb") as f:
    sale_data = f.read()

import re

def dump_form_properties(data, offset, name):
    print(f"\n=================== Form {name} around 0x{offset:x} ===================")
    chunk = data[offset:offset+10000]
    # find all strings
    strs = [m.group(0).decode('latin1') for m in re.finditer(b"[\x20-\x7E]{3,}", chunk)]
    print(f"Total strings found: {len(strs)}")
    
    # Let's filter labels, text, buttons, and captions
    interesting = []
    for s in strs:
        if any(c in s for c in ['txt', 'btn', 'lbl', 'cmd', 'dtp', 'cmb', 'grd', 'flx', 'grid', 'Invoice', 'Date', 'Vendor', 'Customer', 'Product', 'Qty', 'Price', 'Rate', 'Total', 'Amount', 'Tax', 'Discount', 'Net', 'Save', 'Print', 'Delete', 'Close', 'Clear', 'Find', 'Search', 'Account', 'Cash', 'Credit']):
            interesting.append(s)
            
    for item in interesting[:80]:
        print(f"  {item}")

dump_form_properties(pur_data, 0x1a2908, "Purchaseinvoices")
dump_form_properties(sale_data, 0x1a6e44, "SaleInvoice")

