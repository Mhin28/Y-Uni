using API.DTOs.Ai;

namespace Services.Services.GeminiAIService;

public interface IGeminiAIService
{
    Task<List<AIOption>> GenerateOptions(string userMessage, string context);
}