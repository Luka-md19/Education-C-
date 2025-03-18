using System.ComponentModel.DataAnnotations;

namespace Server.Models.CourseDtos
{
    public class GetCourseDto : BaseCourseDto
    {
        [Key]

        public int CourseId { get; set; }
    }
}
