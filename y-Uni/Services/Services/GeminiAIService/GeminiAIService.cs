using Google.GenerativeAI;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using API.DTOs.Ai;

namespace Services.Services.GeminiAIService;

public class GeminiAIService : IGeminiAIService
{
    private readonly GenerativeModel _geminiModel;

    public GeminiAIService(IConfiguration config)
    {
        var apiKey = config["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("GeminiAI:ApiKey not configured");
        var modelName = config["GeminiAI:Model"] ?? "gemini-2.0-flash-exp";

        var generationTool = new Tool
        {
            FunctionDeclarations = new[] { GetFunctionDeclaration() }
        };

        _geminiModel = new GenerativeModel(apiKey,
            model: modelName,
            tools: new[] { generationTool });
    }

    public async Task<List<AIOption>> GenerateOptions(string userMessage, string context)
    {
        var chat = _geminiModel.StartChat();
        var prompt = BuildPrompt(userMessage, context);

        var response = await chat.SendMessageAsync(prompt);

        // The response should contain a function call to our declared function
        var functionCall = response.GetFunctionCalls().FirstOrDefault();
        if (functionCall == null)
        {
            // Fallback or error handling if the AI didn't use the tool
            throw new InvalidOperationException("AI did not return the expected function call.");
        }

        // The arguments are a dictionary. We convert them to JSON and then deserialize.
        var jsonArgs = JsonSerializer.Serialize(functionCall.Arguments);
        var parsedArgs = JsonSerializer.Deserialize<FunctionCallArgs>(jsonArgs, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
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

    private FunctionDeclaration GetFunctionDeclaration()
    {
        // This is the C# representation of the JSON schema
        return new FunctionDeclaration
        {
            Name = "generate_assignment_event_options",
            Description = "Generates 3 options for creating a new assignment or event for the user to choose from.",
            Parameters = new OpenApiSchema
            {
                Type = OpenApiType.Object,
                Properties =
                {
                    { "options", new OpenApiSchema
                        {
                            Type = OpenApiType.Array,
                            Description = "An array of exactly 3 assignment or event options.",
                            Items = new OpenApiSchema
                            {
                                Type = OpenApiType.Object,
                                Properties =
                                {
                                    { "type", new OpenApiSchema { Type = OpenApiType.String, Description = "'assignment' or 'event'" } },
                                    { "title", new OpenApiSchema { Type = OpenApiType.String } },
                                    { "description", new OpenApiSchema { Type = OpenApiType.String } },
                                    { "dueDate", new OpenApiSchema { Type = OpenApiType.String, Format = "date-time" } },
                                    { "priority", new OpenApiSchema { Type = OpenApiType.String, Enum = new List<string> { "Low", "Medium", "High" } } },
                                    { "estimatedTimeMinutes", new OpenApiSchema { Type = OpenApiType.Integer, Description = "Estimated time in minutes" } },
                                    { "reminderType", new OpenApiSchema { Type = OpenApiType.String, Description = "'template' or 'custom'" } },
                                    { "reminderValueMinutes", new OpenApiSchema { Type = OpenApiType.Integer, Description = "Reminder time in minutes before the due date" } },
                                    { "reasoning", new OpenApiSchema { Type = OpenApiType.String, Description = "A brief explanation of why this option is suggested." } }
                                },
                                Required = new List<string> { "type", "title", "dueDate", "priority", "reasoning" }
                            }
                        }
                    }
                },
                Required = new List<string> { "options" }
            }
        };
    }

    // Helper class for deserializing the function call arguments
    private class FunctionCallArgs
    {
        public List<AIOption> Options { get; set; } = new();
    }
}