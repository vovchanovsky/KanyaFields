using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationSvc.Application.Models.Messages;
using AuthenticationSvc.Application.Resources;
using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Producer;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthenticationSvc.Application.Messaging
{
    public class MessageHandler : IMessageHandler
    {
        public MessageHandler(
            ILogger<MessageHandler> logger,
            IMessageConsumer<long, AuthenticationSvcRequestMessage> messageConsumer,
            IMessageProducer<long, AuthenticationSvcResponseMessage> messageProducer,
            IMediator mediator)
        {
            _logger = logger;
            _messageConsumer = messageConsumer;
            _messageProducer = messageProducer;
            _mediator = mediator;
        }


        public async Task HandleAsync(CancellationToken cancellationToken)
        {
            try
            {
                var consumeResult = _messageConsumer.Consume(RequestTopicName);
                if (consumeResult is null)
                {
                    return;
                }

                _logger.LogInformation(
                    string.Format(CultureInfo.InvariantCulture, InfoMessages.StartProcessingMessage, 
                        consumeResult.Message.Value.ToString()));

                var commandHandler = MessageHandlerExtension.GetCommandHandler(consumeResult.Message.Value.Command);
                var response = await commandHandler(_mediator, consumeResult.Message.Value);

                await _messageProducer.SendMessageAsync(DateTime.Now.Ticks, response, ResponseTopicName);
                _messageConsumer.Commit(RequestTopicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.MessageHandlerUnexpectedError);
            }
        }


        private readonly ILogger<MessageHandler> _logger;
        private readonly IMessageConsumer<long, AuthenticationSvcRequestMessage> _messageConsumer;
        private readonly IMessageProducer<long, AuthenticationSvcResponseMessage> _messageProducer;
        private readonly IMediator _mediator;
        private const string RequestTopicName = "ApiSvc_AuthenticationSvc_Request_Queue";
        private const string ResponseTopicName = "ApiSvc_AuthenticationSvc_Response_Queue";
    }
}
