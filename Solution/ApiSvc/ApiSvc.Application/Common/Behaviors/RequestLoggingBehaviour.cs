using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ApiSvc.Application.Common.Behaviors
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

            _logger.LogInformation("KanyaFields Request: {Name} {@UserName} {@Request}",
                requestName, userName, request);

            return Task.CompletedTask;
        }
    }
}
