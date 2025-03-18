using Server.Data;
using Server.Contract;
using AutoMapper;
using Server.Repository;

namespace Server.Repositories
{
    public class CourseDepartmentRepository : GenericRepository<CourseDepartment>, ICourseDepartmentRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public CourseDepartmentRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Additional CourseDepartment-specific methods can be added here
    }
}
