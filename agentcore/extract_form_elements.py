import re

def extract_form_elements(dll_path, form_token):
    print(f"\n================ Form Elements for {form_token} in {dll_path} ================")
    with open(dll_path, "rb") as f:
        data = f.read()

    pos = 0
    while True:
        idx = data.find(form_token.encode('latin1'), pos)
        if idx == -1: break
        print(f"Token {form_token} at 0x{idx:x}")
        # Search backward and forward
        chunk = data[max(0, idx-500):min(len(data), idx+6000)]
        
        # Look for control definitions
        # VB6 controls:
        # e.g., JeweledBut.JeweledButton, MSDataGridLib.DataGrid, VSFlexGrid, ThunderTextBox, etc.
        # or Labels and Captions
        lines = re.findall(b"[\x20-\x7E]{2,}", chunk)
        elements = []
        for l in lines:
            s = l.decode('latin1')
            if any(s.startswith(p) for p in ['Txt', 'Btn', 'Lbl', 'Label', 'Dtp', 'Cmb', 'cmd', 'btn', 'txt', 'lbl', 'chk', 'Chk', 'Grd', 'grd', 'MSDataGrid', 'JeweledBut']):
                elements.append(s)
            elif len(s) > 2 and any(w in s for w in ['Invoice', 'Purchase', 'Sale', 'Vendor', 'Customer', 'Product', 'Gross', 'Net', 'Total', 'Tare', 'Weight', 'Price', 'Rate', 'Discount', 'Tax', 'Save', 'Print', 'Clear', 'Delete', 'Close']):
                elements.append(f"Caption: {s}")
                
        print(f"Extracted {len(elements)} elements:")
        for el in elements[:60]:
            print(f"  {el}")
        pos = idx + len(form_token)

extract_form_elements(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "Purchaseinvoices")
extract_form_elements(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll", "SaleInvoice")
extract_form_elements(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "MultiInvoices")

