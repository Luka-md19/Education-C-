using System.ComponentModel.DataAnnotations;

namespace Server.Models.LecturerDtos
{
    public class GetCourselecturerDto : BaseCourselecturerDto
    {
        [Key]
        public int LecturerId { get; set; }

    }
}
