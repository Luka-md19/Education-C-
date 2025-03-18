using Server.Data;
using Server.Models.CourseDepartmentDtos;
using Server.Models.CourseDtos;
using Server.Models.FacultyDtos;
using System.Collections.Generic;

namespace Server.Contract
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<List<CourseDepartmentResponseDto>> GetCoursesByDepartmentId(int departmentId);
       
    }

}

