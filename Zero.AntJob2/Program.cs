using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife.Log;

namespace Zero.AntJob2
{
    class Program
    {

        static void Main(string[] args)
        {
            XTrace.UseConsole();
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var startup = new Startup();
                    startup.ConfigureServices(services);

                    services.AddHostedService<Worker>();
                });

    }
}
