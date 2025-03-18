namespace Server.API.Models.Users
{
    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }  
        public bool IsAuthSuccessful { get; set; }  
        public string ErrorMessage { get; set; }  
    }
}
