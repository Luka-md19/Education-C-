using Server.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class UserDevice
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("ApiUser")]
    public string UserId { get; set; }

    public virtual ApiUser User { get; set; }

    public string DeviceIdentifier { get; set; }
    public string IPAddress { get; set; } // Add IP address property
    public DateTime LastAccessTime { get; set; }
}
