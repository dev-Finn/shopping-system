using Catalog.Service.Domain.Repositories;
using Catalog.Service.Endpoints.Products;
using Catalog.Service.Infrastructure;
using Catalog.Service.Infrastructure.Repositories;
using Catalog.Service.Infrastructure.SieveProcessors;
using Catalog.Service.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Sieve.Models;
using Sieve.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
builder.Services.AddScoped<ISieveProcessor, ProductSieveProcessor>();
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console();
});

builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddDbContext<CatalogContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDb"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "Catalog Api", Version = "v1"});
    c.EnableAnnotations();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

WebApplication app = builder.Build();
app.UseSerilogRequestLogging();
app.UseMiddleware<CatalogServiceExceptionMiddleware>();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapSwagger();
app.Run();