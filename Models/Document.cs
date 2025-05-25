using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_Api.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }  // Original file name

        [Required]
        public string FilePath { get; set; }  // Stored file path (unique name)

        [Required]
        public string FileType { get; set; }  // e.g., PDF, DOCX

        public string? Status { get; set; }   // optional status (e.g., "Approved")

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // upload date/time

        public DateTime? UpdatedAt { get; set; }

        public string? UploadedBy { get; set; }

        // Foreign key to Client
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        // Navigation property
        public Client Client { get; set; }


        // Update Page
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CaseName { get; set; }


    }
}
