param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$TargetDb = "MyMandi_2025-26"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$TargetDb;Trusted_Connection=True;")
$conn.Open()

function Ensure-Column($table, $col, $type, $default = $null) {
    $checkSql = "SELECT COUNT(*) FROM sys.columns WHERE name = '$col' AND object_id = OBJECT_ID('dbo.[$table]')"
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $checkSql
    $count = [int]$cmd.ExecuteScalar()
    if ($count -eq 0) {
        $defStr = if ($default) { " DEFAULT $default" } else { "" }
        $alterSql = "ALTER TABLE dbo.[$table] ADD [$col] $type$defStr;"
        $cmdAlter = $conn.CreateCommand()
        $cmdAlter.CommandText = $alterSql
        $cmdAlter.ExecuteNonQuery() | Out-Null
        Write-Host "Added column [$col] to table [$table]" -ForegroundColor Green
    }
}

# Sectors
Ensure-Column "Sectors" "Name" "nvarchar(100) NULL"
Ensure-Column "Sectors" "IsActive" "bit NOT NULL" "1"
Ensure-Column "Sectors" "RowVersion" "rowversion NULL"
Ensure-Column "Sectors" "CreatedBy" "int NOT NULL" "1"
Ensure-Column "Sectors" "CreatedAt" "datetime2 NOT NULL" "GETDATE()"
Ensure-Column "Sectors" "UpdatedBy" "int NULL"
Ensure-Column "Sectors" "UpdatedAt" "datetime2 NULL"
Ensure-Column "Sectors" "IsDeleted" "bit NOT NULL" "0"

# ProductGroups
Ensure-Column "ProductGroups" "Name" "nvarchar(100) NULL"
Ensure-Column "ProductGroups" "IsActive" "bit NOT NULL" "1"
Ensure-Column "ProductGroups" "RowVersion" "rowversion NULL"
Ensure-Column "ProductGroups" "CreatedBy" "int NOT NULL" "1"
Ensure-Column "ProductGroups" "CreatedAt" "datetime2 NOT NULL" "GETDATE()"
Ensure-Column "ProductGroups" "UpdatedBy" "int NULL"
Ensure-Column "ProductGroups" "UpdatedAt" "datetime2 NULL"
Ensure-Column "ProductGroups" "IsDeleted" "bit NOT NULL" "0"

# Parties
Ensure-Column "Parties" "AccountId" "int NULL"
Ensure-Column "Parties" "TownId" "int NULL"
Ensure-Column "Parties" "Phone" "nvarchar(50) NULL"
Ensure-Column "Parties" "CNIC" "nvarchar(30) NULL"
Ensure-Column "Parties" "CreditLimit" "decimal(18,2) NOT NULL" "0"
Ensure-Column "Parties" "WHT_Percentage" "decimal(18,2) NOT NULL" "0"
Ensure-Column "Parties" "DefaultMarkupRate" "decimal(18,2) NOT NULL" "0"
Ensure-Column "Parties" "IsActive" "bit NOT NULL" "1"
Ensure-Column "Parties" "RowVersion" "rowversion NULL"
Ensure-Column "Parties" "CreatedBy" "int NOT NULL" "1"
Ensure-Column "Parties" "CreatedAt" "datetime2 NOT NULL" "GETDATE()"
Ensure-Column "Parties" "UpdatedBy" "int NULL"
Ensure-Column "Parties" "UpdatedAt" "datetime2 NULL"
Ensure-Column "Parties" "IsDeleted" "bit NOT NULL" "0"

# Products
Ensure-Column "Products" "ProductGroupId" "int NULL"
Ensure-Column "Products" "CostPrice" "decimal(18,2) NOT NULL" "0"
Ensure-Column "Products" "OpeningStock" "decimal(18,3) NOT NULL" "0"
Ensure-Column "Products" "CurrentStock" "decimal(18,3) NOT NULL" "0"
Ensure-Column "Products" "IsActive" "bit NOT NULL" "1"
Ensure-Column "Products" "RowVersion" "rowversion NULL"
Ensure-Column "Products" "CreatedBy" "int NOT NULL" "1"
Ensure-Column "Products" "CreatedAt" "datetime2 NOT NULL" "GETDATE()"
Ensure-Column "Products" "UpdatedBy" "int NULL"
Ensure-Column "Products" "UpdatedAt" "datetime2 NULL"
Ensure-Column "Products" "IsDeleted" "bit NOT NULL" "0"

# Sync data for display names
$updateCmd = $conn.CreateCommand()
$updateCmd.CommandText = @"
UPDATE dbo.Sectors SET [Name] = [SectorName] WHERE [Name] IS NULL AND [SectorName] IS NOT NULL;
UPDATE dbo.ProductGroups SET [Name] = [GroupName] WHERE [Name] IS NULL AND [GroupName] IS NOT NULL;
UPDATE dbo.Parties SET [Name] = [PartyName], [PartyNo] = [PartyID], [Phone] = ISNULL([Phone1], [Mobile]) WHERE [Name] IS NULL AND [PartyName] IS NOT NULL;
UPDATE dbo.Products SET [Name] = [ProductName], [ProductCode] = [ProductID], [CostPrice] = ISNULL([PurchasePrice], 0) WHERE [Name] IS NULL AND [ProductName] IS NOT NULL;
"@
$updateCmd.ExecuteNonQuery() | Out-Null
Write-Host "All missing columns ensured and synced successfully!" -ForegroundColor Green

$conn.Close()
