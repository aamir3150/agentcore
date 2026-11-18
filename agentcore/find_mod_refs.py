with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

# Let's search for "ATPurchaseForms" and "ATSalesForms" references in AgenSoft.exe
for mod in [b"ATPurchaseForms", b"ATSalesForms", b"ATBrokrageForms", b"PurchaseInvoiceForm", b"SalesForm", b"MultiInvoiceForm"]:
    pos = 0
    print(f"\nOccurrences of {mod.decode()}:")
    while True:
        idx = data.find(mod, pos)
        if idx == -1: break
        print(f"  0x{idx:x}")
        # print snippet around it
        snippet = data[max(0, idx-50):min(len(data), idx+100)]
        pos = idx + len(mod)

