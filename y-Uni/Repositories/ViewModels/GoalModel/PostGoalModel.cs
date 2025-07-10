using System;

namespace Repositories.ViewModels.GoalModel
{
    public class PostGoalModel
    {
        public string GoalName { get; set; }
        public string Description { get; set; }
        public DateOnly TargetDate { get; set; }
        public string Status { get; set; }
    }
} 