using System;
using System.Threading.Tasks;

namespace PasswordMediatorSvc.InfrastructureInterfaces.Services
{
    public interface IDistributedCacheProvider
    {
        Task SetRecordAsync<T>(
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null);

        Task<T> GetRecordAsync<T>(string recordId);
    }
}