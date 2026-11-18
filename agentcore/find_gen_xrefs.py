with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

# Let's search for the bytes of the string "BtnGenPurchaseInvoice"
idx = data.find(b"BtnGenPurchaseInvoice")
print(f"BtnGenPurchaseInvoice found at 0x{idx:x}")

# Let's look at the object table or form events in AgenSoft.exe
# Search for MniPurchaseInvoice as well
idx_mni = data.find(b"MniPurchaseInvoice")
print(f"MniPurchaseInvoice found at 0x{idx_mni:x}")

# In VB6, controls on the main MDI/Form have event handlers. Let's search all strings starting with "BtnGen" or "MniGen" or "Mni"
import re
all_mni = re.findall(b"(?:BtnGen|MniGen|MniPurchase|MniSale)[A-Za-z0-9_]*", data)
print("Unique menu/button IDs:", set(m.decode('latin1') for m in all_mni))

