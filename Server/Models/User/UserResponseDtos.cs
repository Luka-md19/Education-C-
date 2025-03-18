using System.ComponentModel.DataAnnotations;

namespace Server.API.Models.Users
{
    public class UserResponseDto : BaseUserDto
    {

        public string QRCode { get; set; }

    }
}