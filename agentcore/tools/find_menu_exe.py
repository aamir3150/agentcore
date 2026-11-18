import re

exe_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSOFT.exe"
with open(exe_path, "rb") as f:
    data = f.read()

# Let's search for "Opening" in AgenSOFT.exe
strings = re.findall(rb'[\x20-\x7E]{4,}', data)
opening_strs = [s.decode('latin1', errors='ignore') for s in strings if b'opening' in s.lower() or b'balance' in s.lower()]
print("Strings in AgenSOFT.exe with Opening / Balance:")
for s in set(opening_strs):
    print("  ", s)
