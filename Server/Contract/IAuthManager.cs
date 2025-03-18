using Server.API.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Contract;
using Server.Data;

namespace Server.API.Contract
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> ValidateUser(ApiUserDto userDto, string referralId);
        Task<IEnumerable<IdentityError>> ValidateUsers(ApiUserDto userDto);
        Task<AuthResponseDto> Login(LoginDto loginDto, string deviceIdentifier, string ipAddress);
        Task<IEnumerable<IdentityError>> CreateAdminUser(ApiUserDto userDto);

        Task<string> CreateRefreshToken();
        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
        Task<DeviceUpdateResult> HandleDeviceRegistrationOrUpdate(string userId, string deviceIdentifier, string ipAddress);
        Task<string> GenerateToken();

        Task<ApiUser> SignInWithGoogleAsync(string accessToken);
        Task<GoogleUser> GetGoogleUserData(string accessToken);





    }
}
