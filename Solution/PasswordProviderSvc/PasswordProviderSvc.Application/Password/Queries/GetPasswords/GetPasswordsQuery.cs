using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordProviderSvc.Domain.Entities;
using PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider;

namespace PasswordProviderSvc.Application.Password.Queries.GetPasswords
{
    public class GetPasswordsQuery : IRequest<IEnumerable<PasswordItem>>
    {
        public GetPasswordsQuery()
        { }

        public GetPasswordsQuery(Guid userId)
        {
            UserId = userId;
        }


        public Guid UserId { get; set; }
    }

    public class GetPasswordsQueryHandler : IRequestHandler<GetPasswordsQuery, IEnumerable<PasswordItem>>
    {
        public GetPasswordsQueryHandler(IPasswordStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public Task<IEnumerable<PasswordItem>> Handle(GetPasswordsQuery request, CancellationToken cancellationToken)
        {
            return _storageProvider.GetPasswords(request.UserId);
        }

        private readonly IPasswordStorageProvider _storageProvider;
    }
}
