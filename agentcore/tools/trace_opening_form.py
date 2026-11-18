import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Look for references to AccountsOpeningBalanceForm
needle = b"AccountsOpeningBalanceForm"
matches = [m.start() for m in re.finditer(needle, data)]
print(f"Matches for {needle.decode()}: {[hex(m) for m in matches]}")

# Let's inspect each match
for m in matches:
    print(f"\n--- Around 0x{m:x} ---")
    chunk = data[max(0, m - 500): min(len(data), m + 1000)]
    strings = re.findall(rb'[\x20-\x7E]{3,}', chunk)
    for s in strings:
        print("  ", s.decode('latin1', errors='ignore'))

