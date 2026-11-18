with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

chunk = acc_data[0x8a1a4:0x93000]
ascii_strs = [m.group(0).decode('latin1', errors='ignore') for m in re.finditer(b"[\x20-\x7E]{3,}", chunk)]
print(f"Total ASCII strings in range: {len(ascii_strs)}")
for i, s in enumerate(ascii_strs):
    # filter out photoshop metadata chunks
    if any(p in s for p in ['Photoshop', '8BIM', 'print', 'crop', 'UntF', 'Prc@', 'Pxl@']):
        continue
    print(f"  [{i}]: {s}")

