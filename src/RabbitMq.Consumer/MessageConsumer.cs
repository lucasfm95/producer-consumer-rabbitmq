using System.Text.Json;
using MassTransit;
using RabbitMq.Core;

namespace RabbitMq.Consumer;

public class MessageConsumer : IConsumer<MessagePostRequest>
{
    public Task Consume(ConsumeContext<MessagePostRequest> context)
    {
        Console.WriteLine($"Received message: {JsonSerializer.Serialize(context.Message)}");
        
        return Task.CompletedTask;
    }
}