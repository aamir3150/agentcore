import os, glob, re

agen_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
files = glob.glob(os.path.join(agen_dir, "*.dll")) + glob.glob(os.path.join(agen_dir, "*.exe"))

search_terms = [b"cheque", b"check", b"generation", b"book", b"leaf"]

print("Scanning for Cheque Book in AgenSoft files...")
for f in files:
    try:
        with open(f, "rb") as fp:
            data = fp.read()
        
        # Look for forms or menu items containing cheque book
        matches = []
        for kw in [b"chequebook", b"checkbook", b"chqbook", b"cheque book", b"check book", b"frmcheque", b"frmcheck"]:
            for m in re.finditer(kw, data, re.IGNORECASE):
                pos = m.start()
                snippet = data[max(0, pos-40):min(len(data), pos+100)]
                readable = [r.decode('latin1', errors='ignore') for r in re.findall(b"[\x20-\x7E]{3,}", snippet)]
                matches.append((pos, kw.decode('latin1'), readable))
        
        if matches:
            print(f"\n*** {os.path.basename(f)}: {len(matches)} matches ***")
            for pos, kw, r in matches[:10]:
                print(f"  0x{pos:x} [{kw}]: {r}")
    except Exception as e:
        print(f"Error {f}: {e}")

