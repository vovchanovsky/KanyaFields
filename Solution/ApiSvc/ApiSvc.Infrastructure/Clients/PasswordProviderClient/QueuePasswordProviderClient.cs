using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApiSvc.Domain.Entities;
using ApiSvc.Infrastructure.Models.Enums;
using ApiSvc.Infrastructure.Models.Messages;
using ApiSvc.Infrastructure.Models.Messages.Extensions;
using ApiSvc.Infrastructure.Models.PasswordRequestObjects;
using ApiSvc.Infrastructure.Resources;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient;
using ApiSvc.InfrastructureInterfaces.Services;
using KafkaInfrastructure.Producer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiSvc.Infrastructure.Clients.PasswordProviderClient
{
    public class QueuePasswordProviderClient : IPasswordProviderClient
    {
        public QueuePasswordProviderClient(
            ISynchronizer synchronizer, 
            ILogger<QueuePasswordProviderClient> logger,
            IMessageProducer<long, PasswordRequestMessage> messageProducer,
            IDistributedCacheProvider cache,
            IHttpContextAccessor contextAccessor)
        {
            _synchronizer = synchronizer;
            _logger = logger;
            _messageProducer = messageProducer;
            _cache = cache;
            _contextAccessor = contextAccessor;
        }


        public async Task<IEnumerable<PasswordItem>> GetPasswords()
        {
            var message = new PasswordRequestMessage
            {
                JwtToken = await GetJwtTokenFromContext(),
                OperationId = Guid.NewGuid(),
                CorrelationId = "some",
                ApplicationInstanceId = _synchronizer.ApplicationInstanceId,
                Command = PasswordCommands.GetAllPasswords
            };
            
            var responseMessage = await SendMessageAndGetResultAsync(message);
            var result = responseMessage.GetResponseObject<IEnumerable<PasswordItem>>();

            return result;
        }

        public async Task<PasswordItem> GetPasswordItem(Guid id)
        {
            var getPasswordQuery = new GetPasswordQuery(id);

            var message = new PasswordRequestMessage
            {
                JwtToken = await GetJwtTokenFromContext(),
                OperationId = Guid.NewGuid(),
                CorrelationId = "some",
                ApplicationInstanceId = _synchronizer.ApplicationInstanceId,
                Command = PasswordCommands.GetPasswordItem
            };

            message.SetRequestObject(getPasswordQuery);

            var responseMessage = await SendMessageAndGetResultAsync(message);
            var result = responseMessage.GetResponseObject<IEnumerable<PasswordItem>>();

            return result.FirstOrDefault();
        }

        public async Task DeletePassword(Guid id)
        {
            var deletePasswordCommand = new DeletePasswordCommand(id);

            var message = new PasswordRequestMessage
            {
                JwtToken = await GetJwtTokenFromContext(),
                OperationId = Guid.NewGuid(),
                CorrelationId = "some",
                ApplicationInstanceId = _synchronizer.ApplicationInstanceId,
                Command = PasswordCommands.DeletePassword
            };

            message.SetRequestObject(deletePasswordCommand);

            await _messageProducer.SendMessageAsync(DateTime.Now.Ticks, message, RequestTopicName);
        }

        public async Task<Guid> InsertPassword(PasswordItem passwordItem)
        {
            var createPasswordCommand = new CreatePasswordCommand(passwordItem);

            var message = new PasswordRequestMessage
            {
                JwtToken = await GetJwtTokenFromContext(),
                OperationId = Guid.NewGuid(),
                CorrelationId = "some",
                ApplicationInstanceId = _synchronizer.ApplicationInstanceId,
                Command = PasswordCommands.InsertPassword,
            };

            message.SetRequestObject(createPasswordCommand);

            var responseMessage = await SendMessageAndGetResultAsync(message);
            var result = responseMessage.GetResponseObject<IEnumerable<PasswordItem>>();

            return result.FirstOrDefault()?.Id ?? Guid.Empty;
        }


        private async Task<PasswordResponseMessage> SendMessageAndGetResultAsync(PasswordRequestMessage message)
        {
            await _messageProducer.SendMessageAsync(DateTime.Now.Ticks, message, RequestTopicName);
            await _synchronizer.CreateAndWaitAsync(message.OperationId);

            var recordKey = GetRecordKey(message.OperationId);
            var result = await _cache.GetRecordAsync<PasswordResponseMessage>(recordKey);

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
            $"PasswordCommand_{id}";

        private Task<string> GetJwtTokenFromContext() =>
            (_contextAccessor.HttpContext 
                ?? throw new InvalidOperationException(ErrorMessages.HttpContextIsNull))
            .GetTokenAsync("access_token");

        private readonly ISynchronizer _synchronizer;
        private readonly ILogger<QueuePasswordProviderClient> _logger;
        private readonly IMessageProducer<long, PasswordRequestMessage> _messageProducer;
        private readonly IDistributedCacheProvider _cache;
        private readonly IHttpContextAccessor _contextAccessor;
        private const string RequestTopicName = "ApiSvc_Password_Request_Queue";
    }
}
