using System;

namespace CMS_Api.DTOs
{
	public class ContactInfoDto
	{
		public Guid TenantId { get; set; } // Or use `Identifier` if you prefer
		public string ContactPersonFullName { get; set; }
		public string ContactEmail { get; set; }
		public string ContactPhoneNumber { get; set; }
	}
}
