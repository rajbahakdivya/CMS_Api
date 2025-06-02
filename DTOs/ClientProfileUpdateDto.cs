namespace CMS_Api.DTOs
{
    public class ClientProfileUpdateDto
    {
        //For Client Detail Page: Profile

        public string ClientName { get; set; }
        public string AccountType { get; set; } = "Individual";
        public string? OrganizationName { get; set; }
        public string PassportNumber { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
