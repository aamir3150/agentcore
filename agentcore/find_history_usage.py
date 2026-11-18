with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

# Look for references to "BankChequeBookHistory" in AccountsForms.dll or AgenSoft.exe
print("Searching for BankChequeBookHistory in AccountsForms.dll...")
pos = 0
while True:
    idx = acc_data.find(b"BankChequeBookHistory", pos)
    if idx == -1: break
    print(f"Found BankChequeBookHistory at 0x{idx:x}")
    chunk = acc_data[max(0, idx-100):min(len(acc_data), idx+300)]
    readable = re.findall(b"[\x20-\x7E]{3,}", chunk)
    print(f"  Readable: {[r.decode('latin1', errors='ignore') for r in readable]}")
    pos = idx + 20

# Also check AgenSoft.exe
with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    exe_data = f.read()
print("\nSearching for BankChequeBookHistory in AgenSoft.exe...")
pos = 0
while True:
    idx = exe_data.find(b"BankChequeBookHistory", pos)
    if idx == -1: break
    print(f"Found BankChequeBookHistory at 0x{idx:x}")
    pos = idx + 20

