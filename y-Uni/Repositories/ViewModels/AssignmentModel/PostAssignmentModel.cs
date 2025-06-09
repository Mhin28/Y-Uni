using System;

namespace Repositories.ViewModels.AssignmentModel
{
    public class PostAssignmentModel
    {
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public byte? PriorityId { get; set; }
        
        public int? EstimatedTime { get; set; }
        
        public Guid? SubjectId { get; set; }
        
        public Guid? UserId { get; set; }
    }
} 