using Server.Models.ContentDtos;

namespace Server.Models.ChapterDtos
{
    public class LearnChapterDto

    {
        public int ChapterId { get; set; }
        public int ChapterOrder { get; set; }
        public string ChapterName { get; set; }
        public virtual IList<ContentDto> Contents { get; set; }
    }
}
