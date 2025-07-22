namespace API.DTOs.Ai;

public class ChatCreateRequest
{
    public required string Message { get; set; }
    public string? ConversationId { get; set; }
}