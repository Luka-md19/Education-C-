using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.API.Exceptions;
using Server.Contract;
using Server.Data;
using Server.Models.FacultyDtos;
using Server.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IMapper _mapper;

        public FacultiesController(IFacultyRepository facultyRepository, IMapper mapper)
        {
            _facultyRepository = facultyRepository;
            _mapper = mapper;
        }

        // GET: api/Faculties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetFacultyDtos>>> GetFaculties()
        {
            var faculties = await _facultyRepository.GetAllAsync<GetFacultyDtos>();
            return Ok(faculties);
        }

        // GET: api/Faculties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacultyDtos>> GetFaculty(int id)
        {
            var facultyDto = await _facultyRepository.GetAsync<FacultyDtos>(id);
            if (facultyDto == null)
            {
                return NotFound();
            }

            return Ok(facultyDto);
        }

        // PUT: api/Faculties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFaculty(int id, UpdateFacultyDtos facultyDto)
        {
            if (id != facultyDto.FacultyId)
            {
                return BadRequest();
            }

            try
            {
                await _facultyRepository.UpdateAsync<UpdateFacultyDtos>(id, facultyDto);
            }
            catch (NotFoundException)
            {
                if (!await FacultyExists(id))
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

        // POST: api/Faculties
        [HttpPost]
        public async Task<ActionResult<Faculty>> PostFaculty(CreateFacultyDtos createFacultyDto)
        {
            var facultyDto = await _facultyRepository.AddAsync<CreateFacultyDtos, GetFacultyDtos>(createFacultyDto);
            return CreatedAtAction("GetFaculty", new { id = facultyDto.FacultyId }, facultyDto);
        }

        // DELETE: api/Faculties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaculty(int id)
        {
            await _facultyRepository.DeleteAsync(id);
            return NoContent();
        }


        // GET: api/Departments/Faculty
        [HttpGet("Faculty/{id}")]
        public async Task<ActionResult<FacultyDetailsDto>> GetFacultyDetails(int id)
        {
            var facultyDetails = await _facultyRepository.FacultyDetailsIdAsync(id);
            return Ok(facultyDetails);
        }

        private async Task<bool> FacultyExists(int id)
        {
            return await _facultyRepository.Exists(id);
        }
    }
}
