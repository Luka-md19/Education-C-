using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.CommunityFeedDtos;
using Server.Models.CourseDepartmentDtos;
using Server.Models.DetailesDtos;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.CourseDtos
{
    public class CourseDto : BaseCourseDto
    {
       
        public int CourseId { get; set; }
        public virtual ICollection<CommunityFeedDto> CommunityPosts { get; set; }
        public virtual ICollection<ChapterDto> Chapters { get; set; }
        public virtual ICollection<CourseDepartmentDto> CourseDepartments { get; set; }
        public virtual ICollection<DetailsDto> Details { get; set; }
        public virtual ICollection<UserCourse> UserCourses { get; set; }
    }
}
