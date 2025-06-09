using System;
using System.Collections.Generic;

namespace Repositories.ViewModels.AssignmentModel
{
    public class AssignmentModel
    {
        public Guid AssignmentId { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public DateTime? CompletedDate { get; set; }
        
        public string Status { get; set; }
        
        public byte? PriorityId { get; set; }
        
        public string PriorityName { get; set; }
        
        public string PriorityColorCode { get; set; }
        
        public int? EstimatedTime { get; set; }
        
        public Guid? SubjectId { get; set; }
        
        public string SubjectName { get; set; }
        
        public Guid? UserId { get; set; }
    }
} 