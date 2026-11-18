with open(r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSoft.exe", "rb") as f:
    data = f.read()

pos = 0
while True:
    idx = data.find(b"Button_Click", pos)
    if idx == -1: break
    print(f"Button_Click at 0x{idx:x}")
    pos = idx + 12

pos = 0
while True:
    idx = data.find(b"ImgIcons_Click", pos)
    if idx == -1: break
    print(f"ImgIcons_Click at 0x{idx:x}")
    pos = idx + 14

