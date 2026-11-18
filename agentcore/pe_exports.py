import struct
import os

def parse_pe(path):
    print(f"\n=================== {os.path.basename(path)} ===================")
    with open(path, "rb") as f:
        data = f.read()

    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    num_sections = struct.unpack_from("<H", data, e_lfanew + 6)[0]
    opt_header_size = struct.unpack_from("<H", data, e_lfanew + 20)[0]
    
    # Export directory is data directory 0
    export_rva, export_size = struct.unpack_from("<II", data, e_lfanew + 120)
    print(f"Export RVA: {hex(export_rva)}, Size: {hex(export_size)}")

    # Sections
    sections = []
    sec_offset = e_lfanew + 24 + opt_header_size
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

    if export_rva != 0:
        exp_off = rva_to_offset(export_rva)
        if exp_off:
            num_funcs, num_names, funcs_rva, names_rva, ords_rva = struct.unpack_from("<IIIII", data, exp_off + 20)
            names_off = rva_to_offset(names_rva)
            print(f"Exported Functions ({num_names} names):")
            for i in range(num_names):
                name_rva = struct.unpack_from("<I", data, names_off + i*4)[0]
                name_file_off = rva_to_offset(name_rva)
                if name_file_off:
                    n_end = data.find(b'\x00', name_file_off)
                    print(f"  {data[name_file_off:n_end].decode('latin1')}")

for f in ["ATPurchaseForms.dll", "ATSalesForms.dll", "ATBrokrageForms.dll"]:
    p = os.path.join(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft", f)
    if os.path.exists(p):
        parse_pe(p)

