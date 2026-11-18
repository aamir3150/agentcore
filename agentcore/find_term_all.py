import os, glob

agen_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
files = glob.glob(os.path.join(agen_dir, "*.dll")) + glob.glob(os.path.join(agen_dir, "*.exe"))

for f in files:
    try:
        with open(f, "rb") as fp:
            d = fp.read()
        for term in [b"BankChequeBookHistory", b"TxtFromChequeNo", b"BankChequeGenerate"]:
            if term.lower() in d.lower():
                print(f"{os.path.basename(f)} matches {term.decode()}")
    except:
        pass

