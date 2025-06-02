using System.ComponentModel.DataAnnotations;

namespace CMS_Api.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        

    }
}
