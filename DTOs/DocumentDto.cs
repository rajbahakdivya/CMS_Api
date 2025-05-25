namespace CMS_Api.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Status { get; set; }
        public string UploadDate { get; set; }
        public string UploadedBy { get; set; }
        public string DownloadUrl { get; set; }
    }
}
