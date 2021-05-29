using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Producer;
using Microsoft.Extensions.Logging;
using PasswordMediatorSvc.Application.Models.Messages;
using PasswordMediatorSvc.InfrastructureInterfaces.Services;

namespace PasswordMediatorSvc.Application.Messaging
{
    public class AuthenticationSvcMessageHandler : IAuthenticationSvcMessageHandler
    {
        public AuthenticationSvcMessageHandler(
            ILogger<AuthenticationSvcMessageHandler> logger,
            IMessageConsumer<long, AuthenticationSvcResponseMessage> messageConsumer,
            IMessageProducer<long, AuthenticationSvcResponseMessage> messageProducer,
            IDistributedCacheProvider cache)
        {
            _logger = logger;
            _messageConsumer = messageConsumer;
            _messageProducer = messageProducer;
            _cache = cache;
        }


        public async Task HandleAsync(CancellationToken cancellationToken)
        {
            try
            {
                var consumeResult = _messageConsumer.Consume(ResponseTopicName);
                if (consumeResult is null)
                {
                    return;
                }

                _logger.LogInformation(consumeResult.Message.Value.ToString());

                await _cache.SetRecordAsync($"AuthenticationCommand_{consumeResult.Message.Value.OperationId}", consumeResult.Message.Value);

                var concreteResponseTopic = GetConcreteResponseTopicName(consumeResult.Message.Value.ApplicationInstanceId);
                await _messageProducer.SendMessageAsync(DateTime.Now.Ticks, consumeResult.Message.Value, concreteResponseTopic);
                _messageConsumer.Commit(ResponseTopicName);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, ErrorMessages.MessageHandlerUnexpectedError);
                _logger.LogError(ex.Message);
            }
        }


        private static string GetConcreteResponseTopicName(Guid applicationInstanceId) =>
            $"ApiSvc_AuthenticationSvc_Response_Queue_{applicationInstanceId}";


        private readonly ILogger<AuthenticationSvcMessageHandler> _logger;
        private readonly IMessageConsumer<long, AuthenticationSvcResponseMessage> _messageConsumer;
        private readonly IMessageProducer<long, AuthenticationSvcResponseMessage> _messageProducer;
        private readonly IDistributedCacheProvider _cache;
        private const string ResponseTopicName = "ApiSvc_AuthenticationSvc_Response_Queue";
    }
}