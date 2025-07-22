namespace API.DTOs.Ai;

public class ChatCreateResponse
{
    public required List<OptionWithConflicts> Options { get; set; }
    public required string ConversationId { get; set; }
}