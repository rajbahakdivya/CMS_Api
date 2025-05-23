namespace CMS_Api.DTOs
{
    public class ClientCreateDto
    {
        public string AccountType { get; set; }
        public string OrganizationName { get; set; }
        public string PassportNumber { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string RecentCase { get; set; }
    }
}
