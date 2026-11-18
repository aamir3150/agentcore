import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATDefintionForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Look for Form names
# In VB6, form names often appear after "Forms." or in class definitions
matches = re.findall(rb'(?:frm|Frm|Def)[a-zA-Z0-9_]{3,30}', data)
print("Forms in ATDefintionForms.dll:")
for m in sorted(set(m.decode('latin1') for m in matches)):
    print("  ", m)

