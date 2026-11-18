with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

# find all strings containing 'insert', 'select', 'update', 'delete', 'BankCheque', 'ChequeNo', etc.
queries = []
for m in re.finditer(b"[\x20-\x7E]{6,}", acc_data):
    s = m.group(0).decode('latin1')
    if any(k in s.lower() for k in ['bankcheque', 'chequebook', 'chequeno', 'fromchequeno', 'tochequeno']):
        queries.append((m.start(), s))

print(f"Found {len(queries)} matching query/code strings in AccountsForms.dll:")
for off, q in queries:
    print(f"0x{off:x}: {q}")

