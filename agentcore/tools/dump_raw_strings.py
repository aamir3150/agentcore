import re

dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

offset = 1088684
start = max(0, offset - 2000)
end = min(len(data), offset + 2000)
chunk = data[start:end]

ascii_strs = re.findall(rb'[\x20-\x7E]{2,}', chunk)
for s in ascii_strs:
    print(s.decode('latin1', errors='ignore'))

