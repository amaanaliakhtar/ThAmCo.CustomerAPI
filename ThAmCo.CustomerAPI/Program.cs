using ThAmCo.CustomerAPI.Services.Product;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var environment = builder.Configuration.GetSection("BuildConfig");

if (environment.Value != null && environment.Value.Equals("Develop"))
{
    builder.Services.AddTransient<IProductService, ProductServiceFake>();
}
else
{
    builder.Services.AddHttpClient<IProductService, ProductService>();
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
