using System;

namespace ApiSvc.Infrastructure.Models.Messages
{
    public class PasswordResponseMessage
    {
        public Guid ApplicationInstanceId { get; set; }

        public Guid OperationId { get; set; }

        public string CorrelationId { get; set; }

        public bool Success { get; set; }

        public string SerializedResponseObject { get; set; }
    }
}