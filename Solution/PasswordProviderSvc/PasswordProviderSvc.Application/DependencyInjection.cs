using System.Reflection;
using FluentValidation;
using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Producer;
using KafkaInfrastructure.SchemaRegistry;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PasswordProviderSvc.Application.Common.Behaviors;
using PasswordProviderSvc.Application.HostedServices;
using PasswordProviderSvc.Application.Messaging;

namespace PasswordProviderSvc.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddSingleton<IMessageHandler, MessageHandler>();
            
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehaviour<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            services.AddSingleton<ISchemaRegistryFactory, SchemaRegistryFactory>();
            services.AddSingleton(typeof(IConsumerFactory<,>), typeof(ConsumerFactory<,>));
            services.AddTransient(typeof(IMessageProducer<,>), typeof(MessageProducer<,>));
            services.AddTransient(typeof(IProducerFactory<,>), typeof(ProducerFactory<,>));
            services.AddSingleton(typeof(IMessageConsumer<,>), typeof(MessageConsumer<,>));

            return services;
        }
    }
}