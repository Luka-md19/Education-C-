using System.ComponentModel.DataAnnotations;

namespace Server.Models.CommunityFeedDtos
{
    public class CommunityFeedDto : BaseCommunityFeedDto
    {
        [Key]
        public int PostId { get; set; }
    }
}
