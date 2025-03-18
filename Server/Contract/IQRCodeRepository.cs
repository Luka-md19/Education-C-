namespace Server.Contracts
{
    public interface IQRCodeRepository
    {
        Task<string> GenerateReferralLinkForUserAsync(string userId);
        Task<bool> RedeemQRCodeAsync(string userId, string qrCode);
        Task<bool> HandleReferralAsync(string referralUserId);
        Task<string> GetQRCodeForUserIdAsync(string userId); 
    }
}
