using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiSvc.InfrastructureInterfaces.Clients.IdentityProviderClient;
using Microsoft.IdentityModel.Tokens;

namespace ApiSvc.Infrastructure.Clients.IdentityProviderClient
{
    public class FakeIdentityProviderClient : IIdentityProviderClient
    {
        public FakeIdentityProviderClient()
        {
            _userClaims = GetUserClaims();
        }

        public Task RegisterUser(string username, string password)
        {
            _userClaims.Add(
                (username, password),
                new Claim[]
                {
                    new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                    new(ClaimTypes.Role, "Admin")
                });

            return Task.CompletedTask;
        }

        public Task<string> Authenticate(string username, string password)
        {
            if (_userClaims.TryGetValue((username, password), out Claim[] claims) is false)
            {
                throw new Exception($"User does not exist: {username}");
            }

            var secret = "OMG_THIS_SECRET_IS_SOOOOO_LONG___WELL_NOT_SO_LONG_INDEED";
            var issuer = "KanyaFieldsIdentityProvider";
            var audience = "KanyaFieldsUsers";

            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(tokenJson);
        }


        private static Dictionary<(string username, string password), Claim[]> GetUserClaims() =>
            new()
            {
                {
                    ("jhon", "doe"),
                    new Claim[]
                    {
                        new(JwtRegisteredClaimNames.Sub, "f8f7d5e9-21df-4b01-82c7-23decd1a57a8"),
                        new(ClaimTypes.Role, "Admin")
                    }
                },
                {
                    ("marko", "Dead"),
                    new Claim[]
                    {
                        new(JwtRegisteredClaimNames.Sub, "c5ac1bda-521e-4995-944e-18abb5fb54af"),
                        new(ClaimTypes.Role, "Admin")
                    }
                }
            };


        private readonly Dictionary<(string username, string password), Claim[]> _userClaims;
    }
}