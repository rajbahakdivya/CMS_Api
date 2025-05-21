using System;

namespace CMS_Api.DTOs
{
    public class CaseDto
    {
        public string CaseTitle { get; set; }
        public string Status { get; set; }
        public string Court { get; set; }
        public Guid ClientId { get; set; }
    }
}
