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
        return await context.Products
            .Include(p => p.Company)
            .Include(p => p.Unit)
            .Include(p => p.ProductGroup)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductGroup>> GetAllProductGroupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ProductGroups.Where(pg => !pg.IsDeleted).ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Products.Add(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Products.Update(product);
        await context.SaveChangesAsync();
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
