using System;

namespace CMS_Api.Dtos
{
    public class CaseReadDto
    {
        public int Id { get; set; }
        public string CaseTitle { get; set; }
        public string Status { get; set; }
        public string Court { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
