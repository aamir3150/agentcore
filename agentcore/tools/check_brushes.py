import os
import re

theme_path = r"d:\Projects\agentcore\agentcore\MyMandiSystem\Themes\ModernTheme.xaml"
with open(theme_path, "r", encoding="utf-8") as f:
    theme_text = f.read()

keys = set(re.findall(r'x:Key="([^"]+)"', theme_text))

base_dir = r"d:\Projects\agentcore\agentcore\MyMandiSystem"
missing = []
for root, dirs, files in os.walk(base_dir):
    if "bin" in root or "obj" in root:
        continue
    for file in files:
        if file.endswith(".xaml"):
            fp = os.path.join(root, file)
            with open(fp, "r", encoding="utf-8") as f:
                content = f.read()
            for m in re.finditer(r'\{StaticResource\s+([A-Za-z0-9_]+)\}', content):
                key = m.group(1)
                if key.startswith("Brush") or key.startswith("Color"):
                    if key not in keys:
                        missing.append((file, key))

print("Missing resources count:", len(missing))
for f, k in set(missing):
    print(f"File: {f} -> Missing: {k}")

