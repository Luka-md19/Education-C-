using System.ComponentModel.DataAnnotations;

namespace Server.Models.CourseDtos
{
    public abstract class BaseCourseDto
    {
       
        public string CourseName { get; set; } 
        public string Department { get; set; } 
        public int Price { get; set; }
        public string Synopses { get; set; }

        public double Progress { get; set; }
    }
}
