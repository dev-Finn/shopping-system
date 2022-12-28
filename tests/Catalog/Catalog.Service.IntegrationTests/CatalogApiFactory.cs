using Catalog.Service.Domain.Models;
using Catalog.Service.Infrastructure;
using Catalog.Service.IntegrationTests.Clients;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Refit;
using Serilog;

namespace Catalog.Service.IntegrationTests;

public sealed class CatalogApiFactory : WebApplicationFactory<ICatalogAssemblyMarker>, IAsyncLifetime
{
    public ILogger Logger { get; set; }

    private readonly PostgreSqlTestcontainer _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithAutoRemove(true)
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "integrationTestDb",
            Username = "Testuser",
            Password = "testpassword"
        }).Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var settings = new RefitSettings(
            new NewtonsoftJsonContentSerializer(
                new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        }));
        
        builder.ConfigureLogging(c => c.AddSerilog(Logger));
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(CatalogContext));
            services.RemoveAll<DbContextOptions<CatalogContext>>();
            services.AddDbContext<CatalogContext>(options =>
            {
                options.UseNpgsql(_dbContainer.ConnectionString);
            });
            services.AddRefitClient<ICatalogClient>(_ => settings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:80"));
        });
        base.ConfigureWebHost(builder);
    }
    
    public async Task InsertProductsForEndpointTesting(params Product[] products)
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        CatalogContext context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await using var asyncScope = Services.CreateAsyncScope();
        CatalogContext context = asyncScope.ServiceProvider.GetRequiredService<CatalogContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}