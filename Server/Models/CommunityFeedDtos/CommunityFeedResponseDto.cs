using Server.Models.ChapterDtos;

namespace Server.Models.CommunityFeedDtos
{
    public class CommunityFeedResponseDto
    {
        public int PostId { get; set; }

        public string Question { get; set; }
        public DateTime PostedDate { get; set; }
        public int Upvotes { get; set; }
        public int CourseId { get; set; }
        public int ContentId { get; set; }
        public List<AnswerDto> answers { get; set; }

    }
}
