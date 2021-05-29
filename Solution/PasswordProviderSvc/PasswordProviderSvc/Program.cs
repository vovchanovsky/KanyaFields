using Microsoft.Extensions.Hosting;
using PasswordProviderSvc.Application;
using PasswordProviderSvc.Infrastructure;

namespace PasswordProviderSvc
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddInfrastructure(hostContext.HostingEnvironment, hostContext.Configuration);
                    services.AddApplication();
                });
    }
}
