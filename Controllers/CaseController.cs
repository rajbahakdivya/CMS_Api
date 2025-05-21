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
    public class CaseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CaseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCase([FromBody] CaseDto dto)
        {
            var newCase = new Case
            {
                CaseTitle = dto.CaseTitle,
                Status = dto.Status,
                Court = dto.Court,
                ClientId = dto.ClientId
            };

            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Case added", newCase.Id });
        }

         [HttpGet("client/{clientId}")]
         public async Task<IActionResult> GetCasesByClient(Guid clientId)
         {
           var cases = await _context.Cases
              .Where(c => c.ClientId == clientId)
           .Include(c => c.Client)
           .ToListAsync();

         return Ok(cases);
        }

        

    }
}
