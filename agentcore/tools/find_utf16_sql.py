import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

# Search for UTF-16LE SQL queries
queries = []
for m in re.finditer(rb'(?:(S\x00E\x00L\x00E\x00C\x00T\x00|U\x00P\x00D\x00A\x00T\x00E\x00|I\x00N\x00S\x00E\x00R\x00T\x00|D\x00E\x00L\x00E\x00T\x00E\x00|E\x00X\x00E\x00C\x00)[^\x00]{2,300})', data, re.IGNORECASE):
    try:
        # extract utf-16 string
        raw = data[m.start():m.start() + 400]
        # decode up to double null
        s = raw.decode('utf-16le', errors='ignore').split('\x00')[0]
        if any(w in s.lower() for w in ['open', 'opening', 'balance', 'chartofaccounts']):
            queries.append(s)
    except:
        pass

for q in set(queries):
    print("Found Query:", q)

