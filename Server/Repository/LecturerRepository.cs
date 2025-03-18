using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Server.Contract;
using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.CourseDepartmentDtos;
using Server.Models.LecturerDtos;

namespace Server.Repository
{
    public class LecturerRepository : GenericRepository<Lecturer>, ILecturerRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public LecturerRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<List<CourselecturerDto>> GetlecturerByIdAsync(int lecturerId)
        {
            return await _context.Lecturers // Assuming Lecturers is your DbSet for lecturer entities
                .Where(l => l.LecturerId == lecturerId) // Assuming Id is the primary key of the Lecturer entity
                .Select(l => new CourselecturerDto
                {
                    LecturerId = l.LecturerId,
                    LecturerName = l.LecturerName, // Assuming Name is a property of the Lecturer entity
                    Description = l.Description, // Assuming Description is a property of the Lecturer entity
                    Image = l.Image, // Assuming ImageUrl is a property of the Lecturer entity
                    CourseId = l.CourseId // Assuming CourseId is a foreign key in Lecturer entity pointing to Course
                })
                .ToListAsync();
        }
    }
}