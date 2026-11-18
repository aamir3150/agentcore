dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

start = 0x107000
end = 0x10a500
chunk = data[start:end]

import re
# Find all printable strings of length >= 1
pos = 0
while pos < len(chunk):
    # find string
    m = re.search(rb'[\x20-\x7E]{2,}', chunk[pos:])
    if not m:
        break
    s = m.group(0).decode('latin1', errors='ignore')
    actual_pos = start + pos + m.start()
    print(f"0x{actual_pos:x}: {s}")
    pos += m.end() + 1

