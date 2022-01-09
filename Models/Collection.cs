using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace JackBallers.Api.Models;

public sealed class Collection : RedisModel
{
    public const string AllCollections = "collections";
    
    [JsonInclude]
    [JsonPropertyName("id")]
    public Guid Id { get; init; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string? Name { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    public static RedisKey FormatKey(Guid id) => $"collection:{id}";
    public static RedisKey ItemsKey(Guid id) => $"collection-items:{id}";
    public override RedisKey FormatKey() => FormatKey(Id);
}