# AgenSoft Reverse Engineering & Exact Parity Rules (AgentCore)

## 1. Mandatory REA (Reverse Engineer Anything) Rule
Jab bhi AgenSoft ke kisi bhi module, form, feature, calculation, ya database query ko scan ya implement karne ki request aayi ho:
1. **Hamesha REA MCP Tool / Engine Use Karna Lazmi Hai**:
   - Kabhi bhi UI controls, field names, calculations ya database queries ko guess (estimate ya andaza) nahi lagana.
   - Hamesha binary analysis aur REA tools ke zariye exact controls, exact queries aur exact table columns nikaal kar implement karna hai.
   - Goal: **AgentCore aur AgenSoft mein 0.0001% bhi farq na ho**.

## 2. AgenSoft Binaries Path & Module Mapping
Tamam AgenSoft binaries yahan mojood hain:
`D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\`

Module-wise binary mapping:
- **Master Definitions (Parties, Products, Towns, Sectors, Units, Companies)**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATDefintionForms.dll`
- **Banking, Cheques & Vouchers (Cash Receipt, Payment, Bank Cheques, Deposits)**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountsForms.dll`
- **Purchase & Procurement (Kharidari Sauda, Purchase Invoices, Weighments)**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATPurchaseForms.dll`
- **Sales & Bikri (Farokht Sauda, Sale Invoices, Returns)**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATSalesForms.dll`
- **Brokerage / Aarhat (Commission, Shortages, GatePass)**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATBrokrageForms.dll`
- **Reports (Ledger, CashBook, Stock, Trial Balance)**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AccountReports.dll`
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATRptSales.dll`
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATRptPurchase.dll`
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATRptStock.dll`
- **Database Engine & DDL**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll`
- **Main Shell**:
  `D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\AgenSOFT.exe`

## 3. Form Scanning Standard Checklist (Every Form Must Extract)
Har form ke scan ke doran yeh 4 cheezein lazmi nikaalni hain:
1. **Control Names & Types**: Har TextBox, ComboBox, Grid, CheckBox, Label ka exact name.
2. **Keyboard Shortcuts / Hotkeys**: (e.g. F1 Search, F2 Save, F3 Clear, F5 Print, Enter for Next Control).
3. **Fetching Queries (SELECT)**: Form khulte waqt ya search karte waqt AgenSoft database se data kis exact SQL se lata hai.
4. **Saving Queries (INSERT / UPDATE)**: Save button dabane par kin exact tables aur columns mein data jata hai aur balances kaise update hote hain.

## 4. Pure AgenSoft Database Parity
- Koi naya table ya extra column create nahi karna.
- EF Core Migrations run nahi karni (`Database.Migrate()` prohibited).
- Direct AgenSoft ke 89 tables aur unke exact columns par data store aur fetch karna hai.

