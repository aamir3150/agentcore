dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's search for "txtAccNameFilter" or "CmbFilter" or "ChartOfAccounts"
import re
queries = []
# Find all occurrences of "ChartOfAccounts"
for m in re.finditer(b"ChartOfAccounts", data):
    start = max(0, m.start() - 300)
    end = min(len(data), m.start() + 400)
    chunk = data[start:end]
    # Check if there is SELECT or UPDATE
    s = chunk.decode('latin1', errors='ignore')
    if any(k in s.lower() for k in ['select', 'update', 'where', 'from']):
        print(f"--- Query near 0x{m.start():x} ---")
        # print readable strings
        lines = re.findall(r'[\x20-\x7E]{4,}', s)
        for l in lines:
            print("  ", l)

