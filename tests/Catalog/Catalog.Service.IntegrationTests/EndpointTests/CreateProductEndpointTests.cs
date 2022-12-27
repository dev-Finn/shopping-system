using System.Net;
using Bogus;
using Catalog.Service.Domain.Models;
using Catalog.Service.IntegrationTests.Clients;
using FluentAssertions;
using Refit;

namespace Catalog.Service.IntegrationTests.EndpointTests;

public sealed class CreateProductEndpointTests : IClassFixture<CatalogApiFactory>
{
    private readonly CatalogApiFactory _apiFactory;
    private readonly ICatalogClient _client;

    private readonly Product _duplicateProduct = new Faker<Product>()
        .CustomInstantiator(faker =>
            Product.Create(
                faker.Random.String2(5, 64),
                faker.Random.String2(5, 512),
                faker.Random.Decimal(1m, 10m)))
        .Generate();

    private readonly CreateProductRequest _validRequest = new Faker<CreateProductRequest>()
        .CustomInstantiator(faker =>
            new CreateProductRequest(
                faker.Random.String2(5, 64),
                faker.Random.String2(5, 512),
                faker.Random.Decimal(1m, 10m)))
        .Generate();

    private readonly CreateProductRequest _invalidRequest = new Faker<CreateProductRequest>()
        .CustomInstantiator(faker =>
            new CreateProductRequest(
                faker.Random.String2(0, 4),
                faker.Random.String2(0, 512),
                faker.Random.Decimal(0m, 10m)))
        .Generate();

    public CreateProductEndpointTests(CatalogApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = RestService.For<ICatalogClient>(apiFactory.CreateClient());
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreated_WhenRequestIsValid()
    {
        IApiResponse response = await _client.CreateProduct(_validRequest);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WhenRequestIsNotValid()
    {
        IApiResponse response = await _client.CreateProduct(_invalidRequest);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_ReturnsInternalServerError_WhenViolatingProductIndex()
    {
        await _apiFactory.InsertProductsForEndpointTesting(_duplicateProduct);
        IApiResponse response = await _client.CreateProduct(new CreateProductRequest(_duplicateProduct.Name,
                _duplicateProduct.Description, _duplicateProduct.Price));
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}