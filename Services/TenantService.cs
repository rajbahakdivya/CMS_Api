
using CMS_Api.Models;

namespace CMS_Api.Services
{
    public class TenantService : ITenantService
    {
        private Tenant _tenant;
        public void SetTenant(Tenant tenant) => _tenant = tenant;
        public Tenant GetTenant() => _tenant;
    }
}
