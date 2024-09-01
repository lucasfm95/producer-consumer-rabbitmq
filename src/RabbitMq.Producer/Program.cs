using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
    {
        var massTransitHost = Environment.GetEnvironmentVariable("MASS_TRANSIT_HOST") ?? string.Empty;
        var massTransitPort = ushort.Parse(Environment.GetEnvironmentVariable("MASS_TRANSIT_PORT") ?? "0");
        var massTransitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty;
        var massTransitPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? string.Empty;
        
        cfg.Host(massTransitHost, massTransitPort, "/", hostConfigurator =>
        {
            hostConfigurator.Username(massTransitUser);
            hostConfigurator.Password(massTransitPassword);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();