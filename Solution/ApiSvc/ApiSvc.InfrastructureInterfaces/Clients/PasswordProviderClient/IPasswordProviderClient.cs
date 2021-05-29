using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiSvc.Domain.Entities;

namespace ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient
{
    public interface IPasswordProviderClient
    {
        Task<IEnumerable<PasswordItem>> GetPasswords();

        Task<PasswordItem> GetPasswordItem(Guid id);

        Task DeletePassword(Guid id);

        Task<Guid> InsertPassword(PasswordItem passwordItem);
    }
}
