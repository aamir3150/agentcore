import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's search for SQL queries near 0x107000 or in the code segment
# Let's search for "FROM ChartOfAccounts" case-insensitively
matches = [m.start() for m in re.finditer(b"FROM[\\s\\[]+ChartOfAccounts", data, re.IGNORECASE)]
print("FROM ChartOfAccounts occurrences:", [hex(m) for m in matches])

for m in matches:
    print(f"\n--- 0x{m:x} ---")
    chunk = data[max(0, m - 100): min(len(data), m + 300)]
    strings = re.findall(rb'[\x20-\x7E]{3,}', chunk)
    for s in strings:
        print("  ", s.decode('latin1', errors='ignore'))

