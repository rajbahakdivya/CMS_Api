namespace CMS_Api.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string TenantIdentifier { get; set; } // e.g. org code or subdomain
    }
}
