namespace Ordering.Service.Options;

public sealed class MassTransitOptions
{
    public RabbitMqConfig RabbitMq { get; set; }

    public sealed class RabbitMqConfig
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
