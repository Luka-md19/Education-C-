using System.ComponentModel.DataAnnotations;

namespace Server.Models.DetailesDtos
{
    public class GetDetailsDto : BaseDetailsDto
    {
        [Key]
        public int DetailsId { get; set; }
    }
}
