using System.ComponentModel.DataAnnotations;

namespace Server.Models.UserDeviceDtos
{
    public class GetUserDeviceDto : BaseUserDeviceDto
    {
        [Key]
        public int Id { get; set; }
    }
}
