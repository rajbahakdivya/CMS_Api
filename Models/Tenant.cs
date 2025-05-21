namespace CMS_Api.Models
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; } // E.g., subdomain or org code
        public string Name { get; set; }
        public string PanVatNumber { get; set; }
        public string BarLicenseNumber { get; set; }
        public string LicenseIssuingAuthority { get; set; }
        public string AccountType { get; set; } // Corporate or Individual



        public string? ContactPersonFullName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhoneNumber { get; set; }

    }

}
