using System.Text;
using ApiSvc.Infrastructure.Clients.IdentityProviderClient;
using ApiSvc.Infrastructure.Clients.PasswordProviderClient;
using ApiSvc.Infrastructure.Infrastructure;
using ApiSvc.Infrastructure.Services;
using ApiSvc.InfrastructureInterfaces.Clients.IdentityProviderClient;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient;
using ApiSvc.InfrastructureInterfaces.Services;
using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Producer;
using KafkaInfrastructure.SchemaRegistry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Utilities.DateTimeService;

namespace ApiSvc.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IWebHostEnvironment env, IConfiguration configuration)
        {
            services.AddHostedService<ResultQueueListener>();
            services.AddHostedService<AuthenticationSvcResultQueueListener>();

            services.AddTransient<IDateTime, DateTimeImpl>();
            services.AddSingleton<ISynchronizer, Synchronizer>();
            services.AddSingleton<IDistributedCacheProvider, RedisCacheProvider>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{configuration[EnvironmentVariables.RedisHost]}:" +
                                        $"{configuration[EnvironmentVariables.RedisPort]}";
                options.InstanceName = "ApiSvc_";
            });

            services.AddSingleton<ISchemaRegistryFactory, SchemaRegistryFactory>();
            services.AddSingleton(typeof(IConsumerFactory<,>), typeof(ConsumerFactory<,>));
            services.AddScoped(typeof(IMessageProducer<,>), typeof(MessageProducer<,>));
            services.AddScoped(typeof(IProducerFactory<,>), typeof(ProducerFactory<,>));
            services.AddSingleton(typeof(IMessageConsumer<,>), typeof(MessageConsumer<,>));

            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config =>
                {
                    var publicKey = configuration[EnvironmentVariables.JwtTokenIdentityProviderPublicKey];

                    // TODO: read the standard, these must be DNS addresses
                    var issuer = configuration[EnvironmentVariables.JwtTokenIssuer];
                    var audience = configuration[EnvironmentVariables.JwtTokenAudience];

                    var publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);

                    // TODO: Use asymmetric encryption or certificate
                    var key = new SymmetricSecurityKey(publicKeyBytes);

                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = key
                    };
                });


            if (env.IsDevelopment())
            {
                services.AddSingleton<IPasswordProviderClient, FakePasswordProviderClient>();
                services.AddSingleton<IIdentityProviderClient, FakeIdentityProviderClient>();
            }
            else
            {
                services.AddScoped<IPasswordProviderClient, QueuePasswordProviderClient>();
                services.AddScoped<IIdentityProviderClient, QueueIdentityProviderClient>();
            }

            return services;
        }
    }
}
