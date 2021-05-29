using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationSvc.Application.Resources;
using AuthenticationSvc.Domain.Models.Extensions;
using AuthenticationSvc.InfrastructureInterfaces.Clients.ClaimsProvider;
using MediatR;

namespace AuthenticationSvc.Application.User.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Guid OperationId { get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
    {
        public RegisterUserCommandHandler(IUserClaimsProvider claimsProvider)
        {
            _claimsProvider = claimsProvider;
        }

        public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _claimsProvider.UserIsAlreadyExist(request.Username))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    ErrorMessages.UserIsAlreadyExist,
                    request.Username,
                    request.OperationId);

                throw new ApplicationException(errorMessage);
            }

            var claims = new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, "Admin")
            };

            var user = new Domain.Models.User
            {
                Username = request.Username,
                Password = request.Password
            };

            user.SetClaims(claims);

            await _claimsProvider.CreateUser(user);

            return Unit.Value;
        }

        private readonly IUserClaimsProvider _claimsProvider;
    }
}