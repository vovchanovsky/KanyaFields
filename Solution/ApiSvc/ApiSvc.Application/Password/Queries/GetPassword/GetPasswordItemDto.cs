using System;
using ApiSvc.Application.Common.Mappings;
using ApiSvc.Domain.Common;
using ApiSvc.Domain.Entities;

namespace ApiSvc.Application.Password.Queries.GetPassword
{
    public class GetPasswordItemDto : AuditableEntity, IMapFrom<PasswordItem>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Password { get; set; }
    }
}
