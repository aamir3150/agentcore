with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

for token in [b"GenerateChequeBookForm", b"frmBankChequeGenerate", b"BankChequeBookHistory"]:
    pos = 0
    print(f"\n=================== Token: {token.decode()} ===================")
    while True:
        idx = acc_data.find(token, pos)
        if idx == -1: break
        print(f"Found at 0x{idx:x}")
        # print 2000 bytes
        chunk = acc_data[max(0, idx-100):min(len(acc_data), idx+3000)]
        readable = [r.decode('latin1', errors='ignore') for r in re.findall(b"[\x20-\x7E]{3,}", chunk)]
        print(f"  Strings ({len(readable)}):")
        for r in readable[:50]:
            print(f"    {r}")
        pos = idx + len(token)

