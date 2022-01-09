using System.Text.Json.Serialization;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JackBallers.Api.Models;

public class CollectionItem : RedisModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; } = Guid.NewGuid();

    [JsonProperty("collectionId")]
    public Guid CollectionId { get; init; }
    
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("image")]
    public Uri? Image { get; init; }

    [JsonPropertyName("number")]
    public ulong Number { get; init; } = 0;
    
    [JsonPropertyName("fiatPrice")]
    public decimal FiatPrice { get; init; } = 1.0m;

    public override RedisKey FormatKey() => FormatKey(Id);
    
    public static RedisKey FormatKey(Guid id) => $"item:{id}";

}