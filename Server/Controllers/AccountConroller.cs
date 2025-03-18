using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.API.Models.Users;
using Server.Identity;
using Server.Repositories;
using Server.API.Contract;
using Server.API.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Data;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApiUser> _signInManager;

        public AccountController(
            UserManager<ApiUser> userManager,
            SignInManager<ApiUser> signInManager,
            IAuthManager authManager,
            ILogger<AccountController> logger,
            IUserDeviceRepository userDeviceRepository,
            IMapper mapper,
            HttpClient httpClient)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authManager = authManager;
            _logger = logger;
            _userDeviceRepository = userDeviceRepository;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RegisterUser([FromBody] ApiUserDto apiUserDto)
        {
            _logger.LogInformation($"Registration Attempt for {apiUserDto.Email}");
            var errors = await _authManager.ValidateUsers(apiUserDto);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost]
        [Route("registerWithUniqueQrCode")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponseDto>> RegisterUserWithReferral([FromBody] ApiUserDto apiUserDto, string referralId)
        {
            _logger.LogInformation($"Registration Attempt for {apiUserDto.Email}");

            var errors = await _authManager.ValidateUser(apiUserDto, referralId);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(apiUserDto.Email);
            if (user == null)
            {
                _logger.LogError($"User {apiUserDto.Email} is not found after successful registration.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var userResponseDto = _mapper.Map<UserResponseDto>(user);

            return Ok(userResponseDto);
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            string deviceIdentifier = Request.Headers["User-Agent"].ToString();
            if (string.IsNullOrEmpty(deviceIdentifier))
            {
                deviceIdentifier = "DefaultDevice";
            }
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            var loginResponse = await _authManager.Login(loginDto, deviceIdentifier, ipAddress);

            if (loginResponse == null)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            if (!string.IsNullOrEmpty(loginResponse.ErrorMessage))
            {
                return BadRequest(new { Message = loginResponse.ErrorMessage });
            }

            return Ok(loginResponse);
        }

        [HttpPost]
        [Route("register-admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateAdminUser([FromBody] ApiUserDto apiUserDto)
        {
            var errors = await _authManager.CreateAdminUser(apiUserDto);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            var authResponse = await _authManager.VerifyRefreshToken(request);

            if (authResponse == null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);
        }

        [HttpGet]
        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            string clientId = "738232612369-ul0p0l7d66eshli51ckg7782elii5keq.apps.googleusercontent.com";
            string redirectUri = "https://localhost:7243/api/AccountController/signin-google";
            string responseType = "code";
            string scope = "email profile openid";
            string state = Guid.NewGuid().ToString("N");
            HttpContext.Session.SetString("oauth-state", state);

            _logger.LogInformation($"State sent to Google: {state}");

            string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type={responseType}&scope={scope}&access_type=offline&prompt=consent&state={state}";

            return Redirect(authUrl);
        }

        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleCallback(string code, string returnedState)
        {
            _logger.LogInformation($"State returned from Google: {returnedState}");
            var expectedState = HttpContext.Session.GetString("oauth-state");
            if (returnedState != expectedState)
            {
                _logger.LogError("Invalid state parameter");
                return BadRequest("Invalid state parameter");
            }

            try
            {
                var tokens = await ExchangeCodeForTokensAsync(code);
                var googleUser = await _authManager.GetGoogleUserData(tokens.AccessToken);
                var user = await _authManager.SignInWithGoogleAsync(tokens.AccessToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during Google sign-in: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        private async Task<TokenResponse> ExchangeCodeForTokensAsync(string code)
        {
            var requestData = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = "738232612369-ul0p0l7d66eshli51ckg7782elii5keq.apps.googleusercontent.com",
                ["client_secret"] = "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",
                ["redirect_uri"] = "https://localhost:7243/api/AccountController/signin-google",
                ["grant_type"] = "authorization_code"
            };

            var requestContent = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error exchanging code for tokens.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenResponse>(responseContent);
        }
    }
}
