import struct

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

# Base address of AgenSoft.exe is usually 0x400000
image_base = 0x400000

# String offset:
str_offset = 0x19768
str_va = image_base + str_offset

print(f"Searching for pointers to 0x{str_va:x} or 0x{str_offset:x}...")
p1 = struct.pack("<I", str_va)
p2 = struct.pack("<I", str_offset)

for p, label in [(p1, "VA"), (p2, "Offset")]:
    pos = 0
    while True:
        idx = data.find(p, pos)
        if idx == -1: break
        print(f"Found {label} pointer at 0x{idx:x}")
        pos = idx + 4

