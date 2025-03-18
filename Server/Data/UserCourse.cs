using Server.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class UserCourse
{
    [Key]
    public int UserCourseId { get; set; }

    public double Progress { get; set; }

    [ForeignKey("ApiUser")]
    public string UserId { get; set; }
    public virtual ApiUser User { get; set; }

    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public virtual Course Course { get; set; }
}
