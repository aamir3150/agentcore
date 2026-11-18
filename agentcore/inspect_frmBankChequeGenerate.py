with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

chunk = acc_data[0x8a000:0x98000]
print(f"Scanning 0x8a000 - 0x98000 in AccountsForms.dll...")

# Find ascii strings
ascii_strs = [m.group(0).decode('latin1', errors='ignore') for m in re.finditer(b"[\x20-\x7E]{3,}", chunk)]
# Find utf-16 strings
utf_strs = []
try:
    utf_strs = [m.group(0).decode('utf-16le', errors='ignore') for m in re.finditer(b"(?:[\x20-\x7E]\x00){3,}", chunk)]
except:
    pass

print(f"ASCII strings found: {len(ascii_strs)}")
print("First 100 ASCII strings:")
for s in ascii_strs[:100]:
    if any(k in s.lower() for k in ['txt', 'btn', 'lbl', 'cmd', 'dtp', 'cmb', 'chk', 'bank', 'cheque', 'check', 'book', 'leaf', 'from', 'to', 'account', 'grid', 'label', 'generate', 'save', 'clear', 'close', 'delete', 'print']):
        print(f"  {s}")

print(f"\nUTF-16 strings found: {len(utf_strs)}")
print("First 100 UTF-16 strings:")
for s in utf_strs[:100]:
    if any(k in s.lower() for k in ['txt', 'btn', 'lbl', 'cmd', 'dtp', 'cmb', 'chk', 'bank', 'cheque', 'check', 'book', 'leaf', 'from', 'to', 'account', 'grid', 'label', 'generate', 'save', 'clear', 'close', 'delete', 'print']):
        print(f"  {s}")

