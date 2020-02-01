using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Tuxboard.UI.LamarRegistry;

namespace Tuxboard.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new HostBuilder();
            builder
                .UseLamar<TuxboardRegistry>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(
                        "appsettings.json", optional: false, reloadOnChange: false);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            builder.Build().Start();
        }
    }
}
