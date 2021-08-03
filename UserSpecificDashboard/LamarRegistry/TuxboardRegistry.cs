using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Services;

namespace UserSpecificDashboard.LamarRegistry
{
    public class TuxboardRegistry : ServiceRegistry
    {
        public TuxboardRegistry()
        {
            this.AddTransient<IDashboardService, DashboardService>();
            this.AddScoped<ITuxDbContext, TuxDbContext>();
        }
    }
}
