using AutoMapper;
using CMS_Api.Data;
using CMS_Api.Dtos;
using CMS_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CaseController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/case
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaseReadDto>>> GetCases()
        {
            var cases = await _context.Cases.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CaseReadDto>>(cases));
        }

        // GET: api/case/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CaseReadDto>> GetCase(int id)
        {
            var caseEntity = await _context.Cases.FindAsync(id);
            if (caseEntity == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CaseReadDto>(caseEntity));
        }

        // POST: api/case
        [HttpPost]
        public async Task<ActionResult<CaseReadDto>> CreateCase(CaseCreateDto dto)
        {
            var caseEntity = _mapper.Map<Case>(dto);
            _context.Cases.Add(caseEntity);
            await _context.SaveChangesAsync();

            var readDto = _mapper.Map<CaseReadDto>(caseEntity);
            return CreatedAtAction(nameof(GetCase), new { id = readDto.Id }, readDto);
        }

        // DELETE: api/case/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var caseEntity = await _context.Cases.FindAsync(id);
            if (caseEntity == null)
            {
                return NotFound();
            }

            _context.Cases.Remove(caseEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
