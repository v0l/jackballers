using System.Text.Json;
using StackExchange.Redis;

namespace JackBallers.Api;

public static class RedisExtensions
{
    public static async Task<T?> GetJson<T>(this IDatabase db, RedisKey k)
    {
        var v = await db.StringGetAsync(k);
        return v.HasValue ? JsonSerializer.Deserialize<T>(v) : default;
    }

    public static Task<bool> SetJson<T>(this IDatabase db, RedisKey k, T v)
    {
        return db.StringSetAsync(k, JsonSerializer.Serialize(v));
    }
}