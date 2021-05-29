using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationSvc.Application.Models.Enums;
using AuthenticationSvc.Application.Models.Messages;
using AuthenticationSvc.Application.Models.Messages.Extensions;
using AuthenticationSvc.Application.User.Commands.CreateJwtToken;
using AuthenticationSvc.Application.User.Commands.RegisterUser;
using MediatR;

namespace AuthenticationSvc.Application.Messaging
{
    public static class MessageHandlerExtension
    {
        public static Func<IMediator, AuthenticationSvcRequestMessage, Task<AuthenticationSvcResponseMessage>> GetCommandHandler(AuthenticationSvcCommands command) =>
            CommandHandlers[command];


        #region CommandHandlers

        private static async Task<AuthenticationSvcResponseMessage> RegisterUserCommandHandler(
            IMediator mediator, AuthenticationSvcRequestMessage requestMessage)
        {
            var reqRegisterUserCommand = new RegisterUserCommand
            {
                OperationId = requestMessage.OperationId,
                Username = requestMessage.Username,
                Password = requestMessage.Password
            };

            await mediator.Send(reqRegisterUserCommand);
            var responseMessage = requestMessage.ToResponseMessage(isSuccess: true);

            return responseMessage;
        }

        private static async Task<AuthenticationSvcResponseMessage> CreateJwtTokenCommandHandler(
            IMediator mediator, AuthenticationSvcRequestMessage requestMessage)
        {
            var createJwtTokenCommand = new CreateJwtTokenCommand
            {
                OperationId = requestMessage.OperationId,
                Username = requestMessage.Username,
                Password = requestMessage.Password
            };
            
            var result = await mediator.Send(createJwtTokenCommand);

            var responseMessage = requestMessage.ToResponseMessage(isSuccess: true);
            responseMessage.JwtToken = result;

            return responseMessage;
        }

        #endregion


        private static readonly Dictionary<AuthenticationSvcCommands, Func<IMediator, AuthenticationSvcRequestMessage, Task<AuthenticationSvcResponseMessage>>> CommandHandlers =
            new()
            {
                { AuthenticationSvcCommands.RegisterUser, RegisterUserCommandHandler },
                { AuthenticationSvcCommands.CreateJwtToken, CreateJwtTokenCommandHandler }
            };
    }
}