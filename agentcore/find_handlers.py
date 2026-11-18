with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

handlers = [
    b"BtnGenPurchaseInvoice_Click",
    b"BtnGenSaleInvoice_Click",
    b"BtnGenGenMultiInvoice_Click",
    b"MniPurchaseInvoice_Click",
    b"MniSaleInvoice_Click",
    b"MniGenMultiInvoice_Click"
]

for h in handlers:
    idx = data.find(h)
    print(f"{h.decode()}: index = {idx}")

