with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll", "rb") as f:
    upd_data = f.read()

import re

# find CREATE TABLE for Cheque
for m in re.finditer(b"CREATE TABLE [^\n\r]+(?:Cheque|BankCheque)[^\n\r]+", upd_data, re.IGNORECASE):
    idx = m.start()
    chunk = upd_data[idx:idx+1500].decode('latin1', errors='ignore')
    print(f"\n================ CREATE TABLE at 0x{idx:x} ================")
    print(chunk[:1000])

