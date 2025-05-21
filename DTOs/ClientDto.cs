using System;

namespace CMS_Api.DTOs
{
    public class ClientDto
    {
        public string ClientName { get; set; }
        public string ClientType { get; set; }
        public string? OrganizationName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
    }

}