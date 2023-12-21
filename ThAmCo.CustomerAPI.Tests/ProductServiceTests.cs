using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using ThAmCo.CustomerAPI.Controllers;
using ThAmCo.CustomerAPI.Services.Product;

namespace ThAmCo.CustomerAPI.Tests;

[TestClass]
public class ProductServiceTest
{
    private static ProductDto[] GetTestProducts() => new ProductDto[] {
        new ProductDto { Id = 1, Name = "Nike Air Max 1", CategoryName = "Shoes", BrandName = "Nike", Price = 130.00, InStock = true },
        new ProductDto { Id = 2, Name = "Nike Air Max 90", CategoryName = "Shoes", BrandName = "Nike", Price = 95.00, InStock = true },
        new ProductDto { Id = 3, Name = "Adidas Ultraboost", CategoryName = "Shoes", BrandName = "Adidas", Price = 120.00, InStock = true },
        new ProductDto { Id = 4, Name = "Adidas NMD", CategoryName = "Shoes", BrandName = "Adidas", Price = 100.00, InStock = false },            
        new ProductDto { Id = 5, Name = "New Balance 990", CategoryName = "Shoes", BrandName = "New Balance", Price = 200.00, InStock = false }
    };

    private static
        Mock<HttpMessageHandler> CreateHttpMock(HttpStatusCode expectedCode, string? expectedJson)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = expectedCode
            };

            if (expectedJson != null)
            {
                response.Content = new StringContent(expectedJson, Encoding.UTF8, "application/json");
            }

            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();

            return mock;
        }

        private static IProductService CreateProductService(HttpClient client)
        {
            var mockConfiguration = new Mock<IConfiguration>(MockBehavior.Strict);

            mockConfiguration.Setup(c => c["WebServices:Products:BaseUrl"]).Returns("https://localhost:7065/");
            mockConfiguration.Setup(c => c["Auth:Authority"]).Returns("https://dev-fcu3j3tbm1e1imkf.us.auth0.com/");
            mockConfiguration.Setup(c => c["Auth:AuthAudience"]).Returns("https://secure-customer-api.com");
            mockConfiguration.Setup(c => c["Auth:ClientId"]).Returns("ClientId");
            mockConfiguration.Setup(c => c["Auth:ClientSecret"]).Returns("ClientSecret");

            return new ProductService(client, mockConfiguration.Object);
        }

        [TestMethod]
        public async Task GetProductAsync_WithValid_ShouldOkEntity()
        {
            // Arrange
            var expectedResult = new ProductDto { Id = 1, Name = "Nike Air Max 1", CategoryName = "Shoes", BrandName = "Nike", Price = 130.00, InStock = true };
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("https://localhost:7065/product/1");

            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);

            // var authMock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            // var authClient = new HttpClient(authMock.Object);

            var service =  CreateProductService(client);

            // Act
            var result = await service.GetProductAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult.Id, result.Id);
            // FIXME: could assert other result property values
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(
                            req => req.Method == HttpMethod.Get
                                   && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                        );
        }

        [TestMethod]
        public async Task GetProductAsync_WithInvalid_ShouldReturnNull()
        {
            // Arrange
            var expectedUri = new Uri("https://localhost:7065/product/100");
            var expectedResult = new ProductDto { Id = 1, Name = "Nike Air Max 1", CategoryName = "Shoes", BrandName = "Nike", Price = 130.00, InStock = true };
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            

            var mock = CreateHttpMock(HttpStatusCode.NotFound, null);
            var client = new HttpClient(mock.Object);

            // var authMock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            // var authClient = new HttpClient(authMock.Object);

            var service = CreateProductService(client);

            // Act
            var result = await service.GetProductAsync(100);

            // Assert
            Assert.IsNull(result);
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(
                            req => req.Method == HttpMethod.Get
                                   && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                        );
        }

        [TestMethod]
        public async Task GetProductAsync_OnHttpBad_ShouldThrow()
        {
            // Arrange
            var expectedUri = new Uri("https://localhost:7065/product/1");
            

            var mock = CreateHttpMock(HttpStatusCode.ServiceUnavailable, null);
            var client = new HttpClient(mock.Object);

            // var authMock = CreateHttpMock(HttpStatusCode.ServiceUnavailable, null);
            // var authClient =new HttpClient(authMock.Object);

            var service = CreateProductService(client);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(
                () => service.GetProductAsync(1));
        }

        [TestMethod]
        public async Task GetProductsAsync_WithValid_ShouldAllProductList()
        {
            // Arrange
            var expectedResult = GetTestProducts();
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("https://localhost:7065/product");

            var expectedToken = new TokenDto { access_token = "access", token_type = "type", expires_in = 1 };
            var tokenJson = JsonConvert.SerializeObject(expectedToken);

            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);

            // var authMock = CreateHttpMock(HttpStatusCode.OK, tokenJson);
            // var authClient =new HttpClient(authMock.Object);

            var service = CreateProductService(client);

            // Act
            var result = (await service.GetProductsAsync()).ToArray();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(expectedResult[i].Id, result[i].Id);
                Assert.AreEqual(expectedResult[i].Name, result[i].Name);
                Assert.AreEqual(expectedResult[i].Ean, result[i].Ean);
                Assert.AreEqual(expectedResult[i].BrandName, result[i].BrandName);
                Assert.AreEqual(expectedResult[i].CategoryName, result[i].CategoryName);
                Assert.AreEqual(expectedResult[i].Price, result[i].Price);
            }
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(
                            req => req.Method == HttpMethod.Get
                                   && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                        );
        }

        // [TestMethod]
        // public async Task GetProductsAsync_OnHttpBad_ShouldThrow()
        // {
        //     var expectedResult = GetTestProducts();
        //     var expectedJson = JsonConvert.SerializeObject(expectedResult);
        //     var expectedUri = new Uri("https://localhost:7065/product");

        //     var expectedToken = new TokenDto { access_token = "access", token_type = "type", expires_in = 1 };
        //     var tokenJson = JsonConvert.SerializeObject(expectedToken);

        //     var mock = CreateHttpMock(HttpStatusCode.BadRequest, expectedJson);
        //     var client = new HttpClient(mock.Object);

        //     var authMock = CreateHttpMock(HttpStatusCode.OK, tokenJson);
        //     var authClient = new HttpClient(authMock.Object);

        //     var service = CreateProductService(client, authClient);

        //     // Act
        //     var result = (await service.GetProductsAsync()).ToArray();

        //     // Assert
        //     mock.Protected()
        //         .Verify("SendAsync",
        //                 Times.Once(),
        //                 ItExpr.Is<HttpRequestMessage>(
        //                     req => req.Method == HttpMethod.Get
        //                            && req.RequestUri == expectedUri),
        //                 ItExpr.IsAny<CancellationToken>()
        //                 );
        // }

    
}

internal class TokenDto
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
}