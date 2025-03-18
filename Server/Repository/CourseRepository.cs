using AutoMapper;
using Server.Contract;
using Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Server.API.Exceptions;
using Server.Models.CourseDtos;
using AutoMapper.QueryableExtensions;
using Server.Models.ChapterDtos;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Server.Repository
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseRepository> _logger;
        private readonly UserManager<ApiUser> _userManager;
        private ApiUser _user;

        public CourseRepository(ServerDbContext context, UserManager<ApiUser> userManager, IMapper mapper, ILogger<CourseRepository> logger) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }


        public async Task<CourseResponsesDto> GetCourseDetailsByNameAsync(string courseName)
        {
            var courseDetails = await _context.Courses
                                      .Include(c => c.Lecturers)
                                      .Include(c => c.Chapters)
                                          .ThenInclude(chap => chap.Contents)
                                      .Where(c => c.CourseName == courseName)
                                      .ProjectTo<CourseResponsesDto>(_mapper.ConfigurationProvider)
                                      .FirstOrDefaultAsync();

            return courseDetails;
        }

        public async Task<CourseResponsesDto> GetCourseDetailsByIdAsync(int courseId)
        {
            var courseDetails = await _context.Courses
                                      .Include(c => c.Lecturers)
                                      .Include(c => c.Chapters)
                                          .ThenInclude(chap => chap.Contents)
                                      .Where(c => c.CourseId == courseId)
                                      .ProjectTo<CourseResponsesDto>(_mapper.ConfigurationProvider)
                                      .FirstOrDefaultAsync();

            return courseDetails;
        }
        public async Task<CourseResponsesDto> GetCourseIfAccessible(string userId, int courseId)
        {
          
            _logger.LogInformation($"Received userId: {userId} and courseId: {courseId} for access check.");

            bool hasAccess = await _context.UserCourses.AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You do not have access to this course.");
            }

            var courseDetails = await _context.Courses

                               .Include(c => c.CourseName)
                               .Include(c => c.Lecturers)
                               .Include(c => c.Chapters)
                                   .ThenInclude(chap => chap.Contents)
                               .Where(c => c.CourseId == courseId)
                               .ProjectTo<CourseResponsesDto>(_mapper.ConfigurationProvider)
                               .FirstOrDefaultAsync();

            if (courseDetails == null)
            {
                return null;
            }

            return courseDetails;
        }

        public async Task<bool> IsUserEnrolledInCourse(string userId, int courseId)
        {
            return await _context.UserCourses.AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        }

        public async Task<bool> EnrollUserInCourse(string userId, int courseId, string stripeSessionId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException(nameof(Course), courseId);
            }

            var isAlreadyEnrolled = await IsUserEnrolledInCourse(userId, courseId);
            if (isAlreadyEnrolled)
            {
                throw new BadRequestException("User is already enrolled in this course.");
            }

            var sessionService = new SessionService();
            Session session;
            try
            {
                session = await sessionService.GetAsync(stripeSessionId);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error during retrieving session: {stripeSessionId}");
                throw new BadRequestException("Error retrieving Stripe session.");
            }

            if (session.PaymentStatus != "paid")
            {
                throw new BadRequestException("Payment not successful.");
            }

            
            var userCourse = new UserCourse { UserId = userId, CourseId = courseId };
            _context.UserCourses.Add(userCourse);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<IEnumerable<GetCourseDto>> GetPurchasedCourses(string userId)
        {
            _logger.LogInformation($"GetPurchasedCourses called with userId: {userId}");
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException(nameof(ApiUser), userId);
            }

            var courses = await _context.UserCourses
                                        .Where(uc => uc.UserId == userId)
                                        .Select(uc => uc.Course)
                                        .ProjectTo<GetCourseDto>(_mapper.ConfigurationProvider) // Use AutoMapper to project to the new DTO
                                        .ToListAsync();

            return courses;
        }


        public async Task<CourseProgressDto> GetCourseProgressAsync(int courseId, string userId)
        {
            var course = await _context.Courses
                .Include(c => c.Chapters)
                    .ThenInclude(ch => ch.Contents)
                .Include(c => c.UserCourses)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), courseId);
            }

            var progressCalculationService = new ProgressCalculationService(_context);
            double progressPercentage = await progressCalculationService.CalculateProgressPercentageAsync(course, userId);

            var courseProgressDto = new CourseProgressDto
            {
                ProgressPercentage = progressPercentage
                
            };

            return courseProgressDto;
        }


        public async Task<bool> UpdateContentCompletionStatus(string userId, int contentId, bool isCompleted)
        {
            var completionRecord = await _context.UserContentCompletions
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ContentId == contentId);

            if (completionRecord != null)
            {
                completionRecord.IsCompleted = isCompleted;
            }
            else
            {
                
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                var contentExists = await _context.Contents.AnyAsync(c => c.ContentId == contentId);

                if (!userExists || !contentExists)
                {
                    throw new BadRequestException("Invalid User ID or Content ID.");
                }

                _context.UserContentCompletions.Add(new UserContentCompletion
                {
                    UserId = userId,
                    ContentId = contentId,
                    IsCompleted = isCompleted
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }


    }
}