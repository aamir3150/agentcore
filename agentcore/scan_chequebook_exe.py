with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    exe_data = f.read()

import re

for kw in [b"GenerateChequeBook", b"ChequeBook", b"Cheque Book", b"BankChequeGenerate", b"MniBankChequeGenerate", b"BtnBankChequeGenerate"]:
    pos = 0
    while True:
        idx = exe_data.find(kw, pos)
        if idx == -1: break
        print(f"Found {kw.decode('latin1', errors='ignore')} at 0x{idx:x}")
        snippet = exe_data[max(0, idx-60):min(len(exe_data), idx+120)]
        readable = [r.decode('latin1', errors='ignore') for r in re.findall(b"[\x20-\x7E]{3,}", snippet)]
        print(f"  Snippet: {readable}")
        pos = idx + len(kw)

