using Microsoft.EntityFrameworkCore;
using CMS_Api.Services;
using CMS_Api.Models;

namespace CMS_Api.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, ITenantService tenantService)
        {
            var identifier = context.Request.Headers["X-Tenant-Identifier"].ToString();
            var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Identifier == identifier);

            if (tenant == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Tenant not found");
                return;
            }

            tenantService.SetTenant(tenant);
            await _next(context);
        }
    }
}
