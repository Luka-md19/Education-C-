using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.CommunityFeedDtos;
using System.Text.Json.Serialization;

namespace Server.Models.ContentDtos
{
    public class ContentDto : BaseContentDto
    {
        public int ContentId { get; set; }
        [JsonIgnore]
        public virtual ChapterDto Chapter { get; set; }
      
    }
}