import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

offset = 1088684
start = max(0, offset - 4000)
end = min(len(data), offset + 15000)
chunk = data[start:end]

# Find all ASCII and UTF-16 strings
ascii_strs = re.findall(rb'[\x20-\x7E]{3,}', chunk)
print("=== ASCII STRINGS around 1088684 ===")
for s in ascii_strs:
    decoded = s.decode('latin1', errors='ignore')
    if any(k in decoded.lower() for k in ['txt', 'btn', 'cmd', 'lbl', 'grid', 'vsflex', 'msflex', 'comb', 'cbo', 'account', 'balance', 'debit', 'credit', 'open', 'save', 'clear', 'exit', 'print', 'select', 'update', 'insert']):
        print("  ", decoded)

# Also look for Unicode strings (UTF-16LE)
uni_strs = re.findall(rb'(?:[\x20-\x7E]\x00){3,}', chunk)
print("\n=== UNICODE STRINGS around 1088684 ===")
for s in uni_strs:
    try:
        decoded = s.decode('utf-16le')
        if any(k in decoded.lower() for k in ['txt', 'btn', 'cmd', 'lbl', 'grid', 'vsflex', 'msflex', 'comb', 'cbo', 'account', 'balance', 'debit', 'credit', 'open', 'save', 'clear', 'exit', 'print', 'select', 'update', 'insert']):
            print("  ", decoded)
    except:
        pass

