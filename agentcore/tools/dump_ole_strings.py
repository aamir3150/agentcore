import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's search all strings in 0x6a000 to 0x75000
chunk = data[0x6a7f6:0x75000]

# All ascii strings of length >= 3
strings = re.findall(b"[\x20-\x7E]{3,}", chunk)
for s in strings:
    text = s.decode('latin1')
    if len(text) > 3:
        print(text)

