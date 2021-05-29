using System;
using System.Threading;
using System.Threading.Tasks;
using ApiSvc.Domain.Entities;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient;
using AutoMapper;
using MediatR;

namespace ApiSvc.Application.Password.Queries.GetPassword
{
    public class GetPasswordQuery : IRequest<GetPasswordItemDto>
    {
        public GetPasswordQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class GetPasswordQueryHandler : IRequestHandler<GetPasswordQuery, GetPasswordItemDto>
    {
        public GetPasswordQueryHandler(IPasswordProviderClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<GetPasswordItemDto> Handle(GetPasswordQuery request, CancellationToken cancellationToken)
        {
            var passwordItem = await _client.GetPasswordItem(request.Id);
            return _mapper.Map<PasswordItem, GetPasswordItemDto>(passwordItem);
        }

        private readonly IPasswordProviderClient _client;
        private readonly IMapper _mapper;
    }
}
