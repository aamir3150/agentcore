using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IProductService
{
    Task<Product?> GetProductByCodeAsync(string code);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int productId);

    // Product Groups
    Task<IEnumerable<ProductGroup>> GetAllProductGroupsAsync();
    Task AddProductGroupAsync(ProductGroup group);
    Task UpdateProductGroupAsync(ProductGroup group);
    Task DeleteProductGroupAsync(int groupId);
}
