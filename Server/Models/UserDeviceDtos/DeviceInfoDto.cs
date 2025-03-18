namespace Server.Models.UserDeviceDtos
{
    public class DeviceInfoDto
    {
        public int Id { get; set; }
        public string DeviceIdentifier { get; set; }
        public string IPAddress { get; set; } // Add IP address property
        public DateTime LastAccessTime { get; set; }
    }
}
