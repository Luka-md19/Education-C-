using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.CourseDtos;

namespace Server.Contract
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        Task<ChapterDto> GetDetails(string chapterName);
        Task<List<LearnChapterDto>> GetLearnChaptersByCourseNameAsync(string courseName);
        Task<List<LearnChapterDto>> GetLearnChaptersByCourseIdAsync(int courseId);
    }

}
