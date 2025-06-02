using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Api.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [ForeignKey("Tenant")]
        public int TenantId { get; set; }

        // Dropdown: "Corporate" or "Individual"
        public string AccountType { get; set; } = "Individual";

        // Only relevant if AccountType is "Corporate"
        public string? OrganizationName { get; set; }

        // Passport Number
        public string PassportNumber { get; set; }

        // Optional fields for display in next page
        public string ClientName { get; set; }         
        public string Email { get; set; }        
        public string Status { get; set; }       
        //public string RecentCase { get; set; }   // Recent case title or status

        // For ClientProfile
        public string Country { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public int CreatedBy { get; set; } 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
