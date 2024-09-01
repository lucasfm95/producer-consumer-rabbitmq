using MassTransit;
using RabbitMq.Consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.AddConsumer<MessageConsumer>();
    busRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
    {
        var massTransitHost = Environment.GetEnvironmentVariable("MASS_TRANSIT_HOST") ?? string.Empty;
        var massTransitPort = ushort.Parse(Environment.GetEnvironmentVariable("MASS_TRANSIT_PORT") ?? "0");
        var massTransitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty;
        var massTransitPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? string.Empty;
        var massTransitQueueName = Environment.GetEnvironmentVariable("MASS_TRANSIT_QUEUE_NAME") ?? string.Empty;

        cfg.Host(massTransitHost, massTransitPort, "/", hostConfigurator =>
        {
            hostConfigurator.Username(massTransitUser);
            hostConfigurator.Password(massTransitPassword);
        });

        cfg.ReceiveEndpoint(massTransitQueueName, endpointConfigurator => { endpointConfigurator.Consumer<MessageConsumer>(context); });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();