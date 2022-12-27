using System.Net;
using Bogus;
using Catalog.Service.Domain.Models;
using Catalog.Service.IntegrationTests.Clients;
using FluentAssertions;
using Refit;

namespace Catalog.Service.IntegrationTests.EndpointTests;

public sealed class GetProductEndpointTests : IClassFixture<CatalogApiFactory>
{
    private readonly CatalogApiFactory _apiFactory;
    private readonly ICatalogClient _client;
    
    private readonly Product _persistedProduct = new Faker<Product>()
        .CustomInstantiator(faker =>
            Product.Create(
                faker.Random.String2(5, 64),
                faker.Random.String2(5, 512),
                faker.Finance.Amount(1m, 10m, 4)));

    public GetProductEndpointTests(CatalogApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = RestService.For<ICatalogClient>(apiFactory.CreateClient());
    }
    
    [Fact]
    public async Task GetProduct_ReturnsOkWithResponse_WhenRequestIsValid()
    {
        await _apiFactory.InsertProductsForEndpointTesting(_persistedProduct);
        ApiResponse<ProductDto> response = await _client.GetProduct(_persistedProduct.Id);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(Product.AsDto(_persistedProduct));
    }

    [Fact]
    public async Task GetProduct_ReturnsBadRequest_WhenRequestIsNotValid()
    {
        ApiResponse<ProductDto> response = await _client.GetProduct(Guid.Empty);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenEntityNotFound()
    {
        ApiResponse<ProductDto> response = await _client.GetProduct(Guid.NewGuid());
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}