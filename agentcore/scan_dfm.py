import os
import glob
import re

agen_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"

def scan_strings_and_forms(filepath):
    print(f"=== Scanning {os.path.basename(filepath)} ===")
    with open(filepath, "rb") as f:
        data = f.read()
    
    # Delphi forms usually start with "TPF0" (binary DFM) or "object " (text DFM)
    pos = 0
    dfms = []
    while True:
        idx = data.find(b"TPF0", pos)
        if idx == -1:
            break
        # Read form name after TPF0
        # Format of binary DFM:
        # TPF0 + length byte + class name or object name
        name_len = data[idx+4]
        try:
            name = data[idx+5:idx+5+name_len].decode('latin1')
            dfms.append((idx, name))
        except:
            pass
        pos = idx + 4
        
    print(f"Found {len(dfms)} DFM resources:")
    for offset, name in dfms:
        print(f"  Offset 0x{offset:x}: {name}")

for dll in ["ATPurchaseForms.dll", "ATSalesForms.dll", "ATBrokrageForms.dll", "AgenSoft.exe"]:
    p = os.path.join(agen_dir, dll)
    if os.path.exists(p):
        scan_strings_and_forms(p)

