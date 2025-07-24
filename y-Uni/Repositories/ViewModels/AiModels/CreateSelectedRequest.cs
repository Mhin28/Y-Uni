namespace Repositories.ViewModels.AiModels;

public class CreateSelectedRequest
{
    public required AIOption SelectedOption { get; set; }
    public required string ConversationId { get; set; }
}