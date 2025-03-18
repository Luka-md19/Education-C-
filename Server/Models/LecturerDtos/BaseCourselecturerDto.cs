namespace Server.Models.LecturerDtos
{
    public abstract class BaseCourselecturerDto
    {
       
        public string LecturerName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int CourseId { get; set; }
    }
}
