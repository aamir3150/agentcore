import os
import re

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

menus = [
    b"MniPurchaseInvoice",
    b"MniSaleInvoice",
    b"MniGenMultiInvoice"
]

for mnu in menus:
    pos = 0
    while True:
        idx = data.find(mnu, pos)
        if idx == -1: break
        print(f"=== {mnu.decode()} at 0x{idx:x} ===")
        snippet = data[max(0, idx-100):min(len(data), idx+200)]
        readable = re.findall(b"[\x20-\x7E]{3,}", snippet)
        print("  Strings nearby:")
        for r in readable:
            print(f"    {r.decode('latin1')}")
        pos = idx + len(mnu)

