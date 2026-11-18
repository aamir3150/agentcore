import os
import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll"
with open(dll_path, "rb") as f:
    data = f.read()

off = 294029
chunk = data[off:off+10000].decode('latin1', errors='ignore')

# Clean RTF tags like \par, \tab
cleaned = chunk.replace('\\par', '\n').replace('\\tab', '\t')

# Find CREATE view VuAccVouchersAccounts until the next GO or next CREATE
idx_start = cleaned.find("CREATE view VuAccVouchersAccounts")
idx_end = cleaned.find("GO", idx_start)

sql = cleaned[idx_start:idx_end].strip()
print("Extracted SQL length:", len(sql))
print(sql[:1500])

with open(r"d:\Projects\agentcore\agentcore\tools\VuAccVouchersAccounts.sql", "w", encoding="utf-8") as out:
    out.write(sql)

