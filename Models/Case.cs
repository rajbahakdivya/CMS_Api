using System;

namespace CMS_Api.Models
{
    public class Case
    {
        public int Id { get; set; } // Case ID (auto-increment)
        public string CaseTitle { get; set; }
        public string Status { get; set; } // "Active", "Closed", etc.
        public string Court { get; set; }  // e.g., "Supreme Court"

        public int ClientId { get; set; } // Foreign key
        public Client Client { get; set; } // Navigation property

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
