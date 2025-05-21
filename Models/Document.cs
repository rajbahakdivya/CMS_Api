using System;

namespace CMS_Api.Models
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; }
        public string FileType { get; set; } // PDF, PPT, DOC
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UploadedBy { get; set; }
        public string Status { get; set; } // "Pending", "Processed", etc.
        public Guid ClientId { get; set; }
        public Client Client { get; set; }
    }
}
