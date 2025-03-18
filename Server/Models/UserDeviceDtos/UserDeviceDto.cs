using Server.Data;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.UserDeviceDtos
{
    public class UserDeviceDto : BaseUserDeviceDto
    {
     
        public int Id { get; set; }
        public virtual ApiUser User { get; set; }  
    }
}
