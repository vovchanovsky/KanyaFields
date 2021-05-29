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
    public class PasswordProviderSvcApiSvcMediator : BackgroundService
    {
        public PasswordProviderSvcApiSvcMediator(
            ILogger<PasswordProviderSvcApiSvcMediator> logger,
            IPasswordProviderSvcMessageHandler messageHandler,
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


        private readonly ILogger<PasswordProviderSvcApiSvcMediator> _logger;
        private readonly IPasswordProviderSvcMessageHandler _messageHandler;
        private readonly IConfiguration _configuration;
        private const string ResponseTopicName = "ApiSvc_Password_Result_Queue";
    }
}
