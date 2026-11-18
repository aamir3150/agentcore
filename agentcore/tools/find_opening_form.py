import os
import subprocess

agensoft_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
for f in os.listdir(agensoft_dir):
    if f.lower().endswith((".dll", ".exe")):
        path = os.path.join(agensoft_dir, f)
        try:
            res = subprocess.run(["ilspycmd", "-l", path], capture_output=True, text=True, timeout=10)
            for line in res.stdout.splitlines():
                if any(w in line.lower() for w in ["opening", "openbal", "balance"]):
                    print(f"{f}: {line.strip()}")
        except Exception as e:
            pass

