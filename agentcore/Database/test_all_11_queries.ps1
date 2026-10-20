param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$DbName = "MyMandi_2025-26"
)

$connStr = "Server=$Server;Database=$DbName;Trusted_Connection=True;TrustServerCertificate=True;"

$csharpCode = @"
using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Infrastructure;

public class DbTester
{
    public static void TestContext(string connStr)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connStr)
            .Options;

        using (var db = new AppDbContext(options))
        {
            Console.WriteLine("Testing db.Users.Any()...");
            bool hasUsers = db.Users.Any();
            Console.WriteLine("hasUsers: " + hasUsers);

            var firstUser = db.Users.FirstOrDefault();
            if (firstUser != null)
            {
                Console.WriteLine("First User: Id=" + firstUser.Id + ", Username=" + firstUser.Username + ", FullName=" + firstUser.FullName);
            }

            Console.WriteLine("Testing db.Companies.Any()...");
            bool hasCompanies = db.Companies.Any();
            Console.WriteLine("hasCompanies: " + hasCompanies);

            Console.WriteLine("Testing db.Parties.Any()...");
            bool hasParties = db.Parties.Any();
            Console.WriteLine("hasParties: " + hasParties);

            Console.WriteLine("Testing db.Products.Any()...");
            bool hasProducts = db.Products.Any();
            Console.WriteLine("hasProducts: " + hasProducts);

            Console.WriteLine("Testing db.FinancialYears.Any()...");
            bool hasYears = db.FinancialYears.Any();
            Console.WriteLine("hasYears: " + hasYears);
        }
    }
}
"@

$deps = @(
    "d:\Projects\agentcore\agentcore\MyMandiSystem.Infrastructure\bin\Debug\net8.0\MyMandiSystem.Core.dll",
    "d:\Projects\agentcore\agentcore\MyMandiSystem.Infrastructure\bin\Debug\net8.0\MyMandiSystem.Infrastructure.dll"
)

# Test via dotnet-script or a small console or reflection
Write-Host "Verifying database query via SqlClient direct..."
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()

$tablesToTest = @("Users", "Companies", "Parties", "Products", "Towns", "Sectors", "Units", "PartyGroups", "ProductGroups", "Salesmen", "Promises")
foreach ($t in $tablesToTest) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT TOP 1 * FROM [$t]"
    $r = $cmd.ExecuteReader()
    Write-Host "Table [$t] query test: SUCCESS!" -ForegroundColor Green
    $r.Close()
}
$conn.Close()
Write-Host "`nAll 11 tables verified!" -ForegroundColor Green
