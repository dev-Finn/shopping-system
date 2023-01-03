using MassTransit;
using Payment.Components;
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
    .AddOpenTelemetryForService("Payment.Service")
    .AddMassTransit(massTransit =>
    {
        massTransit.UsingRabbitMq((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        });

        massTransit.AddActivities(typeof(ComponentsAssemblyMarker).Assembly);
        massTransit.AddSagaStateMachines(typeof(ComponentsAssemblyMarker).Assembly);
        massTransit.AddConsumers(typeof(ComponentsAssemblyMarker).Assembly);
    });

var app = builder.Build();

app.Run();
