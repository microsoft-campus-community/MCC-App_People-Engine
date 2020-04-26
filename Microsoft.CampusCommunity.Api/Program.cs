using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.CampusCommunity.Api
{
    /// <summary>
    /// Startup program for API
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method of program
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates host builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider((context, options) =>
                {
                    // see https://stackoverflow.com/a/59605979
                    //var isDevelopment = context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.EnvironmentName.StartsWith("Development");
                    var isDevelopment = true;
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    builder
                        .AddJsonFile("appsettings.json", true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName.ToLower()}.json", true);

                    // add Azure environment variables
                    builder.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.AddApplicationInsights();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseContentRoot(Directory.GetCurrentDirectory());
        }
    }
}