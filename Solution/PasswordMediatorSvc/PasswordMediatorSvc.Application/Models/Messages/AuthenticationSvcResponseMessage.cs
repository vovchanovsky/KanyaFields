using System;

namespace PasswordMediatorSvc.Application.Models.Messages
{
    public class AuthenticationSvcResponseMessage
    {
        public Guid OperationId { get; set; }

        public string CorrelationId { get; set; }

        public Guid ApplicationInstanceId { get; set; }

        // UserId of the created user
        //public Guid UserId { get; set; }

        public bool Success { get; set; }

        public string JwtToken { get; set; }
    }
}