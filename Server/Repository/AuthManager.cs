using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using global::Server.API.Contract;
using global::Server.Data;
using global::Server.Repositories;
using global::Server.Contract;
using Server.API.Models.Users;
using Newtonsoft.Json;

namespace Server.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthManager> _logger;
        private ApiUser _user;
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly QRCodeService _qrCodeService;
        private const string _loginProvider = "ServerAPI";
        private const string _refreshToken = "RefreshToken";

        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration, ILogger<AuthManager> logger, IUserDeviceRepository userDeviceRepository, QRCodeService qrCodeService, HttpClient httpClient)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
            this._logger = logger;
            this._userDeviceRepository = userDeviceRepository;
            this._qrCodeService = qrCodeService;
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<IdentityError>> CreateAdminUser(ApiUserDto userDto)
        {
            var _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = userDto.Email;
            var result = await _userManager.CreateAsync(_user, userDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "Administrator");
            }

            return result.Errors;
        }
        public async Task<IEnumerable<IdentityError>> ValidateUsers(ApiUserDto userDto)
        {
            var _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = userDto.Email;
            var result = await _userManager.CreateAsync(_user, userDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;

        }
        public async Task<IEnumerable<IdentityError>> ValidateUser(ApiUserDto userDto, string referralId)
        {
            // Map the DTO to the ApiUser entity
            var _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = userDto.Email;

            // Validate the referralId
            if (!string.IsNullOrEmpty(referralId))
            {
                var referrer = await _userManager.FindByIdAsync(referralId);
                if (referrer == null)
                {
                    // Handle the case where the referrer does not exist
                    return new List<IdentityError> {
                new IdentityError {
                    Code = "InvalidReferralId",
                    Description = "Referral ID is invalid or does not exist."
                }
            };
                }

                // Set the referrer ID if valid
                _user.ReferrerId = referralId;
            }

            // Attempt to create the user
            var creationResult = await _userManager.CreateAsync(_user, userDto.Password);

            // If the user creation is successful, proceed to generate a QR code
            if (creationResult.Succeeded)
            {
                // Use the user's ID to generate a referral link
                string userId = _user.Id; // Assuming Id is the property for the user's ID
                string referralLink = $"https://serverapi-trev.azurewebsites.net/swagger?ReferrerId={userId}";

                var qrCode = _qrCodeService.GenerateQRCode(referralLink);
                _user.QRCode = qrCode;

                // Update the user with the QR code information
                var updateResult = await _userManager.UpdateAsync(_user);
                if (!updateResult.Succeeded)
                {
                    // If the update fails, return the errors
                    return updateResult.Errors;
                }

                // Add the user to the default role
                var addToRoleResult = await _userManager.AddToRoleAsync(_user, "User");
                if (!addToRoleResult.Succeeded)
                {
                    // If adding to role fails, return the errors
                    return addToRoleResult.Errors;
                }
            }

            // Return any errors from the user creation process
            return creationResult.Errors;
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);
            return newRefreshToken;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto, string deviceIdentifier, string ipAddress)
        {
            _logger.LogInformation("Login attempt for user: {Email}", loginDto.Email);

            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (_user == null)
            {
                _logger.LogWarning("No user found with the email: {Email}", loginDto.Email);
                return new AuthResponseDto { ErrorMessage = "User not found." };
            }

            bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);
            if (!isValidUser)
            {
                _logger.LogWarning("Invalid password entered for user: {Email}", loginDto.Email);
                return new AuthResponseDto { ErrorMessage = "Invalid password." };
            }

            var deviceUpdateResult = await HandleDeviceRegistrationOrUpdate(_user.Id, deviceIdentifier, ipAddress);
            switch (deviceUpdateResult)
            {
                case DeviceUpdateResult.Success:
                    var token = await GenerateToken();
                    var refreshToken = await CreateRefreshToken();
                    return new AuthResponseDto
                    {
                        IsAuthSuccessful = true,
                        Token = token,
                        UserId = _user.Id,
                        RefreshToken = refreshToken,
                        Message = "Signin successful"
                    };

                case DeviceUpdateResult.LimitReached:
                    return new AuthResponseDto
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "Device limit reached. Please manage your devices."
                    };

                case DeviceUpdateResult.Error:
                    _logger.LogError("An error occurred during device registration.");
                    return new AuthResponseDto
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "An error occurred during device registration."
                    };

                default:
                    _logger.LogError("Unexpected result from device registration.");
                    return new AuthResponseDto
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "An unexpected error occurred."
                    };
            }
        }



       public async Task<string> GenerateToken()
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id),
            }
            .Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);

            if (_user == null || _user.Id != request.UserId)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);

            if (isValidRefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }
        public async Task<DeviceUpdateResult> HandleDeviceRegistrationOrUpdate(string userId, string deviceIdentifier, string ipAddress)
        {
            var userDevice = new UserDevice
            {
                UserId = userId,
                DeviceIdentifier = deviceIdentifier,
                IPAddress = ipAddress,  // Include the IP address here
                LastAccessTime = DateTime.UtcNow
            };

            // This method now returns the result of the AddOrUpdateUserDeviceAsync method call
            return await _userDeviceRepository.AddOrUpdateUserDeviceAsync(userId, deviceIdentifier, ipAddress);
        }

        public async Task<GoogleUser> GetGoogleUserData(string accessToken)
        {
            var requestUri = $"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}";
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error fetching Google user data.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleUser>(content);
        }


        public async Task<ApiUser> SignInWithGoogleAsync(string accessToken)
        {
            var googleUser = await GetGoogleUserData(accessToken);

            if (!googleUser.VerifiedEmail)
            {
                throw new Exception("Google account email not verified.");
            }

            var user = await _userManager.FindByEmailAsync(googleUser.Email);

            if (user == null)
            {
                
                user = new ApiUser
                {
                    Email = googleUser.Email,
                    UserName = googleUser.Email,
                    FirstName = googleUser.GivenName, 
                    LastName = googleUser.FamilyName, 
                                                      
                };

                var createUserResult = await _userManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception("Failed to create user.");
                }

                // Assign default role to the new user
                var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception("Failed to assign user role.");
                }
            }
            else
            {
                
                bool updateRequired = false;

                
                if (user.FirstName != googleUser.GivenName)
                {
                    user.FirstName = googleUser.GivenName;
                    updateRequired = true;
                }
                if (user.LastName != googleUser.FamilyName)
                {
                    user.LastName = googleUser.FamilyName;
                    updateRequired = true;
                }

   

                if (updateRequired)
                {
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        throw new Exception("Failed to update user details.");
                    }
                }
            }

            return user;
        }







    }
}