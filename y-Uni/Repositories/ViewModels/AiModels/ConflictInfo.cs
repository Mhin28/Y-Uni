namespace Repositories.ViewModels.AiModels;

public class ConflictInfo
{
    public required string Message { get; set; }
    public bool IsHardConflict { get; set; }
}