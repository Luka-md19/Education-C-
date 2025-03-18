using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.CourseDepartmentDtos;
using Server.Models.CourseDtos;
using System.Threading.Tasks;

namespace Server.Contract
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<CourseResponsesDto> GetCourseDetailsByNameAsync(string courseName);
        Task<CourseResponsesDto> GetCourseDetailsByIdAsync(int courseId);
        Task<CourseResponsesDto> GetCourseIfAccessible(string userId, int courseId);
        Task<bool> IsUserEnrolledInCourse(string userId, int courseId);
        Task<bool> EnrollUserInCourse(string userId, int courseId, string stripeSessionId);
        Task<IEnumerable<GetCourseDto>> GetPurchasedCourses(string userId);
        Task<CourseProgressDto> GetCourseProgressAsync(int courseId, string userId);
        Task<bool> UpdateContentCompletionStatus(string userId, int contentId, bool isCompleted);
        
        
    }
}
