using System;
using AuthenticationSvc.Application.Models.Enums;

namespace AuthenticationSvc.Application.Models.Messages
{
    public class AuthenticationSvcRequestMessage
    {
        public Guid OperationId { get; set; }

        public string CorrelationId { get; set; }

        public AuthenticationSvcCommands Command { get; set; }

        public Guid ApplicationInstanceId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}