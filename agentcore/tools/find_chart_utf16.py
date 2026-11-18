import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

needle = b"C\x00h\x00a\x00r\x00t\x00O\x00f\x00A\x00c\x00c\x00o\x00u\x00n\x00t\x00s\x00"
matches = [m.start() for m in re.finditer(needle, data, re.IGNORECASE)]
print(f"UTF-16 ChartOfAccounts occurrences: {len(matches)} at {[hex(m) for m in matches]}")

for m in matches:
    print(f"\n--- UTF-16 around 0x{m:x} ---")
    chunk = data[max(0, m - 100): min(len(data), m + 300)]
    try:
        s = chunk.decode('utf-16le', errors='ignore')
        print(s)
    except:
        pass

