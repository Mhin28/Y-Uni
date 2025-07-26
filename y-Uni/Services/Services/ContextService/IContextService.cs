namespace Services.Services.ContextService;

public interface IContextService
{
    Task<string> GetRelevantContext(string userPrompt, Guid userId);
}