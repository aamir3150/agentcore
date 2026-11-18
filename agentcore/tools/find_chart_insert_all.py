import os
import re

agensoft_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
for f in os.listdir(agensoft_dir):
    if f.lower().endswith((".dll", ".exe")):
        fp = os.path.join(agensoft_dir, f)
        with open(fp, "rb") as bf:
            data = bf.read()
        
        matches = [m.start() for m in re.finditer(b"INSERT[\\s\\[]+INTO[\\s\\[]+ChartOfAccounts", data, re.IGNORECASE)]
        if matches:
            print(f"{f} ASCII matches at {[hex(m) for m in matches]}")
            
        needle = b"I\x00N\x00S\x00E\x00R\x00T\x00"
        for m in re.finditer(needle, data):
            chunk = data[m.start(): min(len(data), m.start() + 400)]
            try:
                s = chunk.decode('utf-16le', errors='ignore')
                if 'chartofaccounts' in s.lower():
                    print(f"{f} UTF-16 match: {s[:150]}")
            except:
                pass

