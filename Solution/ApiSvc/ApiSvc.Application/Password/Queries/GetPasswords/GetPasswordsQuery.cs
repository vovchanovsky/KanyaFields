using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiSvc.Application.Password.Queries.GetPassword;
using ApiSvc.Domain.Entities;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient;
using AutoMapper;
using MediatR;

namespace ApiSvc.Application.Password.Queries.GetPasswords
{
    public class GetPasswordsQuery : IRequest<IEnumerable<GetPasswordItemDto>>
    { }

    public class GetPasswordsQueryHandler : IRequestHandler<GetPasswordsQuery, IEnumerable<GetPasswordItemDto>>
    {
        public GetPasswordsQueryHandler(IPasswordProviderClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetPasswordItemDto>> Handle(GetPasswordsQuery request, CancellationToken cancellationToken)
        {
            var passwords = await _client.GetPasswords();
            return passwords.Select(item => _mapper.Map<PasswordItem, GetPasswordItemDto>(item)).ToList();
        }

        private readonly IPasswordProviderClient _client;
        private readonly IMapper _mapper;
    }
}
