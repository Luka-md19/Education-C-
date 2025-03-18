using Microsoft.AspNetCore.Mvc;
using Server.Contract;
using Server.Models;
using AutoMapper;
using System.Threading.Tasks;
using Server.API.Exceptions;
using Server.Models.DepartmentDtos;
using Server.Data;
using Server.Models.CourseDepartmentDtos;
using Server.Models.FacultyDtos;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentsController(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDepartmentDto>>> GetDepartments()
        {
            var departments = await _departmentRepository.GetAllAsync<GetDepartmentDto>();
            return Ok(departments);
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var departmentDto = await _departmentRepository.GetAsync<DepartmentDto>(id);
            if (departmentDto == null)
            {
                return NotFound();
            }

            return Ok(departmentDto);
        }



        // GET: api/Departments/5/Courses
        [HttpGet("{id}/Courses")]
        public async Task<ActionResult<IEnumerable<CourseDepartmentDto>>> GetDepartmentCourses(int id)
        {
            var courses = await _departmentRepository.GetCoursesByDepartmentId(id);
            if (courses == null || !courses.Any())
            {
                return NotFound($"No courses found for department with ID {id}.");
            }

            return Ok(courses);
        }
            // PUT: api/Departments/5
            [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, UpdateDepartmentDto departmentDto)
        {
            if (id != departmentDto.DepartmentId)
            {
                return BadRequest();
            }

            try
            {
                await _departmentRepository.UpdateAsync<UpdateDepartmentDto>(id, departmentDto);
            }
            catch (NotFoundException)
            {
                if (!await DepartmentExists(id))
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

        // POST: api/Departments
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(CreateDepartmentDto createDepartmentDto)
        {
            var departmentDto = await _departmentRepository.AddAsync<CreateDepartmentDto, GetDepartmentDto>(createDepartmentDto);
            return CreatedAtAction("GetDepartment", new { id = departmentDto.DepartmentId }, departmentDto);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _departmentRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> DepartmentExists(int id)
        {
            return await _departmentRepository.Exists(id);
        }
    }
}
