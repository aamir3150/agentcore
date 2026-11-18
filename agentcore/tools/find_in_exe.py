import re

exe_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSOFT.exe"
with open(exe_path, "rb") as f:
    data = f.read()

for m in re.finditer(rb'AccountsOpeningBalance[a-zA-Z0-9_]*', data):
    print("Found in AgenSOFT.exe:", m.group(0).decode('latin1'))

# Let's also search around the matches
for m in re.finditer(rb'AccountsOpeningBalance', data):
    start = max(0, m.start() - 200)
    end = min(len(data), m.start() + 400)
    print("Context:", data[start:end])

