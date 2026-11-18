import re

def dump_form_controls(dll_path, form_keyword):
    print(f"\n=================== Searching {form_keyword} in {dll_path} ===================")
    with open(dll_path, "rb") as f:
        data = f.read()

    pos = 0
    matches = []
    kw_bytes = form_keyword.encode('latin1')
    while True:
        idx = data.find(kw_bytes, pos)
        if idx == -1: break
        matches.append(idx)
        pos = idx + len(kw_bytes)

    for idx in matches:
        print(f"Match at 0x{idx:x}:")
        # dump 1000 bytes around it
        start = max(0, idx - 100)
        end = min(len(data), idx + 2000)
        snippet = data[start:end]
        
        # Look for controls like txt..., lbl..., btn..., dtp..., grid..., etc.
        ctrls = re.findall(b"(?:txt|lbl|cmd|btn|dtp|cmb|chk|opt|grd|flx|vfg)[A-Za-z0-9_]+", snippet, re.IGNORECASE)
        captions = re.findall(b"[\x20-\x7E]{4,}", snippet)
        
        print("  Controls:", set(c.decode('latin1') for c in ctrls))
        # print first 20 captions
        print("  Captions/Strings:", [c.decode('latin1') for c in captions[:30]])

dump_form_controls(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "PurchaseInvoiceForm")
dump_form_controls(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "MultiInvoiceForm")
dump_form_controls(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll", "SalesForm")

