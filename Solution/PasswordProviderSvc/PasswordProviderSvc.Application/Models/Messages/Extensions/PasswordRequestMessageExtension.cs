using System;
using Newtonsoft.Json;
using PasswordProviderSvc.Application.Resources;

namespace PasswordProviderSvc.Application.Models.Messages.Extensions
{
    public static class PasswordRequestMessageExtension
    {
        public static T GetRequestObject<T>(this PasswordRequestMessage requestMessage) =>
            JsonConvert.DeserializeObject<T>(requestMessage.SerializedRequestObject) 
                ?? throw new InvalidOperationException(string.Format(
                    ErrorMessages.SerializedRequestObjectIsEmpty,
                    requestMessage.OperationId,
                    requestMessage.SerializedRequestObject));

        public static PasswordResponseMessage ToResponseMessage(this PasswordRequestMessage requestMessage, bool isSuccess) =>
            new()
            {
                ApplicationInstanceId = requestMessage.ApplicationInstanceId,
                OperationId = requestMessage.OperationId,
                CorrelationId = requestMessage.CorrelationId,
                Success = isSuccess
            };
    }
}