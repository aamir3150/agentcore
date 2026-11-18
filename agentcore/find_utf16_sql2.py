with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

# find utf-16 strings
utf_strs = re.findall(b"(?:[\x20-\x7E]\x00){4,}", acc_data)
decoded = [u.decode('utf-16le') for u in utf_strs]

sql_matches = [s for s in decoded if any(k in s.lower() for k in ['insert', 'select', 'update', 'delete', 'bank', 'cheque', 'history', 'account'])]
print(f"Found {len(sql_matches)} interesting UTF-16 strings:")
for s in sql_matches[:60]:
    print(f"  {s}")

