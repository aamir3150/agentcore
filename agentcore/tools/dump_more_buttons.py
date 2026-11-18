dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

pos = 0x10756e
chunk = data[pos + 1000 : pos + 6000]

import re
strings = re.findall(rb'[\x20-\x7E]{2,}', chunk)
for s in strings:
    print(s.decode('latin1', errors='ignore'))

