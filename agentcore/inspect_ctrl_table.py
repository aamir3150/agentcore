import struct

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

image_base = 0x400000

# Let's read the control structure around 0xa12c
chunk = data[0xa000:0xa600]
print("Examining 0xa000 - 0xa600:")
for i in range(0xa000, 0xa500, 4):
    val = struct.unpack_from("<I", data, i)[0]
    if 0x400000 <= val < 0x400000 + len(data):
        target_off = val - 0x400000
        # Check if it points to a string
        end = data.find(b"\x00", target_off)
        if end != -1 and 0 < end - target_off < 60:
            try:
                s = data[target_off:end].decode('latin1')
                if any(c.isalnum() for c in s):
                    print(f"0x{i:x}: points to 0x{val:x} -> '{s}'")
            except:
                pass

