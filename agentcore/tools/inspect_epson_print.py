import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

matches = [m.start() for m in re.finditer(b"E\x00P\x00S\x00O\x00N\x00", data, re.IGNORECASE)]
print(f"EPSON matches at: {[hex(m) for m in matches]}")

for m in matches:
    print(f"\n================ 0x{m:x} ================")
    start = max(0, m - 1000)
    end = min(len(data), m + 3000)
    chunk = data[start:end]
    
    # Extract ASCII strings
    strings = [s.decode('latin1') for s in re.findall(b"[\x20-\x7E]{2,}", chunk)]
    # Extract UTF-16 strings
    utf16_strings = [s.decode('utf-16le', errors='ignore') for s in re.findall(b"(?:[\x20-\x7E\r\n\t]\x00){2,}", chunk)]
    
    print("UTF-16 strings:")
    for s in utf16_strings:
        if len(s.strip()) > 1:
            print("  [W]", s.strip())
