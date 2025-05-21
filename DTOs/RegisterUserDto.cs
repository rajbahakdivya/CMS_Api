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
        public string TenantIdentifier { get; set; }
    }
}
