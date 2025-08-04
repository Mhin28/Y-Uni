using System;

namespace Repositories.ViewModels.AssignmentModel
{
    public class UpdateAssignmentModel
    {
        public Guid AssignmentId { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public DateTime? CompletedDate { get; set; }
        
        public string Status { get; set; }
        
        public byte? PriorityId { get; set; }
        
        public int? EstimatedTime { get; set; }
        
        public Guid? SubjectId { get; set; }
        
        // Removed navigation properties:
        // - PriorityName (not needed for update)
        // - PriorityColorCode (not needed for update)
        // - SubjectName (not needed for update)
        // - UserId (extracted from JWT token)
    }
}