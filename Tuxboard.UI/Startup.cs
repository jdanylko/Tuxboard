using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.UI;

namespace Tuxboard.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            var appConfig = new TuxboardConfig();
            Configuration
                .GetSection(nameof(TuxboardConfig))
                .Bind(appConfig);

            services.AddDbContext<TuxDbContext>(options => 
                options.UseSqlServer(appConfig.ConnectionString));

            services.AddControllersWithViews();
            
            services.Configure<RazorViewEngineOptions>(o =>
            {
                //o.ViewLocationFormats.Add(appConfig.WidgetFolder);
                //o.ViewLocationFormats.Add(appConfig.ViewFolder);
                //o.ViewLocationFormats.Add(appConfig.ComponentFolder + RazorViewEngine.ViewExtension);
                o.ViewLocationExpanders.Add(
                    new TuxboardViewLocationExpander(
                        appConfig.WidgetFolder,
                        appConfig.ViewFolder,
                        appConfig.ComponentFolder + RazorViewEngine.ViewExtension));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
