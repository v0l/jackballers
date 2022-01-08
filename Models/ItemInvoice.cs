using JackBallers.Api.Strike;
using StackExchange.Redis;

namespace JackBallers.Api.Models;

public class ItemInvoice : RedisModel
{
    public ItemInvoice(Guid id, Guid item)
    {
        Id = id;
        ItemId = item;
    }
    
    public Guid Id { get; init; }
    
    public Guid ItemId { get; init; }
    
    public Invoice? Invoice { get; init; }
    public InvoiceQuote Quote { get; init; }

    public static RedisKey FormatKey(Guid id)
    {
        return $"invoice:{id}";
    }

    public override RedisKey FormatKey()
    {
        return FormatKey(Id);
    }
}