with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

offsets = [0x19768, 0x196dc, 0x19870, 0x19de46, 0x232ef0, 0x1a2122]
for off in offsets:
    print(f"\n================ Offset 0x{off:x} ================")
    start = max(0, off - 300)
    end = min(len(data), off + 500)
    chunk = data[start:end]
    # Look for unicode and ascii strings
    # in VB6, strings can be BSTR (UTF-16LE) or ASCII
    import re
    # print ascii
    ascii_strs = [m.group(0).decode('latin1') for m in re.finditer(b"[\x20-\x7E]{4,}", chunk)]
    print("ASCII:", ascii_strs)
    # print utf-16le
    try:
        utf16_strs = [m.group(0).decode('utf-16le') for m in re.finditer(b"(?:[\x20-\x7E]\x00){4,}", chunk)]
        print("UTF-16:", utf16_strs)
    except Exception as e:
        pass

