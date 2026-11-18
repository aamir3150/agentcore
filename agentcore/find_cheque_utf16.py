with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

utf_strs = re.findall(b"(?:[\x20-\x7E]\x00){4,}", acc_data)
decoded = [u.decode('utf-16le') for u in utf_strs]

cheque_matches = [s for s in decoded if any(k in s.lower() for k in ['cheque', 'fromcheque', 'tocheque', 'chequeno', 'generate'])]
print(f"Found {len(cheque_matches)} Cheque/Generate UTF-16 strings:")
for s in cheque_matches[:100]:
    print(f"  {s}")

