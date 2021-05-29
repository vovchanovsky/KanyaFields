using System;
using System.Globalization;
using System.Threading.Tasks;
using ApiSvc.Infrastructure.Models.Enums;
using ApiSvc.Infrastructure.Models.Messages;
using ApiSvc.Infrastructure.Resources;
using ApiSvc.InfrastructureInterfaces.Clients.IdentityProviderClient;
using ApiSvc.InfrastructureInterfaces.Services;
using KafkaInfrastructure.Producer;
using Microsoft.Extensions.Logging;

namespace ApiSvc.Infrastructure.Clients.IdentityProviderClient
{
    public class QueueIdentityProviderClient : IIdentityProviderClient
    {
        public QueueIdentityProviderClient(
            ISynchronizer synchronizer,
            ILogger<QueueIdentityProviderClient> logger,
            IMessageProducer<long, AuthenticationSvcRequestMessage> messageProducer,
            IDistributedCacheProvider cache)
        {
            _synchronizer = synchronizer;
            _logger = logger;
            _messageProducer = messageProducer;
            _cache = cache;
        }

        public async Task RegisterUser(string username, string password)
        {
            var message = new AuthenticationSvcRequestMessage
            {
                OperationId = Guid.NewGuid(),
                CorrelationId = "some",
                ApplicationInstanceId = _synchronizer.ApplicationInstanceId,
                Command = AuthenticationSvcCommands.RegisterUser,
                Username = username,
                Password = password
            };

            await _messageProducer.SendMessageAsync(DateTime.Now.Ticks, message, RequestTopicName);
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var message = new AuthenticationSvcRequestMessage
            {
                OperationId = Guid.NewGuid(),
                CorrelationId = "some",
                ApplicationInstanceId = _synchronizer.ApplicationInstanceId,
                Command = AuthenticationSvcCommands.CreateJwtToken,
                Username = username,
                Password = password
            };

            var responseMessage = await SendMessageAndGetResultAsync(message);

            return responseMessage.JwtToken;
        }


        private async Task<AuthenticationSvcResponseMessage> SendMessageAndGetResultAsync(AuthenticationSvcRequestMessage message)
        {
            await _messageProducer.SendMessageAsync(DateTime.Now.Ticks, message, RequestTopicName);
            await _synchronizer.CreateAndWaitAsync(message.OperationId);

            var recordKey = GetRecordKey(message.OperationId);
            var result = await _cache.GetRecordAsync<AuthenticationSvcResponseMessage>(recordKey);

            if (result is null)
            {
                var errorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessages.NoResultRecordInCache, recordKey);

                _logger.LogError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            if (result.Success)
            {
                return result;
            }

            throw new ApplicationException(string.Format(CultureInfo.InvariantCulture, ErrorMessages.SomethingWentWrong, recordKey));
        }


        private static string GetRecordKey(Guid id) =>
            $"AuthenticationCommand_{id}";



        private readonly ISynchronizer _synchronizer;
        private readonly ILogger<QueueIdentityProviderClient> _logger;
        private readonly IMessageProducer<long, AuthenticationSvcRequestMessage> _messageProducer;
        private readonly IDistributedCacheProvider _cache;

        private const string RequestTopicName = "ApiSvc_AuthenticationSvc_Request_Queue";
    }
}