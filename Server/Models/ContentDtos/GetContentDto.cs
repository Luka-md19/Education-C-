using System.ComponentModel.DataAnnotations;

namespace Server.Models.ContentDtos
{
    public class GetContentDto : BaseContentDto
    {
        [Key]
        public int ContentId { get; set; }
    }
}
