using StackExchange.Redis;

namespace JackBallers.Api.Models;

public abstract class RedisModel
{
    public virtual Task Save(IDatabase db)
    {
        return db.SetJson(FormatKey(), this);
    }

    public abstract RedisKey FormatKey();
}