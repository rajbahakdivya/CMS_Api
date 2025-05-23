using AutoMapper;
using CMS_Api.Data;
using CMS_Api.DTOs;
using CMS_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ClientController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Client
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientReadDto>>> GetClients()
        {
            var clients = await _context.Clients.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ClientReadDto>>(clients));
        }

        // GET: api/Client/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientReadDto>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            return Ok(_mapper.Map<ClientReadDto>(client));
        }

        // POST: api/Client
        [HttpPost]
        public async Task<ActionResult<ClientReadDto>> PostClient(ClientCreateDto dto)
        {
            var client = _mapper.Map<Client>(dto);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var readDto = _mapper.Map<ClientReadDto>(client);
            return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, readDto);
        }

        // PUT: api/Client/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, ClientCreateDto dto)
        {
            var existingClient = await _context.Clients.FindAsync(id);
            if (existingClient == null)
                return NotFound();

            _mapper.Map(dto, existingClient);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Client/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
