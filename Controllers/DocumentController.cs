using AutoMapper;
using CMS_Api.Data;
using CMS_Api.DTOs;
using CMS_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CMS_Api.Controllers
{
    [Route("api/clients/{clientId}/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public DocumentsController(AppDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        // GET: api/clients/5/documents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(int clientId)
        {
            var documents = await _context.Documents
                .Where(d => d.ClientId == clientId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            var documentDtos = _mapper.Map<List<DocumentDto>>(documents);

            // Set DownloadUrl manually because it depends on routing/URL helpers
            foreach (var doc in documentDtos)
            {
                doc.DownloadUrl = Url.Action(nameof(DownloadDocument), new { clientId, documentId = doc.Id });
            }

            return Ok(documentDtos);
        }

        // POST: api/clients/5/documents
        [HttpPost]
        public async Task<ActionResult> UploadDocument(int clientId, [FromForm] DocumentUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded");

            if (string.IsNullOrEmpty(dto.FileType))
                return BadRequest("File type is required");

            var client = await _context.Clients.FindAsync(clientId);
            if (client == null) return NotFound("Client not found");

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "documents");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            // Use timestamp for unique filename
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var safeFileName = Path.GetFileNameWithoutExtension(dto.File.FileName);
            var extension = Path.GetExtension(dto.File.FileName);
            var uniqueName = $"{timestamp}_{safeFileName}{extension}";
            var filePath = Path.Combine(uploadsDir, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // Map from DTO to entity and set extra fields
            var document = _mapper.Map<Document>(dto);
            document.FileName = dto.File.FileName;
            document.FilePath = uniqueName;
            document.UploadedBy = User.Identity?.Name ?? "System";
            document.ClientId = clientId;

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new { document.Id });
        }

        // GET: api/clients/5/documents/3/download
        [HttpGet("{documentId}/download")]
        public async Task<IActionResult> DownloadDocument(int clientId, int documentId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId && d.ClientId == clientId);

            if (document == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "uploads", "documents", document.FilePath);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetMimeType(document.FileType), document.FileName);
        }


        [HttpPut("{documentId}")]
        public async Task<ActionResult<DocumentDto>> UpdateDocument(int clientId, int documentId, [FromForm] DocumentUploadDto dto)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId && d.ClientId == clientId);

            if (document == null)
                return NotFound("Document not found");

            // Update metadata
            if (!string.IsNullOrEmpty(dto.Title))
                document.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                document.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.CaseName))
                document.CaseName = dto.CaseName;

            // Handle file update
            if (dto.File != null && dto.File.Length > 0)
            {
                // Delete old file
                var oldFilePath = Path.Combine(_env.WebRootPath, "uploads", "documents", document.FilePath);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save new file
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                var safeFileName = Path.GetFileNameWithoutExtension(dto.File.FileName);
                var extension = Path.GetExtension(dto.File.FileName);
                var uniqueName = $"{timestamp}_{safeFileName}{extension}";
                var newFilePath = Path.Combine(_env.WebRootPath, "uploads", "documents", uniqueName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                // Update file properties
                document.FileName = dto.File.FileName;
                document.FilePath = uniqueName;
                document.FileType = dto.FileType ?? document.FileType;
            }

            // Set update timestamp
            document.UpdatedAt = DateTime.UtcNow;

            _context.Documents.Update(document);
            await _context.SaveChangesAsync();

            // Return updated document
            var documentDto = _mapper.Map<DocumentDto>(document);
            documentDto.DownloadUrl = Url.Action(nameof(DownloadDocument), new { clientId, documentId });

            return Ok(documentDto);
        }

        // DELETE: api/clients/5/documents/3
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int clientId, int documentId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId && d.ClientId == clientId);

            if (document == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "uploads", "documents", document.FilePath);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GetMimeType(string fileType)
        {
            return fileType.ToUpper() switch
            {
                "PDF" => "application/pdf",
                "DOC" => "application/msword",
                "DOCX" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "PPT" => "application/vnd.ms-powerpoint",
                "PPTX" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                _ => "application/octet-stream"
            };
        }
    }
}
