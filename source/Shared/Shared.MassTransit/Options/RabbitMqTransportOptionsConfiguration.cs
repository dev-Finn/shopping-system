using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Shared.MassTransit.Options;

public sealed class RabbitMqTransportOptionsConfiguration : IConfigureOptions<RabbitMqTransportOptions>
{
    private readonly IConfiguration _configuration;


    public RabbitMqTransportOptionsConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(RabbitMqTransportOptions options)
    {
        _configuration.GetSection("RabbitMq").Bind(options);
    }
}
