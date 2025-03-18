using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.API.Models.Users;
using Server.Contracts;
using Server.Models.User;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRCodeController : ControllerBase
    {
        private readonly IQRCodeRepository _qrCodeRepository;
        private readonly UserManager<ApiUser> _userManager; // Add this

        public QRCodeController(IQRCodeRepository qrCodeRepository, UserManager<ApiUser> userManager) // Modify this
        {
            _qrCodeRepository = qrCodeRepository;
            _userManager = userManager; // Initialize UserManager
        }

        // POST: api/QRCode/generate
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQRCode([FromBody] LoginDto loginDto)
        {
            try
            {
                var qrCode = await _qrCodeRepository.GenerateReferralLinkForUserAsync(loginDto.Email);
                return Ok(new { QRCode = qrCode });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            
        }



        // GET: api/User/points
        [HttpGet("points")]
        public async Task<IActionResult> GetAllUserPoints()
        {
            var users = await _userManager.Users.ToListAsync(); // Using ToListAsync for async operation
            var userPoints = users.Select(u => new RedeemQrCodeResDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Points = u.Points
            }).ToList();

            return Ok(userPoints);
        }
        [HttpGet("userQRCode")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserQRCode()
        {
            
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Invalid token." });
            }

            try
            {
                var qrCode = await _qrCodeRepository.GetQRCodeForUserIdAsync(userId);

                if (qrCode == null)
                {
                    return NotFound(new { Message = "QR code not found for the user." });
                }

                return Ok(new { QRCode = qrCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}

