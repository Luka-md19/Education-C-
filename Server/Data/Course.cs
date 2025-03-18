using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
      
        public string CourseName { get; set; }

        public string Synopses { get; set; }
        
        public string Department { get; set; }
       
        
        public int Price { get; set; }


        
        public virtual ICollection<CommunityFeed> CommunityPosts { get; set; }
       
        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual ICollection<CourseDepartment> CourseDepartments { get; set; }
        public virtual ICollection<Detail> Details { get; set; }
        public virtual ICollection<Lecturer> Lecturers{ get; set; }
        public virtual ICollection<UserCourse> UserCourses { get; set; }
    }
}
