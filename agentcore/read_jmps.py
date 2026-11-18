# Disassemble the jump table targets around 0x40c000 - 0x40c300
import struct

with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

# In VB6: jump table has `jmp relative` (opcode 0xE9)
# Let's read what each target at 0xa454..0xa500 jumps to:
print("Jump table entries:")
for i in range(0xa454, 0xa500, 4):
    va = struct.unpack_from("<I", data, i)[0]
    file_off = va - 0x400000
    if 0 <= file_off < len(data):
        b = data[file_off:file_off+5]
        if b[0] == 0xE9: # jmp
            rel = struct.unpack_from("<i", b, 1)[0]
            target_va = va + 5 + rel
            print(f"Index {(i-0xa454)//4}: VA 0x{va:x} -> jmp 0x{target_va:x} (file off 0x{target_va-0x400000:x})")
        else:
            print(f"Index {(i-0xa454)//4}: VA 0x{va:x} -> raw bytes {b.hex()}")

