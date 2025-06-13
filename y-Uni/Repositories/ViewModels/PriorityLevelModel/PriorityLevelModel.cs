using System;

namespace Repositories.ViewModels.PriorityLevelModel
{
    public class PriorityLevelModel
    {
        public byte PriorityId { get; set; }
        
        public string LevelName { get; set; }
        
        public string ColorCode { get; set; }
    }
} 