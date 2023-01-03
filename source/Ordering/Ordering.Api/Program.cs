using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Ordering.Api;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;
using Ordering.Contracts.Responses;
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
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddOpenTelemetry("Ordering.Api")
    .AddMassTransit(massTransit => massTransit.UsingRabbitMq());

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
    var bus = context.RequestServices.GetRequiredService<IRequestClient<CancelOrder>>();
    var response =  await bus.GetResponse<OrderCancelled, OrderNotFound>(new CancelOrder(orderId));
    if (response.Is(out Response<OrderNotFound>? notfound))
    {
        await Results.NotFound().ExecuteAsync(context);
    }
}

namespace Ordering.Api
{
    public sealed record SubmitOrderRequest(OrderItem[] Items);
}
