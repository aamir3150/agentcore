import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Search UTF-16LE for AccountsOpeningBalance
matches_utf16 = [m.start() for m in re.finditer(b"A\x00c\x00c\x00o\x00u\x00n\x00t\x00s\x00O\x00p\x00e\x00n\x00i\x00n\x00g\x00", data, re.IGNORECASE)]
print(f"UTF-16 matches for AccountsOpening: {[hex(m) for m in matches_utf16]}")

# Search ASCII for AccountsOpeningBalance
matches_ascii = [m.start() for m in re.finditer(b"AccountsOpeningBalance", data, re.IGNORECASE)]
print(f"ASCII matches for AccountsOpeningBalance: {[hex(m) for m in matches_ascii]}")

for off in set(matches_utf16 + matches_ascii):
    print(f"\n==================== OFFSET 0x{off:x} ====================")
    chunk = data[max(0, off - 500): min(len(data), off + 3000)]
    
    # Extract ASCII strings
    raw_ascii = re.findall(b"[\x20-\x7E]{3,}", chunk)
    ascii_strings = [s.decode('latin1') for s in raw_ascii]
    
    # Extract UTF-16 strings
    raw_utf16 = re.findall(b"(?:[\x20-\x7E]\x00){3,}", chunk)
    utf16_strings = [s.decode('utf-16le') for s in raw_utf16]
    
    print("ASCII strings:")
    for s in ascii_strings[:25]:
        print(f"  {s}")
        
    print("UTF-16 strings:")
    for s in utf16_strings[:35]:
        print(f"  [W] {s}")

