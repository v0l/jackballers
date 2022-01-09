using JackBallers.Api.Models;
using StackExchange.Redis;

namespace JackBallers.Api;

public class BallerFactory : BackgroundService
{
    private readonly IDatabase _database;
    private readonly ILogger<BallerFactory> _logger;

    public BallerFactory(IDatabase database, ILogger<BallerFactory> logger)
    {
        _database = database;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int targetAmount = 5;
        
        var ballerTemplates = new[]
        {
            new CollectionTemplate()
            {
                Id = new Guid("251F83CA-59DD-4FBC-AAFB-D410869B7811"),
                Name = "Laser Baller",
                Description = "Laser baller. Dying on hills.",
                Image = new Uri("https://pbs.twimg.com/profile_images/1402068570801119235/cDDDb7SP_400x400.jpg")
            },
            new CollectionTemplate()
            {
                Id = new Guid("0DFB74EC-BC09-4376-AC28-AC9AA09324CC"),
                Name = "Young Baller",
                Description = "Before the lust for destroying WesternUnion consumed him.",
                Image = new Uri("https://s3.amazonaws.com/37assets/svn/1561-jackmallers.jpg")
            },
            new CollectionTemplate()
            {
                Id = new Guid("C46D2EFD-00C4-4C10-88F4-090149768263"),
                Name = "Indy500 Baller",
                Description = "Intermediate baller state. Growing stronger everyday.",
                Image = new Uri("https://pbs.twimg.com/media/E2qCMvnWQAE_5D0.jpg")
            }
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var template in ballerTemplates)
            {
                var collection = template.MakeCollection();
                if (!await _database.KeyExistsAsync(collection.FormatKey()))
                {
                    await collection.Save(_database);
                    await _database.SetAddAsync(Collection.AllCollections, collection.Id.ToString());
                    _logger.LogInformation("Created new collection '{name}'", collection.Name);
                }

                var itemsKey = Collection.ItemsKey(collection.Id);
                var current = await _database.SetMembersAsync(itemsKey);
                if (current.Length < targetAmount)
                {
                    //spawn more ballers
                    var toSpawn = targetAmount - current.Length;
                    for (var x = 0; x < toSpawn; x++)
                    {
                        var counter = await _database.StringIncrementAsync(template.CounterKey());
                        var nextItem = template.MakeItem((ulong) counter);
                        await nextItem.Save(_database);
                        await _database.SetAddAsync(itemsKey, nextItem.Id.ToString());
                        
                        _logger.LogInformation("Added item #{item} to '{name}'", counter, collection.Name);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}