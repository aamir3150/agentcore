import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's search for "OpeningDebit" or "OpeningCredit" or "OpeningBalance"
matches = [m.start() for m in re.finditer(b"OpeningDebit|OpeningCredit|OpeningBalance", data, re.IGNORECASE)]
print("Matches for OpeningDebit/Credit/Balance in AccountsForms.dll:", len(matches))

for idx in matches:
    start = max(0, idx - 400)
    end = min(len(data), idx + 600)
    chunk = data[start:end]
    strings = re.findall(rb'[\x20-\x7E]{3,}', chunk)
    print(f"\n--- Around 0x{idx:x} ---")
    for s in strings:
        print("  ", s.decode('latin1', errors='ignore'))

