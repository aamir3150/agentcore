import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's find the OLE compound file header around 0x6f470
# Compound file header starts with: \xD0\xCF\x11\xE0\xA1\xB1\x1A\xE1
ole_sig = b"\xD0\xCF\x11\xE0\xA1\xB1\x1A\xE1"
idx = data.rfind(ole_sig, 0, 0x6f470)
if idx != -1:
    print(f"OLE Header found at: 0x{idx:x}")
else:
    # Look forward
    idx = data.find(ole_sig, 0x6f470)
    print(f"OLE Header forward found at: 0x{idx:x}")

# Let's inspect the entire chunk from 0x6f000 to 0x7a000
chunk = data[0x6f000:0x7a000]

# Extract all ASCII strings >= 3 chars
ascii_strings = [s.decode('latin1') for s in re.findall(b"[\x20-\x7E]{3,}", chunk)]
# Extract all UTF-16 strings
utf16_strings = [s.decode('utf-16le', errors='ignore') for s in re.findall(b"(?:[\x20-\x7E]\x00){3,}", chunk)]

print("--- ASCII strings in CrpCreditVoucher stream ---")
for s in ascii_strings:
    if any(k in s.lower() for k in ["voucher", "date", "cash", "credit", "debit", "account", "amount", "narration", "user", "print", "total", "signature", "authorized", "prepared", "received", "rupees", "balance", "crp", "report", "company", "address", "phone", "jameel", "times", "arial"]):
        print("  [A]", s)

print("\n--- UTF-16 strings in CrpCreditVoucher stream ---")
for s in utf16_strings:
    if any(k in s.lower() for k in ["voucher", "date", "cash", "credit", "debit", "account", "amount", "narration", "user", "print", "total", "signature", "authorized", "prepared", "received", "rupees", "balance", "crp", "report", "company", "address", "phone", "jameel", "times", "arial"]):
        print("  [W]", s)

