using System.Net;
using Bogus;
using Catalog.Service.Domain.Models;
using Catalog.Service.IntegrationTests.Clients;
using FluentAssertions;
using Refit;

namespace Catalog.Service.IntegrationTests.EndpointTests;

public sealed class GetPaginatedProductsEndpointTests : IClassFixture<CatalogApiFactory>
{
    private readonly CatalogApiFactory _apiFactory;
    private readonly ICatalogClient _client;

    private readonly Faker<Product> _faker = new Faker<Product>()
        .CustomInstantiator(faker => Product.Create(
            faker.Random.String2(5, 64),
            faker.Random.String2(5, 512),
            faker.Finance.Amount(1m, 10m, 4)));

    private readonly Product[] _products;

    private readonly SieveModel _validSieveModel = new(1, 100);
    
    private readonly SieveModel _notValidSieveModel = new Faker<SieveModel>()
        .CustomInstantiator(faker =>
            new SieveModel(
                faker.Random.Int(-100, 0),
                faker.Random.Int(-100, 0)))
        .Generate();

    public GetPaginatedProductsEndpointTests(CatalogApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = RestService.For<ICatalogClient>(apiFactory.CreateClient());
        _products = Enumerable.Range(1, 100).Select(_ => _faker.Generate()).ToArray();
    }

    [Fact]
    public async Task GetPaginated_ReturnsOKWithPaginatedProductsResponse_WhenRequestIsValid()
    {
        await _apiFactory.InsertProductsForEndpointTesting(_products);
        ApiResponse<GetPaginatedProductsResponse> response = await _client.GetPaginatedProducts(_validSieveModel);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content?.CurrentPage.Should().Be(_validSieveModel.Page);
        response.Content?.PageSize.Should().Be(_validSieveModel.PageSize);
        response.Content?.HasNext.Should().BeFalse();
        response.Content?.Data.Should().NotBeEmpty();
        response.Content?.Data.Should().BeEquivalentTo(_products.Select(Product.AsDto));
    }

    [Fact]
    public async Task GetPaginated_ReturnsBadRequest_WhenRequestIsNotValid()
    {
        ApiResponse<GetPaginatedProductsResponse> response = await _client.GetPaginatedProducts(_notValidSieveModel);
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}