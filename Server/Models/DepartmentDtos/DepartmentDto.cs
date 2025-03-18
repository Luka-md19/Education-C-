using Server.Data;
using Server.Models.CourseDepartmentDtos;

namespace Server.Models.DepartmentDtos
{
    public class DepartmentDto : BaseDepartmentDto
    {
        public int DepartmentId { get; set; }
        public virtual ICollection<CourseDepartmentDto> CourseDepartments { get; set; }
    }
}
