using StackExchange.Redis;

namespace JackBallers.Api.Models;

public class Artist : RedisModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public ArtistDescription? Description { get; init; }

    public static RedisKey FormatKey(Guid id)
    {
        return $"artist:{id}";
    }

    public override RedisKey FormatKey()
    {
        return FormatKey(Id);
    }
}