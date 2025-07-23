namespace Repositories.ViewModels.AiModels;

public class OptionWithConflicts
{
    public required AIOption Option { get; set; }
    public List<ConflictInfo> Conflicts { get; set; } = new();
    public bool HasHardConflict => Conflicts.Any(c => c.IsHardConflict);
}