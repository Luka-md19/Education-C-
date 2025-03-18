using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Server.Contract;
using Server.Data;
using Server.Models;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using Server.Models.ChapterDtos;
using Server.API.Exceptions;
using Server.Repository;
using Microsoft.AspNetCore.Authorization;
using Server.Models.CourseDtos;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class ChaptersController : ControllerBase
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IMapper _mapper;

        public ChaptersController(IChapterRepository chapterRepository, IMapper mapper)
        {
            _chapterRepository = chapterRepository;
            _mapper = mapper;
        }

        // GET: api/Chapters
        [HttpGet]
        //[Authorize(Roles = "User,Administrator")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetChapterDto>>> GetChapters()
        {
            var chapters = await _chapterRepository.GetAllAsync<GetChapterDto>();
            return Ok(chapters);

        }
        // GET: api/Chapters/chapterName
        [HttpGet("chapters/{chapterName}")]
        //[Authorize(Roles = "User,Administrator")]
        public async Task<ActionResult<ChapterDto>> GetCourseDetails(string chapterName)
        {
            var Chapter = await _chapterRepository.GetDetails(chapterName);
            return Ok(Chapter);
        }


        [HttpGet("learn/by-courseId/{courseId}")] 
        public async Task<ActionResult<LearnChapterDto>> GetCourseChapterDetailsById(int courseId)
        {
            var Chapter = await _chapterRepository.GetLearnChaptersByCourseIdAsync(courseId);
            return Ok(Chapter);
        }

        [HttpGet("learn/by-courseName/{courseName}")] 
        public async Task<ActionResult<LearnChapterDto>> GetCourseChapterDetailsByName(string courseName)
        {
            var course = await _chapterRepository.GetLearnChaptersByCourseNameAsync(courseName);
            return Ok(course);
        }


        // PUT: api/Chapters/5
        [HttpPut("{id}")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutChapter(int id, UpdateChapterDto chapterDto)
        {
            if (id != chapterDto.ChapterId)
            {
                return BadRequest();
            }

            try
            {
                await _chapterRepository.UpdateAsync<UpdateChapterDto>(id, chapterDto);
            }
            catch (NotFoundException)
            {
                if (!await ChapterExists(id))
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

        // POST: api/Chapters
        //[Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<Chapter>> PostChapter(CreateChapterDto createChapterDto)
        {
            var chapterDto = await _chapterRepository.AddAsync<CreateChapterDto, GetChapterDto>(createChapterDto);
            return CreatedAtAction(nameof(GetCourseDetails), new { id = chapterDto.ChapterId }, chapterDto);
        }

        // DELETE: api/Chapters/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            await _chapterRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> ChapterExists(int id)
        {
            return await _chapterRepository.Exists(id);
        }
    }
}