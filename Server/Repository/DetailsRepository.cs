using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Server.Contract;
using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.DetailsDtos;

namespace Server.Repository
{
    public class DetailsRepository : GenericRepository<Detail>, IDetailsRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public DetailsRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }


    }
}
