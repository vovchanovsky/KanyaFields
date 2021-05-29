using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordProviderSvc.Domain.Entities;

namespace PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider
{
    public interface IPasswordStorageProvider
    {
        Task<IEnumerable<PasswordItem>> GetPasswords(Guid userId);

        Task<PasswordItem> GetPasswordItem(Guid userId, Guid passwordId);

        Task DeletePassword(Guid userId, Guid passwordId);

        Task<Guid> InsertPassword(Guid userId, PasswordItem passwordItem);
    }
}