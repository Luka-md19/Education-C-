using System.ComponentModel.DataAnnotations;

namespace Server.Models.ChapterDtos
{
    public class GetChapterDto : BaseChapterDto
    {
        [Key]
        public int ChapterId { get; set; }
    }
}
