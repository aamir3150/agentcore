import clr
import sys
import os

agensoft_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
sys.path.append(agensoft_dir)

import System
from System.Reflection import Assembly, BindingFlags

def inspect_assembly(path):
    print(f"=== Inspecting {os.path.basename(path)} ===")
    asm = Assembly.LoadFrom(path)
    for t in asm.GetTypes():
        if "Purchase" in t.Name or "Sale" in t.Name or "Multi" in t.Name:
            print(f"Type: {t.FullName} (Base: {t.BaseType})")

try:
    inspect_assembly(os.path.join(agensoft_dir, "ATPurchaseForms.dll"))
    inspect_assembly(os.path.join(agensoft_dir, "ATSalesForms.dll"))
except Exception as e:
    print(f"Reflection error: {e}")

