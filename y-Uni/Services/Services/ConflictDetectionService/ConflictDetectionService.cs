using Repositories.ViewModels.AiModels;
using Repositories.Repositories;

namespace Services.Services.ConflictDetectionService;

public class ConflictDetectionService : IConflictDetectionService
{
    private readonly IEventRepo _eventRepo;
    private readonly IAssignmentRepo _assignmentRepo;

    public ConflictDetectionService(IEventRepo eventRepo, IAssignmentRepo assignmentRepo)
    {
        _eventRepo = eventRepo;
        _assignmentRepo = assignmentRepo;
    }

    public async Task<List<OptionWithConflicts>> CheckConflicts(List<AIOption> options, Guid userId)
    {
        var results = new List<OptionWithConflicts>();

        foreach (var option in options)
        {
            var optionWithConflict = new OptionWithConflicts { Option = option };
            
            try
            {
                if (option.Type.Equals("event", StringComparison.OrdinalIgnoreCase))
                {
                    // Hard conflict: Check for overlapping events
                    var endDateTime = option.DueDate.AddMinutes(option.EstimatedTimeMinutes);
                    var overlappingEvents = await _eventRepo.GetEventsInRangeAsync(userId, option.DueDate, endDateTime);
                    if (overlappingEvents.Any())
                    {
                        optionWithConflict.Conflicts.Add(new ConflictInfo 
                        { 
                            Message = $"Time conflict with: {overlappingEvents.First().Title}", 
                            IsHardConflict = true 
                        });
                    }
                }
                else if (option.Type.Equals("assignment", StringComparison.OrdinalIgnoreCase))
                {
                    // Soft conflict: Check for more than 2 other assignments on the same day
                    var startOfDay = option.DueDate.Date;
                    var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                    var sameDayAssignments = await _assignmentRepo.GetAssignmentsDueInRangeAsync(userId, startOfDay, endOfDay);
                    if (sameDayAssignments.Count() >= 2)
                    {
                         optionWithConflict.Conflicts.Add(new ConflictInfo 
                        { 
                            Message = $"Busy day: {sameDayAssignments.Count()} other items due.", 
                            IsHardConflict = false 
                        });
                    }
                }
            }
            catch (Exception)
            {
                // If conflict detection fails, just continue without conflicts
                // This ensures the system remains functional even if conflict detection has issues
            }
            
            results.Add(optionWithConflict);
        }

        return results;
    }
}