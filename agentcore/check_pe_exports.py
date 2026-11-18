import pefile
import os

agen_dir = r"D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"

for name in ["AgenSoft.exe", "ATPurchaseForms.dll", "ATSalesForms.dll"]:
    path = os.path.join(agen_dir, name)
    try:
        pe = pefile.PE(path)
        print(f"\n*** {name} ***")
        if hasattr(pe, 'DIRECTORY_ENTRY_EXPORT'):
            print("Exports:")
            for exp in pe.DIRECTORY_ENTRY_EXPORT.symbols:
                print(f"  {exp.name.decode() if exp.name else 'None'} @ {hex(exp.address)}")
        if hasattr(pe, 'DIRECTORY_ENTRY_RESOURCE'):
            print("Resource Types:")
            for entry in pe.DIRECTORY_ENTRY_RESOURCE.entries:
                type_name = pefile.RESOURCE_TYPE.get(entry.struct.Id, str(entry.struct.Id))
                print(f"  Type: {type_name}")
                if hasattr(entry, 'directory'):
                    for sub in entry.directory.entries:
                        sub_name = sub.name.string.decode() if sub.name else str(sub.struct.Id)
                        print(f"    Name: {sub_name}")
    except Exception as e:
        print(f"Error {name}: {e}")

