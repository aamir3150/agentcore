dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

pos = 0x10756e
chunk = data[pos + 3000 : pos + 10000]

import re
strings = re.findall(rb'[\x20-\x7E]{2,}', chunk)
for s in strings:
    if any(k in s.lower() for k in [b'cmd', b'btn', b'lbl', b'txt', b'f1', b'f2', b'f3', b'f5', b'filter', b'caption', b'label']):
        print(s.decode('latin1', errors='ignore'))

