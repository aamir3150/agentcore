dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

idx = data.find(b"txtAccNameFilter")
print(f"txtAccNameFilter found at 0x{idx:x}")

start = max(0, idx - 1000)
end = min(len(data), idx + 2500)
chunk = data[start:end]

import re
strings = re.findall(rb'[\x20-\x7E]{2,}', chunk)
for s in strings:
    print(s.decode('latin1', errors='ignore'))

