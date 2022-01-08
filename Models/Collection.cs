using StackExchange.Redis;

namespace JackBallers.Api.Models;

public class Collection : RedisModel
{
    public const string AllCollections = "collections";
    
    public Guid Id { get; init; } = Guid.NewGuid();

    public string? Name { get; init; }
    public string? Description { get; init; }
    
    public static RedisKey FormatKey(Guid id) => $"collection:{id}";
    public static RedisKey ItemsKey(Guid id) => $"collection-items:{id}";
    public override RedisKey FormatKey() => FormatKey(Id);
}