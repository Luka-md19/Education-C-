namespace Server.Models.DetailesDtos
{
    public abstract class BaseDetailsDto
    {
        public int CourseId { get; set; }
        public string Next { get; set; }
        public string Prev { get; set; }
    }
}
