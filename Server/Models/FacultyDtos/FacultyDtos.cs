using Server.Data;

namespace Server.Models.FacultyDtos
{
    public class FacultyDtos : BaseFacultyDtos
    {
        public int FacultyId { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
