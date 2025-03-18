using Server.Models.CourseDepartmentDtos;
using Server.Models.DepartmentDtos;

namespace Server.Models.FacultyDtos
{
    public class FacultyDetailsDto
    {

        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public List<DepartmentDetails> Departments { get; set; }
    }
}
