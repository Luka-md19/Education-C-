using Microsoft.AspNetCore.Mvc;
using Server.Contract;
using Server.Models;
using AutoMapper;
using System.Threading.Tasks;
using Server.API.Exceptions;
using Server.Models.ContentDtos;
using Server.Data;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IContentRepository _contentRepository;
        private readonly IMapper _mapper;

        public ContentsController(IContentRepository contentRepository, IMapper mapper)
        {
            _contentRepository = contentRepository;
            _mapper = mapper;
        }

        // GET: api/Contents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetContentDto>>> GetContents()
        {
            var contents = await _contentRepository.GetAllAsync<GetContentDto>();
            return Ok(contents);
        }
        // GET: api/contentsResponse
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<GetContentDto>>> Content(int id)
        {
            var contentsResponse = await _contentRepository.GetAsync<GetContentDto>(id);
            return Ok(contentsResponse);
        }



        // PUT: api/Contents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContent(int id, UpdateContentDto contentDto)
        {
            if (id != contentDto.ContentId)
            {
                return BadRequest();
            }

            try
            {
                await _contentRepository.UpdateAsync<UpdateContentDto>(id, contentDto);
            }
            catch (NotFoundException)
            {
                if (!await ContentExists(id))
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

        // POST: api/Contents
        [HttpPost]
        public async Task<ActionResult<Content>> PostContent(CreateContentDto createContentDto)
        {
            var contentDto = await _contentRepository.AddAsync<CreateContentDto, GetContentDto>(createContentDto);
            return CreatedAtAction("Content", new { id = contentDto.ContentId }, contentDto);
        }

        // DELETE: api/Contents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            await _contentRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> ContentExists(int id)
        {
            return await _contentRepository.Exists(id);
        }
    }
}
