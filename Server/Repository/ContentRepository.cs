using Server.Data;
using Server.Contract;
using AutoMapper;
using Server.Repository;
using Server.Models.ContentDtos;
using Server.Models.CourseDepartmentDtos;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Server.Repositories
{
    public class ContentRepository : GenericRepository<Content>, IContentRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public ContentRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
       

    }
}
