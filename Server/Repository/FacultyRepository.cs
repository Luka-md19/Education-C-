using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Server.Contract;
using Server.Data;
using Server.Models.FacultyDtos;

namespace Server.Repository
{
    public class FacultyRepository : GenericRepository<Faculty>, IFacultyRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public FacultyRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<FacultyDetailsDto> FacultyDetailsIdAsync(int FacultyId)
        {
            var facultyDetails = await _context.Faculties
                       .Include(f => f.Departments)
                       .Where(f => f.FacultyId == FacultyId)
                       .ProjectTo<FacultyDetailsDto>(_mapper.ConfigurationProvider)
                       .FirstOrDefaultAsync();

            return facultyDetails;
        }
    }
}
