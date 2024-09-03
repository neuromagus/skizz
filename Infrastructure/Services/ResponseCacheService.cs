using System.Text.Json;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class ResponseCacheService(IConnectionMultiplexer redis) : IResponseCacheService
{
    // why "1"? because db0 for product...
    private readonly IDatabase _database = redis.GetDatabase(1);
    private readonly static JsonSerializerOptions s_options =  new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    // why object? For maximum flexebility
    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        
        var serializedResponse = JsonSerializer.Serialize(response, s_options);

        await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
    }

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await _database.StringGetAsync(cacheKey);
        if (cachedResponse.IsNullOrEmpty) return null;

        return cachedResponse;
    }

    public async Task RemoveCacheByPatternAsync(string pattern)
    {
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(database: 1, pattern: $"*{pattern}*").ToArray();

        if (keys.Length != 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }
}
