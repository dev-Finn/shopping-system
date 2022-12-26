using MassTransit;
using Ordering.Service.Options;
using Ordering.Service.Sagas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(massTransit =>
{
    var conf = new MassTransitOptions();
    builder.Configuration.GetSection("MassTransit").Bind(conf);

    massTransit.UsingRabbitMq((context, rbmq) =>
    {
        rbmq.Host(conf.RabbitMq.Host, conf.RabbitMq.VirtualHost, host =>
        {
            host.Username(conf.RabbitMq.Username);
            host.Password(conf.RabbitMq.Password);
        });
        rbmq.ConfigureEndpoints(context);
    });

    massTransit
        .AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository(builder.Configuration.GetConnectionString("Marten"), store =>
        {
            store.Schema.For<OrderState>()
                .UseOptimisticConcurrency(true);
        });
});

var app = builder.Build();

app.Run();
