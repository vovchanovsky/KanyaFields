using System;
using System.Threading;
using System.Threading.Tasks;
using ApiSvc.Application.Common.Behaviors;
using ApiSvc.Application.Password.Commands.CreatePassword;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiSvc.Application.UnitTests.Behaviors
{
    public class RequestLoggerTests
    {
        private readonly Mock<ILogger<CreatePasswordCommand>> _logger;


        public RequestLoggerTests()
        {
            _logger = new Mock<ILogger<CreatePasswordCommand>>();
        }

        [Fact]
        public async Task ShouldCallLogInformationOnce()
        {
            var requestLogger = new RequestLoggingBehaviour<CreatePasswordCommand>(_logger.Object);

            await requestLogger.Process(new CreatePasswordCommand(
                new CreatePasswordItemDto
                {
                    Password = nameof(CreatePasswordItemDto.Password),
                    Title = nameof(CreatePasswordItemDto.Title),
                    Description = nameof(CreatePasswordItemDto.Description),
                }
            ), new CancellationToken());


            _logger.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}
