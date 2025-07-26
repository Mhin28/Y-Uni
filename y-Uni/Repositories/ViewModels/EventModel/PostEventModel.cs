using System;

namespace Repositories.ViewModels.EventModel
{
    public class PostEventModel
    {
        public string Title { get; set; }
        
        public DateTime StartDateTime { get; set; }
        
        public DateTime EndDateTime { get; set; }
        
        public string Description { get; set; }
        
        public string RecurrencePattern { get; set; }
        
        public DateOnly? RecurrenceEndDate { get; set; }
        
        public Guid? EvCategoryId { get; set; }
        
        // UserId removed - will be extracted from JWT token
    }
} 