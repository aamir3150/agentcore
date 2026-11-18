import re

def show_urdu_lines(path):
    print(f"=== {path} ===")
    with open(path, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    for i, line in enumerate(lines):
        if re.search(r'[\u0600-\u06FF]', line):
            print(f"L{i+1}: {line.strip()}")

show_urdu_lines(r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralPurchaseInvoiceViewModel.cs')
show_urdu_lines(r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralSaleInvoiceViewModel.cs')
show_urdu_lines(r'd:\Projects\agentcore\agentcore\MyMandiSystem\ViewModels\GeneralMultiInvoiceViewModel.cs')

