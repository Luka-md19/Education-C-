namespace Server.Models.CommunityFeedDtos
{
    public class AnswerDto
    {
        public int AnswerId { get; set; }
        public string Content {get; set;}
        public DateTime PostedDate { get; set; }
        public int Upvotes { get; set; }
    }
}
