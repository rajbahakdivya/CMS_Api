using System;

namespace CMS_Api.Models
{
    public class Case
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CaseTitle { get; set; }
        public string Status { get; set; } // "Active", "Closed", etc.
        public string Court { get; set; }  // e.g., "Supreme Court"
        public Guid ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
