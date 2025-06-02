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
            var pvNumber = await _context.Tenants.AnyAsync(t => t.PanVatNumber == dto.PanVatNumber);
            var activate = await _context.Tenants.AnyAsync(t => t.Activate == true);

            // Check if tenant with the same PAN or VAT already exists
            if (pvNumber && activate)
            {
                return BadRequest("Tenant with this PAN or VAT number already exists.");
            }

            if (!pvNumber)
            {
                // Create new tenant object
                var tenant = new Tenant
                {
                    AccountType = dto.AccountType,
                    OrganizationName = dto.OrganizationName,
                    //PassportNumber = dto.PassportNumber,
                    //BarLicenseNumber = dto.BarLicenseNumber,
                    PanVatNumber = dto.PanVatNumber,
                };

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();
            }
            

            var TenantExists = await _context.Tenants
                .FirstOrDefaultAsync(u => u.PanVatNumber == dto.PanVatNumber);

            if (TenantExists == null)
            {
                return NotFound("Tenant not found");
            }
            var token = _authService.GenerateJwtToken(TenantExists.TenantId);

           

            return Ok(new { errorcode = 0, message = "Tenant registered successfully", token = token });
        }

        // ---------------------- REGISTER USER ----------------------
        // Endpoint: POST api/auth/register-user
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromHeader(Name = "Tenant-ID")] int tenantId, [FromHeader(Name = "User-ID")] int userId, [FromBody] RegisterUserDto dto)
        {
            //var tenant = tenantId;
            if (tenantId == null)
                return BadRequest("Invalid TenantID or TenantID missing.");

            var userExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (userExists)
                return BadRequest("Username already taken, try another email ID or create new account");

            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                TenantId = tenantId,
                Name = dto.Name,
                Role = dto.Role,
                CreatedBy = userId == 0 ? 1 : userId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //Update Activate data from false to true

            var tenantExists = await _context.Tenants.FindAsync(tenantId);
            if (tenantExists == null)
                return NotFound("Tenant not found");

            // Update tenant contact info
            tenantExists.Activate = true;

            _context.Tenants.Update(tenantExists);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateUserToken(user.Id, user.TenantId);

            return Ok(new {
                errorcode = 0,
                message = "User registered successfully",
                token = token
            });
        }


        // ---------------------- UPDATE CONTACT INFO ----------------------
        // Endpoint: PUT api/auth/update-contact-info
        [Authorize]
        [HttpPut("update-contact-info")]
        public async Task<IActionResult> UpdateContactInfo(
            [FromHeader(Name = "Tenant-ID")] int? tenantIdHeader,
            [FromHeader(Name = "User-ID")] int? userIdHeader,
            [FromBody] ContactInfoDto dto)
        {
            // Extract tenantId and userId from JWT token claims
            var tenantIdClaimStr = User.FindFirst("tenantId")?.Value;
            var userIdClaimStr = User.FindFirst("userId")?.Value;

            bool tenantClaimParsed = int.TryParse(tenantIdClaimStr, out int tenantIdClaim);
            bool userClaimParsed = int.TryParse(userIdClaimStr, out int userIdClaim);

            if (!tenantClaimParsed || !userClaimParsed)
                return Unauthorized("Invalid token claims: TenantId or UserId missing");

            // Determine final tenantId to use
            int tenantIdToUse;
            if (tenantIdHeader.HasValue)
            {
                if (tenantIdHeader.Value != tenantIdClaim)
                    return Unauthorized("TenantId header does not match token claim");
                tenantIdToUse = tenantIdHeader.Value;
            }
            else
            {
                tenantIdToUse = tenantIdClaim;
            }

            // Determine final userId to use
            int userIdToUse;
            if (userIdHeader.HasValue)
            {
                if (userIdHeader.Value != userIdClaim)
                    return Unauthorized("UserId header does not match token claim");
                userIdToUse = userIdHeader.Value;
            }
            else
            {
                userIdToUse = userIdClaim;
            }

            var tenant = await _context.Tenants.FindAsync(tenantIdToUse);
            if (tenant == null)
                return NotFound("Tenant not found");

            if (dto == null)
                return BadRequest("Contact info data is required");

            // Update tenant contact info
            tenant.PrimaryContactNumber = dto.PrimaryContactNumber;
            tenant.SecondaryContactNumber = dto.SecondaryContactNumber;
            tenant.Address = dto.Address;
            tenant.PrimaryUserName = dto.PrimaryUserName;

            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();

           

            // Optionally set headers
            Response.Headers.Add("User-ID", userIdToUse.ToString());
            Response.Headers.Add("Tenant-ID", tenantIdToUse.ToString());

            return Ok(new {
                errorcode = 0,
                message = "Contact info updated successfully"
                

            });
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

            var token = _jwtService.GenerateUserToken(user.Id, user.TenantId);

            return Ok(new
            {
                errorcode = 0,
                message = "Login successful",
                token
            });
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