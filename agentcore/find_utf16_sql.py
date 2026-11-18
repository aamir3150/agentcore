with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

# find utf-16 "insert into" or "select"
utf_sql = []
for m in re.finditer(b"(?:i\x00n\x00s\x00e\x00r\x00t|s\x00e\x00l\x00e\x00c\x00t|u\x00p\x00d\x00a\x00t\x00e|d\x00e\x00l\x00e\x00t\x00e)[^\x00]{2,}", acc_data, re.IGNORECASE):
    try:
        s = m.group(0).decode('utf-16le', errors='ignore')
        utf_sql.append((m.start(), s))
    except:
        pass

print(f"Found {len(utf_sql)} UTF-16 SQL strings:")
for off, s in utf_sql[:40]:
    print(f"0x{off:x}: {s}")

