namespace Server.Models.CommunityFeedDtos
{
    public abstract class BaseCommunityFeedDto
    {
        public string Question { get; set; }
        public DateTime PostedDate { get; set; }

        public int Upvotes { get; set; }

        public int ContentId { get; set; }


    }
}
