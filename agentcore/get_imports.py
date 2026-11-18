import struct

def get_pe_imports(path):
    with open(path, "rb") as f:
        data = f.read()

    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    opt_header_size = struct.unpack_from("<H", data, e_lfanew + 20)[0]
    import_rva, import_size = struct.unpack_from("<II", data, e_lfanew + 128)
    
    sections = []
    sec_offset = e_lfanew + 24 + opt_header_size
    num_sections = struct.unpack_from("<H", data, e_lfanew + 6)[0]
    for i in range(num_sections):
        sec_name = data[sec_offset:sec_offset+8].rstrip(b'\x00').decode('latin1')
        vsize, vrva, rsize, roffset = struct.unpack_from("<IIII", data, sec_offset+8)
        sections.append((sec_name, vrva, vsize, roffset, rsize))
        sec_offset += 40

    def rva_to_offset(rva):
        for name, vrva, vsize, roffset, rsize in sections:
            if vrva <= rva < vrva + max(vsize, rsize):
                return roffset + (rva - vrva)
        return None

    imp_off = rva_to_offset(import_rva)
    if not imp_off:
        print("No import dir")
        return

    print("Imported DLLs:")
    while True:
        orig_first_thunk, timedate, fwd_chain, name_rva, first_thunk = struct.unpack_from("<IIIII", data, imp_off)
        if name_rva == 0: break
        name_off = rva_to_offset(name_rva)
        dll_name = data[name_off:data.find(b'\x00', name_off)].decode('latin1')
        print(f"  {dll_name}")
        imp_off += 20

get_pe_imports(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe")

