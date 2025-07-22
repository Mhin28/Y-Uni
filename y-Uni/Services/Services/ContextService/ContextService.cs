using Repositories.Repositories;
using System.Text;

namespace Services.Services.ContextService;

public class ContextService : IContextService
{
    private readonly IAssignmentRepo _assignmentRepo;
    private readonly IEventRepo _eventRepo;

    public ContextService(IAssignmentRepo assignmentRepo, IEventRepo eventRepo)
    {
        _assignmentRepo = assignmentRepo;
        _eventRepo = eventRepo;
    }

    public async Task<string> GetRelevantContext(string userPrompt, Guid userId)
    {
        var contextBuilder = new StringBuilder();
        var now = DateTime.UtcNow;

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

        // Add more context logic here (user preferences, subjects, etc.) as needed.
        
        return contextBuilder.ToString();
    }
}