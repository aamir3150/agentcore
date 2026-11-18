with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    exe_data = f.read()

import re

# Dump all UTF-16 strings around 0x295518 from 0x290000 to 0x2a0000
chunk = exe_data[0x290000:0x2a5000]
utf_strs = re.findall(b"(?:[\x20-\x7E]\x00){3,}", chunk)
decoded = [u.decode('utf-16le', errors='ignore') for u in utf_strs]

print(f"Found {len(decoded)} UTF-16 strings in menu section:")
for s in decoded:
    if any(c.isalpha() for c in s) and len(s) > 3:
        print(f"  {s}")

