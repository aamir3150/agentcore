import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's search for "CrpCreditVoucher"
matches = [m.start() for m in re.finditer(b"CrpCreditVoucher", data, re.IGNORECASE)]
print(f"CrpCreditVoucher at: {[hex(m) for m in matches]}")

for m in matches:
    print(f"\n================ 0x{m:x} ================")
    # Look around this match for report properties, headers, labels, fields
    start = max(0, m - 500)
    end = min(len(data), m + 15000)
    chunk = data[start:end]
    
    # Extract readable strings
    strings = [s.decode('latin1') for s in re.findall(b"[\x20-\x7E]{3,}", chunk)]
    print(f"Total ASCII strings in chunk: {len(strings)}")
    
    # Filter for interesting words
    keywords = ["title", "header", "voucher", "date", "account", "amount", "rupees", "words", "signature", "prepared", "received", "section", "formula", "crp", "field", "font", "arial", "courier", "cash"]
    for s in strings:
        if any(k in s.lower() for k in keywords) and len(s) < 80:
            print("  ", s)

