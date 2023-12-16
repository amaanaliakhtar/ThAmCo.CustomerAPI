using System;

namespace ThAmCo.CustomerAPI.Services.Product;

public class ProductDto 
{
    //these must have get and set to be serializable to json
    public int Id { get; set; }
    public string? Ean {get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName {get; set; }
    public int BrandId { get; set; }
    public string? BrandName {get; set; }
    public string? Name { get; set; }
    public string? Description {get; set; }
    public double Price { get; set; }
    public bool InStock { get; set; }
    public DateTime? ExpectedRestock { get; set; }
}