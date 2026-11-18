with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    exe_data = f.read()

import re

# find all occurrences of 'cheque' or 'check' or 'bank'
matches = re.findall(b"(?:mni|btn|cmd)[A-Za-z0-9_]*(?:cheque|check|bank|book|generate)[A-Za-z0-9_]*", exe_data, re.IGNORECASE)
print("Matching identifiers in AgenSoft.exe:")
for m in sorted(set(matches)):
    print(f"  {m.decode('latin1')}")

