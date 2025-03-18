using Server.API.Models.Users;

namespace Server.Models.User
{
    public class RedeemQrCodeResDto  : BaseUserDto
    {
        public int Points { get; set; }
    }
}
