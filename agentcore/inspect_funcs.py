import struct
import re

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

# Let's inspect each function in that range (0x2e0a00 to 0x2e3000)
# Look for call targets, push string constants, or COM vtable calls
for addr in range(0x2e0a40, 0x2e2c00, 0x50):
    chunk = data[addr:addr+0x100]
    # search for strings or pointers to strings in this chunk
    strs = []
    for i in range(0, len(chunk)-4, 1):
        val = struct.unpack_from("<I", chunk, i)[0]
        if 0x400000 <= val < 0x400000 + len(data):
            tgt = val - 0x400000
            end = data.find(b"\x00", tgt)
            if end != -1 and 3 <= end - tgt <= 50:
                try:
                    s = data[tgt:end].decode('latin1')
                    if any(c.isalnum() for c in s) and not s.startswith("!"):
                        strs.append(s)
                except:
                    pass
    if strs:
        print(f"File 0x{addr:x} (VA 0x{addr+0x400000:x}): {set(strs)}")

