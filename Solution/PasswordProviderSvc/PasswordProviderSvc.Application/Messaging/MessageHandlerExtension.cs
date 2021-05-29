using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PasswordProviderSvc.Application.Models.Enums;
using PasswordProviderSvc.Application.Models.Messages;
using PasswordProviderSvc.Application.Models.Messages.Extensions;
using PasswordProviderSvc.Application.Password.Commands.CreatePassword;
using PasswordProviderSvc.Application.Password.Commands.DeletePassword;
using PasswordProviderSvc.Application.Password.Queries.GetPassword;
using PasswordProviderSvc.Application.Password.Queries.GetPasswords;

namespace PasswordProviderSvc.Application.Messaging
{
    public static class MessageHandlerExtension
    {
        public static Func<IMediator, PasswordRequestMessage, Guid, Task<PasswordResponseMessage>> GetCommandHandler(PasswordCommands command) =>
            CommandHandlers[command];


        #region CommandHandlers

        private static async Task<PasswordResponseMessage> GetAllPasswordsCommandHandler(IMediator mediator, PasswordRequestMessage requestMessage, Guid userId)
        {
            var requestObject = new GetPasswordsQuery(userId);
            var result = await mediator.Send(requestObject);

            var responseMessage = requestMessage.ToResponseMessage(isSuccess: true);
            responseMessage.SetResponseObject(result.ToList());

            return responseMessage;
        }

        private static async Task<PasswordResponseMessage> GetPasswordItemCommandHandler(IMediator mediator, PasswordRequestMessage requestMessage, Guid userId)
        {
            var requestObject = requestMessage.GetRequestObject<GetPasswordQuery>();
            requestObject.UserId = userId;
            var result = await mediator.Send(requestObject);

            var responseMessage = requestMessage.ToResponseMessage(isSuccess: true);
            responseMessage.SetResponseObject(new[] { result }.ToList());

            return responseMessage;
        }

        private static async Task<PasswordResponseMessage> DeletePasswordCommandHandler(IMediator mediator, PasswordRequestMessage requestMessage, Guid userId)
        {
            var requestObject = requestMessage.GetRequestObject<DeletePasswordCommand>();
            requestObject.UserId = userId;
            await mediator.Send(requestObject);

            return requestMessage.ToResponseMessage(true);
        }

        // Well... looks weird, I can generate Guid even on the ApiSvc side
        // and don't send the response at all
        // BUT I do need to be sure that it was inserted successfully
        // SO
        // TODO: refactor InsertPasswordCommandHandler
        private static async Task<PasswordResponseMessage> InsertPasswordCommandHandler(IMediator mediator, PasswordRequestMessage requestMessage, Guid userId)
        {
            var requestObject = requestMessage.GetRequestObject<CreatePasswordCommand>();
            requestObject.UserId = userId;
            requestObject.PasswordItem.PasswordId = Guid.NewGuid();
            requestObject.PasswordItem.UserId = userId;

            await mediator.Send(requestObject);

            var responseMessage = requestMessage.ToResponseMessage(isSuccess: true);
            responseMessage.SetResponseObject(new[] { requestObject.PasswordItem }.ToList());

            return responseMessage;
        }

        #endregion


        private static readonly Dictionary<PasswordCommands, Func<IMediator, PasswordRequestMessage, Guid, Task<PasswordResponseMessage>>> CommandHandlers =
            new()
            {
                { PasswordCommands.GetAllPasswords, GetAllPasswordsCommandHandler},
                { PasswordCommands.GetPasswordItem, GetPasswordItemCommandHandler},
                { PasswordCommands.DeletePassword, DeletePasswordCommandHandler},
                { PasswordCommands.InsertPassword, InsertPasswordCommandHandler},
            };
    }
}