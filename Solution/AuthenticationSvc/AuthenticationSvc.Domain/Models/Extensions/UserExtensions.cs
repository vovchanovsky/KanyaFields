using System.Security.Claims;
using IdentityServer4.Stores.Serialization;
using Newtonsoft.Json;

namespace AuthenticationSvc.Domain.Models.Extensions
{
    public static class UserExtensions
    {
        public static Claim[] GetClaims(this User user) => 
            JsonConvert.DeserializeObject<Claim[]>(user.Claims, new ClaimConverter());

        public static void SetClaims(this User user, Claim[] claims)
        {
            user.Claims = JsonConvert.SerializeObject(claims, new ClaimConverter());
        }
    }
}