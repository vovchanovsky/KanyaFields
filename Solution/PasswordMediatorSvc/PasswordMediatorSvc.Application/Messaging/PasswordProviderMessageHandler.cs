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
    public class PasswordProviderMessageHandler : IPasswordProviderSvcMessageHandler
    {
        public PasswordProviderMessageHandler(
            ILogger<PasswordProviderMessageHandler> logger,
            IMessageConsumer<long, PasswordResponseMessage> messageConsumer,
            IMessageProducer<long, PasswordResponseMessage> messageProducer,
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

                await _cache.SetRecordAsync($"PasswordCommand_{consumeResult.Message.Value.OperationId}", consumeResult.Message.Value);

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
            $"ApiSvc_Password_Result_Queue_{applicationInstanceId}";


        private readonly ILogger<PasswordProviderMessageHandler> _logger;
        private readonly IMessageConsumer<long, PasswordResponseMessage> _messageConsumer;
        private readonly IMessageProducer<long, PasswordResponseMessage> _messageProducer;
        private readonly IDistributedCacheProvider _cache;
        private const string ResponseTopicName = "ApiSvc_Password_Result_Queue";
    }
}
