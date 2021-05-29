using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordMediatorSvc.Infrastructure.Infrastructure;
using PasswordMediatorSvc.Infrastructure.Services;
using PasswordMediatorSvc.InfrastructureInterfaces.Services;

namespace PasswordMediatorSvc.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDistributedCacheProvider, RedisCacheProvider>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{configuration[EnvironmentVariables.RedisHost]}:" +
                                        $"{configuration[EnvironmentVariables.RedisPort]}";

                options.InstanceName = "ApiSvc_";
            });
            
            return services;
        }
    }
}
