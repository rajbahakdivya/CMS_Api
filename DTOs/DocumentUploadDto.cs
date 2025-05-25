using Microsoft.AspNetCore.Http;


namespace CMS_Api.DTOs
{
    public class DocumentUploadDto
    {
            public IFormFile File { get; set; }  // uploaded file from client
            public string FileType { get; set; } // file type like PDF, DOCX, etc.




        // for seperated uploaddocument page
             public string? Title { get; set; }         
             public string? Description { get; set; }   
             public string? CaseName { get; set; }
    }
}
