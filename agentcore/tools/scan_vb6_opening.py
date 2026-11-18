import os
import re

agensoft_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
for f in os.listdir(agensoft_dir):
    if f.lower().endswith((".dll", ".exe")):
        fp = os.path.join(agensoft_dir, f)
        with open(fp, "rb") as bf:
            data = bf.read()
        
        # VB6 forms usually have their names in text or form definitions
        # Look for forms or captions containing "Opening" or "Balance"
        matches = re.findall(b'([a-zA-Z0-9_]{3,30}(?:Opening|OpenBal|Balance)[a-zA-Z0-9_]*)', data, re.IGNORECASE)
        if matches:
            unique_matches = set(m.decode('latin1', errors='ignore') for m in matches)
            print(f"=== {f} ===")
            for u in sorted(unique_matches):
                print(f"   {u}")

