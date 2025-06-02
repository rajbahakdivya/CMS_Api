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

        public string PanVatNumber { get; set; }

        public string? PrimaryContactNumber { get; set; } //Organisation Number

        public string? SecondaryContactNumber { get; set; } //Organisation Manager Number

        public string? Address { get; set; }

        public string? PrimaryUserName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool Activate { get; set; } = false;

    }
}
