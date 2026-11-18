import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's search for all occurrences of "OpeningDebit" in the entire DLL (case insensitive)
matches = [m.start() for m in re.finditer(b"OpeningDebit", data, re.IGNORECASE)]
print("OpeningDebit occurrences:", [hex(m) for m in matches])

for m in matches:
    print(f"\n--- 0x{m:x} ---")
    chunk = data[max(0, m - 200): min(len(data), m + 400)]
    strings = re.findall(rb'[\x20-\x7E]{2,}', chunk)
    for s in strings:
        print("  ", s.decode('latin1', errors='ignore'))

