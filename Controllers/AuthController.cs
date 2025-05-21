using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS_Api.DTOs;
using CMS_Api.Models;
using CMS_Api.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IJwtService _jwtService;

        public AuthController(AppDbContext context, ITenantService tenantService, IJwtService jwtService)
        {
            _context = context;
            _tenantService = tenantService;
            _jwtService = jwtService;
        }

        [HttpPost("register-tenant")]
        public async Task<IActionResult> RegisterTenant([FromBody] TenantRegistrationDto dto)
        {
            if (_context.Tenants.Any(t => t.PanVatNumber == dto.PanVatNumber))
                return BadRequest("Tenant already exists.");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid().ToString(),
                Name = dto.OrganizationName,
                PanVatNumber = dto.PanVatNumber,
                BarLicenseNumber = dto.BarLicenseNumber,
                LicenseIssuingAuthority = dto.LicenseIssuingAuthority,
                AccountType = dto.AccountType
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return Ok(new { tenantId = tenant.Id, identifier = tenant.Identifier });
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Identifier == dto.TenantIdentifier);
            if (tenant == null) return BadRequest("Invalid tenant.");

            if (_context.Users.Any(u => u.Email == dto.Email && u.TenantId == tenant.Id))
                return BadRequest("User already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                TenantId = tenant.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Identifier == dto.TenantIdentifier);
            if (tenant == null)
                return Unauthorized("Invalid tenant");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.TenantId == tenant.Id);
            if (user == null)
                return Unauthorized("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",
                token,
                tenant = tenant.Name,
                userId = user.Id,
                userEmail = user.Email
            });
        }

        [HttpPost("add-contact-info")]
        public async Task<IActionResult> AddContactInfo([FromBody] ContactInfoDto dto)
        {
            var tenant = await _context.Tenants.FindAsync(dto.TenantId);
            if (tenant == null)
                return NotFound("Tenant not found.");

            tenant.ContactPersonFullName = dto.ContactPersonFullName;
            tenant.ContactEmail = dto.ContactEmail;
            tenant.ContactPhoneNumber = dto.ContactPhoneNumber;

            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contact information updated successfully." });
        }
    }
}
