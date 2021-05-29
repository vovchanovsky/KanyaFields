using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider;

namespace PasswordProviderSvc.Application.Password.Commands.DeletePassword
{
    public class DeletePasswordCommand : IRequest
    {
        public Guid UserId { get; set; }

        public Guid PasswordId { get; set; }
    }

    public class DeletePasswordCommandHandler : IRequestHandler<DeletePasswordCommand>
    {
        public DeletePasswordCommandHandler(IPasswordStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public async Task<Unit> Handle(DeletePasswordCommand request, CancellationToken cancellationToken)
        {
            await _storageProvider.DeletePassword(request.UserId, request.PasswordId);
            return Unit.Value;
        }

        private readonly IPasswordStorageProvider _storageProvider;
    }
}
