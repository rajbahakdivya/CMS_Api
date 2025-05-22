using CMS_Api.DTOs;
using CMS_Api.Models;
using Microsoft.EntityFrameworkCore;
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

        // ---------------- Register a New User ----------------
        public async Task<string> RegisterUserAsync(RegisterUserDto dto)
        {
            // Check if email already exists for the tenant
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.TenantId == dto.TenantId);

            if (existingUser != null)
            {
                return "User with this email already exists for the tenant.";
            }

            // Hash password using BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

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
    }
}
