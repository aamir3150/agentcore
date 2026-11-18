import struct
import os

path = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe"
with open(path, "rb") as f:
    data = f.read()

# Check PE signature
e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
pe_sig = data[e_lfanew:e_lfanew+4]
print(f"PE Sig: {pe_sig}")
machine = struct.unpack_from("<H", data, e_lfanew+4)[0]
print(f"Machine: {hex(machine)}")
magic = struct.unpack_from("<H", data, e_lfanew+24)[0]
print(f"Opt Magic: {hex(magic)}")

# Data directories:
# For PE32 (magic 0x10b), CLR header is at e_lfanew + 24 + 14*8 + 8?
# Standard PE32 Optional Header is 96 bytes before data dirs, so data dirs start at e_lfanew + 24 + 96 = e_lfanew + 120
clr_dir = e_lfanew + 120 + 14 * 8
clr_rva, clr_size = struct.unpack_from("<II", data, clr_dir)
print(f"CLR header RVA: {hex(clr_rva)}, Size: {hex(clr_size)}")

