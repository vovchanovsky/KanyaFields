using System;
using System.Threading;
using System.Threading.Tasks;
using ApiSvc.Infrastructure.Models.Messages;
using ApiSvc.Infrastructure.Resources;
using ApiSvc.InfrastructureInterfaces.Services;
using KafkaInfrastructure.AdminClient;
using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiSvc.Infrastructure.Services
{
    public class ResultQueueListener : BackgroundService
    {
        public ResultQueueListener(
            ISynchronizer synchronizer, 
            ILogger<ResultQueueListener> logger,
            IMessageConsumer<long, PasswordResponseMessage> messageConsumer,
            IConfiguration configuration)
        {
            _synchronizer = synchronizer;
            _logger = logger;
            _messageConsumer = messageConsumer;
            _configuration = configuration;
            _topicName = $"ApiSvc_Password_Result_Queue_{_synchronizer.ApplicationInstanceId}";
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
            Task.Factory.StartNew(() => StartConsumerLoopAsync(stoppingToken), stoppingToken);


        private async Task StartConsumerLoopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(InfoMessages.StartingQueueListener);
            _logger.LogInformation(string.Format($"Creating new queue: {_topicName}"));

            await KafkaAdminClient.CreateTopicAsync(
                $"{_configuration[EnvironmentVariables.KafkaServiceHost]}:" +
                $"{_configuration[EnvironmentVariables.KafkaServicePort]}",
                _topicName);

            try
            {
                while (stoppingToken.IsCancellationRequested is false)
                {
                    var consumeResult = _messageConsumer.Consume(_topicName);
                    if (consumeResult is null)
                    {
                        continue;
                    }
                    
                    _synchronizer.ReleaseOne(consumeResult.Message.Value.OperationId);
                    _messageConsumer.Commit(_topicName);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, ErrorMessages.ResultQueueListenerCanceled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessages.ResultQueueListenerUnexpectedError);
            }

            _logger.LogInformation(InfoMessages.StoppingQueueListener);
        }

        private readonly ISynchronizer _synchronizer;
        private readonly ILogger<ResultQueueListener> _logger;
        private readonly IMessageConsumer<long, PasswordResponseMessage> _messageConsumer;
        private readonly IConfiguration _configuration;
        private static string _topicName;
    }
}