using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Server.Data;
using Server.Contracts;
using System;
using System.Threading.Tasks;

namespace Server.Repository
{
    public class QRCodeRepository : IQRCodeRepository
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IMapper _mapper;
        private readonly QRCodeService _qrCodeService; // QR code generation service
        private readonly ILogger<QRCodeRepository> _logger;

        public QRCodeRepository(UserManager<ApiUser> userManager, IMapper mapper, QRCodeService qrCodeService, ILogger<QRCodeRepository> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _qrCodeService = qrCodeService;
            _logger = logger;
        }
        // Generates a QR code containing a referral link for a given user
        public async Task<string> GenerateReferralLinkForUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            string referralLink = $"https://serverapi-trev.azurewebsites.net/swagger?ReferrerId={userId}";
            Console.WriteLine($"Generating QR for: {referralLink}");
            var qrCode = _qrCodeService.GenerateQRCode(referralLink);

            user.QRCode = qrCode;
            await _userManager.UpdateAsync(user);

            return qrCode;
        }

        // Validates if a given QR code is associated with a specified user
        public async Task<bool> RedeemQRCodeAsync(string userId, string qrCode)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.QRCode != qrCode)
            {
                return false;
            }

            return true;
        }

        // Retrieves the ID of the user who referred another user
        private async Task<string> GetReferringUserId(string referredUserId)
        {
            var referredUser = await _userManager.FindByIdAsync(referredUserId);
            if (referredUser != null)
            {
                return referredUser.ReferrerId;
            }
            return null;
        }

        // Rewards points to the user who referred another user
        public async Task<bool> HandleReferralAsync(string referralUserId)
        {
            string referringUserId = await GetReferringUserId(referralUserId);
            _logger.LogInformation($"Referring User ID: {referringUserId}");

            if (string.IsNullOrEmpty(referringUserId))
            {
                _logger.LogWarning($"Referral failed: Referring user ID is null or empty for referred user {referralUserId}");
                return false;
            }

            var referringUser = await _userManager.FindByIdAsync(referringUserId);
            if (referringUser == null)
            {
                _logger.LogWarning($"Referral failed: Referring user not found with ID {referringUserId}");
                return false;
            }

            referringUser.Points += 20;
            var updateResult = await _userManager.UpdateAsync(referringUser);
            if (!updateResult.Succeeded)
            {
                _logger.LogError($"Failed to update points for user {referringUserId}. Errors: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                return false;
            }

            _logger.LogInformation($"Successfully updated points for user {referringUserId}");
            return true;
        }

        public async Task<string> GetQRCodeForUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            return user.QRCode;
        }
    }
}
