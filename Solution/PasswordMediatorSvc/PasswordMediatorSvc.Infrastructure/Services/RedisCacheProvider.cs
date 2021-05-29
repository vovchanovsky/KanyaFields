using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using PasswordMediatorSvc.InfrastructureInterfaces.Services;

namespace PasswordMediatorSvc.Infrastructure.Services
{
    public class RedisCacheProvider : IDistributedCacheProvider
    {
        public RedisCacheProvider(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetRecordAsync<T>(
            string recordId, 
            T data, 
            TimeSpan? absoluteExpireTime = null, 
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                SlidingExpiration = unusedExpireTime
            };

            var jsonData = JsonSerializer.Serialize(data);
            await _cache.SetStringAsync(recordId, jsonData, options);
        }

        public async Task<T> GetRecordAsync<T>(string recordId)
        {
            var jsonData = await _cache.GetStringAsync(recordId);

            if (jsonData is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

        private readonly IDistributedCache _cache;
    }
}
