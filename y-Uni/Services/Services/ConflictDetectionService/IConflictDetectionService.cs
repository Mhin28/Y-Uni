using API.DTOs.Ai;

namespace Services.Services.ConflictDetectionService;

public interface IConflictDetectionService
{
    Task<List<OptionWithConflicts>> CheckConflicts(List<AIOption> options, Guid userId);
}