using System;

namespace Repositories.ViewModels.SubjectModel
{
    public class PostSubjectModel
    {
        public string SubjectName { get; set; }
        
        public string Description { get; set; }

        public Guid UserId { get; set; }
    }
} 