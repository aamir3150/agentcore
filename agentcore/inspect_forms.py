import os
import glob
import re

agensoft_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"

# Search in all DLLs and EXE for FrmPurchase, FrmSales, FrmGenMulti, etc.
files = glob.glob(os.path.join(agensoft_dir, "*.dll")) + glob.glob(os.path.join(agensoft_dir, "*.exe"))

print("Searching assemblies in AgenSoft...")
for f in files:
    fname = os.path.basename(f)
    try:
        with open(f, "rb") as fp:
            content = fp.read().decode("latin1", errors="ignore")
            matches = []
            for m in re.finditer(r'(Frm[A-Za-z0-9_]+|Purchase|Sales|MultiInvoice[A-Za-z0-9_]*)', content):
                val = m.group(1)
                if any(x in val.lower() for x in ['purchase', 'sale', 'multi']) and len(val) > 5:
                    matches.append(val)
            matches = set(matches)
            if matches:
                # filter down to relevant form names
                forms = [x for x in matches if x.startswith('Frm') or 'Invoice' in x]
                if forms:
                    print(f"\n{fname}:")
                    for form in sorted(forms):
                        print(f"  - {form}")
    except Exception as e:
        print(f"Error reading {fname}: {e}")

