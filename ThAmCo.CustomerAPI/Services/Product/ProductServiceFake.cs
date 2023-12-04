using System;

namespace ThAmCo.CustomerAPI.Services.Product;

public class ProductServiceFake : IProductService
{
    private readonly ProductDto[] _products =
    {
            new ProductDto { Id = 1, Name = "Nike Air Max 1", Price = 130.00, Stock = 1 },
            new ProductDto { Id = 2, Name = "Nike Air Max 90", Price = 95.00, Stock = 4 },
            new ProductDto { Id = 3, Name = "Adidas Ultraboost", Price = 120.00, Stock = 6 },
            new ProductDto { Id = 4, Name = "Adidas NMD", Price = 100.00, Stock = 5 },
            new ProductDto { Id = 5, Name = "New Balance 990", Price = 200.00, Stock = 13 },
            new ProductDto { Id = 5, Name = "Customer API", Price = 0.00, Stock = 1 }

        };

    public Task<ProductDto?> GetProductAsync(int id)
    {
        var product = _products.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}