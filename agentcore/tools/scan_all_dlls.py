import os
import re

agensoft_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
for f in os.listdir(agensoft_dir):
    if f.lower().endswith((".dll", ".exe")):
        fp = os.path.join(agensoft_dir, f)
        with open(fp, "rb") as bf:
            data = bf.read()
        
        matches = [m.start() for m in re.finditer(b"AccountsOpeningBalance", data, re.IGNORECASE)]
        if matches:
            print(f"{f}: found {len(matches)} times at {[hex(m) for m in matches]}")

