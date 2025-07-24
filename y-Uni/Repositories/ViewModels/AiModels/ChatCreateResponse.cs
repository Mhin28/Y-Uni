namespace Repositories.ViewModels.AiModels;

public class ChatCreateResponse
{
    public required List<OptionWithConflicts> Options { get; set; }
    public required string ConversationId { get; set; }
}