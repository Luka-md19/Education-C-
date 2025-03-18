using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.API.Exceptions;
using Server.Contract;
using Server.Data;
using Server.Models.CourseDtos;
using Server.Models.LecturerDtos;
using Server.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturersController : ControllerBase
    {
        private readonly ILecturerRepository _lecturerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LecturersController> _logger;

        public LecturersController(ILecturerRepository lecturerRepository, IMapper mapper, ILogger<LecturersController> logger)
        {
            _lecturerRepository = lecturerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCourselecturerDto>>> GetLecturers()
        {
            
           
                var lecturers = await _lecturerRepository.GetAllAsync<GetCourselecturerDto>();
                return Ok(lecturers);
            
           
        }
       
    
        [HttpGet("{id}")]
        public async Task<ActionResult<CourselecturerDto>> GetLecturer(int id)
        {
                var lecturerDto = await _lecturerRepository.GetlecturerByIdAsync(id);      
                return Ok(lecturerDto);
           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLecturer(int id, UpdateCourselecturerDto lecturerDto)
        {
         
            try
            {
                await _lecturerRepository.UpdateAsync<UpdateCourselecturerDto>(id, lecturerDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Lecturer not found: {LecturerId}", id);
                if (!await _lecturerRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in PutLecturer for LecturerId: {LecturerId}", id);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Lecturer>> PostLecturer(CreateCourselecturerDto createLecturerDto)
        {
           
                var lecturerDto = await _lecturerRepository.AddAsync<CreateCourselecturerDto, GetCourselecturerDto>(createLecturerDto);
                return CreatedAtAction("GetLecturer", new { id = lecturerDto.LecturerId }, lecturerDto);
            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            try
            {
                await _lecturerRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Lecturer not found for deletion: {LecturerId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DeleteLecturer for LecturerId: {LecturerId}", id);
                throw;
            }
        }
    }
}
