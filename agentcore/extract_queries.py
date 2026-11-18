import re
import os

def find_sql_and_fields(dll_path, out_file):
    with open(dll_path, "rb") as f:
        data = f.read()

    # Find ascii strings
    strings = []
    # match ascii
    for m in re.finditer(b"[\x20-\x7E]{5,}", data):
        s = m.group(0).decode('latin1')
        if any(w in s.lower() for w in ['select ', 'insert ', 'update ', 'delete ', 'purchase', 'sales', 'tbl', 'order by', 'from ']):
            strings.append((m.start(), s))

    print(f"{os.path.basename(dll_path)}: found {len(strings)} interesting strings")
    with open(out_file, "w", encoding="utf-8") as out:
        for offset, s in strings[:300]:
            out.write(f"0x{offset:x}: {s}\n")

find_sql_and_fields(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll", "pur_strings.txt")
find_sql_and_fields(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll", "sale_strings.txt")
print("Done writing strings.")

