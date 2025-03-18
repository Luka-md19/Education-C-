using System.ComponentModel.DataAnnotations;

namespace Server.API.Models.Users
{
    public class BaseUserDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name should have at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "First name should not exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Last name should have at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Last name should not exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
       
    }
}