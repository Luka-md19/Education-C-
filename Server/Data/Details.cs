using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data
{
    public class Detail
    {
        [Key]
        public int DetailsId { get; set; }
        public string Next { get; set; }
        public string Prev { get; set; }



        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
