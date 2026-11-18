dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's find "SSOleDBGrid" near 0x109cac
pos = 0x105000
idx = data.find(b"SSOleDBGrid", pos)
print("SSOleDBGrid at 0x%x" % idx)

start = max(0, idx - 1000)
end = min(len(data), idx + 2500)
chunk = data[start:end]

import re
strings = re.findall(rb'[\x20-\x7E]{2,}', chunk)
for s in strings:
    print(s.decode('latin1', errors='ignore'))

