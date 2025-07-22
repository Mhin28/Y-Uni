namespace API.DTOs.Ai;

public class AIOption
{
    public string Type { get; set; } = string.Empty; // "assignment" or "event"
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public int EstimatedTimeMinutes { get; set; }
    public string ReminderType { get; set; } = string.Empty; // "template" or "custom"
    public int ReminderValueMinutes { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}