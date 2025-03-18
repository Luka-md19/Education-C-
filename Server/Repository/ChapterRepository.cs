using Server.Data;
using Server.Models.ChapterDtos;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Server.Contract;
using Server.Repository;
using Server.API.Exceptions;
using Server.Models.CourseDtos;

namespace Server.Repositories
{
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public ChapterRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }


        public async Task<List<LearnChapterDto>> GetLearnChaptersByCourseIdAsync(int courseId)
        {

            var chapterDetails = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .SelectMany(c => c.Chapters)
                .ProjectTo<LearnChapterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return chapterDetails;
        }

        public async Task<List<LearnChapterDto>> GetLearnChaptersByCourseNameAsync(string courseName)
        {

            var chapterDetails = await _context.Courses
                .Where(c => c.CourseName == courseName)
                .SelectMany(c => c.Chapters)
                .ProjectTo<LearnChapterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return chapterDetails;
        }

        public async Task<ChapterDto> GetDetails(string chapterName)
        {
            var ChapterDto = await _context.Chapters.Include(q => q.Contents)
                .ProjectTo<ChapterDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(q => q.ChapterName == chapterName);

            if (ChapterDto == null)
            {
                throw new NotFoundException(nameof(GetDetails), chapterName);
            }

            return ChapterDto;
        }

    }
}
