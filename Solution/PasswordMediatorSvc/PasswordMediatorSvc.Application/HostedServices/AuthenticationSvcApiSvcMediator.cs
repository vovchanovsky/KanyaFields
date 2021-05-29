using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaInfrastructure.AdminClient;
using KafkaInfrastructure.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PasswordMediatorSvc.Application.Messaging;

namespace PasswordMediatorSvc.Application.HostedServices
{
    public class AuthenticationSvcApiSvcMediator : BackgroundService
    {
        public AuthenticationSvcApiSvcMediator(
            ILogger<AuthenticationSvcApiSvcMediator> logger,
            IAuthenticationSvcMessageHandler messageHandler,
            IConfiguration configuration)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
            Task.Factory.StartNew(() => StartConsumerLoopAsync(stoppingToken), stoppingToken);


        private async Task StartConsumerLoopAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation(InfoMessages.StartingWorkerService);
            _logger.LogInformation(string.Format($"Creating new queue: {ResponseTopicName}"));

            await KafkaAdminClient.CreateTopicAsync(
                $"{_configuration[EnvironmentVariables.KafkaServiceHost]}:" +
                $"{_configuration[EnvironmentVariables.KafkaServicePort]}",
                ResponseTopicName);

            try
            {
                while (stoppingToken.IsCancellationRequested is false)
                {
                    await _messageHandler.HandleAsync(stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex.Message);
                //_logger.LogError(ex, ErrorMessages.WorkerServiceCanceled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                //_logger.LogError(ex, ErrorMessages.WorkerServiceUnexpectedError);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation(InfoMessages.StoppingWorkerService);
            return base.StopAsync(cancellationToken);
        }


        private readonly ILogger<AuthenticationSvcApiSvcMediator> _logger;
        private readonly IAuthenticationSvcMessageHandler _messageHandler;
        private readonly IConfiguration _configuration;
        private const string ResponseTopicName = "ApiSvc_AuthenticationSvc_Response_Queue";
    }
}