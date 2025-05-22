using CMS_Api.DTOs;
using CMS_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using CMS_Api.Data;

namespace CMS_Api.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> RegisterUserAsync(RegisterUserDto dto)
        {

            // Check if email already exists for the tenant
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.TenantId == dto.TenantId);
            if (existingUser != null)
            {
                return "User with this email already exists for the tenant.";
            }

            var hashedPassword = HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                Password = hashedPassword,
                TenantId = dto.TenantId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully.";
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
