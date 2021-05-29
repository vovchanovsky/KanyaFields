using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordProviderSvc.Domain.Entities;
using PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider;
using Utilities.DateTimeService;

namespace PasswordProviderSvc.Application.Password.Commands.CreatePassword
{
    public class CreatePasswordCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }

        public PasswordItem PasswordItem { get; set; }
    }

    public class CreatePasswordCommandHandler : IRequestHandler<CreatePasswordCommand, Guid>
    {
        public CreatePasswordCommandHandler(IPasswordStorageProvider storageProvider, IDateTime dateTime)
        {
            _storageProvider = storageProvider;
            _dateTime = dateTime;
        }


        public Task<Guid> Handle(CreatePasswordCommand request, CancellationToken cancellationToken)
        {
            request.PasswordItem.Created = _dateTime.Now;

            return _storageProvider.InsertPassword(request.UserId, request.PasswordItem);
        }


        private readonly IPasswordStorageProvider _storageProvider;
        private readonly IDateTime _dateTime;
    }
}
