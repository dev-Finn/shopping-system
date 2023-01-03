using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using Ordering.Components;
using Serilog;
using Serilog.Core;
using Shared.MassTransit.Options;
using Shared.Telemetry.Extensions;

Logger logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(logger);

builder.Services
    .ConfigureOptions<RabbitMqTransportOptionsConfiguration>()
    .AddOpenTelemetry("Ordering.Service")
    // .AddSingleton<BsonClassMap<OrderState>, OrderStateClassMap>()
    .AddMassTransit(massTransit =>
    {
        massTransit.UsingRabbitMq((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        });

        massTransit.AddActivities(typeof(ComponentsAssemblyMarker).Assembly);
        massTransit.AddSagaStateMachines(typeof(ComponentsAssemblyMarker).Assembly);
        massTransit.AddConsumers(typeof(ComponentsAssemblyMarker).Assembly);

        massTransit.SetMongoDbSagaRepositoryProvider(conf =>
        {
            var clientSettings = MongoClientSettings.FromConnectionString("mongodb://root:developer@mongo");
            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
            conf.Connection = "mongodb://root:developer@mongo";
            conf.DatabaseName = "orderingdb";
            conf.ClientFactory(p => new MongoClient(clientSettings));
        });
    });

var app = builder.Build();

app.Run();
