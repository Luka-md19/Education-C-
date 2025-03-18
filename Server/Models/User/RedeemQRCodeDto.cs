using Server.API.Models.Users;

namespace Server.API.Models
{
    public class RedeemQRCodeDto  : BaseUserDto
    {
        public string UserId { get; set; }
        public string QRCode { get; set; }
        public int Points { get; set; }

       

    }
}
