namespace CMS_Api.Dtos
{
    public class CaseCreateDto
    {
        public string CaseTitle { get; set; }
        public string Status { get; set; }
        public string Court { get; set; }
        public int ClientId { get; set; }
    }
}
