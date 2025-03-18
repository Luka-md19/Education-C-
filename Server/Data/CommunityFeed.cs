using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data
{
    public class CommunityFeed
    {
        [Key]
        public int PostId { get; set; }

        public string Question { get; set; }

        public DateTime PostedDate { get; set; }
        public int Upvotes { get; set; }

        public int ContentId { get; set; }

        [ForeignKey("ContentId")]
        public virtual Content Content { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}
