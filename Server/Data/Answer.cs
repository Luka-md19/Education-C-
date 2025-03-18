using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        [Required]
        public string Content { get; set; }  

        public DateTime PostedDate { get; set; }
        public int Upvotes { get; set; }

       
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual CommunityFeed CommunityFeed { get; set; }
    }
}
