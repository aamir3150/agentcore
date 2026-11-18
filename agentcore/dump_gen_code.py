with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll", "rb") as f:
    acc_data = f.read()

import re

token = "insert into bankchequebookhistory".encode('utf-16le')
idx = acc_data.find(token)
print(f"Token found at 0x{idx:x}")

start = max(0, idx - 2000)
end = min(len(acc_data), idx + 2000)
chunk = acc_data[start:end]

utf_strs = re.findall(b"(?:[\x20-\x7E]\x00){2,}", chunk)
for u in utf_strs:
    print(u.decode('utf-16le'))

