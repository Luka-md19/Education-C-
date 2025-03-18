using Server.Data;
using Server.Models.CourseDtos;
using Server.Models.DepartmentDtos;

namespace Server.Models.CourseDepartmentDtos
{
    public class CourseDepartmentDto: BaseCourseDepartmentDto
    {

        public virtual CourseDto Course { get; set; }
        public virtual DepartmentDto Department { get; set; }
    }
}
