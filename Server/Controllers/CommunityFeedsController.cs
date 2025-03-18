using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contract;
using Server.Models.CommunityFeedDtos;
using Server.API.Exceptions;
using Microsoft.AspNetCore.OData.Query;
using Server.Data;
using Server.Models.ChapterDtos;
using Server.Repositories;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityFeedsController : ControllerBase
    {
        private readonly ICommunityFeedRepository _communityFeedRepository;
        private readonly IMapper _mapper;

        public CommunityFeedsController(ICommunityFeedRepository communityFeedRepository, IMapper mapper)
        {
            _communityFeedRepository = communityFeedRepository;
            _mapper = mapper;
        }

        // GET: api/CommunityFeeds
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<CommunityFeedDto>>> GetCommunityFeeds()
        {
            var feeds = await _communityFeedRepository.GetAllAsync<CommunityFeedDto>();
            return Ok(feeds);
        }

        // GET: api/CommunityFeeds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommunityFeedDto>> GetCommunityFeed(int id)
        {
            var feedDto = await _communityFeedRepository.GetAsync<CommunityFeedDto>(id);
         
            return Ok(feedDto);
        }
        // GET: api/CommunityFeeds/Answers/5
        [HttpGet("Answers/{id}")]
        public async Task<ActionResult<AnswerDto>> GetAnswer(int id)
        {
            var answerDto = await _communityFeedRepository.GetAnswerByIdAsync(id);

            if (answerDto == null)
            {
                return NotFound();
            }

            return Ok(answerDto);
        }
        [HttpGet("CommunityFeed/by-Questions-Answers/{ContentId}")]
        public async Task<ActionResult<CommunityFeedResponseDto>> GetCourseChapterDetailsById(int ContentId)
        {
            var postIds = await _communityFeedRepository.GetCommunityFeedsWithAnswersAsync(ContentId);
            return Ok(postIds);
        }

        // PUT: api/CommunityFeeds/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommunityFeed(int id, UpdateCommunityFeedDto communityFeedDto)
        {
            if (id != communityFeedDto.PostId)
            {
                return BadRequest();
            }

            try
            {
                await _communityFeedRepository.UpdateAsync(id, communityFeedDto);
            }
            catch (NotFoundException)
            {
                if (!await FeedExists(id))
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

        // POST: api/CommunityFeeds
        [HttpPost("questions")]
        public async Task<ActionResult<CommunityFeed>> PostCommunityFeed(CreateCommunityFeedDto createCommunityFeedDto)
        {
            var feedDto = await _communityFeedRepository.AddAsync<CreateCommunityFeedDto, GetCommunityFeedDto>(createCommunityFeedDto);
            return CreatedAtAction("GetCommunityFeed", new { id = feedDto.PostId }, feedDto);
        }
        // POST: api/Answers
        [HttpPost("answers")]
        public async Task<ActionResult<AnswerDto>> PostAnswer(CreateAnswerDto createAnswerDto)
        {
            // Assuming AddAnswerAsync handles the logic of creating an answer and associating it with a CommunityFeed
            var answerDto = await _communityFeedRepository.AddAnswerAsync(createAnswerDto);


            return CreatedAtAction(nameof(GetAnswer), new { id = answerDto.AnswerId }, answerDto);
        }

        // DELETE: api/CommunityFeeds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommunityFeed(int id)
        {
            await _communityFeedRepository.DeleteAsync(id);
            return NoContent();
        }
        // Upvote a community feed post
        [HttpPatch("upvote/{postId}")]
        public async Task<IActionResult> UpvotePost(int postId)
        {
            try
            {
                await _communityFeedRepository.UpvotePost(postId);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // Downvote a community feed post
        [HttpPatch("downvote/{postId}")]
        public async Task<IActionResult> DownvotePost(int postId)
        {
            try
            {
                await _communityFeedRepository.DownvotePost(postId);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
        // ... existing properties and methods ...

        // GET: api/CommunityFeeds/most-upvoted
        [HttpGet("most-upvoted")]
        public async Task<ActionResult<CommunityFeedDto>> GetMostUpvotedPost()
        {
            var (mostUpvotedPost, upvoteCount) = await _communityFeedRepository.GetMostUpvotedPostAsync();
            if (mostUpvotedPost == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<CommunityFeedDto>(mostUpvotedPost);
            dto.Upvotes = upvoteCount; // Assuming CommunityFeedDto has an Upvotes property
            return Ok(dto);
        }

        // GET: api/CommunityFeeds/least-upvoted
        [HttpGet("least-upvoted")]
        public async Task<ActionResult<CommunityFeedDto>> GetLeastUpvotedPost()
        {
            var (leastUpvotedPost, upvoteCount) = await _communityFeedRepository.GetLeastUpvotedPostAsync();
            if (leastUpvotedPost == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<CommunityFeedDto>(leastUpvotedPost);
            dto.Upvotes = upvoteCount; // Assuming CommunityFeedDto has an Upvotes property
            return Ok(dto);
        }
        private async Task<bool> FeedExists(int id)
        {
            return await _communityFeedRepository.Exists(id);
        }
    }
}
