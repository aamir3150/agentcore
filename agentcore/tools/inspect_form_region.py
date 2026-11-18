import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

start = 0x105000
end = 0x112000
chunk = data[start:end]

# Extract all ASCII strings of length >= 3
raw_ascii = re.findall(b"[\x20-\x7E]{3,}", chunk)
ascii_strings = [s.decode('latin1') for s in raw_ascii]

print(f"Total ASCII strings between 0x{start:x} and 0x{end:x}: {len(ascii_strings)}")
for s in ascii_strings:
    # Print controls, captions, labels, etc.
    if any(k in s.lower() for k in ['txt', 'btn', 'cmd', 'lbl', 'label', 'grid', 'vsflex', 'msflex', 'comb', 'cbo', 'account', 'balance', 'debit', 'credit', 'open', 'save', 'clear', 'exit', 'print', 'close', 'f1', 'f2', 'f3', 'f5', 'enter', 'date', 'type', 'name', 'code', 'id']):
        print("  ", s)

