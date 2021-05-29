using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Producer;
using KafkaInfrastructure.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;
using PasswordMediatorSvc.Application.HostedServices;
using PasswordMediatorSvc.Application.Messaging;

namespace PasswordMediatorSvc.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddHostedService<PasswordProviderSvcApiSvcMediator>();
            services.AddHostedService<AuthenticationSvcApiSvcMediator>();

            services.AddSingleton<IPasswordProviderSvcMessageHandler, PasswordProviderMessageHandler>();
            services.AddSingleton<IAuthenticationSvcMessageHandler, AuthenticationSvcMessageHandler>();

            services.AddSingleton<ISchemaRegistryFactory, SchemaRegistryFactory>();
            services.AddSingleton(typeof(IConsumerFactory<,>), typeof(ConsumerFactory<,>));
            services.AddTransient(typeof(IMessageProducer<,>), typeof(MessageProducer<,>));
            services.AddTransient(typeof(IProducerFactory<,>), typeof(ProducerFactory<,>));
            services.AddSingleton(typeof(IMessageConsumer<,>), typeof(MessageConsumer<,>));

            return services;
        }
    }
}