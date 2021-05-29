using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordProviderSvc.Domain.Entities;
using PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider;

namespace PasswordProviderSvc.Application.Password.Queries.GetPassword
{
    public class GetPasswordQuery : IRequest<PasswordItem>
    {
        public Guid UserId { get; set; }

        public Guid PasswordId { get; set; }
    }

    public class GetPasswordQueryHandler : IRequestHandler<GetPasswordQuery, PasswordItem>
    {
        public GetPasswordQueryHandler(IPasswordStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public Task<PasswordItem> Handle(GetPasswordQuery request, CancellationToken cancellationToken)
        {
            return _storageProvider.GetPasswordItem(request.UserId, request.PasswordId);
        }

        private readonly IPasswordStorageProvider _storageProvider;
    }
}
