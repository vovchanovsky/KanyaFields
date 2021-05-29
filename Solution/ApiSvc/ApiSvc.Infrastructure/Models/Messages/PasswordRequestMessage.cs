using System;
using ApiSvc.Infrastructure.Models.Enums;

namespace ApiSvc.Infrastructure.Models.Messages
{
    public class PasswordRequestMessage
    {
        public string JwtToken { get; set; }

        public Guid OperationId { get; set; }

        public string CorrelationId { get; set; }

        public PasswordCommands Command { get; set; }

        public Guid ApplicationInstanceId { get; set; }

        public string SerializedRequestObject { get; set; }
    }
}