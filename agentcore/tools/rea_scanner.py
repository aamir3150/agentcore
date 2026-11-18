import os
import glob
import re
import sys

AGENSOFT_DIR = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"

def scan_agensoft_form(form_keyword):
    print(f"\n" + "="*80)
    print(f"[*] REA DEEP SCAN FOR FORM / MODULE: '{form_keyword}'")
    print(f"[*] Target Directory: {AGENSOFT_DIR}")
    print("="*80)

    files = glob.glob(os.path.join(AGENSOFT_DIR, "*.dll")) + glob.glob(os.path.join(AGENSOFT_DIR, "*.exe"))
    
    found_any = False
    for fpath in files:
        fname = os.path.basename(fpath)
        with open(fpath, "rb") as fp:
            data = fp.read()

        # Search for form token
        matches = [m.start() for m in re.finditer(form_keyword.encode('latin1'), data, re.IGNORECASE)]
        if not matches:
            continue

        found_any = True
        print(f"\n[+] FOUND in binary: {fname} (Matches: {len(matches)})")

        for idx in matches[:5]:
            print(f"  --- Form chunk at offset 0x{idx:x} ---")
            chunk = data[max(0, idx - 1000): min(len(data), idx + 8000)]
            
            # 1. Controls
            ascii_strings = [s.decode('latin1', errors='ignore') for s in re.findall(b"[\x20-\x7E]{2,}", chunk)]
            controls = []
            captions = []
            for s in ascii_strings:
                if any(s.startswith(p) for p in ['Txt', 'Btn', 'Lbl', 'Label', 'Dtp', 'Cmb', 'cmd', 'btn', 'txt', 'lbl', 'chk', 'Chk', 'Grd', 'grd', 'MSDataGrid', 'JeweledBut', 'VSFlexGrid']):
                    if s not in controls: controls.append(s)
                elif any(w in s.lower() for w in ['caption', 'shortcut', 'save', 'clear', 'print', 'delete', 'close', 'search', 'f1', 'f2', 'f3', 'f5']):
                    if s not in captions and len(s) < 40: captions.append(s)

            if controls:
                print(f"  [Controls Identified ({len(controls)})]:")
                print("   ", ", ".join(controls[:30]))
            
            if captions:
                print(f"  [Captions / Actions ({len(captions)})]:")
                print("   ", ", ".join(captions[:15]))

            # 2. SQL Queries in this chunk
            sql_patterns = re.findall(rb'(?:SELECT|INSERT\s+INTO|UPDATE|DELETE\s+FROM)\s+[^;]{5,200}', chunk, re.IGNORECASE)
            if sql_patterns:
                print(f"  [SQL Queries Detected ({len(sql_patterns)})]:")
                for sql in sql_patterns[:5]:
                    clean_sql = re.sub(r'\s+', ' ', sql.decode('latin1', errors='ignore')).strip()
                    print(f"    -> {clean_sql}")

    if not found_any:
        print(f"[-] No direct binary matches found for '{form_keyword}'. Try searching broader terms like 'Cheque', 'Purchase', 'Sale', 'Contract'.")
    print("="*80 + "\n")

if __name__ == "__main__":
    kw = sys.argv[1] if len(sys.argv) > 1 else "Cheque"
    scan_agensoft_form(kw)

