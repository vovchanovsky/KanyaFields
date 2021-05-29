using Microsoft.Extensions.Hosting;
using PasswordMediatorSvc.Application;
using PasswordMediatorSvc.Infrastructure;

namespace PasswordMediatorSvc
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
                    services.AddInfrastructure(hostContext.Configuration);
                    services.AddApplication();
                });
    }
}
