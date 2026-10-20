param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$TargetDb = "MyMandi_2025-26"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$TargetDb;Trusted_Connection=True;")
$conn.Open()

$batches = @(
    # 5. SECTORS (TownID already exists in AgenSoft!)
    @"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Sectors'))
BEGIN
    ALTER TABLE dbo.Sectors ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Sectors ADD [Name] nvarchar(100) NULL;
    ALTER TABLE dbo.Sectors ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Sectors ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Sectors ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Sectors ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Sectors ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Sectors ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Sectors ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END
"@,

    @"
UPDATE dbo.Sectors SET [Name] = [SectorName] WHERE [Name] IS NULL AND [SectorName] IS NOT NULL;
"@,

    # 7. PRODUCTGROUPS (CompanyID already exists in AgenSoft!)
    @"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.ProductGroups'))
BEGIN
    ALTER TABLE dbo.ProductGroups ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.ProductGroups ADD [Name] nvarchar(100) NULL;
    ALTER TABLE dbo.ProductGroups ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.ProductGroups ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.ProductGroups ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.ProductGroups ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.ProductGroups ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.ProductGroups ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.ProductGroups ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END
"@,

    @"
UPDATE dbo.ProductGroups SET [Name] = [GroupName] WHERE [Name] IS NULL AND [GroupName] IS NOT NULL;
"@,

    # 10. PARTIES (SectorID, PartyGroupID, Phone1, Mobile, Address, City already exist in AgenSoft!)
    @"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Parties'))
BEGIN
    ALTER TABLE dbo.Parties ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Parties ADD [PartyNo] nvarchar(50) NULL;
    ALTER TABLE dbo.Parties ADD [Name] nvarchar(150) NULL;
    ALTER TABLE dbo.Parties ADD [PartyType] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Parties ADD [AccountId] int NULL;
    ALTER TABLE dbo.Parties ADD [TownId] int NULL;
    ALTER TABLE dbo.Parties ADD [Phone] nvarchar(50) NULL;
    ALTER TABLE dbo.Parties ADD [CNIC] nvarchar(30) NULL;
    ALTER TABLE dbo.Parties ADD [CreditLimit] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Parties ADD [WHT_Percentage] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Parties ADD [DefaultMarkupRate] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Parties ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Parties ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Parties ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Parties ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Parties ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Parties ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Parties ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END
"@,

    @"
UPDATE dbo.Parties SET 
    [Name] = [PartyName], 
    [PartyNo] = [PartyID], 
    [Phone] = ISNULL([Phone1], [Mobile])
WHERE [Name] IS NULL AND [PartyName] IS NOT NULL;
"@,

    # 11. PRODUCTS (CompanyID, GroupID, UnitID already exist in AgenSoft!)
    @"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    ALTER TABLE dbo.Products ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Products ADD [ProductCode] nvarchar(50) NULL;
    ALTER TABLE dbo.Products ADD [Name] nvarchar(150) NULL;
    ALTER TABLE dbo.Products ADD [ProductGroupId] int NULL;
    ALTER TABLE dbo.Products ADD [CostPrice] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Products ADD [OpeningStock] decimal(18,3) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Products ADD [CurrentStock] decimal(18,3) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Products ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Products ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Products ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Products ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Products ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Products ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Products ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END
"@,

    @"
UPDATE dbo.Products SET 
    [Name] = [ProductName], 
    [ProductCode] = [ProductID], 
    [CostPrice] = ISNULL([PurchasePrice], 0)
WHERE [Name] IS NULL AND [ProductName] IS NOT NULL;
"@
)

$idx = 1
foreach ($b in $batches) {
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $b
        $cmd.CommandTimeout = 60
        $cmd.ExecuteNonQuery() | Out-Null
        Write-Host "Batch $idx executed successfully!" -ForegroundColor Green
    } catch {
        Write-Host "Batch $idx Error: $($_.Exception.Message)" -ForegroundColor Red
    }
    $idx++
}

$conn.Close()
Write-Host "`nAll compatibility batches completed!" -ForegroundColor Green
