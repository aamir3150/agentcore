import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Search UTF-16LE for CreditVoucher
matches_utf16 = [m.start() for m in re.finditer(b"C\x00r\x00e\x00d\x00i\x00t\x00V\x00o\x00u\x00c\x00h\x00e\x00r\x00", data, re.IGNORECASE)]
print(f"UTF-16 matches for CreditVoucher: {[hex(m) for m in matches_utf16]}")

# Also search for "Voucher" in general
matches_vch_utf16 = [m.start() for m in re.finditer(b"V\x00o\x00u\x00c\x00h\x00e\x00r\x00", data, re.IGNORECASE)]
print(f"UTF-16 matches for Voucher: {len(matches_vch_utf16)}")

offsets = [0x2d3ec, 0x6f473, 0xfda4e, 0x179d88, 0x189da6, 0x38cac8] + matches_utf16

for off in offsets[:10]:
    print(f"\n==================== OFFSET 0x{off:x} ====================")
    chunk = data[max(0, off - 500): min(len(data), off + 2000)]
    
    # Extract ASCII strings
    raw_ascii = re.findall(b"[\x20-\x7E]{3,}", chunk)
    ascii_strings = [s.decode('latin1') for s in raw_ascii]
    
    # Extract UTF-16 strings
    raw_utf16 = re.findall(b"(?:[\x20-\x7E]\x00){3,}", chunk)
    utf16_strings = [s.decode('utf-16le') for s in raw_utf16]
    
    print("ASCII strings:")
    for s in ascii_strings[:15]:
        print(f"  {s}")
        
    print("UTF-16 strings:")
    for s in utf16_strings[:20]:
        print(f"  [W] {s}")
