using JackBallers.Api.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace JackBallers.Api.Controllers;

[Route("api/collection")]
public class CollectionController : ApiBaseController
{
    private readonly IDatabase _database;

    public CollectionController(IDatabase database)
    {
        _database = database;
    }

    [HttpGet]
    public async IAsyncEnumerable<Collection> GetCollections()
    {
        var collections = await _database.SetMembersAsync(Collection.AllCollections);
        foreach (var cid in collections)
        {
            var gid = Guid.Parse(cid.ToString());
            var cv = await _database.GetJson<Collection>(Collection.FormatKey(gid));
            if (cv != default) yield return cv;
        }
    }

    [HttpGet]
    [Route("{collection:guid}")]
    public async IAsyncEnumerable<CollectionItem> GetItems([FromRoute] Guid collection)
    {
        var collectionItems = await _database.SetMembersAsync(Collection.ItemsKey(collection));
        foreach (var itemId in collectionItems)
        {
            var id = Guid.Parse(itemId.ToString());
            var item = await _database.GetJson<CollectionItem>(CollectionItem.FormatKey(id));
            if (item != default) yield return item;
        }
    }
}