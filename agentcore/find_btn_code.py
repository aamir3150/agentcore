import os
import re

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

buttons = [
    b"BtnGenPurchaseInvoice",
    b"BtnGenSaleInvoice",
    b"BtnGenGenMultiInvoice",
    b"MniPurchaseInvoice",
    b"MniSaleInvoice",
    b"MniGenMultiInvoice"
]

for btn in buttons:
    pos = 0
    while True:
        idx = data.find(btn, pos)
        if idx == -1: break
        print(f"=== {btn.decode()} at 0x{idx:x} ===")
        # Look around 200 bytes before and 300 bytes after
        snippet = data[max(0, idx-100):min(len(data), idx+200)]
        # find readable strings
        readable = re.findall(b"[\x20-\x7E]{3,}", snippet)
        print("  Strings nearby:")
        for r in readable:
            print(f"    {r.decode('latin1')}")
        pos = idx + len(btn)

