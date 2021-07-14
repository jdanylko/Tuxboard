using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace UserDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseLamar<TuxboardRegistry>()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        //public static async Task Main(string[] args)
        //{
        //    var builder = new HostBuilder();
        //    builder.UseLamar<TuxboardRegistry>()
        //        .ConfigureAppConfiguration((hostingContext, config) =>
        //        {
        //            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        //        })
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        //    await builder.Build().RunAsync();
        //}
    }
}
