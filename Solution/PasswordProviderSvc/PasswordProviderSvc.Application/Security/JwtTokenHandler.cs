using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PasswordProviderSvc.Application.Infrastructure;

namespace PasswordProviderSvc.Application.Security
{
    public static class JwtTokenHandler
    {
        // TODO: make it service
        public static ClaimsPrincipal ValidateJwtToken(IConfiguration configuration, string token)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();
            var publicKey = configuration[EnvironmentVariables.JwtTokenIdentityProviderPublicKey];
            var issuer = configuration[EnvironmentVariables.JwtTokenIssuer];
            var audience = configuration[EnvironmentVariables.JwtTokenAudience];
            var publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);
            var key = new SymmetricSecurityKey(publicKeyBytes);

            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key
            }, out _);

            return claimsPrincipal;
        }
    }
}