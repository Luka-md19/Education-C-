using Server.Models.CourseDtos;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.DetailesDtos
{
    public class DetailsDto : BaseDetailsDto
    {
        
        public int DetailsId { get; set; }
        public virtual CourseDto Course { get; set; }
    }
}
