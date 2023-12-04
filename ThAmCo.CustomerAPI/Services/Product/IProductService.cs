using System;

namespace ThAmCo.CustomerAPI.Services.Product;

public interface IProductService 
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task<ProductDto?> GetProductAsync(int id);
}