using System.ComponentModel.DataAnnotations;

namespace RabbitMq.Domain.Requests;

public class MessagePostRequest
{
    public int MessageId { get; set; }
    [Required]
    public string? Message { get; set; }
}