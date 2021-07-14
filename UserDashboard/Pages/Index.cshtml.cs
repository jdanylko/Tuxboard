using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;

namespace UserDashboard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config;

        public Dashboard Dashboard { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            IDashboardService service,
            IConfiguration config)
        {
            _logger = logger;
            _service = service;
            config
                .GetSection(nameof(TuxboardConfig))
                .Bind(_config);
        }

        public async Task OnGet()
        {
            var userId = GetCurrentUserId();

            // General public dashboard.
            if (string.IsNullOrEmpty(userId))
            {
                Dashboard = await _service.GetDashboardAsync(_config);
            }
            else
            {
                // User-specific dashboard
                Dashboard = await _service.GetDashboardForAsync(_config, userId);
            }
        }

        private string GetCurrentUserId()
        {
            if (User.Identity == null 
                || string.IsNullOrEmpty(User.Identity.Name))
                return null;

            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim.Value;
        }
    }
}
