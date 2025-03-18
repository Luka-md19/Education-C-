using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Server.Contract;
using Server.Models;
using AutoMapper;
using System.Threading.Tasks;
using Server.API.Exceptions;
using Server.Models.CourseDtos;
using Server.Data;
using Server.Repositories;
using Server.Models.LecturerDtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Stripe;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CoursesController> _logger;
        private readonly ServerDbContext _context;

        public CoursesController(ICourseRepository courseRepository, IMapper mapper, ILogger<CoursesController> logger, ServerDbContext context)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        [EnableQuery]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetCourseDto>>> GetCourses()
        {
            var courses = await _courseRepository.GetAllAsync<GetCourseDto>();
            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Administrator")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            // Extract the 'uid' claim that contains the user's GUID
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value
                         ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var courseDto = await _courseRepository.GetCourseIfAccessible(userId, id);
                return Ok(courseDto);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // GET: api/Courses/purchased
        [HttpGet("purchased")]
        [Authorize(Roles = "User,Administrator")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetPurchasedCourses()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value
                         ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var courses = await _courseRepository.GetPurchasedCourses(userId);
            return Ok(courses);
        }

        // GET: api/Courses/name/{courseName}
        [HttpGet("name/{courseName}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseResponsesDto>> GetCourseDetails(string courseName)
        {
            var course = await _courseRepository.GetCourseDetailsByNameAsync(courseName);
            if (course == null)
            {
                return NotFound("Course not found.");
            }
            return Ok(course);
        }


        // PUT: api/Courses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")] // Only administrators can update courses
        public async Task<IActionResult> PutCourse(int id, UpdateCourseDto courseDto)
        {
            if (id != courseDto.CourseId)
            {
                return BadRequest();
            }

            try
            {
                await _courseRepository.UpdateAsync(id, courseDto);
            }
            catch (NotFoundException)
            {
                if (!await CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")] // Only administrators can delete courses
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await _courseRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> CourseExists(int id)
        {
            return await _courseRepository.Exists(id);
        }

        [HttpPost]
        public async Task<ActionResult<Chapter>> PostChapter(CreateCourseDto createChapterDto)
        {
            var CourseDto = await _courseRepository.AddAsync<CreateCourseDto, GetCourseDto>(createChapterDto);
            return CreatedAtAction(nameof(GetCourse), new { id = CourseDto.CourseId }, CourseDto);
        }

        // POST: api/Courses/content/completion
        [HttpPost("content/completion")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateContentCompletion([FromBody] ContentCompletionRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            bool success = await _courseRepository.UpdateContentCompletionStatus(userId, request.ContentId, request.IsCompleted);
            if (!success)
            {
                return BadRequest("Unable to update content completion status.");
            }

            return Ok();
        }

        // GET: api/Courses/progress/{courseId}
        [HttpGet("progress/{courseId}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<CourseProgressDto>> GetCourseProgress(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var course = await _courseRepository.GetCourseDetailsByIdAsync(courseId);

            if (course == null)
            {
                return NotFound($"Course with ID '{courseId}' not found.");
            }

            var courseProgressDto = _mapper.Map<CourseProgressDto>(course, opts =>
            {
                opts.Items["userId"] = userId;
            });

            return Ok(courseProgressDto);
        }

        [HttpPost("enroll")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            try
            {
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(request.StripeSessionId);

                if (session.PaymentStatus != "paid")
                {
                    return BadRequest("Payment not successful.");
                }

                // Assuming multiple courseIds are stored in a comma-separated format in metadata
                var courseIdsString = session.Metadata["courseIds"];
                var courseIds = courseIdsString.Split(',').Select(int.Parse);

                List<bool> enrollments = new List<bool>();
                foreach (var courseId in courseIds)
                {
                    var success = await _courseRepository.EnrollUserInCourse(userId, courseId, request.StripeSessionId);
                    enrollments.Add(success);
                }

                if (enrollments.All(e => e))
                {
                    return Ok("Enrollment successful.");
                }
                else
                {
                    return BadRequest("One or more enrollments failed.");
                }
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error during enrollment");
                return StatusCode(StatusCodes.Status500InternalServerError, "Stripe error occurred during enrollment.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during enrollment");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during enrollment.");
            }
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] int[] courseIds) // Changed to [FromBody]
        {
            try
            {
                var courses = new List<Course>();
                foreach (var courseId in courseIds)
                {
                    var course = await _context.Courses.FindAsync(courseId);
                    if (course == null)
                    {
                        return NotFound($"Course with ID {courseId} not found");
                    }
                    courses.Add(course);
                }

                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var lineItems = new List<SessionLineItemOptions>();
                foreach (var course in courses)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = Convert.ToInt64(course.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = course.CourseName,
                            },
                        },
                        Quantity = 1,
                    });
                }

                string courseIdsQueryParam = string.Join(",", courseIds);
                string successUrl = $"https://localhost:7243/success?session_id={{CHECKOUT_SESSION_ID}}&courseIds={courseIdsQueryParam}";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = successUrl,
                    CancelUrl = "https://localhost:7243/cancel",
                    Metadata = new Dictionary<string, string>
            {
                { "courseIds", courseIdsQueryParam },
                { "userId", userId }
            }
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new { Url = session.Url });
            }
            catch (Exception ex)
            {
                // Add detailed logging
                _logger.LogError(ex, "Error creating checkout session");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}