using Server.Data;
using Server.Models.ContentDtos;
using Server.Models.CourseDtos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Server.Models.ChapterDtos
{
    public class ChapterDto : BaseChapterDto
    {
       
        public int ChapterId { get; set; }
        public virtual IList<ContentDto> Contents { get; set; }
        
    }
}
