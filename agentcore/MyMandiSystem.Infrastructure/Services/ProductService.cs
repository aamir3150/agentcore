using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ProductService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Product?> GetProductByCodeAsync(string code)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Products.FirstOrDefaultAsync(p => p.ProductCode == code);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            return await context.Products
                .Include(p => p.Company)
                .Include(p => p.Unit)
                .Include(p => p.ProductGroup)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }
        catch
        {
            var list = new List<Product>();
            try
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    IF OBJECT_ID('Products', 'U') IS NOT NULL
                    BEGIN
                        SELECT ProductID, ProductName, ISNULL(PurchasePrice, 0), ISNULL(SalePrice, 0), ISNULL(NameUrdu, ''), ISNULL(BarCode, '')
                        FROM Products
                        ORDER BY ProductName;
                    END";

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Product
                    {
                        ProductCode = reader[0]?.ToString()?.Trim() ?? string.Empty,
                        Name = reader[1]?.ToString()?.Trim() ?? string.Empty,
                        PurchasePrice = reader[2] != DBNull.Value ? Convert.ToDecimal(reader[2]) : 0m,
                        SalePrice = reader[3] != DBNull.Value ? Convert.ToDecimal(reader[3]) : 0m,
                        UrduName = reader[4]?.ToString()?.Trim() ?? string.Empty,
                        BarcodeNo = reader[5]?.ToString()?.Trim() ?? string.Empty
                    });
                }
            }
            catch
            {
            }
            return list;
        }
    }

    public async Task<IEnumerable<ProductGroup>> GetAllProductGroupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ProductGroups.Where(pg => !pg.IsDeleted).ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        if (product.CompanyId <= 0)
        {
            var defaultComp = await context.Companies.FirstOrDefaultAsync(c => c.IsActive) 
                           ?? await context.Companies.FirstOrDefaultAsync();
            if (defaultComp != null) product.CompanyId = defaultComp.Id;
        }

        if (product.UnitId <= 0)
        {
            var defaultUnit = await context.Units.FirstOrDefaultAsync();
            if (defaultUnit != null) product.UnitId = defaultUnit.Id;
        }

        context.Products.Add(product);
        await context.SaveChangesAsync();

        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                UPDATE Products 
                SET ProductID = CAST(Id AS VARCHAR(5)),
                    ProductName = Name,
                    NameUrdu = ISNULL(UrduName, Name),
                    PurchasePrice = CostPrice
                WHERE Id = {0}", product.Id);
        }
        catch { }
    }

    public async Task UpdateProductAsync(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Products.Update(product);
        await context.SaveChangesAsync();

        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                UPDATE Products 
                SET ProductID = CAST(Id AS VARCHAR(5)),
                    ProductName = Name,
                    NameUrdu = ISNULL(UrduName, Name),
                    PurchasePrice = CostPrice
                WHERE Id = {0}", product.Id);
        }
        catch { }
    }

    public async Task DeleteProductAsync(int productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var product = await context.Products.FindAsync(productId);
        if (product != null)
        {
            product.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    // --- Product Groups ---

    public async Task AddProductGroupAsync(ProductGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.ProductGroups.Add(group);
        await context.SaveChangesAsync();
    }

    public async Task UpdateProductGroupAsync(ProductGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.ProductGroups.Update(group);
        await context.SaveChangesAsync();
    }

    public async Task DeleteProductGroupAsync(int groupId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var group = await context.ProductGroups.FindAsync(groupId);
        if (group != null)
        {
            group.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }
}
