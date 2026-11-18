with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    exe_data = f.read()

import re

# Find all Mni... menu names in AgenSoft.exe
mni_items = re.findall(b"Mni[A-Za-z0-9_]+", exe_data)
print(f"Total Mni items: {len(mni_items)}")
for m in sorted(set(mni_items)):
    s = m.decode('latin1')
    if any(k in s.lower() for k in ['cheque', 'check', 'book', 'bank', 'trans', 'gen']):
        print(f"  {s}")

