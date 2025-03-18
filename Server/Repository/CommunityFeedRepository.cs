using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Server.API.Exceptions;
using Server.Contract;
using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.CommunityFeedDtos;

namespace Server.Repository
{
    public class CommunityFeedRepository : GenericRepository<CommunityFeed>, ICommunityFeedRepository
    {
        private readonly ServerDbContext _context;
        private readonly IMapper _mapper;

        public CommunityFeedRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task UpvotePost(int postId)
        {
            var post = await _context.CommunityFeeds.FindAsync(postId);
            if (post == null)
            {
                throw new NotFoundException("CommunityPost", postId);
            }
            post.Upvotes += 1;
            await _context.SaveChangesAsync();
        }

        public async Task DownvotePost(int postId)
        {
            var post = await _context.CommunityFeeds.FindAsync(postId);
            if (post == null)
            {
                throw new NotFoundException("CommunityPost", postId);
            }
            // Ensure that upvotes don't go negative
            if (post.Upvotes > 0)
            {
                post.Upvotes -= 1;
            }
            await _context.SaveChangesAsync();
        }
        public async Task<(CommunityFeed MostUpvotedPost, int UpvoteCount)> GetMostUpvotedPostAsync()
        {
            var post = await _context.CommunityFeeds
                                 .OrderByDescending(p => p.Upvotes)
                                 .Select(p => new { Post = p, UpvoteCount = p.Upvotes })
                                 .FirstOrDefaultAsync();

            return (post.Post, post.UpvoteCount);
        }

        public async Task<(CommunityFeed LeastUpvotedPost, int UpvoteCount)> GetLeastUpvotedPostAsync()
        {
            var post = await _context.CommunityFeeds
                                 .OrderBy(p => p.Upvotes)
                                 .Select(p => new { Post = p, UpvoteCount = p.Upvotes })
                                 .FirstOrDefaultAsync();

            return (post.Post, post.UpvoteCount);
        }

        public async Task<IEnumerable<CommunityFeedResponseDto>> GetCommunityFeedsWithAnswersAsync(int contentId)
        {
            var communityFeeds = await _context.Contents
                .Where(c => c.ContentId == contentId)
                .SelectMany(c => c.CommunityFeeds)
                .ProjectTo<CommunityFeedResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync(); 

            return communityFeeds; 
        }

        public async Task<AnswerDto> AddAnswerAsync(CreateAnswerDto createAnswerDto)
        {
            var answer = new Answer
            {
                Content = createAnswerDto.Content,
                PostId = createAnswerDto.PostId,
                PostedDate = DateTime.UtcNow, // Assuming current UTC date/time for simplicity
                Upvotes = 0 // Assuming new answers start with 0 upvotes
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return _mapper.Map<AnswerDto>(answer); // Assuming AutoMapper is used for DTO-entity mapping
        }
        public async Task<AnswerDto> GetAnswerByIdAsync(int answerId)
        {
            var answer = await _context.Answers
                                          .Where(a => a.AnswerId == answerId)
                                          .Select(a => new AnswerDto
                                          {
                                              AnswerId = a.AnswerId,
                                              Content = a.Content,
                                              PostedDate = a.PostedDate,
                                              Upvotes = a.Upvotes
                                          })
                                          .FirstOrDefaultAsync();

            return answer;
        }


    }
}

