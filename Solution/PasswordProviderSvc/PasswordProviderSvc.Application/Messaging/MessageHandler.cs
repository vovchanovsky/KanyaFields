using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Producer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PasswordProviderSvc.Application.Models.Messages;
using PasswordProviderSvc.Application.Resources;
using PasswordProviderSvc.Application.Security;

namespace PasswordProviderSvc.Application.Messaging
{
    public class MessageHandler : IMessageHandler
    {
        public MessageHandler(
            ILogger<MessageHandler> logger,
            IMessageConsumer<long, PasswordRequestMessage> messageConsumer,
            IMessageProducer<long, PasswordResponseMessage> messageProducer,
            IMediator mediator,
            IConfiguration configuration)
        {
            _logger = logger;
            _messageConsumer = messageConsumer;
            _messageProducer = messageProducer;
            _mediator = mediator;
            _configuration = configuration;
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

                var claimsPrincipal = JwtTokenHandler.ValidateJwtToken(_configuration, consumeResult.Message.Value.JwtToken);
                if (claimsPrincipal.IsInRole(Role) is false)
                {
                    throw new Exception(ErrorMessages.UserIsNotAdmin);
                }

                var userIdClaim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim is null)
                {
                    throw new Exception(ErrorMessages.NoSubClaim);
                }

                var commandHandler = MessageHandlerExtension.GetCommandHandler(consumeResult.Message.Value.Command);
                var response = await commandHandler(_mediator, consumeResult.Message.Value, Guid.Parse(userIdClaim.Value));

                await _messageProducer.SendMessageAsync(DateTime.Now.Ticks, response, ResponseTopicName);
                _messageConsumer.Commit(RequestTopicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.MessageHandlerUnexpectedError);
            }
        }


        private readonly ILogger<MessageHandler> _logger;
        private readonly IMessageConsumer<long, PasswordRequestMessage> _messageConsumer;
        private readonly IMessageProducer<long, PasswordResponseMessage> _messageProducer;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private const string RequestTopicName = "ApiSvc_Password_Request_Queue";
        private const string ResponseTopicName = "ApiSvc_Password_Result_Queue";
        private const string Role = "Admin";
    }
}
