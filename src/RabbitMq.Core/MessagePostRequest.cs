using System.ComponentModel.DataAnnotations;

namespace RabbitMq.Core;

public class MessagePostRequest
{
    public int MessageId { get; set; }
    [Required]
    public string? Message { get; set; }
}