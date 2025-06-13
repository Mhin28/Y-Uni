using System;

namespace Repositories.ViewModels.EventCategoryModel
{
    public class EventCategoryModel
    {
        public Guid EvCategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        public string Description { get; set; }
    }
} 