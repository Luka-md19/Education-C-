using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data
{
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }
        public string LecturerName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public int CourseId { get; set; }  

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
