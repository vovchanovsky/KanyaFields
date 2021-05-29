using System;
using System.Threading;
using System.Threading.Tasks;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient;
using MediatR;

namespace ApiSvc.Application.Password.Commands.DeletePassword
{
    public class DeletePasswordCommand : IRequest
    {
        public DeletePasswordCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class DeletePasswordCommandHandler : IRequestHandler<DeletePasswordCommand>
    {
        public DeletePasswordCommandHandler(IPasswordProviderClient client)
        {
            _client = client;
        }

        public async Task<Unit> Handle(DeletePasswordCommand request, CancellationToken cancellationToken)
        {
            await _client.DeletePassword(request.Id);
            return Unit.Value;
        }

        private readonly IPasswordProviderClient _client;
    }
}
