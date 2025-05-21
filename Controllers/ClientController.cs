using Microsoft.EntityFrameworkCore;
using CMS_Api.DTOs;
using CMS_Api.Models;
using CMS_Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;


namespace CMS_Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] ClientDto dto)
        {
            var client = new Client
            {
                ClientName = dto.ClientName,
                ClientType = dto.ClientType,
                OrganizationName = dto.OrganizationName,
                Email = dto.Email,
                MobileNumber = dto.MobileNumber,
                City = dto.City,
                Country = dto.Country,
                Gender = dto.Gender,
                Description = dto.Description
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Client created successfully", client.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _context.Clients.ToListAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return Ok(client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, ClientDto dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            // update fields
            client.ClientName = dto.ClientName;
            client.ClientType = dto.ClientType;
            client.OrganizationName = dto.OrganizationName;
            client.Email = dto.Email;
            client.MobileNumber = dto.MobileNumber;
            client.City = dto.City;
            client.Country = dto.Country;
            client.Gender = dto.Gender;
            client.Description = dto.Description;

            await _context.SaveChangesAsync();
            return Ok(client);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound(new { message = "Client not found" });

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Client deleted successfully" });
        }

    }
}
