using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaInfrastructure.AdminClient;
using KafkaInfrastructure.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PasswordProviderSvc.Application.Messaging;
using PasswordProviderSvc.Application.Resources;

namespace PasswordProviderSvc.Application.HostedServices
{
    public class Worker : BackgroundService
    {
        public Worker(
            ILogger<Worker> logger,
            IMessageHandler messageHandler,
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
            _logger.LogInformation(InfoMessages.StartingWorkerService);
            _logger.LogInformation(string.Format($"Creating new queue: {RequestTopicName}"));

            await KafkaAdminClient.CreateTopicAsync(
                $"{_configuration[EnvironmentVariables.KafkaServiceHost]}:" +
                $"{_configuration[EnvironmentVariables.KafkaServicePort]}",
                RequestTopicName);

            try
            {
                while (stoppingToken.IsCancellationRequested is false)
                {
                    await _messageHandler.HandleAsync(stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, ErrorMessages.WorkerServiceCanceled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.WorkerServiceUnexpectedError);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(InfoMessages.StoppingWorkerService);
            return base.StopAsync(cancellationToken);
        }


        private readonly ILogger<Worker> _logger;
        private readonly IMessageHandler _messageHandler;
        private readonly IConfiguration _configuration;
        private const string RequestTopicName = "ApiSvc_Password_Request_Queue";
    }
}
