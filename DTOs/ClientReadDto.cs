namespace CMS_Api.DTOs
{
    public class ClientReadDto
    {

        // For Admin : add new client
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string MobileNumber { get; set; }
        public string PassportNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }
}
