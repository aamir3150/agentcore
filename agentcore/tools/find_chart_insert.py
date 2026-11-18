import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Search for "INSERT INTO ChartOfAccounts" in ASCII or UTF-16
matches = [m.start() for m in re.finditer(b"INSERT[\\s\\[]+INTO[\\s\\[]+ChartOfAccounts", data, re.IGNORECASE)]
print("ASCII INSERT INTO ChartOfAccounts:", [hex(m) for m in matches])

needle = b"I\x00N\x00S\x00E\x00R\x00T\x00 \x00I\x00N\x00T\x00O\x00 \x00C\x00h\x00a\x00r\x00t\x00O\x00f\x00A\x00c\x00c\x00o\x00u\x00n\x00t\x00s\x00"
matches_u = [m.start() for m in re.finditer(needle, data, re.IGNORECASE)]
print("UTF-16 INSERT INTO ChartOfAccounts:", [hex(m) for m in matches_u])

for m in matches + matches_u:
    print(f"\n--- 0x{m:x} ---")
    chunk = data[m: min(len(data), m + 800)]
    try:
        print(chunk.decode('utf-16le', errors='ignore'))
    except:
        pass
    try:
        print(chunk.decode('latin1', errors='ignore'))
    except:
        pass

