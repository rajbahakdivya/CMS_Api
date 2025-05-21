using System;

namespace CMS_Api.DTOs
{
    public class DocumentDto
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string UploadedBy { get; set; }
        public string Status { get; set; }
        public Guid ClientId { get; set; }
    }
}
