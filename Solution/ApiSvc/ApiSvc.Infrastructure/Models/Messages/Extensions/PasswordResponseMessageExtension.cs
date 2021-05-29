using System;
using ApiSvc.Infrastructure.Resources;
using Newtonsoft.Json;

namespace ApiSvc.Infrastructure.Models.Messages.Extensions
{
    public static class PasswordResponseMessageExtension
    {
        public static T GetResponseObject<T>(this PasswordResponseMessage responseMessage) =>
            JsonConvert.DeserializeObject<T>(responseMessage.SerializedResponseObject)
            ?? throw new InvalidOperationException(string.Format(
                ErrorMessages.SerializedRequestObjectIsEmpty,
                responseMessage.OperationId,
                responseMessage.SerializedResponseObject));
    }
}