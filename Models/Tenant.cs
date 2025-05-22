using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Api.Models
{
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }

        [Required]
        public string AccountType { get; set; }

        [Required]
        [StringLength(100)]
        public string OrganizationName { get; set; }

        public string PassportNumber { get; set; }
        public string BarLicenseNumber { get; set; }
        public string LicenseIssuingAuthority { get; set; }

        public string? PrimaryContactNumber { get; set; }
        public string? SecondaryContactNumber { get; set; }
        public string? Address { get; set; }
        public string? PrimaryUserName { get; set; }

        // Navigation property
        public ICollection<Client> Clients { get; set; }
    }
}
