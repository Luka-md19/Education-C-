using Server.Data;
using System.Text.Json.Serialization;

namespace Server.Models.ContentDtos
{
    public abstract class BaseContentDto

    {
        
        public int ChapterId { get; set; }
        public string ContentType { get; set; }
        public string ContentTitle { get; set; }
        public string ContentUrl { get; set; }
        public int ContentOrder { get; set; }
    }
}
