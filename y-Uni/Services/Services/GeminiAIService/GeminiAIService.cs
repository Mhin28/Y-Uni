using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Repositories.ViewModels.AiModels;
using System.Text;

namespace Services.Services.GeminiAIService;

public class GeminiAIService : IGeminiAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiAIService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("GeminiAI:ApiKey not configured");
        _model = config["GeminiAI:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<List<AIOption>> GenerateOptions(string userMessage, string context)
    {
        var prompt = BuildPrompt(userMessage, context);
        
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            tools = new[]
            {
                new
                {
                    function_declarations = new[]
                    {
                        GetFunctionDeclaration()
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Gemini API error: {response.StatusCode} - {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        // Extract function call from response
        var functionCall = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.FunctionCall;
        if (functionCall?.Args == null)
        {
            throw new InvalidOperationException("AI did not return the expected function call.");
        }

        var optionsJson = JsonSerializer.Serialize(functionCall.Args);
        var parsedArgs = JsonSerializer.Deserialize<FunctionCallArgs>(optionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return parsedArgs?.Options ?? new List<AIOption>();
    }

    private string BuildPrompt(string userMessage, string context)
    {
        return $@"
        ## SYSTEM INSTRUCTION
        You are an intelligent assistant integrated into the 'Y-Uni' student management app. Your primary function is to help users create assignments or events by generating three structured options based on their request.
        - Analyze the user's request and the provided context carefully.
        - Use the `generate_assignment_event_options` function to format your response. You MUST call this function.
        - Generate exactly 3 options with these variations:
          - Option 1 (Conservative): Lower priority, more estimated time, and an early reminder (e.g., 2 days before).
          - Option 2 (Balanced): Medium priority, standard time, and a standard reminder (e.g., 1 day before).
          - Option 3 (Intensive): High priority, less estimated time, and a close reminder (e.g., a few hours before).
        - Use realistic and helpful titles and descriptions.
        - Set appropriate due dates based on the user's request and the current date.
        - The current date is: {DateTime.Now:dddd, MMMM dd, yyyy}.

        ## CONTEXT
        Here is the relevant information about the user's schedule and preferences:
        {context}

        ## USER REQUEST
        {userMessage}
        ";
    }

    private object GetFunctionDeclaration()
    {
        return new
        {
            name = "generate_assignment_event_options",
            description = "Generates 3 options for creating a new assignment or event for the user to choose from.",
            parameters = new
            {
                type = "object",
                properties = new
                {
                    options = new
                    {
                        type = "array",
                        description = "An array of exactly 3 assignment or event options.",
                        items = new
                        {
                            type = "object",
                            properties = new
                            {
                                type = new { type = "string", description = "'assignment' or 'event'" },
                                title = new { type = "string" },
                                description = new { type = "string" },
                                dueDate = new { type = "string", format = "date-time" },
                                priority = new { type = "string", @enum = new[] { "Low", "Medium", "High" } },
                                estimatedTimeMinutes = new { type = "integer", description = "Estimated time in minutes" },
                                reminderType = new { type = "string", description = "'template' or 'custom'" },
                                reminderValueMinutes = new { type = "integer", description = "Reminder time in minutes before the due date" },
                                reasoning = new { type = "string", description = "A brief explanation of why this option is suggested." },
                                subjectName = new { type = "string", description = "Subject name for assignments (choose from available subjects or suggest new one)" },
                                categoryName = new { type = "string", description = "Category name for events (choose from available categories or suggest new one)" }
                            },
                            required = new[] { "type", "title", "dueDate", "priority", "reasoning" }
                        }
                    }
                },
                required = new[] { "options" }
            }
        };
    }

    // Helper classes for deserializing the Gemini API response
    private class GeminiResponse
    {
        public List<Candidate>? Candidates { get; set; }
    }

    private class Candidate
    {
        public Content? Content { get; set; }
    }

    private class Content
    {
        public List<Part>? Parts { get; set; }
    }

    private class Part
    {
        public FunctionCall? FunctionCall { get; set; }
    }

    private class FunctionCall
    {
        public string? Name { get; set; }
        public object? Args { get; set; }
    }

    private class FunctionCallArgs
    {
        public List<AIOption> Options { get; set; } = new();
    }
}