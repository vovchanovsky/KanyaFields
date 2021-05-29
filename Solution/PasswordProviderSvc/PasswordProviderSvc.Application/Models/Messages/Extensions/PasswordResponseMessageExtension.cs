using Newtonsoft.Json;

namespace PasswordProviderSvc.Application.Models.Messages.Extensions
{
    public static class PasswordResponseMessageExtension
    {
        public static void SetResponseObject<T>(this PasswordResponseMessage responseMessage, T responseObject)
        {
            var serializedRequestObject = JsonConvert.SerializeObject(responseObject);
            responseMessage.SerializedResponseObject = serializedRequestObject;
        }
    }
}