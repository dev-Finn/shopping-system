using System.Net;
using Bogus;
using Catalog.Service.Domain.Models;
using Catalog.Service.IntegrationTests.Clients;
using FluentAssertions;
using Refit;

namespace Catalog.Service.IntegrationTests.EndpointTests;

public sealed class DeleteProductEndpointTests : IClassFixture<CatalogApiFactory>
{
    private readonly CatalogApiFactory _apiFactory;
    private readonly ICatalogClient _client;

    private readonly Product _existingProduct = new Faker<Product>()
        .CustomInstantiator(faker =>
            Product.Create(
                faker.Random.String2(5, 64),
                faker.Random.String2(5, 512),
                faker.Random.Decimal(1m, 10m)));

    public DeleteProductEndpointTests(CatalogApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = RestService.For<ICatalogClient>(apiFactory.CreateClient());
    }
    
    [Fact]
    public async Task DeleteProduct_ReturnsOk_WhenRequestIsValid()
    {
        await _apiFactory.InsertProductsForEndpointTesting(_existingProduct);
        IApiResponse response = await _client.DeleteProduct(_existingProduct.Id);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task DeleteProduct_ReturnsBadRequest_WhenRequestIsNotValid()
    {
        IApiResponse response = await _client.DeleteProduct(Guid.Empty);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WhenEntityNotFound()
    {
        IApiResponse response = await _client.DeleteProduct(Guid.NewGuid());
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}