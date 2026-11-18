with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    exe_data = f.read()

import re

# Search for "Transaction" in ascii and utf-16
print("Ascii matches for 'Transaction':")
for m in re.finditer(b"Transaction", exe_data, re.IGNORECASE):
    idx = m.start()
    snippet = exe_data[max(0, idx-40):min(len(exe_data), idx+120)]
    readable = re.findall(b"[\x20-\x7E]{3,}", snippet)
    print(f"  0x{idx:x}: {[r.decode('latin1', errors='ignore') for r in readable]}")

print("\nUTF-16 matches for 'Transaction':")
for m in re.finditer(b"T\x00r\x00a\x00n\x00s\x00a\x00c\x00t\x00i\x00o\x00n", exe_data, re.IGNORECASE):
    idx = m.start()
    snippet = exe_data[max(0, idx-80):min(len(exe_data), idx+200)]
    # decode utf16
    try:
        utf_strs = re.findall(b"(?:[\x20-\x7E]\x00){3,}", snippet)
        print(f"  0x{idx:x}: {[u.decode('utf-16le', errors='ignore') for u in utf_strs]}")
    except:
        pass

