using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PasswordProviderSvc.Infrastructure.Clients.StorageProvider;
using PasswordProviderSvc.Infrastructure.Services;
using PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider;
using Utilities.DateTimeService;

namespace PasswordProviderSvc.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IHostEnvironment env, IConfiguration configuration)
        {
            services.AddSingleton<IDateTime, DateTimeImpl>();

            if (env.IsDevelopment())
            {
                services.AddSingleton<IPasswordStorageProvider, FakePasswordStorageProvider>();
            }
            else
            {
                services.AddHostedService<CassandraInfrastructureInitializer>();
                services.AddScoped<IPasswordStorageProvider, CassandraPasswordStorageProvider>();
            }

            return services;
        }
    }
}