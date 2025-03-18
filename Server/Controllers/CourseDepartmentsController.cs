using Microsoft.AspNetCore.Mvc;
using Server.Contract;
using Server.Models;
using AutoMapper;
using System.Threading.Tasks;
using Server.API.Exceptions;
using Server.Models.CourseDepartmentDtos;
using Server.Data;
using Server.Models.CourseDtos;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseDepartmentsController : ControllerBase
    {
        private readonly ICourseDepartmentRepository _courseDepartmentRepository;
        private readonly IMapper _mapper;

        public CourseDepartmentsController(ICourseDepartmentRepository courseDepartmentRepository, IMapper mapper)
        {
            _courseDepartmentRepository = courseDepartmentRepository;
            _mapper = mapper;
        }

        // GET: api/CourseDepartments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCourseDepartmentDto>>> GetCourseDepartments()
        {
            var courseDepartments = await _courseDepartmentRepository.GetAllAsync<GetCourseDepartmentDto>();
            return Ok(courseDepartments);
        }

        // GET: api/CourseDepartments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDepartmentDto>> GetCourseDepartment(int id)
        {
            var courseDepartmentDto = await _courseDepartmentRepository.GetAsync<CourseDepartmentDto>(id);
            if (courseDepartmentDto == null)
            {
                return NotFound();
            }

            return Ok(courseDepartmentDto);
        }


        // PUT: api/CourseDepartments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseDepartment(int id, CourseDepartmentDto courseDepartmentDto)
        {
            if (id != courseDepartmentDto.CourseId) // Ensure this logic is correct for composite key
            {
                return BadRequest();
            }

            try
            {
                await _courseDepartmentRepository.UpdateAsync<CourseDepartmentDto>(id, courseDepartmentDto);
            }
            catch (NotFoundException)
            {
                if (!await CourseDepartmentExists(id))
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

        // POST: api/CourseDepartments
        [HttpPost]
        public async Task<ActionResult<CourseDepartment>> PostCourseDepartment(CreateCourseDepartmentDto createCourseDepartmentDto)
        {
            var courseDepartmentDto = await _courseDepartmentRepository.AddAsync<CreateCourseDepartmentDto, GetCourseDepartmentDto>(createCourseDepartmentDto);
            return CreatedAtAction("GetCourseDepartment", new { id = courseDepartmentDto.DepartmentId }, courseDepartmentDto);
        }

        // DELETE: api/CourseDepartments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourseDepartment(int id)
        {
            await _courseDepartmentRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> CourseDepartmentExists(int id)
        {
            return await _courseDepartmentRepository.Exists(id);
        }
    }
}
