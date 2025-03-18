namespace Server.Models.ContentDtos
{
    public class UpdateContentDto : BaseContentDto
    {
        public int ContentId { get; set; }
        public int PostId { get; set; }

    }
}
