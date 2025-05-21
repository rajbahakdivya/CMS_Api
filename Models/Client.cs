using System;

namespace CMS_Api.Models
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClientName { get; set; }
        public string ClientType { get; set; } // "Individual", "Corporate", etc.
        public string? OrganizationName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
