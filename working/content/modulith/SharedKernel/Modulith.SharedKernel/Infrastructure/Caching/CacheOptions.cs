namespace Modulith.SharedKernel.Infrastructure.Caching;

public class CacheOptions
{
    public TimeSpan? DefaultExpiration { get; set; }
    public bool UseSlidingExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public bool UseAbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpiration { get; set; }
    public bool UseMemoryCache { get; set; }
    public bool UseDistributedCache { get; set; }
    public string? InstanceName { get; set; }
}

public static class CacheOptionsExtensions
{
    public static DistributedCacheEntryOptions ToDistributedCacheEntryOptions(this CacheOptions options)
    {
        var cacheOptions = new DistributedCacheEntryOptions();

        if (options.UseSlidingExpiration && options.SlidingExpiration.HasValue)
        {
            cacheOptions.SlidingExpiration = options.SlidingExpiration;
        }

        if (options.UseAbsoluteExpiration && options.AbsoluteExpiration.HasValue)
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration;
        }

        return cacheOptions;
    }
} 