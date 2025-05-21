using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS_Api.DTOs;
using CMS_Api.Models;
using System.Threading.Tasks;
using System;

namespace CMS_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddDocument([FromBody] DocumentDto dto)
        {
            var doc = new Document
            {
                FileName = dto.FileName,
                FileType = dto.FileType,
                UploadedBy = dto.UploadedBy,
                Status = dto.Status,
                ClientId = dto.ClientId
            };

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Document uploaded", doc.Id });
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetDocumentsByClient(Guid clientId)
        {
            var documents = await _context.Documents
                .Where(d => d.ClientId == clientId)
                .Include(d => d.Client)
                .ToListAsync();

            return Ok(documents);
        }
    }
}
