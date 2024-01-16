using System;
using System.Net;

namespace ThAmCo.CustomerAPI.Services.Product;      //Test deployment

public class ProductService : IProductService
{
    private readonly HttpClient _client;

    public ProductService(HttpClient client, IConfiguration config)
    {
        var baseUrl = config["WebServices:Products:BaseUrl"];
        client.BaseAddress = new System.Uri(baseUrl);
        // client.BaseAddress = new System.Uri("http://localhost:3733/");
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client = client;
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        var response = await _client.GetAsync("/product/" + id);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadAsAsync<ProductDto>();

        return product;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var response = await _client.GetAsync("/product");
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadAsAsync<IEnumerable<ProductDto>>();
        
        return products;
    }
}