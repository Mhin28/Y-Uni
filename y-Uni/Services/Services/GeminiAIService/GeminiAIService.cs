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
        // Step 1: Validate and preprocess the prompt
        var validationResult = ValidatePrompt(userMessage);
        if (!validationResult.IsValid)
        {
            return GenerateHelpfulResponse(validationResult.Issue, userMessage);
        }

        // Step 2: Enhanced prompt with better instructions
        var enhancedPrompt = BuildEnhancedPrompt(userMessage, context);
        
        // Step 3: Call Gemini with improved error handling
        try
        {
            var response = await CallGeminiWithRetry(enhancedPrompt);
            var options = ParseGeminiResponse(response);
            
            // Step 4: Post-process options to fix common issues
            return PostProcessOptions(options, userMessage);
        }
        catch (Exception ex)
        {
            return GenerateFallbackOptions(userMessage, context);
        }
    }

    private PromptValidationResult ValidatePrompt(string userMessage)
    {
        var message = userMessage?.Trim().ToLower() ?? "";
        
        // Check for empty or too short
        if (string.IsNullOrEmpty(message) || message.Length < 3)
        {
            return new PromptValidationResult 
            { 
                IsValid = false, 
                Issue = PromptIssue.TooShort 
            };
        }
        
        // Check for nonsense (only very obvious nonsense)
        var meaningfulWords = new[] { 
            "create", "make", "add", "schedule", "plan", "assignment", "homework", 
            "project", "meeting", "event", "study", "exam", "test", "class", 
            "lecture", "presentation", "deadline", "due", "tomorrow", "today",
            "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday",
            "morning", "afternoon", "evening", "night", "math", "science", "english",
            "chemistry", "physics", "biology", "history", "computer", "programming",
            "work", "task", "session", "review", "prepare", "finish", "complete", "submit"
        };
        
        // Only reject if it's really obvious nonsense (no meaningful words AND very short)
        var hasAnyMeaningfulWord = meaningfulWords.Any(word => message.Contains(word));
        if (!hasAnyMeaningfulWord && message.Length < 10)
        {
            return new PromptValidationResult 
            { 
                IsValid = false, 
                Issue = PromptIssue.NoMeaningfulContent 
            };
        }
        
        // Check for greeting only
        var greetings = new[] { "hello", "hi", "hey", "good morning", "good afternoon" };
        if (greetings.Any(greeting => message == greeting || message.StartsWith(greeting + " ")))
        {
            return new PromptValidationResult 
            { 
                IsValid = false, 
                Issue = PromptIssue.GreetingOnly 
            };
        }
        
        // Check for explicit help requests only
        var helpKeywords = new[] { "help me", "how do i", "what can you", "what should i" };
        if (helpKeywords.Any(keyword => message.Contains(keyword)))
        {
            return new PromptValidationResult 
            { 
                IsValid = false, 
                Issue = PromptIssue.HelpRequest 
            };
        }
        
        return new PromptValidationResult { IsValid = true };
    }

    private async Task<string> CallGeminiWithRetry(string prompt)
    {
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

        return await response.Content.ReadAsStringAsync();
    }

    private List<AIOption> ParseGeminiResponse(string responseContent)
    {
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

    private string BuildEnhancedPrompt(string userMessage, string context)
    {
        var currentTime = DateTime.Now;
        var timeContext = GenerateTimeContext(currentTime);
        
        return $@"
## SYSTEM INSTRUCTION
You are an intelligent assistant for the Y-Uni student management app. Generate exactly 3 options for assignments or events.

## IMPORTANT TIME HANDLING RULES:
1. If no specific time is mentioned for assignments, set due date to 11:59 PM of the suggested day
2. If no specific time is mentioned for events, suggest appropriate times based on context:
   - Morning: 9:00 AM - 11:00 AM
   - Afternoon: 2:00 PM - 4:00 PM  
   - Evening: 6:00 PM - 8:00 PM
3. If no date is mentioned, suggest reasonable timeframes:
   - Assignments: 1-7 days from now
   - Events: 1-3 days from now
4. Always use realistic and helpful times, never midnight (00:00:00) unless specifically requested

## TIME CONTEXT
Current date and time: {currentTime:dddd, MMMM dd, yyyy 'at' HH:mm}
{timeContext}

## CONTEXT
{context}

## USER REQUEST
{userMessage}

## RESPONSE REQUIREMENTS
- Generate exactly 3 options
- Use the generate_assignment_event_options function
- Provide realistic due dates and times
- Include helpful reasoning for each option
- Make reasonable assumptions for missing details (don't ask for more specifics)
- Focus on creating useful options rather than requesting clarification
";
    }

    private string GenerateTimeContext(DateTime currentTime)
    {
        var timeOfDay = currentTime.Hour switch
        {
            >= 6 and < 12 => "morning",
            >= 12 and < 17 => "afternoon", 
            >= 17 and < 21 => "evening",
            _ => "night"
        };
        
        var dayOfWeek = currentTime.DayOfWeek;
        var isWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        
        return $@"
Time of day: {timeOfDay}
Day of week: {dayOfWeek} ({(isWeekend ? "weekend" : "weekday")})

Suggested default times:
- Assignment due times: End of day (11:59 PM)
- Morning events: 9:00 AM - 11:00 AM
- Afternoon events: 2:00 PM - 4:00 PM
- Evening events: 6:00 PM - 8:00 PM
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

    // Helper classes for prompt validation and response generation
    private class PromptValidationResult
    {
        public bool IsValid { get; set; }
        public PromptIssue Issue { get; set; }
    }

    private enum PromptIssue
    {
        TooShort,
        NoMeaningfulContent,
        GreetingOnly,
        HelpRequest
    }

    private List<AIOption> GenerateHelpfulResponse(PromptIssue issue, string originalMessage)
    {
        return issue switch
        {
            PromptIssue.TooShort => GenerateExampleOptions("Please provide more details. Try something like:"),
            PromptIssue.NoMeaningfulContent => GenerateExampleOptions("I didn't understand that. Here are some examples:"),
            PromptIssue.GreetingOnly => GenerateGreetingResponse(),
            PromptIssue.HelpRequest => GenerateHelpOptions(),
            _ => GenerateGenericHelpOptions()
        };
    }

    private List<AIOption> GenerateExampleOptions(string message)
    {
        return new List<AIOption>
        {
            new AIOption
            {
                Type = "suggestion",
                Title = "Example: Assignment Creation",
                Description = "Try: 'Create assignment due tomorrow' or 'I need homework by Friday'",
                DueDate = DateTime.Now.AddDays(1).Date.AddHours(23).AddMinutes(59),
                Priority = "Medium",
                EstimatedTimeMinutes = 0,
                ReminderType = "none",
                ReminderValueMinutes = 0,
                Reasoning = message,
                SubjectName = null,
                CategoryName = null
            },
            new AIOption
            {
                Type = "suggestion", 
                Title = "Example: Event Creation",
                Description = "Try: 'Schedule meeting tomorrow afternoon' or 'Book study session for Monday'",
                DueDate = DateTime.Now.AddDays(1).Date.AddHours(14),
                Priority = "Medium",
                EstimatedTimeMinutes = 0,
                ReminderType = "none",
                ReminderValueMinutes = 0,
                Reasoning = "Events help you schedule time-based activities",
                SubjectName = null,
                CategoryName = null
            },
            new AIOption
            {
                Type = "suggestion",
                Title = "Example: Simple Requests", 
                Description = "Try: 'Add lab to my calendar' or 'Create project due next week'",
                DueDate = DateTime.Now.AddDays(7).Date.AddHours(23).AddMinutes(59),
                Priority = "Medium",
                EstimatedTimeMinutes = 0,
                ReminderType = "none",
                ReminderValueMinutes = 0,
                Reasoning = "Simple descriptions work well - I'll fill in the details",
                SubjectName = null,
                CategoryName = null
            }
        };
    }

    private List<AIOption> GenerateGreetingResponse()
    {
        return new List<AIOption>
        {
            new AIOption
            {
                Type = "greeting",
                Title = "Hello! I'm your AI assistant",
                Description = "I can help you create assignments and schedule events using natural language",
                DueDate = DateTime.Now.AddHours(1),
                Priority = "Medium",
                EstimatedTimeMinutes = 0,
                ReminderType = "none", 
                ReminderValueMinutes = 0,
                Reasoning = "Tell me what you'd like to create or schedule, and I'll generate options for you!",
                SubjectName = null,
                CategoryName = null
            }
        };
    }

    private List<AIOption> GenerateHelpOptions()
    {
        return new List<AIOption>
        {
            new AIOption
            {
                Type = "help",
                Title = "How to Create Assignments",
                Description = "Tell me about assignments you need to complete, including subject and due date",
                DueDate = DateTime.Now.AddDays(1).Date.AddHours(23).AddMinutes(59),
                Priority = "Medium",
                EstimatedTimeMinutes = 0,
                ReminderType = "none",
                ReminderValueMinutes = 0,
                Reasoning = "I can help you create assignments with proper subjects and deadlines",
                SubjectName = null,
                CategoryName = null
            },
            new AIOption
            {
                Type = "help",
                Title = "How to Schedule Events",
                Description = "Describe events you want to schedule, including time and purpose",
                DueDate = DateTime.Now.AddDays(1).Date.AddHours(14),
                Priority = "Medium",
                EstimatedTimeMinutes = 0,
                ReminderType = "none",
                ReminderValueMinutes = 0,
                Reasoning = "I can help you schedule events with appropriate timing and categories",
                SubjectName = null,
                CategoryName = null
            }
        };
    }

    private List<AIOption> GenerateGenericHelpOptions()
    {
        return GenerateExampleOptions("I'm here to help! Here are some examples:");
    }

    private List<AIOption> PostProcessOptions(List<AIOption> options, string userMessage)
    {
        foreach (var option in options)
        {
            // Fix midnight times for assignments
            if (option.Type == "assignment" && option.DueDate.TimeOfDay == TimeSpan.Zero)
            {
                option.DueDate = option.DueDate.Date.AddHours(23).AddMinutes(59);
            }
            
            // Fix midnight times for events - suggest appropriate times
            if (option.Type == "event" && option.DueDate.TimeOfDay == TimeSpan.Zero)
            {
                option.DueDate = SuggestEventTime(option.DueDate.Date, userMessage);
            }
            
            // Ensure minimum estimated time
            if (option.EstimatedTimeMinutes <= 0 && (option.Type == "assignment" || option.Type == "event"))
            {
                option.EstimatedTimeMinutes = option.Type == "assignment" ? 60 : 30;
            }
            
            // Fix empty titles
            if (string.IsNullOrEmpty(option.Title) && (option.Type == "assignment" || option.Type == "event"))
            {
                option.Title = GenerateDefaultTitle(option.Type, option.SubjectName, option.CategoryName);
            }
        }
        
        return options;
    }

    private DateTime SuggestEventTime(DateTime date, string userMessage)
    {
        var message = userMessage.ToLower();
        
        // Check for specific time mentions
        if (message.Contains("morning")) return date.AddHours(9);
        if (message.Contains("afternoon")) return date.AddHours(14);
        if (message.Contains("evening")) return date.AddHours(18);
        if (message.Contains("night")) return date.AddHours(19);
        
        // Check for activity-based suggestions
        if (message.Contains("meeting") || message.Contains("discussion")) return date.AddHours(14);
        if (message.Contains("study") || message.Contains("review")) return date.AddHours(15);
        if (message.Contains("class") || message.Contains("lecture")) return date.AddHours(10);
        if (message.Contains("lab") || message.Contains("workshop")) return date.AddHours(13);
        
        // Default to afternoon
        return date.AddHours(14);
    }

    private string GenerateDefaultTitle(string type, string? subjectName, string? categoryName)
    {
        if (type == "assignment")
        {
            return subjectName != null ? $"{subjectName} Assignment" : "General Assignment";
        }
        
        if (type == "event")
        {
            return categoryName != null ? $"{categoryName} Event" : "Scheduled Event";
        }
        
        return "New Item";
    }

    private List<AIOption> GenerateFallbackOptions(string userMessage, string context)
    {
        var message = userMessage.ToLower();
        var isAssignmentRelated = ContainsAssignmentKeywords(message);
        var isEventRelated = ContainsEventKeywords(message);
        
        if (!isAssignmentRelated && !isEventRelated)
        {
            // Generate both types as suggestions
            return new List<AIOption>
            {
                GenerateFallbackAssignment(userMessage),
                GenerateFallbackEvent(userMessage),
                GenerateHelpOption()
            };
        }
        
        if (isAssignmentRelated)
        {
            return GenerateFallbackAssignments(userMessage);
        }
        
        return GenerateFallbackEvents(userMessage);
    }

    private bool ContainsAssignmentKeywords(string message)
    {
        var assignmentKeywords = new[] { "assignment", "homework", "project", "study", "exam", "test", "quiz" };
        return assignmentKeywords.Any(keyword => message.Contains(keyword));
    }

    private bool ContainsEventKeywords(string message)
    {
        var eventKeywords = new[] { "meeting", "event", "appointment", "session", "class", "lecture", "seminar", "workshop", "schedule" };
        return eventKeywords.Any(keyword => message.Contains(keyword));
    }

    private AIOption GenerateFallbackAssignment(string userMessage)
    {
        return new AIOption
        {
            Type = "assignment",
            Title = "General Assignment",
            Description = $"Based on your request: '{userMessage}' - Please provide more specific details",
            DueDate = DateTime.Now.AddDays(3).Date.AddHours(23).AddMinutes(59),
            Priority = "Medium",
            EstimatedTimeMinutes = 120,
            ReminderType = "template",
            ReminderValueMinutes = 1440,
            Reasoning = "Generated a general assignment since specific details weren't clear. You can modify this after creation.",
            SubjectName = null,
            CategoryName = null
        };
    }

    private AIOption GenerateFallbackEvent(string userMessage)
    {
        return new AIOption
        {
            Type = "event", 
            Title = "Scheduled Activity",
            Description = $"Based on your request: '{userMessage}' - Please provide more specific details",
            DueDate = DateTime.Now.AddDays(1).Date.AddHours(14),
            Priority = "Medium",
            EstimatedTimeMinutes = 60,
            ReminderType = "template",
            ReminderValueMinutes = 30,
            Reasoning = "Generated a general event since specific details weren't clear. You can modify this after creation.",
            SubjectName = null,
            CategoryName = null
        };
    }

    private AIOption GenerateHelpOption()
    {
        return new AIOption
        {
            Type = "help",
            Title = "Need More Specific Details",
            Description = "Try being more specific about what you want to create",
            DueDate = DateTime.Now.AddHours(1),
            Priority = "Medium",
            EstimatedTimeMinutes = 0,
            ReminderType = "none",
            ReminderValueMinutes = 0,
            Reasoning = "I work best when you tell me exactly what you want to create or schedule",
            SubjectName = null,
            CategoryName = null
        };
    }

    private List<AIOption> GenerateFallbackAssignments(string userMessage)
    {
        return new List<AIOption>
        {
            GenerateFallbackAssignment(userMessage),
            new AIOption
            {
                Type = "assignment",
                Title = "Study Task",
                Description = "General study task based on your request",
                DueDate = DateTime.Now.AddDays(2).Date.AddHours(23).AddMinutes(59),
                Priority = "Low",
                EstimatedTimeMinutes = 90,
                ReminderType = "template",
                ReminderValueMinutes = 2880,
                Reasoning = "Conservative approach for unclear assignment request",
                SubjectName = null,
                CategoryName = null
            },
            new AIOption
            {
                Type = "assignment",
                Title = "Quick Task",
                Description = "Urgent task based on your request",
                DueDate = DateTime.Now.AddDays(1).Date.AddHours(23).AddMinutes(59),
                Priority = "High",
                EstimatedTimeMinutes = 60,
                ReminderType = "custom",
                ReminderValueMinutes = 360,
                Reasoning = "Intensive approach for quick completion",
                SubjectName = null,
                CategoryName = null
            }
        };
    }

    private List<AIOption> GenerateFallbackEvents(string userMessage)
    {
        return new List<AIOption>
        {
            GenerateFallbackEvent(userMessage),
            new AIOption
            {
                Type = "event",
                Title = "Extended Session",
                Description = "Longer session based on your request",
                DueDate = DateTime.Now.AddDays(1).Date.AddHours(15),
                Priority = "Medium",
                EstimatedTimeMinutes = 90,
                ReminderType = "template",
                ReminderValueMinutes = 60,
                Reasoning = "Extended time for thorough completion",
                SubjectName = null,
                CategoryName = null
            },
            new AIOption
            {
                Type = "event",
                Title = "Quick Session",
                Description = "Brief session based on your request",
                DueDate = DateTime.Now.AddDays(1).Date.AddHours(16),
                Priority = "High",
                EstimatedTimeMinutes = 30,
                ReminderType = "custom",
                ReminderValueMinutes = 15,
                Reasoning = "Quick session for immediate needs",
                SubjectName = null,
                CategoryName = null
            }
        };
    }
}