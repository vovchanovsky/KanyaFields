using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationSvc.Application.Infrastructure;
using AuthenticationSvc.Application.Resources;
using AuthenticationSvc.Domain.Models.Extensions;
using AuthenticationSvc.InfrastructureInterfaces.Clients.ClaimsProvider;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationSvc.Application.User.Commands.CreateJwtToken
{
    public class CreateJwtTokenCommand : IRequest<string>
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Guid OperationId { get; set; }
    }

    public class CreateJwtTokenCommandHandler : IRequestHandler<CreateJwtTokenCommand, string>
    {
        public CreateJwtTokenCommandHandler(
            IUserClaimsProvider claimsProvider, 
            IConfiguration configuration)
        {
            _claimsProvider = claimsProvider;
            _configuration = configuration;
        }


        public async Task<string> Handle(CreateJwtTokenCommand request, CancellationToken cancellationToken)
        {
            // TODO: read the standard, these must be DNS addresses
            var issuer = _configuration[EnvironmentVariables.JwtTokenIssuer];
            var audience = _configuration[EnvironmentVariables.JwtTokenAudience];

            var publicKey = _configuration[EnvironmentVariables.JwtTokenIdentityProviderPublicKey];
            var publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);

            // TODO: Use asymmetric encryption or certificate
            var key = new SymmetricSecurityKey(publicKeyBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(key, algorithm);

            var user = await _claimsProvider.GetUser(request.Username);

            if (user.Password != request.Password)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    ErrorMessages.WrongUsernameOrPassword,
                    request.Username,
                    request.OperationId);

                throw new ApplicationException(errorMessage);
            }

            var token = new JwtSecurityToken(
                issuer,
                audience,
                user.GetClaims(),
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private readonly IUserClaimsProvider _claimsProvider;
        private readonly IConfiguration _configuration;
    }
}