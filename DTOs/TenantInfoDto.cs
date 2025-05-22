namespace CMS_Api.DTOs
{
    public class TenantInfoDto
    {
        public int TenantId { get; set; }
        public string AccountType { get; set; }
        public string OrganizationName { get; set; }
        public string PassportNumber { get; set; }
        public string BarLicenseNumber { get; set; }
        public string LicenseIssuingAuthority { get; set; }
        public string? PrimaryContactNumber { get; set; }
        public string? SecondaryContactNumber { get; set; }
        public string? Address { get; set; }
        public string? PrimaryUserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
