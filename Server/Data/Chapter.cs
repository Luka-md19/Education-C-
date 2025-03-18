using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Server.Data
{
    public class Chapter
    {
        [Key]
        public int ChapterId { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
       

        [StringLength(100)]
        public string ChapterName { get; set; }
        public int ChapterOrder { get; set; }


        public virtual Course Course { get; set; }
        [JsonIgnore]
        public virtual IList<Content> Contents { get; set; }
    }
}