using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Server.Data
{
    public class Content
    {
        [Key]
        public int ContentId { get; set; }

        public int ChapterId { get; set; }

        public virtual Chapter Chapter { get; set; }

        public string ContentType { get; set; }
        public string ContentTitle { get; set; }

        public string ContentUrl { get; set; }
        public int ContentOrder { get; set; }

      
        public virtual ICollection<CommunityFeed> CommunityFeeds { get; set; }
    }
}