using Server.Data;

namespace Server.Models.UserDeviceDtos
{
    public abstract class BaseUserDeviceDto
    {
        public string UserId { get; set; }  
        public string DeviceIdentifier { get; set; }
        public string IPAddress { get; set; } // Add IP address property
        public DateTime LastAccessTime { get; set; }
    }
}
