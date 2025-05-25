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

        // (For client profile)PUT: api/Client/5 
        [HttpPut("{id}/addnewClient")]
        public async Task<IActionResult> UpdateBasicInfo(int id, ClientCreateDto dto)
        {
            var existingClient = await _context.Clients.FindAsync(id);
            if (existingClient == null)
                return NotFound();

            _mapper.Map(dto, existingClient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Client/5/profile
        [HttpPut("{id}/clientProfile")]
        public async Task<ActionResult<ClientReadDto>> UpdateClientProfile(
    int id,
    [FromBody] ClientProfileUpdateDto dto)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingClient = await _context.Clients.FindAsync(id);
            if (existingClient == null)
            {
                return NotFound(new { Message = $"Client with ID {id} not found" });
            }

            try
            {
                // Update only profile-related fields
                existingClient.ClientName = dto.ClientName;
                existingClient.AccountType = dto.AccountType;
                existingClient.OrganizationName = dto.OrganizationName;
                existingClient.PassportNumber = dto.PassportNumber;
                existingClient.Email = dto.Email;
                existingClient.Country = dto.Country;
                existingClient.MobileNumber = dto.MobileNumber;
                existingClient.Gender = dto.Gender;
                existingClient.City = dto.City;
                existingClient.Description = dto.Description;
                existingClient.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Return the updated client information
                var updatedClientDto = _mapper.Map<ClientReadDto>(existingClient);
                return Ok(updatedClientDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Message = "An error occurred while updating the client profile" });
            }
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
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
