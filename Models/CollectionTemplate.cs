using StackExchange.Redis;

namespace JackBallers.Api.Models;

public class CollectionTemplate
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; set; }
    public Uri? Image { get; init; }

    public Collection MakeCollection()
    {
        return new()
        {
            Id = Id, 
            Name = Name, 
            Description = Description
        };
    }

    public CollectionItem MakeItem(ulong rank)
    {
        return new CollectionItem()
        {
            Name = Name,
            Description = Description,
            Image = Image,
            Number = rank,
            FiatPrice = 0.5m + (3.0m * (decimal)new Random().NextDouble()),
            CollectionId = Id
        };
    }

    public RedisKey CounterKey() => $"collection-counter:{Id}";
}