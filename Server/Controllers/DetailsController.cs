using Server.Contract;
using Server.Models;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.API.Exceptions;
using Server.Models.DetailesDtos;
using Server.Data;
using Server.Repository;
using Server.Models.CourseDtos;
using Server.Models.DetailsDtos;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly IDetailsRepository _detailesRepository;
        private readonly IMapper _mapper;

        public DetailsController(IDetailsRepository detailesRepository, IMapper mapper)
        {
            _detailesRepository = detailesRepository;
            _mapper = mapper;
        }

        // GET: api/Details

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDetailsDto>>> GetDetailes()
        {
            var detailes = await _detailesRepository.GetAllAsync<GetDetailsDto>();
            return Ok(detailes);
        }
        


        // GET: api/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetailsDto>> GetDetailes(int id)
        {
            var detailesDto = await _detailesRepository.GetAsync<DetailsDto>(id);
            if (detailesDto == null)
            {
                return NotFound();
            }

            return Ok(detailesDto);
        }

        // PUT: api/Details/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetailes(int id, UpdateDetailsDto detailesDto)
        {
            if (id != detailesDto.DetailsId)
            {
                return BadRequest();
            }

            try
            {
                await _detailesRepository.UpdateAsync<UpdateDetailsDto>(id, detailesDto);
            }
            catch (NotFoundException)
            {
                if (!await DetailesExists(id))
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

        // POST: api/Details
        [HttpPost]
        public async Task<ActionResult<Detail>> PostDetailes(CreateDetailsDto createDetailesDto)
        {
            var detailesDto = await _detailesRepository.AddAsync<CreateDetailsDto, GetDetailsDto>(createDetailesDto);
            return CreatedAtAction("GetDetailes", new { id = detailesDto.DetailsId }, detailesDto);
        }

        // DELETE: api/Details/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetailes(int id)
        {
            await _detailesRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> DetailesExists(int id)
        {
            return await _detailesRepository.Exists(id);
        }
    }
}
