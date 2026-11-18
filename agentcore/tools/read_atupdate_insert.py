dll_path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll"
with open(dll_path, "rb") as f:
    data = f.read()

start = 0x680cc
chunk = data[start: start + 600]
print(chunk.decode('latin1', errors='ignore'))

