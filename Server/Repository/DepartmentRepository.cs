using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Server.Contract;
using Server.Data;
using Server.Models.CourseDepartmentDtos;
using Server.Repository;
using AutoMapper.QueryableExtensions;
using Server.API.Exceptions;
using Server.Models.FacultyDtos;

namespace Server.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        public async Task<List<CourseDepartmentResponseDto>> GetCoursesByDepartmentId(int departmentId)
        {
            return await _context.CourseDepartments
                                 .Where(cd => cd.DepartmentId == departmentId)
                                 .Select(cd => new CourseDepartmentResponseDto
                                 {
                                     CourseId = cd.CourseId,
                                     CourseName = cd.Course.CourseName,
                                     DepartmentId = cd.DepartmentId   // Initialize properties here
                                 })
                                 .ToListAsync();
        }



    }
}