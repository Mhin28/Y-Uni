using System;

namespace Repositories.ViewModels.EventModel
{
    public class EventModel
    {
        public Guid EventId { get; set; }
        
        public string Title { get; set; }
        
        public DateTime StartDateTime { get; set; }
        
        public DateTime EndDateTime { get; set; }
        
        public string Description { get; set; }
        
        public string RecurrencePattern { get; set; }
        
        public DateOnly? RecurrenceEndDate { get; set; }
        
        public Guid? EvCategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        public Guid? UserId { get; set; }
    }
} 