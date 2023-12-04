using System;

namespace ThAmCo.CustomerAPI.Services.Product;

public class ProductDto 
{
    //these must have get and set to be serializable as json
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
}