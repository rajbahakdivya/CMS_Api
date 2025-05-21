using CMS_Api.Models;

namespace CMS_Api.Services
{
    public interface ITenantService
    {
        void SetTenant(Tenant tenant);
        Tenant GetTenant();
    }
}
