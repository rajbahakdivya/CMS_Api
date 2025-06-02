using System;

namespace CMS_Api.DTOs
{
	public class ContactInfoDto
	{
        public string? PrimaryContactNumber { get; set; } //Organisation PhoneNumber
        public string? SecondaryContactNumber { get; set; }
        public string? Address { get; set; }
        public string? PrimaryUserName { get; set; }
    
	}
}
