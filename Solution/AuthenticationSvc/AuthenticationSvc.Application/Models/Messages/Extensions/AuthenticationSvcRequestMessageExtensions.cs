namespace AuthenticationSvc.Application.Models.Messages.Extensions
{
    public static class AuthenticationSvcRequestMessageExtensions
    {
        public static AuthenticationSvcResponseMessage ToResponseMessage(
            this AuthenticationSvcRequestMessage requestMessage, bool isSuccess) =>
            new()
            {
                ApplicationInstanceId = requestMessage.ApplicationInstanceId,
                OperationId = requestMessage.OperationId,
                CorrelationId = requestMessage.CorrelationId,
                Success = isSuccess
            };
    }
}