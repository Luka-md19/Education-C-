using Server.Models.ChapterDtos;

namespace Server.Models.CourseDtos
{
    public class CourseResponsesDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int Price { get; set; }
        #region//what u will learn in this course 
        public string Synopses { get; set; }
        #endregion
        #region get lecturer details 
        public string LecturerName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double Progress { get; set; }

       
        #endregion

        // Array of Chapter Details
        public List<ChapterDetailDto> Chapters { get; set; }
    }
}
