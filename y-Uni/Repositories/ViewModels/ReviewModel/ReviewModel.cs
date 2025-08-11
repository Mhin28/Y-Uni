using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.ReviewModel
{
    public class ReviewModel
    {
        public Guid ReviewId { get; set; } 
        public Guid UserId { get; set; }   
        public Guid ProductId { get; set; } 
        public int Rating { get; set; }   
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostReviewModel
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

}
