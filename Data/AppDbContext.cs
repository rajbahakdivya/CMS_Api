using Microsoft.EntityFrameworkCore;
using CMS_Api.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<Document> Documents { get; set; }


}
