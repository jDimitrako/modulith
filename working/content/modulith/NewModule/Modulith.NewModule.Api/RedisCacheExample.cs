using Microsoft.Extensions.Caching.Distributed;

namespace Modulith.NewModule.Api;

public class RedisCacheExampleService
{
    private readonly IDistributedCache _cache;
    public RedisCacheExampleService(IDistributedCache cache) => _cache = cache;

    public async Task SetValueAsync(string key, string value)
    {
        await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
    }

    public async Task<string?> GetValueAsync(string key)
    {
        return await _cache.GetStringAsync(key);
    }
} 