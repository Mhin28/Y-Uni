using Repositories.Repositories;
using System.Text;

namespace Services.Services.ContextService;

public class ContextService : IContextService
{
    private readonly IAssignmentRepo _assignmentRepo;
    private readonly IEventRepo _eventRepo;
    private readonly ISubjectRepo _subjectRepo;
    private readonly IEventCategoryRepo _eventCategoryRepo;

    public ContextService(IAssignmentRepo assignmentRepo, IEventRepo eventRepo, ISubjectRepo subjectRepo, IEventCategoryRepo eventCategoryRepo)
    {
        _assignmentRepo = assignmentRepo;
        _eventRepo = eventRepo;
        _subjectRepo = subjectRepo;
        _eventCategoryRepo = eventCategoryRepo;
    }

    public async Task<string> GetRelevantContext(string userPrompt, Guid userId)
    {
        var contextBuilder = new StringBuilder();
        var now = DateTime.Now;

        // 1. Get conflicts for the next 7 days to inform the AI
        contextBuilder.AppendLine("Upcoming Schedule (Next 7 Days):");
        
        try
        {
            var upcomingAssignments = await _assignmentRepo.GetAssignmentsDueInRangeAsync(userId, now, now.AddDays(7));
            var upcomingEvents = await _eventRepo.GetEventsInRangeAsync(userId, now, now.AddDays(7));

            if (!upcomingAssignments.Any() && !upcomingEvents.Any())
            {
                contextBuilder.AppendLine("  - Schedule is clear.");
            }
            else
            {
                foreach (var assignment in upcomingAssignments.Take(3))
                {
                    contextBuilder.AppendLine($"  - Assignment: '{assignment.Title}' due {assignment.DueDate:MMM dd}.");
                }
                foreach (var evt in upcomingEvents.Take(3))
                {
                    contextBuilder.AppendLine($"  - Event: '{evt.Title}' on {evt.StartDateTime:MMM dd 'at' HH:mm}.");
                }
            }
        }
        catch (Exception)
        {
            contextBuilder.AppendLine("  - Unable to retrieve schedule information.");
        }

        // 2. Add available subjects for assignments
        try
        {
            var userSubjects = await _subjectRepo.GetSubjectsByUserIdAsync(userId);
            if (userSubjects.Any())
            {
                contextBuilder.AppendLine("\nAvailable Subjects:");
                foreach (var subject in userSubjects.Take(5))
                {
                    contextBuilder.AppendLine($"  - {subject.SubjectName}");
                }
            }
        }
        catch (Exception)
        {
            // Continue if subjects can't be retrieved
        }

        // 3. Add available event categories
        try
        {
            var userCategories = await _eventCategoryRepo.GetCategoriesByUserIdAsync(userId);
            if (userCategories.Any())
            {
                contextBuilder.AppendLine("\nAvailable Event Categories:");
                foreach (var category in userCategories.Take(5))
                {
                    contextBuilder.AppendLine($"  - {category.CategoryName}");
                }
            }
        }
        catch (Exception)
        {
            // Continue if categories can't be retrieved
        }

        // 4. Add keyword-based suggestions
        var prompt = userPrompt.ToLower();
        if (ContainsSubjectKeywords(prompt))
        {
            contextBuilder.AppendLine("\nNote: This appears to be assignment-related. Please suggest an appropriate subject.");
        }
        if (ContainsEventKeywords(prompt))
        {
            contextBuilder.AppendLine("\nNote: This appears to be event-related. Please suggest an appropriate category.");
        }
        
        return contextBuilder.ToString();
    }

    private bool ContainsSubjectKeywords(string prompt)
    {
        var assignmentKeywords = new[] { "assignment", "homework", "project", "study", "exam", "test", "quiz", "math", "science", "english", "history" };
        return assignmentKeywords.Any(keyword => prompt.Contains(keyword));
    }

    private bool ContainsEventKeywords(string prompt)
    {
        var eventKeywords = new[] { "meeting", "event", "appointment", "session", "class", "lecture", "seminar", "workshop" };
        return eventKeywords.Any(keyword => prompt.Contains(keyword));
    }
}