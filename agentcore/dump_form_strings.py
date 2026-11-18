import re

def dump_all_strings(data, offset, length=12000):
    chunk = data[offset:offset+length]
    # ascii
    ascii_strs = [m.group(0).decode('latin1') for m in re.finditer(b"[\x20-\x7E]{3,}", chunk)]
    # utf16
    utf16_strs = []
    try:
        utf16_strs = [m.group(0).decode('utf-16le') for m in re.finditer(b"(?:[\x20-\x7E]\x00){3,}", chunk)]
    except:
        pass
    return ascii_strs, utf16_strs

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "rb") as f:
    pur_data = f.read()

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll", "rb") as f:
    sale_data = f.read()

print("=== Purchaseinvoices (ASCII) ===")
a, u = dump_all_strings(pur_data, 0x1a2908 - 2000, 10000)
for s in a[:60]:
    print(f"  {s}")

print("\n=== Purchaseinvoices (UTF-16) ===")
for s in u[:60]:
    print(f"  {s}")

print("\n=== SaleInvoice (ASCII) ===")
a, u = dump_all_strings(sale_data, 0x1a6e44 - 2000, 10000)
for s in a[:60]:
    print(f"  {s}")

print("\n=== SaleInvoice (UTF-16) ===")
for s in u[:60]:
    print(f"  {s}")

