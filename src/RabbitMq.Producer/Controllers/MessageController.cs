using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMq.Core;

namespace RabbitMq.Producer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(ILogger<MessageController> logger, IBus bus) : ControllerBase
{
    private static int _messageId;
    
    [HttpPost]
    public IActionResult SendMessage([FromBody] MessagePostRequest request)
    {
        request.MessageId = _messageId++;
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
        
        var message = JsonSerializer.Serialize(request);
        
        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish("", queueName, null, body);
        
        logger.LogInformation($"Message sent: {message}");
        
        return Ok("your message has been sent");
    }

    [HttpPost("MassTransit")]
    public async Task<IActionResult> SendMessageMassTransit([FromBody] MessagePostRequest request)
    {
        var queueName = Environment.GetEnvironmentVariable("MASS_TRANSIT_QUEUE_NAME") ?? string.Empty;
        
        var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
        
        await endpoint.Send(request);
        
        return Ok("your message has been sent");
    }
}