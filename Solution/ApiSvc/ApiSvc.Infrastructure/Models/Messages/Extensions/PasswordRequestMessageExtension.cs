using Newtonsoft.Json;

namespace ApiSvc.Infrastructure.Models.Messages.Extensions
{
    public static class PasswordRequestMessageExtension
    {
        public static void SetRequestObject<T>(this PasswordRequestMessage requestMessage, T requestObject)
        {
            var serializedRequestObject = JsonConvert.SerializeObject(requestObject);
            requestMessage.SerializedRequestObject = serializedRequestObject;
        }
    }
}