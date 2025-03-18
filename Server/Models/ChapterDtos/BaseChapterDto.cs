using System.ComponentModel.DataAnnotations;

namespace Server.Models.ChapterDtos
{
    public abstract class BaseChapterDto
    {

        public int CourseId { get; set; }


        [StringLength(100)]
        public string ChapterName { get; set; }
        public int ChapterOrder { get; set; }
    }
}
