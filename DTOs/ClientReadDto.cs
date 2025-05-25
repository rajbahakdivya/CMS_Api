namespace CMS_Api.DTOs
{
    public class ClientReadDto
    {

        // For Admin : add new client
        public int ClientId { get; set; }
        public string AccountType { get; set; }
        public string OrganizationName { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string RecentCase { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
