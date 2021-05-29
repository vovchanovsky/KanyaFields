using System;
using PasswordProviderSvc.Application.Models.Enums;

namespace PasswordProviderSvc.Application.Models.Messages
{
    public class PasswordRequestMessage
    {
        public string JwtToken { get; set; }

        public Guid OperationId { get; set; }

        public string CorrelationId { get; set; }

        public PasswordCommands Command { get; set; }

        public Guid ApplicationInstanceId { get; set; }

        public string SerializedRequestObject { get; set; }


        public override string ToString() =>
            $"OperationId: {OperationId}, CorrelationId: {CorrelationId}, Command: {Command}, SerializedRequestObject: {SerializedRequestObject}";
    }
}