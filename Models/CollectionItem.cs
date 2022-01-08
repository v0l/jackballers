using StackExchange.Redis;

namespace JackBallers.Api.Models;

public class CollectionItem : RedisModel
{
    public Guid Id { get; init; }

    public string? Name { get; init; }
    public string? Description { get; init; }

    public Uri? Image { get; init; }

    public uint Rank { get; init; } = 0;

    public override RedisKey FormatKey() => FormatKey(Id);
    
    public static RedisKey FormatKey(Guid id) => $"item:{id}";

    public decimal FiatPrice => Rank < 10 ? 5.0m : 1.0m;
}