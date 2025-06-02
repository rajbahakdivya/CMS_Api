namespace CMS_Api.DTOs
{
    public class TenantRegistrationDto
    {
        public string AccountType { get; set; } = "Corporate";
        public string OrganizationName { get; set; }
        //public string PassportNumber { get; set; }
        public string PanVatNumber { get; set; }
    }
}
