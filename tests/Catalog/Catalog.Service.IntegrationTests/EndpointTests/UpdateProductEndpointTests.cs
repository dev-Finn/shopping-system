using System.Net;
using Bogus;
using Catalog.Service.Domain.Models;
using Catalog.Service.IntegrationTests.Clients;
using FluentAssertions;
using Refit;

namespace Catalog.Service.IntegrationTests.EndpointTests;

public sealed class UpdateProductEndpointTests : IClassFixture<CatalogApiFactory>
{
    private readonly CatalogApiFactory _apiFactory;
    private readonly ICatalogClient _client;

    private readonly Product _persistedProduct = new Faker<Product>()
        .CustomInstantiator(faker =>
            Product.Create(
                faker.Random.String2(5, 64),
                faker.Random.String2(5, 512),
                faker.Random.Decimal(1m, 10m)));

    private readonly UpdateProductRequest _validRequest = new Faker<UpdateProductRequest>()
        .CustomInstantiator(faker =>
            new UpdateProductRequest(
                faker.Random.String2(5, 64),
                faker.Random.String2(5, 512),
                faker.Random.Decimal(1m, 5m)))
        .Generate();

    public UpdateProductEndpointTests(CatalogApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = RestService.For<ICatalogClient>(apiFactory.CreateClient());
    }

    [Fact]
    public async Task UpdateProduct_ReturnsOk_WhenRequestIsValid()
    {
        await _apiFactory.InsertProductsForEndpointTesting(_persistedProduct);
        IApiResponse response = await _client.UpdateProduct(_persistedProduct.Id, _validRequest);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequest_WhenRequestIsNotValid()
    {
        IApiResponse response = await _client.UpdateProduct(Guid.Empty, _validRequest);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNotFound_WhenEntityNotFound()
    {
        IApiResponse response = await _client.UpdateProduct(Guid.NewGuid(), _validRequest);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}