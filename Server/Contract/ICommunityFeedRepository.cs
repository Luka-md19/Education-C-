using Server.Data;
using Server.Models.CommunityFeedDtos;

namespace Server.Contract
{
    public interface ICommunityFeedRepository : IGenericRepository<CommunityFeed>
    {
        Task<(CommunityFeed MostUpvotedPost, int UpvoteCount)> GetMostUpvotedPostAsync();
        Task<(CommunityFeed LeastUpvotedPost, int UpvoteCount)> GetLeastUpvotedPostAsync();
        Task UpvotePost(int postId);
        Task DownvotePost(int postId);
        Task<IEnumerable<CommunityFeedResponseDto>> GetCommunityFeedsWithAnswersAsync(int contentId);
        Task<AnswerDto> AddAnswerAsync(CreateAnswerDto createAnswerDto);
        Task<AnswerDto> GetAnswerByIdAsync(int answerId);
      

    }
}
