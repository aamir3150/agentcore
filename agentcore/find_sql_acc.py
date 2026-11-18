with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

# find all occurrences of "insert into" or "select " in AccountsForms.dll
for m in re.finditer(b"insert into [^\x00\r\n]+", acc_data, re.IGNORECASE):
    s = m.group(0).decode('latin1', errors='ignore')
    print(f"INSERT at 0x{m.start():x}: {s}")

for m in re.finditer(b"select [^\x00\r\n]+from [^\x00\r\n]+", acc_data, re.IGNORECASE):
    s = m.group(0).decode('latin1', errors='ignore')
    if any(k in s.lower() for k in ['cheque', 'bank', 'history']):
        print(f"SELECT at 0x{m.start():x}: {s}")

