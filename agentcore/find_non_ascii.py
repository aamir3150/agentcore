import re

def clean_file(path, replacements):
    with open(path, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
    
    for old, new in replacements.items():
        content = content.replace(old, new)
        
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)

# Test read and replace
for fpath in [r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralPurchaseInvoiceViewModel.cs',
              r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralSaleInvoiceViewModel.cs',
              r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralMultiInvoiceViewModel.cs']:
    with open(fpath, 'rb') as f:
        data = f.read()
    non_ascii = set(re.findall(rb'[^\x00-\x7F]+', data))
    print(fpath, f'has {len(non_ascii)} non-ascii byte sequences')
    for seq in list(non_ascii)[:10]:
        print(' ', repr(seq))

