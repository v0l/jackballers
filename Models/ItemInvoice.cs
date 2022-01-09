using JackBallers.Api.Strike;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JackBallers.Api.Models;

public class ItemInvoice : RedisModel
{
    public ItemInvoice(Guid id, Guid item)
    {
        Id = id;
        ItemId = item;
    }
    
    [JsonProperty("name")]
    public Guid Id { get; init; }
    
    [JsonProperty("itemId")]
    public Guid ItemId { get; init; }
    
    [JsonProperty("invoice")]
    public Invoice? Invoice { get; set; }
    
    [JsonProperty("quote")]
    public InvoiceQuote? Quote { get; init; }

    public static RedisKey FormatKey(Guid id) => $"invoice:{id}";
    
    public override RedisKey FormatKey() => FormatKey(Id);
}