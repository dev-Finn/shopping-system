using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Models;
using Ordering.Service;
using Ordering.Service.CommandHandlers;
using Ordering.Service.Options;
using Ordering.Service.Sagas;
using Serilog;
using Weasel.Postgresql;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(logger);

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.Services.AddMassTransit(massTransit =>
{
    var conf = new MassTransitOptions();
    builder.Configuration.GetSection("MassTransit").Bind(conf);

    massTransit.UsingRabbitMq((context, rbmq) =>
    {
        rbmq.Host(conf.RabbitMq.Host, host =>
        {
            host.Username(conf.RabbitMq.Username);
            host.Password(conf.RabbitMq.Password);
        });
        rbmq.ConfigureEndpoints(context);
        // rbmq.SendTopology.UseCorrelationId<SubmitOrder>(x => NewId.NextGuid());
        // rbmq.SendTopology.UseCorrelationId<CancelOrder>(x => x.OrderId);
    });

    massTransit.AddConsumer<SubmitOrderHandler>();
    massTransit.AddConsumer<CancelOrderHandler>();

    massTransit
        .AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository(builder.Configuration.GetConnectionString("Marten"), store =>
        {
            store.AutoCreateSchemaObjects = AutoCreate.All;
            store.Schema.For<OrderState>()
                .UseOptimisticConcurrency(true);
        });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/orders", SubmitOrder);
app.MapDelete("/orders/{orderId:guid}", CancelOrder);

app.Run();

static async Task SubmitOrder(HttpContext context, [FromBody] SubmitOrderRequest request)
{
    var bus = context.RequestServices.GetService<IBus>();
    await bus.Publish(new SubmitOrder(request.Items));
}

static async Task CancelOrder(HttpContext context, [FromRoute] Guid orderId)
{
    var bus = context.RequestServices.GetService<IBus>();
    await bus.Publish(new CancelOrder(orderId));
}

public sealed record SubmitOrderRequest(OrderItem[] Items);
