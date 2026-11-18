import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Let's search for "AccountsOpeningBalance" in data
for m in re.finditer(b"AccountsOpeningBalance", data):
    print("Found at:", m.start())

# Let's look for SQL queries in AccountsForms.dll that touch OpeningBalances or ChartOfAccounts
sql_patterns = [
    rb"SELECT\s+[^;]{5,200}",
    rb"UPDATE\s+[^;]{5,200}",
    rb"INSERT\s+INTO\s+[^;]{5,200}",
    rb"DELETE\s+FROM\s+[^;]{5,200}",
    rb"EXEC\s+[^;]{5,100}"
]

print("\n--- SQL Queries in AccountsForms.dll matching Opening/Account ---")
for p in sql_patterns:
    for m in re.finditer(p, data, re.IGNORECASE):
        query = m.group(0).decode('latin1', errors='ignore')
        if any(w in query.lower() for w in ['open', 'opening', 'openingbalance', 'openingbalances']):
            print("SQL:", query[:150])

