using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS_Api.DTOs;
using CMS_Api.Models;
using CMS_Api.Services;
using Microsoft.AspNetCore.Authorization;
using CMS_Api.Data;


namespace CMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly AuthService _authService;

        public AuthController(AppDbContext context, IJwtService jwtService, AuthService authService)
        {
            _context = context;
            _jwtService = jwtService;
            _authService = authService;
        }

        // ---------------------- REGISTER TENANT ----------------------
        // Endpoint: POST api/auth/register-tenant
        [HttpPost("register-tenant")]
        public async Task<IActionResult> RegisterTenant([FromBody] TenantRegistrationDto dto)
        {
            // Check if tenant with the same PassportNumber already exists
            if (!string.IsNullOrWhiteSpace(dto.PassportNumber) &&
                await _context.Tenants.AnyAsync(t => t.PassportNumber == dto.PassportNumber))
            {
                return BadRequest("Tenant with this passport number already exists.");
            }

            // Create new tenant object
            var tenant = new Tenant
            {
                AccountType = dto.AccountType,
                OrganizationName = dto.OrganizationName,
                PassportNumber = dto.PassportNumber,
                BarLicenseNumber = dto.BarLicenseNumber,
                LicenseIssuingAuthority = dto.LicenseIssuingAuthority,
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return Ok(new { tenantId = tenant.TenantId, message = "Tenant registered successfully" });
        }

        // ---------------------- REGISTER USER ----------------------
        // Endpoint: POST api/auth/register-user
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
        {
            var tenant = await _context.Tenants.FindAsync(dto.TenantId);
            if (tenant == null)
                return BadRequest("Invalid TenantId.");

            var userExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.TenantId == dto.TenantId);
            if (userExists)
                return BadRequest("User already exists for this tenant.");

            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                TenantId = dto.TenantId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        // ---------------------- LOGIN ----------------------
        // Endpoint: POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",
                token
            });
        }

        // ---------------------- GET TENANT INFO ----------------------
        // Endpoint: GET api/auth/tenant-info
        [Authorize]
        [HttpGet("tenant-info")]
        public async Task<IActionResult> GetTenantInfo()
        {
            var tenantId = GetTenantIdFromClaims();
            var tenant = await _context.Tenants.FindAsync(tenantId);

            if (tenant == null)
                return NotFound("Tenant not found");

            return Ok(tenant);
        }

        // ---------------------- UPDATE CONTACT INFO ----------------------
        // Endpoint: PUT api/auth/update-contact-info
        [Authorize]
        [HttpPut("update-contact-info")]
        public async Task<IActionResult> UpdateContactInfo([FromBody] ContactInfoDto dto)
        {
            var tenantId = GetTenantIdFromClaims();
            var tenant = await _context.Tenants.FindAsync(tenantId);

            if (tenant == null)
                return NotFound("Tenant not found");

            tenant.PrimaryContactNumber = dto.PrimaryContactNumber;
            tenant.SecondaryContactNumber = dto.SecondaryContactNumber;
            tenant.Address = dto.Address;
            tenant.PrimaryUserName = dto.PrimaryUserName;

            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contact info updated successfully" });
        }

        // ---------------------- EXTRACT TENANT ID FROM JWT ----------------------
        private int GetTenantIdFromClaims()
        {
            var tenantIdClaim = User.FindFirst("tenantId")?.Value;
            if (int.TryParse(tenantIdClaim, out int tenantId))
                return tenantId;

            throw new UnauthorizedAccessException("TenantId claim missing from token");
        }
    }
}
