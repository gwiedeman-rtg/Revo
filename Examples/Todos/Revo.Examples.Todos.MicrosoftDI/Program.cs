using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.Core.Configuration;
using Revo.Core.Core;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.Examples.Todos.MicrosoftDI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Configure Revo using the simplified startup
                    var configuration = context.Configuration;
                    var logger = LoggerFactory.Create(builder => builder.AddConsole())
                        .CreateLogger<Program>();
                    
                    var revoStartup = new RevoMicrosoftDIStartup(services, configuration, logger);
                    revoStartup.Configure();
                })
                .Configure(app =>
                {
                    // Simple middleware configuration
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", async context =>
                        {
                            var serviceLocator = context.RequestServices.GetRequiredService<IServiceLocator>();
                            await context.Response.WriteAsync($"✅ Simplified Microsoft DI approach working!\nService locator type: {serviceLocator.GetType().Name}");
                        });
                    });
                });
    }
}
