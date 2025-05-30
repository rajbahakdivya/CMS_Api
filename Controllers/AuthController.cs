using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS_Api.DTOs;
using CMS_Api.Models;
using CMS_Api.Services;
using Microsoft.AspNetCore.Authorization;
using CMS_Api.Data;
using Azure;


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
            var TenantExists = await _context.Tenants
                .FirstOrDefaultAsync(u => u.BarLicenseNumber == dto.BarLicenseNumber);

            if (TenantExists == null)
            {
                return NotFound("Tenant not found");
            }
            var token = _authService.GenerateJwtToken(TenantExists.TenantId);

            
            return Ok(new { errorcode = 0, message = "Tenant registered successfully" , token  = token });
        }

        // ---------------------- REGISTER USER ----------------------
        // Endpoint: POST api/auth/register-user
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromHeader(Name = "Tenant-ID")] int tenantId, [FromBody] RegisterUserDto dto)
        {
            var tenant = tenantId;
            if (tenant == null)
                return BadRequest("Invalid TenantID or TenantID missing.");

            var userExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (userExists)
                return BadRequest("Username already taken, try another email ID or create new account");

            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                TenantId = tenantId
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

            var token = _jwtService.GenerateUserToken(user.Id);

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

            var tenantDto = new TenantInfoDto
            {
                TenantId = tenant.TenantId,
                AccountType = tenant.AccountType,
                OrganizationName = tenant.OrganizationName,
                PassportNumber = tenant.PassportNumber,
                BarLicenseNumber = tenant.BarLicenseNumber,
                LicenseIssuingAuthority = tenant.LicenseIssuingAuthority,
                PrimaryContactNumber = tenant.PrimaryContactNumber,
                SecondaryContactNumber = tenant.SecondaryContactNumber,
                Address = tenant.Address,
                PrimaryUserName = tenant.PrimaryUserName,
                CreatedAt = tenant.CreatedAt
            };

            return Ok(tenantDto);
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
