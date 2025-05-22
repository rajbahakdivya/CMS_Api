using Microsoft.EntityFrameworkCore;
using CMS_Api.Services;
using CMS_Api.Models;
using Microsoft.AspNetCore.Http;
using CMS_Api.Data;


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
            var tenantIdHeader = context.Request.Headers["X-Tenant-Id"].ToString();

            if (!int.TryParse(tenantIdHeader, out int tenantId))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid or missing TenantId header");
                return;
            }

            var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.TenantId == tenantId);

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
