using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (Environment.GetEnvironmentVariable("USE_MASS_TRANSIT") == "false")
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var factory = new ConnectionFactory
                {
                    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? string.Empty,
                    UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty,
                    Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? string.Empty
                };
            
                var queueName = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE_NAME") ?? string.Empty;
            
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
            
                channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            
                var consumer = new EventingBasicConsumer(channel);
            
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                
                    _logger.LogInformation($"Received message: {message}");
                    Console.WriteLine($"Received message: {message}");
                };
            
                channel.BasicConsume(queueName, true, consumer);
            }
        }
        
        return Task.CompletedTask;
    }
}