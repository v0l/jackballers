using System.Text.Json.Serialization;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JackBallers.Api;

public static class RedisExtensions
{
    public static async Task<T?> GetJson<T>(this IDatabase db, RedisKey k) where T : class
    {
        var v = await db.StringGetAsync(k);
        return v.HasValue ? JsonConvert.DeserializeObject<T>(v) : default;
    }

    public static Task<bool> SetJson<T>(this IDatabase db, RedisKey k, T v) where T : class
    {
        //var json = JsonSerializer.Serialize(v);
        var json = JsonConvert.SerializeObject(v);
        return db.StringSetAsync(k, json);
    }
}