import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's inspect all strings from 0x30000 to 0x39000
chunk = data[0x30000:0x39000]

# All UTF-16 strings
utf16_matches = re.findall(b"(?:[\x20-\x7E\r\n\t]\x00){2,}", chunk)
utf16_strings = [s.decode('utf-16le') for s in utf16_matches]

print(f"Total UTF-16 strings in range: {len(utf16_strings)}")

# Filter for interesting strings
with open(r"d:\Projects\agentcore\agentcore\tools\credit_voucher_strings.txt", "w", encoding="utf-8") as out:
    for s in utf16_strings:
        s_clean = s.strip()
        if len(s_clean) > 1:
            out.write(s_clean + "\n")

print("Wrote strings to tools/credit_voucher_strings.txt")

