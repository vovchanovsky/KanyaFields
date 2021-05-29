using System;
using System.Threading;
using System.Threading.Tasks;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient;
using MediatR;
using Utilities.DateTimeService;

namespace ApiSvc.Application.Password.Commands.CreatePassword
{
    public class CreatePasswordCommand : IRequest<Guid>
    {
        public CreatePasswordCommand(CreatePasswordItemDto createPasswordItemDto)
        {
            CreatePasswordItemDto = createPasswordItemDto;
        }

        public CreatePasswordItemDto CreatePasswordItemDto { get; set; }
    }

    public class CreatePasswordCommandHandler : IRequestHandler<CreatePasswordCommand, Guid>
    {
        public CreatePasswordCommandHandler(IPasswordProviderClient client, IDateTime dateTime)
        {
            _client = client;
            _dateTime = dateTime;
        }

        public Task<Guid> Handle(CreatePasswordCommand request, CancellationToken cancellationToken)
        {
            var passwordItem = request.CreatePasswordItemDto.ToPasswordItem();

            // TODO: get user (or maybe user id) from the auth context
            passwordItem.CreatedBy = "Someone cool";
            passwordItem.Created = _dateTime.Now;

            return _client.InsertPassword(passwordItem);
        }

        private readonly IPasswordProviderClient _client;
        private readonly IDateTime _dateTime;
    }
}
