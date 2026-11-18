import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Search for AccountsOpeningBalanceForm and surrounding text
target = b"AccountsOpeningBalance"
pos = 0
found_offsets = []
while True:
    idx = data.find(target, pos)
    if idx == -1:
        break
    found_offsets.append(idx)
    pos = idx + len(target)

print(f"Found {len(found_offsets)} occurrences of {target.decode()}: {found_offsets}")

for offset in found_offsets:
    start = max(0, offset - 1000)
    end = min(len(data), offset + 5000)
    chunk = data[start:end]
    
    # Extract readable strings
    strings = re.findall(rb'[\x20-\x7E]{4,}', chunk)
    print(f"\n--- Strings around offset {offset} ---")
    for s in strings[:60]:
        print("  ", s.decode('latin1', errors='ignore'))

