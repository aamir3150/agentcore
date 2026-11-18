dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll"
with open(dll_path, "rb") as f:
    data = f.read()

start = 0x30c00
end = 0x31600
chunk = data[start:end]

try:
    s = chunk.decode('utf-16le', errors='ignore')
    # print all non-garbage strings
    import re
    lines = re.split(r'[\x00-\x1f]+', s)
    for l in lines:
        if len(l.strip()) > 3:
            print("UTF16:", l.strip())
except Exception as e:
    print(e)

