using Microsoft.AspNetCore.Mvc;
using ThAmCo.CustomerAPI.Services.Product;

namespace ThAmCo.CustomerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{

    private readonly ILogger _logger;
    private readonly IProductService _ProductService;

    public CustomerController(ILogger<CustomerController> logger, IProductService ProductService)
    {
        _logger = logger;
        _ProductService = ProductService;
    }

    //GET: /product
    [HttpGet("/product")]
    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        IEnumerable<ProductDto> products;
        try
        {
            products = await _ProductService.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using Products service.");
            products = Array.Empty<ProductDto>();
        }

        return products;
    }

    //GET: /Product/{id}
    [HttpGet("/product/{id}")]
    public async Task<ProductDto?> GetProductAsync(int id)
    {
        // if (id == null)
        // {
        //     return BadRequest();
        // }

        try
        {
            var product = await _ProductService.GetProductAsync(id);
            // if (product == null)
            // {
            //     return NotFound();
            // }
            return product;
        }
        catch
        {
            _logger.LogWarning("Exception occurred using Products service.");
            return null;
        }
    }
}
