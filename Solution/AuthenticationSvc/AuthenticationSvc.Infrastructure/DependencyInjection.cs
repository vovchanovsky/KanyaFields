using AuthenticationSvc.Infrastructure.Clients.ClaimsProvider;
using AuthenticationSvc.Infrastructure.Services;
using AuthenticationSvc.InfrastructureInterfaces.Clients.ClaimsProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthenticationSvc.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IHostEnvironment env, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                services.AddSingleton<IUserClaimsProvider, FakeClaimsProvider>();
            }
            else
            {
                services.AddHostedService<CassandraInfrastructureInitializer>();
                services.AddSingleton<IUserClaimsProvider, CassandraUserClaimsProvider>();
            }

            return services;
        }
    }
}