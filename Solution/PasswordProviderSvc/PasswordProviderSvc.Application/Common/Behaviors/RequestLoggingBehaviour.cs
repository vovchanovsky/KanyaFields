using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace PasswordProviderSvc.Application.Common.Behaviors
{
    public class RequestLoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;

        public RequestLoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            string userName = string.Empty;

            _logger.LogInformation("PasswordProviderSvc Request: {Name} {@UserName} {@Request}",
                requestName, userName, request);

            return Task.CompletedTask;
        }
    }
}
